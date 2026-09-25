namespace NalamVazha.DAL
{
    using System;
    using System.Data;
    using System.Data.Common;
    using Npgsql;
    using NpgsqlTypes;
    using Newtonsoft.Json.Linq;
    using NalamVazha.Models;

    //This class is a lightweight DAL for HolisticHistory (linked to IPDApplicationForm).
    public class HolisticHistoryDAL
    {
        public virtual string db_connectionstring { get; set; }

        public HolisticHistoryDAL(string connectionString)
        {
            db_connectionstring = connectionString;
        }

        public virtual string Add_HolisticHistory(HolisticHistoryModel model, string IPDFormid)
        {
            string ResponseMessage = "";

            try
            {
                Guid ipdGuid = Guid.Empty;
                Guid.TryParse(IPDFormid, out ipdGuid);

                using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
                {
                    npsql.Open();
                    using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"Add_HolisticHistory\"(@pvar_holistichistoryid,@pvar_tenantid,@pvar_ipdapplicationformid,@pvar_verifiedstatus,@pvar_verifiedby,@pvar_holisticanswers,@pvar_createduser)", npsql))
                    {
                        dbCommand.CommandType = CommandType.Text;

                        dbCommand.Parameters.AddWithValue("pvar_holistichistoryid", NpgsqlDbType.Uuid, (object)model.HolisticHistoryid ?? DBNull.Value);
                        dbCommand.Parameters.AddWithValue("pvar_tenantid", NpgsqlDbType.Uuid, (object)model.tenantid ?? DBNull.Value);
                        dbCommand.Parameters.AddWithValue("pvar_ipdapplicationformid", NpgsqlDbType.Uuid, ipdGuid == Guid.Empty ? (object)DBNull.Value : ipdGuid);

                        dbCommand.Parameters.AddWithValue("pvar_verifiedstatus", NpgsqlDbType.Varchar, (object)model.verifiedstatus ?? DBNull.Value);
                        dbCommand.Parameters.AddWithValue("pvar_verifiedby", NpgsqlDbType.Uuid, (object)model.verifiedby ?? DBNull.Value);

                        // model.holisticanswers is expected to be a valid JSON string.
                        dbCommand.Parameters.AddWithValue("pvar_holisticanswers", NpgsqlDbType.Json, (object)model.holisticanswers ?? DBNull.Value);
                        dbCommand.Parameters.AddWithValue("pvar_createduser", NpgsqlDbType.Uuid, (object)model.createduser ?? DBNull.Value);

                        NpgsqlParameter outParm = new NpgsqlParameter("pvar_returnMessage", NpgsqlDbType.Varchar)
                        {
                            Direction = ParameterDirection.Output
                        };
                        dbCommand.Parameters.Add(outParm);

                        dbCommand.ExecuteNonQuery();
                        ResponseMessage = outParm.Value?.ToString() ?? "";

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

        public virtual JObject HolisticHistory_List(string tenantid, string IPDFormid, int? pagesize = 1000, int? pagenumber = 0, string searchterm = "", string sort_fields = "")
        {
            object dalResponse = null;

            try
            {
                Guid ipdGuid = Guid.Empty;
                Guid.TryParse(IPDFormid, out ipdGuid);

                using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
                {
                    npsql.Open();
                    using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"HolisticHistory_List\"(@pvar_tenantid,@pvar_ipdapplicationformid,@pvar_pagesize,@pvar_pagenumber,@pvar_searchterm,@pvar_sort_fields)", npsql))
                    {
                        dbCommand.CommandType = CommandType.Text;

                        dbCommand.Parameters.AddWithValue("pvar_tenantid", NpgsqlDbType.Varchar, (object)tenantid ?? DBNull.Value);
                        dbCommand.Parameters.AddWithValue("pvar_ipdapplicationformid", NpgsqlDbType.Uuid, ipdGuid == Guid.Empty ? (object)DBNull.Value : ipdGuid);
                        dbCommand.Parameters.AddWithValue("pvar_pagesize", (object)pagesize ?? DBNull.Value);
                        dbCommand.Parameters.AddWithValue("pvar_pagenumber", (object)pagenumber ?? DBNull.Value);
                        dbCommand.Parameters.AddWithValue("pvar_searchterm", (object)searchterm ?? DBNull.Value);

                        if (sort_fields != null && sort_fields.Length > 2)
                            dbCommand.Parameters.AddWithValue("pvar_sort_fields", NpgsqlDbType.Json, sort_fields);
                        else
                            dbCommand.Parameters.AddWithValue("pvar_sort_fields", NpgsqlDbType.Json, DBNull.Value);

                        dalResponse = dbCommand.ExecuteScalar();
                        npsql.Close();
                    }
                }
            }
            catch
            {
                throw;
            }

            if (dalResponse == null) return new JObject();
            return JObject.Parse(dalResponse.ToString());
        }
    }
}

