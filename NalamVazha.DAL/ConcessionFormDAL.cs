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

			    //This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 05/26/2026 07:14:50
			    public class ConcessionFormDAL
			    {
					public virtual string db_connectionstring{get;set;}
					
			 	    public ConcessionFormDAL(string connectionString)
				    {
						
					    db_connectionstring=connectionString;
				    }

					public virtual void EnsureWhichOfTheseDoYouOwnNoneLookup()
					{
						try
						{
							using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
							{
								npsql.Open();
								using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"EnsureWhichOfTheseDoYouOwnNoneLookup\"()", npsql))
								{
									dbCommand.CommandType = CommandType.Text;
									dbCommand.ExecuteNonQuery();
								}
								npsql.Close();
							}
						}
						catch
						{
							throw;
						}
					}
				  
			        public virtual System.Data.DataTable getById_earningmembers(string ConcessionFormid)
			 {
					DataTable dataTable=new DataTable();
					DataSet dataSet=new DataSet();
					try
					{
						  
							using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
							{
								npsql.Open();
								using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"getById_sp_ConcessionForm_earningmembers\"(@pvar_concessionformid)", npsql))
								{
									dbCommand.CommandType = CommandType.Text;
									dbCommand.Parameters.AddWithValue("pvar_concessionformid",(object)ConcessionFormid??DBNull.Value);
								
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


              public virtual string Add_Concession_Form(ConcessionFormModel model)
			  { 
				  String ResponseMessage="";
					 
					try{
							 
                            using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
					        {
						        npsql.Open();
						        using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"Add_Concession_Form\"(@pvar_concessionformid,@pvar_tenantid,@pvar_patientname,@pvar_bookingreferencenumber,@pvar_residentialhousetype,@pvar_totalannualfamilyincome,@pvar_whichofthesedoyouown,@pvar_paidservicesaccessed,@pvar_haveyoutraveledinternationallyinthelast5years,@pvar_purposeoftravel,@pvar_otherpleasespecify,@pvar_doanyofthefollowingapplytoyou,@pvar_specifyotherindustry,@pvar_requestedconcessionamount,@pvar_verifiedstatus,@pvar_earningmembers,@pvar_createduser)", npsql))
						        {
                                        dbCommand.CommandType = CommandType.Text;
						            	
								        					dbCommand.Parameters.AddWithValue("pvar_concessionformid",NpgsqlDbType.Uuid,(object)model.ConcessionFormid??DBNull.Value);
dbCommand.Parameters.AddWithValue("pvar_tenantid",NpgsqlDbType.Uuid,(object)model.tenantid??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_patientname",NpgsqlDbType.Uuid,(object)model.patientname??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_bookingreferencenumber",NpgsqlDbType.Uuid,(object)model.bookingreferencenumber??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_residentialhousetype",NpgsqlDbType.Varchar,(object)model.residentialhousetype??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_totalannualfamilyincome",NpgsqlDbType.Varchar,(object)model.totalannualfamilyincome??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_whichofthesedoyouown",NpgsqlDbType.Varchar,(object)model.whichofthesedoyouown??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_paidservicesaccessed",NpgsqlDbType.Varchar,(object)model.paidservicesaccessed??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_haveyoutraveledinternationallyinthelast5years",NpgsqlDbType.Varchar,(object)model.haveyoutraveledinternationallyinthelast5years??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_purposeoftravel",NpgsqlDbType.Varchar,(object)model.purposeoftravel??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_otherpleasespecify",NpgsqlDbType.Varchar,(object)model.otherpleasespecify??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_doanyofthefollowingapplytoyou",NpgsqlDbType.Varchar,(object)model.doanyofthefollowingapplytoyou??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_specifyotherindustry",NpgsqlDbType.Varchar,(object)model.specifyotherindustry??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_requestedconcessionamount",NpgsqlDbType.Numeric,(object)model.requestedconcessionamount??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_verifiedstatus",NpgsqlDbType.Varchar,(object)model.verifiedstatus??DBNull.Value);
if(model.earningmembers !=null  && model.earningmembers.Count >0)
dbCommand.Parameters.AddWithValue("pvar_earningmembers",NpgsqlDbType.Json,JsonConvert.SerializeObject(model.earningmembers));
else
dbCommand.Parameters.AddWithValue("pvar_earningmembers",NpgsqlDbType.Json,DBNull.Value);
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
public virtual ConcessionFormModel getById_ConcessionForm(string ConcessionFormid)
									 {
										DataTable dataTable = new DataTable();
										DataSet dataSet = new DataSet();
										try{
												 
												using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
												{
													npsql.Open();
													using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"getById_sp_ConcessionForm\"(@pvar_concessionformid)", npsql))
													{
														dbCommand.CommandType = CommandType.Text;
														dbCommand.Parameters.AddWithValue("pvar_concessionformid",(object)ConcessionFormid??DBNull.Value);
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
											return ModelConverter.ConvertDataRowToModel<ConcessionFormModel>(row);
										}
										else
										{
											return null;
										}
									 }
			 public virtual string  Update_Concession_Form(ConcessionFormModel model)
			 { 
				 String ResponseMessage="";
					try{
						 	 
							using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
							{
								npsql.Open();
								using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"Update_Concession_Form\"(@pvar_concessionformid,@pvar_tenantid,@pvar_patientname,@pvar_bookingreferencenumber,@pvar_residentialhousetype,@pvar_totalannualfamilyincome,@pvar_whichofthesedoyouown,@pvar_paidservicesaccessed,@pvar_haveyoutraveledinternationallyinthelast5years,@pvar_purposeoftravel,@pvar_otherpleasespecify,@pvar_doanyofthefollowingapplytoyou,@pvar_specifyotherindustry,@pvar_requestedconcessionamount,@pvar_verifiedstatus,@pvar_earningmembers,@pvar_modifieduser)", npsql))
								{
										dbCommand.CommandType = CommandType.Text;
															dbCommand.Parameters.AddWithValue("pvar_concessionformid",NpgsqlDbType.Uuid,(object)model.ConcessionFormid??DBNull.Value);
dbCommand.Parameters.AddWithValue("pvar_tenantid",NpgsqlDbType.Uuid,(object)model.tenantid??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_patientname",NpgsqlDbType.Uuid,(object)model.patientname??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_bookingreferencenumber",NpgsqlDbType.Uuid,(object)model.bookingreferencenumber??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_residentialhousetype",NpgsqlDbType.Varchar,(object)model.residentialhousetype??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_totalannualfamilyincome",NpgsqlDbType.Varchar,(object)model.totalannualfamilyincome??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_whichofthesedoyouown",NpgsqlDbType.Varchar,(object)model.whichofthesedoyouown??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_paidservicesaccessed",NpgsqlDbType.Varchar,(object)model.paidservicesaccessed??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_haveyoutraveledinternationallyinthelast5years",NpgsqlDbType.Varchar,(object)model.haveyoutraveledinternationallyinthelast5years??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_purposeoftravel",NpgsqlDbType.Varchar,(object)model.purposeoftravel??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_otherpleasespecify",NpgsqlDbType.Varchar,(object)model.otherpleasespecify??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_doanyofthefollowingapplytoyou",NpgsqlDbType.Varchar,(object)model.doanyofthefollowingapplytoyou??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_specifyotherindustry",NpgsqlDbType.Varchar,(object)model.specifyotherindustry??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_requestedconcessionamount",NpgsqlDbType.Numeric,(object)model.requestedconcessionamount??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_verifiedstatus",NpgsqlDbType.Varchar,(object)model.verifiedstatus??DBNull.Value);
if(model.earningmembers !=null  && model.earningmembers.Count >0)
dbCommand.Parameters.AddWithValue("pvar_earningmembers",NpgsqlDbType.Json,JsonConvert.SerializeObject(model.earningmembers));
else
dbCommand.Parameters.AddWithValue("pvar_earningmembers",NpgsqlDbType.Json,DBNull.Value);
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
public virtual string  Remove_Concession_Form(string id,string loginUserID)
			  { 
				  String ResponseMessage="";
					try{ 
							using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
							{
								npsql.Open();
								using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"Remove_Concession_Form\"(@pvar_concessionformid,@pvar_modifieduser)", npsql))
								{
										dbCommand.CommandType = CommandType.Text;
										dbCommand.Parameters.AddWithValue("pvar_concessionformid",(object)id??DBNull.Value);
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
public virtual System.Data.DataTable Added_Concession_Form(string tenantid
,string verifiedstatus
)
			  { 
					DataTable dataTable = new DataTable();
                DataSet dataSet = new DataSet(); 

					try{
 
							using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
							{
								npsql.Open();
								using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"Added_Concession_Form\"(@pvar_tenantid,@pvar_verifiedstatus)", npsql))
								{
									dbCommand.CommandType = CommandType.Text;
									dbCommand.Parameters.AddWithValue("pvar_tenantid",(object)tenantid??DBNull.Value);
dbCommand.Parameters.AddWithValue("pvar_verifiedstatus",(object)verifiedstatus??DBNull.Value);

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
			   
			 
public virtual System.Data.DataTable get_all_ConcessionForm(string tenantid)
			  { 

				    DataTable dataTable = new DataTable();
					DataSet dataSet = new DataSet();

					try{
 
							using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
							{
								npsql.Open();
								using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"get_all_ConcessionForm\"(@pvar_tenantid)", npsql))
								{
									dbCommand.CommandType = CommandType.Text;
									dbCommand.Parameters.AddWithValue("pvar_tenantid",(object)tenantid??DBNull.Value);
									
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
public virtual System.Data.DataTable count_of_ConcessionForm(string tenantid
)
			  { 
					DataTable dataTable = new DataTable();
                DataSet dataSet = new DataSet(); 

					try{
 
							using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
							{
								npsql.Open();
								using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"count_of_ConcessionForm\"(@pvar_tenantid)", npsql))
								{
									dbCommand.CommandType = CommandType.Text;
									dbCommand.Parameters.AddWithValue("pvar_tenantid",(object)tenantid??DBNull.Value);

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
			   
			 
public virtual System.Data.DataTable Concession_Form_for_Review(string tenantid
,string verifiedstatus
)
			  { 
					DataTable dataTable = new DataTable();
                DataSet dataSet = new DataSet(); 

					try{
 
							using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
							{
								npsql.Open();
								using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"Concession_Form_for_Review\"(@pvar_tenantid,@pvar_verifiedstatus)", npsql))
								{
									dbCommand.CommandType = CommandType.Text;
									dbCommand.Parameters.AddWithValue("pvar_tenantid",(object)tenantid??DBNull.Value);
dbCommand.Parameters.AddWithValue("pvar_verifiedstatus",(object)verifiedstatus??DBNull.Value);

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
			   
			 
public virtual System.Data.DataTable Concession_Form_List(string tenantid
)
			  { 
					DataTable dataTable = new DataTable();
                DataSet dataSet = new DataSet(); 

					try{
 
							using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
							{
								npsql.Open();
								using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"Concession_Form_List\"(@pvar_tenantid)", npsql))
								{
									dbCommand.CommandType = CommandType.Text;
									dbCommand.Parameters.AddWithValue("pvar_tenantid",(object)tenantid??DBNull.Value);

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
			   
			 
public virtual System.Data.DataTable getById_allinfo_ConcessionForm(string ConcessionFormid)
			 {
				DataSet dataSet=new DataSet();
				DataTable dataTable = new DataTable();
				try{
					     
						using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
						{
							npsql.Open();
							using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"getById_sp_all_ConcessionForm\"(@pvar_concessionformid)", npsql))
							{
								dbCommand.CommandType = CommandType.Text;
								dbCommand.Parameters.AddWithValue("pvar_concessionformid",(object)ConcessionFormid??DBNull.Value);
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
public virtual System.Data.DataTable get_Existing_Concession_Form(string tenantid, string patientname, string ipdnumber)
			 {
				DataSet dataSet=new DataSet();
				DataTable dataTable = new DataTable();
				try{
						using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
						{
							npsql.Open();
							using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"get_Existing_Concession_Form\"(@pvar_tenantid,@pvar_patientname,@pvar_bookingreferencenumber)", npsql))
							{
								dbCommand.CommandType = CommandType.Text;
								dbCommand.Parameters.AddWithValue("pvar_tenantid",(object)tenantid??DBNull.Value);
								dbCommand.Parameters.AddWithValue("pvar_patientname",(object)patientname??DBNull.Value);
								dbCommand.Parameters.AddWithValue("pvar_bookingreferencenumber",(object)ipdnumber??DBNull.Value);
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
			  public virtual string  verify_ConcessionForm(ConcessionFormReviewModel model)
			 { 
				 String ResponseMessage="";
					try{
						 
							 
							using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
							{
								npsql.Open();
								if (string.Equals(model.verifiedstatus, "Approved", StringComparison.OrdinalIgnoreCase))
								{
									if (!model.approvedconcessionamount.HasValue)
									{
										return "Approved Concession Amount is required.";
									}
									if (model.approvedconcessionamount.Value > 0)
									{
										model.approvedconcessionamount = -model.approvedconcessionamount.Value;
									}
									if (model.approvedconcessionamount.Value < -99999999 || model.approvedconcessionamount.Value > 99999999)
									{
										return "Approved Concession Amount should be between -99999999 and 99999999.";
									}
									using (var validateCommand = new NpgsqlCommand("SELECT \"Verify_Concession_ApprovedAmount\"(@pvar_concessionformid,@pvar_approvedconcessionamount)", npsql))
									{
										validateCommand.Parameters.AddWithValue("pvar_concessionformid",(object)model.ConcessionFormid??DBNull.Value);
										validateCommand.Parameters.AddWithValue("pvar_approvedconcessionamount",NpgsqlDbType.Numeric,(object)model.approvedconcessionamount??DBNull.Value);
										var invalidCount = Convert.ToInt32(validateCommand.ExecuteScalar() ?? 0);
										if (invalidCount > 0)
										{
											return "Approved concession amount cannot exceed requested concession amount.";
										}
									}
								}
								using (var transaction = npsql.BeginTransaction())
								using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"verify_ConcessionForm\"(@pvar_concessionformid,@pvar_verifiedby,@pvar_verifiedstatus,@pvar_reviewcomments,@pvar_approvedconcessionamount)", npsql, transaction))
								{
										dbCommand.CommandType = CommandType.Text;
										 dbCommand.Parameters.AddWithValue("pvar_concessionformid",(object)model.ConcessionFormid??DBNull.Value);
										dbCommand.Parameters.AddWithValue("pvar_verifiedby",(object)model.verifiedby??DBNull.Value);
										dbCommand.Parameters.AddWithValue("pvar_verifiedstatus",(object)model.verifiedstatus??DBNull.Value);
										dbCommand.Parameters.AddWithValue("pvar_reviewcomments",(object)model.reviewcomments??DBNull.Value); 
										dbCommand.Parameters.AddWithValue("pvar_approvedconcessionamount",NpgsqlDbType.Numeric,(object)model.approvedconcessionamount??DBNull.Value);
											
										NpgsqlParameter outParm = new NpgsqlParameter("@returnMessage", NpgsqlDbType.Varchar)
										{
											 Direction = ParameterDirection.Output
										};
										dbCommand.Parameters.Add(outParm);

										dbCommand.ExecuteNonQuery();
										ResponseMessage = outParm.Value.ToString();
										if (ResponseMessage == "201.1" && string.Equals(model.verifiedstatus, "Approved", StringComparison.OrdinalIgnoreCase))
										{
											CreateApprovedConcessionReceivables(npsql, transaction, model);
										}
										transaction.Commit();

								}
								npsql.Close();
							}
						 

					}catch(Exception ex){
						ResponseMessage=ex.Message;
					}
					
					return ResponseMessage;

			   }

		private void CreateApprovedConcessionReceivables(NpgsqlConnection connection, NpgsqlTransaction transaction, ConcessionFormReviewModel model)
		{
			if (!Guid.TryParse(model.verifiedby, out var createdUser))
			{
				throw new InvalidOperationException("A valid verified-by user ID is required to create concession receivables.");
			}

			using (var command = new NpgsqlCommand("SELECT public.\"CreateApprovedConcessionReceivables\"(CAST(@pvar_concessionformid AS character varying), @pvar_createduser)", connection, transaction))
			{
				command.Parameters.AddWithValue("pvar_concessionformid", NpgsqlDbType.Varchar, (object)model.ConcessionFormid ?? DBNull.Value);
				command.Parameters.AddWithValue("pvar_createduser", NpgsqlDbType.Uuid, createdUser);
				command.ExecuteScalar();
			}
		}
public virtual System.Data.DataTable lookup_ConcessionForm_patientname(String tenantid,string searchterm="", int? pagesize=1000, int? pagenumber=0)
							        {
                                            DataSet dataSet = new DataSet();
									        DataTable dataTable=new DataTable();
									        try{

                                        		        using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
					                                    {
						                                    npsql.Open();
						                                    using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"lookup_ConcessionForm_patientname\"(@pvar_tenantid,@pvar_searchterm,@pvar_pagesize,@pvar_pagenumber)", npsql))
						                                    {
                                            
                                                                dbCommand.Parameters.AddWithValue("pvar_tenantid",(object)tenantid??DBNull.Value);  
																dbCommand.Parameters.AddWithValue("pvar_searchterm", (object)searchterm?? DBNull.Value);
                dbCommand.Parameters.AddWithValue("pvar_pagesize", (object)pagesize ?? DBNull.Value);
                dbCommand.Parameters.AddWithValue("pvar_pagenumber", (object)pagenumber ?? DBNull.Value);
																 
                                                                dbCommand.CommandType = CommandType.Text;
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
public virtual System.Data.DataTable lookup_ConcessionForm_bookingreferencenumber(String tenantid,String patientname,string searchterm="", int? pagesize=1000, int? pagenumber=0)
							        {
                                            DataSet dataSet = new DataSet();
									        DataTable dataTable=new DataTable();
									        try{

                                        		        using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
					                                    {
						                                    npsql.Open();
						                                    using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"lookup_ConcessionForm_bookingreferencenumber\"(@pvar_tenantid,@pvar_patientname,@pvar_searchterm,@pvar_pagesize,@pvar_pagenumber)", npsql))
						                                    {
                                            
                                                                dbCommand.Parameters.AddWithValue("pvar_tenantid",(object)tenantid??DBNull.Value);dbCommand.Parameters.AddWithValue("pvar_patientname",(object)patientname??DBNull.Value);  
																dbCommand.Parameters.AddWithValue("pvar_searchterm", (object)searchterm?? DBNull.Value);
                dbCommand.Parameters.AddWithValue("pvar_pagesize", (object)pagesize ?? DBNull.Value);
                dbCommand.Parameters.AddWithValue("pvar_pagenumber", (object)pagenumber ?? DBNull.Value);
																 
                                                                dbCommand.CommandType = CommandType.Text;
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
