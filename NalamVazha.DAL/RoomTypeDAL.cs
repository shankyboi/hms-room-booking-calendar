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

			    //This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:41:18
			    public class RoomTypeDAL
			    {
					public virtual string db_connectionstring{get;set;}
					
			 	    public RoomTypeDAL(string connectionString)
				    {
						
					    db_connectionstring=connectionString;
				    }
				  
			        public virtual System.Data.DataTable getById_refundpolicy(string RoomTypeid)
			 {
					DataTable dataTable=new DataTable();
					DataSet dataSet=new DataSet();
					try
					{
						  
							using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
							{
								npsql.Open();
								using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"getById_sp_RoomType_refundpolicy\"(@pvar_roomtypeid)", npsql))
								{
									dbCommand.CommandType = CommandType.Text;
									dbCommand.Parameters.AddWithValue("pvar_roomtypeid",(object)RoomTypeid??DBNull.Value);
								
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


              public virtual string Add_Room_Type(RoomTypeModel model)
			  { 
				  String ResponseMessage="";
					 
					try{
							 
                            using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
					        {
						        npsql.Open();
						        using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"Add_Room_Type\"(@pvar_roomtypeid,@pvar_tenantid,@pvar_name,@pvar_prebookingdaylimit,@pvar_minbookingdays,@pvar_maxbookingdays,@pvar_concessoneligibility,@pvar_suitabilityforvip,@pvar_gendersuitability,@pvar_roomtypeicon,@pvar_deposittype,@pvar_costperday,@pvar_advanceperday,@pvar_bookingdeposit,@pvar_variableofbookingdays,@pvar_attendantcostperday,@pvar_attendantadvanceperday,@pvar_attendantbookingdeposit,@pvar_attendantvariableofbookingdays,@pvar_hourlychargesapplicable,@pvar_chargeperhour,@pvar_billingwaiverfordelayedstart,@pvar_waiverpercentage,@pvar_roomtransfercost,@pvar_description,@pvar_refundpolicy,@pvar_createduser)", npsql))
						        {
                                        dbCommand.CommandType = CommandType.Text;
						            	
								        					dbCommand.Parameters.AddWithValue("pvar_roomtypeid",NpgsqlDbType.Uuid,(object)model.RoomTypeid??DBNull.Value);
dbCommand.Parameters.AddWithValue("pvar_tenantid",NpgsqlDbType.Uuid,(object)model.tenantid??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_name",NpgsqlDbType.Varchar,(object)model.name??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_prebookingdaylimit",NpgsqlDbType.Integer,(object)model.prebookingdaylimit??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_minbookingdays",NpgsqlDbType.Integer,(object)model.minbookingdays??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_maxbookingdays",NpgsqlDbType.Integer,(object)model.maxbookingdays??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_concessoneligibility",NpgsqlDbType.Varchar,(object)model.concessoneligibility??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_suitabilityforvip",NpgsqlDbType.Varchar,(object)model.suitabilityforvip??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_gendersuitability",NpgsqlDbType.Varchar,(object)model.gendersuitability??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_roomtypeicon",NpgsqlDbType.Varchar,(object)model.roomtypeicon??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_deposittype",NpgsqlDbType.Varchar,(object)model.deposittype??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_costperday",NpgsqlDbType.Numeric,(object)model.costperday??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_advanceperday",NpgsqlDbType.Numeric,(object)model.advanceperday??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_bookingdeposit",NpgsqlDbType.Numeric,(object)model.bookingdeposit??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_variableofbookingdays",NpgsqlDbType.Numeric,(object)model.variableofbookingdays??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_attendantcostperday",NpgsqlDbType.Numeric,(object)model.attendantcostperday??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_attendantadvanceperday",NpgsqlDbType.Numeric,(object)model.attendantadvanceperday??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_attendantbookingdeposit",NpgsqlDbType.Numeric,(object)model.attendantbookingdeposit??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_attendantvariableofbookingdays",NpgsqlDbType.Numeric,(object)model.attendantvariableofbookingdays??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_hourlychargesapplicable",NpgsqlDbType.Boolean,(object)model.hourlychargesapplicable??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_chargeperhour",NpgsqlDbType.Numeric,(object)model.chargeperhour??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_billingwaiverfordelayedstart",NpgsqlDbType.Varchar,(object)model.billingwaiverfordelayedstart??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_waiverpercentage",NpgsqlDbType.Numeric,(object)model.waiverpercentage??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_roomtransfercost",NpgsqlDbType.Varchar,(object)model.roomtransfercost??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_description",NpgsqlDbType.Varchar,(object)model.description??DBNull.Value);
if(model.refundpolicy !=null  && model.refundpolicy.Count >0)
dbCommand.Parameters.AddWithValue("pvar_refundpolicy",NpgsqlDbType.Json,JsonConvert.SerializeObject(model.refundpolicy));
else
dbCommand.Parameters.AddWithValue("pvar_refundpolicy",NpgsqlDbType.Json,DBNull.Value);
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
public virtual RoomTypeModel getById_RoomType(string RoomTypeid)
									 {
										DataTable dataTable = new DataTable();
										DataSet dataSet = new DataSet();
										try{
												 
												using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
												{
													npsql.Open();
													using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"getById_sp_RoomType\"(@pvar_roomtypeid)", npsql))
													{
														dbCommand.CommandType = CommandType.Text;
														dbCommand.Parameters.AddWithValue("pvar_roomtypeid",(object)RoomTypeid??DBNull.Value);
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
											return ModelConverter.ConvertDataRowToModel<RoomTypeModel>(row);
										}
										else
										{
											return null;
										}
									 }
			 public virtual string  Update_Room_Type(RoomTypeModel model)
			 { 
				 String ResponseMessage="";
					try{
						 	 
							using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
							{
								npsql.Open();
								using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"Update_Room_Type\"(@pvar_roomtypeid,@pvar_tenantid,@pvar_name,@pvar_prebookingdaylimit,@pvar_minbookingdays,@pvar_maxbookingdays,@pvar_concessoneligibility,@pvar_suitabilityforvip,@pvar_gendersuitability,@pvar_roomtypeicon,@pvar_deposittype,@pvar_costperday,@pvar_advanceperday,@pvar_bookingdeposit,@pvar_variableofbookingdays,@pvar_attendantcostperday,@pvar_attendantadvanceperday,@pvar_attendantbookingdeposit,@pvar_attendantvariableofbookingdays,@pvar_hourlychargesapplicable,@pvar_chargeperhour,@pvar_billingwaiverfordelayedstart,@pvar_waiverpercentage,@pvar_roomtransfercost,@pvar_description,@pvar_refundpolicy,@pvar_modifieduser)", npsql))
								{
										dbCommand.CommandType = CommandType.Text;
															dbCommand.Parameters.AddWithValue("pvar_roomtypeid",NpgsqlDbType.Uuid,(object)model.RoomTypeid??DBNull.Value);
dbCommand.Parameters.AddWithValue("pvar_tenantid",NpgsqlDbType.Uuid,(object)model.tenantid??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_name",NpgsqlDbType.Varchar,(object)model.name??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_prebookingdaylimit",NpgsqlDbType.Integer,(object)model.prebookingdaylimit??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_minbookingdays",NpgsqlDbType.Integer,(object)model.minbookingdays??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_maxbookingdays",NpgsqlDbType.Integer,(object)model.maxbookingdays??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_concessoneligibility",NpgsqlDbType.Varchar,(object)model.concessoneligibility??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_suitabilityforvip",NpgsqlDbType.Varchar,(object)model.suitabilityforvip??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_gendersuitability",NpgsqlDbType.Varchar,(object)model.gendersuitability??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_roomtypeicon",NpgsqlDbType.Varchar,(object)model.roomtypeicon??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_deposittype",NpgsqlDbType.Varchar,(object)model.deposittype??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_costperday",NpgsqlDbType.Numeric,(object)model.costperday??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_advanceperday",NpgsqlDbType.Numeric,(object)model.advanceperday??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_bookingdeposit",NpgsqlDbType.Numeric,(object)model.bookingdeposit??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_variableofbookingdays",NpgsqlDbType.Numeric,(object)model.variableofbookingdays??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_attendantcostperday",NpgsqlDbType.Numeric,(object)model.attendantcostperday??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_attendantadvanceperday",NpgsqlDbType.Numeric,(object)model.attendantadvanceperday??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_attendantbookingdeposit",NpgsqlDbType.Numeric,(object)model.attendantbookingdeposit??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_attendantvariableofbookingdays",NpgsqlDbType.Numeric,(object)model.attendantvariableofbookingdays??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_hourlychargesapplicable",NpgsqlDbType.Boolean,(object)model.hourlychargesapplicable??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_chargeperhour",NpgsqlDbType.Numeric,(object)model.chargeperhour??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_billingwaiverfordelayedstart",NpgsqlDbType.Varchar,(object)model.billingwaiverfordelayedstart??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_waiverpercentage",NpgsqlDbType.Numeric,(object)model.waiverpercentage??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_roomtransfercost",NpgsqlDbType.Varchar,(object)model.roomtransfercost??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_description",NpgsqlDbType.Varchar,(object)model.description??DBNull.Value);
if(model.refundpolicy !=null  && model.refundpolicy.Count >0)
dbCommand.Parameters.AddWithValue("pvar_refundpolicy",NpgsqlDbType.Json,JsonConvert.SerializeObject(model.refundpolicy));
else
dbCommand.Parameters.AddWithValue("pvar_refundpolicy",NpgsqlDbType.Json,DBNull.Value);
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
public virtual string  Remove_Room_Type(string id,string loginUserID)
			  { 
				  String ResponseMessage="";
					try{ 
							using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
							{
								npsql.Open();
								using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"Remove_Room_Type\"(@pvar_roomtypeid,@pvar_modifieduser)", npsql))
								{
										dbCommand.CommandType = CommandType.Text;
										dbCommand.Parameters.AddWithValue("pvar_roomtypeid",(object)id??DBNull.Value);
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
public virtual System.Data.DataTable Room_Type_List(string tenantid
,string concessoneligibility="",string suitabilityforvip="",string gendersuitability=""
)
			  {
					DataTable dataTable = new DataTable();
                DataSet dataSet = new DataSet();

					try{

							using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
							{
								npsql.Open();
								using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"Room_Type_List\"(@pvar_tenantid,@pvar_concessoneligibility,@pvar_suitabilityforvip,@pvar_gendersuitability)", npsql))
								{
									dbCommand.CommandType = CommandType.Text;
									dbCommand.Parameters.AddWithValue("pvar_tenantid",(object)tenantid??DBNull.Value);
									dbCommand.Parameters.AddWithValue("pvar_concessoneligibility",(object)concessoneligibility??DBNull.Value);
									dbCommand.Parameters.AddWithValue("pvar_suitabilityforvip",(object)suitabilityforvip??DBNull.Value);
									dbCommand.Parameters.AddWithValue("pvar_gendersuitability",(object)gendersuitability??DBNull.Value);

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
			   
			 
public virtual System.Data.DataTable get_all_RoomType(string tenantid, string patientGender = null, string attendantGender = null)
			  { 

				    DataTable dataTable = new DataTable();
					DataSet dataSet = new DataSet();

					try{
 
							using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
							{
								npsql.Open();
								using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"get_all_RoomType\"(@pvar_tenantid, @pvar_patient_gender, @pvar_attendant_gender)", npsql))
								{
									dbCommand.CommandType = CommandType.Text;
									dbCommand.Parameters.AddWithValue("pvar_tenantid",(object)tenantid??DBNull.Value);
									dbCommand.Parameters.AddWithValue("pvar_patient_gender", string.IsNullOrWhiteSpace(patientGender) ? (object)DBNull.Value : patientGender.Trim());
									dbCommand.Parameters.AddWithValue("pvar_attendant_gender", string.IsNullOrWhiteSpace(attendantGender) ? (object)DBNull.Value : attendantGender.Trim());
									
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
public virtual System.Data.DataTable getById_allinfo_RoomType(string RoomTypeid)
			 {
				DataSet dataSet=new DataSet();
				DataTable dataTable = new DataTable();
				try{
					     
						using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
						{
							npsql.Open();
							using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"getById_sp_all_RoomType\"(@pvar_roomtypeid)", npsql))
							{
								dbCommand.CommandType = CommandType.Text;
								dbCommand.Parameters.AddWithValue("pvar_roomtypeid",(object)RoomTypeid??DBNull.Value);
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
