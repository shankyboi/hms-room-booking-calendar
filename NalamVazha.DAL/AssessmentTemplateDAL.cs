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

			    //This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/23/2026 13:35:11
			    public class AssessmentTemplateDAL
			    {
					public virtual string db_connectionstring{get;set;}
					
			 	    public AssessmentTemplateDAL(string connectionString)
				    {
						
					    db_connectionstring=connectionString;
				    }
				  
			        public virtual System.Data.DataTable getById_templatequestions(string AssessmentTemplateid)
			 {
					DataTable dataTable=new DataTable();
					DataSet dataSet=new DataSet();
					try
					{
						  
							using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
							{
								npsql.Open();
								using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"getById_sp_AssessmentTemplate_templatequestions\"(@pvar_assessmenttemplateid)", npsql))
								{
									dbCommand.CommandType = CommandType.Text;
									dbCommand.Parameters.AddWithValue("pvar_assessmenttemplateid",(object)AssessmentTemplateid??DBNull.Value);
								
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

public virtual System.Data.DataTable getById_templateapplicability(string AssessmentTemplateid)
			 {
					DataTable dataTable=new DataTable();
					DataSet dataSet=new DataSet();
					try
					{
						  
							using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
							{
								npsql.Open();
								using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"getById_sp_AssessmentTemplate_templateapplicability\"(@pvar_assessmenttemplateid)", npsql))
								{
									dbCommand.CommandType = CommandType.Text;
									dbCommand.Parameters.AddWithValue("pvar_assessmenttemplateid",(object)AssessmentTemplateid??DBNull.Value);
								
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


              public virtual string Add_Assessment_Template(AssessmentTemplateModel model)
			  { 
				  String ResponseMessage="";
					 
					try{
							 
                            using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
					        {
						        npsql.Open();
						        using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"Add_Assessment_Template\"(@pvar_assessmenttemplateid,@pvar_tenantid,@pvar_templatename,@pvar_taskname,@pvar_isdefaulttemplate,@pvar_templatequestions,@pvar_templateapplicability,@pvar_createduser)", npsql))
						        {
                                        dbCommand.CommandType = CommandType.Text;
						            	
								        					dbCommand.Parameters.AddWithValue("pvar_assessmenttemplateid",NpgsqlDbType.Uuid,(object)model.AssessmentTemplateid??DBNull.Value);
dbCommand.Parameters.AddWithValue("pvar_tenantid",NpgsqlDbType.Uuid,(object)model.tenantid??DBNull.Value);

						dbCommand.Parameters.AddWithValue("pvar_taskname", NpgsqlDbType.Varchar, (object)model.taskname ?? DBNull.Value);

						dbCommand.Parameters.AddWithValue("pvar_isdefaulttemplate", NpgsqlDbType.Boolean, (object)model.isdefaulttemplate ?? DBNull.Value);

						
						dbCommand.Parameters.AddWithValue("pvar_templatename",NpgsqlDbType.Varchar,(object)model.templatename??DBNull.Value);
if(model.templatequestions !=null  && model.templatequestions.Count >0)
dbCommand.Parameters.AddWithValue("pvar_templatequestions",NpgsqlDbType.Json,JsonConvert.SerializeObject(model.templatequestions));
else
dbCommand.Parameters.AddWithValue("pvar_templatequestions",NpgsqlDbType.Json,DBNull.Value);
if(model.templateapplicability !=null  && model.templateapplicability.Count >0)
dbCommand.Parameters.AddWithValue("pvar_templateapplicability",NpgsqlDbType.Json,JsonConvert.SerializeObject(model.templateapplicability));
else
dbCommand.Parameters.AddWithValue("pvar_templateapplicability",NpgsqlDbType.Json,DBNull.Value);
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
public virtual AssessmentTemplateModel getById_AssessmentTemplate(string AssessmentTemplateid)
									 {
										DataTable dataTable = new DataTable();
										DataSet dataSet = new DataSet();
										try{
												 
												using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
												{
													npsql.Open();
													using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"getById_sp_AssessmentTemplate\"(@pvar_assessmenttemplateid)", npsql))
													{
														dbCommand.CommandType = CommandType.Text;
														dbCommand.Parameters.AddWithValue("pvar_assessmenttemplateid",(object)AssessmentTemplateid??DBNull.Value);
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
											var model = ModelConverter.ConvertDataRowToModel<AssessmentTemplateModel>(row);
											PopulateAssessmentTemplateStoredValues(model, AssessmentTemplateid);
											return model;
										}
										else
										{
											return null;
										}
									 }
				private void PopulateAssessmentTemplateStoredValues(AssessmentTemplateModel model, string AssessmentTemplateid)
				{
					if (model == null || string.IsNullOrWhiteSpace(AssessmentTemplateid))
						return;

					using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
					{
						npsql.Open();
						using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"Get_Assessment_Template_Stored_Values\"(@pvar_assessmenttemplateid)", npsql))
						{
							dbCommand.CommandType = CommandType.Text;
							dbCommand.Parameters.AddWithValue("pvar_assessmenttemplateid", NpgsqlDbType.Uuid, new Guid(AssessmentTemplateid));
							using (var reader = dbCommand.ExecuteReader())
							{
								if (reader.Read())
								{
									if (!reader.IsDBNull(0))
										model.taskname = reader.GetString(0);
									model.isdefaulttemplate = reader.GetBoolean(1);
								}
							}
						}
					}
				}
			 public virtual string  Update_Assessment_Template(AssessmentTemplateModel model)
			 { 
				 String ResponseMessage="";
					try{
						 	 
							using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
							{
								npsql.Open();
								using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"Update_Assessment_Template\"(@pvar_assessmenttemplateid,@pvar_tenantid,@pvar_templatename,@pvar_taskname,@pvar_isdefaulttemplate,@pvar_templatequestions,@pvar_templateapplicability,@pvar_modifieduser)", npsql))
								{
										dbCommand.CommandType = CommandType.Text;
															dbCommand.Parameters.AddWithValue("pvar_assessmenttemplateid",NpgsqlDbType.Uuid,(object)model.AssessmentTemplateid??DBNull.Value);
dbCommand.Parameters.AddWithValue("pvar_tenantid",NpgsqlDbType.Uuid,(object)model.tenantid??DBNull.Value);

						dbCommand.Parameters.AddWithValue("pvar_taskname", NpgsqlDbType.Varchar, (object)model.taskname ?? DBNull.Value);

						dbCommand.Parameters.AddWithValue("pvar_isdefaulttemplate", NpgsqlDbType.Boolean, (object)model.isdefaulttemplate ?? DBNull.Value);

						
						dbCommand.Parameters.AddWithValue("pvar_templatename",NpgsqlDbType.Varchar,(object)model.templatename??DBNull.Value);
if(model.templatequestions !=null  && model.templatequestions.Count >0)
dbCommand.Parameters.AddWithValue("pvar_templatequestions",NpgsqlDbType.Json,JsonConvert.SerializeObject(model.templatequestions));
else
dbCommand.Parameters.AddWithValue("pvar_templatequestions",NpgsqlDbType.Json,DBNull.Value);
if(model.templateapplicability !=null  && model.templateapplicability.Count >0)
dbCommand.Parameters.AddWithValue("pvar_templateapplicability",NpgsqlDbType.Json,JsonConvert.SerializeObject(model.templateapplicability));
else
dbCommand.Parameters.AddWithValue("pvar_templateapplicability",NpgsqlDbType.Json,DBNull.Value);
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
public virtual string  Remove_Assessment_Template(string id,string loginUserID)
			  { 
				  String ResponseMessage="";
					try{ 
							using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
							{
								npsql.Open();
								using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"Remove_Assessment_Template\"(@pvar_assessmenttemplateid,@pvar_modifieduser)", npsql))
								{
										dbCommand.CommandType = CommandType.Text;
										dbCommand.Parameters.AddWithValue("pvar_assessmenttemplateid",(object)id??DBNull.Value);
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
public virtual System.Data.DataTable Assessment_Template_List(string tenantid,string taskname
)
			  { 
					DataTable dataTable = new DataTable();
                DataSet dataSet = new DataSet(); 

					try{
 
							using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
							{
								npsql.Open();
								using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"Assessment_Template_List\"(@pvar_tenantid,@pvar_taskname)", npsql))
								{
									dbCommand.CommandType = CommandType.Text;
									dbCommand.Parameters.AddWithValue("pvar_tenantid",(object)tenantid??DBNull.Value);
						dbCommand.Parameters.AddWithValue("pvar_taskname", (object)taskname ?? DBNull.Value);
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
			   
			 
public virtual System.Data.DataTable get_all_AssessmentTemplate(string tenantid)
			  { 

				    DataTable dataTable = new DataTable();
					DataSet dataSet = new DataSet();

					try{
 
							using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
							{
								npsql.Open();
								using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"get_all_AssessmentTemplate\"(@pvar_tenantid)", npsql))
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
public virtual System.Data.DataTable getById_allinfo_AssessmentTemplate(string AssessmentTemplateid)
			 {
				DataSet dataSet=new DataSet();
				DataTable dataTable = new DataTable();
				try{
					     
						using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
						{
							npsql.Open();
							using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"getById_sp_all_AssessmentTemplate\"(@pvar_assessmenttemplateid)", npsql))
							{
								dbCommand.CommandType = CommandType.Text;
								dbCommand.Parameters.AddWithValue("pvar_assessmenttemplateid",(object)AssessmentTemplateid??DBNull.Value);
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
			  
public virtual System.Data.DataTable lookup_AssessmentTemplate_templatequestions_questioncategory(String tenantid)
							        {
                                            DataSet dataSet = new DataSet();
									        DataTable dataTable=new DataTable();
									        try{

                                        		        using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
					                                    {
						                                    npsql.Open();
						                                    using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"lookup_AssessmentTemplate_templatequestions_questioncategory\"(@pvar_tenantid)", npsql))
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
public virtual System.Data.DataTable lookup_AssessmentTemplate_templatequestions_questionsub(String tenantid,String questioncategory)
							        {
                                            DataSet dataSet = new DataSet();
									        DataTable dataTable=new DataTable();
									        try{

                                        		        using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
					                                    {
						                                    npsql.Open();
						                                    using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"lookup_AssessmentTemplate_templatequestions_questionsub\"(@pvar_tenantid,@pvar_questioncategoryname)", npsql))
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
public virtual System.Data.DataTable lookup_AssessmentTemplate_templatequestions_question(String tenantid,String questioncategory,String questionsub)
							        {
                                            DataSet dataSet = new DataSet();
									        DataTable dataTable=new DataTable();
									        try{

                                        		        using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
					                                    {
						                                    npsql.Open();
						                                    using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"lookup_AssessmentTemplate_templatequestions_question\"(@pvar_tenantid,@pvar_questioncategory,@pvar_questionsubcategory)", npsql))
						                                    {
                                            
                                                                dbCommand.Parameters.AddWithValue("pvar_tenantid",(object)tenantid??DBNull.Value);dbCommand.Parameters.AddWithValue("pvar_questioncategory",(object)questioncategory??DBNull.Value);dbCommand.Parameters.AddWithValue("pvar_questionsubcategory",(object)questionsub??DBNull.Value);  
																
																 
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
public virtual System.Data.DataTable lookup_AssessmentTemplate_templateapplicability_patientcategory(String tenantid)
							        {
                                            DataSet dataSet = new DataSet();
									        DataTable dataTable=new DataTable();
									        try{

                                        		        using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
					                                    {
						                                    npsql.Open();
						                                    using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"lookup_AssessmentTemplate_templateapplicability_patientcategory\"(@pvar_tenantid)", npsql))
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
public virtual System.Data.DataTable lookup_AssessmentTemplate_templateapplicability_medicalcondition(string searchterm="", int? pagesize=1000, int? pagenumber=0)
							        {
                                            DataSet dataSet = new DataSet();
									        DataTable dataTable=new DataTable();
									        try{

                                        		        using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
					                                    {
						                                    npsql.Open();
						                                    using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"lookup_AssessmentTemplate_templateapplicability_medicalcondition\"(@pvar_searchterm,@pvar_pagesize,@pvar_pagenumber)", npsql))
						                                    {
                                            
                                                                  
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


public virtual System.Data.DataTable lookup_change_templatequestions_AssessmentTemplate_question(string AssessmentQuestionid)
						{
							DataTable dataTable=new DataTable();
                            DataSet dataSet=new DataSet();
								try
								{
								 
                                         using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
                                        {
	                                        npsql.Open();
	                                        using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"lookup_change_templatequestions_AssessmentTemplate_question\"(@pvar_assessmentquestionid)", npsql))
	                                        {
		                                        dbCommand.CommandType = CommandType.Text;
		                                        dbCommand.Parameters.AddWithValue("pvar_assessmentquestionid",NpgsqlDbType.Varchar,(object)AssessmentQuestionid??DBNull.Value);
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



		public virtual System.Data.DataTable lookup_AssessmentTemplate_taskname(String tenantid)
		{
			DataSet dataSet = new DataSet();
			DataTable dataTable = new DataTable();
			try
			{

				using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
				{
					npsql.Open();
					using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"lookup_AssessmentTemplate_taskname\"(@pvar_tenantid)", npsql))
					{

						dbCommand.Parameters.AddWithValue("pvar_tenantid", (object)tenantid ?? DBNull.Value);


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



			}
			catch
			{
				throw;
			}
			return dataTable;
		}


	}


}
