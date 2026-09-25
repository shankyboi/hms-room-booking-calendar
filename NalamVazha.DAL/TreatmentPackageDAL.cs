namespace NalamVazha.DAL
{
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

    //This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/14/2026 05:00:12
    public class TreatmentPackageDAL
    {
        public virtual string db_connectionstring { get; set; }

        public TreatmentPackageDAL(string connectionString)
        {

            db_connectionstring = connectionString;
        }

        public virtual System.Data.DataTable getById_roomtypes(string TreatmentPackageid)
        {
            DataTable dataTable = new DataTable();
            DataSet dataSet = new DataSet();
            try
            {

                using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
                {
                    npsql.Open();
                    using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"getById_sp_TreatmentPackage_roomtypes\"(@pvar_treatmentpackageid)", npsql))
                    {
                        dbCommand.CommandType = CommandType.Text;
                        dbCommand.Parameters.AddWithValue("pvar_treatmentpackageid", (object)TreatmentPackageid ?? DBNull.Value);

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

        public virtual System.Data.DataTable getById_therapy(string TreatmentPackageid)
        {
            DataTable dataTable = new DataTable();
            DataSet dataSet = new DataSet();
            try
            {

                using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
                {
                    npsql.Open();
                    using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"getById_sp_TreatmentPackage_therapy\"(@pvar_treatmentpackageid)", npsql))
                    {
                        dbCommand.CommandType = CommandType.Text;
                        dbCommand.Parameters.AddWithValue("pvar_treatmentpackageid", (object)TreatmentPackageid ?? DBNull.Value);

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

        public virtual System.Data.DataTable getById_therapykits(string TreatmentPackageid)
        {
            DataTable dataTable = new DataTable();
            DataSet dataSet = new DataSet();
            try
            {

                using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
                {
                    npsql.Open();
                    using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"getById_sp_TreatmentPackage_therapykits\"(@pvar_treatmentpackageid)", npsql))
                    {
                        dbCommand.CommandType = CommandType.Text;
                        dbCommand.Parameters.AddWithValue("pvar_treatmentpackageid", (object)TreatmentPackageid ?? DBNull.Value);

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

        public virtual System.Data.DataTable getById_therapyitems(string TreatmentPackageid)
        {
            DataTable dataTable = new DataTable();
            DataSet dataSet = new DataSet();
            try
            {

                using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
                {
                    npsql.Open();
                    using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"getById_sp_TreatmentPackage_therapyitems\"(@pvar_treatmentpackageid)", npsql))
                    {
                        dbCommand.CommandType = CommandType.Text;
                        dbCommand.Parameters.AddWithValue("pvar_treatmentpackageid", (object)TreatmentPackageid ?? DBNull.Value);

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

        public virtual System.Data.DataTable getById_medicines(string TreatmentPackageid)
        {
            DataTable dataTable = new DataTable();
            DataSet dataSet = new DataSet();
            try
            {

                using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
                {
                    npsql.Open();
                    using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"getById_sp_TreatmentPackage_medicines\"(@pvar_treatmentpackageid)", npsql))
                    {
                        dbCommand.CommandType = CommandType.Text;
                        dbCommand.Parameters.AddWithValue("pvar_treatmentpackageid", (object)TreatmentPackageid ?? DBNull.Value);

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

        public virtual System.Data.DataTable getById_refundpolicy(string TreatmentPackageid)
        {
            DataTable dataTable = new DataTable();
            DataSet dataSet = new DataSet();
            try
            {

                using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
                {
                    npsql.Open();
                    using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"getById_sp_TreatmentPackage_refundpolicy\"(@pvar_treatmentpackageid)", npsql))
                    {
                        dbCommand.CommandType = CommandType.Text;
                        dbCommand.Parameters.AddWithValue("pvar_treatmentpackageid", (object)TreatmentPackageid ?? DBNull.Value);

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


        public virtual string Add_Treatment_Package(TreatmentPackageModel model)
        {
            String ResponseMessage = "";

            try
            {

                using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
                {
                    npsql.Open();
                    using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"Add_Treatment_Package\"(@pvar_treatmentpackageid,@pvar_tenantid,@pvar_packagename,@pvar_noofdays,@pvar_roomtypeamountaverage,@pvar_therapyamount,@pvar_therapykitamount,@pvar_therapyitemamount,@pvar_medicineamount,@pvar_calculatedpackagecost,@pvar_packagecost,@pvar_packagebookingadvance,@pvar_packagebookingdeposit,@pvar_billingwaiverfordelayedstart,@pvar_waiverpercentage,@pvar_roomtransfercost,@pvar_packagedescription,@pvar_roomtypes,@pvar_therapy,@pvar_therapykits,@pvar_therapyitems,@pvar_medicines,@pvar_refundpolicy,@pvar_createduser)", npsql))
                    {
                        dbCommand.CommandType = CommandType.Text;

                        dbCommand.Parameters.AddWithValue("pvar_treatmentpackageid", NpgsqlDbType.Uuid, (object)model.TreatmentPackageid ?? DBNull.Value);
                        dbCommand.Parameters.AddWithValue("pvar_tenantid", NpgsqlDbType.Uuid, (object)model.tenantid ?? DBNull.Value);

                        dbCommand.Parameters.AddWithValue("pvar_packagename", NpgsqlDbType.Varchar, (object)model.packagename ?? DBNull.Value);

                        dbCommand.Parameters.AddWithValue("pvar_noofdays", NpgsqlDbType.Integer, (object)model.noofdays ?? DBNull.Value);

                        dbCommand.Parameters.AddWithValue("pvar_roomtypeamountaverage", NpgsqlDbType.Varchar, (object)model.roomtypeamountaverage ?? DBNull.Value);

                        dbCommand.Parameters.AddWithValue("pvar_therapyamount", NpgsqlDbType.Varchar, (object)model.therapyamount ?? DBNull.Value);

                        dbCommand.Parameters.AddWithValue("pvar_therapykitamount", NpgsqlDbType.Varchar, (object)model.therapykitamount ?? DBNull.Value);

                        dbCommand.Parameters.AddWithValue("pvar_therapyitemamount", NpgsqlDbType.Varchar, (object)model.therapyitemamount ?? DBNull.Value);

                        dbCommand.Parameters.AddWithValue("pvar_medicineamount", NpgsqlDbType.Varchar, (object)model.medicineamount ?? DBNull.Value);

                        dbCommand.Parameters.AddWithValue("pvar_calculatedpackagecost", NpgsqlDbType.Varchar, (object)model.calculatedpackagecost ?? DBNull.Value);

                        dbCommand.Parameters.AddWithValue("pvar_packagecost", NpgsqlDbType.Numeric, (object)model.packagecost ?? DBNull.Value);

                        dbCommand.Parameters.AddWithValue("pvar_packagebookingadvance", NpgsqlDbType.Numeric, (object)model.packagebookingadvance ?? DBNull.Value);

                        dbCommand.Parameters.AddWithValue("pvar_packagebookingdeposit", NpgsqlDbType.Numeric, (object)model.packagebookingdeposit ?? DBNull.Value);

                        dbCommand.Parameters.AddWithValue("pvar_billingwaiverfordelayedstart", NpgsqlDbType.Varchar, (object)model.billingwaiverfordelayedstart ?? DBNull.Value);

                        dbCommand.Parameters.AddWithValue("pvar_waiverpercentage", NpgsqlDbType.Numeric, (object)model.waiverpercentage ?? DBNull.Value);

                        dbCommand.Parameters.AddWithValue("pvar_roomtransfercost", NpgsqlDbType.Varchar, (object)model.roomtransfercost ?? DBNull.Value);

                        dbCommand.Parameters.AddWithValue("pvar_packagedescription", NpgsqlDbType.Varchar, (object)model.packagedescription ?? DBNull.Value);
                        if (model.roomtypes != null && model.roomtypes.Count > 0)
                            dbCommand.Parameters.AddWithValue("pvar_roomtypes", NpgsqlDbType.Json, JsonConvert.SerializeObject(model.roomtypes));
                        else
                            dbCommand.Parameters.AddWithValue("pvar_roomtypes", NpgsqlDbType.Json, DBNull.Value);
                        if (model.therapy != null && model.therapy.Count > 0)
                            dbCommand.Parameters.AddWithValue("pvar_therapy", NpgsqlDbType.Json, JsonConvert.SerializeObject(model.therapy));
                        else
                            dbCommand.Parameters.AddWithValue("pvar_therapy", NpgsqlDbType.Json, DBNull.Value);
                        if (model.therapykits != null && model.therapykits.Count > 0)
                            dbCommand.Parameters.AddWithValue("pvar_therapykits", NpgsqlDbType.Json, JsonConvert.SerializeObject(model.therapykits));
                        else
                            dbCommand.Parameters.AddWithValue("pvar_therapykits", NpgsqlDbType.Json, DBNull.Value);
                        if (model.therapyitems != null && model.therapyitems.Count > 0)
                            dbCommand.Parameters.AddWithValue("pvar_therapyitems", NpgsqlDbType.Json, JsonConvert.SerializeObject(model.therapyitems));
                        else
                            dbCommand.Parameters.AddWithValue("pvar_therapyitems", NpgsqlDbType.Json, DBNull.Value);
                        if (model.medicines != null && model.medicines.Count > 0)
                            dbCommand.Parameters.AddWithValue("pvar_medicines", NpgsqlDbType.Json, JsonConvert.SerializeObject(model.medicines));
                        else
                            dbCommand.Parameters.AddWithValue("pvar_medicines", NpgsqlDbType.Json, DBNull.Value);
                        if (model.refundpolicy != null && model.refundpolicy.Count > 0)
                            dbCommand.Parameters.AddWithValue("pvar_refundpolicy", NpgsqlDbType.Json, JsonConvert.SerializeObject(model.refundpolicy));
                        else
                            dbCommand.Parameters.AddWithValue("pvar_refundpolicy", NpgsqlDbType.Json, DBNull.Value);
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
        public virtual TreatmentPackageModel getById_TreatmentPackage(string TreatmentPackageid)
        {
            DataTable dataTable = new DataTable();
            DataSet dataSet = new DataSet();
            try
            {

                using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
                {
                    npsql.Open();
                    using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"getById_sp_TreatmentPackage\"(@pvar_treatmentpackageid)", npsql))
                    {
                        dbCommand.CommandType = CommandType.Text;
                        dbCommand.Parameters.AddWithValue("pvar_treatmentpackageid", (object)TreatmentPackageid ?? DBNull.Value);
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
            if (dataTable.Rows.Count > 0)
            {
                DataRow row = dataTable.Rows[0];
                return ModelConverter.ConvertDataRowToModel<TreatmentPackageModel>(row);
            }
            else
            {
                return null;
            }
        }
        public virtual string Update_Treatment_Package(TreatmentPackageModel model)
        {
            String ResponseMessage = "";
            try
            {

                using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
                {
                    npsql.Open();
                    using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"Update_Treatment_Package\"(@pvar_treatmentpackageid,@pvar_tenantid,@pvar_packagename,@pvar_noofdays,@pvar_roomtypeamountaverage,@pvar_therapyamount,@pvar_therapykitamount,@pvar_therapyitemamount,@pvar_medicineamount,@pvar_calculatedpackagecost,@pvar_packagecost,@pvar_packagebookingadvance,@pvar_packagebookingdeposit,@pvar_billingwaiverfordelayedstart,@pvar_waiverpercentage,@pvar_roomtransfercost,@pvar_packagedescription,@pvar_roomtypes,@pvar_therapy,@pvar_therapykits,@pvar_therapyitems,@pvar_medicines,@pvar_refundpolicy,@pvar_modifieduser)", npsql))
                    {
                        dbCommand.CommandType = CommandType.Text;
                        dbCommand.Parameters.AddWithValue("pvar_treatmentpackageid", NpgsqlDbType.Uuid, (object)model.TreatmentPackageid ?? DBNull.Value);
                        dbCommand.Parameters.AddWithValue("pvar_tenantid", NpgsqlDbType.Uuid, (object)model.tenantid ?? DBNull.Value);

                        dbCommand.Parameters.AddWithValue("pvar_packagename", NpgsqlDbType.Varchar, (object)model.packagename ?? DBNull.Value);

                        dbCommand.Parameters.AddWithValue("pvar_noofdays", NpgsqlDbType.Integer, (object)model.noofdays ?? DBNull.Value);

                        dbCommand.Parameters.AddWithValue("pvar_roomtypeamountaverage", NpgsqlDbType.Varchar, (object)model.roomtypeamountaverage ?? DBNull.Value);

                        dbCommand.Parameters.AddWithValue("pvar_therapyamount", NpgsqlDbType.Varchar, (object)model.therapyamount ?? DBNull.Value);

                        dbCommand.Parameters.AddWithValue("pvar_therapykitamount", NpgsqlDbType.Varchar, (object)model.therapykitamount ?? DBNull.Value);

                        dbCommand.Parameters.AddWithValue("pvar_therapyitemamount", NpgsqlDbType.Varchar, (object)model.therapyitemamount ?? DBNull.Value);

                        dbCommand.Parameters.AddWithValue("pvar_medicineamount", NpgsqlDbType.Varchar, (object)model.medicineamount ?? DBNull.Value);

                        dbCommand.Parameters.AddWithValue("pvar_calculatedpackagecost", NpgsqlDbType.Varchar, (object)model.calculatedpackagecost ?? DBNull.Value);

                        dbCommand.Parameters.AddWithValue("pvar_packagecost", NpgsqlDbType.Numeric, (object)model.packagecost ?? DBNull.Value);

                        dbCommand.Parameters.AddWithValue("pvar_packagebookingadvance", NpgsqlDbType.Numeric, (object)model.packagebookingadvance ?? DBNull.Value);

                        dbCommand.Parameters.AddWithValue("pvar_packagebookingdeposit", NpgsqlDbType.Numeric, (object)model.packagebookingdeposit ?? DBNull.Value);

                        dbCommand.Parameters.AddWithValue("pvar_billingwaiverfordelayedstart", NpgsqlDbType.Varchar, (object)model.billingwaiverfordelayedstart ?? DBNull.Value);

                        dbCommand.Parameters.AddWithValue("pvar_waiverpercentage", NpgsqlDbType.Numeric, (object)model.waiverpercentage ?? DBNull.Value);

                        dbCommand.Parameters.AddWithValue("pvar_roomtransfercost", NpgsqlDbType.Varchar, (object)model.roomtransfercost ?? DBNull.Value);

                        dbCommand.Parameters.AddWithValue("pvar_packagedescription", NpgsqlDbType.Varchar, (object)model.packagedescription ?? DBNull.Value);
                        if (model.roomtypes != null && model.roomtypes.Count > 0)
                            dbCommand.Parameters.AddWithValue("pvar_roomtypes", NpgsqlDbType.Json, JsonConvert.SerializeObject(model.roomtypes));
                        else
                            dbCommand.Parameters.AddWithValue("pvar_roomtypes", NpgsqlDbType.Json, DBNull.Value);
                        if (model.therapy != null && model.therapy.Count > 0)
                            dbCommand.Parameters.AddWithValue("pvar_therapy", NpgsqlDbType.Json, JsonConvert.SerializeObject(model.therapy));
                        else
                            dbCommand.Parameters.AddWithValue("pvar_therapy", NpgsqlDbType.Json, DBNull.Value);
                        if (model.therapykits != null && model.therapykits.Count > 0)
                            dbCommand.Parameters.AddWithValue("pvar_therapykits", NpgsqlDbType.Json, JsonConvert.SerializeObject(model.therapykits));
                        else
                            dbCommand.Parameters.AddWithValue("pvar_therapykits", NpgsqlDbType.Json, DBNull.Value);
                        if (model.therapyitems != null && model.therapyitems.Count > 0)
                            dbCommand.Parameters.AddWithValue("pvar_therapyitems", NpgsqlDbType.Json, JsonConvert.SerializeObject(model.therapyitems));
                        else
                            dbCommand.Parameters.AddWithValue("pvar_therapyitems", NpgsqlDbType.Json, DBNull.Value);
                        if (model.medicines != null && model.medicines.Count > 0)
                            dbCommand.Parameters.AddWithValue("pvar_medicines", NpgsqlDbType.Json, JsonConvert.SerializeObject(model.medicines));
                        else
                            dbCommand.Parameters.AddWithValue("pvar_medicines", NpgsqlDbType.Json, DBNull.Value);
                        if (model.refundpolicy != null && model.refundpolicy.Count > 0)
                            dbCommand.Parameters.AddWithValue("pvar_refundpolicy", NpgsqlDbType.Json, JsonConvert.SerializeObject(model.refundpolicy));
                        else
                            dbCommand.Parameters.AddWithValue("pvar_refundpolicy", NpgsqlDbType.Json, DBNull.Value);
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
        public virtual string Remove_Treatment_Package(string id, string loginUserID)
        {
            String ResponseMessage = "";
            try
            {
                using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
                {
                    npsql.Open();
                    using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"Remove_Treatment_Package\"(@pvar_treatmentpackageid,@pvar_modifieduser)", npsql))
                    {
                        dbCommand.CommandType = CommandType.Text;
                        dbCommand.Parameters.AddWithValue("pvar_treatmentpackageid", (object)id ?? DBNull.Value);
                        dbCommand.Parameters.AddWithValue("pvar_modifieduser", (object)loginUserID ?? DBNull.Value);
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
        public virtual System.Data.DataTable Treatment_Package_List(string tenantid
        )
        {
            DataTable dataTable = new DataTable();
            DataSet dataSet = new DataSet();

            try
            {

                using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
                {
                    npsql.Open();
                    using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"Treatment_Package_List\"(@pvar_tenantid)", npsql))
                    {
                        dbCommand.CommandType = CommandType.Text;
                        dbCommand.Parameters.AddWithValue("pvar_tenantid", (object)tenantid ?? DBNull.Value);

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


        public virtual System.Data.DataTable get_all_TreatmentPackage(string tenantid)
        {

            DataTable dataTable = new DataTable();
            DataSet dataSet = new DataSet();

            try
            {

                using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
                {
                    npsql.Open();
                    using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"get_all_TreatmentPackage\"(@pvar_tenantid)", npsql))
                    {
                        dbCommand.CommandType = CommandType.Text;
                        dbCommand.Parameters.AddWithValue("pvar_tenantid", (object)tenantid ?? DBNull.Value);

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
        public virtual System.Data.DataTable getById_allinfo_TreatmentPackage(string TreatmentPackageid)
        {
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();
            try
            {

                using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
                {
                    npsql.Open();
                    using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"getById_sp_all_TreatmentPackage\"(@pvar_treatmentpackageid)", npsql))
                    {
                        dbCommand.CommandType = CommandType.Text;
                        dbCommand.Parameters.AddWithValue("pvar_treatmentpackageid", (object)TreatmentPackageid ?? DBNull.Value);
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

        public virtual System.Data.DataTable lookup_TreatmentPackage_roomtypes_roomtype(String tenantid)
        {
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();
            try
            {

                using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
                {
                    npsql.Open();
                    using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"lookup_TreatmentPackage_roomtypes_roomtype\"(@pvar_tenantid)", npsql))
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
        public virtual System.Data.DataTable lookup_TreatmentPackage_therapy_therapyname(String tenantid)
        {
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();
            try
            {

                using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
                {
                    npsql.Open();
                    using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"lookup_TreatmentPackage_therapy_therapyname\"(@pvar_tenantid)", npsql))
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
        public virtual System.Data.DataTable lookup_TreatmentPackage_therapykits_therapykitname(String tenantid)
        {
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();
            try
            {

                using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
                {
                    npsql.Open();
                    using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"lookup_TreatmentPackage_therapykits_therapykitname\"(@pvar_tenantid)", npsql))
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
        public virtual System.Data.DataTable lookup_TreatmentPackage_therapyitems_therapyitem(String tenantid)
        {
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();
            try
            {

                using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
                {
                    npsql.Open();
                    using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"lookup_TreatmentPackage_therapyitems_therapyitem\"(@pvar_tenantid)", npsql))
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
        public virtual System.Data.DataTable lookup_TreatmentPackage_medicines_medicinename(String tenantid, string searchterm = "", int? pagesize = 1000, int? pagenumber = 0)
        {
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();
            try
            {

                using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
                {
                    npsql.Open();
                    using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"lookup_TreatmentPackage_medicines_medicinename\"(@pvar_tenantid,@pvar_searchterm,@pvar_pagesize,@pvar_pagenumber)", npsql))
                    {

                        dbCommand.Parameters.AddWithValue("pvar_tenantid", (object)tenantid ?? DBNull.Value);
                        dbCommand.Parameters.AddWithValue("pvar_searchterm", (object)searchterm ?? DBNull.Value);
                        dbCommand.Parameters.AddWithValue("pvar_pagesize", (object)pagesize ?? DBNull.Value);
                        dbCommand.Parameters.AddWithValue("pvar_pagenumber", (object)pagenumber ?? DBNull.Value);

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


        public virtual System.Data.DataTable lookup_change_medicines_TreatmentPackage_medicinename(string Medicineid)
        {
            DataTable dataTable = new DataTable();
            DataSet dataSet = new DataSet();
            try
            {

                using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
                {
                    npsql.Open();
                    using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"lookup_change_medicines_TreatmentPackage_medicinename\"(@pvar_medicineid)", npsql))
                    {
                        dbCommand.CommandType = CommandType.Text;
                        dbCommand.Parameters.AddWithValue("pvar_medicineid", NpgsqlDbType.Varchar, (object)Medicineid ?? DBNull.Value);
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
        public virtual System.Data.DataTable lookup_change_therapyitems_TreatmentPackage_therapyitem(string TherapyItemid)
        {
            DataTable dataTable = new DataTable();
            DataSet dataSet = new DataSet();
            try
            {

                using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
                {
                    npsql.Open();
                    using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"lookup_change_therapyitems_TreatmentPackage_therapyitem\"(@pvar_therapyitemid)", npsql))
                    {
                        dbCommand.CommandType = CommandType.Text;
                        dbCommand.Parameters.AddWithValue("pvar_therapyitemid", NpgsqlDbType.Varchar, (object)TherapyItemid ?? DBNull.Value);
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
        public virtual System.Data.DataTable lookup_change_therapykits_TreatmentPackage_therapykitname(string TherapyKitid)
        {
            DataTable dataTable = new DataTable();
            DataSet dataSet = new DataSet();
            try
            {

                using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
                {
                    npsql.Open();
                    using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"lookup_change_therapykits_TreatmentPackage_therapykitname\"(@pvar_therapykitid)", npsql))
                    {
                        dbCommand.CommandType = CommandType.Text;
                        dbCommand.Parameters.AddWithValue("pvar_therapykitid", NpgsqlDbType.Varchar, (object)TherapyKitid ?? DBNull.Value);
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
        public virtual System.Data.DataTable lookup_change_therapy_TreatmentPackage_therapyname(string Therapiesid)
        {
            DataTable dataTable = new DataTable();
            DataSet dataSet = new DataSet();
            try
            {

                using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
                {
                    npsql.Open();
                    using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"lookup_change_therapy_TreatmentPackage_therapyname\"(@pvar_therapiesid)", npsql))
                    {
                        dbCommand.CommandType = CommandType.Text;
                        dbCommand.Parameters.AddWithValue("pvar_therapiesid", NpgsqlDbType.Varchar, (object)Therapiesid ?? DBNull.Value);
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
        public virtual System.Data.DataTable lookup_change_roomtypes_TreatmentPackage_roomtype(string RoomTypeid)
        {
            DataTable dataTable = new DataTable();
            DataSet dataSet = new DataSet();
            try
            {

                using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
                {
                    npsql.Open();
                    using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"lookup_change_roomtypes_TreatmentPackage_roomtype\"(@pvar_roomtypeid)", npsql))
                    {
                        dbCommand.CommandType = CommandType.Text;
                        dbCommand.Parameters.AddWithValue("pvar_roomtypeid", NpgsqlDbType.Varchar, (object)RoomTypeid ?? DBNull.Value);
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
