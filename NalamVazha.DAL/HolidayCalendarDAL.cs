namespace NalamVazha.DAL{
			    using EncrypDecrypt;
			    using NalamVazha.Models;
			    using Newtonsoft.Json;
				using Newtonsoft.Json.Linq;
                using Npgsql;
				using NpgsqlTypes;
			    using System;
    using System.Collections.Generic;
			    using System.Data;
			    using System.Data.Common;
				using System.Linq;
			    using System.Text;
				using System.Text.RegularExpressions;
    using System.Threading.Tasks;

    //This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/30/2026 12:48:26
			    public class HolidayCalendarDAL
			    {
					public virtual string db_connectionstring{get;set;}
					
			 	    public HolidayCalendarDAL(string connectionString)
				    {
						
					    db_connectionstring=connectionString;
				    }
				  
			        public virtual System.Data.DataTable getById_taskallowed(string HolidayCalendarid)
			 {
					DataTable dataTable=new DataTable();
					DataSet dataSet=new DataSet();
					try
					{
						  
							using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
							{
								npsql.Open();
								using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"getById_sp_HolidayCalendar_taskallowed\"(@pvar_holidaycalendarid)", npsql))
								{
									dbCommand.CommandType = CommandType.Text;
									dbCommand.Parameters.AddWithValue("pvar_holidaycalendarid",(object)HolidayCalendarid??DBNull.Value);
								
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


              public virtual string Add_Holiday(HolidayCalendarModel model)
			  { 
				  String ResponseMessage="";
					 
					try{
							 
                            using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
					        {
						        npsql.Open();
						        using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"Add_Holiday\"(@pvar_holidaycalendarid,@pvar_tenantid,@pvar_holidayname,@pvar_holidaydate,@pvar_taskallowed,@pvar_createduser)", npsql))
						        {
                                        dbCommand.CommandType = CommandType.Text;
						            	
								        					dbCommand.Parameters.AddWithValue("pvar_holidaycalendarid",NpgsqlDbType.Uuid,(object)model.HolidayCalendarid??DBNull.Value);
dbCommand.Parameters.AddWithValue("pvar_tenantid",NpgsqlDbType.Uuid,(object)model.tenantid??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_holidayname",NpgsqlDbType.Varchar,(object)model.holidayname??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_holidaydate",NpgsqlDbType.Date,(object)model.holidaydate??DBNull.Value);
if(model.taskallowed !=null  && model.taskallowed.Count >0)
dbCommand.Parameters.AddWithValue("pvar_taskallowed",NpgsqlDbType.Json,JsonConvert.SerializeObject(model.taskallowed));
else
dbCommand.Parameters.AddWithValue("pvar_taskallowed",NpgsqlDbType.Json,DBNull.Value);
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
public virtual HolidayCalendarModel getById_HolidayCalendar(string HolidayCalendarid)
									 {
										DataTable dataTable = new DataTable();
										DataSet dataSet = new DataSet();
										try{
												 
												using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
												{
													npsql.Open();
													using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"getById_sp_HolidayCalendar\"(@pvar_holidaycalendarid)", npsql))
													{
														dbCommand.CommandType = CommandType.Text;
														dbCommand.Parameters.AddWithValue("pvar_holidaycalendarid",(object)HolidayCalendarid??DBNull.Value);
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
											return ModelConverter.ConvertDataRowToModel<HolidayCalendarModel>(row);
										}
										else
										{
											return null;
										}
									 }
			 public virtual string  Update_Holiday(HolidayCalendarModel model)
			 { 
				 String ResponseMessage="";
					try{
						 	 
							using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
							{
								npsql.Open();
								using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"Update_Holiday\"(@pvar_holidaycalendarid,@pvar_tenantid,@pvar_holidayname,@pvar_holidaydate,@pvar_taskallowed,@pvar_modifieduser)", npsql))
								{
										dbCommand.CommandType = CommandType.Text;
															dbCommand.Parameters.AddWithValue("pvar_holidaycalendarid",NpgsqlDbType.Uuid,(object)model.HolidayCalendarid??DBNull.Value);
dbCommand.Parameters.AddWithValue("pvar_tenantid",NpgsqlDbType.Uuid,(object)model.tenantid??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_holidayname",NpgsqlDbType.Varchar,(object)model.holidayname??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_holidaydate",NpgsqlDbType.Date,(object)model.holidaydate??DBNull.Value);
if(model.taskallowed !=null  && model.taskallowed.Count >0)
dbCommand.Parameters.AddWithValue("pvar_taskallowed",NpgsqlDbType.Json,JsonConvert.SerializeObject(model.taskallowed));
else
dbCommand.Parameters.AddWithValue("pvar_taskallowed",NpgsqlDbType.Json,DBNull.Value);
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
public virtual string  Remove_Holiday(string id,string loginUserID)
			  { 
				  String ResponseMessage="";
					try{ 
							using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
							{
								npsql.Open();
								using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"Remove_Holiday\"(@pvar_holidaycalendarid,@pvar_modifieduser)", npsql))
								{
										dbCommand.CommandType = CommandType.Text;
										dbCommand.Parameters.AddWithValue("pvar_holidaycalendarid",(object)id??DBNull.Value);
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
public virtual System.Data.DataTable Holiday_Calendar(string tenantid
,string holidaydate_automatonfrom
,string holidaydate_automatonto
)
			  { 
					DataTable dataTable = new DataTable();
                DataSet dataSet = new DataSet(); 

					try{
 
							using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
							{
								npsql.Open();
								using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"Holiday_Calendar\"(@pvar_tenantid,@pvar_holidaydate_automatonfrom,@pvar_holidaydate_automatonto)", npsql))
								{
									dbCommand.CommandType = CommandType.Text;
									dbCommand.Parameters.AddWithValue("pvar_tenantid",(object)tenantid??DBNull.Value);
dbCommand.Parameters.AddWithValue("pvar_holidaydate_automatonfrom",(object)holidaydate_automatonfrom??DBNull.Value);
dbCommand.Parameters.AddWithValue("pvar_holidaydate_automatonto",(object)holidaydate_automatonto??DBNull.Value);

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
			   
			 
public virtual System.Data.DataTable get_all_HolidayCalendar(string tenantid)
			  { 

				    DataTable dataTable = new DataTable();
					DataSet dataSet = new DataSet();

					try{
 
							using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
							{
								npsql.Open();
								using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"get_all_HolidayCalendar\"(@pvar_tenantid)", npsql))
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
public virtual System.Data.DataTable getById_allinfo_HolidayCalendar(string HolidayCalendarid)
			 {
				DataSet dataSet=new DataSet();
				DataTable dataTable = new DataTable();
				try{
					     
						using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
						{
							npsql.Open();
							using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"getById_sp_all_HolidayCalendar\"(@pvar_holidaycalendarid)", npsql))
							{
								dbCommand.CommandType = CommandType.Text;
								dbCommand.Parameters.AddWithValue("pvar_holidaycalendarid",(object)HolidayCalendarid??DBNull.Value);
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
			  
public virtual System.Data.DataTable lookup_HolidayCalendar_taskallowed_taskname(String tenantid)
							        {
                                            DataSet dataSet = new DataSet();
									        DataTable dataTable=new DataTable();
									        try{

                                        		        using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
					                                    {
						                                    npsql.Open();
						                                    using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"lookup_HolidayCalendar_taskallowed_taskname\"(@pvar_tenantid)", npsql))
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

public virtual System.Data.DataTable Get_Holiday_Blocked_Dates(string tenantid, string taskid = "", string taskname = "", string datefrom = "", string dateto = "")
							  {
								  DataTable dataTable = new DataTable();
								  DataSet dataSet = new DataSet();
								  try
								  {
									  using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
									  {
										  npsql.Open();
										  using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"Get_Holiday_Blocked_Dates\"(@pvar_tenantid,@pvar_taskid,@pvar_taskname,@pvar_datefrom,@pvar_dateto)", npsql))
										  {
											  dbCommand.CommandType = CommandType.Text;
											  dbCommand.Parameters.AddWithValue("pvar_tenantid", NpgsqlDbType.Uuid, string.IsNullOrWhiteSpace(tenantid) || !Guid.TryParse(tenantid.Split('|').LastOrDefault(), out var tenantGuid) ? (object)DBNull.Value : tenantGuid);
											  dbCommand.Parameters.AddWithValue("pvar_taskid", NpgsqlDbType.Uuid, string.IsNullOrWhiteSpace(taskid) || !Guid.TryParse(taskid, out var taskGuid) ? (object)DBNull.Value : taskGuid);
											  dbCommand.Parameters.AddWithValue("pvar_taskname", NpgsqlDbType.Varchar, (object)(taskname ?? "") ?? DBNull.Value);
											  dbCommand.Parameters.AddWithValue("pvar_datefrom", NpgsqlDbType.Date, string.IsNullOrWhiteSpace(datefrom) || !DateTime.TryParse(datefrom, out var fromDate) ? (object)DBNull.Value : fromDate.Date);
											  dbCommand.Parameters.AddWithValue("pvar_dateto", NpgsqlDbType.Date, string.IsNullOrWhiteSpace(dateto) || !DateTime.TryParse(dateto, out var toDate) ? (object)DBNull.Value : toDate.Date);
											  using (NpgsqlDataAdapter dataAdapter = new NpgsqlDataAdapter(dbCommand))
											  {
												  dataSet.Reset();
												  dataAdapter.Fill(dataSet);
												  dataTable = dataSet.Tables[0];
												  if (dbCommand.Connection.State != ConnectionState.Closed)
													  dbCommand.Connection.Dispose();
											  }
										  }
										  npsql.Close();
									  }
								  }
								  catch { throw; }
								  return dataTable;
							  }

        public async Task<List<CalendarHolidayDto>>
    Get_Holiday_By_Date(
        Guid tenantid,
        DateTime datefrom,
        DateTime dateto)
        {
            var result =
                new List<CalendarHolidayDto>();


            using (var connection =
                new NpgsqlConnection(db_connectionstring))
            {
                await connection.OpenAsync();


                using (var command =
                    new NpgsqlCommand(
                        @"
                SELECT
                    holidaydate,
                    holidayname

                FROM public.""Get_Holiday_By_Date""(
                    @pvar_tenantid,
                    @pvar_datefrom,
                    @pvar_dateto
                );
                ",
                        connection))
                {

                    command.Parameters.Add(
                        "pvar_tenantid",
                        NpgsqlTypes.NpgsqlDbType.Uuid
                    ).Value = tenantid;


                    command.Parameters.Add(
                        "pvar_datefrom",
                        NpgsqlTypes.NpgsqlDbType.Date
                    ).Value = datefrom.Date;


                    command.Parameters.Add(
                        "pvar_dateto",
                        NpgsqlTypes.NpgsqlDbType.Date
                    ).Value = dateto.Date;


                    using (var reader =
                        await command.ExecuteReaderAsync())
                    {
                        while (
                            await reader.ReadAsync()
                        )
                        {
                            result.Add(
                                new CalendarHolidayDto
                                {
                                    holidaydate =
                                        reader.GetDateTime(
                                            reader.GetOrdinal(
                                                "holidaydate"
                                            )
                                        ),

                                    holidayname =
                                        reader["holidayname"]
                                            ?.ToString() ?? ""
                                }
                            );
                        }
                    }
                }
            }


            return result;
        }
    }


			    }
