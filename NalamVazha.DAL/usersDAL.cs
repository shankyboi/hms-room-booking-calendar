namespace NalamVazha.DAL{
			    using EncrypDecrypt;
			    using NalamVazha.Models;
			    using Newtonsoft.Json;
				using Newtonsoft.Json.Linq;
                using Npgsql;
				using NpgsqlTypes;
			    using System;
			    using System.Data;
			    using System.Data.Common;
    using System.Diagnostics;
			    using System.Text;
				using System.Text.RegularExpressions;

    //This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:40:22
			    public class usersDAL
			    {
					public virtual string db_connectionstring{get;set;}
					
			 	    public usersDAL(string connectionString)
				    {
						
					    db_connectionstring=connectionString;
				    }
				  
			        
              public virtual string Register_Profile(usersModel model)
			  { 
				  String ResponseMessage="";
					 
					try{
							 
                            using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
					        {
						        npsql.Open();
						        using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"Register_Profile\"(@pvar_usersid,@pvar_tenantid,@pvar_firstname,@pvar_lastname,@pvar_profilepicture,@pvar_username,@pvar_userpassword,@pvar_emailid,@pvar_mobilenumber,@pvar_userrole,@pvar_passwordkey,@pvar_createduser)", npsql))
						        {
                                        dbCommand.CommandType = CommandType.Text;
						            	RandomStringGenerator objRandom =new RandomStringGenerator();
				string user_passwordkey = objRandom.CreateRandomPassword(8);
				string user_password = PwdEncrypDecrypt.Encrypt(user_passwordkey,model.userpassword);
				model.userpassword=user_password;
				model.passwordkey=user_passwordkey;
								        					dbCommand.Parameters.AddWithValue("pvar_usersid",NpgsqlDbType.Uuid,(object)model.usersid??DBNull.Value);
dbCommand.Parameters.AddWithValue("pvar_tenantid",NpgsqlDbType.Uuid,(object)model.tenantid??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_firstname",NpgsqlDbType.Varchar,(object)model.firstname??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_lastname",NpgsqlDbType.Varchar,(object)model.lastname??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_profilepicture",NpgsqlDbType.Varchar,(object)model.profilepicture??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_username",NpgsqlDbType.Varchar,(object)model.username??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_userpassword",NpgsqlDbType.Varchar,(object)model.userpassword??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_emailid",NpgsqlDbType.Varchar,(object)model.emailid??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_mobilenumber",NpgsqlDbType.Varchar,(object)model.mobilenumber??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_userrole",NpgsqlDbType.Varchar,(object)model.userrole??DBNull.Value);
dbCommand.Parameters.AddWithValue("pvar_passwordkey",NpgsqlDbType.Varchar,model.passwordkey);
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
public virtual usersModel getById_users(string usersid)
									 {
										DataTable dataTable = new DataTable();
										DataSet dataSet = new DataSet();
										try{
												 
												using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
												{
													npsql.Open();
													using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"getById_sp_users\"(@pvar_usersid)", npsql))
													{
														dbCommand.CommandType = CommandType.Text;
														dbCommand.Parameters.AddWithValue("pvar_usersid",(object)usersid??DBNull.Value);
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
											return ModelConverter.ConvertDataRowToModel<usersModel>(row);
										}
										else
										{
											return null;
										}
									 }
			 public virtual string  Update_Profile(usersModel model)
			 { 
				 String ResponseMessage="";
					try{
						 	 
							using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
							{
								npsql.Open();
								using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"Update_Profile\"(@pvar_usersid,@pvar_tenantid,@pvar_firstname,@pvar_lastname,@pvar_profilepicture,@pvar_username,@pvar_emailid,@pvar_mobilenumber,@pvar_userrole,@pvar_modifieduser)", npsql))
								{
										dbCommand.CommandType = CommandType.Text;
															dbCommand.Parameters.AddWithValue("pvar_usersid",NpgsqlDbType.Uuid,(object)model.usersid??DBNull.Value);
dbCommand.Parameters.AddWithValue("pvar_tenantid",NpgsqlDbType.Uuid,(object)model.tenantid??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_firstname",NpgsqlDbType.Varchar,(object)model.firstname??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_lastname",NpgsqlDbType.Varchar,(object)model.lastname??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_profilepicture",NpgsqlDbType.Varchar,(object)model.profilepicture??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_username",NpgsqlDbType.Varchar,(object)model.username??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_emailid",NpgsqlDbType.Varchar,(object)model.emailid??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_mobilenumber",NpgsqlDbType.Varchar,(object)model.mobilenumber??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_userrole",NpgsqlDbType.Varchar,(object)model.userrole??DBNull.Value);
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
public virtual string  Suspend_Profile(string id,string loginUserID)
			  { 
				  String ResponseMessage="";
					try{ 
							using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
							{
								npsql.Open();
								using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"Suspend_Profile\"(@pvar_usersid,@pvar_modifieduser)", npsql))
								{
										dbCommand.CommandType = CommandType.Text;
										dbCommand.Parameters.AddWithValue("pvar_usersid",(object)id??DBNull.Value);
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

				public virtual string get_decryptedPassword(string userName, Guid? tenantId = null)
				{

					string decryptedPassword = "";
					DataTable pwdandKey = checkUserKey_users(userName, tenantId);
					if (pwdandKey.Rows.Count > 0)
					{
						string user_password = pwdandKey.Rows[0]["userpassword"].ToString();
						string user_passwordKey = pwdandKey.Rows[0]["passwordkey"].ToString();
						decryptedPassword = PwdEncrypDecrypt.Decrypt(user_passwordKey, user_password);
						//password= decryptedPassword; 

					}

					return decryptedPassword;
				}
		
			public virtual System.Data.DataTable get_roleAuthorizations(string viewactionroles)
			{
					DataTable dataTable=new DataTable();
                    DataSet dataSet=new DataSet();
					 
					try
					{ 
                            using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
                            {
	                            npsql.Open();
	                            using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"get_sp_roleauthorizations\"(@pvar_viewactionroles)", npsql))
	                            {
		                            dbCommand.CommandType = CommandType.Text;
		                            dbCommand.Parameters.AddWithValue("pvar_viewactionroles",(object)viewactionroles??DBNull.Value);
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
					catch{
						throw;
					} 
					return dataTable;	
				
			}
			public virtual System.Data.DataTable get_project_Menu(string viewactionroles,string subsystem)
			{

				   DataTable dataTable=new DataTable();
                    DataSet dataSet=new DataSet();
					try
					{
							  
                            
                             using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
                            {
	                            npsql.Open();
	                            using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"get_sp_project_Menu\"(@pvar_viewactionroles,@pvar_subsystem)", npsql))
	                            {
		                            dbCommand.CommandType = CommandType.Text;
		                            dbCommand.Parameters.AddWithValue("pvar_viewactionroles",(object)viewactionroles??DBNull.Value);
									dbCommand.Parameters.AddWithValue("pvar_subsystem",(object)subsystem??DBNull.Value);
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
					catch{
						throw;
					}
					return dataTable;	
				
			}
			public virtual System.Data.DataTable get_roles(string roles,string parentname)
			{ 
 				   DataTable dataTable=new DataTable();
                    DataSet dataSet=new DataSet();
					try
					{
                            using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
                            {
	                            npsql.Open();
								using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"get_sp_roles\"(@pvar_roles,@pvar_parentname)", npsql))
	                            {
		                            dbCommand.CommandType = CommandType.Text;
		                            dbCommand.Parameters.AddWithValue("pvar_roles",(object)roles??DBNull.Value);
									dbCommand.Parameters.AddWithValue("pvar_parentname",(object)parentname??DBNull.Value);
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
					catch{
						throw;
					}
					return dataTable;	
			}
			public virtual System.Data.DataTable checkUserKey_users(string username, Guid? tenantId = null)
			{
				   DataTable dataTable=new DataTable();
                    DataSet dataSet=new DataSet();
					try
					{
						 	 
                            using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
                            {
	                            npsql.Open();
								using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"checkUserKey_sp_users\"(@pvar_username,@pvar_tenantid)", npsql))
								{
		                            dbCommand.CommandType = CommandType.Text;
		                            dbCommand.Parameters.AddWithValue("pvar_username",(object)username?.Trim()??DBNull.Value);
		                            dbCommand.Parameters.AddWithValue("pvar_tenantid",NpgsqlDbType.Uuid,(object)tenantId??DBNull.Value);
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
					catch{
						throw;
					}
					return dataTable;	

					 
			}

			
			
			public virtual System.Data.DataTable CheckAuthentication(userloginModel model)
			{ 
				
				DataTable dataTable=new DataTable();
            if (model.source == "Internal" && !string.IsNullOrEmpty(model.userpassword)){
            
                
                    model.username = model.username?.Trim();
                    DataTable pwdandKey = checkUserKey_users(model.username, model.tenantid);
				if(pwdandKey.Rows.Count > 0)
				{
					var suppliedPassword = model.userpassword;
					DataRow matchedUser = null;

					foreach (DataRow candidate in pwdandKey.Rows)
					{
						string encryptedPassword = candidate["userpassword"].ToString();
						string passwordKey = candidate["passwordkey"].ToString();
						string decryptedPassword = PwdEncrypDecrypt.Decrypt(passwordKey, encryptedPassword);

						if (suppliedPassword == decryptedPassword)
						{
							matchedUser = candidate;
							break;
						}
					}

					if (matchedUser == null)
						return dataTable;

					// A phone number may be shared by multiple patients. Continue with
					// the canonical username belonging to the matching password.
					model.username = matchedUser["username"].ToString();
					model.userpassword = matchedUser["userpassword"].ToString();
				}
				}

				   
                    DataSet dataSet=new DataSet();
					try
					{
                Debug.WriteLine("Username/Email being searched: " + model.username);
                Debug.WriteLine("Password: " + (string.IsNullOrEmpty(model.userpassword) ? "EMPTY (OTP)" : "PROVIDED"));
                Debug.WriteLine("Source: " + model.source);

                if (model.userpassword != null && model.mobilenumber != null
   && model.source != "Internal")
                {
								RandomStringGenerator objRandom =new RandomStringGenerator();
								string user_passwordkey = objRandom.CreateRandomPassword(8);
								string user_password = PwdEncrypDecrypt.Encrypt(user_passwordkey,model.userpassword);
								model.userpassword=user_password;
								 
				
							}
                            using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
                            {
	                            npsql.Open();
	                            using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"checkUser_sp_users\"(@pvar_username,@pvar_userpassword,@pvar_source,@pvar_devicename,@pvar_deviceid,@pvar_notificationid,@pvar_mobilenumber,@pvar_tenantid)", npsql))
	                            {
		                            dbCommand.CommandType = CommandType.Text;
		                            dbCommand.Parameters.AddWithValue("pvar_username",(object)model.username??DBNull.Value);
                                    dbCommand.Parameters.AddWithValue("pvar_userpassword",(object)model.userpassword??DBNull.Value);
									dbCommand.Parameters.AddWithValue("pvar_source",(object)model.source??DBNull.Value);
									dbCommand.Parameters.AddWithValue("pvar_devicename",(object)model.devicename??DBNull.Value);
									dbCommand.Parameters.AddWithValue("pvar_deviceid",(object)model.deviceid??DBNull.Value);
									dbCommand.Parameters.AddWithValue("pvar_notificationid",(object)model.notificationid??DBNull.Value);
									dbCommand.Parameters.AddWithValue("pvar_mobilenumber",(object)model.mobilenumber??DBNull.Value);
									dbCommand.Parameters.AddWithValue("pvar_tenantid",NpgsqlDbType.Uuid,(object)model.tenantid??DBNull.Value);
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
					catch{
						throw;
					}
					return dataTable;	

			}

			public virtual System.Data.DataTable get_Dashboard_Items(string viewactionroles)
			{
				DataSet dataSet = new DataSet();
				DataTable dataTable = new DataTable();
				try
				{

					using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
					{
						npsql.Open();
						using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"get_sp_dashboard_items\"(@pvar_viewactionroles)", npsql))
						{
							dbCommand.CommandType = CommandType.Text;
							dbCommand.Parameters.AddWithValue("pvar_viewactionroles", (object)viewactionroles ?? DBNull.Value);
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

public virtual System.Data.DataTable List_of_User_Profiles(string tenantid
)
			  { 
					DataTable dataTable = new DataTable();
                DataSet dataSet = new DataSet(); 

					try{
 
							using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
							{
								npsql.Open();
								using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"List_of_User_Profiles\"(@pvar_tenantid)", npsql))
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

        public virtual DataTable ManageOTP(string username, string otp, string action)
        {
            DataTable dt = new DataTable();
            try
            {
                using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
                {
                    npsql.Open();
                    using (var dbCommand = new NpgsqlCommand(
                        "SELECT * FROM public.manage_user_otp(@pvar_username, @pvar_otp_code, @pvar_action)",  
                        npsql))
                    {
                        dbCommand.CommandType = CommandType.Text;
                        dbCommand.Parameters.AddWithValue("@pvar_username", (object)username ?? DBNull.Value);
                        dbCommand.Parameters.AddWithValue("@pvar_otp_code", string.IsNullOrEmpty(otp) ? 0 : int.Parse(otp));  
                        dbCommand.Parameters.AddWithValue("@pvar_action", (object)action ?? DBNull.Value);

                        using (NpgsqlDataAdapter da = new NpgsqlDataAdapter(dbCommand))
                        {
                            da.Fill(dt);
                        }
                    }
                    npsql.Close();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return dt;
        }

        public virtual string ManageProfileEmailChange(string profileType, Guid profileId,
            Guid tenantId, string newEmail, Guid modifiedUser, bool apply)
        {
            try
            {
                using (var connection = new NpgsqlConnection(db_connectionstring))
                using (var command = new NpgsqlCommand(
                    "SELECT * FROM \"Manage_Profile_Email_Change\"(@pvar_profiletype,@pvar_profileid,@pvar_tenantid,@pvar_newemail,@pvar_modifieduser,@pvar_apply)",
                    connection))
                {
                    command.Parameters.AddWithValue("pvar_profiletype", NpgsqlDbType.Varchar, profileType);
                    command.Parameters.AddWithValue("pvar_profileid", NpgsqlDbType.Uuid, profileId);
                    command.Parameters.AddWithValue("pvar_tenantid", NpgsqlDbType.Uuid, tenantId);
                    command.Parameters.AddWithValue("pvar_newemail", NpgsqlDbType.Varchar, newEmail);
                    command.Parameters.AddWithValue("pvar_modifieduser", NpgsqlDbType.Uuid, modifiedUser);
                    command.Parameters.AddWithValue("pvar_apply", NpgsqlDbType.Boolean, apply);
                    connection.Open();
                    return command.ExecuteScalar()?.ToString() ?? "Email change failed.";
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }



        public virtual System.Data.DataTable get_all_users(string tenantid)
			  { 

				    DataTable dataTable = new DataTable();
					DataSet dataSet = new DataSet();

					try{
 
							using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
							{
								npsql.Open();
								using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"get_all_users\"(@pvar_tenantid)", npsql))
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
public virtual string ChangePassword(usersChangePasswordModel  model)
			    { 
				  String ResponseMessage="";
					 
					try{
							 
                            using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
					        {
						        npsql.Open();
						        using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"ChangePassword\"(@pvar_usersid, @pvar_userpassword, @pvar_passwordkey, @pvar_modifieduser)", npsql))
						        {
                                        dbCommand.CommandType = CommandType.Text;
                                        RandomStringGenerator objRandom =new RandomStringGenerator();
                                        string user_passwordkey = objRandom.CreateRandomPassword(8);
                                        string user_password = PwdEncrypDecrypt.Encrypt(user_passwordkey,model.userpassword);
                                        model.userpassword=user_password;
                                        model.passwordkey=user_passwordkey;

                                        dbCommand.Parameters.AddWithValue("pvar_usersid",(object)model.usersid.ToString()??DBNull.Value);
                                        dbCommand.Parameters.AddWithValue("pvar_userpassword",(object)model.userpassword??DBNull.Value);
										dbCommand.Parameters.AddWithValue("pvar_passwordkey",(object)model.passwordkey??DBNull.Value);

                                        dbCommand.Parameters.AddWithValue("pvar_modifieduser",NpgsqlDbType.Uuid,model.modifieduser);


		     			 

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
public virtual System.Data.DataTable getById_allinfo_users(string usersid)
			 {
				DataSet dataSet=new DataSet();
				DataTable dataTable = new DataTable();
				try{
					     
						using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
						{
							npsql.Open();
							using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"getById_sp_all_users\"(@pvar_usersid)", npsql))
							{
								dbCommand.CommandType = CommandType.Text;
								dbCommand.Parameters.AddWithValue("pvar_usersid",(object)usersid??DBNull.Value);
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
