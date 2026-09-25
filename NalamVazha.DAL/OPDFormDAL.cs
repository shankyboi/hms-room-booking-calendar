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

			    //This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:42:54
			    public class OPDFormDAL
			    {
					public virtual string db_connectionstring{get;set;}

			 	    public OPDFormDAL(string connectionString)
				    {

					    db_connectionstring=connectionString;
				    }

			        public virtual System.Data.DataTable getById_medicalinfo(string OPDFormid)
			 {
					DataTable dataTable=new DataTable();
					DataSet dataSet=new DataSet();
					try
					{

							using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
							{
								npsql.Open();
								using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"getById_sp_OPDForm_medicalinfo\"(@pvar_opdformid)", npsql))
								{
									dbCommand.CommandType = CommandType.Text;
									dbCommand.Parameters.AddWithValue("pvar_opdformid",(object)OPDFormid??DBNull.Value);

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

public virtual System.Data.DataTable getById_medicationinfo(string OPDFormid)
			 {
					DataTable dataTable=new DataTable();
					DataSet dataSet=new DataSet();
					try
					{

							using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
							{
								npsql.Open();
								using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"getById_sp_OPDForm_medicationinfo\"(@pvar_opdformid)", npsql))
								{
									dbCommand.CommandType = CommandType.Text;
									dbCommand.Parameters.AddWithValue("pvar_opdformid",(object)OPDFormid??DBNull.Value);

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

public virtual System.Data.DataTable getById_medicalrecords(string OPDFormid)
			 {
					DataTable dataTable=new DataTable();
					DataSet dataSet=new DataSet();
					try
					{

							using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
							{
								npsql.Open();
								using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"getById_sp_OPDForm_medicalrecords\"(@pvar_opdformid)", npsql))
								{
									dbCommand.CommandType = CommandType.Text;
									dbCommand.Parameters.AddWithValue("pvar_opdformid",(object)OPDFormid??DBNull.Value);

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

public virtual System.Data.DataTable getById_appointmentpreferences(string OPDFormid)
			 {
					DataTable dataTable=new DataTable();
					DataSet dataSet=new DataSet();
					try
					{

							using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
							{
								npsql.Open();
								using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"getById_sp_OPDForm_appointmentpreferences\"(@pvar_opdformid)", npsql))
								{
									dbCommand.CommandType = CommandType.Text;
									dbCommand.Parameters.AddWithValue("pvar_opdformid",(object)OPDFormid??DBNull.Value);

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


              public virtual string Add_OPD_Form(OPDFormModel model)
			  {
				  String ResponseMessage="";

					try{

                            using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
					        {
						        npsql.Open();
						        using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"Add_OPD_Form\"(@pvar_opdformid,@pvar_tenantid,@pvar_bookingreferencenumber,@pvar_patientname,@pvar_appointmentmode,@pvar_preferreddoctor,@pvar_task,@pvar_verifiedstatus,@pvar_medicalinfo,@pvar_medicationinfo,@pvar_medicalrecords,@pvar_appointmentpreferences,@pvar_createduser)", npsql))
						        {
                                        dbCommand.CommandType = CommandType.Text;

								        					dbCommand.Parameters.AddWithValue("pvar_opdformid",NpgsqlDbType.Uuid,(object)model.OPDFormid??DBNull.Value);
dbCommand.Parameters.AddWithValue("pvar_tenantid",NpgsqlDbType.Uuid,(object)model.tenantid??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_bookingreferencenumber",NpgsqlDbType.Varchar,(object)model.bookingreferencenumber??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_patientname",NpgsqlDbType.Uuid,(object)model.patientname??DBNull.Value);
						dbCommand.Parameters.AddWithValue("pvar_appointmentmode",NpgsqlDbType.Varchar,(object)model.appointmentmode??DBNull.Value);
						dbCommand.Parameters.AddWithValue("pvar_preferreddoctor",NpgsqlDbType.Uuid,(object)model.preferreddoctor??DBNull.Value);
						dbCommand.Parameters.AddWithValue("pvar_task", NpgsqlDbType.Uuid, model.task.HasValue && model.task.Value != Guid.Empty ? (object)model.task.Value : DBNull.Value);


						dbCommand.Parameters.AddWithValue("pvar_verifiedstatus",NpgsqlDbType.Varchar,(object)model.verifiedstatus??DBNull.Value);
if(model.medicalinfo !=null  && model.medicalinfo.Count >0)
dbCommand.Parameters.AddWithValue("pvar_medicalinfo",NpgsqlDbType.Json,JsonConvert.SerializeObject(model.medicalinfo));
else
dbCommand.Parameters.AddWithValue("pvar_medicalinfo",NpgsqlDbType.Json,DBNull.Value);
if(model.medicationinfo !=null  && model.medicationinfo.Count >0)
dbCommand.Parameters.AddWithValue("pvar_medicationinfo",NpgsqlDbType.Json,JsonConvert.SerializeObject(model.medicationinfo));
else
dbCommand.Parameters.AddWithValue("pvar_medicationinfo",NpgsqlDbType.Json,DBNull.Value);
if(model.medicalrecords !=null  && model.medicalrecords.Count >0)
dbCommand.Parameters.AddWithValue("pvar_medicalrecords",NpgsqlDbType.Json,JsonConvert.SerializeObject(model.medicalrecords));
else
dbCommand.Parameters.AddWithValue("pvar_medicalrecords",NpgsqlDbType.Json,DBNull.Value);
if(model.appointmentpreferences !=null  && model.appointmentpreferences.Count >0)
dbCommand.Parameters.AddWithValue("pvar_appointmentpreferences",NpgsqlDbType.Json,JsonConvert.SerializeObject(model.appointmentpreferences));
else
dbCommand.Parameters.AddWithValue("pvar_appointmentpreferences",NpgsqlDbType.Json,DBNull.Value);
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
public virtual OPDFormModel getById_OPDForm(string OPDFormid)
									 {
										DataTable dataTable = new DataTable();
										DataSet dataSet = new DataSet();
										try{

												using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
												{
													npsql.Open();
													using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"getById_sp_OPDForm\"(@pvar_opdformid)", npsql))
													{
														dbCommand.CommandType = CommandType.Text;
														dbCommand.Parameters.AddWithValue("pvar_opdformid",(object)OPDFormid??DBNull.Value);
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
											return ModelConverter.ConvertDataRowToModel<OPDFormModel>(row);
										}
										else
										{
											return null;
										}
									 }
			 public virtual string  Update_OPD_Form(OPDFormModel model)
			 {
				 String ResponseMessage="";
					try{

							using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
							{
								npsql.Open();
								using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"Update_OPD_Form\"(@pvar_opdformid,@pvar_tenantid,@pvar_bookingreferencenumber,@pvar_patientname,@pvar_appointmentmode,@pvar_preferreddoctor,@pvar_verifiedstatus,@pvar_medicalinfo,@pvar_medicationinfo,@pvar_medicalrecords,@pvar_appointmentpreferences,@pvar_modifieduser)", npsql))
								{
										dbCommand.CommandType = CommandType.Text;
															dbCommand.Parameters.AddWithValue("pvar_opdformid",NpgsqlDbType.Uuid,(object)model.OPDFormid??DBNull.Value);
dbCommand.Parameters.AddWithValue("pvar_tenantid",NpgsqlDbType.Uuid,(object)model.tenantid??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_bookingreferencenumber",NpgsqlDbType.Varchar,(object)model.bookingreferencenumber??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_patientname",NpgsqlDbType.Uuid,(object)model.patientname??DBNull.Value);
						dbCommand.Parameters.AddWithValue("pvar_appointmentmode",NpgsqlDbType.Varchar,(object)model.appointmentmode??DBNull.Value);
						dbCommand.Parameters.AddWithValue("pvar_preferreddoctor",NpgsqlDbType.Uuid,(object)model.preferreddoctor??DBNull.Value);

						dbCommand.Parameters.AddWithValue("pvar_verifiedstatus",NpgsqlDbType.Varchar,(object)model.verifiedstatus??DBNull.Value);
if(model.medicalinfo !=null  && model.medicalinfo.Count >0)
dbCommand.Parameters.AddWithValue("pvar_medicalinfo",NpgsqlDbType.Json,JsonConvert.SerializeObject(model.medicalinfo));
else
dbCommand.Parameters.AddWithValue("pvar_medicalinfo",NpgsqlDbType.Json,DBNull.Value);
if(model.medicationinfo !=null  && model.medicationinfo.Count >0)
dbCommand.Parameters.AddWithValue("pvar_medicationinfo",NpgsqlDbType.Json,JsonConvert.SerializeObject(model.medicationinfo));
else
dbCommand.Parameters.AddWithValue("pvar_medicationinfo",NpgsqlDbType.Json,DBNull.Value);
if(model.medicalrecords !=null  && model.medicalrecords.Count >0)
dbCommand.Parameters.AddWithValue("pvar_medicalrecords",NpgsqlDbType.Json,JsonConvert.SerializeObject(model.medicalrecords));
else
dbCommand.Parameters.AddWithValue("pvar_medicalrecords",NpgsqlDbType.Json,DBNull.Value);
if(model.appointmentpreferences !=null  && model.appointmentpreferences.Count >0)
dbCommand.Parameters.AddWithValue("pvar_appointmentpreferences",NpgsqlDbType.Json,JsonConvert.SerializeObject(model.appointmentpreferences));
else
dbCommand.Parameters.AddWithValue("pvar_appointmentpreferences",NpgsqlDbType.Json,DBNull.Value);
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
public virtual string  Remove_OPD_Form(string id,string loginUserID)
			  {
				  String ResponseMessage="";
					try{
							using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
							{
								npsql.Open();
								using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"Remove_OPD_Form\"(@pvar_opdformid,@pvar_modifieduser)", npsql))
								{
										dbCommand.CommandType = CommandType.Text;
										dbCommand.Parameters.AddWithValue("pvar_opdformid",(object)id??DBNull.Value);
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
        public virtual JObject Added_OPD_Form(
    string tenantid,
    string patientname,
    string verifiedstatus,
    int? pagesize = 1000,
    int? pagenumber = 0,
    string searchterm = "",
    string sort_fields = "",
    string createddate_automatonfrom = "",
    string createddate_automatonto = "",
    string bookingnumber = "", string workflowstatus = "", string financialstatus = "", string paymentmethod = "")
        {
            object dalResponse = null;

            try
            {
                using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
                {
                    using (var dbCommand = new NpgsqlCommand(
                        "SELECT * FROM \"Added_OPD_Form\"(" +
                        "@pvar_tenantid," +
                        "@pvar_patientname," +
                        "@pvar_verifiedstatus," +
                        "@pvar_pagesize," +
                        "@pvar_pagenumber," +
                        "@pvar_searchterm," +
                        "@pvar_sort_fields," +
                        "@pvar_createddate_automatonfrom," +
                        "@pvar_createddate_automatonto," +
                        "@pvar_bookingnumber,@pvar_workflowstatus,@pvar_financialstatus,@pvar_paymentmethod" +
                        ")",
                        npsql))
                    {
                        dbCommand.CommandType = CommandType.Text;

                        dbCommand.Parameters.AddWithValue("pvar_tenantid", (object)tenantid ?? DBNull.Value);
                        dbCommand.Parameters.AddWithValue("pvar_patientname", (object)patientname ?? DBNull.Value);

                        dbCommand.Parameters.AddWithValue("pvar_verifiedstatus", (object)verifiedstatus ?? DBNull.Value);

                        dbCommand.Parameters.AddWithValue("pvar_pagesize", (object)pagesize ?? DBNull.Value);
                        dbCommand.Parameters.AddWithValue("pvar_pagenumber", (object)pagenumber ?? DBNull.Value);

                        dbCommand.Parameters.AddWithValue("pvar_searchterm", (object)searchterm ?? DBNull.Value);

                        if (sort_fields != null && sort_fields.Length > 2)
                            dbCommand.Parameters.AddWithValue("pvar_sort_fields", NpgsqlDbType.Json, sort_fields);
                        else
                            dbCommand.Parameters.AddWithValue("pvar_sort_fields", NpgsqlDbType.Json, DBNull.Value);

                        dbCommand.Parameters.AddWithValue("pvar_createddate_automatonfrom", (object)createddate_automatonfrom ?? DBNull.Value);
                        dbCommand.Parameters.AddWithValue("pvar_createddate_automatonto", (object)createddate_automatonto ?? DBNull.Value);
                        dbCommand.Parameters.AddWithValue("pvar_bookingnumber", (object)bookingnumber ?? DBNull.Value);
                        dbCommand.Parameters.AddWithValue("pvar_workflowstatus", (object)workflowstatus ?? DBNull.Value);
                        dbCommand.Parameters.AddWithValue("pvar_financialstatus", (object)financialstatus ?? DBNull.Value);
                        dbCommand.Parameters.AddWithValue("pvar_paymentmethod", (object)paymentmethod ?? DBNull.Value);

                        npsql.Open();
                        dalResponse = dbCommand.ExecuteScalar();
                        npsql.Close();
                    }
                }
            }
            catch
            {
                throw;
            }

            return JObject.Parse(dalResponse.ToString());
        }


        public virtual System.Data.DataTable get_all_OPDForm(string tenantid,string searchterm="", int? pagesize=1000, int? pagenumber=0)
			  {

				    DataTable dataTable = new DataTable();
					DataSet dataSet = new DataSet();

					try{

							using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
							{
								npsql.Open();
								using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"get_all_OPDForm\"(@pvar_tenantid)", npsql))
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
public virtual System.Data.DataTable count_of_OPDForm(string tenantid
,string patientname
)
		{
					DataTable dataTable = new DataTable();
                DataSet dataSet = new DataSet();

					try{

							using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
							{
								npsql.Open();
								using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"count_of_OPDForm\"(@pvar_tenantid,@pvar_patientname)", npsql))
								{
									dbCommand.CommandType = CommandType.Text;
									dbCommand.Parameters.AddWithValue("pvar_tenantid",(object)tenantid??DBNull.Value);
dbCommand.Parameters.AddWithValue("pvar_patientname",(object)patientname??DBNull.Value);

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


public virtual JObject OPD_Forms_for_Review(string tenantid
,string patientname
, string verifiedstatus
, int? pagesize=1000 , int? pagenumber=0,string searchterm="",string  sort_fields="")
			  {
				  object dalResponse = null;

					try{

							using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
							{

								using (var dbCommand = new NpgsqlCommand("SELECT * FROM\"OPD_Forms_for_Review\"(@pvar_tenantid,@pvar_patientname,@pvar_verifiedstatus,@pvar_pagesize,@pvar_pagenumber,@pvar_searchterm,@pvar_sort_fields)", npsql))
								{
									dbCommand.CommandType = CommandType.Text;
									dbCommand.Parameters.AddWithValue("pvar_tenantid",(object)tenantid??DBNull.Value);
dbCommand.Parameters.AddWithValue("pvar_patientname",(object)patientname??DBNull.Value);
						dbCommand.Parameters.AddWithValue("pvar_verifiedstatus",(object)verifiedstatus??DBNull.Value);

									dbCommand.Parameters.AddWithValue("pvar_pagesize",(object)pagesize??DBNull.Value);
									dbCommand.Parameters.AddWithValue("pvar_pagenumber",(object)pagenumber??DBNull.Value);

									dbCommand.Parameters.AddWithValue("pvar_searchterm",(object)searchterm??DBNull.Value);
									if (sort_fields != null && sort_fields.Length > 2)
									dbCommand.Parameters.AddWithValue("pvar_sort_fields", NpgsqlDbType.Json, sort_fields);
									else
									dbCommand.Parameters.AddWithValue("pvar_sort_fields", NpgsqlDbType.Json, DBNull.Value);


									npsql.Open();
									dalResponse = dbCommand.ExecuteScalar();
									npsql.Close();

								}

							}



					}catch{
						throw;
					}


					return JObject.Parse(dalResponse.ToString());




			   }


public virtual JObject Approved_OPD_Forms(string tenantid
,string patientname
,string preferreddate_automatonfrom
,string preferreddate_automatonto
,string preferreddoctor
, int? pagesize=1000 , int? pagenumber=0,string searchterm="",string  sort_fields="")
			  {
				  object dalResponse = null;

					try{

							using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
							{

								using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"Approved_OPD_Forms\"(@pvar_tenantid,@pvar_patientname,@pvar_preferreddate_automatonfrom,@pvar_preferreddate_automatonto,@pvar_preferreddoctor,@pvar_pagesize,@pvar_pagenumber,@pvar_searchterm,@pvar_sort_fields)", npsql))
								{
									dbCommand.CommandType = CommandType.Text;
									dbCommand.Parameters.AddWithValue("pvar_tenantid",NpgsqlDbType.Varchar,(object)tenantid??DBNull.Value);
dbCommand.Parameters.AddWithValue("pvar_patientname",NpgsqlDbType.Varchar,(object)patientname??DBNull.Value);
dbCommand.Parameters.AddWithValue("pvar_preferreddate_automatonfrom",NpgsqlDbType.Varchar,(object)preferreddate_automatonfrom??DBNull.Value);
dbCommand.Parameters.AddWithValue("pvar_preferreddate_automatonto",NpgsqlDbType.Varchar,(object)preferreddate_automatonto??DBNull.Value);
dbCommand.Parameters.AddWithValue("pvar_preferreddoctor",NpgsqlDbType.Varchar,(object)preferreddoctor??DBNull.Value);
						dbCommand.Parameters.AddWithValue("pvar_pagesize",(object)pagesize??DBNull.Value);
									dbCommand.Parameters.AddWithValue("pvar_pagenumber",(object)pagenumber??DBNull.Value);

									dbCommand.Parameters.AddWithValue("pvar_searchterm",NpgsqlDbType.Varchar,(object)searchterm??DBNull.Value);
									if (sort_fields != null && sort_fields.Length > 2)
									dbCommand.Parameters.AddWithValue("pvar_sort_fields", NpgsqlDbType.Json, sort_fields);
									else
									dbCommand.Parameters.AddWithValue("pvar_sort_fields", NpgsqlDbType.Json, DBNull.Value);


									npsql.Open();
									dalResponse = dbCommand.ExecuteScalar();
									npsql.Close();

								}

							}



					}catch{
						throw;
					}


					return JObject.Parse(dalResponse.ToString());




			   }


/// <summary>Returns the latest active ClinicalAppointment linked to an OPD form via bookingid.</summary>
		public virtual System.Data.DataTable Get_Appointment_By_OPDForm(string OPDFormid)
		{
			DataSet dataSet = new DataSet();
			DataTable dataTable = new DataTable();
			try
			{
				using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
				{
					npsql.Open();
					using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"Get_Appointment_By_OPDForm\"(@pvar_opdformid)", npsql))
					{
						dbCommand.CommandType = CommandType.Text;
						dbCommand.Parameters.AddWithValue("pvar_opdformid", (object)OPDFormid ?? DBNull.Value);
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
		/// <summary>
		/// Creates an 'OPD Consultation Fee' receivable when the OPD form is approved.
		/// Idempotent � safe to call multiple times for the same OPD.
		/// Returns "201.1" on success, or an error message if something fails.
		/// </summary>
		public virtual string Ensure_OPD_Consultation_Fee_Receivable(string OPDFormid, string createdby)
		{
			try
			{
				if (!Guid.TryParse(OPDFormid, out Guid opdId))  return "Invalid OPDFormid";
				if (!Guid.TryParse(createdby, out Guid userId)) return "Invalid createdby";

				Guid? tenantId = null;
				Guid? taskId   = null;
				using (var npsql = new NpgsqlConnection(db_connectionstring))
				{
					npsql.Open();
					using (var cmd = new NpgsqlCommand(
						"SELECT * FROM \"Get_OPD_Task_Tenant\"(@id)", npsql))
					{
						cmd.Parameters.AddWithValue("id", NpgsqlDbType.Uuid, opdId);
						using (var reader = cmd.ExecuteReader())
						{
							if (!reader.Read()) return "OPD not found";
							tenantId = reader.IsDBNull(0) ? (Guid?)null : reader.GetGuid(0);
							taskId   = reader.IsDBNull(1) ? (Guid?)null : reader.GetGuid(1);
						}
					}
				}

				var receivableDAL = new ReceivableDAL(db_connectionstring);
				receivableDAL.CreateOPDConsultationFeeReceivable(opdId, tenantId, userId, taskId);
				return "201.1";
			}
			catch (Exception ex) { return ex.Message; }
		}

		public virtual DataTable Get_OPD_Action_Eligibility(
			string OPDFormid,
			string userId,
			string userRole)
		{
			var dataTable = new DataTable();
			using (var connection = new NpgsqlConnection(db_connectionstring))
			using (var command = new NpgsqlCommand(
				"SELECT * FROM \"Get_OPD_Action_Eligibility\"(@pvar_opdformid,@pvar_userid,@pvar_userrole)",
				connection))
			using (var adapter = new NpgsqlDataAdapter(command))
			{
				command.Parameters.AddWithValue(
					"pvar_opdformid",
					NpgsqlDbType.Uuid,
					Guid.TryParse(OPDFormid, out var opdId) ? opdId : (object)DBNull.Value);
				command.Parameters.AddWithValue(
					"pvar_userid",
					NpgsqlDbType.Uuid,
					Guid.TryParse(userId, out var parsedUserId) ? parsedUserId : (object)DBNull.Value);
				command.Parameters.AddWithValue(
					"pvar_userrole",
					NpgsqlDbType.Varchar,
					(object)userRole ?? DBNull.Value);
				adapter.Fill(dataTable);
			}
			return dataTable;
		}

		public virtual DataTable Get_OPD_Cancellation_Quote(string OPDFormid)
		{
			var dataTable = new DataTable();
			using (var connection = new NpgsqlConnection(db_connectionstring))
			using (var command = new NpgsqlCommand(
				"SELECT * FROM \"Get_OPD_Cancellation_Quote\"(@pvar_opdformid)",
				connection))
			using (var adapter = new NpgsqlDataAdapter(command))
			{
				command.Parameters.AddWithValue(
					"pvar_opdformid",
					NpgsqlDbType.Uuid,
					Guid.TryParse(OPDFormid, out var opdId) ? opdId : (object)DBNull.Value);
				adapter.Fill(dataTable);
			}
			return dataTable;
		}

		public virtual string Add_OPD_Manual_Receivable(
			string OPDFormid,
			Guid? tenantId,
			Guid? patientId,
			string receivableFor,
			decimal amount,
			string remarks,
			string createdBy)
		{
			if (!Guid.TryParse(OPDFormid, out var opdId))
				return "Invalid OPDFormid.";
			if (!Guid.TryParse(createdBy, out var createdUserId))
				return "Invalid created user.";

			using (var connection = new NpgsqlConnection(db_connectionstring))
			using (var command = new NpgsqlCommand(
				"SELECT \"Add_OPD_Manual_Receivable\"(@pvar_opdformid,@pvar_tenantid,@pvar_patientname,@pvar_receivablefor,@pvar_amount,@pvar_remarks,@pvar_createduser)",
				connection))
			{
				command.Parameters.AddWithValue("pvar_opdformid", NpgsqlDbType.Uuid, opdId);
				command.Parameters.AddWithValue("pvar_tenantid", NpgsqlDbType.Uuid, (object)tenantId ?? DBNull.Value);
				command.Parameters.AddWithValue("pvar_patientname", NpgsqlDbType.Uuid, (object)patientId ?? DBNull.Value);
				command.Parameters.AddWithValue("pvar_receivablefor", NpgsqlDbType.Varchar, (object)receivableFor ?? DBNull.Value);
				command.Parameters.AddWithValue("pvar_amount", NpgsqlDbType.Numeric, amount);
				command.Parameters.AddWithValue("pvar_remarks", NpgsqlDbType.Varchar, (object)remarks ?? DBNull.Value);
				command.Parameters.AddWithValue("pvar_createduser", NpgsqlDbType.Uuid, createdUserId);
				connection.Open();
				return Convert.ToString(command.ExecuteScalar()) ?? "";
			}
		}

		public virtual System.Data.DataTable getById_allinfo_OPDForm(string OPDFormid)
			 {
				DataSet dataSet=new DataSet();
				DataTable dataTable = new DataTable();
				try{

						using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
						{
							npsql.Open();
							using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"getById_sp_all_OPDForm\"(@pvar_opdformid)", npsql))
							{
								dbCommand.CommandType = CommandType.Text;
								dbCommand.Parameters.AddWithValue("pvar_opdformid",(object)OPDFormid??DBNull.Value);
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

		public virtual System.Data.DataTable Get_Latest_Previous_OPD_Prefill_By_Patient(string tenantid, string patientname)
			 {
				DataSet dataSet=new DataSet();
				DataTable dataTable = new DataTable();
				try{

						using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
						{
							npsql.Open();
							using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"Get_Latest_Previous_OPD_Prefill_By_Patient\"(@pvar_tenantid,@pvar_patientname)", npsql))
							{
								dbCommand.CommandType = CommandType.Text;
								dbCommand.Parameters.AddWithValue("pvar_tenantid",(object)tenantid??DBNull.Value);
								dbCommand.Parameters.AddWithValue("pvar_patientname",(object)patientname??DBNull.Value);
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
			  public virtual string  verify_OPDForm(OPDFormReviewModel model)
			 {
				 String ResponseMessage="";
					try{


							using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
							{
								npsql.Open();
								using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"verify_OPDForm\"(@pvar_opdformid,@pvar_verifiedby,@pvar_verifiedstatus,@pvar_reviewcomments,@pvar_task)", npsql))
								{
										dbCommand.CommandType = CommandType.Text;
										 dbCommand.Parameters.AddWithValue("pvar_opdformid",(object)model.OPDFormid??DBNull.Value);
										dbCommand.Parameters.AddWithValue("pvar_verifiedby",(object)model.verifiedby??DBNull.Value);
										dbCommand.Parameters.AddWithValue("pvar_verifiedstatus",(object)model.verifiedstatus??DBNull.Value);
										dbCommand.Parameters.AddWithValue("pvar_reviewcomments",(object)model.reviewcomments??DBNull.Value);
										dbCommand.Parameters.AddWithValue("pvar_task",(object)model.task??DBNull.Value);

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
		/// <summary>
		/// Cancels an OPD form in a single transaction:
		///   1. Sets OPDForm.verifiedstatus = 'Cancelled' and inserts a review log.
		///   2. Sets ClinicalAppointment.status = 'Cancelled' for any linked appointment
		///      (matched by bookingid = OPDFormid) regardless of its current status.
		///   3. Marks unpaid Receivable rows for this OPD as 'Cancelled'.
		/// </summary>
		public virtual string Cancel_OPD_Direct(string opdFormId, string cancelledByUserId, string cancellationReason)
		{
			string responseMessage = "";
			try
			{
				using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
				{
					npsql.Open();
					using (var transaction = npsql.BeginTransaction())
					{
						try
						{
							// 1. Cancel the OPD form
							using (var cmd = new NpgsqlCommand("SELECT \"Cancel_OPD_Form\"(@pvar_opdformid::uuid,@pvar_reason,@pvar_userid::uuid)", npsql, transaction))
							{
								cmd.Parameters.AddWithValue("pvar_opdformid", NpgsqlTypes.NpgsqlDbType.Varchar, (object)opdFormId ?? DBNull.Value);
								cmd.Parameters.AddWithValue("pvar_userid",    NpgsqlTypes.NpgsqlDbType.Varchar, (object)cancelledByUserId ?? DBNull.Value);
								cmd.Parameters.AddWithValue("pvar_reason",    NpgsqlTypes.NpgsqlDbType.Varchar, (object)cancellationReason ?? DBNull.Value);
								cmd.ExecuteNonQuery();
							}

							// 2. Cancel linked ClinicalAppointment(s) � match by bookingid regardless of current status
							using (var cmd = new NpgsqlCommand("SELECT \"Cancel_OPD_Appointments\"(@pvar_opdformid,@pvar_userid::uuid)", npsql, transaction))
							{
								cmd.Parameters.AddWithValue("pvar_opdformid", NpgsqlTypes.NpgsqlDbType.Varchar, (object)opdFormId ?? DBNull.Value);
								cmd.Parameters.AddWithValue("pvar_userid",    NpgsqlTypes.NpgsqlDbType.Varchar, (object)cancelledByUserId ?? DBNull.Value);
								cmd.ExecuteNonQuery();
							}

							// 3. Cancel unpaid Receivable rows for this OPD form
							using (var cmd = new NpgsqlCommand("SELECT \"Cancel_OPD_Receivables\"(@pvar_opdformid::uuid,@pvar_userid::uuid)", npsql, transaction))
							{
								cmd.Parameters.AddWithValue("pvar_opdformid", NpgsqlTypes.NpgsqlDbType.Varchar, (object)opdFormId ?? DBNull.Value);
								cmd.Parameters.AddWithValue("pvar_userid",    NpgsqlTypes.NpgsqlDbType.Varchar, (object)cancelledByUserId ?? DBNull.Value);
								cmd.ExecuteNonQuery();
							}

							transaction.Commit();
							responseMessage = "201.1";
						}
						catch (Exception exTrans)
						{
							transaction.Rollback();
							responseMessage = exTrans.Message;
						}
					}
					npsql.Close();
				}
			}
			catch (Exception ex) { responseMessage = ex.Message; }
			return responseMessage;
		}

public virtual System.Data.DataTable lookup_OPDForm_patientname(String tenantid,string searchterm="", int? pagesize=1000, int? pagenumber=0)
							        {
                                            DataSet dataSet = new DataSet();
									        DataTable dataTable=new DataTable();
									        try{

                                        		        using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
					                                    {
						                                    npsql.Open();
						                                    using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"lookup_OPDForm_patientname\"(@pvar_tenantid,@pvar_searchterm,@pvar_pagesize,@pvar_pagenumber)", npsql))
						                                    {

                                                                dbCommand.Parameters.AddWithValue("pvar_tenantid",(object)tenantid??DBNull.Value);
																dbCommand.Parameters.AddWithValue("pvar_searchterm", (object)searchterm?? DBNull.Value);
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



									        }catch{
										        throw;
									        }
									        return dataTable;
							        }
public virtual System.Data.DataTable lookup_OPDForm_preferreddoctor(String tenantid,string searchterm="", int? pagesize=1000, int? pagenumber=0)
							        {
                                            DataSet dataSet = new DataSet();
									        DataTable dataTable=new DataTable();
									        try{

                                        		        using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
					                                    {
						                                    npsql.Open();
						                                    using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"lookup_OPDForm_preferreddoctor\"(@pvar_tenantid,@pvar_searchterm,@pvar_pagesize,@pvar_pagenumber)", npsql))
						                                    {

                                                                dbCommand.Parameters.AddWithValue("pvar_tenantid",(object)tenantid??DBNull.Value);
																dbCommand.Parameters.AddWithValue("pvar_searchterm", (object)searchterm?? DBNull.Value);
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



									        }catch{
										        throw;
									        }
									        return dataTable;
							        }
		public virtual System.Data.DataTable lookup_OPDForm_task(String tenantid)
		{
			DataSet dataSet = new DataSet();
			DataTable dataTable = new DataTable();
			try
			{

				using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
				{
					npsql.Open();
					using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"lookup_OPDForm_task\"(@pvar_tenantid)", npsql))
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

		public virtual string Get_OPD_Task_Eligibility(string patientname, string excludeopdformid = "")
		{
			if (!Guid.TryParse(patientname, out var patientId)) return "OP New";
			Guid? excludedOpdFormId = Guid.TryParse(excludeopdformid, out var parsedExcludedOpdFormId)
				? parsedExcludedOpdFormId
				: (Guid?)null;
			using (var connection = new NpgsqlConnection(db_connectionstring))
			using (var command = new NpgsqlCommand("SELECT * FROM \"Get_OPD_Task_Eligibility\"(@pvar_patientname, NOW()::timestamp, @pvar_excludeopdformid)", connection))
			{
				command.Parameters.AddWithValue("pvar_patientname", NpgsqlDbType.Uuid, patientId);
				command.Parameters.AddWithValue(
					"pvar_excludeopdformid",
					NpgsqlDbType.Uuid,
					excludedOpdFormId.HasValue ? (object)excludedOpdFormId.Value : DBNull.Value);
				connection.Open();
				return command.ExecuteScalar()?.ToString() ?? "OP New";
			}
		}

		public virtual string Validate_OPD_Task_Eligibility(string patientname, string task, string excludeopdformid = "")
		{
			if (!Guid.TryParse(patientname, out var patientId)) return "Please choose a valid patient.";
			if (!Guid.TryParse(task, out var taskId)) return "Please choose a valid OPD consultation task.";

			Guid? excludedOpdFormId = Guid.TryParse(excludeopdformid, out var parsedExcludedOpdFormId)
				? parsedExcludedOpdFormId
				: (Guid?)null;
			using (var connection = new NpgsqlConnection(db_connectionstring))
			using (var command = new NpgsqlCommand("SELECT * FROM \"Validate_OPD_Task_Eligibility\"(@pvar_patientname, @pvar_task, NOW()::timestamp, @pvar_excludeopdformid)", connection))
			{
				command.Parameters.AddWithValue("pvar_patientname", NpgsqlDbType.Uuid, patientId);
				command.Parameters.AddWithValue("pvar_task", NpgsqlDbType.Uuid, taskId);
				command.Parameters.AddWithValue(
					"pvar_excludeopdformid",
					NpgsqlDbType.Uuid,
					excludedOpdFormId.HasValue ? (object)excludedOpdFormId.Value : DBNull.Value);
				connection.Open();
				return command.ExecuteScalar()?.ToString() ?? "Unable to validate the OPD consultation task.";
			}
		}

		public virtual DataTable Get_OPD_Online_Doctor_Workflow_Context(string opdformid)
		{
			var table = new DataTable();
			if (!Guid.TryParse(opdformid, out var opdId)) return table;
			using (var connection = new NpgsqlConnection(db_connectionstring))
			using (var command = new NpgsqlCommand("SELECT * FROM \"Get_OPD_Online_Doctor_Workflow_Context\"(@pvar_opdformid)", connection))
			using (var adapter = new NpgsqlDataAdapter(command))
			{
				command.Parameters.AddWithValue("pvar_opdformid", NpgsqlDbType.Uuid, opdId);
				adapter.Fill(table);
			}
			return table;
		}

		public virtual System.Data.DataTable lookup_OPDForm_medicalinfo_medicalconditionname(string searchterm="", int? pagesize=1000, int? pagenumber=0)
							        {
                                            DataSet dataSet = new DataSet();
									        DataTable dataTable=new DataTable();
									        try{

                                        		        using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
					                                    {
						                                    npsql.Open();
						                                    using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"lookup_OPDForm_medicalinfo_medicalconditionname\"(@pvar_searchterm,@pvar_pagesize,@pvar_pagenumber)", npsql))
						                                    {


																dbCommand.Parameters.AddWithValue("pvar_searchterm", (object)searchterm?? DBNull.Value);
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



									        }catch{
										        throw;
									        }
									        return dataTable;
							        }
		/// <summary>Returns all time slots for a doctor on a given date, flagging each as booked or free.
		/// Shift hours come from StaffAttendance first; falls back to ShiftPlanning when no attendance record exists.</summary>
		public virtual System.Data.DataTable Get_Doctor_Available_Slots(string Peopleid, string appointmentdate, string tenantid = "",string taskid="", string taskname = "")
		{
			DataTable dataTable = new DataTable();
			DataSet dataSet = new DataSet();
			try
			{
				if ((!string.IsNullOrWhiteSpace(taskid) || !string.IsNullOrWhiteSpace(taskname)) && !string.IsNullOrWhiteSpace(appointmentdate))
				{
					var holidayDate = DateTime.Parse(appointmentdate).Date.ToString("yyyy-MM-dd");
					var holidayDAL = new HolidayCalendarDAL(db_connectionstring);
					var blockedDates = holidayDAL.Get_Holiday_Blocked_Dates(tenantid, taskid, taskname, holidayDate, holidayDate);
					if (blockedDates.Rows.Count > 0)
						return dataTable;
				}

				using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
				{
					npsql.Open();
					using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"Get_Doctor_Available_Slots_Latest\"(@pvar_peopleid,@pvar_appointmentdate,@pvar_tenantid,@pvar_taskid,@pvar_taskname)", npsql))
					{
						dbCommand.CommandType = CommandType.Text;
						dbCommand.Parameters.AddWithValue("pvar_peopleid", NpgsqlDbType.Uuid, string.IsNullOrWhiteSpace(Peopleid) || !Guid.TryParse(Peopleid, out var peopleGuid) ? (object)DBNull.Value : peopleGuid);
						dbCommand.Parameters.AddWithValue("pvar_appointmentdate", NpgsqlDbType.Timestamp, string.IsNullOrWhiteSpace(appointmentdate) ? (object)DBNull.Value : DateTime.Parse(appointmentdate));
						dbCommand.Parameters.AddWithValue("pvar_tenantid", NpgsqlDbType.Uuid, string.IsNullOrWhiteSpace(tenantid) || !Guid.TryParse(tenantid, out var tenantGuid) ? (object)DBNull.Value : tenantGuid);
						dbCommand.Parameters.AddWithValue("pvar_taskid", NpgsqlDbType.Uuid, string.IsNullOrWhiteSpace(taskid) || !Guid.TryParse(taskid, out var taskGuid) ? (object)DBNull.Value : taskGuid);
						dbCommand.Parameters.AddWithValue("pvar_taskname", NpgsqlDbType.Varchar, string.IsNullOrWhiteSpace(taskname) ? (object)DBNull.Value : taskname);
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

		public virtual System.Data.DataTable Get_Doctor_Available_Slots_By_Preferences(string Peopleid, string appointmentdate, string slotpreferences, string tenantid = "", string taskid = "", string taskname = "")
		{
			DataTable dataTable = new DataTable();
			DataSet dataSet = new DataSet();
			try
			{
				if ((!string.IsNullOrWhiteSpace(taskid) || !string.IsNullOrWhiteSpace(taskname)) && !string.IsNullOrWhiteSpace(appointmentdate))
				{
					var holidayDate = DateTime.Parse(appointmentdate).Date.ToString("yyyy-MM-dd");
					var holidayDAL = new HolidayCalendarDAL(db_connectionstring);
					var blockedDates = holidayDAL.Get_Holiday_Blocked_Dates(tenantid, taskid, taskname, holidayDate, holidayDate);
					if (blockedDates.Rows.Count > 0)
						return dataTable;
				}

				using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
				{
					npsql.Open();
					using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"Get_Doctor_Available_Slots_By_Preferences\"(@pvar_peopleid,@pvar_appointmentdate,@pvar_slotpreferences,@pvar_tenantid,@pvar_taskid,@pvar_taskname)", npsql))
					{
						dbCommand.CommandType = CommandType.Text;
						dbCommand.Parameters.AddWithValue("pvar_peopleid", NpgsqlDbType.Uuid, string.IsNullOrWhiteSpace(Peopleid) || !Guid.TryParse(Peopleid, out var peopleGuid) ? (object)DBNull.Value : peopleGuid);
						dbCommand.Parameters.AddWithValue("pvar_appointmentdate", NpgsqlDbType.Timestamp, string.IsNullOrWhiteSpace(appointmentdate) ? (object)DBNull.Value : DateTime.Parse(appointmentdate));
						dbCommand.Parameters.AddWithValue("pvar_slotpreferences", NpgsqlDbType.Varchar, string.IsNullOrWhiteSpace(slotpreferences) ? (object)DBNull.Value : slotpreferences);
						dbCommand.Parameters.AddWithValue("pvar_tenantid", NpgsqlDbType.Uuid, string.IsNullOrWhiteSpace(tenantid) || !Guid.TryParse(tenantid, out var tenantGuid) ? (object)DBNull.Value : tenantGuid);
						dbCommand.Parameters.AddWithValue("pvar_taskid", NpgsqlDbType.Uuid, string.IsNullOrWhiteSpace(taskid) || !Guid.TryParse(taskid, out var taskGuid) ? (object)DBNull.Value : taskGuid);
						dbCommand.Parameters.AddWithValue("pvar_taskname", NpgsqlDbType.Varchar, string.IsNullOrWhiteSpace(taskname) ? (object)DBNull.Value : taskname);
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
			catch
			{
				dataTable = FilterSlotsByPreferences(
					Get_Doctor_Available_Slots(Peopleid, appointmentdate, tenantid, taskid, taskname),
					slotpreferences);
			}
			return dataTable;
		}

		private System.Data.DataTable FilterSlotsByPreferences(System.Data.DataTable source, string slotpreferences)
		{
			if (source == null)
				return new DataTable();

			var preferences = (slotpreferences ?? "")
				.Split(',', StringSplitOptions.RemoveEmptyEntries)
				.Select(p => p.Trim().ToLowerInvariant())
				.Where(p => !string.IsNullOrWhiteSpace(p))
				.Distinct()
				.ToList();

			if (preferences.Count == 0 || preferences.Contains("any slot"))
				return source;

			var filtered = source.Clone();
			var slotRows = source.AsEnumerable()
				.Select(row => new
				{
					Row = row,
					SlotFrom = TimeSpan.TryParse(Convert.ToString(row["slotfrom"]), out var slotFrom) ? slotFrom : TimeSpan.Zero
				})
				.ToList();

			if (slotRows.Count == 0)
				return filtered;

			var lastSlot = slotRows.Max(x => x.SlotFrom);
			foreach (var item in slotRows)
			{
				var include =
					(preferences.Contains("early morning") && item.SlotFrom < new TimeSpan(10, 0, 0))
					|| (preferences.Contains("closer to lunch") && Math.Abs((item.SlotFrom - new TimeSpan(12, 0, 0)).TotalMinutes) <= 120)
					|| (preferences.Contains("closer to day") && item.SlotFrom >= lastSlot.Subtract(new TimeSpan(2, 0, 0)));

				if (include)
					filtered.ImportRow(item.Row);
			}

			var sortedRows = filtered.AsEnumerable()
				.OrderBy(row => TimeSpan.TryParse(Convert.ToString(row["slotfrom"]), out var slotFrom) ? slotFrom : TimeSpan.Zero)
				.ToList();

			filtered.Rows.Clear();
			foreach (var row in sortedRows)
				filtered.ImportRow(row);

			return filtered;
		}

		/// <summary>Updates only the status field of a ClinicalAppointment (Confirmed / In-Progress / Completed / Cancelled).</summary>
		public virtual string Update_Appointment_Status(string ClinicalAppointmentid, string status, string modifieduser)
		{
			string ResponseMessage = "";
			try
			{
				using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
				{
					npsql.Open();
					using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"Update_Appointment_Status\"(@pvar_clinicalappointmentid,@pvar_status,@pvar_modifieduser)", npsql))
					{
						dbCommand.CommandType = CommandType.Text;
						dbCommand.Parameters.AddWithValue("pvar_clinicalappointmentid", NpgsqlDbType.Uuid,string.IsNullOrEmpty(ClinicalAppointmentid)? (object)DBNull.Value: Guid.Parse(ClinicalAppointmentid));
						dbCommand.Parameters.AddWithValue("pvar_status", NpgsqlDbType.Varchar, (object)status ?? DBNull.Value);
						dbCommand.Parameters.AddWithValue("pvar_modifieduser", NpgsqlDbType.Uuid,string.IsNullOrEmpty(modifieduser) ? (object)DBNull.Value : Guid.Parse(modifieduser));
						NpgsqlParameter outParm = new NpgsqlParameter("pvar_returnMessage", NpgsqlDbType.Varchar)
						{
							Direction = ParameterDirection.Output
						};
						dbCommand.Parameters.Add(outParm);
						dbCommand.ExecuteNonQuery();
						ResponseMessage = outParm.Value.ToString();
						if (ResponseMessage == "201.1" && ShouldSyncPatientVisitStatus(status))
						{
							SyncOPDPatientVisitStatus(npsql, ClinicalAppointmentid, NormalizePatientVisitStatus(status), modifieduser);
						}
						if (dbCommand.Connection.State != ConnectionState.Closed)
							dbCommand.Connection.Dispose();
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

		private static bool ShouldSyncPatientVisitStatus(string status)
		{
			return string.Equals(status, "Completed", StringComparison.OrdinalIgnoreCase)
				|| string.Equals(status, "Cancelled", StringComparison.OrdinalIgnoreCase)
				|| string.Equals(status, "In Progress", StringComparison.OrdinalIgnoreCase)
				|| string.Equals(status, "In-Progress", StringComparison.OrdinalIgnoreCase);
		}

		private static string NormalizePatientVisitStatus(string status)
		{
			if (string.Equals(status, "In Progress", StringComparison.OrdinalIgnoreCase))
				return "In-Progress";

			return status;
		}

		private static void SyncOPDPatientVisitStatus(NpgsqlConnection npsql, string clinicalAppointmentId, string visitStatus, string modifieduser)
		{
			using (var syncCommand = new NpgsqlCommand("SELECT * FROM \"SyncOPDPatientVisitStatus\"(@pvar_clinicalappointmentid,@pvar_visitstatus,@pvar_modifieduser)", npsql))
			{
				syncCommand.CommandType = CommandType.Text;
				syncCommand.Parameters.AddWithValue("pvar_clinicalappointmentid", NpgsqlDbType.Uuid, string.IsNullOrEmpty(clinicalAppointmentId) ? (object)DBNull.Value : Guid.Parse(clinicalAppointmentId));
				syncCommand.Parameters.AddWithValue("pvar_visitstatus", NpgsqlDbType.Varchar, (object)visitStatus ?? DBNull.Value);
				syncCommand.Parameters.AddWithValue("pvar_modifieduser", NpgsqlDbType.Uuid, string.IsNullOrEmpty(modifieduser) ? (object)DBNull.Value : Guid.Parse(modifieduser));
				syncCommand.ExecuteScalar();
			}
		}

		/// <summary>Clinical appointments for this IPD form with task type containing IP Screening (bookingid = form id or booking reference).</summary>
		public virtual System.Data.DataTable get_ClinicalAppointmentids_IP_Screening_for_IPD(string ipdapplicationformid)
		{
			DataTable dataTable = new DataTable();
			DataSet dataSet = new DataSet();
			try
			{
				using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
				{
					npsql.Open();
					using (var dbCommand = new NpgsqlCommand(
						"SELECT * FROM \"get_ClinicalAppointmentids_IP_Screening_for_IPD\"(@pvar_ipdapplicationformid)", npsql))
					{
						dbCommand.CommandType = CommandType.Text;
						dbCommand.Parameters.AddWithValue("pvar_ipdapplicationformid", NpgsqlDbType.Uuid,
							string.IsNullOrWhiteSpace(ipdapplicationformid) ? (object)DBNull.Value : Guid.Parse(ipdapplicationformid));
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

		/// <summary>Sets status to Completed for IP Screening clinical appointments linked to the IPD form.</summary>
		public virtual string Complete_IP_Screening_Appointments_For_IPD(string ipdapplicationformid, string modifieduser)
		{
			string ResponseMessage = "201.1";
			try
			{
				var dt = get_ClinicalAppointmentids_IP_Screening_for_IPD(ipdapplicationformid);
				if (dt == null || dt.Rows.Count == 0)
					return "201.1";

				foreach (DataRow row in dt.Rows)
				{
					var id = row[0]?.ToString();
					if (string.IsNullOrWhiteSpace(id))
						continue;
					var msg = Update_Appointment_Status(id, "Completed", modifieduser);
					if (msg == null || !msg.Replace("\"", "").Contains("201.1"))
						return msg ?? "Failed to update clinical appointment status.";
				}
			}
			catch (Exception ex)
			{
				ResponseMessage = ex.Message;
			}
			return ResponseMessage;
		}

		/// <summary>Sets status to Completed for IP New clinical appointments linked to the IPD form.</summary>
		public virtual string Complete_IP_New_Appointments_For_IPD(string ipdapplicationformid, string modifieduser)
		{
			string ResponseMessage = "201.1";
			try
			{
				DataTable dt = new DataTable();
				using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
				{
					npsql.Open();
					using (var cmd = new NpgsqlCommand(
						"SELECT * FROM \"Get_IP_New_Appointments_For_IPD\"(@bkid)",
						npsql))
					{
						cmd.Parameters.AddWithValue("bkid", NpgsqlDbType.Varchar, ipdapplicationformid ?? "");
						using (var da = new NpgsqlDataAdapter(cmd))
						{
							DataSet ds = new DataSet();
							da.Fill(ds);
							if (ds.Tables.Count > 0)
								dt = ds.Tables[0];
						}
					}
					npsql.Close();
				}
				if (dt == null || dt.Rows.Count == 0)
					return "201.1";

				foreach (DataRow row in dt.Rows)
				{
					var id = row[0]?.ToString();
					if (string.IsNullOrWhiteSpace(id))
						continue;
					var msg = Update_Appointment_Status(id, "Completed", modifieduser);
					if (msg == null || !msg.Replace("\"", "").Contains("201.1"))
						return msg ?? "Failed to update IP New appointment status.";
				}
			}
			catch (Exception ex)
			{
				ResponseMessage = ex.Message;
			}
			return ResponseMessage;
		}

		/// <summary>Returns the auto-generated consultation fee receivable row(s) for an OPD form.</summary>
		public virtual System.Data.DataTable Get_OPD_Consultation_Fee(string OPDFormid)
		{
			DataTable dataTable = new DataTable();
			DataSet dataSet = new DataSet();
			try
			{
				using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
				{
					npsql.Open();
					using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"Get_OPD_Consultation_Fee\"(@pvar_opdformid)", npsql))
					{
						dbCommand.CommandType = CommandType.Text;
						dbCommand.Parameters.AddWithValue("pvar_opdformid", NpgsqlDbType.Uuid, string.IsNullOrEmpty(OPDFormid) ? (object)DBNull.Value : Guid.Parse(OPDFormid));
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

				if ((dataTable == null || dataTable.Rows.Count == 0) && !string.IsNullOrWhiteSpace(OPDFormid))
				{
					dataTable = Get_OPD_Consultation_Fee_From_Receivable(OPDFormid);
				}
			}
			catch { throw; }
			return dataTable;
		}

		private System.Data.DataTable Get_OPD_Consultation_Fee_From_Receivable(string OPDFormid)
		{
			DataSet dataSet = new DataSet();
			DataTable dataTable = new DataTable();
			using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
			{
				npsql.Open();
				using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"Get_OPD_Consultation_Fee_From_Receivable\"(@pvar_opdformid)", npsql))
				{
					dbCommand.CommandType = CommandType.Text;
					dbCommand.Parameters.AddWithValue("pvar_opdformid", NpgsqlDbType.Uuid, Guid.Parse(OPDFormid));
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
			return dataTable;
		}

		/// <summary>Returns patient, booking, tenant and fee-schedule details for an OPD form (used by billing/Razorpay flow).</summary>
		public virtual System.Data.DataTable Get_OPD_Consultation_Fee_By_OPDForm(string OPDFormid)
		{
			DataTable dataTable = new DataTable();
			DataSet dataSet = new DataSet();
			try
			{
				using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
				{
					npsql.Open();
					using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"Get_OPD_Consultation_Fee_By_OPDForm\"(@pvar_opdformid)", npsql))
					{
						dbCommand.CommandType = CommandType.Text;
						dbCommand.Parameters.AddWithValue("pvar_opdformid", NpgsqlDbType.Uuid, string.IsNullOrEmpty(OPDFormid) ? (object)DBNull.Value : Guid.Parse(OPDFormid));
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

		/// <summary>Returns the consultation fee(s) configured for a doctor, optionally filtered by task type name.</summary>
		public virtual System.Data.DataTable Get_Doctor_Consultation_Fee(string Peopleid, string tasktype)
		{
			DataTable dataTable = new DataTable();
			DataSet dataSet = new DataSet();
			try
			{
				using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
				{
					npsql.Open();
					using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"Get_Doctor_Consultation_Fee\"(@pvar_peopleid,@pvar_tasktype)", npsql))
					{
						dbCommand.CommandType = CommandType.Text;
						dbCommand.Parameters.AddWithValue("pvar_peopleid", NpgsqlDbType.Uuid, (object)Peopleid ?? DBNull.Value);
						dbCommand.Parameters.AddWithValue("pvar_tasktype", NpgsqlDbType.Varchar, (object)tasktype ?? DBNull.Value);
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

		/// <summary>Returns the IPD consultation fee(s) configured for a doctor (IP New / IP Follow up task types), optionally filtered by task type name.</summary>
		public virtual System.Data.DataTable Get_IPD_Doctor_Consultation_Fee(string Peopleid, string tasktype)
		{
			DataTable dataTable = new DataTable();
			DataSet dataSet = new DataSet();
			try
			{
				using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
				{
					npsql.Open();
					using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"Get_IPD_Doctor_Consultation_Fee\"(@pvar_peopleid,@pvar_tasktype)", npsql))
					{
						dbCommand.CommandType = CommandType.Text;
						dbCommand.Parameters.AddWithValue("pvar_peopleid",NpgsqlDbType.Uuid,string.IsNullOrEmpty(Peopleid) ? (object)DBNull.Value : Guid.Parse(Peopleid)
);
						dbCommand.Parameters.AddWithValue("pvar_tasktype", NpgsqlDbType.Varchar, (object)tasktype ?? DBNull.Value);
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






	}


			    }
