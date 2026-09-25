namespace NalamVazha.DAL
{
	using System;
	using System.Text;
	using System.Data;
	using System.Data.Common;
	using NalamVazha.Models;
	using Npgsql;
	using NpgsqlTypes;

	public class lookupsDAL
	{
		public lookupsDAL(string connectionString)
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

		public virtual string ins_lookups(lookupsModel model)
		{
			string ResponseMessage = "";
			try
			{
				using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
				{
					npsql.Open();
					using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"ins_sp_lookups\"(@pvar_entityname, @pvar_attributetype, @pvar_fieldname, @pvar_fielddesc, @pvar_createduser)", npsql))
					{
						dbCommand.CommandType = CommandType.Text;
						dbCommand.Parameters.AddWithValue("pvar_entityname", (object)model.entityname ?? DBNull.Value);
						dbCommand.Parameters.AddWithValue("pvar_attributetype", (object)model.attributetype ?? DBNull.Value);
						dbCommand.Parameters.AddWithValue("pvar_fieldname", (object)model.fieldname ?? DBNull.Value);
						dbCommand.Parameters.AddWithValue("pvar_fielddesc", (object)model.fielddesc ?? DBNull.Value);
						dbCommand.Parameters.AddWithValue("pvar_createduser", NpgsqlDbType.Uuid, (object)model.createduser ?? DBNull.Value);

						NpgsqlParameter outParm = new NpgsqlParameter("pvar_returnMessage", NpgsqlDbType.Varchar)
						{
							Direction = ParameterDirection.Output
						};
						dbCommand.Parameters.Add(outParm);

						dbCommand.ExecuteNonQuery();
						ResponseMessage = outParm.Value.ToString();
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

		public virtual DataTable get_lookups()
		{
			DataSet dataSet = new DataSet();
			DataTable dataTable = new DataTable();
			try
			{
				using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
				{
					npsql.Open();
					using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"get_sp_lookups\"()", npsql))
					{
						dbCommand.CommandType = CommandType.Text;
						using (NpgsqlDataAdapter dataAdapter = new NpgsqlDataAdapter(dbCommand))
						{
							dataSet.Reset();
							dataAdapter.Fill(dataSet);
							dataTable = dataSet.Tables[0];
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

		public virtual DataTable getById_lookups(string id)
		{
			DataSet dataSet = new DataSet();
			DataTable dataTable = new DataTable();
			try
			{
				using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
				{
					npsql.Open();
					using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"getById_sp_lookups\"(@pvar_lookup_id)", npsql))
					{
						dbCommand.CommandType = CommandType.Text;
						dbCommand.Parameters.AddWithValue("pvar_lookup_id", id ?? (object)DBNull.Value);
						using (NpgsqlDataAdapter dataAdapter = new NpgsqlDataAdapter(dbCommand))
						{
							dataSet.Reset();
							dataAdapter.Fill(dataSet);
							dataTable = dataSet.Tables[0];
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

		public virtual DataTable get_lookups_by_entity(string id)
		{
			DataSet dataSet = new DataSet();
			DataTable dataTable = new DataTable();
			try
			{
				using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
				{
					npsql.Open();
					using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"get_sp_lookups_by_entity\"(@pvar_entityname)", npsql))
					{
						dbCommand.CommandType = CommandType.Text;
						dbCommand.Parameters.AddWithValue("pvar_entityname", id ?? (object)DBNull.Value);
						using (NpgsqlDataAdapter dataAdapter = new NpgsqlDataAdapter(dbCommand))
						{
							dataSet.Reset();
							dataAdapter.Fill(dataSet);
							dataTable = dataSet.Tables[0];
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

		public virtual DataTable getLookUp_tenant_id()
		{
			DataSet dataSet = new DataSet();
			DataTable dataTable = new DataTable();
			try
			{
				using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
				{
					npsql.Open();
					using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"get_lookUptenant_id\"()", npsql))
					{
						dbCommand.CommandType = CommandType.Text;
						using (NpgsqlDataAdapter dataAdapter = new NpgsqlDataAdapter(dbCommand))
						{
							dataSet.Reset();
							dataAdapter.Fill(dataSet);
							dataTable = dataSet.Tables[0];
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

