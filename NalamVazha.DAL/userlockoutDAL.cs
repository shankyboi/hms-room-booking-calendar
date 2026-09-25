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

	//This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/09/2026 05:55:01
    public class userlockoutDAL
	{
		public userlockoutDAL(string connectionString)
		{
			db_connectionstring = connectionString;
		}

		private string _connectionstring;
		public virtual string db_connectionstring
		{
			get { return _connectionstring; }
			set { _connectionstring = value; }
		}

		public virtual string ins_userlockout(userlockoutModel model)
		{
			string ResponseMessage = "";
			try
			{
				using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
				{
					npsql.Open();
				using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"ins_sp_userlockout\"(@pvar_username,@pvar_latlan,@pvar_remoteipaddress,@pvar_clientipaddress)", npsql))
				{
						dbCommand.CommandType = CommandType.Text;

				 		dbCommand.Parameters.Add(new NpgsqlParameter("pvar_username", NpgsqlDbType.Varchar) { Value = model.username });
						dbCommand.Parameters.Add(new NpgsqlParameter("pvar_latlan", NpgsqlDbType.Varchar) { Value = (object)model.latlan ?? DBNull.Value });
						dbCommand.Parameters.Add(new NpgsqlParameter("pvar_remoteipaddress", NpgsqlDbType.Varchar) { Value = (object)model.remoteipaddress ?? DBNull.Value });
						dbCommand.Parameters.Add(new NpgsqlParameter("pvar_clientipaddress", NpgsqlDbType.Varchar) { Value = (object)model.clientipaddress ?? DBNull.Value });

						NpgsqlParameter outParm = new NpgsqlParameter("pvar_returnMessage", NpgsqlDbType.Varchar)
						{
							Direction = ParameterDirection.Output
						};
						dbCommand.Parameters.Add(outParm);

						dbCommand.ExecuteNonQuery();
						ResponseMessage = outParm.Value != DBNull.Value ? outParm.Value.ToString() : "";
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

		public virtual string verify_userlockout(userlockoutModel model)
		{
			string ResponseMessage = "";

			try
			{
				using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
				{
					npsql.Open();
					using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"verify_sp_userlockout\"(@pvar_username)", npsql))
					{
						dbCommand.CommandType = CommandType.Text;

						dbCommand.Parameters.Add(new NpgsqlParameter("pvar_username", NpgsqlDbType.Varchar) { Value = model.username });

						NpgsqlParameter outParm = new NpgsqlParameter("pvar_returnMessage", NpgsqlDbType.Varchar)
						{
							Direction = ParameterDirection.Output
						};
						dbCommand.Parameters.Add(outParm);

						dbCommand.ExecuteNonQuery();
						ResponseMessage = outParm.Value != DBNull.Value ? outParm.Value.ToString() : "";
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

		public virtual string upd_userlockout(userlockoutModel model)
		{
			string ResponseMessage = "";
			try
			{
				using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
				{
					npsql.Open();
					using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"upd_sp_userlockout\"(@pvar_lockoutid,@pvar_loginUser)", npsql))
					{	
						dbCommand.CommandType = CommandType.Text;

						dbCommand.Parameters.Add(new NpgsqlParameter("pvar_lockoutid", NpgsqlDbType.Integer) { Value = model.lockoutid });
						dbCommand.Parameters.Add(new NpgsqlParameter("pvar_loginUser", NpgsqlDbType.Varchar) { Value = model.loginUser });

						// Fixed parameter name to match stored procedure expectation
						NpgsqlParameter outParm = new NpgsqlParameter("pvar_returnMessage", NpgsqlDbType.Varchar)
						{
							Direction = ParameterDirection.Output
						};
						dbCommand.Parameters.Add(outParm);

						dbCommand.ExecuteNonQuery();
						ResponseMessage = outParm.Value != DBNull.Value ? outParm.Value.ToString() : "";
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

		public virtual DataTable get_userlockout()
		{
			DataSet dataSet = new DataSet();
			DataTable dataTable = new DataTable();

			try
			{
				using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
				{
					npsql.Open();
					using (var dbCommand = new NpgsqlCommand("SELECT * FROM get_sp_userlockout()", npsql))
					{
						dbCommand.CommandType = CommandType.Text;

						using (NpgsqlDataAdapter dataAdapter = new NpgsqlDataAdapter(dbCommand))
						{
							dataSet.Reset();
							dataAdapter.Fill(dataSet);
							if (dataSet.Tables.Count > 0)
							{
								dataTable = dataSet.Tables[0];
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
