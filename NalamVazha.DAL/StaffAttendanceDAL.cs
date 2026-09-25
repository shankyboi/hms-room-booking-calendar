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

			    //This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:43:25
			    public class StaffAttendanceDAL
			    {
					public virtual string db_connectionstring{get;set;}
					
			 	    public StaffAttendanceDAL(string connectionString)
				    {
						
					    db_connectionstring=connectionString;
				    }
				  
			        
              public virtual string Add_Staff_Attendance(StaffAttendanceModel model)
			  { 
				  String ResponseMessage="";
					 
					try{
							 
                            using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
					        {
						        npsql.Open();
						        using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"Add_Staff_Attendance\"(@pvar_staffattendanceid,@pvar_tenantid,@pvar_shiftdate,@pvar_shift,@pvar_shiftstarttime,@pvar_shiftendtime,@pvar_shifthours,@pvar_workprofile,@pvar_peoplename,@pvar_punchdateandtime,@pvar_earlyinmin,@pvar_earlyoutmin,@pvar_latemin,@pvar_workhours,@pvar_createduser)", npsql))
						        {
                                        dbCommand.CommandType = CommandType.Text;
						            	
								        					dbCommand.Parameters.AddWithValue("pvar_staffattendanceid",NpgsqlDbType.Uuid,(object)model.StaffAttendanceid??DBNull.Value);
dbCommand.Parameters.AddWithValue("pvar_tenantid",NpgsqlDbType.Uuid,(object)model.tenantid??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_shiftdate",NpgsqlDbType.Date,(object)model.shiftdate??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_shift",NpgsqlDbType.Uuid,(object)model.shift??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_shiftstarttime",NpgsqlDbType.Varchar,(object)model.shiftstarttime??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_shiftendtime",NpgsqlDbType.Varchar,(object)model.shiftendtime??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_shifthours",NpgsqlDbType.Numeric,(object)model.shifthours??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_workprofile",NpgsqlDbType.Uuid,(object)model.workprofile??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_peoplename",NpgsqlDbType.Uuid,(object)model.peoplename??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_punchdateandtime",NpgsqlDbType.Timestamp,(object)model.punchdateandtime??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_earlyinmin",NpgsqlDbType.Numeric,(object)model.earlyinmin??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_earlyoutmin",NpgsqlDbType.Numeric,(object)model.earlyoutmin??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_latemin",NpgsqlDbType.Numeric,(object)model.latemin??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_workhours",NpgsqlDbType.Integer,(object)model.workhours??DBNull.Value);
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
public virtual StaffAttendanceModel getById_StaffAttendance(string StaffAttendanceid)
									 {
										DataTable dataTable = new DataTable();
										DataSet dataSet = new DataSet();
										try{
												 
												using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
												{
													npsql.Open();
													using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"getById_sp_StaffAttendance\"(@pvar_staffattendanceid)", npsql))
													{
														dbCommand.CommandType = CommandType.Text;
														dbCommand.Parameters.AddWithValue("pvar_staffattendanceid",(object)StaffAttendanceid??DBNull.Value);
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
											return ModelConverter.ConvertDataRowToModel<StaffAttendanceModel>(row);
										}
										else
										{
											return null;
										}
									 }
			 public virtual string  Update_Staff_Attendance(StaffAttendanceModel model)
			 { 
				 String ResponseMessage="";
					try{
						 	 
							using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
							{
								npsql.Open();
								using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"Update_Staff_Attendance\"(@pvar_staffattendanceid,@pvar_tenantid,@pvar_shiftdate,@pvar_shift,@pvar_shiftstarttime,@pvar_shiftendtime,@pvar_shifthours,@pvar_workprofile,@pvar_peoplename,@pvar_punchdateandtime,@pvar_earlyinmin,@pvar_earlyoutmin,@pvar_latemin,@pvar_workhours,@pvar_modifieduser)", npsql))
								{
										dbCommand.CommandType = CommandType.Text;
															dbCommand.Parameters.AddWithValue("pvar_staffattendanceid",NpgsqlDbType.Uuid,(object)model.StaffAttendanceid??DBNull.Value);
dbCommand.Parameters.AddWithValue("pvar_tenantid",NpgsqlDbType.Uuid,(object)model.tenantid??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_shiftdate",NpgsqlDbType.Date,(object)model.shiftdate??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_shift",NpgsqlDbType.Uuid,(object)model.shift??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_shiftstarttime",NpgsqlDbType.Varchar,(object)model.shiftstarttime??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_shiftendtime",NpgsqlDbType.Varchar,(object)model.shiftendtime??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_shifthours",NpgsqlDbType.Numeric,(object)model.shifthours??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_workprofile",NpgsqlDbType.Uuid,(object)model.workprofile??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_peoplename",NpgsqlDbType.Uuid,(object)model.peoplename??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_punchdateandtime",NpgsqlDbType.Timestamp,(object)model.punchdateandtime??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_earlyinmin",NpgsqlDbType.Numeric,(object)model.earlyinmin??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_earlyoutmin",NpgsqlDbType.Numeric,(object)model.earlyoutmin??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_latemin",NpgsqlDbType.Numeric,(object)model.latemin??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_workhours",NpgsqlDbType.Integer,(object)model.workhours??DBNull.Value);
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
public virtual string  Remove_Staff_Attendance(string id,string loginUserID)
			  { 
				  String ResponseMessage="";
					try{ 
							using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
							{
								npsql.Open();
								using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"Remove_Staff_Attendance\"(@pvar_staffattendanceid,@pvar_modifieduser)", npsql))
								{
										dbCommand.CommandType = CommandType.Text;
										dbCommand.Parameters.AddWithValue("pvar_staffattendanceid",(object)id??DBNull.Value);
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
public virtual JObject Staff_Attendance_List(string tenantid
,string shiftdate_automatonfrom
,string shiftdate_automatonto
,string peoplename
, int? pagesize=1000 , int? pagenumber=0,string searchterm="",string  sort_fields="")
			  { 
				  object dalResponse = null;
			
					try{
 
							using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
							{
								
								using (var dbCommand = new NpgsqlCommand("SELECT * FROM\"Staff_Attendance_List\"(@pvar_tenantid,@pvar_shiftdate_automatonfrom,@pvar_shiftdate_automatonto,@pvar_peoplename,@pvar_pagesize,@pvar_pagenumber,@pvar_searchterm,@pvar_sort_fields)", npsql))
								{
									dbCommand.CommandType = CommandType.Text;
									dbCommand.Parameters.AddWithValue("pvar_tenantid",(object)tenantid??DBNull.Value);
dbCommand.Parameters.AddWithValue("pvar_shiftdate_automatonfrom",(object)shiftdate_automatonfrom??DBNull.Value);
dbCommand.Parameters.AddWithValue("pvar_shiftdate_automatonto",(object)shiftdate_automatonto??DBNull.Value);
dbCommand.Parameters.AddWithValue("pvar_peoplename",(object)peoplename??DBNull.Value);

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
			   
			 
public virtual System.Data.DataTable get_all_StaffAttendance(string tenantid,string searchterm="", int? pagesize=1000, int? pagenumber=0)
			  { 

				    DataTable dataTable = new DataTable();
					DataSet dataSet = new DataSet();

					try{
 
							using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
							{
								npsql.Open();
								using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"get_all_StaffAttendance\"(@pvar_tenantid)", npsql))
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
public virtual System.Data.DataTable getById_allinfo_StaffAttendance(string StaffAttendanceid)
			 {
				DataSet dataSet=new DataSet();
				DataTable dataTable = new DataTable();
				try{
					     
						using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
						{
							npsql.Open();
							using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"getById_sp_all_StaffAttendance\"(@pvar_staffattendanceid)", npsql))
							{
								dbCommand.CommandType = CommandType.Text;
								dbCommand.Parameters.AddWithValue("pvar_staffattendanceid",(object)StaffAttendanceid??DBNull.Value);
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
			  
public virtual System.Data.DataTable lookup_StaffAttendance_shift(String tenantid,string searchterm="", int? pagesize=1000, int? pagenumber=0)
							        {
                                            DataSet dataSet = new DataSet();
									        DataTable dataTable=new DataTable();
									        try{

                                        		        using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
					                                    {
						                                    npsql.Open();
						                                    using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"lookup_StaffAttendance_shift\"(@pvar_tenantid,@pvar_searchterm,@pvar_pagesize,@pvar_pagenumber)", npsql))
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
public virtual System.Data.DataTable lookup_StaffAttendance_workprofile(String tenantid)
							        {
                                            DataSet dataSet = new DataSet();
									        DataTable dataTable=new DataTable();
									        try{

                                        		        using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
					                                    {
						                                    npsql.Open();
						                                    using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"lookup_StaffAttendance_workprofile\"(@pvar_tenantid)", npsql))
						                                    {
                                            
                                                                dbCommand.Parameters.AddWithValue("pvar_tenantid",(object)tenantid??DBNull.Value);  
																
																 
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
public virtual System.Data.DataTable lookup_StaffAttendance_peoplename(String tenantid,String workprofile,string searchterm="", int? pagesize=1000, int? pagenumber=0)
							        {
                                            DataSet dataSet = new DataSet();
									        DataTable dataTable=new DataTable();
									        try{

                                        		        using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
					                                    {
						                                    npsql.Open();
						                                    using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"lookup_StaffAttendance_peoplename\"(@pvar_tenantid,@pvar_workprofile,@pvar_searchterm,@pvar_pagesize,@pvar_pagenumber)", npsql))
						                                    {
                                            
                                                                dbCommand.Parameters.AddWithValue("pvar_tenantid",(object)tenantid??DBNull.Value);dbCommand.Parameters.AddWithValue("pvar_workprofile",(object)workprofile??DBNull.Value);  
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


						public virtual System.Data.DataTable lookup_change_StaffAttendance_shift(string Shiftid)
						{
							DataTable dataTable=new DataTable();
                            DataSet dataSet=new DataSet();
							try
							{
								 	 
                                    using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
									{
										npsql.Open();
										using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"lookup_change_StaffAttendance_shift\"(@pvar_shiftid)", npsql))
										{
											dbCommand.CommandType = CommandType.Text;
											dbCommand.Parameters.AddWithValue("pvar_shiftid",NpgsqlDbType.Varchar,(object)Shiftid??DBNull.Value);
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
