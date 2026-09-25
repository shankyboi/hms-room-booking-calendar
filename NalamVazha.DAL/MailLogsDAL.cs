namespace NalamVazha.DAL{
			    using System;
			    using System.Text;
			    using System.Data;
			    using System.Data.Common;
			    using NalamVazha.Models;
			    using EncrypDecrypt;
			    using Newtonsoft.Json;
				using Newtonsoft.Json.Linq;
                using Npgsql;
				using NpgsqlTypes;
				using System.Text.RegularExpressions;

			    //This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:40:28
			    public class MailLogsDAL
			    {
					public virtual string db_connectionstring{get;set;}
					public virtual string str_approvalurl { get; set; }
			 	    public MailLogsDAL(string connectionString,string approvalurl)
				    {
						 str_approvalurl = approvalurl;
					    db_connectionstring=connectionString;
				    }
				  
			        public virtual System.Data.DataTable MailSender(string mailfor,string entityid,string createduser){
										    DataTable dataTable=new DataTable();
											DataSet dataSet=new DataSet();
											try
											{
							  
                            
													 using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
													{
														npsql.Open();
														using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"MailSender\"(@pvar_mailfor,@pvar_entityid,@pvar_createduser)", npsql))
														{
															dbCommand.CommandType = CommandType.Text;
															dbCommand.Parameters.AddWithValue("pvar_mailfor",(object)mailfor??DBNull.Value);
															dbCommand.Parameters.AddWithValue("pvar_entityid",(object)entityid??DBNull.Value);
															dbCommand.Parameters.AddWithValue("pvar_createduser",(object)createduser??DBNull.Value);
															using (NpgsqlDataAdapter dataAdapter = new NpgsqlDataAdapter(dbCommand))
															{
																dataSet.Reset();
																dataAdapter.Fill(dataSet);
																dataTable = dataSet.Tables[0];
																if (dbCommand.Connection.State != ConnectionState.Closed)
																{
																	dbCommand.Connection.Dispose();
																}
															}
														}
														npsql.Close();
													}

						
											}
											catch{
												throw;
											}
											return dataTable;	
										}
										 public virtual mailmodel Mailer(string entityname, string entityactionname, string entityid,string tenantid="")
										{

											string alertcontent = "";
											string alertsubject = "";
											string actionmethodname = "";
											mailmodel objmailmodel = new mailmodel();
											DataTable dataTable = new DataTable();
											DataSet dataSet = new DataSet();
											try
											{
												using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
												{
													npsql.Open();
													using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"Alert_Templates_List\"(@pvar_tenantid,@pvar_entityname,@pvar_entityaction,@pvar_alerttype)", npsql))
													{
														dbCommand.CommandType = CommandType.Text;
						dbCommand.Parameters.AddWithValue("pvar_tenantid", (object)tenantid ?? DBNull.Value);
						dbCommand.Parameters.AddWithValue("pvar_entityname", (object)entityname ?? DBNull.Value);
														dbCommand.Parameters.AddWithValue("pvar_entityaction", (object)entityactionname ?? DBNull.Value);
														dbCommand.Parameters.AddWithValue("pvar_alerttype", "Email");
						 

						using (var reader = dbCommand.ExecuteReader())
														{
															if (reader.Read())
															{
																// Retrieve the values from the reader
																alertsubject = reader["alertsubject"].ToString(); 
																alertcontent = reader["alertcontent"].ToString(); 
																actionmethodname = reader["actionmethodname"].ToString();
															}
															else
															{
																// Handle the case where no data is returned from the stored procedure
															}
														}
													}
														npsql.Close();
													}

													// Keep the dashboard reminder usable even when a tenant has not
													// created a custom PaymentReminder alert template yet.
											if (string.Equals(entityactionname, "PaymentReminder", StringComparison.OrdinalIgnoreCase))
											{
												if (string.IsNullOrWhiteSpace(alertsubject))
													alertsubject = "Payment reminder for IPD booking {bookingreferencenumber}";

												if (string.IsNullOrWhiteSpace(alertcontent))
													alertcontent = "<p>Dear Health Seeker,</p>"
													+ "<p>This is a reminder that a payment is pending for your IPD booking "
													+ "<strong>{bookingreferencenumber}</strong>.</p>"
													+ "<p>~paymentlink~</p>"
													+ "<p>If you have already completed the payment, please ignore this message.</p>"
													+ "<p>From {tenantname}</p>";
											}

													// Define a regular expression pattern to match text inside curly braces
												string pattern = @"\{([^}]*)\}";

												// Use Regex.Matches to find all matches in the input string
												MatchCollection matches = Regex.Matches(alertcontent, pattern);

												// Create an array to store the matched values
												string[] extractedValues = new string[matches.Count];

												// Extract and store the matched values in the array
												for (int i = 0; i < matches.Count; i++)
												{
													extractedValues[i] = matches[i].Groups[1].Value;
												}




												using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
												{
													npsql.Open();
													using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"getById_sp_all_"+entityname+"\"(@pvar_"+entityname.ToLower()+"id)", npsql))
													{
														dbCommand.CommandType = CommandType.Text;
														dbCommand.Parameters.AddWithValue("pvar_"+entityname.ToLower()+"id", (object)entityid ?? DBNull.Value);
														using (NpgsqlDataAdapter dataAdapter = new NpgsqlDataAdapter(dbCommand))
														{
															dataSet.Reset();
															dataAdapter.Fill(dataSet);
															dataTable = dataSet.Tables[0];


															if (dbCommand.Connection.State != ConnectionState.Closed)
															{
																dbCommand.Connection.Dispose();
															}
														}
													}
													npsql.Close();
												}

												if (entityactionname == "ReadyForReview")
												{
													alertcontent = alertcontent.Replace("~clickhere~","<a href='"+ str_approvalurl + "/" + entityname + "/"+ actionmethodname + "?id=" + entityid +"'>Click Here</a>");
												}
												foreach (string value in extractedValues)
													{
					string replacement = "";

					// Tenant branding is resolved by MailSender after this API call.
					// Do not erase the token merely because it is not a column in
					// getById_sp_all_<entity>; otherwise templates end with a blank
					// "For" and subjects contain an empty tenant-name segment.
					if (string.Equals(value, "tenantname", StringComparison.OrdinalIgnoreCase))
					{
						replacement = "{tenantname}";
					}
					else if (value == "currentDate")
					{
						replacement = DateTime.Now.ToString("dd MMM yyyy");
					}
					else if (dataTable.Columns.Contains(value))
					{
						replacement = dataTable.Rows[0][value].ToString();
					}

					alertsubject = alertsubject.Replace("{" + value + "}", replacement);
					alertcontent = alertcontent.Replace("{" + value + "}", replacement);
				}
												if (dataTable.Columns.Contains("_tenantname"))
													objmailmodel.tenantname = dataTable.Rows[0]["_tenantname"]?.ToString() ?? "";
												else if (dataTable.Columns.Contains("tenantname"))
													objmailmodel.tenantname = dataTable.Rows[0]["tenantname"]?.ToString() ?? "";

												// Render tenant branding and the booking-deposit link here as well as in
												// the MVC sender. Booking-status notifications can be sent directly by
												// the API, so leaving rendering only to the MVC layer exposes raw tokens.
												alertsubject = ApplyTenantBranding(alertsubject, objmailmodel.tenantname);
												alertcontent = ApplyTenantBranding(alertcontent, objmailmodel.tenantname);
												if (!string.IsNullOrWhiteSpace(str_approvalurl))
												{
													string paymentUrl = str_approvalurl.TrimEnd('/')
														+ "/IPDApplicationForm/Initiate_Payment?IPDApplicationFormid="
														+ Uri.EscapeDataString(entityid ?? "");
													string paymentText = string.Equals(entityactionname, "PaymentReminder", StringComparison.OrdinalIgnoreCase)
														? "Pay Now"
														: "Click Here to Pay Booking Deposit";
													string paymentLink = "<a href='" + paymentUrl
														+ "' style='display:inline-block;padding:10px 20px;background-color:#3399cc;color:#ffffff;text-decoration:none;border-radius:4px;font-weight:bold;'>"
														+ paymentText + "</a>";
													alertcontent = ApplyPaymentLink(alertcontent, paymentLink);
												}
												objmailmodel.mailbody = alertcontent;
												objmailmodel.mailsubject = alertsubject;
												// Patient-facing notifications → patient email; Staff-facing → authorized checker users
												var patientFacingActions = new System.Collections.Generic.HashSet<string>(StringComparer.OrdinalIgnoreCase)
												{
													"ProvisionalBooking", "Rejected", "Waitlisted", "ApprovedForAdmission",
													"ReworkRequested", "ProvisionalConfirmed", "ConfirmedForArrival",
													"AdmissionConfirmed", "AdmittedCancelled", "Admitted",
													"OpdCancelled", "AppointmentCancelled", "AppointmentRescheduled", "PaymentReminder","PreAdmissionNotification"
                                                };
												// Actions where the doctor email column holds the recipient
												var doctorFacingActions = new System.Collections.Generic.HashSet<string>(StringComparer.OrdinalIgnoreCase)
												{
											"OpdRescheduledDoctor", "AppointmentRescheduledDoctor", "AppointmentCancelledDoctor"
												};
												// Actions where the entity's own emailaddress column holds the recipient
												var enquiryFacingActions = new System.Collections.Generic.HashSet<string>(StringComparer.OrdinalIgnoreCase)
												{
													"EnquiryStatusUpdate"
												};
												if (patientFacingActions.Contains(entityactionname))
													objmailmodel.mailto = dataTable.Rows[0]["patient_emailaddress"]?.ToString() ?? "";
												else if (doctorFacingActions.Contains(entityactionname) && dataTable.Columns.Contains("doctor_emailaddress"))
													objmailmodel.mailto = dataTable.Rows[0]["doctor_emailaddress"]?.ToString() ?? "";
												else if (enquiryFacingActions.Contains(entityactionname) && dataTable.Columns.Contains("emailaddress"))
													objmailmodel.mailto = dataTable.Rows[0]["emailaddress"]?.ToString() ?? "";
												else
													objmailmodel.mailto = dataTable.Rows[0]["authorized_users"]?.ToString() ?? "";

											}
											catch
											{
												throw;
											}
											return objmailmodel;
										}

					private static string ApplyPaymentLink(string content, string paymentLink)
					{
						if (string.IsNullOrEmpty(content) || string.IsNullOrEmpty(paymentLink))
							return content;

						const string tilde = @"(?:~|&#0*126;|&#x0*7e;|&tilde;)";
						string replaced = Regex.Replace(
							content,
							tilde + @"(?:\s|&nbsp;)*payment\s*link(?:\s|&nbsp;)*" + tilde,
							_ => paymentLink,
							RegexOptions.IgnoreCase);

						return Regex.Replace(
							replaced,
							@"\{\s*payment\s*link\s*\}",
							_ => paymentLink,
							RegexOptions.IgnoreCase);
					}

					private static string ApplyTenantBranding(string content, string tenantName)
					{
						if (string.IsNullOrEmpty(content) || string.IsNullOrWhiteSpace(tenantName))
							return content;

						string branded = content.Replace("{tenantname}", tenantName, StringComparison.OrdinalIgnoreCase);
						return Regex.Replace(
							branded,
							@"\bFor\s+(?:Nisargopachar\s+Kendra|" + Regex.Escape(tenantName) + @")(?=\s*(?:<br\s*/?>|</p>|</div>|$))",
							"From " + tenantName,
							RegexOptions.IgnoreCase);
					}
										 public virtual mailmodel WhatsApp(string entityname, string entityactionname, string entityid,string tenantid="")
										{

											string alertcontent = "";
											string alertsubject = "";
											string actionmethodname = "";
											mailmodel objmailmodel = new mailmodel();
											DataTable dataTable = new DataTable();
											DataSet dataSet = new DataSet();
											try
											{
												using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
												{
													npsql.Open();
													using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"Alert_Templates_List\"(@pvar_tenantid,@pvar_entityname,@pvar_entityaction,@pvar_alerttype)", npsql))
													{
														dbCommand.CommandType = CommandType.Text;
						dbCommand.Parameters.AddWithValue("pvar_tenantid", (object)tenantid ?? DBNull.Value);
						dbCommand.Parameters.AddWithValue("pvar_entityname", (object)entityname ?? DBNull.Value);
														dbCommand.Parameters.AddWithValue("pvar_entityaction", (object)entityactionname ?? DBNull.Value);
														dbCommand.Parameters.AddWithValue("pvar_alerttype", "WhatsApp");
														using (var reader = dbCommand.ExecuteReader())
														{
															if (reader.Read())
															{
																// Retrieve the values from the reader
																alertsubject = reader["alertsubject"].ToString(); 
																alertcontent = reader["alertcontent"].ToString(); 
																actionmethodname = reader["actionmethodname"].ToString();

															}
															else
															{
																// Handle the case where no data is returned from the stored procedure
															}
														}
													}
													npsql.Close();
												}

												// Define a regular expression pattern to match text inside curly braces
												string pattern = @"\{([^}]*)\}";

												// Use Regex.Matches to find all matches in the input string
												MatchCollection matches = Regex.Matches(alertcontent, pattern);

												// Create an array to store the matched values
												string[] extractedValues = new string[matches.Count];

												// Extract and store the matched values in the array
												for (int i = 0; i < matches.Count; i++)
												{
													extractedValues[i] = matches[i].Groups[1].Value;
												}




												using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
												{
													npsql.Open();
													using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"getById_sp_all_"+entityname+"\"(@pvar_"+entityname.ToLower()+"id)", npsql))
													{
														dbCommand.CommandType = CommandType.Text;
														dbCommand.Parameters.AddWithValue("pvar_"+entityname.ToLower()+"id", (object)entityid ?? DBNull.Value);
														using (NpgsqlDataAdapter dataAdapter = new NpgsqlDataAdapter(dbCommand))
														{
															dataSet.Reset();
															dataAdapter.Fill(dataSet);
															dataTable = dataSet.Tables[0];


															if (dbCommand.Connection.State != ConnectionState.Closed)
															{
																dbCommand.Connection.Dispose();
															}
														}
													}
													npsql.Close();
												}

												if (entityactionname == "ReadyForReview")
												{
													alertcontent = alertcontent.Replace("~clickhere~"," Click Here " + str_approvalurl + "/" + entityname + "/"+ actionmethodname + "?id=" + entityid);
												}

												foreach (string value in extractedValues)
												{
													alertsubject = alertsubject.Replace("{" + value + "}", dataTable.Rows[0][value].ToString());
													alertcontent = alertcontent.Replace("{" + value + "}", dataTable.Rows[0][value].ToString());
												}
												objmailmodel.mailbody = alertcontent;
												objmailmodel.mailsubject = alertsubject;
												objmailmodel.mailto = dataTable.Rows[0]["authorized_users_mobile"].ToString();

											}
											catch
											{
												throw;
											}
											return objmailmodel;
										}

								

              public virtual string Create_MailLog(MailLogsModel model)
			  { 
				  String ResponseMessage="";
					 
					try{
							 
                            using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
					        {
						        npsql.Open();
						        using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"Create_MailLog\"(@pvar_maillogsid,@pvar_entityname,@pvar_entityid,@pvar_mailfor,@pvar_mailsubject,@pvar_mailto,@pvar_mailbody,@pvar_issent,@pvar_createduser)", npsql))
						        {
                                        dbCommand.CommandType = CommandType.Text;
						            	
								        					dbCommand.Parameters.AddWithValue("pvar_maillogsid",NpgsqlDbType.Uuid,(object)model.MailLogsid??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_entityname",NpgsqlDbType.Varchar,(object)model.entityname??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_entityid",NpgsqlDbType.Varchar,(object)model.entityid??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_mailfor",NpgsqlDbType.Varchar,(object)model.mailfor??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_mailsubject",NpgsqlDbType.Varchar,(object)model.mailsubject??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_mailto",NpgsqlDbType.Varchar,(object)model.mailto??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_mailbody",NpgsqlDbType.Varchar,(object)model.mailbody??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_issent",NpgsqlDbType.Boolean,(object)model.issent??DBNull.Value);
dbCommand.Parameters.AddWithValue("pvar_createduser",NpgsqlDbType.Uuid,(object)model.createduser??DBNull.Value);	
															
										
                                        NpgsqlParameter outParm = new NpgsqlParameter("pvar_returnMessage", NpgsqlDbType.Varchar)
                                        {
                                             Direction = ParameterDirection.Output
                                        };
                                        dbCommand.Parameters.Add(outParm);

                                        dbCommand.ExecuteNonQuery();
								        ResponseMessage = outParm.Value.ToString();
								        if (dbCommand.Connection.State != ConnectionState.Closed)
                    			        {
										         dbCommand.Connection.Dispose();
								        }

						        }
						        npsql.Close();
					        }
 

					}catch(Exception ex){
						ResponseMessage=ex.Message;
						Console.WriteLine(ex);
					} 
					
					return ResponseMessage;

			   }
public virtual MailLogsModel getById_MailLogs(string MailLogsid)
									 {
										DataTable dataTable = new DataTable();
										DataSet dataSet = new DataSet();
										try{
												 
												using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
												{
													npsql.Open();
													using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"getById_sp_MailLogs\"(@pvar_maillogsid)", npsql))
													{
														dbCommand.CommandType = CommandType.Text;
														dbCommand.Parameters.AddWithValue("pvar_maillogsid",(object)MailLogsid??DBNull.Value);
														using (NpgsqlDataAdapter dataAdapter = new NpgsqlDataAdapter(dbCommand))
														{
															dataSet.Reset();
															dataAdapter.Fill(dataSet);
															dataTable = dataSet.Tables[0];
															if (dbCommand.Connection.State != ConnectionState.Closed)
															{
																dbCommand.Connection.Dispose();
															}
														}
													}
													npsql.Close();
												}
					 
										}catch{
												throw;
										}
										if (dataTable.Rows.Count > 0)
										{
											DataRow row = dataTable.Rows[0];
											return ModelConverter.ConvertDataRowToModel<MailLogsModel>(row);
										}
										else
										{
											return null;
										}
									 }
			 public virtual string  Update_MailLog(MailLogsModel model)
			 { 
				 String ResponseMessage="";
					try{
						 	 
							using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
							{
								npsql.Open();
								using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"Update_MailLog\"(@pvar_maillogsid,@pvar_entityname,@pvar_entityid,@pvar_mailfor,@pvar_mailsubject,@pvar_mailto,@pvar_mailbody,@pvar_issent,@pvar_modifieduser)", npsql))
								{
										dbCommand.CommandType = CommandType.Text;
															dbCommand.Parameters.AddWithValue("pvar_maillogsid",NpgsqlDbType.Uuid,(object)model.MailLogsid??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_entityname",NpgsqlDbType.Varchar,(object)model.entityname??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_entityid",NpgsqlDbType.Varchar,(object)model.entityid??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_mailfor",NpgsqlDbType.Varchar,(object)model.mailfor??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_mailsubject",NpgsqlDbType.Varchar,(object)model.mailsubject??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_mailto",NpgsqlDbType.Varchar,(object)model.mailto??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_mailbody",NpgsqlDbType.Varchar,(object)model.mailbody??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_issent",NpgsqlDbType.Boolean,(object)model.issent??DBNull.Value);
dbCommand.Parameters.AddWithValue("pvar_modifieduser",NpgsqlDbType.Uuid,model.modifieduser);	
															
										NpgsqlParameter outParm = new NpgsqlParameter("@returnMessage", NpgsqlDbType.Varchar)
										{
											 Direction = ParameterDirection.Output
										};
										dbCommand.Parameters.Add(outParm);

										dbCommand.ExecuteNonQuery();
										ResponseMessage = outParm.Value.ToString();
										if (dbCommand.Connection.State != ConnectionState.Closed)
										{
												 dbCommand.Connection.Dispose();
										}

								}
								npsql.Close();
							}		 

					}catch(Exception ex){
						ResponseMessage=ex.Message;
					}
					
					return ResponseMessage;

			   }
public virtual string  Remove_MailLog(string id,string loginUserID)
			  { 
				  String ResponseMessage="";
					try{ 
							using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
							{
								npsql.Open();
								using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"Remove_MailLog\"(@pvar_maillogsid,@pvar_modifieduser)", npsql))
								{
										dbCommand.CommandType = CommandType.Text;
										dbCommand.Parameters.AddWithValue("pvar_maillogsid",(object)id??DBNull.Value);
										dbCommand.Parameters.AddWithValue("pvar_modifieduser",(object)loginUserID??DBNull.Value);
										NpgsqlParameter outParm = new NpgsqlParameter("@returnMessage", NpgsqlDbType.Varchar)
										{
											 Direction = ParameterDirection.Output
										};
										dbCommand.Parameters.Add(outParm);

										dbCommand.ExecuteNonQuery();
										ResponseMessage = outParm.Value.ToString();
										if (dbCommand.Connection.State != ConnectionState.Closed)
										{
												 dbCommand.Connection.Dispose();
										}

								}
								npsql.Close();
							}	 

					}catch(Exception ex){
						ResponseMessage=ex.Message;
					}
					
					return ResponseMessage;

			   }
public virtual JObject MailLogs_List(string entityname
,string mailfor
, int? pagesize=1000 , int? pagenumber=0,string searchterm="",string  sort_fields="")
			  { 
				  object dalResponse = null;
			
					try{
 
							using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
							{
								
								using (var dbCommand = new NpgsqlCommand("SELECT * FROM\"MailLogs_List\"(@pvar_entityname,@pvar_mailfor,@pvar_pagesize,@pvar_pagenumber,@pvar_searchterm,@pvar_sort_fields)", npsql))
								{
									dbCommand.CommandType = CommandType.Text;
									dbCommand.Parameters.AddWithValue("pvar_entityname",(object)entityname??DBNull.Value);
dbCommand.Parameters.AddWithValue("pvar_mailfor",(object)mailfor??DBNull.Value);

									dbCommand.Parameters.AddWithValue("pvar_pagesize",(object)pagesize??DBNull.Value);
									dbCommand.Parameters.AddWithValue("pvar_pagenumber",(object)pagenumber??DBNull.Value);

									dbCommand.Parameters.AddWithValue("pvar_searchterm",(object)searchterm??DBNull.Value);
									if (sort_fields != null && sort_fields.Length > 2)
									dbCommand.Parameters.AddWithValue("pvar_sort_fields", NpgsqlDbType.Json, sort_fields);
									else
									dbCommand.Parameters.AddWithValue("pvar_sort_fields", NpgsqlDbType.Json, DBNull.Value);


									npsql.Open();
									dalResponse = dbCommand.ExecuteScalar();
									npsql.Close();
									
								}
								
							}

						 

					}catch{
						throw;
					}


					return JObject.Parse(dalResponse.ToString());	


					 

			   }
			   
			 
public virtual System.Data.DataTable get_all_MailLogs(string tenantid,string searchterm="", int? pagesize=1000, int? pagenumber=0)
			  { 

				    DataTable dataTable = new DataTable();
					DataSet dataSet = new DataSet();

					try{
 
							using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
							{
								npsql.Open();
								using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"get_all_MailLogs\"(@pvar_tenantid)", npsql))
								{
									dbCommand.CommandType = CommandType.Text;
									dbCommand.Parameters.AddWithValue("pvar_tenantid",(object)tenantid??DBNull.Value);
									dbCommand.Parameters.AddWithValue("pvar_searchterm", (object)searchterm?? DBNull.Value);
                dbCommand.Parameters.AddWithValue("pvar_pagesize", (object)pagesize ?? DBNull.Value);
                dbCommand.Parameters.AddWithValue("pvar_pagenumber", (object)pagenumber ?? DBNull.Value);
									using (NpgsqlDataAdapter dataAdapter = new NpgsqlDataAdapter(dbCommand))
									{
										dataSet.Reset();
										dataAdapter.Fill(dataSet);
										dataTable = dataSet.Tables[0];
										if (dbCommand.Connection.State != ConnectionState.Closed)
										{
											dbCommand.Connection.Dispose();
										}
									}
								}
								npsql.Close();
							}
						

					}catch{
						throw;
					}
					return dataTable;	


					 

			   }
public virtual System.Data.DataTable getById_allinfo_MailLogs(string MailLogsid)
			 {
				DataSet dataSet=new DataSet();
				DataTable dataTable = new DataTable();
				try{
					     
						using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
						{
							npsql.Open();
							using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"getById_sp_all_MailLogs\"(@pvar_maillogsid)", npsql))
							{
								dbCommand.CommandType = CommandType.Text;
								dbCommand.Parameters.AddWithValue("pvar_maillogsid",(object)MailLogsid??DBNull.Value);
								using (NpgsqlDataAdapter dataAdapter = new NpgsqlDataAdapter(dbCommand))
								{
									dataSet.Reset();
									dataAdapter.Fill(dataSet);
									dataTable = dataSet.Tables[0];
									if (dbCommand.Connection.State != ConnectionState.Closed)
									{
										dbCommand.Connection.Dispose();
									}
								}
							}
							npsql.Close();
						}
					 
				}catch{
						throw;
				}
				return dataTable;
			 }
			  







			    }


			    }
