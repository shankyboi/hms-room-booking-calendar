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

			    //This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:43:41
			    public class EnquiryFormDAL
			    {
					public virtual string db_connectionstring{get;set;}
					
			 	    public EnquiryFormDAL(string connectionString)
				    {
						
					    db_connectionstring=connectionString;
				    }
				  
			        public virtual System.Data.DataTable getById_medicalinfo(string EnquiryFormid)
			 {
					DataTable dataTable=new DataTable();
					DataSet dataSet=new DataSet();
					try
					{
						  
							using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
							{
								npsql.Open();
								using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"getById_sp_EnquiryForm_medicalinfo\"(@pvar_enquiryformid)", npsql))
								{
									dbCommand.CommandType = CommandType.Text;
									dbCommand.Parameters.AddWithValue("pvar_enquiryformid",(object)EnquiryFormid??DBNull.Value);
								
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


              public virtual string Add_Enquiry(EnquiryFormModel model)
			  { 
				  String ResponseMessage="";
					 
					try{
							 
                            using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
					        {
						        npsql.Open();
						        using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"Add_Enquiry\"(@pvar_enquiryformid,@pvar_tenantid,@pvar_enquirynumber,@pvar_enquirydate,@pvar_enquirytype,@pvar_isroombookingrelated,@pvar_patientname,@pvar_firstname,@pvar_lastname,@pvar_gender,@pvar_age,@pvar_phonenumber,@pvar_emailaddress,@pvar_preferredcontactmethod,@pvar_enquiryreason,@pvar_enquiredvia,@pvar_preferredroomtype,@pvar_preferreddateofarrival,@pvar_preferreddateofdeparture,@pvar_joinwaitinglist,@pvar_enquirystatus,@pvar_verifiedstatus,@pvar_medicalinfo,@pvar_createduser)", npsql))
						        {
                                        dbCommand.CommandType = CommandType.Text;
						            	
								        					dbCommand.Parameters.AddWithValue("pvar_enquiryformid",NpgsqlDbType.Uuid,(object)model.EnquiryFormid??DBNull.Value);
dbCommand.Parameters.AddWithValue("pvar_tenantid",NpgsqlDbType.Uuid,(object)model.tenantid??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_enquirynumber",NpgsqlDbType.Varchar,(object)model.enquirynumber??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_enquirydate",NpgsqlDbType.Date,(object)model.enquirydate??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_enquirytype",NpgsqlDbType.Uuid,(object)model.enquirytype??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_isroombookingrelated",NpgsqlDbType.Varchar,(object)model.isroombookingrelated??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_patientname",NpgsqlDbType.Uuid,(object)model.patientname??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_firstname",NpgsqlDbType.Varchar,(object)model.firstname??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_lastname",NpgsqlDbType.Varchar,(object)model.lastname??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_gender",NpgsqlDbType.Varchar,(object)model.gender??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_age",NpgsqlDbType.Bigint,(object)model.age??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_phonenumber",NpgsqlDbType.Varchar,(object)model.phonenumber??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_emailaddress",NpgsqlDbType.Varchar,(object)model.emailaddress??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_preferredcontactmethod",NpgsqlDbType.Varchar,(object)model.preferredcontactmethod??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_enquiryreason",NpgsqlDbType.Varchar,(object)model.enquiryreason??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_enquiredvia",NpgsqlDbType.Varchar,(object)model.enquiredvia??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_preferredroomtype",NpgsqlDbType.Uuid,(object)model.preferredroomtype??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_preferreddateofarrival",NpgsqlDbType.Date,(object)model.preferreddateofarrival??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_preferreddateofdeparture",NpgsqlDbType.Date,(object)model.preferreddateofdeparture??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_joinwaitinglist",NpgsqlDbType.Boolean,(object)model.joinwaitinglist??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_enquirystatus",NpgsqlDbType.Varchar,(object)model.enquirystatus??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_verifiedstatus",NpgsqlDbType.Varchar,(object)model.verifiedstatus??DBNull.Value);
if(model.medicalinfo !=null  && model.medicalinfo.Count >0)
dbCommand.Parameters.AddWithValue("pvar_medicalinfo",NpgsqlDbType.Json,JsonConvert.SerializeObject(model.medicalinfo));
else
dbCommand.Parameters.AddWithValue("pvar_medicalinfo",NpgsqlDbType.Json,DBNull.Value);
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
public virtual EnquiryFormModel getById_EnquiryForm(string EnquiryFormid)
									 {
										DataTable dataTable = new DataTable();
										DataSet dataSet = new DataSet();
										try{
												 
												using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
												{
													npsql.Open();
													using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"getById_sp_EnquiryForm\"(@pvar_enquiryformid)", npsql))
													{
														dbCommand.CommandType = CommandType.Text;
														dbCommand.Parameters.AddWithValue("pvar_enquiryformid",(object)EnquiryFormid??DBNull.Value);
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
											return ModelConverter.ConvertDataRowToModel<EnquiryFormModel>(row);
										}
										else
										{
											return null;
										}
									 }
			 public virtual string  Update_Enquiry(EnquiryFormModel model)
			 { 
				 String ResponseMessage="";
					try{
						 	 
							using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
							{
								npsql.Open();
								using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"Update_Enquiry\"(@pvar_enquiryformid,@pvar_tenantid,@pvar_enquirynumber,@pvar_enquirydate,@pvar_enquirytype,@pvar_isroombookingrelated,@pvar_patientname,@pvar_firstname,@pvar_lastname,@pvar_gender,@pvar_age,@pvar_phonenumber,@pvar_emailaddress,@pvar_preferredcontactmethod,@pvar_enquiryreason,@pvar_enquiredvia,@pvar_preferredroomtype,@pvar_preferreddateofarrival,@pvar_preferreddateofdeparture,@pvar_joinwaitinglist,@pvar_enquirystatus,@pvar_verifiedstatus,@pvar_medicalinfo,@pvar_modifieduser)", npsql))
								{
										dbCommand.CommandType = CommandType.Text;
															dbCommand.Parameters.AddWithValue("pvar_enquiryformid",NpgsqlDbType.Uuid,(object)model.EnquiryFormid??DBNull.Value);
dbCommand.Parameters.AddWithValue("pvar_tenantid",NpgsqlDbType.Uuid,(object)model.tenantid??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_enquirynumber",NpgsqlDbType.Varchar,(object)model.enquirynumber??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_enquirydate",NpgsqlDbType.Date,(object)model.enquirydate??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_enquirytype",NpgsqlDbType.Uuid,(object)model.enquirytype??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_isroombookingrelated",NpgsqlDbType.Varchar,(object)model.isroombookingrelated??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_patientname",NpgsqlDbType.Uuid,(object)model.patientname??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_firstname",NpgsqlDbType.Varchar,(object)model.firstname??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_lastname",NpgsqlDbType.Varchar,(object)model.lastname??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_gender",NpgsqlDbType.Varchar,(object)model.gender??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_age",NpgsqlDbType.Bigint,(object)model.age??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_phonenumber",NpgsqlDbType.Varchar,(object)model.phonenumber??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_emailaddress",NpgsqlDbType.Varchar,(object)model.emailaddress??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_preferredcontactmethod",NpgsqlDbType.Varchar,(object)model.preferredcontactmethod??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_enquiryreason",NpgsqlDbType.Varchar,(object)model.enquiryreason??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_enquiredvia",NpgsqlDbType.Varchar,(object)model.enquiredvia??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_preferredroomtype",NpgsqlDbType.Uuid,(object)model.preferredroomtype??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_preferreddateofarrival",NpgsqlDbType.Date,(object)model.preferreddateofarrival??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_preferreddateofdeparture",NpgsqlDbType.Date,(object)model.preferreddateofdeparture??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_joinwaitinglist",NpgsqlDbType.Boolean,(object)model.joinwaitinglist??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_enquirystatus",NpgsqlDbType.Varchar,(object)model.enquirystatus??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_verifiedstatus",NpgsqlDbType.Varchar,(object)model.verifiedstatus??DBNull.Value);
if(model.medicalinfo !=null  && model.medicalinfo.Count >0)
dbCommand.Parameters.AddWithValue("pvar_medicalinfo",NpgsqlDbType.Json,JsonConvert.SerializeObject(model.medicalinfo));
else
dbCommand.Parameters.AddWithValue("pvar_medicalinfo",NpgsqlDbType.Json,DBNull.Value);
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
public virtual string  Remove_Enquiry(string id,string loginUserID)
			  { 
				  String ResponseMessage="";
					try{ 
							using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
							{
								npsql.Open();
								using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"Remove_Enquiry\"(@pvar_enquiryformid,@pvar_modifieduser)", npsql))
								{
										dbCommand.CommandType = CommandType.Text;
										dbCommand.Parameters.AddWithValue("pvar_enquiryformid",(object)id??DBNull.Value);
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
        public virtual JObject Added_Enquiries(
            string tenantid,
            string enquirynumber,
            string patientname,
            string phonenumber,
            string emailaddress,
            string enquirystatus,
            string verifiedstatus,
            int? pagesize = 1000,
            int? pagenumber = 0,
            string searchterm = "",
            string sort_fields = "",
            string createddate_automatonfrom = "",
            string createddate_automatonto = "")
        {
            object dalResponse = null;

            try
            {
                using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
                {
                    using (var dbCommand = new NpgsqlCommand(
                        "SELECT * FROM \"Added_Enquiries\"(" +
                        "@pvar_tenantid," +
                        "@pvar_enquirynumber," +
                        "@pvar_patientname," +
                        "@pvar_phonenumber," +
                        "@pvar_emailaddress," +
                        "@pvar_enquirystatus," +
                        "@pvar_verifiedstatus," +
                        "@pvar_pagesize," +
                        "@pvar_pagenumber," +
                        "@pvar_searchterm," +
                        "@pvar_sort_fields," +
                        "@pvar_createddate_automatonfrom," +
                        "@pvar_createddate_automatonto" +
                        ")", npsql))
                    {
                        dbCommand.CommandType = CommandType.Text;

                        dbCommand.Parameters.AddWithValue("pvar_tenantid", (object)tenantid ?? DBNull.Value);
                        dbCommand.Parameters.AddWithValue("pvar_enquirynumber", (object)enquirynumber ?? DBNull.Value);
                        dbCommand.Parameters.AddWithValue("pvar_patientname", (object)patientname ?? DBNull.Value);
                        dbCommand.Parameters.AddWithValue("pvar_phonenumber", (object)phonenumber ?? DBNull.Value);
                        dbCommand.Parameters.AddWithValue("pvar_emailaddress", (object)emailaddress ?? DBNull.Value);
                        dbCommand.Parameters.AddWithValue("pvar_enquirystatus", (object)enquirystatus ?? DBNull.Value);
                        dbCommand.Parameters.AddWithValue("pvar_verifiedstatus", (object)verifiedstatus ?? DBNull.Value);

                        dbCommand.Parameters.AddWithValue("pvar_pagesize", (object)pagesize ?? DBNull.Value);
                        dbCommand.Parameters.AddWithValue("pvar_pagenumber", (object)pagenumber ?? DBNull.Value);
                        dbCommand.Parameters.AddWithValue("pvar_searchterm", (object)searchterm ?? DBNull.Value);

                        if (sort_fields != null && sort_fields.Length > 2)
                            dbCommand.Parameters.AddWithValue("pvar_sort_fields", NpgsqlDbType.Json, sort_fields);
                        else
                            dbCommand.Parameters.AddWithValue("pvar_sort_fields", NpgsqlDbType.Json, DBNull.Value);

                        dbCommand.Parameters.AddWithValue("pvar_createddate_automatonfrom", (object)createddate_automatonfrom ?? DBNull.Value);
                        dbCommand.Parameters.AddWithValue("pvar_createddate_automatonto", (object)createddate_automatonto ?? DBNull.Value);

                        npsql.Open();
                        dalResponse = dbCommand.ExecuteScalar();
                        npsql.Close();
                    }
                }
            }
            catch
            {
                throw;
            }

            return JObject.Parse(dalResponse.ToString());
        }

        public virtual System.Data.DataTable get_all_EnquiryForm(string tenantid,string searchterm="", int? pagesize=1000, int? pagenumber=0)
			  { 

				    DataTable dataTable = new DataTable();
					DataSet dataSet = new DataSet();

					try{
 
							using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
							{
								npsql.Open();
								using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"get_all_EnquiryForm\"(@pvar_tenantid)", npsql))
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
public virtual System.Data.DataTable count_of_EnquiryForm(string tenantid
,string enquirynumber
,string patientname
,string phonenumber
,string emailaddress
,string enquirystatus
)
			  { 
					DataTable dataTable = new DataTable();
                DataSet dataSet = new DataSet(); 

					try{
 
							using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
							{
								npsql.Open();
								using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"count_of_EnquiryForm\"(@pvar_tenantid,@pvar_enquirynumber,@pvar_patientname,@pvar_phonenumber,@pvar_emailaddress,@pvar_enquirystatus)", npsql))
								{
									dbCommand.CommandType = CommandType.Text;
									dbCommand.Parameters.AddWithValue("pvar_tenantid",(object)tenantid??DBNull.Value);
dbCommand.Parameters.AddWithValue("pvar_enquirynumber",(object)enquirynumber??DBNull.Value);
dbCommand.Parameters.AddWithValue("pvar_patientname",(object)patientname??DBNull.Value);
dbCommand.Parameters.AddWithValue("pvar_phonenumber",(object)phonenumber??DBNull.Value);
dbCommand.Parameters.AddWithValue("pvar_emailaddress",(object)emailaddress??DBNull.Value);
dbCommand.Parameters.AddWithValue("pvar_enquirystatus",(object)enquirystatus??DBNull.Value);

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
			   
			 
public virtual System.Data.DataTable count_of_EnquiryForm_enquirystatus(string tenantid
,string enquirynumber
,string patientname
,string phonenumber
,string emailaddress
,string enquirystatus
)
			  { 
					DataTable dataTable = new DataTable();
                DataSet dataSet = new DataSet(); 

					try{
 
							using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
							{
								npsql.Open();
								using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"count_of_EnquiryForm_enquirystatus\"(@pvar_tenantid,@pvar_enquirynumber,@pvar_patientname,@pvar_phonenumber,@pvar_emailaddress,@pvar_enquirystatus)", npsql))
								{
									dbCommand.CommandType = CommandType.Text;
									dbCommand.Parameters.AddWithValue("pvar_tenantid",(object)tenantid??DBNull.Value);
dbCommand.Parameters.AddWithValue("pvar_enquirynumber",(object)enquirynumber??DBNull.Value);
dbCommand.Parameters.AddWithValue("pvar_patientname",(object)patientname??DBNull.Value);
dbCommand.Parameters.AddWithValue("pvar_phonenumber",(object)phonenumber??DBNull.Value);
dbCommand.Parameters.AddWithValue("pvar_emailaddress",(object)emailaddress??DBNull.Value);
dbCommand.Parameters.AddWithValue("pvar_enquirystatus",(object)enquirystatus??DBNull.Value);

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
			   
			 
public virtual JObject Enquiries_for_Review(string tenantid
,string enquirystatus
,string verifiedstatus
, int? pagesize=1000 , int? pagenumber=0,string searchterm="",string  sort_fields="")
			  { 
				  object dalResponse = null;
			
					try{
 
							using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
							{
								
								using (var dbCommand = new NpgsqlCommand("SELECT * FROM\"Enquiries_for_Review\"(@pvar_tenantid,@pvar_enquirystatus,@pvar_verifiedstatus,@pvar_pagesize,@pvar_pagenumber,@pvar_searchterm,@pvar_sort_fields)", npsql))
								{
									dbCommand.CommandType = CommandType.Text;
									dbCommand.Parameters.AddWithValue("pvar_tenantid",(object)tenantid??DBNull.Value);
dbCommand.Parameters.AddWithValue("pvar_enquirystatus",(object)enquirystatus??DBNull.Value);
dbCommand.Parameters.AddWithValue("pvar_verifiedstatus",(object)verifiedstatus??DBNull.Value);

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
			   
			 
public virtual JObject Enquiries(string tenantid
,string enquirystatus
, int? pagesize=1000 , int? pagenumber=0,string searchterm="",string  sort_fields="")
			  { 
				  object dalResponse = null;
			
					try{
 
							using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
							{
								
								using (var dbCommand = new NpgsqlCommand("SELECT * FROM\"Enquiries\"(@pvar_tenantid,@pvar_enquirystatus,@pvar_pagesize,@pvar_pagenumber,@pvar_searchterm,@pvar_sort_fields)", npsql))
								{
									dbCommand.CommandType = CommandType.Text;
									dbCommand.Parameters.AddWithValue("pvar_tenantid",(object)tenantid??DBNull.Value);
dbCommand.Parameters.AddWithValue("pvar_enquirystatus",(object)enquirystatus??DBNull.Value);

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
			   
			 
public virtual System.Data.DataTable getById_allinfo_EnquiryForm(string EnquiryFormid)
			 {
				DataSet dataSet=new DataSet();
				DataTable dataTable = new DataTable();
				try{
					     
						using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
						{
							npsql.Open();
							using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"getById_sp_all_EnquiryForm\"(@pvar_enquiryformid)", npsql))
							{
								dbCommand.CommandType = CommandType.Text;
								dbCommand.Parameters.AddWithValue("pvar_enquiryformid",(object)EnquiryFormid??DBNull.Value);
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
			  public virtual string  verify_EnquiryForm(EnquiryFormReviewModel model)
			 { 
				 String ResponseMessage="";
					try{
						 
							 
							using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
							{
								npsql.Open();
								using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"verify_EnquiryForm\"(@pvar_enquiryformid,@pvar_verifiedby,@pvar_verifiedstatus,@pvar_reviewcomments)", npsql))
								{
										dbCommand.CommandType = CommandType.Text;
										 dbCommand.Parameters.AddWithValue("pvar_enquiryformid",(object)model.EnquiryFormid??DBNull.Value);
										dbCommand.Parameters.AddWithValue("pvar_verifiedby",(object)model.verifiedby??DBNull.Value);
										dbCommand.Parameters.AddWithValue("pvar_verifiedstatus",(object)model.verifiedstatus??DBNull.Value);
										dbCommand.Parameters.AddWithValue("pvar_reviewcomments",(object)model.reviewcomments??DBNull.Value); 
											
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
public virtual System.Data.DataTable lookup_EnquiryForm_enquirytype(String tenantid)
							        {
                                            DataSet dataSet = new DataSet();
									        DataTable dataTable=new DataTable();
									        try{

                                        		        using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
					                                    {
						                                    npsql.Open();
						                                    using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"lookup_EnquiryForm_enquirytype\"(@pvar_tenantid)", npsql))
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
public virtual System.Data.DataTable lookup_EnquiryForm_patientname(String tenantid,string searchterm="", int? pagesize=1000, int? pagenumber=0)
							        {
                                            DataSet dataSet = new DataSet();
									        DataTable dataTable=new DataTable();
									        try{

                                        		        using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
					                                    {
						                                    npsql.Open();
						                                    using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"lookup_EnquiryForm_patientname\"(@pvar_tenantid,@pvar_searchterm,@pvar_pagesize,@pvar_pagenumber)", npsql))
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
public virtual System.Data.DataTable lookup_EnquiryForm_preferredroomtype(String tenantid)
							        {
                                            DataSet dataSet = new DataSet();
									        DataTable dataTable=new DataTable();
									        try{

                                        		        using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
					                                    {
						                                    npsql.Open();
						                                    using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"lookup_EnquiryForm_preferredroomtype\"(@pvar_tenantid)", npsql))
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
public virtual System.Data.DataTable lookup_EnquiryForm_medicalinfo_medicalcondition(string searchterm="", int? pagesize=1000, int? pagenumber=0)
							        {
                                            DataSet dataSet = new DataSet();
									        DataTable dataTable=new DataTable();
									        try{

                                        		        using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
					                                    {
						                                    npsql.Open();
						                                    using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"lookup_EnquiryForm_medicalinfo_medicalcondition\"(@pvar_searchterm,@pvar_pagesize,@pvar_pagenumber)", npsql))
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


						public virtual System.Data.DataTable lookup_change_EnquiryForm_enquirytype(string EnquiryTypeid)
						{
							DataTable dataTable=new DataTable();
                            DataSet dataSet=new DataSet();
							try
							{
								 	 
                                    using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
									{
										npsql.Open();
										using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"lookup_change_EnquiryForm_enquirytype\"(@pvar_enquirytypeid)", npsql))
										{
											dbCommand.CommandType = CommandType.Text;
											dbCommand.Parameters.AddWithValue("pvar_enquirytypeid",NpgsqlDbType.Varchar,(object)EnquiryTypeid??DBNull.Value);
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

public virtual System.Data.DataTable lookup_change_medicalinfo_EnquiryForm_medicalcondition(string MedicalConditionid)
						{
							DataTable dataTable=new DataTable();
                            DataSet dataSet=new DataSet();
								try
								{
								 
                                         using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
                                        {
	                                        npsql.Open();
	                                        using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"lookup_change_medicalinfo_EnquiryForm_medicalcondition\"(@pvar_medicalconditionid)", npsql))
	                                        {
		                                        dbCommand.CommandType = CommandType.Text;
		                                        dbCommand.Parameters.AddWithValue("pvar_medicalconditionid",NpgsqlDbType.Varchar,(object)MedicalConditionid??DBNull.Value);
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
