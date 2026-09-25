namespace NalamVazha.DAL
{
    using System;
    using System.Collections.Generic;
    using Npgsql;
    using NpgsqlTypes;
    using NalamVazha.Models;

    public class IssueTrackerDAL
    {
        public virtual string db_connectionstring { get; set; }

        public IssueTrackerDAL(string connectionString)
        {
            db_connectionstring = connectionString;
        }

        // ── Add ──────────────────────────────────────────────────────────────
        public virtual string Add_Issue(IssueTrackerModel model)
        {
            using (var conn = new NpgsqlConnection(db_connectionstring))
            {
                conn.Open();
                const string sql = @"
                    INSERT INTO issuetracker
                        (issuetrackerid, entityname, filename, pageurl,
                         issuetext, priority, status, reporterdesc,
                         screenshotdata, createddate, isdeleted)
                    VALUES
                        (@id, @entityname, @filename, @pageurl,
                         @issuetext, @priority, @status, @reporterdesc,
                         @screenshotdata, now(), false)";

                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("id",             NpgsqlDbType.Uuid,    Guid.NewGuid());
                    cmd.Parameters.AddWithValue("entityname",     NpgsqlDbType.Varchar, (object)model.entityname     ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("filename",       NpgsqlDbType.Varchar, (object)model.filename       ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("pageurl",        NpgsqlDbType.Varchar, (object)model.pageurl        ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("issuetext",      NpgsqlDbType.Text,    (object)model.issuetext      ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("priority",       NpgsqlDbType.Varchar, (object)model.priority       ?? "Medium");
                    cmd.Parameters.AddWithValue("status",         NpgsqlDbType.Varchar, (object)model.status         ?? "Open");
                    cmd.Parameters.AddWithValue("reporterdesc",   NpgsqlDbType.Varchar, (object)model.reporterdesc   ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("screenshotdata", NpgsqlDbType.Text,    (object)model.screenshotdata ?? DBNull.Value);
                    cmd.ExecuteNonQuery();
                }
            }
            return "success";
        }

        // ── Get All (list view – no screenshot blob, only a flag) ────────────
        public virtual List<IssueTrackerModel> Get_Issues()
        {
            var list = new List<IssueTrackerModel>();
            using (var conn = new NpgsqlConnection(db_connectionstring))
            {
                conn.Open();
                const string sql = @"
                    SELECT issuetrackerid, entityname, filename, pageurl,
                           issuetext, priority, status, reporterdesc,
                           createddate, modifieddate,
                           (screenshotdata IS NOT NULL AND screenshotdata <> '') AS hasscreenshot
                    FROM   issuetracker
                    WHERE  isdeleted = false
                    ORDER  BY createddate DESC";

                using (var cmd    = new NpgsqlCommand(sql, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new IssueTrackerModel
                        {
                            Issuetrackerid = reader.GetGuid(0),
                            entityname     = reader.IsDBNull(1)  ? null  : reader.GetString(1),
                            filename       = reader.IsDBNull(2)  ? null  : reader.GetString(2),
                            pageurl        = reader.IsDBNull(3)  ? null  : reader.GetString(3),
                            issuetext      = reader.IsDBNull(4)  ? null  : reader.GetString(4),
                            priority       = reader.IsDBNull(5)  ? null  : reader.GetString(5),
                            status         = reader.IsDBNull(6)  ? null  : reader.GetString(6),
                            reporterdesc   = reader.IsDBNull(7)  ? null  : reader.GetString(7),
                            createddate    = reader.IsDBNull(8)  ? (DateTime?)null : reader.GetDateTime(8),
                            modifieddate   = reader.IsDBNull(9)  ? (DateTime?)null : reader.GetDateTime(9),
                            hasscreenshot  = reader.IsDBNull(10) ? false : reader.GetBoolean(10),
                        });
                    }
                }
            }
            return list;
        }

        // ── Get Screenshot (returns only the blob for one issue) ──────────────
        public virtual string Get_Screenshot(Guid id)
        {
            using (var conn = new NpgsqlConnection(db_connectionstring))
            {
                conn.Open();
                const string sql = @"
                    SELECT screenshotdata
                    FROM   issuetracker
                    WHERE  issuetrackerid = @id
                      AND  isdeleted = false";

                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("id", NpgsqlDbType.Uuid, id);
                    var result = cmd.ExecuteScalar();
                    if (result == DBNull.Value || result == null) return null;
                var s = (string)result;
                return string.IsNullOrWhiteSpace(s) ? null : s;
                }
            }
        }

        // ── Update Status ────────────────────────────────────────────────────
        public virtual string Update_IssueStatus(Guid id, string status)
        {
            using (var conn = new NpgsqlConnection(db_connectionstring))
            {
                conn.Open();
                const string sql = @"
                    UPDATE issuetracker
                    SET    status = @status, modifieddate = now()
                    WHERE  issuetrackerid = @id";

                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("status", NpgsqlDbType.Varchar, status);
                    cmd.Parameters.AddWithValue("id",     NpgsqlDbType.Uuid,    id);
                    cmd.ExecuteNonQuery();
                }
            }
            return "success";
        }

        // ── Soft Delete ──────────────────────────────────────────────────────
        public virtual string Delete_Issue(Guid id)
        {
            using (var conn = new NpgsqlConnection(db_connectionstring))
            {
                conn.Open();
                const string sql = @"
                    UPDATE issuetracker
                    SET    isdeleted = true, modifieddate = now()
                    WHERE  issuetrackerid = @id";

                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("id", NpgsqlDbType.Uuid, id);
                    cmd.ExecuteNonQuery();
                }
            }
            return "success";
        }

        // ── Fallback: distinct controller names from roleauthorization ────────
        // Used when the Admin/Views folder is inaccessible (e.g. DLL deployment).
        public virtual List<string> GetEntitiesFromDB()
        {
            var list = new List<string>();
            using (var conn = new NpgsqlConnection(db_connectionstring))
            {
                conn.Open();
                const string sql = @"
                    SELECT DISTINCT controllername
                    FROM   roleauthorization
                    
                    ORDER  BY controllername";

                using (var cmd    = new NpgsqlCommand(sql, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                        list.Add(reader.GetString(0));
                }
            }
            return list;
        }

        // ── Fallback: distinct action names for a controller ─────────────────
        // Used when the Views/<entityname> folder is inaccessible.
        public virtual List<string> GetFilesFromDB(string entityname)
        {
            var list = new List<string>();
            if (string.IsNullOrWhiteSpace(entityname)) return list;

            using (var conn = new NpgsqlConnection(db_connectionstring))
            {
                conn.Open();
                const string sql = @"
                    SELECT DISTINCT actionmethodname
                    FROM   roleauthorization
                    WHERE  controllername = @ctrl
                       
                    ORDER  BY actionmethodname";

                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("ctrl", NpgsqlDbType.Varchar, entityname);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                            list.Add(reader.GetString(0));
                    }
                }
            }
            return list;
        }
    }
}
