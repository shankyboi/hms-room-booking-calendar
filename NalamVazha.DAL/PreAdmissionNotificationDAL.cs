namespace NalamVazha.DAL
{
    using System;
    using System.Data;
    using Npgsql;
    using NpgsqlTypes;

    public sealed class PreAdmissionNotificationDAL
    {
        private readonly string _connectionString;

        public PreAdmissionNotificationDAL(string connectionString)
        {
            _connectionString = connectionString;
        }

        public DataTable GetCandidates(DateTime businessDate)
        {
            var result = new DataTable();
            using var connection = new NpgsqlConnection(_connectionString);
            using var command = new NpgsqlCommand(
                "SELECT * FROM \"Get_PreAdmission_Notification_Candidates\"(@pvar_businessdate)", connection);
            command.Parameters.AddWithValue("pvar_businessdate", NpgsqlDbType.Date, businessDate.Date);
            using var adapter = new NpgsqlDataAdapter(command);
            adapter.Fill(result);
            return result;
        }
    }
}
