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

			    //This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/23/2026 13:27:20
			    public class AssessmentQuestionDAL
			    {
					public virtual string db_connectionstring{get;set;}
					
			 	    public AssessmentQuestionDAL(string connectionString)
				    {
						
					    db_connectionstring=connectionString;
				    }
				  
			        
              public virtual string Add_Assessment_Question(AssessmentQuestionModel model)
			  { 
				  String ResponseMessage="";
					 
					try{
							 
                            using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
					        {
						        npsql.Open();
						        using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"Add_Assessment_Question\"(@pvar_assessmentquestionid,@pvar_tenantid,@pvar_questionnairereferencenumber,@pvar_questioncategory,@pvar_questionsubcategory,@pvar_questiontext,@pvar_answertype,@pvar_optiontext,@pvar_optionvalue,@pvar_defaultvalue,@pvar_scalerangemin,@pvar_scalerangemax,@pvar_isrequired,@pvar_scorevalue,@pvar_createduser)", npsql))
						        {
                                        dbCommand.CommandType = CommandType.Text;
						            	
								        					dbCommand.Parameters.AddWithValue("pvar_assessmentquestionid",NpgsqlDbType.Uuid,(object)model.AssessmentQuestionid??DBNull.Value);
dbCommand.Parameters.AddWithValue("pvar_tenantid",NpgsqlDbType.Uuid,(object)model.tenantid??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_questionnairereferencenumber",NpgsqlDbType.Varchar,(object)model.questionnairereferencenumber??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_questioncategory",NpgsqlDbType.Uuid,(object)model.questioncategory??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_questionsubcategory",NpgsqlDbType.Uuid,(object)model.questionsubcategory??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_questiontext",NpgsqlDbType.Varchar,(object)model.questiontext??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_answertype",NpgsqlDbType.Varchar,(object)model.answertype??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_optiontext",NpgsqlDbType.Varchar,(object)model.optiontext??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_optionvalue",NpgsqlDbType.Varchar,(object)model.optionvalue??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_defaultvalue",NpgsqlDbType.Varchar,(object)model.defaultvalue??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_scalerangemin",NpgsqlDbType.Integer,(object)model.scalerangemin??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_scalerangemax",NpgsqlDbType.Integer,(object)model.scalerangemax??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_isrequired",NpgsqlDbType.Boolean,(object)model.isrequired??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_scorevalue",NpgsqlDbType.Integer,(object)model.scorevalue??DBNull.Value);
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
public virtual AssessmentQuestionModel getById_AssessmentQuestion(string AssessmentQuestionid)
									 {
										DataTable dataTable = new DataTable();
										DataSet dataSet = new DataSet();
										try{
												 
												using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
												{
													npsql.Open();
													using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"getById_sp_AssessmentQuestion\"(@pvar_assessmentquestionid)", npsql))
													{
														dbCommand.CommandType = CommandType.Text;
														dbCommand.Parameters.AddWithValue("pvar_assessmentquestionid",(object)AssessmentQuestionid??DBNull.Value);
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
											return ModelConverter.ConvertDataRowToModel<AssessmentQuestionModel>(row);
										}
										else
										{
											return null;
										}
									 }
			 public virtual string  Update_Assessment_Question(AssessmentQuestionModel model)
			 { 
				 String ResponseMessage="";
					try{
						 	 
							using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
							{
								npsql.Open();
								using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"Update_Assessment_Question\"(@pvar_assessmentquestionid,@pvar_tenantid,@pvar_questionnairereferencenumber,@pvar_questioncategory,@pvar_questionsubcategory,@pvar_questiontext,@pvar_answertype,@pvar_optiontext,@pvar_optionvalue,@pvar_defaultvalue,@pvar_scalerangemin,@pvar_scalerangemax,@pvar_isrequired,@pvar_scorevalue,@pvar_modifieduser)", npsql))
								{
										dbCommand.CommandType = CommandType.Text;
															dbCommand.Parameters.AddWithValue("pvar_assessmentquestionid",NpgsqlDbType.Uuid,(object)model.AssessmentQuestionid??DBNull.Value);
dbCommand.Parameters.AddWithValue("pvar_tenantid",NpgsqlDbType.Uuid,(object)model.tenantid??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_questionnairereferencenumber",NpgsqlDbType.Varchar,(object)model.questionnairereferencenumber??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_questioncategory",NpgsqlDbType.Uuid,(object)model.questioncategory??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_questionsubcategory",NpgsqlDbType.Uuid,(object)model.questionsubcategory??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_questiontext",NpgsqlDbType.Varchar,(object)model.questiontext??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_answertype",NpgsqlDbType.Varchar,(object)model.answertype??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_optiontext",NpgsqlDbType.Varchar,(object)model.optiontext??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_optionvalue",NpgsqlDbType.Varchar,(object)model.optionvalue??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_defaultvalue",NpgsqlDbType.Varchar,(object)model.defaultvalue??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_scalerangemin",NpgsqlDbType.Integer,(object)model.scalerangemin??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_scalerangemax",NpgsqlDbType.Integer,(object)model.scalerangemax??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_isrequired",NpgsqlDbType.Boolean,(object)model.isrequired??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_scorevalue",NpgsqlDbType.Integer,(object)model.scorevalue??DBNull.Value);
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
public virtual string  Remove_Assessment_Question(string id,string loginUserID)
			  { 
				  String ResponseMessage="";
					try{ 
							using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
							{
								npsql.Open();
								using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"Remove_Assessment_Question\"(@pvar_assessmentquestionid,@pvar_modifieduser)", npsql))
								{
										dbCommand.CommandType = CommandType.Text;
										dbCommand.Parameters.AddWithValue("pvar_assessmentquestionid",(object)id??DBNull.Value);
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
public virtual System.Data.DataTable Assessment_Question_List(string tenantid
,string questioncategory
,string questionsubcategory
)
			  { 
					DataTable dataTable = new DataTable();
                DataSet dataSet = new DataSet(); 

					try{
 
							using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
							{
								npsql.Open();
								using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"Assessment_Question_List\"(@pvar_tenantid,@pvar_questioncategory,@pvar_questionsubcategory)", npsql))
								{
									dbCommand.CommandType = CommandType.Text;
									dbCommand.Parameters.AddWithValue("pvar_tenantid",(object)tenantid??DBNull.Value);
dbCommand.Parameters.AddWithValue("pvar_questioncategory",(object)questioncategory??DBNull.Value);
dbCommand.Parameters.AddWithValue("pvar_questionsubcategory",(object)questionsubcategory??DBNull.Value);

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
			   
			 
public virtual System.Data.DataTable get_all_AssessmentQuestion(string tenantid)
			  { 

				    DataTable dataTable = new DataTable();
					DataSet dataSet = new DataSet();

					try{
 
							using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
							{
								npsql.Open();
								using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"get_all_AssessmentQuestion\"(@pvar_tenantid)", npsql))
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
public virtual System.Data.DataTable getById_allinfo_AssessmentQuestion(string AssessmentQuestionid)
			 {
				DataSet dataSet=new DataSet();
				DataTable dataTable = new DataTable();
				try{
					     
						using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
						{
							npsql.Open();
							using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"getById_sp_all_AssessmentQuestion\"(@pvar_assessmentquestionid)", npsql))
							{
								dbCommand.CommandType = CommandType.Text;
								dbCommand.Parameters.AddWithValue("pvar_assessmentquestionid",(object)AssessmentQuestionid??DBNull.Value);
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
			  
public virtual System.Data.DataTable lookup_AssessmentQuestion_questioncategory(String tenantid)
							        {
                                            DataSet dataSet = new DataSet();
									        DataTable dataTable=new DataTable();
									        try{

                                        		        using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
					                                    {
						                                    npsql.Open();
						                                    using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"lookup_AssessmentQuestion_questioncategory\"(@pvar_tenantid)", npsql))
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
public virtual System.Data.DataTable lookup_AssessmentQuestion_questionsubcategory(String tenantid,String questioncategory)
							        {
                                            DataSet dataSet = new DataSet();
									        DataTable dataTable=new DataTable();
									        try{

                                        		        using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
					                                    {
						                                    npsql.Open();
						                                    using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"lookup_AssessmentQuestion_questionsubcategory\"(@pvar_tenantid,@pvar_questioncategoryname)", npsql))
						                                    {
                                            
                                                                dbCommand.Parameters.AddWithValue("pvar_tenantid",(object)tenantid??DBNull.Value);dbCommand.Parameters.AddWithValue("pvar_questioncategoryname",(object)questioncategory??DBNull.Value);  
																
																 
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
