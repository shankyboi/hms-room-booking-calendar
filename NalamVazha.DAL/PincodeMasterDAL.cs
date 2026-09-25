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

			    //This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/16/2026 09:42:32
			    public class PincodeMasterDAL
			    {
					public virtual string db_connectionstring{get;set;}
					
			 	    public PincodeMasterDAL(string connectionString)
				    {
						
					    db_connectionstring=connectionString;
				    }
				  
			        
              public virtual string Add_Pincode(PincodeMasterModel model)
			  { 
				  String ResponseMessage="";
					 
					try{
							 
                            using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
					        {
						        npsql.Open();
						        using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"Add_Pincode\"(@pvar_pincodemasterid,@pvar_circlename,@pvar_regionname,@pvar_divisionname,@pvar_officename,@pvar_pincode,@pvar_officetype,@pvar_delivery,@pvar_district,@pvar_statename,@pvar_latitude,@pvar_longitude,@pvar_createduser)", npsql))
						        {
                                        dbCommand.CommandType = CommandType.Text;
						            	
								        					dbCommand.Parameters.AddWithValue("pvar_pincodemasterid",NpgsqlDbType.Uuid,(object)model.PincodeMasterid??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_circlename",NpgsqlDbType.Varchar,(object)model.circlename??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_regionname",NpgsqlDbType.Varchar,(object)model.regionname??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_divisionname",NpgsqlDbType.Varchar,(object)model.divisionname??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_officename",NpgsqlDbType.Varchar,(object)model.officename??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_pincode",NpgsqlDbType.Varchar,(object)model.pincode??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_officetype",NpgsqlDbType.Varchar,(object)model.officetype??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_delivery",NpgsqlDbType.Varchar,(object)model.delivery??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_district",NpgsqlDbType.Varchar,(object)model.district??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_statename",NpgsqlDbType.Varchar,(object)model.statename??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_latitude",NpgsqlDbType.Varchar,(object)model.latitude??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_longitude",NpgsqlDbType.Varchar,(object)model.longitude??DBNull.Value);
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
public virtual PincodeMasterModel getById_PincodeMaster(string PincodeMasterid)
									 {
										DataTable dataTable = new DataTable();
										DataSet dataSet = new DataSet();
										try{
												 
												using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
												{
													npsql.Open();
													using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"getById_sp_PincodeMaster\"(@pvar_pincodemasterid)", npsql))
													{
														dbCommand.CommandType = CommandType.Text;
														dbCommand.Parameters.AddWithValue("pvar_pincodemasterid",(object)PincodeMasterid??DBNull.Value);
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
											return ModelConverter.ConvertDataRowToModel<PincodeMasterModel>(row);
										}
										else
										{
											return null;
										}
									 }
			 public virtual string  Update_Pincode(PincodeMasterModel model)
			 { 
				 String ResponseMessage="";
					try{
						 	 
							using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
							{
								npsql.Open();
								using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"Update_Pincode\"(@pvar_pincodemasterid,@pvar_circlename,@pvar_regionname,@pvar_divisionname,@pvar_officename,@pvar_pincode,@pvar_officetype,@pvar_delivery,@pvar_district,@pvar_statename,@pvar_latitude,@pvar_longitude,@pvar_modifieduser)", npsql))
								{
										dbCommand.CommandType = CommandType.Text;
															dbCommand.Parameters.AddWithValue("pvar_pincodemasterid",NpgsqlDbType.Uuid,(object)model.PincodeMasterid??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_circlename",NpgsqlDbType.Varchar,(object)model.circlename??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_regionname",NpgsqlDbType.Varchar,(object)model.regionname??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_divisionname",NpgsqlDbType.Varchar,(object)model.divisionname??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_officename",NpgsqlDbType.Varchar,(object)model.officename??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_pincode",NpgsqlDbType.Varchar,(object)model.pincode??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_officetype",NpgsqlDbType.Varchar,(object)model.officetype??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_delivery",NpgsqlDbType.Varchar,(object)model.delivery??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_district",NpgsqlDbType.Varchar,(object)model.district??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_statename",NpgsqlDbType.Varchar,(object)model.statename??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_latitude",NpgsqlDbType.Varchar,(object)model.latitude??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_longitude",NpgsqlDbType.Varchar,(object)model.longitude??DBNull.Value);
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
public virtual string  Remove_Pincode(string id,string loginUserID)
			  { 
				  String ResponseMessage="";
					try{ 
							using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
							{
								npsql.Open();
								using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"Remove_Pincode\"(@pvar_pincodemasterid,@pvar_modifieduser)", npsql))
								{
										dbCommand.CommandType = CommandType.Text;
										dbCommand.Parameters.AddWithValue("pvar_pincodemasterid",(object)id??DBNull.Value);
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
public virtual JObject Pincode_List(string pincode
, int? pagesize=1000 , int? pagenumber=0,string searchterm="",string  sort_fields="")
			  { 
				  object dalResponse = null;
			
					try{
 
							using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
							{
								
								using (var dbCommand = new NpgsqlCommand("SELECT * FROM\"Pincode_List\"(@pvar_pincode,@pvar_pagesize,@pvar_pagenumber,@pvar_searchterm,@pvar_sort_fields)", npsql))
								{
									dbCommand.CommandType = CommandType.Text;
									dbCommand.Parameters.AddWithValue("pvar_pincode",(object)pincode??DBNull.Value);

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
			   
			 
public virtual System.Data.DataTable get_all_PincodeMaster(string tenantid,string searchterm="", int? pagesize=1000, int? pagenumber=0)
			  { 

				    DataTable dataTable = new DataTable();
					DataSet dataSet = new DataSet();

					try{
 
							using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
							{
								npsql.Open();
								using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"get_all_PincodeMaster\"(@pvar_tenantid)", npsql))
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
public virtual System.Data.DataTable getById_allinfo_PincodeMaster(string PincodeMasterid)
			 {
				DataSet dataSet=new DataSet();
				DataTable dataTable = new DataTable();
				try{
					     
						using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
						{
							npsql.Open();
							using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"getById_sp_all_PincodeMaster\"(@pvar_pincodemasterid)", npsql))
							{
								dbCommand.CommandType = CommandType.Text;
								dbCommand.Parameters.AddWithValue("pvar_pincodemasterid",(object)PincodeMasterid??DBNull.Value);
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
