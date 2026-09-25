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

			    //This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:43:34
			    public class ClinicalAppointmentDAL
			    {
					public virtual string db_connectionstring{get;set;}
					
			 	    public ClinicalAppointmentDAL(string connectionString)
				    {
						
					    db_connectionstring=connectionString;
				    }
				  
			        public virtual System.Data.DataTable getById_reshedulehistory(string ClinicalAppointmentid)
			 {
					DataTable dataTable=new DataTable();
					DataSet dataSet=new DataSet();
					try
					{
						  
							using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
							{
								npsql.Open();
								using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"getById_sp_ClinicalAppointment_reshedulehistory\"(@pvar_clinicalappointmentid)", npsql))
								{
									dbCommand.CommandType = CommandType.Text;
									dbCommand.Parameters.AddWithValue("pvar_clinicalappointmentid",(object)ClinicalAppointmentid??DBNull.Value);
								
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


              public virtual string Add_Clinical_Appointment(ClinicalAppointmentModel model)
			  { 
				  String ResponseMessage="";
					 
					try{
							 
                            using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
					        {
						        npsql.Open();
								using (var transaction = npsql.BeginTransaction())
								{
									if (string.Equals(model.origin?.Trim(), "OPD", StringComparison.OrdinalIgnoreCase)
										&& model.tenantid.HasValue
										&& model.patient.HasValue)
									{
										// Serialize OPD booking attempts for the same tenant, patient and day so
										// concurrent requests cannot both pass the stored-function validation.
										var appointmentDate = model.appointmentdate.Date;
										using (var duplicateCommand = new NpgsqlCommand(
											"SELECT public.\"Validate_OPD_Appointment_Duplicate\"(@pvar_tenantid,@pvar_patient,@pvar_appointmentdate)",
											npsql,
											transaction))
										{
											duplicateCommand.Parameters.AddWithValue("pvar_tenantid", NpgsqlDbType.Uuid, model.tenantid.Value);
											duplicateCommand.Parameters.AddWithValue("pvar_patient", NpgsqlDbType.Uuid, model.patient.Value);
											duplicateCommand.Parameters.AddWithValue("pvar_appointmentdate", NpgsqlDbType.Date, appointmentDate);
											var validationMessage = duplicateCommand.ExecuteScalar()?.ToString()
												?? "Unable to validate the OPD appointment.";
											if (!string.Equals(validationMessage, "201.1", StringComparison.Ordinal))
											{
												transaction.Rollback();
												return validationMessage;
											}
										}
									}

						        using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"Add_Clinical_Appointment\"(@pvar_clinicalappointmentid,@pvar_tenantid,@pvar_tasktype,@pvar_task,@pvar_patient,@pvar_practitioner,@pvar_photo,@pvar_actualpractitioner,@pvar_appointmentdate,@pvar_durationfrom,@pvar_durationto,@pvar_status,@pvar_origin,@pvar_bookingid,@pvar_tokennumber,@pvar_reshedulehistory,@pvar_createduser)", npsql))
						        {
										dbCommand.Transaction = transaction;
                                        dbCommand.CommandType = CommandType.Text;
						            	
								        					dbCommand.Parameters.AddWithValue("pvar_clinicalappointmentid",NpgsqlDbType.Uuid,(object)model.ClinicalAppointmentid??DBNull.Value);
dbCommand.Parameters.AddWithValue("pvar_tenantid",NpgsqlDbType.Uuid,(object)model.tenantid??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_tasktype",NpgsqlDbType.Varchar,(object)model.tasktype??DBNull.Value);
                        dbCommand.Parameters.AddWithValue("pvar_task", NpgsqlDbType.Varchar, (object)model.taskname ?? DBNull.Value);

                        dbCommand.Parameters.AddWithValue("pvar_patient",NpgsqlDbType.Uuid,(object)model.patient??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_practitioner",NpgsqlDbType.Uuid,(object)model.practitioner??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_photo",NpgsqlDbType.Varchar,(object)model.photo??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_actualpractitioner",NpgsqlDbType.Uuid,(object)model.actualpractitioner??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_appointmentdate",NpgsqlDbType.Date,(object)model.appointmentdate??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_durationfrom",NpgsqlDbType.Varchar,(object)model.durationfrom??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_durationto",NpgsqlDbType.Varchar,(object)model.durationto??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_status",NpgsqlDbType.Varchar,(object)model.status??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_origin",NpgsqlDbType.Varchar,(object)model.origin??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_bookingid",NpgsqlDbType.Varchar,(object)model.bookingid??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_tokennumber",NpgsqlDbType.Varchar,(object)model.tokennumber??DBNull.Value);
if(model.reshedulehistory !=null  && model.reshedulehistory.Count >0)
dbCommand.Parameters.AddWithValue("pvar_reshedulehistory",NpgsqlDbType.Json,JsonConvert.SerializeObject(model.reshedulehistory));
else
dbCommand.Parameters.AddWithValue("pvar_reshedulehistory",NpgsqlDbType.Json,DBNull.Value);
dbCommand.Parameters.AddWithValue("pvar_createduser",NpgsqlDbType.Uuid,(object)model.createduser??DBNull.Value);	
															
										
                                        NpgsqlParameter outParm = new NpgsqlParameter("pvar_returnMessage", NpgsqlDbType.Varchar)
                                        {
                                             Direction = ParameterDirection.Output
                                        };
                                        dbCommand.Parameters.Add(outParm);

                                        dbCommand.ExecuteNonQuery();
								        ResponseMessage = outParm.Value.ToString();

							        }
									transaction.Commit();
								}
						        npsql.Close();
					        }
 

					}catch(Exception ex){
						ResponseMessage=ex.Message;
						Console.WriteLine(ex);
					} 
					
					return ResponseMessage;

			   }
public virtual ClinicalAppointmentModel getById_ClinicalAppointment(string ClinicalAppointmentid)
									 {
										DataTable dataTable = new DataTable();
										DataSet dataSet = new DataSet();
										try{
												 
												using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
												{
													npsql.Open();
													using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"getById_sp_ClinicalAppointment\"(@pvar_clinicalappointmentid)", npsql))
													{
														dbCommand.CommandType = CommandType.Text;
														dbCommand.Parameters.AddWithValue("pvar_clinicalappointmentid",(object)ClinicalAppointmentid??DBNull.Value);
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
											return ModelConverter.ConvertDataRowToModel<ClinicalAppointmentModel>(row);
										}
										else
										{
											return null;
										}
									 }
			 public virtual string  Reschedule_Appointment(ClinicalAppointmentModel model)
			 { 
				 String ResponseMessage="";
					try{
						 	 
							using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
							{
								npsql.Open();
								using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"Reschedule_Appointment\"(@pvar_clinicalappointmentid,@pvar_tenantid,@pvar_tasktype,@pvar_patient,@pvar_practitioner,@pvar_photo,@pvar_actualpractitioner,@pvar_appointmentdate,@pvar_durationfrom,@pvar_durationto,@pvar_status,@pvar_origin,@pvar_bookingid,@pvar_tokennumber,@pvar_reshedulehistory,@pvar_modifieduser)", npsql))
								{
										dbCommand.CommandType = CommandType.Text;
															dbCommand.Parameters.AddWithValue("pvar_clinicalappointmentid",NpgsqlDbType.Uuid,(object)model.ClinicalAppointmentid??DBNull.Value);
dbCommand.Parameters.AddWithValue("pvar_tenantid",NpgsqlDbType.Uuid,(object)model.tenantid??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_tasktype",NpgsqlDbType.Varchar,(object)model.tasktype??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_patient",NpgsqlDbType.Uuid,(object)model.patient??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_practitioner",NpgsqlDbType.Uuid,(object)model.practitioner??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_photo",NpgsqlDbType.Varchar,(object)model.photo??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_actualpractitioner",NpgsqlDbType.Uuid,(object)model.actualpractitioner??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_appointmentdate",NpgsqlDbType.Date,(object)model.appointmentdate??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_durationfrom",NpgsqlDbType.Varchar,(object)model.durationfrom??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_durationto",NpgsqlDbType.Varchar,(object)model.durationto??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_status",NpgsqlDbType.Varchar,(object)model.status??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_origin",NpgsqlDbType.Varchar,(object)model.origin??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_bookingid",NpgsqlDbType.Varchar,(object)model.bookingid??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_tokennumber",NpgsqlDbType.Varchar,(object)model.tokennumber??DBNull.Value);
if(model.reshedulehistory !=null  && model.reshedulehistory.Count >0)
dbCommand.Parameters.AddWithValue("pvar_reshedulehistory",NpgsqlDbType.Json,JsonConvert.SerializeObject(model.reshedulehistory));
else
dbCommand.Parameters.AddWithValue("pvar_reshedulehistory",NpgsqlDbType.Json,DBNull.Value);
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

			 public virtual string  Update_Clinical_Appointment(ClinicalAppointmentModel model)
			 { 
				 String ResponseMessage="";
					try{
						 	 
							using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
							{
								npsql.Open();
								using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"Update_Clinical_Appointment\"(@pvar_clinicalappointmentid,@pvar_tenantid,@pvar_tasktype,@pvar_patient,@pvar_practitioner,@pvar_photo,@pvar_actualpractitioner,@pvar_appointmentdate,@pvar_durationfrom,@pvar_durationto,@pvar_status,@pvar_origin,@pvar_bookingid,@pvar_tokennumber,@pvar_reshedulehistory,@pvar_modifieduser)", npsql))
								{
										dbCommand.CommandType = CommandType.Text;
															dbCommand.Parameters.AddWithValue("pvar_clinicalappointmentid",NpgsqlDbType.Uuid,(object)model.ClinicalAppointmentid??DBNull.Value);
dbCommand.Parameters.AddWithValue("pvar_tenantid",NpgsqlDbType.Uuid,(object)model.tenantid??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_tasktype",NpgsqlDbType.Varchar,(object)model.tasktype??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_patient",NpgsqlDbType.Uuid,(object)model.patient??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_practitioner",NpgsqlDbType.Uuid,(object)model.practitioner??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_photo",NpgsqlDbType.Varchar,(object)model.photo??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_actualpractitioner",NpgsqlDbType.Uuid,(object)model.actualpractitioner??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_appointmentdate",NpgsqlDbType.Date,(object)model.appointmentdate??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_durationfrom",NpgsqlDbType.Varchar,(object)model.durationfrom??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_durationto",NpgsqlDbType.Varchar,(object)model.durationto??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_status",NpgsqlDbType.Varchar,(object)model.status??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_origin",NpgsqlDbType.Varchar,(object)model.origin??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_bookingid",NpgsqlDbType.Varchar,(object)model.bookingid??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_tokennumber",NpgsqlDbType.Varchar,(object)model.tokennumber??DBNull.Value);
if(model.reshedulehistory !=null  && model.reshedulehistory.Count >0)
dbCommand.Parameters.AddWithValue("pvar_reshedulehistory",NpgsqlDbType.Json,JsonConvert.SerializeObject(model.reshedulehistory));
else
dbCommand.Parameters.AddWithValue("pvar_reshedulehistory",NpgsqlDbType.Json,DBNull.Value);
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
public virtual string  Remove_Clinical_Appointment(string id,string loginUserID)
			  { 
				  String ResponseMessage="";
					try{ 
							using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
							{
								npsql.Open();
								using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"Remove_Clinical_Appointment\"(@pvar_clinicalappointmentid,@pvar_modifieduser)", npsql))
								{
										dbCommand.CommandType = CommandType.Text;
										dbCommand.Parameters.AddWithValue("pvar_clinicalappointmentid",(object)id??DBNull.Value);
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
public virtual JObject Clinical_Appointment_List(string tenantid
,string patient
,string practitioner
,string appointmentdate_automatonfrom
,string appointmentdate_automatonto
, int? pagesize=1000 , int? pagenumber=0,string searchterm="",string  sort_fields="")
			  { 
				  object dalResponse = null;
			
					try{
 
							using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
							{
								
								using (var dbCommand = new NpgsqlCommand("SELECT * FROM\"Clinical_Appointment_List\"(@pvar_tenantid,@pvar_patient,@pvar_practitioner,@pvar_appointmentdate_automatonfrom,@pvar_appointmentdate_automatonto,@pvar_pagesize,@pvar_pagenumber,@pvar_searchterm,@pvar_sort_fields)", npsql))
								{
									dbCommand.CommandType = CommandType.Text;
									dbCommand.Parameters.AddWithValue("pvar_tenantid",(object)tenantid??DBNull.Value);
dbCommand.Parameters.AddWithValue("pvar_patient",(object)patient??DBNull.Value);
dbCommand.Parameters.AddWithValue("pvar_practitioner",(object)practitioner??DBNull.Value);
dbCommand.Parameters.AddWithValue("pvar_appointmentdate_automatonfrom",(object)appointmentdate_automatonfrom??DBNull.Value);
dbCommand.Parameters.AddWithValue("pvar_appointmentdate_automatonto",(object)appointmentdate_automatonto??DBNull.Value);

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
			   
			 
public virtual System.Data.DataTable get_all_ClinicalAppointment(string tenantid,string searchterm="", int? pagesize=1000, int? pagenumber=0)
			  { 

				    DataTable dataTable = new DataTable();
					DataSet dataSet = new DataSet();

					try{
 
							using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
							{
								npsql.Open();
								using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"get_all_ClinicalAppointment\"(@pvar_tenantid)", npsql))
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
public virtual System.Data.DataTable getById_allinfo_ClinicalAppointment(string ClinicalAppointmentid)
			 {
				DataSet dataSet=new DataSet();
				DataTable dataTable = new DataTable();
				try{
					     
						using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
						{
							npsql.Open();
							using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"getById_sp_all_ClinicalAppointment\"(@pvar_clinicalappointmentid)", npsql))
							{
								dbCommand.CommandType = CommandType.Text;
								dbCommand.Parameters.AddWithValue("pvar_clinicalappointmentid",(object)ClinicalAppointmentid??DBNull.Value);
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
			  
public virtual System.Data.DataTable lookup_ClinicalAppointment_patient(String tenantid,string searchterm="", int? pagesize=1000, int? pagenumber=0)
							        {
                                            DataSet dataSet = new DataSet();
									        DataTable dataTable=new DataTable();
									        try{

                                        		        using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
					                                    {
						                                    npsql.Open();
						                                    using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"lookup_ClinicalAppointment_patient\"(@pvar_tenantid,@pvar_searchterm,@pvar_pagesize,@pvar_pagenumber)", npsql))
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
public virtual System.Data.DataTable lookup_ClinicalAppointment_practitioner(String tenantid,string searchterm="", int? pagesize=1000, int? pagenumber=0)
							        {
                                            DataSet dataSet = new DataSet();
									        DataTable dataTable=new DataTable();
									        try{

                                        		        using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
					                                    {
						                                    npsql.Open();
						                                    using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"lookup_ClinicalAppointment_practitioner\"(@pvar_tenantid,@pvar_searchterm,@pvar_pagesize,@pvar_pagenumber)", npsql))
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
public virtual System.Data.DataTable lookup_ClinicalAppointment_actualpractitioner(String tenantid,string searchterm="", int? pagesize=1000, int? pagenumber=0)
							        {
                                            DataSet dataSet = new DataSet();
									        DataTable dataTable=new DataTable();
									        try{

                                        		        using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
					                                    {
						                                    npsql.Open();
						                                    using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"lookup_ClinicalAppointment_actualpractitioner\"(@pvar_tenantid,@pvar_searchterm,@pvar_pagesize,@pvar_pagenumber)", npsql))
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


						public virtual System.Data.DataTable lookup_change_ClinicalAppointment_practitioner(string Peopleid)
						{
							DataTable dataTable=new DataTable();
                            DataSet dataSet=new DataSet();
							try
							{
								 	 
                                    using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
									{
										npsql.Open();
										using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"lookup_change_ClinicalAppointment_practitioner\"(@pvar_peopleid)", npsql))
										{
											dbCommand.CommandType = CommandType.Text;
											dbCommand.Parameters.AddWithValue("pvar_peopleid",NpgsqlDbType.Varchar,(object)Peopleid??DBNull.Value);
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





		public virtual string UpdateAppointmentStatus(string ClinicalAppointmentid, string status, string modifieduser)
		{
			String ResponseMessage = "";
			try
			{
				using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
				{
					npsql.Open();
					using (var dbCommand = new NpgsqlCommand(
						"SELECT * FROM \"UpdateAppointmentStatus\"(@pvar_clinicalappointmentid,@pvar_status,@pvar_modifieduser)", npsql))
					{
						dbCommand.CommandType = CommandType.Text;
						Guid appointmentId = Guid.Parse(ClinicalAppointmentid);
						Guid modifiedUserId = Guid.Parse(modifieduser);
						dbCommand.Parameters.AddWithValue("pvar_clinicalappointmentid", NpgsqlDbType.Uuid, appointmentId);
						dbCommand.Parameters.AddWithValue("pvar_status", NpgsqlDbType.Varchar, status ?? (object)DBNull.Value);
						dbCommand.Parameters.AddWithValue("pvar_modifieduser", NpgsqlDbType.Uuid, modifiedUserId);

						
						NpgsqlParameter outParm = new NpgsqlParameter("@returnMessage", NpgsqlDbType.Varchar)
						{
							Direction = ParameterDirection.Output
						};
						dbCommand.Parameters.Add(outParm);

						dbCommand.ExecuteNonQuery();
						ResponseMessage = outParm.Value.ToString();

						if (dbCommand.Connection.State != ConnectionState.Closed)
							dbCommand.Connection.Dispose();
					}
					npsql.Close();
				}
			}
			catch (Exception ex)
			{
				ResponseMessage = ex.Message;
				Console.WriteLine(ex);
			}

			return ResponseMessage;
		}


		public virtual string Cancel_IPD_Appointments_By_Booking(string IPDApplicationFormid, string modifieduser)
		{
			string ResponseMessage = "201.1";
			try
			{
				using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
				{
					npsql.Open();
					using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"Cancel_IPD_Appointments_By_Booking\"(@pvar_ipdapplicationformid,@pvar_modifieduser)", npsql))
					{
						dbCommand.CommandType = CommandType.Text;
						dbCommand.Parameters.AddWithValue("pvar_ipdapplicationformid", NpgsqlDbType.Varchar, (object)IPDApplicationFormid ?? DBNull.Value);
						dbCommand.Parameters.AddWithValue("pvar_modifieduser", NpgsqlDbType.Uuid, string.IsNullOrWhiteSpace(modifieduser) ? (object)DBNull.Value : Guid.Parse(modifieduser));

						NpgsqlParameter outParm = new NpgsqlParameter("@returnMessage", NpgsqlDbType.Varchar)
						{
							Direction = ParameterDirection.Output
						};
						dbCommand.Parameters.Add(outParm);

						dbCommand.ExecuteNonQuery();
						ResponseMessage = outParm.Value.ToString();

						if (dbCommand.Connection.State != ConnectionState.Closed)
							dbCommand.Connection.Dispose();
					}
					npsql.Close();
				}
			}
			catch (Exception ex)
			{
				ResponseMessage = ex.Message;
				Console.WriteLine(ex);
			}

			return ResponseMessage;
		}



		public virtual System.Data.DataTable get_all_TaskType(string tenantid)
		{

			DataTable dataTable = new DataTable();
			DataSet dataSet = new DataSet();

			try
			{

				using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
				{
					npsql.Open();
					using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"lookup_ClinicalAppointment_task\"(@pvar_tenantid)", npsql))
					{
						dbCommand.CommandType = CommandType.Text;
						dbCommand.Parameters.AddWithValue("pvar_tenantid", (object)tenantid ?? DBNull.Value);

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
			catch
			{
				throw;
			}
			return dataTable;




		}


        public virtual DataTable Get_Clinical_Slot_Data(
    string tenantid,
    string fromdate,
    string todate)
        {
            DataTable dataTable = new DataTable();

            try
            {
                using (NpgsqlConnection npsql =
                    new NpgsqlConnection(db_connectionstring))
                {
                    npsql.Open();

                    using (var dbCommand = new NpgsqlCommand(
                        @"SELECT *
                  FROM public.""Get_Clinical_Slot_Data""(
                      @pvar_tenantid,
                      @pvar_fromdate,
                      @pvar_todate
                  )",
                        npsql))
                    {
                        dbCommand.CommandType =
                            CommandType.Text;


                        Guid tenantGuid;

                        dbCommand.Parameters.AddWithValue(
                            "pvar_tenantid",
                            NpgsqlDbType.Uuid,
                            Guid.TryParse(
                                tenantid,
                                out tenantGuid
                            )
                                ? (object)tenantGuid
                                : DBNull.Value
                        );


                        DateTime fromDateValue;

                        dbCommand.Parameters.AddWithValue(
                            "pvar_fromdate",
                            NpgsqlDbType.Date,
                            DateTime.TryParse(
                                fromdate,
                                out fromDateValue
                            )
                                ? (object)fromDateValue.Date
                                : DBNull.Value
                        );


                        DateTime toDateValue;

                        dbCommand.Parameters.AddWithValue(
                            "pvar_todate",
                            NpgsqlDbType.Date,
                            DateTime.TryParse(
                                todate,
                                out toDateValue
                            )
                                ? (object)toDateValue.Date
                                : DBNull.Value
                        );


                        using (
                            NpgsqlDataAdapter dataAdapter =
                                new NpgsqlDataAdapter(
                                    dbCommand
                                )
                        )
                        {
                            dataAdapter.Fill(
                                dataTable
                            );
                        }
                    }
                }
            }
            catch
            {
                throw;
            }

            return dataTable;
        }

        public virtual DataTable Get_Clinical_Slot_Doctor_Schedule(
    string tenantid,
    string selecteddate)
        {
            DataTable dt = new DataTable();

            using (NpgsqlConnection connection =
                new NpgsqlConnection(db_connectionstring))
            {
                connection.Open();

                using (NpgsqlCommand command =
                    new NpgsqlCommand(
                        @"SELECT *
                  FROM public.""Get_Clinical_Slot_Doctor_Schedule""
                  (
                      @pvar_tenantid,
                      @pvar_selecteddate
                  );",
                        connection))
                {
                    Guid tenantGuid;

                    if (Guid.TryParse(
                        tenantid,
                        out tenantGuid))
                    {
                        command.Parameters.AddWithValue(
                            "pvar_tenantid",
                            NpgsqlTypes.NpgsqlDbType.Uuid,
                            tenantGuid
                        );
                    }
                    else
                    {
                        command.Parameters.AddWithValue(
                            "pvar_tenantid",
                            NpgsqlTypes.NpgsqlDbType.Uuid,
                            DBNull.Value
                        );
                    }


                    DateTime selectedDate;

                    if (!DateTime.TryParse(
                        selecteddate,
                        out selectedDate))
                    {
                        selectedDate =
                            DateTime.Today;
                    }


                    command.Parameters.AddWithValue(
                        "pvar_selecteddate",
                        NpgsqlTypes.NpgsqlDbType.Date,
                        selectedDate.Date
                    );


                    using (NpgsqlDataAdapter adapter =
                        new NpgsqlDataAdapter(command))
                    {
                        adapter.Fill(dt);
                    }
                }
            }

            return dt;
        }
    }


}
