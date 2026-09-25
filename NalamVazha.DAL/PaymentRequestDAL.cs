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

			    //This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:43:49
			    public class PaymentRequestDAL
			    {
					public virtual string db_connectionstring{get;set;}
					
			 	    public PaymentRequestDAL(string connectionString)
				    {
						
					    db_connectionstring=connectionString;
				    }
				  
			        
              public virtual string Add_Payment_Request(PaymentRequestModel model)
			  { 
				  String ResponseMessage="";
					 
					try{
							 
                            using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
					        {
						        npsql.Open();
						        using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"Add_Payment_Request\"(@pvar_paymentrequestid,@pvar_tenantid,@pvar_paymentgateway,@pvar_requestdatetime,@pvar_patientname,@pvar_people,@pvar_paymenttype,@pvar_merchantid,@pvar_orderid,@pvar_paymentid,@pvar_amount,@pvar_currency,@pvar_customername,@pvar_customeremail,@pvar_customerphone,@pvar_orderpaymentdesc,@pvar_returnurl,@pvar_notifyurl,@pvar_signatureorchecksum,@pvar_createduser)", npsql))
						        {
                                        dbCommand.CommandType = CommandType.Text;
						            	
								        					dbCommand.Parameters.AddWithValue("pvar_paymentrequestid",NpgsqlDbType.Uuid,(object)model.PaymentRequestid??DBNull.Value);
dbCommand.Parameters.AddWithValue("pvar_tenantid",NpgsqlDbType.Uuid,(object)model.tenantid??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_paymentgateway",NpgsqlDbType.Varchar,(object)model.paymentgateway??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_requestdatetime",NpgsqlDbType.Timestamp,(object)model.requestdatetime??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_patientname",NpgsqlDbType.Uuid,(object)model.patientname??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_people",NpgsqlDbType.Uuid,(object)model.people??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_paymenttype",NpgsqlDbType.Varchar,(object)model.paymenttype??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_merchantid",NpgsqlDbType.Varchar,(object)model.merchantid??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_orderid",NpgsqlDbType.Varchar,(object)model.orderid??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_paymentid",NpgsqlDbType.Varchar,(object)model.paymentid??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_amount",NpgsqlDbType.Numeric,(object)model.amount??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_currency",NpgsqlDbType.Varchar,(object)model.currency??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_customername",NpgsqlDbType.Varchar,(object)model.customername??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_customeremail",NpgsqlDbType.Varchar,(object)model.customeremail??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_customerphone",NpgsqlDbType.Varchar,(object)model.customerphone??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_orderpaymentdesc",NpgsqlDbType.Varchar,(object)model.orderpaymentdesc??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_returnurl",NpgsqlDbType.Varchar,(object)model.returnurl??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_notifyurl",NpgsqlDbType.Varchar,(object)model.notifyurl??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_signatureorchecksum",NpgsqlDbType.Varchar,(object)model.signatureorchecksum??DBNull.Value);
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
public virtual PaymentRequestModel getById_PaymentRequest(string PaymentRequestid)
									 {
										DataTable dataTable = new DataTable();
										DataSet dataSet = new DataSet();
										try{
												 
												using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
												{
													npsql.Open();
													using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"getById_sp_PaymentRequest\"(@pvar_paymentrequestid)", npsql))
													{
														dbCommand.CommandType = CommandType.Text;
														dbCommand.Parameters.AddWithValue("pvar_paymentrequestid",(object)PaymentRequestid??DBNull.Value);
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
											return ModelConverter.ConvertDataRowToModel<PaymentRequestModel>(row);
										}
										else
										{
											return null;
										}
									 }
			 public virtual string  Update_Payment_Request(PaymentRequestModel model)
			 { 
				 String ResponseMessage="";
					try{
						 	 
							using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
							{
								npsql.Open();
								using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"Update_Payment_Request\"(@pvar_paymentrequestid,@pvar_tenantid,@pvar_paymentgateway,@pvar_requestdatetime,@pvar_patientname,@pvar_people,@pvar_paymenttype,@pvar_merchantid,@pvar_orderid,@pvar_paymentid,@pvar_amount,@pvar_currency,@pvar_customername,@pvar_customeremail,@pvar_customerphone,@pvar_orderpaymentdesc,@pvar_returnurl,@pvar_notifyurl,@pvar_signatureorchecksum,@pvar_modifieduser)", npsql))
								{
										dbCommand.CommandType = CommandType.Text;
															dbCommand.Parameters.AddWithValue("pvar_paymentrequestid",NpgsqlDbType.Uuid,(object)model.PaymentRequestid??DBNull.Value);
dbCommand.Parameters.AddWithValue("pvar_tenantid",NpgsqlDbType.Uuid,(object)model.tenantid??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_paymentgateway",NpgsqlDbType.Varchar,(object)model.paymentgateway??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_requestdatetime",NpgsqlDbType.Timestamp,(object)model.requestdatetime??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_patientname",NpgsqlDbType.Uuid,(object)model.patientname??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_people",NpgsqlDbType.Uuid,(object)model.people??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_paymenttype",NpgsqlDbType.Varchar,(object)model.paymenttype??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_merchantid",NpgsqlDbType.Varchar,(object)model.merchantid??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_orderid",NpgsqlDbType.Varchar,(object)model.orderid??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_paymentid",NpgsqlDbType.Varchar,(object)model.paymentid??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_amount",NpgsqlDbType.Numeric,(object)model.amount??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_currency",NpgsqlDbType.Varchar,(object)model.currency??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_customername",NpgsqlDbType.Varchar,(object)model.customername??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_customeremail",NpgsqlDbType.Varchar,(object)model.customeremail??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_customerphone",NpgsqlDbType.Varchar,(object)model.customerphone??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_orderpaymentdesc",NpgsqlDbType.Varchar,(object)model.orderpaymentdesc??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_returnurl",NpgsqlDbType.Varchar,(object)model.returnurl??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_notifyurl",NpgsqlDbType.Varchar,(object)model.notifyurl??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_signatureorchecksum",NpgsqlDbType.Varchar,(object)model.signatureorchecksum??DBNull.Value);
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
public virtual string  Remove_Payment_Request(string id,string loginUserID)
			  { 
				  String ResponseMessage="";
					try{ 
							using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
							{
								npsql.Open();
								using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"Remove_Payment_Request\"(@pvar_paymentrequestid,@pvar_modifieduser)", npsql))
								{
										dbCommand.CommandType = CommandType.Text;
										dbCommand.Parameters.AddWithValue("pvar_paymentrequestid",(object)id??DBNull.Value);
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
public virtual JObject Payment_Request_List(string tenantid
,string paymentgateway
,string requestdatetime_automatonfrom
,string requestdatetime_automatonto
,string patientname
,string people
,string paymentid
, int? pagesize=1000 , int? pagenumber=0,string searchterm="",string  sort_fields="")
			  { 
				  object dalResponse = null;
			
					try{
 
							using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
							{
								
								using (var dbCommand = new NpgsqlCommand("SELECT * FROM\"Payment_Request_List\"(@pvar_tenantid,@pvar_paymentgateway,@pvar_requestdatetime_automatonfrom,@pvar_requestdatetime_automatonto,@pvar_patientname,@pvar_people,@pvar_paymentid,@pvar_pagesize,@pvar_pagenumber,@pvar_searchterm,@pvar_sort_fields)", npsql))
								{
									dbCommand.CommandType = CommandType.Text;
									dbCommand.Parameters.AddWithValue("pvar_tenantid",(object)tenantid??DBNull.Value);
dbCommand.Parameters.AddWithValue("pvar_paymentgateway",(object)paymentgateway??DBNull.Value);
dbCommand.Parameters.AddWithValue("pvar_requestdatetime_automatonfrom",(object)requestdatetime_automatonfrom??DBNull.Value);
dbCommand.Parameters.AddWithValue("pvar_requestdatetime_automatonto",(object)requestdatetime_automatonto??DBNull.Value);
dbCommand.Parameters.AddWithValue("pvar_patientname",(object)patientname??DBNull.Value);
dbCommand.Parameters.AddWithValue("pvar_people",(object)people??DBNull.Value);
dbCommand.Parameters.AddWithValue("pvar_paymentid",(object)paymentid??DBNull.Value);

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
			   
			 
public virtual System.Data.DataTable get_all_PaymentRequest(string tenantid,string searchterm="", int? pagesize=1000, int? pagenumber=0)
			  { 

				    DataTable dataTable = new DataTable();
					DataSet dataSet = new DataSet();

					try{
 
							using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
							{
								npsql.Open();
								using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"get_all_PaymentRequest\"(@pvar_tenantid)", npsql))
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
public virtual System.Data.DataTable getById_allinfo_PaymentRequest(string PaymentRequestid)
			 {
				DataSet dataSet=new DataSet();
				DataTable dataTable = new DataTable();
				try{
					     
						using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
						{
							npsql.Open();
							using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"getById_sp_all_PaymentRequest\"(@pvar_paymentrequestid)", npsql))
							{
								dbCommand.CommandType = CommandType.Text;
								dbCommand.Parameters.AddWithValue("pvar_paymentrequestid",(object)PaymentRequestid??DBNull.Value);
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
			  
public virtual System.Data.DataTable lookup_PaymentRequest_patientname(String tenantid,string searchterm="", int? pagesize=1000, int? pagenumber=0)
							        {
                                            DataSet dataSet = new DataSet();
									        DataTable dataTable=new DataTable();
									        try{

                                        		        using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
					                                    {
						                                    npsql.Open();
						                                    using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"lookup_PaymentRequest_patientname\"(@pvar_tenantid,@pvar_searchterm,@pvar_pagesize,@pvar_pagenumber)", npsql))
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
public virtual System.Data.DataTable lookup_PaymentRequest_people(String tenantid,string searchterm="", int? pagesize=1000, int? pagenumber=0)
							        {
                                            DataSet dataSet = new DataSet();
									        DataTable dataTable=new DataTable();
									        try{

                                        		        using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
					                                    {
						                                    npsql.Open();
						                                    using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"lookup_PaymentRequest_people\"(@pvar_tenantid,@pvar_searchterm,@pvar_pagesize,@pvar_pagenumber)", npsql))
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
