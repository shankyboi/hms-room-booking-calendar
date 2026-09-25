using System;
using System.Data;
using Npgsql;

namespace NalamVazha.DAL
{
    public class BookingEmergencyContactDAL
    {
        private readonly string connectionString;

        public BookingEmergencyContactDAL(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public void Save(string entityName, Guid entityId, string contactsJson)
        {
            using var connection = new NpgsqlConnection(connectionString);
            connection.Open();
            using var command = new NpgsqlCommand(
                "SELECT \"Save_Booking_Emergency_Contacts\"(@pvar_entity_name,@pvar_entity_id,@pvar_contacts_json)",
                connection);
            command.Parameters.AddWithValue("pvar_entity_name", entityName);
            command.Parameters.AddWithValue("pvar_entity_id", entityId);
            command.Parameters.AddWithValue("pvar_contacts_json", contactsJson ?? "[]");
            command.ExecuteNonQuery();
        }

        public int SaveOpdAndMergePatientProfile(
            Guid opdFormId,
            Guid patientProfileId,
            string contactsJson,
            Guid modifiedUser)
        {
            using var connection = new NpgsqlConnection(connectionString);
            connection.Open();
            using var command = new NpgsqlCommand(
                "SELECT \"Save_OPD_Emergency_Contacts\"(@pvar_opdformid,@pvar_patientprofileid,@pvar_contacts_json,@pvar_modifieduser)",
                connection);
            command.Parameters.AddWithValue("pvar_opdformid", opdFormId);
            command.Parameters.AddWithValue("pvar_patientprofileid", patientProfileId);
            command.Parameters.AddWithValue("pvar_contacts_json", contactsJson ?? "[]");
            command.Parameters.AddWithValue("pvar_modifieduser", modifiedUser);
            return Convert.ToInt32(command.ExecuteScalar());
        }

        public DataTable Get(string entityName, Guid entityId)
        {
            using var connection = new NpgsqlConnection(connectionString);
            connection.Open();
            using var command = new NpgsqlCommand(
                "SELECT * FROM \"Get_Booking_Emergency_Contacts\"(@pvar_entity_name,@pvar_entity_id)",
                connection);
            command.Parameters.AddWithValue("pvar_entity_name", entityName);
            command.Parameters.AddWithValue("pvar_entity_id", entityId);
            using var adapter = new NpgsqlDataAdapter(command);
            var table = new DataTable();
            adapter.Fill(table);
            return table;
        }
    }
}
