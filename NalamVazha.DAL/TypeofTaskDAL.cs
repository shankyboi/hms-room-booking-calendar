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

			    //This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 08/03/2026 08:38:48
			    public class TypeofTaskDAL
			    {
					public virtual string db_connectionstring{get;set;}
					
			 	    public TypeofTaskDAL(string connectionString)
				    {
						
					    db_connectionstring=connectionString;
				    }
				  
			        
              public virtual string Add_Type_of_Task(TypeofTaskModel model)
			  { 
				  String ResponseMessage="";
					 
					try{
							 
                            using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
					        {
						        npsql.Open();
						        using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"Add_Type_of_Task\"(@pvar_typeoftaskid,@pvar_tenantid,@pvar_tasktype,@pvar_createduser)", npsql))
						        {
                                        dbCommand.CommandType = CommandType.Text;
						            	
								        					dbCommand.Parameters.AddWithValue("pvar_typeoftaskid",NpgsqlDbType.Uuid,(object)model.TypeofTaskid??DBNull.Value);
dbCommand.Parameters.AddWithValue("pvar_tenantid",NpgsqlDbType.Uuid,(object)model.tenantid??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_tasktype",NpgsqlDbType.Varchar,(object)model.tasktype??DBNull.Value);
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
public virtual TypeofTaskModel getById_TypeofTask(string TypeofTaskid)
									 {
										DataTable dataTable = new DataTable();
										DataSet dataSet = new DataSet();
										try{
												 
												using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
												{
													npsql.Open();
													using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"getById_sp_TypeofTask\"(@pvar_typeoftaskid)", npsql))
													{
														dbCommand.CommandType = CommandType.Text;
														dbCommand.Parameters.AddWithValue("pvar_typeoftaskid",(object)TypeofTaskid??DBNull.Value);
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
											return ModelConverter.ConvertDataRowToModel<TypeofTaskModel>(row);
										}
										else
										{
											return null;
										}
									 }
			 public virtual string  Update_Type_of_Task(TypeofTaskModel model)
			 { 
				 String ResponseMessage="";
					try{
						 	 
							using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
							{
								npsql.Open();
								using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"Update_Type_of_Task\"(@pvar_typeoftaskid,@pvar_tenantid,@pvar_tasktype,@pvar_modifieduser)", npsql))
								{
										dbCommand.CommandType = CommandType.Text;
															dbCommand.Parameters.AddWithValue("pvar_typeoftaskid",NpgsqlDbType.Uuid,(object)model.TypeofTaskid??DBNull.Value);
dbCommand.Parameters.AddWithValue("pvar_tenantid",NpgsqlDbType.Uuid,(object)model.tenantid??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_tasktype",NpgsqlDbType.Varchar,(object)model.tasktype??DBNull.Value);
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
public virtual string  Remove_Type_of_Task(string id,string loginUserID)
			  { 
				  String ResponseMessage="";
					try{ 
							using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
							{
								npsql.Open();
								using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"Remove_Type_of_Task\"(@pvar_typeoftaskid,@pvar_modifieduser)", npsql))
								{
										dbCommand.CommandType = CommandType.Text;
										dbCommand.Parameters.AddWithValue("pvar_typeoftaskid",(object)id??DBNull.Value);
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
public virtual JObject Type_of_Task_List(string tenantid
, int? pagesize=1000 , int? pagenumber=0,string searchterm="",string  sort_fields="")
			  { 
				  object dalResponse = null;
			
					try{
 
							using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
							{
								
								using (var dbCommand = new NpgsqlCommand("SELECT * FROM\"Type_of_Task_List\"(@pvar_tenantid,@pvar_pagesize,@pvar_pagenumber,@pvar_searchterm,@pvar_sort_fields)", npsql))
								{
									dbCommand.CommandType = CommandType.Text;
									dbCommand.Parameters.AddWithValue("pvar_tenantid",(object)tenantid??DBNull.Value);

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
			   
			 
public virtual System.Data.DataTable get_all_TypeofTask(string tenantid,string searchterm="", int? pagesize=1000, int? pagenumber=0)
			  { 

				    DataTable dataTable = new DataTable();
					DataSet dataSet = new DataSet();

					try{
 
							using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
							{
								npsql.Open();
								using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"get_all_TypeofTask\"(@pvar_tenantid)", npsql))
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
public virtual System.Data.DataTable getById_allinfo_TypeofTask(string TypeofTaskid)
			 {
				DataSet dataSet=new DataSet();
				DataTable dataTable = new DataTable();
				try{
					     
						using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
						{
							npsql.Open();
							using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"getById_sp_all_TypeofTask\"(@pvar_typeoftaskid)", npsql))
							{
								dbCommand.CommandType = CommandType.Text;
								dbCommand.Parameters.AddWithValue("pvar_typeoftaskid",(object)TypeofTaskid??DBNull.Value);
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
