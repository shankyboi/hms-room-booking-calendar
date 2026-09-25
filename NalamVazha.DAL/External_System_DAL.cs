namespace NalamVazha.DAL
						{
							using System;
							using System.Text;
							using System.Data;
							using System.Data.Common;
							using NalamVazha.Models;
							using EncrypDecrypt;
							using Npgsql;
							using NpgsqlTypes;
							using System.IdentityModel.Tokens.Jwt;
							using System.Linq;
							//Britman Machine Version : 2.0.26.11.2019 02:26PM on 21/08/2025 20:27:45

							public class External_System_DAL
							{
								public External_System_DAL(string connectionString)
								{
									db_connectionstring = connectionString;
								}

								private string _connectionstring;
								public virtual string db_connectionstring
								{
									get
									{
										return _connectionstring;
									}
									set
									{
										_connectionstring = value;
									}
								}

								public virtual string Create_access_logs(access_logsModel model)
								{
									string ResponseMessage = "";

									try
									{
										using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
										{
											npsql.Open();
											using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"Create_access_logs\"(@pvar_access_logsid, @pvar_users_id, @pvar_logged_date, @pvar_expiry_date, @pvar_user_token, @pvar_latlan, @pvar_clientipaddress, @pvar_devicename, @pvar_browsername, @pvar_external_entity_name, @pvar_external_users_id, @pvar_createduser)", npsql))
											{
												dbCommand.CommandType = CommandType.Text;

												dbCommand.Parameters.AddWithValue("pvar_access_logsid", NpgsqlDbType.Uuid, (object)model.access_logsid ?? DBNull.Value);
												dbCommand.Parameters.AddWithValue("pvar_users_id", NpgsqlDbType.Varchar, (object)model.users_id ?? DBNull.Value);
												dbCommand.Parameters.AddWithValue("pvar_logged_date", NpgsqlDbType.Timestamp, (object)model.logged_date ?? DBNull.Value);
												dbCommand.Parameters.AddWithValue("pvar_expiry_date", NpgsqlDbType.Timestamp, (object)model.expiry_date ?? DBNull.Value);
												dbCommand.Parameters.AddWithValue("pvar_user_token", NpgsqlDbType.Varchar, (object)model.user_token ?? DBNull.Value);
												dbCommand.Parameters.AddWithValue("pvar_latlan", NpgsqlDbType.Varchar, (object)model.latlan ?? DBNull.Value);
												dbCommand.Parameters.AddWithValue("pvar_clientipaddress", NpgsqlDbType.Varchar, (object)model.clientipaddress ?? DBNull.Value);
												dbCommand.Parameters.AddWithValue("pvar_devicename", NpgsqlDbType.Varchar, (object)model.devicename ?? DBNull.Value);
												dbCommand.Parameters.AddWithValue("pvar_browsername", NpgsqlDbType.Varchar, (object)model.browsername ?? DBNull.Value);
												dbCommand.Parameters.AddWithValue("pvar_external_entity_name", NpgsqlDbType.Varchar, (object)model.external_entity_name ?? DBNull.Value);
												dbCommand.Parameters.AddWithValue("pvar_external_users_id", NpgsqlDbType.Varchar, (object)model.external_users_id ?? DBNull.Value);
												dbCommand.Parameters.AddWithValue("pvar_createduser", NpgsqlDbType.Uuid, (object)model.createduser ?? DBNull.Value);

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
									}
									catch (Exception ex)
									{
										ResponseMessage = ex.Message;
										Console.WriteLine(ex);
									}

									return ResponseMessage;
								}


								public virtual string create_access_logs_details(access_logsdetailsModel model)
								{
									string ResponseMessage = "";

									try
									{
										using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
										{
											npsql.Open();
											using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"create_access_logs_details\"(@pvar_access_logsid, @pvar_action_method_name, @pvar_api_response)", npsql))
											{
												dbCommand.CommandType = CommandType.Text; // Ensuring it's a text query to execute a stored procedure.

												// Adding parameters for the stored procedure
												dbCommand.Parameters.AddWithValue("pvar_access_logsid", NpgsqlDbType.Uuid, (object)model.access_logsid ?? DBNull.Value);
												dbCommand.Parameters.AddWithValue("pvar_action_method_name", NpgsqlDbType.Varchar, (object)model.action_method_name ?? DBNull.Value);
												dbCommand.Parameters.AddWithValue("pvar_api_response", NpgsqlDbType.Varchar, (object)model.api_response ?? DBNull.Value);

												// Output parameter for return message
												NpgsqlParameter outParm = new NpgsqlParameter("pvar_returnMessage", NpgsqlDbType.Varchar)
												{
													Direction = ParameterDirection.Output
												};
												dbCommand.Parameters.Add(outParm);

												// Execute the stored procedure
												dbCommand.ExecuteNonQuery();
												ResponseMessage = outParm.Value.ToString();

												// Dispose connection and command if not already closed
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

								public virtual string Create_api_access_logs(access_logsModel model)
								{
									string ResponseMessage = "";

									try
									{
										using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
										{
											npsql.Open();
											// Use SELECT * FROM to call the stored procedure with parameters
											using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"Create_api_access_logs\"(@pvar_api_access_logsid, @pvar_users_id, @pvar_logged_date, @pvar_user_token, @pvar_request_type, @pvar_api_url, @pvar_request_json, @pvar_response_json, @pvar_createduser)", npsql))
											{
												dbCommand.CommandType = CommandType.Text;

												// Add parameters
												dbCommand.Parameters.AddWithValue("pvar_api_access_logsid", NpgsqlDbType.Uuid, (object)model.access_logsid ?? DBNull.Value);
												dbCommand.Parameters.AddWithValue("pvar_users_id", NpgsqlDbType.Varchar, (object)model.users_id ?? DBNull.Value);
												dbCommand.Parameters.AddWithValue("pvar_logged_date", NpgsqlDbType.Timestamp, (object)model.logged_date ?? DBNull.Value);
												dbCommand.Parameters.AddWithValue("pvar_user_token", NpgsqlDbType.Varchar, (object)model.user_token ?? DBNull.Value);
												dbCommand.Parameters.AddWithValue("pvar_request_type", NpgsqlDbType.Varchar, (object)model.request_type ?? DBNull.Value);
												dbCommand.Parameters.AddWithValue("pvar_api_url", NpgsqlDbType.Varchar, (object)model.api_url ?? DBNull.Value);

												// Handle JSON values for request_json and response_json
												dbCommand.Parameters.AddWithValue("pvar_request_json", string.IsNullOrEmpty(model.request_json) ? DBNull.Value : (object)model.request_json);
												dbCommand.Parameters.AddWithValue("pvar_response_json", string.IsNullOrEmpty(model.response_json) ? DBNull.Value : (object)model.response_json);

												dbCommand.Parameters.AddWithValue("pvar_createduser", NpgsqlDbType.Uuid, (object)model.createduser ?? DBNull.Value);

												// Output parameter for return message
												NpgsqlParameter outParm = new NpgsqlParameter("pvar_returnMessage", NpgsqlDbType.Varchar)
												{
													Direction = ParameterDirection.Output
												};
												dbCommand.Parameters.Add(outParm);

												// Execute stored procedure
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



								public virtual bool get_active_token(string pvar_users_id, string pvar_external_entity_name, ref string pvar_user_token, ref string pvar_external_users_id)
								{
									bool is_token_active = false;

									try
									{
										using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
										{
											npsql.Open();
											using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"get_active_token\"(@pvar_users_id,@pvar_external_entity_name)", npsql))
											{
												dbCommand.CommandType = CommandType.Text;

												dbCommand.Parameters.AddWithValue("pvar_users_id", NpgsqlDbType.Varchar, (object)pvar_users_id ?? DBNull.Value);
												dbCommand.Parameters.AddWithValue("pvar_external_entity_name", NpgsqlDbType.Varchar, (object)pvar_external_entity_name ?? DBNull.Value);

												using (var reader = dbCommand.ExecuteReader())  // Use ExecuteReader for better performance
												{
													if (reader.HasRows)
													{
														reader.Read();  // Read the first row
														pvar_user_token = reader["user_token"].ToString();
														pvar_external_users_id = reader["external_users_id"].ToString();
														is_token_active = true;
													}
												}
											}
										}
									}
									catch (Exception ex)
									{
										// Log the error or handle it appropriately
										Console.WriteLine($"Error in get_active_token: {ex.Message}");
										throw new Exception("Error retrieving active token.", ex);  // Optionally rethrow with more info
									}

									return is_token_active;
								}
								public virtual string get_users_info_by_token(string user_token)
								{

									// Create a JwtSecurityTokenHandler to validate and read the JWT
									var tokenHandler = new JwtSecurityTokenHandler();

									// Read and parse the JWT string into a JwtSecurityToken
									var jwtToken = tokenHandler.ReadJwtToken(user_token);

									// Access the claims from the JwtSecurityToken's Claims property
									var claims = jwtToken.Claims;

									// Retrieve custom claim values by claim type
									var usersid = claims.FirstOrDefault(c => c.Type == "usersid")?.Value;


									return usersid;


								}

								public virtual String[] get_users_by_token(string user_token)
								{
									String[] userdetails = new String[2];


									DataTable dataTable = new DataTable();
									DataSet dataSet = new DataSet();


									try
									{

										using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
										{
											npsql.Open();
											using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"get_users_by_token\"(@pvar_user_token)", npsql))
											{
												dbCommand.CommandType = CommandType.Text;

												dbCommand.Parameters.AddWithValue("pvar_user_token", NpgsqlDbType.Varchar, (object)user_token ?? DBNull.Value);

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
									catch (Exception ex)
									{
										throw;
									}
									if (dataTable.Rows.Count > 0)
									{
										userdetails[0] = dataTable.Rows[0]["users_id"].ToString();
										userdetails[1] = dataTable.Rows[0]["access_logsid"].ToString();

									}

									return userdetails;


								}

							}

						}


					
