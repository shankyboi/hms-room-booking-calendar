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

			    //This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:40:58
			    public class PatientProfileDAL
			    {
					public virtual string db_connectionstring{get;set;}
					
			 	    public PatientProfileDAL(string connectionString)
				    {
						
					    db_connectionstring=connectionString;
				    }
				  
			        public virtual System.Data.DataTable getById_emergencycontactinfo(string PatientProfileid)
			 {
					DataTable dataTable=new DataTable();
					DataSet dataSet=new DataSet();
					try
					{
						  
							using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
							{
								npsql.Open();
								using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"getById_sp_PatientProfile_emergencycontactinfo\"(@pvar_patientprofileid)", npsql))
								{
									dbCommand.CommandType = CommandType.Text;
									dbCommand.Parameters.AddWithValue("pvar_patientprofileid",(object)PatientProfileid??DBNull.Value);
								
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

		public virtual string Update_Emergency_Contact_Info(PatientProfileEmergencyContactUpdateModel model, Guid modifieduser)
		{
			try
			{
				using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
				using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"Update_PatientProfile_EmergencyContactInfo\"(@pvar_patientprofileid,@pvar_emergencycontactinfo,@pvar_modifieduser)", npsql))
				{
					dbCommand.CommandType = CommandType.Text;
					dbCommand.Parameters.AddWithValue("pvar_patientprofileid", NpgsqlDbType.Uuid, model.PatientProfileid);
					dbCommand.Parameters.AddWithValue("pvar_emergencycontactinfo", NpgsqlDbType.Json, JsonConvert.SerializeObject(model.emergencycontactinfo));
					dbCommand.Parameters.AddWithValue("pvar_modifieduser", NpgsqlDbType.Uuid, modifieduser);

					npsql.Open();
					return Convert.ToString(dbCommand.ExecuteScalar());
				}
			}
			catch
			{
				throw;
			}
		}


              public virtual string Add_Patient_Profile(PatientProfileModel model)
			  { 
				  String ResponseMessage="";
					 
					try{
                RandomStringGenerator objRandom = new RandomStringGenerator();
                string plainPassword = objRandom.CreateRandomPassword(8);
                string passwordKey = objRandom.CreateRandomPassword(8);
                string encryptedPassword = PwdEncrypDecrypt.Encrypt(passwordKey, plainPassword);
                Guid usersid = Guid.NewGuid();

                using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
					        {
						        npsql.Open();
						        using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"Add_Patient_Profile\"(@pvar_patientprofileid,@pvar_tenantid,@pvar_registrationid,@pvar_firstname,@pvar_lastname,@pvar_gender,@pvar_dateofbirth,@pvar_age,@pvar_bloodgroup,@pvar_nationality,@pvar_countryoforigin,@pvar_emailaddress,@pvar_mobilenumber,@pvar_whatsappnumber,@pvar_photo,@pvar_paddressline1,@pvar_paddressline2,@pvar_pzip,@pvar_ptown,@pvar_pcityordistrict,@pvar_ppstatename,@pvar_sameaspermanentaddress,@pvar_caddressline1,@pvar_caddressline2,@pvar_czip,@pvar_ctown,@pvar_ccityordistrict,@pvar_cstatename,@pvar_idprooftype,@pvar_idproofnumber,@pvar_uploadidproof,@pvar_languagesknown,@pvar_languagespreferrable,@pvar_otherlanguages,@pvar_maritalstatus,@pvar_education,@pvar_occupation,@pvar_meditationpractice,@pvar_typeofpractice,@pvar_creativeactivities,@pvar_othercreativeactivities,@pvar_insurancetype,@pvar_insurancecompany,@pvar_policynumber,@pvar_policyclaimlimit,@pvar_policyexpirydate,@pvar_referralsource,@pvar_referraltype,@pvar_referrername,@pvar_referrerphonenumber,@pvar_magazinename,@pvar_socialmediaplatform,@pvar_otherreferral,@pvar_emergencycontactinfo,@pvar_createduser,@pvar_userpassword,@pvar_passwordkey,@pvar_usersid)", npsql))
						        {
                                        dbCommand.CommandType = CommandType.Text;
						            	
								        					dbCommand.Parameters.AddWithValue("pvar_patientprofileid",NpgsqlDbType.Uuid,(object)model.PatientProfileid??DBNull.Value);
dbCommand.Parameters.AddWithValue("pvar_tenantid",NpgsqlDbType.Uuid,(object)model.tenantid??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_registrationid",NpgsqlDbType.Varchar,(object)model.registrationid??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_firstname",NpgsqlDbType.Varchar,(object)model.firstname??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_lastname",NpgsqlDbType.Varchar,(object)model.lastname??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_gender",NpgsqlDbType.Varchar,(object)model.gender??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_dateofbirth",NpgsqlDbType.Date,(object)model.dateofbirth??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_age",NpgsqlDbType.Integer,(object)model.age??DBNull.Value);
						dbCommand.Parameters.AddWithValue("pvar_bloodgroup", NpgsqlDbType.Varchar, (object)model.bloodgroup ?? DBNull.Value);

						dbCommand.Parameters.AddWithValue("pvar_nationality",NpgsqlDbType.Varchar,(object)model.nationality??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_countryoforigin",NpgsqlDbType.Uuid,(object)model.countryoforigin??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_emailaddress",NpgsqlDbType.Varchar,(object)model.emailaddress??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_mobilenumber",NpgsqlDbType.Varchar,(object)model.mobilenumber??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_whatsappnumber",NpgsqlDbType.Varchar,(object)model.whatsappnumber??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_photo",NpgsqlDbType.Varchar,(object)model.photo??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_paddressline1",NpgsqlDbType.Varchar,(object)model.paddressline1??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_paddressline2",NpgsqlDbType.Varchar,(object)model.paddressline2??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_pzip",NpgsqlDbType.Integer,(object)model.pzip??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_ptown",NpgsqlDbType.Varchar,(object)model.ptown??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_pcityordistrict",NpgsqlDbType.Varchar,(object)model.pcityordistrict??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_ppstatename",NpgsqlDbType.Varchar,(object)model.ppstatename??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_sameaspermanentaddress",NpgsqlDbType.Boolean,(object)model.sameaspermanentaddress??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_caddressline1",NpgsqlDbType.Varchar,(object)model.caddressline1??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_caddressline2",NpgsqlDbType.Varchar,(object)model.caddressline2??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_czip",NpgsqlDbType.Integer,(object)model.czip??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_ctown",NpgsqlDbType.Varchar,(object)model.ctown??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_ccityordistrict",NpgsqlDbType.Varchar,(object)model.ccityordistrict??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_cstatename",NpgsqlDbType.Varchar,(object)model.cstatename??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_idprooftype",NpgsqlDbType.Varchar,(object)model.idprooftype??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_idproofnumber",NpgsqlDbType.Varchar,(object)model.idproofnumber??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_uploadidproof",NpgsqlDbType.Varchar,(object)model.uploadidproof??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_languagesknown",NpgsqlDbType.Varchar,(object)model.languagesknown??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_languagespreferrable",NpgsqlDbType.Varchar,(object)model.languagespreferrable??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_otherlanguages",NpgsqlDbType.Varchar,(object)model.otherlanguages??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_maritalstatus",NpgsqlDbType.Varchar,(object)model.maritalstatus??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_education",NpgsqlDbType.Varchar,(object)model.education??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_occupation",NpgsqlDbType.Uuid,(object)model.occupation??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_meditationpractice",NpgsqlDbType.Varchar,(object)model.meditationpractice??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_typeofpractice",NpgsqlDbType.Varchar,(object)model.typeofpractice??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_creativeactivities",NpgsqlDbType.Varchar,(object)model.creativeactivities??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_othercreativeactivities",NpgsqlDbType.Varchar,(object)model.othercreativeactivities??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_insurancetype",NpgsqlDbType.Varchar,(object)model.insurancetype??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_insurancecompany",NpgsqlDbType.Varchar,(object)model.insurancecompany??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_policynumber",NpgsqlDbType.Varchar,(object)model.policynumber??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_policyclaimlimit",NpgsqlDbType.Numeric,(object)model.policyclaimlimit??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_policyexpirydate",NpgsqlDbType.Date,(object)model.policyexpirydate??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_referralsource",NpgsqlDbType.Uuid,(object)model.referralsource??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_referraltype",NpgsqlDbType.Varchar,(object)model.referraltype??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_referrername",NpgsqlDbType.Varchar,(object)model.referrername??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_referrerphonenumber",NpgsqlDbType.Varchar,(object)model.referrerphonenumber??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_magazinename",NpgsqlDbType.Varchar,(object)model.magazinename??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_socialmediaplatform",NpgsqlDbType.Varchar,(object)model.socialmediaplatform??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_otherreferral",NpgsqlDbType.Varchar,(object)model.otherreferral??DBNull.Value);
if(model.emergencycontactinfo !=null  && model.emergencycontactinfo.Count >0)
dbCommand.Parameters.AddWithValue("pvar_emergencycontactinfo",NpgsqlDbType.Json,JsonConvert.SerializeObject(model.emergencycontactinfo));
else
dbCommand.Parameters.AddWithValue("pvar_emergencycontactinfo",NpgsqlDbType.Json,DBNull.Value);
dbCommand.Parameters.AddWithValue("pvar_createduser",NpgsqlDbType.Uuid,(object)model.createduser??DBNull.Value);
                        dbCommand.Parameters.AddWithValue("pvar_userpassword", NpgsqlDbType.Varchar, encryptedPassword);
                        dbCommand.Parameters.AddWithValue("pvar_passwordkey", NpgsqlDbType.Varchar, passwordKey);
                        dbCommand.Parameters.AddWithValue("pvar_usersid", NpgsqlDbType.Uuid, usersid);


                        NpgsqlParameter outParm = new NpgsqlParameter("pvar_returnMessage", NpgsqlDbType.Varchar)
                                        {
                                             Direction = ParameterDirection.Output
                                        };
                                        dbCommand.Parameters.Add(outParm);

                                        dbCommand.ExecuteNonQuery();

                        string spResponse = outParm.Value.ToString();
                        if (spResponse.Contains("201.1"))
                            ResponseMessage = spResponse + "|" + model.emailaddress + "|" + plainPassword;
                        else
                            ResponseMessage = spResponse;

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
public virtual PatientProfileModel getById_PatientProfile(string PatientProfileid)
									 {
										DataTable dataTable = new DataTable();
										DataSet dataSet = new DataSet();
										try{
												 
												using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
												{
													npsql.Open();
													using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"getById_sp_PatientProfile\"(@pvar_patientprofileid)", npsql))
													{
														dbCommand.CommandType = CommandType.Text;
														dbCommand.Parameters.AddWithValue("pvar_patientprofileid",(object)PatientProfileid??DBNull.Value);
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
											return ModelConverter.ConvertDataRowToModel<PatientProfileModel>(row);
										}
										else
										{
											return null;
										}
									 }
			 public virtual string  Add_to_Blacklist(PatientProfileModel model)
			 { 
				 String ResponseMessage="";
					try{
						 	 
							using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
							{
								npsql.Open();
								using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"Add_to_Blacklist\"(@pvar_patientprofileid,@pvar_tenantid,@pvar_registrationid,@pvar_blacklisted,@pvar_reasonforblacklisting,@pvar_detailedremarks,@pvar_modifieduser)", npsql))
								{
										dbCommand.CommandType = CommandType.Text;
															dbCommand.Parameters.AddWithValue("pvar_patientprofileid",NpgsqlDbType.Uuid,(object)model.PatientProfileid??DBNull.Value);
dbCommand.Parameters.AddWithValue("pvar_tenantid",NpgsqlDbType.Uuid,(object)model.tenantid??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_registrationid",NpgsqlDbType.Varchar,(object)model.registrationid??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_blacklisted",NpgsqlDbType.Varchar,(object)model.blacklisted??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_reasonforblacklisting",NpgsqlDbType.Uuid,(object)model.reasonforblacklisting??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_detailedremarks",NpgsqlDbType.Varchar,(object)model.detailedremarks??DBNull.Value);
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
		public virtual string Modify_Patient_Category(PatientProfileModel model)
		{
			String ResponseMessage = "";
			try
			{

				using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
				{
					npsql.Open();
					using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"Modify_Patient_Category\"(@pvar_patientprofileid,@pvar_tenantid,@pvar_patientcategory,@pvar_modifieduser)", npsql))
					{
						dbCommand.CommandType = CommandType.Text;
						dbCommand.Parameters.AddWithValue("pvar_patientprofileid", NpgsqlDbType.Uuid, (object)model.PatientProfileid ?? DBNull.Value);
						dbCommand.Parameters.AddWithValue("pvar_tenantid", NpgsqlDbType.Uuid, (object)model.tenantid ?? DBNull.Value);

						 
						dbCommand.Parameters.AddWithValue("pvar_patientcategory", NpgsqlDbType.Uuid, (object)model.patientcategory ?? DBNull.Value);
						dbCommand.Parameters.AddWithValue("pvar_modifieduser", NpgsqlDbType.Uuid, model.modifieduser);

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

			}
			catch (Exception ex)
			{
				ResponseMessage = ex.Message;
			}

			return ResponseMessage;

		}

		public virtual string  Mark_as_Deceased(PatientProfileModel model)
			 { 
				 String ResponseMessage="";
					try{
						 	 
							using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
							{
								npsql.Open();
								using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"Mark_as_Deceased\"(@pvar_patientprofileid,@pvar_tenantid,@pvar_registrationid,@pvar_deceased,@pvar_causeofdeath,@pvar_dateandtimeofdeath,@pvar_modifieduser)", npsql))
								{
										dbCommand.CommandType = CommandType.Text;
															dbCommand.Parameters.AddWithValue("pvar_patientprofileid",NpgsqlDbType.Uuid,(object)model.PatientProfileid??DBNull.Value);
dbCommand.Parameters.AddWithValue("pvar_tenantid",NpgsqlDbType.Uuid,(object)model.tenantid??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_registrationid",NpgsqlDbType.Varchar,(object)model.registrationid??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_deceased",NpgsqlDbType.Boolean,(object)model.deceased??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_causeofdeath",NpgsqlDbType.Varchar,(object)model.causeofdeath??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_dateandtimeofdeath",NpgsqlDbType.Timestamp,(object)model.dateandtimeofdeath??DBNull.Value);
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

			 public virtual string  Update_Patient_Profile(PatientProfileModel model)
			 { 
				 String ResponseMessage="";
					try{
						 	 
							using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
							{
								npsql.Open();
								using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"Update_Patient_Profile\"(@pvar_patientprofileid,@pvar_tenantid,@pvar_registrationid,@pvar_firstname,@pvar_lastname,@pvar_gender,@pvar_dateofbirth,@pvar_age,@pvar_bloodgroup,@pvar_nationality,@pvar_countryoforigin,@pvar_emailaddress,@pvar_mobilenumber,@pvar_whatsappnumber,@pvar_photo,@pvar_paddressline1,@pvar_paddressline2,@pvar_pzip,@pvar_ptown,@pvar_pcityordistrict,@pvar_ppstatename,@pvar_sameaspermanentaddress,@pvar_caddressline1,@pvar_caddressline2,@pvar_czip,@pvar_ctown,@pvar_ccityordistrict,@pvar_cstatename,@pvar_idprooftype,@pvar_idproofnumber,@pvar_uploadidproof,@pvar_languagesknown,@pvar_languagespreferrable,@pvar_otherlanguages,@pvar_maritalstatus,@pvar_education,@pvar_occupation,@pvar_meditationpractice,@pvar_typeofpractice,@pvar_creativeactivities,@pvar_othercreativeactivities,@pvar_insurancetype,@pvar_insurancecompany,@pvar_policynumber,@pvar_policyclaimlimit,@pvar_policyexpirydate,@pvar_referralsource,@pvar_referraltype,@pvar_referrername,@pvar_referrerphonenumber,@pvar_magazinename,@pvar_socialmediaplatform,@pvar_otherreferral,@pvar_emergencycontactinfo,@pvar_modifieduser)", npsql))
								{
										dbCommand.CommandType = CommandType.Text;
															dbCommand.Parameters.AddWithValue("pvar_patientprofileid",NpgsqlDbType.Uuid,(object)model.PatientProfileid??DBNull.Value);
dbCommand.Parameters.AddWithValue("pvar_tenantid",NpgsqlDbType.Uuid,(object)model.tenantid??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_registrationid",NpgsqlDbType.Varchar,(object)model.registrationid??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_firstname",NpgsqlDbType.Varchar,(object)model.firstname??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_lastname",NpgsqlDbType.Varchar,(object)model.lastname??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_gender",NpgsqlDbType.Varchar,(object)model.gender??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_dateofbirth",NpgsqlDbType.Date,(object)model.dateofbirth??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_age",NpgsqlDbType.Integer,(object)model.age??DBNull.Value);

						dbCommand.Parameters.AddWithValue("pvar_bloodgroup", NpgsqlDbType.Varchar, (object)model.bloodgroup ?? DBNull.Value);
						dbCommand.Parameters.AddWithValue("pvar_nationality",NpgsqlDbType.Varchar,(object)model.nationality??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_countryoforigin",NpgsqlDbType.Uuid,(object)model.countryoforigin??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_emailaddress",NpgsqlDbType.Varchar,(object)model.emailaddress??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_mobilenumber",NpgsqlDbType.Varchar,(object)model.mobilenumber??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_whatsappnumber",NpgsqlDbType.Varchar,(object)model.whatsappnumber??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_photo",NpgsqlDbType.Varchar,(object)model.photo??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_paddressline1",NpgsqlDbType.Varchar,(object)model.paddressline1??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_paddressline2",NpgsqlDbType.Varchar,(object)model.paddressline2??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_pzip",NpgsqlDbType.Integer,(object)model.pzip??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_ptown",NpgsqlDbType.Varchar,(object)model.ptown??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_pcityordistrict",NpgsqlDbType.Varchar,(object)model.pcityordistrict??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_ppstatename",NpgsqlDbType.Varchar,(object)model.ppstatename??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_sameaspermanentaddress",NpgsqlDbType.Boolean,(object)model.sameaspermanentaddress??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_caddressline1",NpgsqlDbType.Varchar,(object)model.caddressline1??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_caddressline2",NpgsqlDbType.Varchar,(object)model.caddressline2??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_czip",NpgsqlDbType.Integer,(object)model.czip??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_ctown",NpgsqlDbType.Varchar,(object)model.ctown??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_ccityordistrict",NpgsqlDbType.Varchar,(object)model.ccityordistrict??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_cstatename",NpgsqlDbType.Varchar,(object)model.cstatename??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_idprooftype",NpgsqlDbType.Varchar,(object)model.idprooftype??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_idproofnumber",NpgsqlDbType.Varchar,(object)model.idproofnumber??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_uploadidproof",NpgsqlDbType.Varchar,(object)model.uploadidproof??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_languagesknown",NpgsqlDbType.Varchar,(object)model.languagesknown??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_languagespreferrable",NpgsqlDbType.Varchar,(object)model.languagespreferrable??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_otherlanguages",NpgsqlDbType.Varchar,(object)model.otherlanguages??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_maritalstatus",NpgsqlDbType.Varchar,(object)model.maritalstatus??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_education",NpgsqlDbType.Varchar,(object)model.education??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_occupation",NpgsqlDbType.Uuid,(object)model.occupation??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_meditationpractice",NpgsqlDbType.Varchar,(object)model.meditationpractice??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_typeofpractice",NpgsqlDbType.Varchar,(object)model.typeofpractice??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_creativeactivities",NpgsqlDbType.Varchar,(object)model.creativeactivities??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_othercreativeactivities",NpgsqlDbType.Varchar,(object)model.othercreativeactivities??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_insurancetype",NpgsqlDbType.Varchar,(object)model.insurancetype??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_insurancecompany",NpgsqlDbType.Varchar,(object)model.insurancecompany??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_policynumber",NpgsqlDbType.Varchar,(object)model.policynumber??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_policyclaimlimit",NpgsqlDbType.Numeric,(object)model.policyclaimlimit??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_policyexpirydate",NpgsqlDbType.Date,(object)model.policyexpirydate??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_referralsource",NpgsqlDbType.Uuid,(object)model.referralsource??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_referraltype",NpgsqlDbType.Varchar,(object)model.referraltype??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_referrername",NpgsqlDbType.Varchar,(object)model.referrername??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_referrerphonenumber",NpgsqlDbType.Varchar,(object)model.referrerphonenumber??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_magazinename",NpgsqlDbType.Varchar,(object)model.magazinename??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_socialmediaplatform",NpgsqlDbType.Varchar,(object)model.socialmediaplatform??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_otherreferral",NpgsqlDbType.Varchar,(object)model.otherreferral??DBNull.Value);
if(model.emergencycontactinfo !=null  && model.emergencycontactinfo.Count >0)
dbCommand.Parameters.AddWithValue("pvar_emergencycontactinfo",NpgsqlDbType.Json,JsonConvert.SerializeObject(model.emergencycontactinfo));
else
dbCommand.Parameters.AddWithValue("pvar_emergencycontactinfo",NpgsqlDbType.Json,DBNull.Value);
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
public virtual string  Remove_Patient_Profile(string id,string loginUserID)
			  { 
				  String ResponseMessage="";
					try{ 
							using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
							{
								npsql.Open();
								using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"Remove_Patient_Profile\"(@pvar_patientprofileid,@pvar_modifieduser)", npsql))
								{
										dbCommand.CommandType = CommandType.Text;
										dbCommand.Parameters.AddWithValue("pvar_patientprofileid",(object)id??DBNull.Value);
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
public virtual JObject Patient_Profiles(string tenantid
, int? pagesize=1000 , int? pagenumber=0,string searchterm="",string  sort_fields="")
			  { 
				  object dalResponse = null;
			
					try{
 
							using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
							{
								
								using (var dbCommand = new NpgsqlCommand("SELECT * FROM\"Patient_Profiles\"(@pvar_tenantid,@pvar_pagesize,@pvar_pagenumber,@pvar_searchterm,@pvar_sort_fields)", npsql))
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
		public virtual JObject Patient_Profile_List(string tenantid
, string firstname
, string dateofbirth_automatonfrom
, string dateofbirth_automatonto
, string emailaddress
, string mobilenumber
, string whatsappnumber
, string idprooftype
, string idproofnumber
, string pzip
, string blacklisted
, string deceased
, int? pagesize = 1000, int? pagenumber = 0, string searchterm = "", string sort_fields = "")
		{
			object dalResponse = null;

			try
			{

				using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
				{

					using (var dbCommand = new NpgsqlCommand("SELECT * FROM\"Patient_Profile_List\"(@pvar_tenantid,@pvar_firstname,@pvar_dateofbirth_automatonfrom,@pvar_dateofbirth_automatonto,@pvar_emailaddress,@pvar_mobilenumber,@pvar_whatsappnumber,@pvar_idprooftype,@pvar_idproofnumber,@pvar_pzip,@pvar_blacklisted,@pvar_deceased,@pvar_pagesize,@pvar_pagenumber,@pvar_searchterm,@pvar_sort_fields)", npsql))
					{
						dbCommand.CommandType = CommandType.Text;
						dbCommand.Parameters.AddWithValue("pvar_tenantid", (object)tenantid ?? DBNull.Value);
						dbCommand.Parameters.AddWithValue("pvar_firstname", (object)firstname ?? DBNull.Value);
						dbCommand.Parameters.AddWithValue("pvar_dateofbirth_automatonfrom", (object)dateofbirth_automatonfrom ?? DBNull.Value);
						dbCommand.Parameters.AddWithValue("pvar_dateofbirth_automatonto", (object)dateofbirth_automatonto ?? DBNull.Value);
						dbCommand.Parameters.AddWithValue("pvar_emailaddress", (object)emailaddress ?? DBNull.Value);
						dbCommand.Parameters.AddWithValue("pvar_mobilenumber", (object)mobilenumber ?? DBNull.Value);
						dbCommand.Parameters.AddWithValue("pvar_whatsappnumber", (object)whatsappnumber ?? DBNull.Value);
						dbCommand.Parameters.AddWithValue("pvar_idprooftype", (object)idprooftype ?? DBNull.Value);
						dbCommand.Parameters.AddWithValue("pvar_idproofnumber", (object)idproofnumber ?? DBNull.Value);
						dbCommand.Parameters.AddWithValue("pvar_pzip", (object)pzip ?? DBNull.Value);
						dbCommand.Parameters.AddWithValue("pvar_blacklisted", (object)blacklisted ?? DBNull.Value);
						dbCommand.Parameters.AddWithValue("pvar_deceased", (object)deceased ?? DBNull.Value);

						dbCommand.Parameters.AddWithValue("pvar_pagesize", (object)pagesize ?? DBNull.Value);
						dbCommand.Parameters.AddWithValue("pvar_pagenumber", (object)pagenumber ?? DBNull.Value);

						dbCommand.Parameters.AddWithValue("pvar_searchterm", (object)searchterm ?? DBNull.Value);
						if (sort_fields != null && sort_fields.Length > 2)
							dbCommand.Parameters.AddWithValue("pvar_sort_fields", NpgsqlDbType.Json, sort_fields);
						else
							dbCommand.Parameters.AddWithValue("pvar_sort_fields", NpgsqlDbType.Json, DBNull.Value);


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
		public virtual string Quick_Add_Patient_Profile(PatientProfileModel model, out string plainPassword)
        {
            string responseMessage = "";
            plainPassword = "";

            try
            {
                // Generate plain password and encrypt � same pattern as Register_Profile
                RandomStringGenerator objRandom = new RandomStringGenerator();
                plainPassword = objRandom.CreateRandomPassword(8);
                string user_passwordkey = objRandom.CreateRandomPassword(8);
                string user_password = PwdEncrypDecrypt.Encrypt(user_passwordkey, plainPassword);

                using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
                {
                    npsql.Open();
                    using (var dbCommand = new NpgsqlCommand(
                        "SELECT * FROM \"Quick_Add_Patient_Profile\"(" +
                        "@pvar_patientprofileid," +
                        "@pvar_tenantid," +
                        "@pvar_firstname," +
                        "@pvar_lastname," +
                        "@pvar_gender," +
                        "@pvar_dateofbirth," +
                        "@pvar_age," +
                        "@pvar_nationality," +
                        "@pvar_emailaddress," +
                        "@pvar_mobilenumber," +
                        "@pvar_whatsappnumber," +
                        "@pvar_photo," +
                        "@pvar_paddressline1," +
                        "@pvar_paddressline2," +
                        "@pvar_pzip," +
                        "@pvar_ptown," +
                        "@pvar_pcityordistrict," +
                        "@pvar_ppstatename," +
                        "@pvar_sameaspermanentaddress," +
                        "@pvar_caddressline1," +
                        "@pvar_caddressline2," +
                        "@pvar_czip," +
                        "@pvar_ctown," +
                        "@pvar_ccityordistrict," +
                        "@pvar_cstatename," +
                        "@pvar_idprooftype," +
                        "@pvar_idproofnumber," +
                        "@pvar_uploadidproof," +
                        "@pvar_userpassword," +
						"@pvar_passwordkey,@pvar_countryoforigin,@pvar_bloodgroup)", npsql))
                    {
                        dbCommand.CommandType = CommandType.Text;

                        dbCommand.Parameters.AddWithValue("pvar_patientprofileid", NpgsqlDbType.Uuid, (object)model.PatientProfileid ?? DBNull.Value);
                        dbCommand.Parameters.AddWithValue("pvar_tenantid", NpgsqlDbType.Uuid, (object)model.tenantid ?? DBNull.Value);
                        dbCommand.Parameters.AddWithValue("pvar_firstname", NpgsqlDbType.Varchar, (object)model.firstname ?? DBNull.Value);
                        dbCommand.Parameters.AddWithValue("pvar_lastname", NpgsqlDbType.Varchar, (object)model.lastname ?? DBNull.Value);
                        dbCommand.Parameters.AddWithValue("pvar_gender", NpgsqlDbType.Varchar, (object)model.gender ?? DBNull.Value);
                        dbCommand.Parameters.AddWithValue("pvar_dateofbirth", NpgsqlDbType.Date, (object)model.dateofbirth ?? DBNull.Value);
                        dbCommand.Parameters.AddWithValue("pvar_age", NpgsqlDbType.Integer, (object)model.age ?? DBNull.Value);
                        dbCommand.Parameters.AddWithValue("pvar_nationality", NpgsqlDbType.Varchar, (object)model.nationality ?? DBNull.Value);
                        dbCommand.Parameters.AddWithValue("pvar_emailaddress", NpgsqlDbType.Varchar, (object)model.emailaddress ?? DBNull.Value);
                        dbCommand.Parameters.AddWithValue("pvar_mobilenumber", NpgsqlDbType.Varchar, (object)model.mobilenumber ?? DBNull.Value);
                        dbCommand.Parameters.AddWithValue("pvar_whatsappnumber", NpgsqlDbType.Varchar, (object)model.whatsappnumber ?? DBNull.Value);
                        dbCommand.Parameters.AddWithValue("pvar_photo", NpgsqlDbType.Varchar, (object)model.photo ?? DBNull.Value);
                        dbCommand.Parameters.AddWithValue("pvar_paddressline1", NpgsqlDbType.Varchar, (object)model.paddressline1 ?? DBNull.Value);
                        dbCommand.Parameters.AddWithValue("pvar_paddressline2", NpgsqlDbType.Varchar, (object)model.paddressline2 ?? DBNull.Value);
                        dbCommand.Parameters.AddWithValue("pvar_pzip", NpgsqlDbType.Integer, (object)model.pzip ?? DBNull.Value);
                        dbCommand.Parameters.AddWithValue("pvar_ptown", NpgsqlDbType.Varchar, (object)model.ptown ?? DBNull.Value);
                        dbCommand.Parameters.AddWithValue("pvar_pcityordistrict", NpgsqlDbType.Varchar, (object)model.pcityordistrict ?? DBNull.Value);
                        dbCommand.Parameters.AddWithValue("pvar_ppstatename", NpgsqlDbType.Varchar, (object)model.ppstatename ?? DBNull.Value);
                        dbCommand.Parameters.AddWithValue("pvar_sameaspermanentaddress", NpgsqlDbType.Boolean, model.sameaspermanentaddress);
                        dbCommand.Parameters.AddWithValue("pvar_caddressline1", NpgsqlDbType.Varchar, (object)model.caddressline1 ?? DBNull.Value);
                        dbCommand.Parameters.AddWithValue("pvar_caddressline2", NpgsqlDbType.Varchar, (object)model.caddressline2 ?? DBNull.Value);
                        dbCommand.Parameters.AddWithValue("pvar_czip", NpgsqlDbType.Integer, (object)model.czip ?? DBNull.Value);
                        dbCommand.Parameters.AddWithValue("pvar_ctown", NpgsqlDbType.Varchar, (object)model.ctown ?? DBNull.Value);
                        dbCommand.Parameters.AddWithValue("pvar_ccityordistrict", NpgsqlDbType.Varchar, (object)model.ccityordistrict ?? DBNull.Value);
                        dbCommand.Parameters.AddWithValue("pvar_cstatename", NpgsqlDbType.Varchar, (object)model.cstatename ?? DBNull.Value);
                        dbCommand.Parameters.AddWithValue("pvar_idprooftype", NpgsqlDbType.Varchar, (object)model.idprooftype ?? DBNull.Value);
                        dbCommand.Parameters.AddWithValue("pvar_idproofnumber", NpgsqlDbType.Varchar, (object)model.idproofnumber ?? DBNull.Value);
                        dbCommand.Parameters.AddWithValue("pvar_uploadidproof", NpgsqlDbType.Varchar, (object)model.uploadidproof ?? DBNull.Value);
                        dbCommand.Parameters.AddWithValue("pvar_userpassword", NpgsqlDbType.Varchar, user_password);
                        dbCommand.Parameters.AddWithValue("pvar_passwordkey", NpgsqlDbType.Varchar, user_passwordkey);
						dbCommand.Parameters.AddWithValue("pvar_countryoforigin", NpgsqlDbType.Uuid, (object)model.countryoforigin ?? DBNull.Value);
                        dbCommand.Parameters.AddWithValue("pvar_bloodgroup", NpgsqlDbType.Varchar, (object)model.bloodgroup ?? DBNull.Value);

						NpgsqlParameter outParm = new NpgsqlParameter("pvar_returnMessage", NpgsqlDbType.Varchar)
                        {
                            Direction = ParameterDirection.Output
                        };
                        dbCommand.Parameters.Add(outParm);

                        dbCommand.ExecuteNonQuery();
                        responseMessage = outParm.Value.ToString();

                        if (dbCommand.Connection.State != ConnectionState.Closed)
                            dbCommand.Connection.Dispose();
                    }
                    npsql.Close();
                }
            }
            catch (Exception ex)
            {
                responseMessage = ex.Message;
                Console.WriteLine(ex);
            }

            return responseMessage;
        }

        public virtual System.Data.DataTable get_all_PatientProfile(string tenantid,string searchterm="", int? pagesize=1000, int? pagenumber=0)
			  { 

				    DataTable dataTable = new DataTable();
					DataSet dataSet = new DataSet();

					try{
 
							using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
							{
								npsql.Open();
								using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"get_all_PatientProfile\"(@pvar_tenantid)", npsql))
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
public virtual System.Data.DataTable getById_allinfo_PatientProfile(string PatientProfileid)
			 {
				DataSet dataSet=new DataSet();
				DataTable dataTable = new DataTable();
				try{
					     
						using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
						{
							npsql.Open();
							using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"getById_sp_all_PatientProfile\"(@pvar_patientprofileid)", npsql))
							{
								dbCommand.CommandType = CommandType.Text;
								dbCommand.Parameters.AddWithValue("pvar_patientprofileid",(object)PatientProfileid??DBNull.Value);
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
			  
public virtual System.Data.DataTable lookup_PatientProfile_countryoforigin()
							        {
                                            DataSet dataSet = new DataSet();
									        DataTable dataTable=new DataTable();
									        try{

                                        		        using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
					                                    {
						                                    npsql.Open();
						                                    using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"lookup_PatientProfile_countryoforigin\"()", npsql))
						                                    {
                                            
                                                                  
																
																 
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
public virtual System.Data.DataTable lookup_PatientProfile_occupation()
							        {
                                            DataSet dataSet = new DataSet();
									        DataTable dataTable=new DataTable();
									        try{

                                        		        using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
					                                    {
						                                    npsql.Open();
						                                    using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"lookup_PatientProfile_occupation\"()", npsql))
						                                    {
                                            
                                                                  
																
																 
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
public virtual System.Data.DataTable lookup_PatientProfile_referralsource(String tenantid)
							        {
                                            DataSet dataSet = new DataSet();
									        DataTable dataTable=new DataTable();
									        try{

                                        		        using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
					                                    {
						                                    npsql.Open();
						                                    using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"lookup_PatientProfile_referralsource\"(@pvar_tenantid)", npsql))
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
public virtual System.Data.DataTable lookup_PatientProfile_reasonforblacklisting(String tenantid)
							        {
                                            DataSet dataSet = new DataSet();
									        DataTable dataTable=new DataTable();
									        try{

                                        		        using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
					                                    {
						                                    npsql.Open();
						                                    using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"lookup_PatientProfile_reasonforblacklisting\"(@pvar_tenantid)", npsql))
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




		public virtual System.Data.DataTable lookup_PatientProfile_patientcategory(String tenantid)
		{
			DataSet dataSet = new DataSet();
			DataTable dataTable = new DataTable();
			try
			{

				using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
				{
					npsql.Open();
					using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"lookup_PatientProfile_patientcategory\"(@pvar_tenantid)", npsql))
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
