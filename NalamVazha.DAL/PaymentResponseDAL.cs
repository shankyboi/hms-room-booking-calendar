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

			    //This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:43:53
			    public class PaymentResponseDAL
			    {
					public virtual string db_connectionstring{get;set;}
					
			 	    public PaymentResponseDAL(string connectionString)
				    {
						
					    db_connectionstring=connectionString;
				    }
				  
			        
              public virtual string Add_Payment_Response(PaymentResponseModel model)
			  { 
				  String ResponseMessage="";
					 
					try{
							 
                            using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
					        {
						        npsql.Open();
						        using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"Add_Payment_Response\"(@pvar_paymentresponseid,@pvar_tenantid,@pvar_paymentrequest,@pvar_paymenttype,@pvar_transactiontime,@pvar_orderid,@pvar_paymentid,@pvar_status,@pvar_amount,@pvar_paymentmethod,@pvar_banktransactionid,@pvar_gatewayresponsecode,@pvar_gatewayresponsemessage,@pvar_responsesignature,@pvar_createduser)", npsql))
						        {
                                        dbCommand.CommandType = CommandType.Text;
						            	
								        					dbCommand.Parameters.AddWithValue("pvar_paymentresponseid",NpgsqlDbType.Uuid,(object)model.PaymentResponseid??DBNull.Value);
dbCommand.Parameters.AddWithValue("pvar_tenantid",NpgsqlDbType.Uuid,(object)model.tenantid??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_paymentrequest",NpgsqlDbType.Uuid,(object)model.paymentrequest??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_paymenttype",NpgsqlDbType.Varchar,(object)model.paymenttype??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_transactiontime",NpgsqlDbType.Timestamp,(object)model.transactiontime??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_orderid",NpgsqlDbType.Varchar,(object)model.orderid??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_paymentid",NpgsqlDbType.Varchar,(object)model.paymentid??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_status",NpgsqlDbType.Varchar,(object)model.status??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_amount",NpgsqlDbType.Numeric,(object)model.amount??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_paymentmethod",NpgsqlDbType.Varchar,(object)model.paymentmethod??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_banktransactionid",NpgsqlDbType.Varchar,(object)model.banktransactionid??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_gatewayresponsecode",NpgsqlDbType.Varchar,(object)model.gatewayresponsecode??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_gatewayresponsemessage",NpgsqlDbType.Varchar,(object)model.gatewayresponsemessage??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_responsesignature",NpgsqlDbType.Varchar,(object)model.responsesignature??DBNull.Value);
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
public virtual PaymentResponseModel getById_PaymentResponse(string PaymentResponseid)
									 {
										DataTable dataTable = new DataTable();
										DataSet dataSet = new DataSet();
										try{
												 
												using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
												{
													npsql.Open();
													using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"getById_sp_PaymentResponse\"(@pvar_paymentresponseid)", npsql))
													{
														dbCommand.CommandType = CommandType.Text;
														dbCommand.Parameters.AddWithValue("pvar_paymentresponseid",(object)PaymentResponseid??DBNull.Value);
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
											return ModelConverter.ConvertDataRowToModel<PaymentResponseModel>(row);
										}
										else
										{
											return null;
										}
									 }
			 public virtual string  Refund(PaymentResponseModel model)
			 { 
				 String ResponseMessage="";
					try{
						 	 
							using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
							{
								npsql.Open();
								using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"Refund\"(@pvar_paymentresponseid,@pvar_tenantid,@pvar_paymentrequest,@pvar_paymenttype,@pvar_transactiontime,@pvar_orderid,@pvar_paymentid,@pvar_refundedamount,@pvar_refundreason,@pvar_modifieduser)", npsql))
								{
										dbCommand.CommandType = CommandType.Text;
															dbCommand.Parameters.AddWithValue("pvar_paymentresponseid",NpgsqlDbType.Uuid,(object)model.PaymentResponseid??DBNull.Value);
dbCommand.Parameters.AddWithValue("pvar_tenantid",NpgsqlDbType.Uuid,(object)model.tenantid??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_paymentrequest",NpgsqlDbType.Uuid,(object)model.paymentrequest??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_paymenttype",NpgsqlDbType.Varchar,(object)model.paymenttype??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_transactiontime",NpgsqlDbType.Timestamp,(object)model.transactiontime??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_orderid",NpgsqlDbType.Varchar,(object)model.orderid??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_paymentid",NpgsqlDbType.Varchar,(object)model.paymentid??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_refundedamount",NpgsqlDbType.Numeric,(object)model.refundedamount??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_refundreason",NpgsqlDbType.Varchar,(object)model.refundreason??DBNull.Value);
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

			 public virtual string  Update_Payment_Response(PaymentResponseModel model)
			 { 
				 String ResponseMessage="";
					try{
						 	 
							using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
							{
								npsql.Open();
								using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"Update_Payment_Response\"(@pvar_paymentresponseid,@pvar_tenantid,@pvar_paymentrequest,@pvar_paymenttype,@pvar_transactiontime,@pvar_orderid,@pvar_paymentid,@pvar_status,@pvar_amount,@pvar_paymentmethod,@pvar_banktransactionid,@pvar_gatewayresponsecode,@pvar_gatewayresponsemessage,@pvar_responsesignature,@pvar_modifieduser)", npsql))
								{
										dbCommand.CommandType = CommandType.Text;
															dbCommand.Parameters.AddWithValue("pvar_paymentresponseid",NpgsqlDbType.Uuid,(object)model.PaymentResponseid??DBNull.Value);
dbCommand.Parameters.AddWithValue("pvar_tenantid",NpgsqlDbType.Uuid,(object)model.tenantid??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_paymentrequest",NpgsqlDbType.Uuid,(object)model.paymentrequest??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_paymenttype",NpgsqlDbType.Varchar,(object)model.paymenttype??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_transactiontime",NpgsqlDbType.Timestamp,(object)model.transactiontime??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_orderid",NpgsqlDbType.Varchar,(object)model.orderid??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_paymentid",NpgsqlDbType.Varchar,(object)model.paymentid??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_status",NpgsqlDbType.Varchar,(object)model.status??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_amount",NpgsqlDbType.Numeric,(object)model.amount??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_paymentmethod",NpgsqlDbType.Varchar,(object)model.paymentmethod??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_banktransactionid",NpgsqlDbType.Varchar,(object)model.banktransactionid??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_gatewayresponsecode",NpgsqlDbType.Varchar,(object)model.gatewayresponsecode??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_gatewayresponsemessage",NpgsqlDbType.Varchar,(object)model.gatewayresponsemessage??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_responsesignature",NpgsqlDbType.Varchar,(object)model.responsesignature??DBNull.Value);
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
public virtual string  Remove_Payment_Response(string id,string loginUserID)
			  { 
				  String ResponseMessage="";
					try{ 
							using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
							{
								npsql.Open();
								using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"Remove_Payment_Response\"(@pvar_paymentresponseid,@pvar_modifieduser)", npsql))
								{
										dbCommand.CommandType = CommandType.Text;
										dbCommand.Parameters.AddWithValue("pvar_paymentresponseid",(object)id??DBNull.Value);
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
public virtual JObject Payment_Response_List(string tenantid
,string paymentrequest
,string paymenttype
,string status
,string paymentmethod
, int? pagesize=1000 , int? pagenumber=0,string searchterm="",string  sort_fields="")
			  { 
				  object dalResponse = null;
			
					try{
 
							using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
							{
								
								using (var dbCommand = new NpgsqlCommand("SELECT * FROM\"Payment_Response_List\"(@pvar_tenantid,@pvar_paymentrequest,@pvar_paymenttype,@pvar_status,@pvar_paymentmethod,@pvar_pagesize,@pvar_pagenumber,@pvar_searchterm,@pvar_sort_fields)", npsql))
								{
									dbCommand.CommandType = CommandType.Text;
									dbCommand.Parameters.AddWithValue("pvar_tenantid",(object)tenantid??DBNull.Value);
dbCommand.Parameters.AddWithValue("pvar_paymentrequest",(object)paymentrequest??DBNull.Value);
dbCommand.Parameters.AddWithValue("pvar_paymenttype",(object)paymenttype??DBNull.Value);
dbCommand.Parameters.AddWithValue("pvar_status",(object)status??DBNull.Value);
dbCommand.Parameters.AddWithValue("pvar_paymentmethod",(object)paymentmethod??DBNull.Value);

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
			   
			 
public virtual System.Data.DataTable get_all_PaymentResponse(string tenantid,string searchterm="", int? pagesize=1000, int? pagenumber=0)
			  { 

				    DataTable dataTable = new DataTable();
					DataSet dataSet = new DataSet();

					try{
 
							using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
							{
								npsql.Open();
								using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"get_all_PaymentResponse\"(@pvar_tenantid)", npsql))
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
public virtual System.Data.DataTable getById_allinfo_PaymentResponse(string PaymentResponseid)
			 {
				DataSet dataSet=new DataSet();
				DataTable dataTable = new DataTable();
				try{
					     
						using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
						{
							npsql.Open();
							using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"getById_sp_all_PaymentResponse\"(@pvar_paymentresponseid)", npsql))
							{
								dbCommand.CommandType = CommandType.Text;
								dbCommand.Parameters.AddWithValue("pvar_paymentresponseid",(object)PaymentResponseid??DBNull.Value);
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
			  
public virtual System.Data.DataTable lookup_PaymentResponse_paymentrequest(String tenantid,string searchterm="", int? pagesize=1000, int? pagenumber=0)
							        {
                                            DataSet dataSet = new DataSet();
									        DataTable dataTable=new DataTable();
									        try{

                                        		        using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
					                                    {
						                                    npsql.Open();
						                                    using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"lookup_PaymentResponse_paymentrequest\"(@pvar_tenantid,@pvar_searchterm,@pvar_pagesize,@pvar_pagenumber)", npsql))
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







			    }


			    }
