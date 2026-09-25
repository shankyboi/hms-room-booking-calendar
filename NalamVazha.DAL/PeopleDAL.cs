namespace NalamVazha.DAL{
			    using System;
			    using System.Text;
			    using System.Data;
			    using System.Data.Common;
			    using System.Linq;
			    using NalamVazha.Models;
			    using EncrypDecrypt;
			    using Newtonsoft.Json;
				using Newtonsoft.Json.Linq;
                using Npgsql;
				using NpgsqlTypes;
				using System.Text.RegularExpressions;

			    //This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 11:34:28
			    public class PeopleDAL
			    {
					public virtual string db_connectionstring{get;set;}
					
			 	    public PeopleDAL(string connectionString)
				    {
						
					    db_connectionstring=connectionString;
				    }
				  
			        public virtual System.Data.DataTable getById_emergencycontact(string Peopleid)
			 {
					DataTable dataTable=new DataTable();
					DataSet dataSet=new DataSet();
					try
					{
						  
							using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
							{
								npsql.Open();
								using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"getById_sp_People_emergencycontact\"(@pvar_peopleid)", npsql))
								{
									dbCommand.CommandType = CommandType.Text;
									dbCommand.Parameters.AddWithValue("pvar_peopleid",(object)Peopleid??DBNull.Value);
								
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

public virtual System.Data.DataTable getById_educationinfo(string Peopleid)
			 {
					DataTable dataTable=new DataTable();
					DataSet dataSet=new DataSet();
					try
					{
						  
							using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
							{
								npsql.Open();
								using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"getById_sp_People_educationinfo\"(@pvar_peopleid)", npsql))
								{
									dbCommand.CommandType = CommandType.Text;
									dbCommand.Parameters.AddWithValue("pvar_peopleid",(object)Peopleid??DBNull.Value);
								
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

public virtual System.Data.DataTable getById_workexperience(string Peopleid)
			 {
					DataTable dataTable=new DataTable();
					DataSet dataSet=new DataSet();
					try
					{
						  
							using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
							{
								npsql.Open();
								using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"getById_sp_People_workexperience\"(@pvar_peopleid)", npsql))
								{
									dbCommand.CommandType = CommandType.Text;
									dbCommand.Parameters.AddWithValue("pvar_peopleid",(object)Peopleid??DBNull.Value);
								
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

public virtual System.Data.DataTable getById_preferredlanguageinfo(string Peopleid)
			 {
					DataTable dataTable=new DataTable();
					DataSet dataSet=new DataSet();
					try
					{
						  
							using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
							{
								npsql.Open();
								using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"getById_sp_People_preferredlanguageinfo\"(@pvar_peopleid)", npsql))
								{
									dbCommand.CommandType = CommandType.Text;
									dbCommand.Parameters.AddWithValue("pvar_peopleid",(object)Peopleid??DBNull.Value);
								
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

public virtual System.Data.DataTable getById_clinicaltaskinfo(string Peopleid)
			 {
					DataTable dataTable=new DataTable();
					DataSet dataSet=new DataSet();
					try
					{
						  
							using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
							{
								npsql.Open();
								using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"getById_sp_People_clinicaltaskinfo\"(@pvar_peopleid)", npsql))
								{
									dbCommand.CommandType = CommandType.Text;
									dbCommand.Parameters.AddWithValue("pvar_peopleid",(object)Peopleid??DBNull.Value);
								
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

public virtual System.Data.DataTable GetDuration_ByPractitioner(string Peopleid, string tenantid)
			 {
					DataTable dataTable = new DataTable();
					DataSet dataSet = new DataSet();
					try
					{
						using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
						{
							npsql.Open();
							using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"GetDuration_ByPractitioner\"(@pvar_practitioner, @pvar_tenantid)", npsql))
							{
								dbCommand.CommandType = CommandType.Text;
								dbCommand.Parameters.AddWithValue("pvar_practitioner", (object)Peopleid ?? DBNull.Value);
								dbCommand.Parameters.AddWithValue("pvar_tenantid", (object)tenantid ?? DBNull.Value);
								using (NpgsqlDataAdapter dataAdapter = new NpgsqlDataAdapter(dbCommand))
								{
									dataSet.Reset();
									dataAdapter.Fill(dataSet);
									dataTable = dataSet.Tables[0];
									if (dbCommand.Connection.State != ConnectionState.Closed)
										dbCommand.Connection.Dispose();
								}
							}
							npsql.Close();
						}
					}
					catch { throw; }
					return dataTable;
			 }

public virtual System.Data.DataTable prefill_People_clinicaltaskinfo(string workprofile, string competencylevel)
						{
							DataTable dataTable=new DataTable();
							DataSet dataSet=new DataSet();
							try
							{
								  

									using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
									{
										npsql.Open();
										using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"prefill_People_clinicaltaskinfo\"(@pvar_workprofile,@pvar_competency)", npsql))
										{
											dbCommand.CommandType = CommandType.Text;
											dbCommand.Parameters.AddWithValue("pvar_workprofile",(object)workprofile??DBNull.Value);
											dbCommand.Parameters.AddWithValue("pvar_competency",(object)competencylevel??DBNull.Value);
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



        public virtual bool CheckEmailExists(string emailid, string tenantid)
        {
            try
            {
                using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
                {
                    npsql.Open();
                    using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"CheckEmailExists_People\"(@pvar_emailid, @pvar_tenantid)", npsql))
                    {
                        dbCommand.CommandType = CommandType.Text;
                        dbCommand.Parameters.AddWithValue("pvar_emailid", NpgsqlDbType.Varchar, (object)emailid ?? DBNull.Value);
                        dbCommand.Parameters.AddWithValue("pvar_tenantid", NpgsqlDbType.Varchar, (object)tenantid ?? DBNull.Value);
                        var result = dbCommand.ExecuteScalar();
                        return result != null && (bool)result;
                    }
                }
            }
            catch
            {
                throw;
            }
        }

		// Read-only compatibility for drafts created by the previous draftdata implementation.
		public virtual PeopleModel GetDraft(string peopleid)
		{
			using var connection = new NpgsqlConnection(db_connectionstring);
			connection.Open();
			using var command = new NpgsqlCommand("SELECT \"Get_People_Draft\"(@peopleid)", connection);
			command.Parameters.AddWithValue("peopleid", NpgsqlDbType.Uuid, Guid.Parse(peopleid));
			var json = command.ExecuteScalar() as string;
			var model = json == null ? null : JsonConvert.DeserializeObject<PeopleModel>(json);
			if (model != null) { model.Peopleid = Guid.Parse(peopleid); model.status = "Draft"; }
			return model;
		}

        public virtual bool CheckEmergencyContactPhoneExists(string phonenumber, string Peopleid = "")
        {
            try
            {
                using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
                {
                    npsql.Open();
                    using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"CheckEmergencyContactPhoneExists_People\"(@pvar_phonenumber, @pvar_peopleid)", npsql))
                    {
                        dbCommand.CommandType = CommandType.Text;
                        dbCommand.Parameters.AddWithValue("pvar_phonenumber", NpgsqlDbType.Varchar, (object)phonenumber ?? DBNull.Value);
                        dbCommand.Parameters.AddWithValue("pvar_peopleid", NpgsqlDbType.Varchar, (object)Peopleid ?? "");
                        var result = dbCommand.ExecuteScalar();
                        return result != null && (bool)result;
                    }
                }
            }
            catch
            {
                throw;
            }
        }

        public virtual string Add_People(PeopleModel model)
        {
            String ResponseMessage = "";

            try
            {

                RandomStringGenerator objRandom = new RandomStringGenerator();
                string plainPassword = objRandom.CreateRandomPassword(8);
                string passwordKey = objRandom.CreateRandomPassword(8);
				string encryptedPassword = PwdEncrypDecrypt.Encrypt(passwordKey, plainPassword);
				Guid usersid = Guid.Parse(model.Peopleid.ToString());

				using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
                {
                    npsql.Open();
                    using var transaction = npsql.BeginTransaction();
                    using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"Add_People\"(@pvar_peopleid,@pvar_tenantid,@pvar_practitionerid,@pvar_firstname,@pvar_lastname,@pvar_workprofile,@pvar_competencylevel,@pvar_designation,@pvar_contactnumber,@pvar_whatsappnumber,@pvar_emailid,@pvar_gender,@pvar_dob,@pvar_age,@pvar_employmentstatus,@pvar_joiningdate,@pvar_contractrenewaldate,@pvar_photo,@pvar_nationality,@pvar_specifycountry,@pvar_idtype,@pvar_idnumber,@pvar_iddocument,@pvar_paddressline1,@pvar_paddressline2,@pvar_pzip,@pvar_ptown,@pvar_pcityordistrict,@pvar_pstatename,@pvar_sameaspermanentaddress,@pvar_caddressline1,@pvar_caddressline2,@pvar_czip,@pvar_ctown,@pvar_ccityordistrict,@pvar_cstatename,@pvar_registrationnumber,@pvar_validtill,@pvar_licenceupload,@pvar_issuingauthority,@pvar_bio,@pvar_screeningmeetinglink,@pvar_emergencycontact,@pvar_educationinfo,@pvar_workexperience,@pvar_preferredlanguageinfo,@pvar_clinicaltaskinfo,@pvar_createduser,@pvar_userpassword,@pvar_passwordkey,@pvar_usersid)", npsql))
                    {
                        dbCommand.Transaction = transaction;
						dbCommand.CommandText = dbCommand.CommandText.Replace("@pvar_createduser", "@pvar_status,@pvar_createduser");
                        dbCommand.CommandType = CommandType.Text;

                        dbCommand.Parameters.AddWithValue("pvar_peopleid", NpgsqlDbType.Uuid, (object)model.Peopleid ?? DBNull.Value);
                        dbCommand.Parameters.AddWithValue("pvar_tenantid", NpgsqlDbType.Uuid, (object)model.tenantid ?? DBNull.Value);

                        dbCommand.Parameters.AddWithValue("pvar_practitionerid", NpgsqlDbType.Varchar, (object)model.practitionerid ?? DBNull.Value);

                        dbCommand.Parameters.AddWithValue("pvar_firstname", NpgsqlDbType.Varchar, (object)model.firstname ?? DBNull.Value);

                        dbCommand.Parameters.AddWithValue("pvar_lastname", NpgsqlDbType.Varchar, (object)model.lastname ?? DBNull.Value);

                        dbCommand.Parameters.AddWithValue("pvar_workprofile", NpgsqlDbType.Uuid, (object)model.workprofile ?? DBNull.Value);

                        dbCommand.Parameters.AddWithValue("pvar_competencylevel", NpgsqlDbType.Uuid, (object)model.competencylevel ?? DBNull.Value);

                        dbCommand.Parameters.AddWithValue("pvar_designation", NpgsqlDbType.Uuid, (object)model.designation ?? DBNull.Value);

                        dbCommand.Parameters.AddWithValue("pvar_contactnumber", NpgsqlDbType.Varchar, (object)model.contactnumber ?? DBNull.Value);

                        dbCommand.Parameters.AddWithValue("pvar_whatsappnumber", NpgsqlDbType.Varchar, (object)model.whatsappnumber ?? DBNull.Value);

                        dbCommand.Parameters.AddWithValue("pvar_emailid", NpgsqlDbType.Varchar, (object)model.emailid ?? DBNull.Value);

                        dbCommand.Parameters.AddWithValue("pvar_gender", NpgsqlDbType.Varchar, (object)model.gender ?? DBNull.Value);

                        dbCommand.Parameters.AddWithValue("pvar_dob", NpgsqlDbType.Date, (object)model.dob ?? DBNull.Value);

                        dbCommand.Parameters.AddWithValue("pvar_age", NpgsqlDbType.Integer, (object)model.age ?? DBNull.Value);

                        dbCommand.Parameters.AddWithValue("pvar_employmentstatus", NpgsqlDbType.Varchar, (object)model.employmentstatus ?? DBNull.Value);

                        dbCommand.Parameters.AddWithValue("pvar_joiningdate", NpgsqlDbType.Date, (object)model.joiningdate ?? DBNull.Value);

                        dbCommand.Parameters.AddWithValue("pvar_contractrenewaldate", NpgsqlDbType.Date, (object)model.contractrenewaldate ?? DBNull.Value);

                        dbCommand.Parameters.AddWithValue("pvar_photo", NpgsqlDbType.Varchar, (object)model.photo ?? DBNull.Value);

                        dbCommand.Parameters.AddWithValue("pvar_nationality", NpgsqlDbType.Varchar, (object)model.nationality ?? DBNull.Value);

                        dbCommand.Parameters.AddWithValue("pvar_specifycountry", NpgsqlDbType.Uuid, (object)model.specifycountry ?? DBNull.Value);

                        dbCommand.Parameters.AddWithValue("pvar_idtype", NpgsqlDbType.Varchar, (object)model.idtype ?? DBNull.Value);

                        dbCommand.Parameters.AddWithValue("pvar_idnumber", NpgsqlDbType.Varchar, (object)model.idnumber ?? DBNull.Value);

                        dbCommand.Parameters.AddWithValue("pvar_iddocument", NpgsqlDbType.Varchar, (object)model.iddocument ?? DBNull.Value);

                        dbCommand.Parameters.AddWithValue("pvar_paddressline1", NpgsqlDbType.Varchar, (object)model.paddressline1 ?? DBNull.Value);

                        dbCommand.Parameters.AddWithValue("pvar_paddressline2", NpgsqlDbType.Varchar, (object)model.paddressline2 ?? DBNull.Value);

                        dbCommand.Parameters.AddWithValue("pvar_pzip", NpgsqlDbType.Integer, (object)model.pzip ?? DBNull.Value);

                        dbCommand.Parameters.AddWithValue("pvar_ptown", NpgsqlDbType.Varchar, (object)model.ptown ?? DBNull.Value);

                        dbCommand.Parameters.AddWithValue("pvar_pcityordistrict", NpgsqlDbType.Varchar, (object)model.pcityordistrict ?? DBNull.Value);

                        dbCommand.Parameters.AddWithValue("pvar_pstatename", NpgsqlDbType.Varchar, (object)model.pstatename ?? DBNull.Value);

                        dbCommand.Parameters.AddWithValue("pvar_sameaspermanentaddress", NpgsqlDbType.Boolean, (object)model.sameaspermanentaddress ?? DBNull.Value);

                        dbCommand.Parameters.AddWithValue("pvar_caddressline1", NpgsqlDbType.Varchar, (object)model.caddressline1 ?? DBNull.Value);

                        dbCommand.Parameters.AddWithValue("pvar_caddressline2", NpgsqlDbType.Varchar, (object)model.caddressline2 ?? DBNull.Value);

                        dbCommand.Parameters.AddWithValue("pvar_czip", NpgsqlDbType.Integer, (object)model.czip ?? DBNull.Value);

                        dbCommand.Parameters.AddWithValue("pvar_ctown", NpgsqlDbType.Varchar, (object)model.ctown ?? DBNull.Value);

                        dbCommand.Parameters.AddWithValue("pvar_ccityordistrict", NpgsqlDbType.Varchar, (object)model.ccityordistrict ?? DBNull.Value);

                        dbCommand.Parameters.AddWithValue("pvar_cstatename", NpgsqlDbType.Varchar, (object)model.cstatename ?? DBNull.Value);

                        dbCommand.Parameters.AddWithValue("pvar_registrationnumber", NpgsqlDbType.Varchar, (object)model.registrationnumber ?? DBNull.Value);

                        dbCommand.Parameters.AddWithValue("pvar_validtill", NpgsqlDbType.Date, (object)model.validtill ?? DBNull.Value);

                        dbCommand.Parameters.AddWithValue("pvar_licenceupload", NpgsqlDbType.Varchar, (object)model.licenceupload ?? DBNull.Value);

                        dbCommand.Parameters.AddWithValue("pvar_issuingauthority", NpgsqlDbType.Varchar, (object)model.issuingauthority ?? DBNull.Value);

                        dbCommand.Parameters.AddWithValue("pvar_bio", NpgsqlDbType.Varchar, (object)model.bio ?? DBNull.Value);
						dbCommand.Parameters.AddWithValue("pvar_screeningmeetinglink", NpgsqlDbType.Varchar, (object)model.screeningmeetinglink ?? DBNull.Value);
						if (model.emergencycontact != null && model.emergencycontact.Count > 0)
                            dbCommand.Parameters.AddWithValue("pvar_emergencycontact", NpgsqlDbType.Json, JsonConvert.SerializeObject(model.emergencycontact));
                        else
                            dbCommand.Parameters.AddWithValue("pvar_emergencycontact", NpgsqlDbType.Json, DBNull.Value);
                        if (model.educationinfo != null && model.educationinfo.Count > 0)
                            dbCommand.Parameters.AddWithValue("pvar_educationinfo", NpgsqlDbType.Json, JsonConvert.SerializeObject(model.educationinfo));
                        else
                            dbCommand.Parameters.AddWithValue("pvar_educationinfo", NpgsqlDbType.Json, DBNull.Value);
                        if (model.workexperience != null && model.workexperience.Count > 0)
                            dbCommand.Parameters.AddWithValue("pvar_workexperience", NpgsqlDbType.Json, JsonConvert.SerializeObject(model.workexperience));
                        else
                            dbCommand.Parameters.AddWithValue("pvar_workexperience", NpgsqlDbType.Json, DBNull.Value);
                        if (model.preferredlanguageinfo != null && model.preferredlanguageinfo.Count > 0)
                            dbCommand.Parameters.AddWithValue("pvar_preferredlanguageinfo", NpgsqlDbType.Json, JsonConvert.SerializeObject(model.preferredlanguageinfo));
                        else
                            dbCommand.Parameters.AddWithValue("pvar_preferredlanguageinfo", NpgsqlDbType.Json, DBNull.Value);
                        if (model.clinicaltaskinfo != null && model.clinicaltaskinfo.Count > 0)
                            dbCommand.Parameters.AddWithValue("pvar_clinicaltaskinfo", NpgsqlDbType.Json, JsonConvert.SerializeObject(model.clinicaltaskinfo));
                        else
                            dbCommand.Parameters.AddWithValue("pvar_clinicaltaskinfo", NpgsqlDbType.Json, DBNull.Value);
                        dbCommand.Parameters.AddWithValue("pvar_status", NpgsqlDbType.Varchar, string.Equals(model.status, "Draft", StringComparison.OrdinalIgnoreCase) ? "Draft" : "Active");
                        dbCommand.Parameters.AddWithValue("pvar_createduser", NpgsqlDbType.Uuid, (object)model.createduser ?? DBNull.Value);
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
                        {
                            ResponseMessage = spResponse + "|" + model.emailid + "|" + plainPassword;
                            transaction.Commit();
                        }
                        else
                        {
                            ResponseMessage = spResponse;
                            transaction.Rollback();
                        }

                    }
                    npsql.Close();
                }


            }
            catch (PostgresException ex) when (ex.ConstraintName == "people_phone_DETAIL")
            {
                ResponseMessage = "Validation Error : Emergency Contact Phone Number already exists.";
            }
            catch (Exception ex)
            {
                ResponseMessage = ex.Message;
                Console.WriteLine(ex);
            }

            return ResponseMessage;

        }




        public virtual PeopleModel getById_People(string Peopleid)
									 {
										DataTable dataTable = new DataTable();
										DataSet dataSet = new DataSet();
										try{
												 
												using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
												{
													npsql.Open();
													using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"getById_sp_People\"(@pvar_peopleid)", npsql))
													{
														dbCommand.CommandType = CommandType.Text;
														dbCommand.Parameters.AddWithValue("pvar_peopleid",(object)Peopleid??DBNull.Value);
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
											return ModelConverter.ConvertDataRowToModel<PeopleModel>(row);
										}
										else
										{
											return null;
										}
									 }
			 public virtual string  Update_People(PeopleModel model)
			 { 
				 String ResponseMessage="";
					try{
						 	 
							using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
							{
								npsql.Open();
								using var transaction = npsql.BeginTransaction();
								var activatingDraft = false;
								string activationEmail = null;
								string activationPassword = null;

								// Stored status (and the immutable practitioner ID, read further down) come from
								// the existing getById_sp_People.
								using (var stateCommand = new NpgsqlCommand("SELECT * FROM \"getById_sp_People\"(@pvar_Peopleid)", npsql, transaction))
								{
									stateCommand.CommandType = CommandType.Text;
									stateCommand.Parameters.AddWithValue("pvar_Peopleid", NpgsqlDbType.Varchar, (object)model.Peopleid?.ToString() ?? DBNull.Value);
									using var stateReader = stateCommand.ExecuteReader();
									if (stateReader.Read())
									{
										activatingDraft = !string.Equals(model.status, "Draft", StringComparison.OrdinalIgnoreCase)
											&& string.Equals(stateReader["status"]?.ToString(), "Draft", StringComparison.OrdinalIgnoreCase);
										// Practitioner ID is an auto-number assigned on activation and is immutable,
										// so the stored value always wins over whatever the client posted.
										if (stateReader["practitionerid"] != DBNull.Value)
											model.practitionerid = stateReader["practitionerid"].ToString();
									}
								}

								// Credentials of the linked login, if the draft already has one.
								if (activatingDraft)
								{
									using var loginCommand = new NpgsqlCommand("SELECT * FROM \"getById_sp_all_users\"(@pvar_usersid)", npsql, transaction);
									loginCommand.CommandType = CommandType.Text;
									loginCommand.Parameters.AddWithValue("pvar_usersid", NpgsqlDbType.Varchar, (object)model.Peopleid?.ToString() ?? DBNull.Value);
									using var loginReader = loginCommand.ExecuteReader();
									if (loginReader.Read()
										&& loginReader["userpassword"] != DBNull.Value
										&& loginReader["passwordkey"] != DBNull.Value)
									{
										activationEmail = loginReader["emailid"]?.ToString();
										activationPassword = PwdEncrypDecrypt.Decrypt(loginReader["passwordkey"].ToString(), loginReader["userpassword"].ToString());
									}
								}

								// A minimal draft may not have had enough information to create its login.
								// Create it atomically when the completed draft is activated.
								if (activatingDraft && string.IsNullOrWhiteSpace(activationPassword))
								{
									var random = new RandomStringGenerator();
									activationPassword = random.CreateRandomPassword(8);
									var passwordKey = random.CreateRandomPassword(8);
									var encryptedPassword = PwdEncrypDecrypt.Encrypt(passwordKey, activationPassword);
									activationEmail = model.emailid;
									using var userCommand = new NpgsqlCommand("SELECT * FROM \"Create_People_Activation_User\"(@pvar_usersid,@pvar_tenantid,@pvar_firstname,@pvar_lastname,@pvar_photo,@pvar_email,@pvar_password,@pvar_passwordkey,@pvar_mobile,@pvar_workprofile,@pvar_modifieduser)", npsql, transaction);
									userCommand.CommandType = CommandType.Text;
									userCommand.Parameters.AddWithValue("pvar_usersid", NpgsqlDbType.Uuid, model.Peopleid.Value);
									userCommand.Parameters.AddWithValue("pvar_tenantid", NpgsqlDbType.Uuid, model.tenantid.Value);
									userCommand.Parameters.AddWithValue("pvar_firstname", NpgsqlDbType.Varchar, (object)model.firstname ?? DBNull.Value);
									userCommand.Parameters.AddWithValue("pvar_lastname", NpgsqlDbType.Varchar, (object)model.lastname ?? DBNull.Value);
									userCommand.Parameters.AddWithValue("pvar_photo", NpgsqlDbType.Varchar, (object)model.photo ?? DBNull.Value);
									userCommand.Parameters.AddWithValue("pvar_email", NpgsqlDbType.Varchar, model.emailid);
									userCommand.Parameters.AddWithValue("pvar_password", NpgsqlDbType.Varchar, encryptedPassword);
									userCommand.Parameters.AddWithValue("pvar_passwordkey", NpgsqlDbType.Varchar, passwordKey);
									userCommand.Parameters.AddWithValue("pvar_mobile", NpgsqlDbType.Varchar, model.contactnumber);
									userCommand.Parameters.AddWithValue("pvar_workprofile", NpgsqlDbType.Uuid, model.workprofile);
									userCommand.Parameters.AddWithValue("pvar_modifieduser", NpgsqlDbType.Uuid, model.modifieduser.Value);
									var insertedUsers = userCommand.ExecuteScalar();
									if (insertedUsers == null || insertedUsers == DBNull.Value || Convert.ToInt32(insertedUsers) != 1)
										throw new InvalidOperationException("Unable to create the linked user account for the completed draft.");
								}
								using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"Update_People\"(@pvar_peopleid,@pvar_tenantid,@pvar_practitionerid,@pvar_firstname,@pvar_lastname,@pvar_workprofile,@pvar_competencylevel,@pvar_designation,@pvar_contactnumber,@pvar_whatsappnumber,@pvar_emailid,@pvar_gender,@pvar_dob,@pvar_age,@pvar_employmentstatus,@pvar_joiningdate,@pvar_contractrenewaldate,@pvar_photo,@pvar_nationality,@pvar_specifycountry,@pvar_idtype,@pvar_idnumber,@pvar_iddocument,@pvar_paddressline1,@pvar_paddressline2,@pvar_pzip,@pvar_ptown,@pvar_pcityordistrict,@pvar_pstatename,@pvar_sameaspermanentaddress,@pvar_caddressline1,@pvar_caddressline2,@pvar_czip,@pvar_ctown,@pvar_ccityordistrict,@pvar_cstatename,@pvar_registrationnumber,@pvar_validtill,@pvar_licenceupload,@pvar_issuingauthority,@pvar_bio,@pvar_screeningmeetinglink,@pvar_emergencycontact,@pvar_educationinfo,@pvar_workexperience,@pvar_preferredlanguageinfo,@pvar_clinicaltaskinfo,@pvar_modifieduser)", npsql))
								{
										dbCommand.Transaction = transaction;
										dbCommand.CommandText = dbCommand.CommandText.Replace("@pvar_modifieduser", "@pvar_status,@pvar_modifieduser");
										dbCommand.CommandType = CommandType.Text;
															dbCommand.Parameters.AddWithValue("pvar_peopleid",NpgsqlDbType.Uuid,(object)model.Peopleid??DBNull.Value);
dbCommand.Parameters.AddWithValue("pvar_tenantid",NpgsqlDbType.Uuid,(object)model.tenantid??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_practitionerid",NpgsqlDbType.Varchar,(object)model.practitionerid??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_firstname",NpgsqlDbType.Varchar,(object)model.firstname??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_lastname",NpgsqlDbType.Varchar,(object)model.lastname??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_workprofile",NpgsqlDbType.Uuid,(object)model.workprofile??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_competencylevel",NpgsqlDbType.Uuid,(object)model.competencylevel??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_designation",NpgsqlDbType.Uuid,(object)model.designation??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_contactnumber",NpgsqlDbType.Varchar,(object)model.contactnumber??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_whatsappnumber",NpgsqlDbType.Varchar,(object)model.whatsappnumber??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_emailid",NpgsqlDbType.Varchar,(object)model.emailid??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_gender",NpgsqlDbType.Varchar,(object)model.gender??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_dob",NpgsqlDbType.Date,(object)model.dob??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_age",NpgsqlDbType.Integer,(object)model.age??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_employmentstatus",NpgsqlDbType.Varchar,(object)model.employmentstatus??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_joiningdate",NpgsqlDbType.Date,(object)model.joiningdate??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_contractrenewaldate",NpgsqlDbType.Date,(object)model.contractrenewaldate??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_photo",NpgsqlDbType.Varchar,(object)model.photo??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_nationality",NpgsqlDbType.Varchar,(object)model.nationality??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_specifycountry",NpgsqlDbType.Uuid,(object)model.specifycountry??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_idtype",NpgsqlDbType.Varchar,(object)model.idtype??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_idnumber",NpgsqlDbType.Varchar,(object)model.idnumber??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_iddocument",NpgsqlDbType.Varchar,(object)model.iddocument??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_paddressline1",NpgsqlDbType.Varchar,(object)model.paddressline1??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_paddressline2",NpgsqlDbType.Varchar,(object)model.paddressline2??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_pzip",NpgsqlDbType.Integer,(object)model.pzip??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_ptown",NpgsqlDbType.Varchar,(object)model.ptown??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_pcityordistrict",NpgsqlDbType.Varchar,(object)model.pcityordistrict??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_pstatename",NpgsqlDbType.Varchar,(object)model.pstatename??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_sameaspermanentaddress",NpgsqlDbType.Boolean,(object)model.sameaspermanentaddress??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_caddressline1",NpgsqlDbType.Varchar,(object)model.caddressline1??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_caddressline2",NpgsqlDbType.Varchar,(object)model.caddressline2??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_czip",NpgsqlDbType.Integer,(object)model.czip??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_ctown",NpgsqlDbType.Varchar,(object)model.ctown??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_ccityordistrict",NpgsqlDbType.Varchar,(object)model.ccityordistrict??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_cstatename",NpgsqlDbType.Varchar,(object)model.cstatename??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_registrationnumber",NpgsqlDbType.Varchar,(object)model.registrationnumber??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_validtill",NpgsqlDbType.Date,(object)model.validtill??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_licenceupload",NpgsqlDbType.Varchar,(object)model.licenceupload??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_issuingauthority",NpgsqlDbType.Varchar,(object)model.issuingauthority??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_bio",NpgsqlDbType.Varchar,(object)model.bio??DBNull.Value);
						dbCommand.Parameters.AddWithValue("pvar_screeningmeetinglink", NpgsqlDbType.Varchar, (object)model.screeningmeetinglink ?? DBNull.Value);
						if (model.emergencycontact !=null  && model.emergencycontact.Count >0)
dbCommand.Parameters.AddWithValue("pvar_emergencycontact",NpgsqlDbType.Json,JsonConvert.SerializeObject(model.emergencycontact));
else
dbCommand.Parameters.AddWithValue("pvar_emergencycontact",NpgsqlDbType.Json,DBNull.Value);
if(model.educationinfo !=null  && model.educationinfo.Count >0)
dbCommand.Parameters.AddWithValue("pvar_educationinfo",NpgsqlDbType.Json,JsonConvert.SerializeObject(model.educationinfo));
else
dbCommand.Parameters.AddWithValue("pvar_educationinfo",NpgsqlDbType.Json,DBNull.Value);
if(model.workexperience !=null  && model.workexperience.Count >0)
dbCommand.Parameters.AddWithValue("pvar_workexperience",NpgsqlDbType.Json,JsonConvert.SerializeObject(model.workexperience));
else
dbCommand.Parameters.AddWithValue("pvar_workexperience",NpgsqlDbType.Json,DBNull.Value);
if(model.preferredlanguageinfo !=null  && model.preferredlanguageinfo.Count >0)
dbCommand.Parameters.AddWithValue("pvar_preferredlanguageinfo",NpgsqlDbType.Json,JsonConvert.SerializeObject(model.preferredlanguageinfo));
else
dbCommand.Parameters.AddWithValue("pvar_preferredlanguageinfo",NpgsqlDbType.Json,DBNull.Value);
if(model.clinicaltaskinfo !=null  && model.clinicaltaskinfo.Count >0)
dbCommand.Parameters.AddWithValue("pvar_clinicaltaskinfo",NpgsqlDbType.Json,JsonConvert.SerializeObject(model.clinicaltaskinfo));
else
dbCommand.Parameters.AddWithValue("pvar_clinicaltaskinfo",NpgsqlDbType.Json,DBNull.Value);
dbCommand.Parameters.AddWithValue("pvar_status",NpgsqlDbType.Varchar,string.Equals(model.status, "Draft", StringComparison.OrdinalIgnoreCase) ? "Draft" : "Active");
dbCommand.Parameters.AddWithValue("pvar_modifieduser",NpgsqlDbType.Uuid,model.modifieduser);	
															
										NpgsqlParameter outParm = new NpgsqlParameter("@returnMessage", NpgsqlDbType.Varchar)
										{
											 Direction = ParameterDirection.Output
										};
										dbCommand.Parameters.Add(outParm);

										dbCommand.ExecuteNonQuery();
										ResponseMessage = outParm.Value.ToString();
										if (ResponseMessage.Contains("201.1"))
										{
											transaction.Commit();
											if (activatingDraft)
												ResponseMessage += "|" + activationEmail + "|" + activationPassword;
										}
										else transaction.Rollback();

								}
								npsql.Close();
							}		 

					}catch(PostgresException ex) when (ex.ConstraintName == "people_phone_DETAIL"){
						ResponseMessage="Validation Error : Emergency Contact Phone Number already exists.";
					}catch(Exception ex){
						ResponseMessage=ex.Message;
					}
					
					return ResponseMessage;

			   }
public virtual string  Remove_People(string id,string loginUserID)
			  { 
				  String ResponseMessage="";
					try{ 
							using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
							{
								npsql.Open();
								using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"Remove_People\"(@pvar_peopleid,@pvar_modifieduser)", npsql))
								{
										dbCommand.CommandType = CommandType.Text;
										dbCommand.Parameters.AddWithValue("pvar_peopleid",(object)id??DBNull.Value);
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
public virtual JObject People_List(string tenantid
, string workprofile="", int? pagesize=1000 , int? pagenumber=0,string searchterm="",string  sort_fields="")
			  { 
				  object dalResponse = null;
				  var usedLegacyFunction = false;
			
					try{
 
							using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
							{
								npsql.Open();
								try
								{
									using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"People_List\"(@pvar_tenantid,@pvar_workprofile,@pvar_pagesize,@pvar_pagenumber,@pvar_searchterm,@pvar_sort_fields)", npsql))
									{
										dbCommand.CommandType = CommandType.Text;
										dbCommand.Parameters.AddWithValue("pvar_tenantid",(object)tenantid??DBNull.Value);
										dbCommand.Parameters.AddWithValue("pvar_workprofile",(object)workprofile??DBNull.Value);
										dbCommand.Parameters.AddWithValue("pvar_pagesize",(object)pagesize??DBNull.Value);
										dbCommand.Parameters.AddWithValue("pvar_pagenumber",(object)pagenumber??DBNull.Value);
										dbCommand.Parameters.AddWithValue("pvar_searchterm",(object)searchterm??DBNull.Value);
										if (sort_fields != null && sort_fields.Length > 2)
											dbCommand.Parameters.AddWithValue("pvar_sort_fields", NpgsqlDbType.Json, sort_fields);
										else
											dbCommand.Parameters.AddWithValue("pvar_sort_fields", NpgsqlDbType.Json, DBNull.Value);

										dalResponse = dbCommand.ExecuteScalar();
									}
								}
								catch (PostgresException ex) when (ex.SqlState == PostgresErrorCodes.UndefinedFunction)
								{
									// Older databases expose the original five-argument function. Keep the
									// list usable until the work-profile-aware overload is deployed.
									usedLegacyFunction = true;
									var legacyPageSize = string.IsNullOrWhiteSpace(workprofile) ? pagesize : int.MaxValue;
									var legacyPageNumber = string.IsNullOrWhiteSpace(workprofile) ? pagenumber : 0;
									using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"People_List\"(@pvar_tenantid,@pvar_pagesize,@pvar_pagenumber,@pvar_searchterm,@pvar_sort_fields)", npsql))
									{
										dbCommand.CommandType = CommandType.Text;
										dbCommand.Parameters.AddWithValue("pvar_tenantid",(object)tenantid??DBNull.Value);
										dbCommand.Parameters.AddWithValue("pvar_pagesize",(object)legacyPageSize??DBNull.Value);
										dbCommand.Parameters.AddWithValue("pvar_pagenumber",(object)legacyPageNumber??DBNull.Value);
										dbCommand.Parameters.AddWithValue("pvar_searchterm",(object)searchterm??DBNull.Value);
										if (sort_fields != null && sort_fields.Length > 2)
											dbCommand.Parameters.AddWithValue("pvar_sort_fields", NpgsqlDbType.Json, sort_fields);
										else
											dbCommand.Parameters.AddWithValue("pvar_sort_fields", NpgsqlDbType.Json, DBNull.Value);

										dalResponse = dbCommand.ExecuteScalar();
									}
								}
								npsql.Close();
							}

						 

					}catch{
						throw;
					}


					var response = JObject.Parse(dalResponse.ToString());
					if (usedLegacyFunction && !string.IsNullOrWhiteSpace(workprofile) && response["detail"] is JArray detail)
					{
						var filteredDetail = detail
							.Where(item => string.Equals(item?["workprofile"]?.ToString(), workprofile, StringComparison.OrdinalIgnoreCase))
							.ToList();
						var filteredCount = filteredDetail.Count;
						var offset = Math.Max(0, pagenumber ?? 0) * Math.Max(0, pagesize ?? filteredCount);
						var pagedDetail = pagesize.HasValue
							? filteredDetail.Skip(offset).Take(Math.Max(0, pagesize.Value))
							: filteredDetail;

						response["count"] = filteredCount;
						response["detail"] = new JArray(pagedDetail);
					}

					return response;


					 

			   }
			   
			 
public virtual System.Data.DataTable get_all_People(string tenantid,string searchterm="", int? pagesize=1000, int? pagenumber=0)
			  { 

				    DataTable dataTable = new DataTable();
					DataSet dataSet = new DataSet();

					try{
 
							using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
							{
								npsql.Open();
								using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"get_all_People\"(@pvar_tenantid)", npsql))
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
public virtual System.Data.DataTable getById_allinfo_People(string Peopleid)
			 {
				DataSet dataSet=new DataSet();
				DataTable dataTable = new DataTable();
				try{
					     
						using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
						{
							npsql.Open();
							using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"getById_sp_all_People\"(@pvar_peopleid)", npsql))
							{
								dbCommand.CommandType = CommandType.Text;
								dbCommand.Parameters.AddWithValue("pvar_peopleid",(object)Peopleid??DBNull.Value);
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
			  
public virtual System.Data.DataTable lookup_People_workprofile(String tenantid)
							        {
                                            DataSet dataSet = new DataSet();
									        DataTable dataTable=new DataTable();
									        try{

                                        		        using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
					                                    {
						                                    npsql.Open();
						                                    using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"lookup_People_workprofile\"(@pvar_tenantid)", npsql))
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
public virtual System.Data.DataTable lookup_People_competencylevel(String tenantid)
							        {
                                            DataSet dataSet = new DataSet();
									        DataTable dataTable=new DataTable();
									        try{

                                        		        using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
					                                    {
						                                    npsql.Open();
						                                    using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"lookup_People_competencylevel\"(@pvar_tenantid)", npsql))
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
public virtual System.Data.DataTable lookup_People_clinicaltask(String tenantid,String workprofile,String competencylevel)
							        {
                                            DataSet dataSet = new DataSet();
									        DataTable dataTable=new DataTable();
									        try{

                                        		        using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
					                                    {
						                                    npsql.Open();
						                                    using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"lookup_People_clinicaltask\"(@pvar_tenantid,@pvar_workprofile,@pvar_competency)", npsql))
						                                    {
                                            
                                                                dbCommand.Parameters.AddWithValue("pvar_tenantid",(object)tenantid??DBNull.Value);dbCommand.Parameters.AddWithValue("pvar_workprofile",(object)workprofile??DBNull.Value);dbCommand.Parameters.AddWithValue("pvar_competency",(object)competencylevel??DBNull.Value);  
																
																 
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
public virtual System.Data.DataTable lookup_People_designation(String tenantid,String workprofile)
							        {
                                            DataSet dataSet = new DataSet();
									        DataTable dataTable=new DataTable();
									        try{

                                        		        using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
					                                    {
						                                    npsql.Open();
						                                    using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"lookup_People_designation\"(@pvar_tenantid,@pvar_workprofile)", npsql))
						                                    {
                                            
                                                                dbCommand.Parameters.AddWithValue("pvar_tenantid",(object)tenantid??DBNull.Value);dbCommand.Parameters.AddWithValue("pvar_workprofile",(object)workprofile??DBNull.Value);  
																
																 
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
public virtual System.Data.DataTable lookup_People_specifycountry()
							        {
                                            DataSet dataSet = new DataSet();
									        DataTable dataTable=new DataTable();
									        try{

                                        		        using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
					                                    {
						                                    npsql.Open();
						                                    using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"lookup_People_specifycountry\"()", npsql))
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
public virtual System.Data.DataTable lookup_People_clinicaltaskinfo_consultations(String tenantid)
							        {
                                            DataSet dataSet = new DataSet();
									        DataTable dataTable=new DataTable();
									        try{

                                        		        using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
					                                    {
						                                    npsql.Open();
						                                    using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"lookup_People_clinicaltaskinfo_consultations\"(@pvar_tenantid)", npsql))
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
public virtual System.Data.DataTable lookup_People_clinicaltaskinfo_workprofile(String tenantid)
							        {
                                            DataSet dataSet = new DataSet();
									        DataTable dataTable=new DataTable();
									        try{

                                        		        using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
					                                    {
						                                    npsql.Open();
						                                    using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"lookup_People_clinicaltaskinfo_workprofile\"(@pvar_tenantid)", npsql))
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
public virtual System.Data.DataTable lookup_People_clinicaltaskinfo_tasktype(String tenantid)
							        {
                                            DataSet dataSet = new DataSet();
									        DataTable dataTable=new DataTable();
									        try{

                                        		        using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
					                                    {
						                                    npsql.Open();
						                                    using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"lookup_People_clinicaltaskinfo_tasktype\"(@pvar_tenantid)", npsql))
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
public virtual System.Data.DataTable lookup_People_clinicaltaskinfo_taskname(String tenantid,String tasktype)
							        {
                                            DataSet dataSet = new DataSet();
									        DataTable dataTable=new DataTable();
									        try{

                                        		        using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
					                                    {
						                                    npsql.Open();
						                                    using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"lookup_People_clinicaltaskinfo_taskname\"(@pvar_tenantid,@pvar_tasktype)", npsql))
						                                    {
                                            
                                                                dbCommand.Parameters.AddWithValue("pvar_tenantid",(object)tenantid??DBNull.Value);dbCommand.Parameters.AddWithValue("pvar_tasktype",(object)tasktype??DBNull.Value);  
																
																 
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
