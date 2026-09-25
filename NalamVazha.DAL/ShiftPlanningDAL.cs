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

			    //This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:43:20
			    public class ShiftPlanningDAL
			    {
					public virtual string db_connectionstring{get;set;}
					
			 	    public ShiftPlanningDAL(string connectionString)
				    {
						
					    db_connectionstring=connectionString;
				    }
				  
			        public virtual System.Data.DataTable getById_people(string ShiftPlanningid)
			 {
					DataTable dataTable=new DataTable();
					DataSet dataSet=new DataSet();
					try
					{
						  
							using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
							{
								npsql.Open();
								using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"getById_sp_ShiftPlanning_people\"(@pvar_shiftplanningid)", npsql))
								{
									dbCommand.CommandType = CommandType.Text;
									dbCommand.Parameters.AddWithValue("pvar_shiftplanningid",(object)ShiftPlanningid??DBNull.Value);
								
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


              public virtual string Add_Shift_Planning(ShiftPlanningModel model)
			  { 
				  String ResponseMessage="";
					 
					try{
							 
                            using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
					        {
						        npsql.Open();
						        using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"Add_Shift_Planning\"(@pvar_shiftplanningid,@pvar_tenantid,@pvar_shiftname,@pvar_shiftstarttime,@pvar_shiftendtime,@pvar_validfrom,@pvar_validto,@pvar_people,@pvar_createduser,@pvar_shiftplanning_people_bulkupload)", npsql))
						        {
                                        dbCommand.CommandType = CommandType.Text;
						            	
								        					dbCommand.Parameters.AddWithValue("pvar_shiftplanningid",NpgsqlDbType.Uuid,(object)model.ShiftPlanningid??DBNull.Value);
dbCommand.Parameters.AddWithValue("pvar_tenantid",NpgsqlDbType.Uuid,(object)model.tenantid??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_shiftname",NpgsqlDbType.Uuid,(object)model.shiftname??DBNull.Value);
						dbCommand.Parameters.AddWithValue("pvar_shiftstarttime", NpgsqlDbType.Varchar, (object)model.shiftstarttime ?? DBNull.Value);

						dbCommand.Parameters.AddWithValue("pvar_shiftendtime", NpgsqlDbType.Varchar, (object)model.shiftendtime ?? DBNull.Value);

						dbCommand.Parameters.AddWithValue("pvar_validfrom",NpgsqlDbType.Date,(object)model.validfrom??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_validto",NpgsqlDbType.Date,(object)model.validto??DBNull.Value);
if(model.people !=null  && model.people.Count >0)
dbCommand.Parameters.AddWithValue("pvar_people",NpgsqlDbType.Json,JsonConvert.SerializeObject(model.people));
else
dbCommand.Parameters.AddWithValue("pvar_people",NpgsqlDbType.Json,DBNull.Value);
dbCommand.Parameters.AddWithValue("pvar_createduser",NpgsqlDbType.Uuid,(object)model.createduser??DBNull.Value);	
															if(model.ShiftPlanning_people_bulkupload !=null  && model.ShiftPlanning_people_bulkupload.Count >0)
dbCommand.Parameters.AddWithValue("pvar_shiftplanning_people_bulkupload",NpgsqlDbType.Json,JsonConvert.SerializeObject(model.ShiftPlanning_people_bulkupload));
else
dbCommand.Parameters.AddWithValue("pvar_shiftplanning_people_bulkupload",NpgsqlDbType.Json,DBNull.Value);

										
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
public virtual ShiftPlanningModel getById_ShiftPlanning(string ShiftPlanningid)
									 {
										DataTable dataTable = new DataTable();
										DataSet dataSet = new DataSet();
										try{
												 
												using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
												{
													npsql.Open();
													using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"getById_sp_ShiftPlanning\"(@pvar_shiftplanningid)", npsql))
													{
														dbCommand.CommandType = CommandType.Text;
														dbCommand.Parameters.AddWithValue("pvar_shiftplanningid",(object)ShiftPlanningid??DBNull.Value);
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
											return ModelConverter.ConvertDataRowToModel<ShiftPlanningModel>(row);
										}
										else
										{
											return null;
										}
									 }
			 public virtual string  Update_Shift_Planning(ShiftPlanningModel model)
			 { 
				 String ResponseMessage="";
					try{
						 	 
							using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
							{
								npsql.Open();
								using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"Update_Shift_Planning\"(@pvar_shiftplanningid,@pvar_tenantid,@pvar_shiftname,@pvar_shiftstarttime,@pvar_shiftendtime,@pvar_validfrom,@pvar_validto,@pvar_people,@pvar_modifieduser,@pvar_shiftplanning_people_bulkupload)", npsql))
								{
										dbCommand.CommandType = CommandType.Text;
															dbCommand.Parameters.AddWithValue("pvar_shiftplanningid",NpgsqlDbType.Uuid,(object)model.ShiftPlanningid??DBNull.Value);
dbCommand.Parameters.AddWithValue("pvar_tenantid",NpgsqlDbType.Uuid,(object)model.tenantid??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_shiftname",NpgsqlDbType.Uuid,(object)model.shiftname??DBNull.Value);
						dbCommand.Parameters.AddWithValue("pvar_shiftstarttime", NpgsqlDbType.Varchar, (object)model.shiftstarttime ?? DBNull.Value);

						dbCommand.Parameters.AddWithValue("pvar_shiftendtime", NpgsqlDbType.Varchar, (object)model.shiftendtime ?? DBNull.Value);
						
						dbCommand.Parameters.AddWithValue("pvar_validfrom",NpgsqlDbType.Date,(object)model.validfrom??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_validto",NpgsqlDbType.Date,(object)model.validto??DBNull.Value);
if(model.people !=null  && model.people.Count >0)
dbCommand.Parameters.AddWithValue("pvar_people",NpgsqlDbType.Json,JsonConvert.SerializeObject(model.people));
else
dbCommand.Parameters.AddWithValue("pvar_people",NpgsqlDbType.Json,DBNull.Value);
dbCommand.Parameters.AddWithValue("pvar_modifieduser",NpgsqlDbType.Uuid,model.modifieduser);	
															if(model.ShiftPlanning_people_bulkupload !=null  && model.ShiftPlanning_people_bulkupload.Count >0)
dbCommand.Parameters.AddWithValue("pvar_shiftplanning_people_bulkupload",NpgsqlDbType.Json,JsonConvert.SerializeObject(model.ShiftPlanning_people_bulkupload));
else
dbCommand.Parameters.AddWithValue("pvar_shiftplanning_people_bulkupload",NpgsqlDbType.Json,DBNull.Value);

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
public virtual string  Remove_Shift_Planning(string id,string loginUserID)
			  { 
				  String ResponseMessage="";
					try{ 
							using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
							{
								npsql.Open();
								using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"Remove_Shift_Planning\"(@pvar_shiftplanningid,@pvar_modifieduser)", npsql))
								{
										dbCommand.CommandType = CommandType.Text;
										dbCommand.Parameters.AddWithValue("pvar_shiftplanningid",(object)id??DBNull.Value);
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

              public virtual string Upload_Shift_Planning(ShiftPlanningModel model)
			  { 
				  String ResponseMessage="";
					 
					try{
							 
                            using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
					        {
						        npsql.Open();
						        using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"Upload_Shift_Planning\"(@pvar_ShiftPlanning_bulkupload)", npsql))
						        {
                                        dbCommand.CommandType = CommandType.Text;
				if(model.ShiftPlanning_bulkupload !=null  && model.ShiftPlanning_bulkupload.Count >0)		            	
dbCommand.Parameters.AddWithValue("pvar_ShiftPlanning_bulkupload",NpgsqlDbType.Json,JsonConvert.SerializeObject(model.ShiftPlanning_bulkupload));
else
dbCommand.Parameters.AddWithValue("pvar_ShiftPlanning_bulkupload",NpgsqlDbType.Json,DBNull.Value);
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
public virtual JObject Shift_Planning_List(string tenantid
,string shiftname
, int? pagesize=1000 , int? pagenumber=0,string searchterm="",string  sort_fields="")
			  { 
				  object dalResponse = null;
			
					try{
 
							using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
							{
								
								using (var dbCommand = new NpgsqlCommand("SELECT * FROM\"Shift_Planning_List\"(@pvar_tenantid,@pvar_shiftname,@pvar_pagesize,@pvar_pagenumber,@pvar_searchterm,@pvar_sort_fields)", npsql))
								{
									dbCommand.CommandType = CommandType.Text;
									dbCommand.Parameters.AddWithValue("pvar_tenantid",(object)tenantid??DBNull.Value);
dbCommand.Parameters.AddWithValue("pvar_shiftname",(object)shiftname??DBNull.Value);

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
			   
			 
public virtual System.Data.DataTable get_all_ShiftPlanning(string tenantid,string searchterm="", int? pagesize=1000, int? pagenumber=0)
			  { 

				    DataTable dataTable = new DataTable();
					DataSet dataSet = new DataSet();

					try{
 
							using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
							{
								npsql.Open();
								using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"get_all_ShiftPlanning\"(@pvar_tenantid)", npsql))
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
public virtual System.Data.DataTable getById_allinfo_ShiftPlanning(string ShiftPlanningid)
			 {
				DataSet dataSet=new DataSet();
				DataTable dataTable = new DataTable();
				try{
					     
						using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
						{
							npsql.Open();
							using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"getById_sp_all_ShiftPlanning\"(@pvar_shiftplanningid)", npsql))
							{
								dbCommand.CommandType = CommandType.Text;
								dbCommand.Parameters.AddWithValue("pvar_shiftplanningid",(object)ShiftPlanningid??DBNull.Value);
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
			  
public virtual System.Data.DataTable lookup_ShiftPlanning_shiftname(String tenantid,string searchterm="", int? pagesize=1000, int? pagenumber=0)
							        {
                                            DataSet dataSet = new DataSet();
									        DataTable dataTable=new DataTable();
									        try{

                                        		        using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
					                                    {
						                                    npsql.Open();
						                                    using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"lookup_ShiftPlanning_shiftname\"(@pvar_tenantid,@pvar_searchterm,@pvar_pagesize,@pvar_pagenumber)", npsql))
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
public virtual System.Data.DataTable lookup_ShiftPlanning_people_personname(String tenantid, String workprofile, string searchterm="", int? pagesize=1000, int? pagenumber=0)
							        {
                                            DataSet dataSet = new DataSet();
									        DataTable dataTable=new DataTable();
									        try{

                                        		        using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
					                                    {
						                                    npsql.Open();
						                                    using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"lookup_ShiftPlanning_people_personname\"(@pvar_tenantid,@pvar_workprofile,@pvar_searchterm,@pvar_pagesize,@pvar_pagenumber)", npsql))
						                                    {
                                            
                                                                dbCommand.Parameters.AddWithValue("pvar_tenantid",(object)tenantid??DBNull.Value);
																dbCommand.Parameters.AddWithValue("pvar_workprofile",(object)workprofile??DBNull.Value);
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
public virtual System.Data.DataTable lookup_ShiftPlanning_people_workprofile(String tenantid)
							        {
                                            DataSet dataSet = new DataSet();
									        DataTable dataTable=new DataTable();
									        try{

                                        		        using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
					                                    {
						                                    npsql.Open();
						                                    using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"lookup_ShiftPlanning_people_workprofile\"(@pvar_tenantid)", npsql))
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


public virtual System.Data.DataTable lookup_change_people_ShiftPlanning_personname(string Peopleid)
						{
							DataTable dataTable=new DataTable();
                            DataSet dataSet=new DataSet();
								try
								{
								 
                                         using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
                                        {
	                                        npsql.Open();
	                                        using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"lookup_change_people_ShiftPlanning_personname\"(@pvar_peopleid)", npsql))
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

		public virtual System.Data.DataTable lookup_change_ShiftPlanning_shiftname(string Shiftid)
		{
			DataTable dataTable = new DataTable();
			DataSet dataSet = new DataSet();
			try
			{

				using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
				{
					npsql.Open();
					using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"lookup_change_ShiftPlanning_shiftname\"(@pvar_shiftid)", npsql))
					{
						dbCommand.CommandType = CommandType.Text;
						dbCommand.Parameters.AddWithValue("pvar_shiftid", NpgsqlDbType.Varchar, (object)Shiftid ?? DBNull.Value);
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



	}


			    }
