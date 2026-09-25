namespace NalamVazha.DAL{
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
				using System.Collections.Generic;
				using System.Linq;

			    //This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:42:02
			    public class IPDApplicationFormDAL
			    {
					public virtual string db_connectionstring{get;set;}
					
				    public IPDApplicationFormDAL(string connectionString)
				    {
						
					    db_connectionstring=connectionString;
				    }

                    private static void ApplyDirectPlannedAdmissionDate(IPDApplicationFormModel model)
                    {
                        if (model?.plannedadmissionstartdate == null
                            || !string.Equals(model.verifiedstatus, "Direct Admission", StringComparison.OrdinalIgnoreCase)
                            || model.preferreddatesofadmission == null
                            || model.preferreddatesofadmission.Count == 0)
                            return;

                        model.preferreddatesofadmission
                            .OrderBy(x => x.record_order)
                            .First().dateofarrival = model.plannedadmissionstartdate.Value.Date;
                    }
				  
			        public virtual System.Data.DataTable get_Active_IPD_Application(string patientprofileid)
			 {
					DataTable dataTable = new DataTable();
					DataSet dataSet = new DataSet();
					try
					{
						using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
						{
							npsql.Open();
							using (var dbCommand = new NpgsqlCommand("SELECT * FROM get_Active_IPD_Application(@p_patientprofileid)", npsql))
							{
								dbCommand.CommandType = CommandType.Text;
								dbCommand.Parameters.AddWithValue("p_patientprofileid", NpgsqlDbType.Uuid, Guid.TryParse(patientprofileid, out var parsed) ? (object)parsed : DBNull.Value);
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
					catch { throw; }
					return dataTable;
			 }

			        public virtual System.Data.DataTable getById_preferreddatesofadmission(string IPDApplicationFormid)
			 {
					DataTable dataTable=new DataTable();
					DataSet dataSet=new DataSet();
					try
					{
						  
							using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
							{
								npsql.Open();
								using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"getById_sp_IPDApplicationForm_preferreddatesofadmission\"(@pvar_ipdapplicationformid)", npsql))
								{
									dbCommand.CommandType = CommandType.Text;
									dbCommand.Parameters.AddWithValue("pvar_ipdapplicationformid",(object)IPDApplicationFormid??DBNull.Value);
								
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
			
		public virtual string Update_IPD_BookingStatus(string IPDApplicationFormid, string bookingstatus, string modifieduser)
		{
			String ResponseMessage = "";
			try
			{
				using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
				{
					npsql.Open();
					using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"Update_IPD_BookingStatus\"(@pvar_ipdapplicationformid,@pvar_bookingstatus,@pvar_modifieduser)", npsql))
					{
						dbCommand.CommandType = CommandType.Text;
						dbCommand.Parameters.AddWithValue("pvar_ipdapplicationformid", NpgsqlDbType.Uuid, Guid.TryParse(IPDApplicationFormid, out var parsedId) ? (object)parsedId : DBNull.Value);
						dbCommand.Parameters.AddWithValue("pvar_bookingstatus", NpgsqlDbType.Varchar, (object)bookingstatus ?? DBNull.Value);
						dbCommand.Parameters.AddWithValue("pvar_modifieduser", NpgsqlDbType.Uuid, Guid.TryParse(modifieduser, out var parsedUser) ? (object)parsedUser : DBNull.Value);
						NpgsqlParameter outParm = new NpgsqlParameter("@returnMessage", NpgsqlDbType.Varchar) { Direction = ParameterDirection.Output };
						dbCommand.Parameters.Add(outParm);
						dbCommand.ExecuteNonQuery();
						ResponseMessage = outParm.Value.ToString();
						if (dbCommand.Connection.State != ConnectionState.Closed) dbCommand.Connection.Dispose();
					}
					npsql.Close();
				}
				if ((ResponseMessage ?? "").Replace("\"", "") == "201.1"
					&& string.Equals(bookingstatus, "Discharged", StringComparison.OrdinalIgnoreCase))
				{
					ResponseMessage = Update_IPD_DischargeDepartureDate(IPDApplicationFormid, modifieduser);
				}
			}
			catch (Exception ex) { ResponseMessage = ex.Message; }
			return ResponseMessage;
		}

		public virtual string Update_IPD_VerifiedStatus(string IPDApplicationFormid, string verifiedstatus, string modifieduser)
		{
			string responseMessage = "";
			try
			{
				using (var npsql = new NpgsqlConnection(db_connectionstring))
				{
					npsql.Open();
					using (var dbCommand = new NpgsqlCommand(
						"SELECT * FROM \"Update_IPD_VerifiedStatus\"(@pvar_ipdapplicationformid,@pvar_verifiedstatus,@pvar_modifieduser)",
						npsql))
					{
						dbCommand.CommandType = CommandType.Text;
						dbCommand.Parameters.AddWithValue("pvar_ipdapplicationformid", NpgsqlDbType.Uuid,
							Guid.TryParse(IPDApplicationFormid, out var parsedId) ? (object)parsedId : DBNull.Value);
						dbCommand.Parameters.AddWithValue("pvar_verifiedstatus", NpgsqlDbType.Varchar,
							(object)verifiedstatus ?? DBNull.Value);
						dbCommand.Parameters.AddWithValue("pvar_modifieduser", NpgsqlDbType.Uuid,
							Guid.TryParse(modifieduser, out var parsedUser) ? (object)parsedUser : DBNull.Value);
						var outParm = new NpgsqlParameter("@returnMessage", NpgsqlDbType.Varchar)
						{
							Direction = ParameterDirection.Output
						};
						dbCommand.Parameters.Add(outParm);
						dbCommand.ExecuteNonQuery();
						responseMessage = outParm.Value?.ToString() ?? "";
					}
				}
			}
			catch (Exception ex)
			{
				responseMessage = ex.Message;
			}
			return responseMessage;
		}

		public virtual void Save_IPD_Cancellation_Request_Remarks(
			string IPDApplicationFormid,
			string cancellationRequestRemarks,
			string modifiedUser)
		{
			using var connection = new NpgsqlConnection(db_connectionstring);
			connection.Open();
			using var command = new NpgsqlCommand(
				@"SELECT public.""Save_IPD_Cancellation_Request_Remarks""(
					@pvar_ipdapplicationformid,
					@pvar_remarks,
					@pvar_modifieduser)", connection);
			command.Parameters.AddWithValue("pvar_remarks", NpgsqlDbType.Varchar,
				(object)cancellationRequestRemarks ?? DBNull.Value);
			command.Parameters.AddWithValue("pvar_modifieduser", NpgsqlDbType.Uuid,
				Guid.TryParse(modifiedUser, out var userId) ? (object)userId : DBNull.Value);
			command.Parameters.AddWithValue("pvar_ipdapplicationformid", NpgsqlDbType.Uuid,
				Guid.Parse(IPDApplicationFormid));
			command.ExecuteScalar();
		}

		public virtual string Get_IPD_Cancellation_Request_Remarks(string IPDApplicationFormid)
		{
			using var connection = new NpgsqlConnection(db_connectionstring);
			connection.Open();
			using var command = new NpgsqlCommand(
				@"SELECT remarks
				  FROM public.""Get_IPD_Cancellation_Request_Remarks""(
					@pvar_ipdapplicationformid)", connection);
			command.Parameters.AddWithValue("pvar_ipdapplicationformid", NpgsqlDbType.Uuid,
				Guid.Parse(IPDApplicationFormid));
			return command.ExecuteScalar()?.ToString() ?? "";
		}

		private string Update_IPD_DischargeDepartureDate(string IPDApplicationFormid, string modifieduser)
		{
			string ResponseMessage = "";
			try
			{
				using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
				{
					npsql.Open();
					using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"Update_IPD_DischargeDepartureDate\"(@pvar_ipdapplicationformid,@pvar_modifieduser)", npsql))
					{
						dbCommand.CommandType = CommandType.Text;
						dbCommand.Parameters.AddWithValue("pvar_ipdapplicationformid", NpgsqlDbType.Uuid, Guid.TryParse(IPDApplicationFormid, out var parsedId) ? (object)parsedId : DBNull.Value);
						dbCommand.Parameters.AddWithValue("pvar_modifieduser", NpgsqlDbType.Uuid, Guid.TryParse(modifieduser, out var parsedUser) ? (object)parsedUser : DBNull.Value);
						ResponseMessage = dbCommand.ExecuteScalar()?.ToString() ?? "";
						if (dbCommand.Connection.State != ConnectionState.Closed) dbCommand.Connection.Dispose();
					}
					npsql.Close();
				}
			}
			catch (Exception ex) { ResponseMessage = ex.Message; }
			return ResponseMessage;
		}

		public virtual string Ensure_ApprovedForAdmission_Receivable(string IPDApplicationFormid, string modifieduser)
		{
			try
			{
				using (var npsql = new NpgsqlConnection(db_connectionstring))
				{
					npsql.Open();
					using (var tx = npsql.BeginTransaction())
					{
						try
						{
							Guid ipdId;
							Guid userId;
							if (!Guid.TryParse(IPDApplicationFormid, out ipdId))
								return "Invalid IPDApplicationFormid";
							if (!Guid.TryParse(modifieduser, out userId))
								return "Invalid modifieduser";

							Guid patientId = Guid.Empty;
							Guid? tenantId = null;
							using (var ipdCmd = new NpgsqlCommand("SELECT * FROM \"Get_IPD_Patient_Tenant\"(@pvar_ipdid)", npsql, tx))
							{
								ipdCmd.Parameters.AddWithValue("pvar_ipdid", NpgsqlDbType.Uuid, ipdId);
								using (var rd = ipdCmd.ExecuteReader())
								{
									if (!rd.Read()) return "IPD not found";
									patientId = rd.GetGuid(0);
									if (!rd.IsDBNull(1)) tenantId = rd.GetGuid(1);
								}
							}

							bool isMandatory = false;
							decimal conditionAmount = 0;
							using (var mandatoryCmd = new NpgsqlCommand("SELECT \"Get_IPD_Booking_Deposit_Mandatory\"()", npsql, tx))
							{
								var mandatoryObj = mandatoryCmd.ExecuteScalar();
								isMandatory = mandatoryObj != null && mandatoryObj != DBNull.Value && Convert.ToBoolean(mandatoryObj);
							}

							// Amount column may not exist in all schema versions. Try; if unavailable, fallback below.
							try
							{
								using (var amountCmd = new NpgsqlCommand("SELECT \"Get_IPD_Booking_Deposit_Amount\"()", npsql, tx))
								{
									var amountObj = amountCmd.ExecuteScalar();
									conditionAmount = amountObj == null || amountObj == DBNull.Value ? 0 : Convert.ToDecimal(amountObj);
								}
							}
							catch
							{
								conditionAmount = 0;
							}

							if (!isMandatory) { tx.Commit(); return "201.1"; }

							if (conditionAmount <= 0)
							{
								using (var amountCmd = new NpgsqlCommand("SELECT \"Get_IPD_Advance_Amount\"(@pvar_ipdid::text)", npsql, tx))
								{
									amountCmd.Parameters.AddWithValue("pvar_ipdid", NpgsqlDbType.Uuid, ipdId);
									var amtObj = amountCmd.ExecuteScalar();
									conditionAmount = amtObj == null || amtObj == DBNull.Value ? 0 : Convert.ToDecimal(amtObj);
								}
							}
							if (conditionAmount <= 0) { tx.Commit(); return "201.1"; }

							const string marker = "Auto:ApprovedForAdmission:IPDBookingDeposit";
							using (var existsCmd = new NpgsqlCommand("SELECT \"Get_IPD_Booking_Deposit_Receivable_Count\"(@pvar_ipdid,@pvar_marker)", npsql, tx))
							{
								existsCmd.Parameters.AddWithValue("pvar_ipdid", NpgsqlDbType.Uuid, ipdId);
								existsCmd.Parameters.AddWithValue("pvar_marker", NpgsqlDbType.Varchar, marker);
								var cnt = Convert.ToInt32(existsCmd.ExecuteScalar() ?? 0);
								if (cnt > 0) { tx.Commit(); return "201.1"; }
							}

							int maxSeq = 0;
							string dayPrefix = DateTime.Now.ToString("yyyyMMdd");
							using (var seqCmd = new NpgsqlCommand("SELECT \"Get_Receivable_Day_Max_Sequence\"(@pvar_dayprefix)", npsql, tx))
							{
								seqCmd.Parameters.AddWithValue("pvar_dayprefix", NpgsqlDbType.Varchar, dayPrefix);
								maxSeq = Convert.ToInt32(seqCmd.ExecuteScalar() ?? 0);
							}

							var newNo = dayPrefix + "-" + (maxSeq + 1).ToString("00000");
							using (var insCmd = new NpgsqlCommand("SELECT \"Insert_IPD_Booking_Deposit_Receivable\"(@pvar_receivableid,@pvar_tenantid,@pvar_receivableno,@pvar_patientid,@pvar_ipdid,@pvar_amount,@pvar_marker,@pvar_createduser)", npsql, tx))
							{
								insCmd.Parameters.AddWithValue("pvar_receivableid", NpgsqlDbType.Uuid, Guid.NewGuid());
								insCmd.Parameters.AddWithValue("pvar_tenantid", NpgsqlDbType.Uuid, (object)tenantId ?? DBNull.Value);
								insCmd.Parameters.AddWithValue("pvar_receivableno", NpgsqlDbType.Varchar, newNo);
								insCmd.Parameters.AddWithValue("pvar_patientid", NpgsqlDbType.Uuid, patientId);
								insCmd.Parameters.AddWithValue("pvar_ipdid", NpgsqlDbType.Uuid, ipdId);
								insCmd.Parameters.AddWithValue("pvar_amount", NpgsqlDbType.Numeric, conditionAmount);
								insCmd.Parameters.AddWithValue("pvar_marker", NpgsqlDbType.Varchar, marker);
								insCmd.Parameters.AddWithValue("pvar_createduser", NpgsqlDbType.Uuid, userId);
								insCmd.ExecuteNonQuery();
							}

							tx.Commit();
							return "201.1";
						}
						catch (Exception exTx)
						{
							tx.Rollback();
							return exTx.Message;
						}
					}
				}
			}
			catch (Exception ex)
			{
				return ex.Message;
			}
		}


		public virtual string Ensure_ScreeningFee_Receivable(string IPDApplicationFormid, string modifieduser)
		{
			try
			{
				if (!Guid.TryParse(IPDApplicationFormid, out Guid ipdId)) return "Invalid IPDApplicationFormid";
				if (!Guid.TryParse(modifieduser, out Guid userId)) return "Invalid modifieduser";

				Guid? tenantId = null;
				using (var npsql = new NpgsqlConnection(db_connectionstring))
				{
					npsql.Open();
					using (var cmd = new NpgsqlCommand(
						"SELECT * FROM \"Get_IPD_Tenant\"(@id)", npsql))
					{
						cmd.Parameters.AddWithValue("id", NpgsqlDbType.Uuid, ipdId);
						var obj = cmd.ExecuteScalar();
						if (obj == null || obj == DBNull.Value) return "IPD not found";
						tenantId = (Guid)obj;
					}
				}

				var receivableDAL = new ReceivableDAL(db_connectionstring);
				receivableDAL.CreateScreeningFeeReceivable(ipdId, tenantId, userId);
				return "201.1";
			}
			catch (Exception ex) { return ex.Message; }
		}

		public virtual string Ensure_IPDAppointmentFee_Receivable(string IPDApplicationFormid, string practitioner, string tasktype, string modifieduser)
		{
			try
			{
				if (!Guid.TryParse(IPDApplicationFormid, out Guid ipdId)) return "Invalid IPDApplicationFormid";
				if (!Guid.TryParse(practitioner, out Guid practitionerId)) return "Invalid practitioner";
				if (!Guid.TryParse(modifieduser, out Guid userId)) return "Invalid modifieduser";
				if (string.IsNullOrWhiteSpace(tasktype)) return "Invalid tasktype";

				Guid? tenantId = null;
				Guid patientId;
				using (var npsql = new NpgsqlConnection(db_connectionstring))
				{
					npsql.Open();
					using (var cmd = new NpgsqlCommand(
						"SELECT * FROM \"Get_IPD_Tenant_Patient\"(@id)", npsql))
					{
						cmd.Parameters.AddWithValue("id", NpgsqlDbType.Uuid, ipdId);
						using var reader = cmd.ExecuteReader();
						if (!reader.Read()) return "IPD not found";
						tenantId = reader["tenantid"] == DBNull.Value ? (Guid?)null : (Guid)reader["tenantid"];
						if (reader["patientname"] == DBNull.Value) return "Patient not found";
						patientId = (Guid)reader["patientname"];
					}
				}

				var receivableDAL = new ReceivableDAL(db_connectionstring);
				var created = receivableDAL.CreateIPDAppointmentFeeReceivable(ipdId, tenantId, patientId, practitionerId, tasktype, userId);
				if (created <= 0) return "No fee amount configured for " + tasktype;
				return "201.1";
			}
			catch (Exception ex) { return ex.Message; }
		}

		public virtual string Ensure_AdmissionFee_Receivable(string IPDApplicationFormid, string modifieduser)
		{
			try
			{
				if (!Guid.TryParse(IPDApplicationFormid, out Guid ipdId)) return "Invalid IPDApplicationFormid";
				if (!Guid.TryParse(modifieduser, out Guid userId)) return "Invalid modifieduser";

				Guid? tenantId = null;
				using (var npsql = new NpgsqlConnection(db_connectionstring))
				{
					npsql.Open();
					using (var cmd = new NpgsqlCommand(
						"SELECT * FROM \"Get_IPD_Tenant\"(@id)", npsql))
					{
						cmd.Parameters.AddWithValue("id", NpgsqlDbType.Uuid, ipdId);
						var obj = cmd.ExecuteScalar();
						if (obj == null || obj == DBNull.Value) return "IPD not found";
						tenantId = (Guid)obj;
					}
				}

				var receivableDAL = new ReceivableDAL(db_connectionstring);
				receivableDAL.CreateAdmissionFeeReceivable(ipdId, tenantId, userId);
				return "201.1";
			}
			catch (Exception ex) { return ex.Message; }
		}

		public virtual System.Data.DataTable Get_IPD_Payment_Details(string IPDApplicationFormid)
		{
			DataTable dataTable = new DataTable();
			DataSet dataSet = new DataSet();
			try
			{
				using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
				{
					npsql.Open();
					using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"Get_IPD_Payment_Details\"(@pvar_ipdapplicationformid)", npsql))
					{
						dbCommand.CommandType = CommandType.Text;
						dbCommand.Parameters.AddWithValue("pvar_ipdapplicationformid", (object)IPDApplicationFormid ?? DBNull.Value);
						using (NpgsqlDataAdapter dataAdapter = new NpgsqlDataAdapter(dbCommand))
						{
							dataSet.Reset();
							dataAdapter.Fill(dataSet);
							dataTable = dataSet.Tables[0];
							if (dbCommand.Connection.State != ConnectionState.Closed) dbCommand.Connection.Dispose();
						}
					}
					npsql.Close();
				}
			}
			catch { throw; }
			return dataTable;
		}

/// <summary>Returns refund breakdown rows per refundtype for the given IPD and cancellationby (Patient or Hospital).</summary>
		public virtual System.Data.DataTable Get_IPD_Refund_Details(string IPDApplicationFormid, string cancellationby)
		{
			DataTable dataTable = new DataTable();
			DataSet dataSet = new DataSet();
			try
			{
				using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
				{
					npsql.Open();
					using (var dbCommand = new NpgsqlCommand("SELECT * FROM get_cancel_ipd_refund_details(@pvar_ipdapplicationformid, @pvar_cancellationby)", npsql))
					{
						dbCommand.CommandType = CommandType.Text;
						dbCommand.Parameters.AddWithValue("pvar_ipdapplicationformid", NpgsqlDbType.Uuid, Guid.TryParse(IPDApplicationFormid, out var refundIpdId) ? (object)refundIpdId : DBNull.Value);
						dbCommand.Parameters.AddWithValue("pvar_cancellationby", (object)cancellationby ?? DBNull.Value);
						using (NpgsqlDataAdapter dataAdapter = new NpgsqlDataAdapter(dbCommand))
						{
							dataSet.Reset();
							dataAdapter.Fill(dataSet);
							dataTable = dataSet.Tables[0];
							if (dbCommand.Connection.State != ConnectionState.Closed) dbCommand.Connection.Dispose();
						}
					}
					npsql.Close();
				}
			}
			catch { throw; }
			return dataTable;
		}

		public virtual System.Data.DataTable Get_Cancel_IPD_Payment_History(string IPDApplicationFormid)
		{
			DataTable dataTable = new DataTable();
			DataSet dataSet = new DataSet();
			try
			{
				using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
				{
					npsql.Open();
					using (var dbCommand = new NpgsqlCommand("SELECT * FROM get_cancel_ipd_payment_history(@pvar_ipdapplicationformid)", npsql))
					{
						dbCommand.CommandType = CommandType.Text;
						dbCommand.Parameters.AddWithValue("pvar_ipdapplicationformid", NpgsqlDbType.Uuid, Guid.TryParse(IPDApplicationFormid, out var paymentHistoryIpdId) ? (object)paymentHistoryIpdId : DBNull.Value);
						using (NpgsqlDataAdapter dataAdapter = new NpgsqlDataAdapter(dbCommand))
						{
							dataSet.Reset();
							dataAdapter.Fill(dataSet);
							dataTable = dataSet.Tables[0];
							if (dbCommand.Connection.State != ConnectionState.Closed) dbCommand.Connection.Dispose();
						}
					}
					npsql.Close();
				}
			}
			catch { throw; }
			return dataTable;
		}

		/// <summary>Returns BillingPayment rows for a given IPD (patientvisit = IPDApplicationFormid).</summary>
		public virtual System.Data.DataTable Get_BillingPayment_ByIPD(string ipdid)
		{
			DataTable dataTable = new DataTable();
			DataSet dataSet = new DataSet();
			try
			{
				using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
				{
					npsql.Open();
					using (var dbCommand = new NpgsqlCommand(
						"SELECT * FROM \"Get_BillingPayment_ByIPD\"(@pvar_ipdid)", npsql))
					{
						dbCommand.CommandType = CommandType.Text;
						dbCommand.Parameters.AddWithValue("pvar_ipdid", (object)ipdid ?? DBNull.Value);
						using (NpgsqlDataAdapter dataAdapter = new NpgsqlDataAdapter(dbCommand))
						{
							dataSet.Reset();
							dataAdapter.Fill(dataSet);
							dataTable = dataSet.Tables[0];
							if (dbCommand.Connection.State != ConnectionState.Closed) dbCommand.Connection.Dispose();
						}
					}
					npsql.Close();
				}
			}
			catch { throw; }
			return dataTable;
		}

		public virtual string Confirm_Arrival(IPDApplicationFormModel model)
		{
			String ResponseMessage = "";
			try
			{

				using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
				{
					npsql.Open();
					using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"Confirm_Arrival\"(@pvar_ipdapplicationformid,@pvar_tenantid,@pvar_bookingreferencenumber,@pvar_verifiedstatus,@pvar_estimatedarrival,@pvar_travelarrangement,@pvar_typeoftravelrequired,@pvar_pickupfrom,@pvar_requiredparkingspace,@pvar_wheelchairassistance,@pvar_requireddinner,@pvar_specialrequest,@pvar_modifieduser)", npsql))
					{
						dbCommand.CommandType = CommandType.Text;
						dbCommand.Parameters.AddWithValue("pvar_ipdapplicationformid", NpgsqlDbType.Uuid, (object)model.IPDApplicationFormid ?? DBNull.Value);
						dbCommand.Parameters.AddWithValue("pvar_tenantid", NpgsqlDbType.Uuid, (object)model.tenantid ?? DBNull.Value);

						dbCommand.Parameters.AddWithValue("pvar_bookingreferencenumber", NpgsqlDbType.Varchar, (object)model.bookingreferencenumber ?? DBNull.Value);

						dbCommand.Parameters.AddWithValue("pvar_verifiedstatus", NpgsqlDbType.Varchar, (object)model.verifiedstatus ?? DBNull.Value);

						dbCommand.Parameters.AddWithValue("pvar_estimatedarrival", NpgsqlDbType.Timestamp, (object)model.estimatedarrival ?? DBNull.Value);

						dbCommand.Parameters.AddWithValue("pvar_travelarrangement", NpgsqlDbType.Varchar, (object)model.travelarrangement ?? DBNull.Value);

						dbCommand.Parameters.AddWithValue("pvar_typeoftravelrequired", NpgsqlDbType.Varchar, (object)model.typeoftravelrequired ?? DBNull.Value);

						dbCommand.Parameters.AddWithValue("pvar_pickupfrom", NpgsqlDbType.Varchar, (object)model.pickupfrom ?? DBNull.Value);

						dbCommand.Parameters.AddWithValue("pvar_requiredparkingspace", NpgsqlDbType.Varchar, (object)model.requiredparkingspace ?? DBNull.Value);

						dbCommand.Parameters.AddWithValue("pvar_wheelchairassistance", NpgsqlDbType.Varchar, (object)model.wheelchairassistance ?? DBNull.Value);

						dbCommand.Parameters.AddWithValue("pvar_requireddinner", NpgsqlDbType.Varchar, (object)model.requireddinner ?? DBNull.Value);

						dbCommand.Parameters.AddWithValue("pvar_specialrequest", NpgsqlDbType.Varchar, (object)model.specialrequest ?? DBNull.Value);
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

		public virtual System.Data.DataTable getById_medicalinfo(string IPDApplicationFormid)
			 {
					DataTable dataTable=new DataTable();
					DataSet dataSet=new DataSet();
					try
					{
						  
							using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
							{
								npsql.Open();
								using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"getById_sp_IPDApplicationForm_medicalinfo\"(@pvar_ipdapplicationformid)", npsql))
								{
									dbCommand.CommandType = CommandType.Text;
									dbCommand.Parameters.AddWithValue("pvar_ipdapplicationformid",(object)IPDApplicationFormid??DBNull.Value);
								
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

public virtual System.Data.DataTable getById_medicationinfo(string IPDApplicationFormid)
			 {
					DataTable dataTable=new DataTable();
					DataSet dataSet=new DataSet();
					try
					{
						  
							using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
							{
								npsql.Open();
								using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"getById_sp_IPDApplicationForm_medicationinfo\"(@pvar_ipdapplicationformid)", npsql))
								{
									dbCommand.CommandType = CommandType.Text;
									dbCommand.Parameters.AddWithValue("pvar_ipdapplicationformid",(object)IPDApplicationFormid??DBNull.Value);
								
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

public virtual System.Data.DataTable getById_medicalrecords(string IPDApplicationFormid)
			 {
					DataTable dataTable=new DataTable();
					DataSet dataSet=new DataSet();
					try
					{
						  
							using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
							{
								npsql.Open();
								using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"getById_sp_IPDApplicationForm_medicalrecords\"(@pvar_ipdapplicationformid)", npsql))
								{
									dbCommand.CommandType = CommandType.Text;
									dbCommand.Parameters.AddWithValue("pvar_ipdapplicationformid",(object)IPDApplicationFormid??DBNull.Value);
								
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

public virtual System.Data.DataTable getById_attendantinfo(string IPDApplicationFormid)
			 {
					DataTable dataTable=new DataTable();
					DataSet dataSet=new DataSet();
					try
					{
						  
							using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
							{
								npsql.Open();
								using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"getById_sp_IPDApplicationForm_attendantinfo\"(@pvar_ipdapplicationformid)", npsql))
								{
									dbCommand.CommandType = CommandType.Text;
									dbCommand.Parameters.AddWithValue("pvar_ipdapplicationformid",(object)IPDApplicationFormid??DBNull.Value);
								
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

public virtual System.Data.DataTable getById_roompreference(string IPDApplicationFormid)
			 {
					DataTable dataTable=new DataTable();
					DataSet dataSet=new DataSet();
					try
					{
						  
							using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
							{
								npsql.Open();
								using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"getById_sp_IPDApplicationForm_roompreference\"(@pvar_ipdapplicationformid)", npsql))
								{
									dbCommand.CommandType = CommandType.Text;
									dbCommand.Parameters.AddWithValue("pvar_ipdapplicationformid",(object)IPDApplicationFormid??DBNull.Value);
								
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

public virtual System.Data.DataTable getById_attendantpreferreddates(string IPDApplicationFormid)
{
    return GetPreferenceRows("getById_sp_IPDApplicationForm_attendantpreferreddates", IPDApplicationFormid);
}

public virtual System.Data.DataTable getById_attendantroompreference(string IPDApplicationFormid)
{
    return GetPreferenceRows("getById_sp_IPDApplicationForm_attendantroompreference", IPDApplicationFormid);
}

private DataTable GetPreferenceRows(string functionName, string IPDApplicationFormid)
{
    var dataTable = new DataTable();
    using (var npsql = new NpgsqlConnection(db_connectionstring))
    {
        npsql.Open();
        using (var dbCommand = new NpgsqlCommand($"SELECT * FROM \"{functionName}\"(@pvar_ipdapplicationformid)", npsql))
        {
            dbCommand.Parameters.AddWithValue("pvar_ipdapplicationformid", (object)IPDApplicationFormid ?? DBNull.Value);
            using (var dataAdapter = new NpgsqlDataAdapter(dbCommand)) dataAdapter.Fill(dataTable);
        }
    }
    return dataTable;
}

public virtual System.Data.DataTable getById_room(string IPDApplicationFormid)
			 {
					DataTable dataTable=new DataTable();
					DataSet dataSet=new DataSet();
					try
					{
						  
							using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
							{
								npsql.Open();
								using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"getById_sp_IPDApplicationForm_room\"(@pvar_ipdapplicationformid)", npsql))
								{
									dbCommand.CommandType = CommandType.Text;
									dbCommand.Parameters.AddWithValue("pvar_ipdapplicationformid",(object)IPDApplicationFormid??DBNull.Value);
								
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


              public virtual string Add_IPD_Application_Form(IPDApplicationFormModel model)
			  { 
				  ApplyDirectPlannedAdmissionDate(model);
				  String ResponseMessage="";
					 
					try{
							 
                            using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
					        {
						        npsql.Open();
						        using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"Add_IPD_Application_Form\"(@pvar_ipdapplicationformid,@pvar_tenantid,@pvar_bookingreferencenumber,@pvar_patientname,@pvar_firstname,@pvar_lastname,@pvar_gender,@pvar_mobilenumber,@pvar_whatsappnumber,@pvar_nationality,@pvar_countryoforigin,@pvar_generalcondition,@pvar_bookingstatus,@pvar_groupbooking,@pvar_areyouthegroupleader,@pvar_numberofmember,@pvar_groupleadersbookingreferencenumber,@pvar_paddressline1,@pvar_paddressline2,@pvar_ppincode,@pvar_ptown,@pvar_pcityordistrict,@pvar_pstatename,@pvar_sameaspermanentaddress,@pvar_caddressline1,@pvar_caddressline2,@pvar_cpincode,@pvar_ctown,@pvar_ccityordistrict,@pvar_cstatename,@pvar_flexiblewithdates,@pvar_flexiblewithroomtype,@pvar_joinwaitinglist,@pvar_passportnumber,@pvar_passportissuingcountry,@pvar_passportexpirydate,@pvar_uploadpassportcopy,@pvar_visatype,@pvar_visanumber,@pvar_visaissuedcountry,@pvar_visaissuedate,@pvar_visaexpirydate,@pvar_uploadvisacopy,@pvar_doyourequireahospitalprovidedattendant,@pvar_preferredduration,@pvar_admissionreason,@pvar_consentform,@pvar_consentfile,@pvar_agreefortermsandconditions,@pvar_signature,@pvar_verifiedstatus,@pvar_preferreddatesofadmission,@pvar_medicalinfo,@pvar_medicationinfo,@pvar_medicalrecords,@pvar_attendantinfo,@pvar_roompreference,@pvar_attendantpreferreddates,@pvar_attendantroompreference,@pvar_bookingtype,@pvar_groupcode,@pvar_createduser )", npsql))
						        {
                                        dbCommand.CommandType = CommandType.Text;
						            	
								        					dbCommand.Parameters.AddWithValue("pvar_ipdapplicationformid",NpgsqlDbType.Uuid,(object)model.IPDApplicationFormid??DBNull.Value);
dbCommand.Parameters.AddWithValue("pvar_tenantid",NpgsqlDbType.Uuid,(object)model.tenantid??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_bookingreferencenumber",NpgsqlDbType.Varchar,(object)model.bookingreferencenumber??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_patientname",NpgsqlDbType.Uuid,(object)model.patientname??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_firstname",NpgsqlDbType.Varchar,(object)model.firstname??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_lastname",NpgsqlDbType.Varchar,(object)model.lastname??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_gender",NpgsqlDbType.Varchar,(object)model.gender??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_mobilenumber",NpgsqlDbType.Varchar,(object)model.mobilenumber??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_whatsappnumber",NpgsqlDbType.Varchar,(object)model.whatsappnumber??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_nationality",NpgsqlDbType.Varchar,(object)model.nationality??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_countryoforigin",NpgsqlDbType.Uuid,(object)model.countryoforigin??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_generalcondition",NpgsqlDbType.Varchar,(object)model.generalcondition??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_bookingstatus",NpgsqlDbType.Varchar,(object)model.bookingstatus??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_groupbooking",NpgsqlDbType.Varchar,(object)model.groupbooking??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_areyouthegroupleader",NpgsqlDbType.Varchar,(object)model.areyouthegroupleader??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_numberofmember",NpgsqlDbType.Integer,(object)model.numberofmember??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_groupleadersbookingreferencenumber",NpgsqlDbType.Varchar,(object)model.groupleadersbookingreferencenumber??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_paddressline1",NpgsqlDbType.Varchar,(object)model.paddressline1??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_paddressline2",NpgsqlDbType.Varchar,(object)model.paddressline2??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_ppincode",NpgsqlDbType.Integer,(object)model.ppincode??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_ptown",NpgsqlDbType.Varchar,(object)model.ptown??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_pcityordistrict",NpgsqlDbType.Varchar,(object)model.pcityordistrict??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_pstatename",NpgsqlDbType.Varchar,(object)model.pstatename??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_sameaspermanentaddress",NpgsqlDbType.Boolean,(object)model.sameaspermanentaddress??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_caddressline1",NpgsqlDbType.Varchar,(object)model.caddressline1??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_caddressline2",NpgsqlDbType.Varchar,(object)model.caddressline2??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_cpincode",NpgsqlDbType.Integer,(object)model.cpincode??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_ctown",NpgsqlDbType.Varchar,(object)model.ctown??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_ccityordistrict",NpgsqlDbType.Varchar,(object)model.ccityordistrict??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_cstatename",NpgsqlDbType.Varchar,(object)model.cstatename??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_flexiblewithdates",NpgsqlDbType.Varchar,(object)model.flexiblewithdates??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_flexiblewithroomtype",NpgsqlDbType.Varchar,(object)model.flexiblewithroomtype??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_joinwaitinglist",NpgsqlDbType.Varchar,(object)model.joinwaitinglist??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_passportnumber",NpgsqlDbType.Varchar,(object)model.passportnumber??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_passportissuingcountry",NpgsqlDbType.Uuid,(object)model.passportissuingcountry??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_passportexpirydate",NpgsqlDbType.Date,(object)model.passportexpirydate??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_uploadpassportcopy",NpgsqlDbType.Varchar,(object)model.uploadpassportcopy??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_visatype",NpgsqlDbType.Varchar,(object)model.visatype??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_visanumber",NpgsqlDbType.Varchar,(object)model.visanumber??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_visaissuedcountry",NpgsqlDbType.Uuid,(object)model.visaissuedcountry??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_visaissuedate",NpgsqlDbType.Date,(object)model.visaissuedate??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_visaexpirydate",NpgsqlDbType.Date,(object)model.visaexpirydate??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_uploadvisacopy",NpgsqlDbType.Varchar,(object)model.uploadvisacopy??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_doyourequireahospitalprovidedattendant",NpgsqlDbType.Varchar,(object)model.doyourequireahospitalprovidedattendant??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_preferredduration",NpgsqlDbType.Varchar,(object)model.preferredduration??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_admissionreason",NpgsqlDbType.Varchar,(object)model.admissionreason??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_consentform",NpgsqlDbType.Uuid,(object)model.consentform??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_consentfile",NpgsqlDbType.Varchar,(object)model.consentfile??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_agreefortermsandconditions",NpgsqlDbType.Boolean,(object)model.agreefortermsandconditions??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_signature",NpgsqlDbType.Varchar,(object)model.signature??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_verifiedstatus",NpgsqlDbType.Varchar,(object)model.verifiedstatus??DBNull.Value);
if(model.preferreddatesofadmission !=null  && model.preferreddatesofadmission.Count >0)
dbCommand.Parameters.AddWithValue("pvar_preferreddatesofadmission",NpgsqlDbType.Json,JsonConvert.SerializeObject(model.preferreddatesofadmission));
else
dbCommand.Parameters.AddWithValue("pvar_preferreddatesofadmission",NpgsqlDbType.Json,DBNull.Value);
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
if(model.attendantinfo !=null  && model.attendantinfo.Count >0)
dbCommand.Parameters.AddWithValue("pvar_attendantinfo",NpgsqlDbType.Json,JsonConvert.SerializeObject(model.attendantinfo));
else
dbCommand.Parameters.AddWithValue("pvar_attendantinfo",NpgsqlDbType.Json,DBNull.Value);
if(model.roompreference !=null  && model.roompreference.Count >0)
dbCommand.Parameters.AddWithValue("pvar_roompreference",NpgsqlDbType.Json,JsonConvert.SerializeObject(model.roompreference));
else
dbCommand.Parameters.AddWithValue("pvar_roompreference",NpgsqlDbType.Json,DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_attendantpreferreddates", NpgsqlDbType.Json, model.attendantpreferreddates != null ? (object)JsonConvert.SerializeObject(model.attendantpreferreddates) : DBNull.Value);
dbCommand.Parameters.AddWithValue("pvar_attendantroompreference", NpgsqlDbType.Json, model.attendantroompreference != null ? (object)JsonConvert.SerializeObject(model.attendantroompreference) : DBNull.Value);


                        dbCommand.Parameters.AddWithValue("pvar_bookingtype", NpgsqlDbType.Varchar, (object)model.bookingtype ?? DBNull.Value);

                        dbCommand.Parameters.AddWithValue("pvar_groupcode", NpgsqlDbType.Varchar, (object)model.groupcode ?? DBNull.Value);

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
public virtual IPDApplicationFormModel getById_IPDApplicationForm(string IPDApplicationFormid)
									 {
										DataTable dataTable = new DataTable();
										DataSet dataSet = new DataSet();
										try{
												 
												using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
												{
													npsql.Open();
													using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"getById_sp_IPDApplicationForm\"(@pvar_ipdapplicationformid)", npsql))
													{
														dbCommand.CommandType = CommandType.Text;
														dbCommand.Parameters.AddWithValue("pvar_ipdapplicationformid",(object)IPDApplicationFormid??DBNull.Value);
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
											var model = ModelConverter.ConvertDataRowToModel<IPDApplicationFormModel>(row);

											var preferredDatesTable = getById_preferreddatesofadmission(IPDApplicationFormid);
											if (preferredDatesTable != null && preferredDatesTable.Rows.Count > 0)
											{
												model.preferreddatesofadmission = new List<IPDApplicationForm_preferreddatesofadmissionModel>();
												foreach (DataRow r in preferredDatesTable.Rows)
													model.preferreddatesofadmission.Add(ModelConverter.ConvertDataRowToModel<IPDApplicationForm_preferreddatesofadmissionModel>(r));
											}

											var roomPrefTable = getById_roompreference(IPDApplicationFormid);
											if (roomPrefTable != null && roomPrefTable.Rows.Count > 0)
											{
												model.roompreference = new List<IPDApplicationForm_roompreferenceModel>();
												foreach (DataRow r in roomPrefTable.Rows)
													model.roompreference.Add(ModelConverter.ConvertDataRowToModel<IPDApplicationForm_roompreferenceModel>(r));
											}

											// Attendant details are stored in a dependent table, not in the main
											// IPD row. Load them for both submitted and draft applications so the
											// shared Add/Update view can restore Personal attendant mode and its row.
											var attendantInfoTable = getById_attendantinfo(IPDApplicationFormid);
											model.attendantinfo = new List<IPDApplicationForm_attendantinfoModel>();
											if (attendantInfoTable != null)
											{
												foreach (DataRow r in attendantInfoTable.Rows)
													model.attendantinfo.Add(ModelConverter.ConvertDataRowToModel<IPDApplicationForm_attendantinfoModel>(r));
											}

											var attendantRoomPreferenceTable = getById_attendantroompreference(IPDApplicationFormid);
											model.attendantroompreference = new List<IPDApplicationForm_attendantroompreferenceModel>();
											if (attendantRoomPreferenceTable != null)
											{
												foreach (DataRow r in attendantRoomPreferenceTable.Rows)
													model.attendantroompreference.Add(ModelConverter.ConvertDataRowToModel<IPDApplicationForm_attendantroompreferenceModel>(r));
											}

											var attendantPreferredDatesTable = getById_attendantpreferreddates(IPDApplicationFormid);
											model.attendantpreferreddates = new List<IPDApplicationForm_attendantpreferreddatesModel>();
											if (attendantPreferredDatesTable != null)
											{
												foreach (DataRow r in attendantPreferredDatesTable.Rows)
													model.attendantpreferreddates.Add(ModelConverter.ConvertDataRowToModel<IPDApplicationForm_attendantpreferreddatesModel>(r));
											}

											return model;
										}
										else
										{
											return null;
										}
									 }
			 public virtual string  Allot_Room(IPDApplicationFormModel model)
			 { 
				 String ResponseMessage="";
					try{
						 	 
							using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
							{
								npsql.Open();
								using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"Allot_Room\"(@pvar_ipdapplicationformid,@pvar_tenantid,@pvar_bookingreferencenumber,@pvar_packagename,@pvar_isbookingdepositmandatory,@pvar_verifiedstatus,@pvar_room,@pvar_modifieduser)", npsql))
								{
										dbCommand.CommandType = CommandType.Text;
															dbCommand.Parameters.AddWithValue("pvar_ipdapplicationformid",NpgsqlDbType.Uuid,(object)model.IPDApplicationFormid??DBNull.Value);
dbCommand.Parameters.AddWithValue("pvar_tenantid",NpgsqlDbType.Uuid,(object)model.tenantid??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_bookingreferencenumber",NpgsqlDbType.Varchar,(object)model.bookingreferencenumber??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_packagename",NpgsqlDbType.Uuid,(object)model.packagename??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_isbookingdepositmandatory",NpgsqlDbType.Boolean,(object)model.isbookingdepositmandatory);

dbCommand.Parameters.AddWithValue("pvar_verifiedstatus",NpgsqlDbType.Varchar,(object)model.verifiedstatus??DBNull.Value);
if(model.room !=null  && model.room.Count >0)
dbCommand.Parameters.AddWithValue("pvar_room",NpgsqlDbType.Json,JsonConvert.SerializeObject(model.room));
else
dbCommand.Parameters.AddWithValue("pvar_room",NpgsqlDbType.Json,DBNull.Value);
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

        public virtual TransferStayRangeModel GetIPDRoomStayRange(
    Guid ipdApplicationFormId,
    string allottedTo)
        {
            TransferStayRangeModel result = null;

            using (var npsql = new NpgsqlConnection(db_connectionstring))
            {
                npsql.Open();

                using (var cmd = new NpgsqlCommand(@"
            SELECT 
                MIN(fromdate) AS stayfrom,
                MAX(todate) AS stayto
            FROM ipdapplicationform_room
            WHERE ipdapplicationformid = @pvar_ipdapplicationformid
              AND (
                    lower(trim(coalesce(allottedto, ''))) = lower(trim(@pvar_allottedto))
                    OR lower(',' || replace(coalesce(allottedto, ''), ' ', '') || ',')
                       LIKE '%,' || lower(replace(@pvar_allottedto, ' ', '')) || ',%'
                  );
        ", npsql))
                {
                    cmd.Parameters.AddWithValue("pvar_ipdapplicationformid", NpgsqlDbType.Uuid, ipdApplicationFormId);
                    cmd.Parameters.AddWithValue("pvar_allottedto", NpgsqlDbType.Varchar, allottedTo ?? "");

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read() &&
                            reader["stayfrom"] != DBNull.Value &&
                            reader["stayto"] != DBNull.Value)
                        {
                            result = new TransferStayRangeModel
                            {
                                StayFrom = Convert.ToDateTime(reader["stayfrom"]).Date,
                                StayTo = Convert.ToDateTime(reader["stayto"]).Date
                            };
                        }
                    }
                }
            }

            return result;
        }
             public class TransferStayRangeModel
        {
            public DateTime StayFrom { get; set; }
            public DateTime StayTo { get; set; }
        }
        public virtual string Transfer_Room(IPDApplicationFormModel model)
			 {
				 string ResponseMessage = "";
				 try
				 {
					 IPDApplicationForm_roomModel transferRoom = null;
					 if (model.room != null)
					 {
						 foreach (var roomRow in model.room)
						 {
							 transferRoom = roomRow;
							 break;
						 }
					 }
					 using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
					 {
						 npsql.Open();
						 using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"Transfer_Room\"(@pvar_ipdapplicationformid,@pvar_tenantid,@pvar_allottedto,@pvar_roomnumber,@pvar_transferdate,@pvar_todate,@pvar_modifieduser)", npsql))
						 {
							 dbCommand.CommandType = CommandType.Text;
							 dbCommand.Parameters.AddWithValue("pvar_ipdapplicationformid", NpgsqlDbType.Uuid, (object)model.IPDApplicationFormid ?? DBNull.Value);
							 dbCommand.Parameters.AddWithValue("pvar_tenantid", NpgsqlDbType.Uuid, (object)model.tenantid ?? DBNull.Value);
							 dbCommand.Parameters.AddWithValue("pvar_allottedto", NpgsqlDbType.Varchar, (object)transferRoom?.allottedto ?? DBNull.Value);
							 dbCommand.Parameters.AddWithValue("pvar_roomnumber", NpgsqlDbType.Uuid, (object)transferRoom?.roomnumber ?? DBNull.Value);
							 dbCommand.Parameters.AddWithValue("pvar_transferdate", NpgsqlDbType.Timestamp, (object)transferRoom?.fromdate ?? DBNull.Value);
							 dbCommand.Parameters.AddWithValue("pvar_todate", NpgsqlDbType.Timestamp, (object)transferRoom?.todate ?? DBNull.Value);
							 dbCommand.Parameters.AddWithValue("pvar_modifieduser", NpgsqlDbType.Uuid, (object)model.modifieduser ?? DBNull.Value);

							 NpgsqlParameter outParm = new NpgsqlParameter("@returnMessage", NpgsqlDbType.Varchar)
							 {
								 Direction = ParameterDirection.Output
							 };
							 dbCommand.Parameters.Add(outParm);
							 dbCommand.ExecuteNonQuery();
							 ResponseMessage = outParm.Value.ToString();
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

			 public virtual string NormalizeTransferRoomDateRanges(Guid ipdApplicationFormId, string allottedTo, Guid newRoomId, DateTime newRoomFromDate, DateTime newRoomToDate, Guid? modifiedUser)
			 {
				 try
				 {
					 using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
					 {
						 npsql.Open();
						 using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"Normalize_Transfer_Room_Date_Ranges\"(@pvar_ipdapplicationformid,@pvar_allottedto,@pvar_newroomid,@pvar_newfromdate,@pvar_newtodate,@pvar_modifieduser)", npsql))
						 {
							 dbCommand.CommandType = CommandType.Text;
							 dbCommand.Parameters.AddWithValue("pvar_ipdapplicationformid", NpgsqlDbType.Uuid, ipdApplicationFormId);
							 dbCommand.Parameters.AddWithValue("pvar_allottedto", NpgsqlDbType.Varchar, (object)allottedTo ?? DBNull.Value);
							 dbCommand.Parameters.AddWithValue("pvar_newroomid", NpgsqlDbType.Uuid, newRoomId);
							 dbCommand.Parameters.AddWithValue("pvar_newfromdate", NpgsqlDbType.Date, newRoomFromDate.Date);
							 dbCommand.Parameters.AddWithValue("pvar_newtodate", NpgsqlDbType.Date, newRoomToDate.Date);
							 dbCommand.Parameters.AddWithValue("pvar_modifieduser", NpgsqlDbType.Uuid, (object)modifiedUser ?? DBNull.Value);
							 var scalar = dbCommand.ExecuteScalar();
							 if (dbCommand.Connection.State != ConnectionState.Closed)
								 dbCommand.Connection.Dispose();
							 return scalar == null || scalar == DBNull.Value ? "" : scalar.ToString();
						 }
					 }
				 }
				 catch (Exception ex)
				 {
					 return ex.Message;
				 }
			 }

			 public virtual bool HasOverlappingRoomRange(Guid ipdApplicationFormId, string allottedTo, Guid? excludedRoomRowId, DateTime fromDate, DateTime toDate, out string message)
			 {
				 message = "";
				 try
				 {
					 using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
					 {
						 npsql.Open();
						 using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"Get_Overlapping_Room_Range_Message\"(@pvar_ipdapplicationformid,@pvar_allottedto,@pvar_excludedroomrowid,@pvar_fromdate,@pvar_todate)", npsql))
						 {
							 dbCommand.CommandType = CommandType.Text;
							 dbCommand.Parameters.AddWithValue("pvar_ipdapplicationformid", NpgsqlDbType.Uuid, ipdApplicationFormId);
							 dbCommand.Parameters.AddWithValue("pvar_allottedto", NpgsqlDbType.Varchar, (object)allottedTo ?? DBNull.Value);
							 dbCommand.Parameters.AddWithValue("pvar_excludedroomrowid", NpgsqlDbType.Uuid, excludedRoomRowId.HasValue ? (object)excludedRoomRowId.Value : DBNull.Value);
							 dbCommand.Parameters.AddWithValue("pvar_fromdate", NpgsqlDbType.Date, fromDate.Date);
							 dbCommand.Parameters.AddWithValue("pvar_todate", NpgsqlDbType.Date, toDate.Date);
							 var scalar = dbCommand.ExecuteScalar();
							 message = scalar == null || scalar == DBNull.Value ? "" : scalar.ToString();
							 if (dbCommand.Connection.State != ConnectionState.Closed)
								 dbCommand.Connection.Dispose();
						 }
					 }
				 }
				 catch (Exception ex)
				 {
					 message = ex.Message;
					 return true;
				 }

				 return !string.IsNullOrWhiteSpace(message);
			 }

			 public virtual bool ValidateRoomDateRangesDoNotOverlap(Guid ipdApplicationFormId, string allottedTo, out string message)
			 {
				 message = "";
				 try
				 {
					 using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
					 {
						 npsql.Open();
						 using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"Validate_Room_Date_Ranges_Do_Not_Overlap\"(@pvar_ipdapplicationformid,@pvar_allottedto)", npsql))
						 {
							 dbCommand.CommandType = CommandType.Text;
							 dbCommand.Parameters.AddWithValue("pvar_ipdapplicationformid", NpgsqlDbType.Uuid, ipdApplicationFormId);
							 dbCommand.Parameters.AddWithValue("pvar_allottedto", NpgsqlDbType.Varchar, (object)allottedTo ?? DBNull.Value);
							 var scalar = dbCommand.ExecuteScalar();
							 message = scalar == null || scalar == DBNull.Value ? "" : scalar.ToString();
							 if (dbCommand.Connection.State != ConnectionState.Closed)
								 dbCommand.Connection.Dispose();
						 }
					 }

					 return string.Equals((message ?? "").Replace("\"", ""), "201.1", StringComparison.OrdinalIgnoreCase);
				 }
				 catch (Exception ex)
				 {
					 message = ex.Message;
					 return false;
				 }
			 }

			 public virtual string Extend_Stay(IPDApplicationFormModel model)
			 {
				 string ResponseMessage = "";
				 try
				 {
					 IPDApplicationForm_roomModel extendRoom = null;
					 if (model.room != null)
					 {
						 foreach (var roomRow in model.room)
						 {
							 extendRoom = roomRow;
							 break;
						 }
					 }
					 if (model.IPDApplicationFormid == null || extendRoom == null || !extendRoom.todate.HasValue)
						 return "Extend stay details are required";
					 using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
					 {
						 npsql.Open();
						 using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"Extend_Stay\"(@pvar_ipdapplicationformid,@pvar_tenantid,@pvar_allottedto,@pvar_roomnumber,@pvar_extensionfromdate,@pvar_newtodate,@pvar_modifieduser)", npsql))
						 {
							 dbCommand.CommandType = CommandType.Text;
							 dbCommand.Parameters.AddWithValue("pvar_ipdapplicationformid", NpgsqlDbType.Uuid, (object)model.IPDApplicationFormid ?? DBNull.Value);
							 dbCommand.Parameters.AddWithValue("pvar_tenantid", NpgsqlDbType.Uuid, (object)model.tenantid ?? DBNull.Value);
							 dbCommand.Parameters.AddWithValue("pvar_allottedto", NpgsqlDbType.Varchar, (object)extendRoom?.allottedto ?? DBNull.Value);
							 dbCommand.Parameters.AddWithValue("pvar_roomnumber", NpgsqlDbType.Uuid, (object)extendRoom?.roomnumber ?? DBNull.Value);
							 dbCommand.Parameters.AddWithValue("pvar_extensionfromdate", NpgsqlDbType.Timestamp, (object)extendRoom?.fromdate ?? DBNull.Value);
							 dbCommand.Parameters.AddWithValue("pvar_newtodate", NpgsqlDbType.Timestamp, (object)extendRoom?.todate ?? DBNull.Value);
							 dbCommand.Parameters.AddWithValue("pvar_modifieduser", NpgsqlDbType.Uuid, (object)model.modifieduser ?? DBNull.Value);

							 NpgsqlParameter outParm = new NpgsqlParameter("@returnMessage", NpgsqlDbType.Varchar)
							 {
								 Direction = ParameterDirection.Output
							 };
							 dbCommand.Parameters.Add(outParm);
							 dbCommand.ExecuteNonQuery();
							 ResponseMessage = outParm.Value.ToString();
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

			 public virtual Guid? GetPatientNameByIPD(Guid ipdId)
			 {
				 try
				 {
					 using (var npsql = new NpgsqlConnection(db_connectionstring))
					 {
						 npsql.Open();
						 using (var cmd = new NpgsqlCommand(
							 "SELECT \"Get_PatientName_By_IPD\"(@pid)", npsql))
						 {
							 cmd.Parameters.AddWithValue("pid", NpgsqlDbType.Uuid, ipdId);
							 var val = cmd.ExecuteScalar();
							 if (val != null && val != DBNull.Value)
								 return (Guid)val;
						 }
					 }
				 }
				 catch (Exception ex)
				 {
					 Console.WriteLine(ex);
				 }
				 return null;
			 }

			 public virtual Guid? GetPatientVisitByIPD(Guid ipdId)
			 {
				 try
				 {
					 using (var npsql = new NpgsqlConnection(db_connectionstring))
					 {
						 npsql.Open();
						 using (var cmd = new NpgsqlCommand(
							 "SELECT patientvisitid FROM patientvisit WHERE ipdnumber = @pid AND COALESCE(isdeleted, false) = false ORDER BY createddate DESC NULLS LAST LIMIT 1", npsql))
						 {
							 cmd.Parameters.AddWithValue("pid", NpgsqlDbType.Uuid, ipdId);
							 var val = cmd.ExecuteScalar();
							 if (val != null && val != DBNull.Value)
								 return (Guid)val;
						 }
					 }
				 }
				 catch (Exception ex)
				 {
					 Console.WriteLine(ex);
				 }
				 return null;
			 }

			 public virtual string  Update_IPD_Application_Form(IPDApplicationFormModel model)
			 { 
				 ApplyDirectPlannedAdmissionDate(model);
				 String ResponseMessage="";
					try{
						 	 
							using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
							{
								npsql.Open();
								using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"Update_IPD_Application_Form\"(@pvar_ipdapplicationformid,@pvar_tenantid,@pvar_bookingreferencenumber,@pvar_patientname,@pvar_firstname,@pvar_lastname,@pvar_gender,@pvar_mobilenumber,@pvar_whatsappnumber,@pvar_nationality,@pvar_countryoforigin,@pvar_generalcondition,@pvar_bookingstatus,@pvar_groupbooking,@pvar_areyouthegroupleader,@pvar_numberofmember,@pvar_groupleadersbookingreferencenumber,@pvar_paddressline1,@pvar_paddressline2,@pvar_ppincode,@pvar_ptown,@pvar_pcityordistrict,@pvar_pstatename,@pvar_sameaspermanentaddress,@pvar_caddressline1,@pvar_caddressline2,@pvar_cpincode,@pvar_ctown,@pvar_ccityordistrict,@pvar_cstatename,@pvar_flexiblewithdates,@pvar_flexiblewithroomtype,@pvar_joinwaitinglist,@pvar_passportnumber,@pvar_passportissuingcountry,@pvar_passportexpirydate,@pvar_uploadpassportcopy,@pvar_visatype,@pvar_visanumber,@pvar_visaissuedcountry,@pvar_visaissuedate,@pvar_visaexpirydate,@pvar_uploadvisacopy,@pvar_doyourequireahospitalprovidedattendant,@pvar_preferredduration,@pvar_admissionreason,@pvar_consentform,@pvar_consentfile,@pvar_agreefortermsandconditions,@pvar_signature,@pvar_verifiedstatus,@pvar_preferreddatesofadmission,@pvar_medicalinfo,@pvar_medicationinfo,@pvar_medicalrecords,@pvar_attendantinfo,@pvar_roompreference,@pvar_attendantpreferreddates,@pvar_attendantroompreference,@pvar_room,@pvar_bookingtype,@pvar_groupcode,@pvar_modifieduser)", npsql))
								{
										dbCommand.CommandType = CommandType.Text;
															dbCommand.Parameters.AddWithValue("pvar_ipdapplicationformid",NpgsqlDbType.Uuid,(object)model.IPDApplicationFormid??DBNull.Value);
dbCommand.Parameters.AddWithValue("pvar_tenantid",NpgsqlDbType.Uuid,(object)model.tenantid??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_bookingreferencenumber",NpgsqlDbType.Varchar,(object)model.bookingreferencenumber??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_patientname",NpgsqlDbType.Uuid,(object)model.patientname??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_firstname",NpgsqlDbType.Varchar,(object)model.firstname??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_lastname",NpgsqlDbType.Varchar,(object)model.lastname??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_gender",NpgsqlDbType.Varchar,(object)model.gender??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_mobilenumber",NpgsqlDbType.Varchar,(object)model.mobilenumber??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_whatsappnumber",NpgsqlDbType.Varchar,(object)model.whatsappnumber??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_nationality",NpgsqlDbType.Varchar,(object)model.nationality??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_countryoforigin",NpgsqlDbType.Uuid,(object)model.countryoforigin??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_generalcondition",NpgsqlDbType.Varchar,(object)model.generalcondition??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_bookingstatus",NpgsqlDbType.Varchar,(object)model.bookingstatus??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_groupbooking",NpgsqlDbType.Varchar,(object)model.groupbooking??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_areyouthegroupleader",NpgsqlDbType.Varchar,(object)model.areyouthegroupleader??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_numberofmember",NpgsqlDbType.Integer,(object)model.numberofmember??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_groupleadersbookingreferencenumber",NpgsqlDbType.Varchar,(object)model.groupleadersbookingreferencenumber??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_paddressline1",NpgsqlDbType.Varchar,(object)model.paddressline1??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_paddressline2",NpgsqlDbType.Varchar,(object)model.paddressline2??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_ppincode",NpgsqlDbType.Integer,(object)model.ppincode??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_ptown",NpgsqlDbType.Varchar,(object)model.ptown??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_pcityordistrict",NpgsqlDbType.Varchar,(object)model.pcityordistrict??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_pstatename",NpgsqlDbType.Varchar,(object)model.pstatename??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_sameaspermanentaddress",NpgsqlDbType.Boolean,(object)model.sameaspermanentaddress??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_caddressline1",NpgsqlDbType.Varchar,(object)model.caddressline1??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_caddressline2",NpgsqlDbType.Varchar,(object)model.caddressline2??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_cpincode",NpgsqlDbType.Integer,(object)model.cpincode??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_ctown",NpgsqlDbType.Varchar,(object)model.ctown??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_ccityordistrict",NpgsqlDbType.Varchar,(object)model.ccityordistrict??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_cstatename",NpgsqlDbType.Varchar,(object)model.cstatename??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_flexiblewithdates",NpgsqlDbType.Varchar,(object)model.flexiblewithdates??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_flexiblewithroomtype",NpgsqlDbType.Varchar,(object)model.flexiblewithroomtype??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_joinwaitinglist",NpgsqlDbType.Varchar,(object)model.joinwaitinglist??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_passportnumber",NpgsqlDbType.Varchar,(object)model.passportnumber??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_passportissuingcountry",NpgsqlDbType.Uuid,(object)model.passportissuingcountry??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_passportexpirydate",NpgsqlDbType.Date,(object)model.passportexpirydate??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_uploadpassportcopy",NpgsqlDbType.Varchar,(object)model.uploadpassportcopy??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_visatype",NpgsqlDbType.Varchar,(object)model.visatype??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_visanumber",NpgsqlDbType.Varchar,(object)model.visanumber??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_visaissuedcountry",NpgsqlDbType.Uuid,(object)model.visaissuedcountry??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_visaissuedate",NpgsqlDbType.Date,(object)model.visaissuedate??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_visaexpirydate",NpgsqlDbType.Date,(object)model.visaexpirydate??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_uploadvisacopy",NpgsqlDbType.Varchar,(object)model.uploadvisacopy??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_doyourequireahospitalprovidedattendant",NpgsqlDbType.Varchar,(object)model.doyourequireahospitalprovidedattendant??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_preferredduration",NpgsqlDbType.Varchar,(object)model.preferredduration??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_admissionreason",NpgsqlDbType.Varchar,(object)model.admissionreason??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_consentform",NpgsqlDbType.Uuid,(object)model.consentform??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_consentfile",NpgsqlDbType.Varchar,(object)model.consentfile??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_agreefortermsandconditions",NpgsqlDbType.Boolean,(object)model.agreefortermsandconditions??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_signature",NpgsqlDbType.Varchar,(object)model.signature??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_verifiedstatus",NpgsqlDbType.Varchar,(object)model.verifiedstatus??DBNull.Value);
if(model.preferreddatesofadmission !=null  && model.preferreddatesofadmission.Count >0)
dbCommand.Parameters.AddWithValue("pvar_preferreddatesofadmission",NpgsqlDbType.Json,JsonConvert.SerializeObject(model.preferreddatesofadmission));
else
dbCommand.Parameters.AddWithValue("pvar_preferreddatesofadmission",NpgsqlDbType.Json,DBNull.Value);
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
if(model.attendantinfo !=null  && model.attendantinfo.Count >0)
dbCommand.Parameters.AddWithValue("pvar_attendantinfo",NpgsqlDbType.Json,JsonConvert.SerializeObject(model.attendantinfo));
else
dbCommand.Parameters.AddWithValue("pvar_attendantinfo",NpgsqlDbType.Json,DBNull.Value);
if(model.roompreference !=null  && model.roompreference.Count >0)
dbCommand.Parameters.AddWithValue("pvar_roompreference",NpgsqlDbType.Json,JsonConvert.SerializeObject(model.roompreference));
else
dbCommand.Parameters.AddWithValue("pvar_roompreference",NpgsqlDbType.Json,DBNull.Value);
dbCommand.Parameters.AddWithValue("pvar_attendantpreferreddates", NpgsqlDbType.Json, model.attendantpreferreddates != null ? (object)JsonConvert.SerializeObject(model.attendantpreferreddates) : DBNull.Value);
dbCommand.Parameters.AddWithValue("pvar_attendantroompreference", NpgsqlDbType.Json, model.attendantroompreference != null ? (object)JsonConvert.SerializeObject(model.attendantroompreference) : DBNull.Value);
if(model.room !=null  && model.room.Count >0)
dbCommand.Parameters.AddWithValue("pvar_room",NpgsqlDbType.Json,JsonConvert.SerializeObject(model.room));
else
dbCommand.Parameters.AddWithValue("pvar_room",NpgsqlDbType.Json,DBNull.Value);

                        dbCommand.Parameters.AddWithValue("pvar_bookingtype", NpgsqlDbType.Varchar, (object)model.bookingtype ?? DBNull.Value);

                        dbCommand.Parameters.AddWithValue("pvar_groupcode", NpgsqlDbType.Varchar, (object)model.groupcode ?? DBNull.Value);



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
			 public virtual string  Update_IPD_Application_Medical_Info(IPDApplicationFormModel model)
			 { 
				 String ResponseMessage="";
					try{
							using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
							{
								npsql.Open();
								using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"Update_IPD_Application_Medical_Info\"(@pvar_ipdapplicationformid,@pvar_medicalinfo,@pvar_medicationinfo,@pvar_medicalrecords,@pvar_modifieduser)", npsql))
								{
										dbCommand.CommandType = CommandType.Text;
										dbCommand.Parameters.AddWithValue("pvar_ipdapplicationformid",NpgsqlDbType.Uuid,(object)model.IPDApplicationFormid??DBNull.Value);

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

        private decimal GetExistingIPDBookingDepositAmount(
    NpgsqlConnection con,
    NpgsqlTransaction tx,
    Guid ipdApplicationFormId)
        {
            using (var cmd = new NpgsqlCommand(@"
        SELECT COALESCE(SUM(COALESCE(amount, 0)), 0)
        FROM receivable
        WHERE ipdnumber = @ipdid
          AND COALESCE(isdeleted, false) = false
          AND LOWER(COALESCE(receivablefor, '')) = 'ipd booking deposit';
    ", con, tx))
            {
                cmd.Parameters.AddWithValue("ipdid", NpgsqlDbType.Uuid, ipdApplicationFormId);

                var result = cmd.ExecuteScalar();

                return result == null || result == DBNull.Value
                    ? 0m
                    : Convert.ToDecimal(result);
            }
        }
        public virtual string Update_IPD_Package_And_Booking_Deposit_Adjustment(Guid ipdApplicationFormId, Guid? packageId, Guid modifiedUser)
			 {
				 try
				 {
					 using (var npsql = new NpgsqlConnection(db_connectionstring))
					 {
						 npsql.Open();
						 using (var tx = npsql.BeginTransaction())
						 {
							 try
							 {
								 Guid? oldPackageId = null;
								 Guid tenantId = Guid.Empty;
								 Guid patientId = Guid.Empty;
								 using (var cmd = new NpgsqlCommand("SELECT * FROM \"Get_IPD_Package_Tenant_Patient_For_Update\"(@ipdid)", npsql, tx))
								 {
									 cmd.Parameters.AddWithValue("ipdid", NpgsqlDbType.Uuid, ipdApplicationFormId);
									 using (var reader = cmd.ExecuteReader())
									 {
										 if (!reader.Read()) return "IPD not found";
										 if (!reader.IsDBNull(0)) oldPackageId = reader.GetGuid(0);
										 if (!reader.IsDBNull(1)) tenantId = reader.GetGuid(1);
										 if (!reader.IsDBNull(2)) patientId = reader.GetGuid(2);
									 }
								 }

								 var normalizedNewPackageId = packageId.HasValue && packageId.Value != Guid.Empty ? packageId : null;
								 if (oldPackageId == normalizedNewPackageId)
								 {
									 tx.Commit();
									 return "201.1";
								 }

                            decimal oldDeposit = GetPackageBookingDeposit(npsql, tx, oldPackageId);

                            if (!oldPackageId.HasValue || oldPackageId.Value == Guid.Empty)
                            {
                                oldDeposit = GetExistingIPDBookingDepositAmount(
                                    npsql,
                                    tx,
                                    ipdApplicationFormId
                                );
                            }

                            decimal newDeposit = GetPackageBookingDeposit(npsql, tx, normalizedNewPackageId);
                            string oldPackageName = GetPackageName(npsql, tx, oldPackageId);
								 string newPackageName = GetPackageName(npsql, tx, normalizedNewPackageId);
								 string refundPolicy = GetPackageRefundPolicySummary(npsql, tx, normalizedNewPackageId);

								 using (var updateCmd = new NpgsqlCommand("SELECT \"Update_IPD_Package\"(@ipdid,@packageid,@modifieduser)", npsql, tx))
								 {
									 updateCmd.Parameters.AddWithValue("packageid", NpgsqlDbType.Uuid, (object)normalizedNewPackageId ?? DBNull.Value);
									 updateCmd.Parameters.AddWithValue("modifieduser", NpgsqlDbType.Uuid, modifiedUser);
									 updateCmd.Parameters.AddWithValue("ipdid", NpgsqlDbType.Uuid, ipdApplicationFormId);
									 updateCmd.ExecuteNonQuery();
								 }

								 decimal diff = newDeposit - oldDeposit;
								 if (diff != 0)
								 {
									 string receivableFor = diff > 0 ? "IPD Booking Deposit" : "Package Change Refund";
									 string marker = diff > 0 ? "Auto:PackageChange:AdditionalBookingDeposit" : "Auto:PackageChange:BookingDepositRefund";
									 string remarks = diff > 0
										 ? $"Auto package change additional deposit. Previous package: {oldPackageName}; Current package: {newPackageName}; Previous deposit: {oldDeposit:0.00}; Current deposit: {newDeposit:0.00}"
										 : $"Auto package change refund. Previous package: {oldPackageName}; Current package: {newPackageName}; Previous deposit: {oldDeposit:0.00}; Current deposit: {newDeposit:0.00}; Refund policy: {refundPolicy}";
									 InsertReceivableAdjustment(npsql, tx, tenantId, patientId, ipdApplicationFormId, normalizedNewPackageId, receivableFor, diff, marker + " | " + remarks, modifiedUser);
								 }

                       

                            tx.Commit();
								 return "201.1";
							 }
							 catch (Exception ex)
							 {
								 tx.Rollback();
								 return ex.Message;
							 }
						 }
					 }
				 }
				 catch (Exception ex)
				 {
					 return ex.Message;
				 }
			 }
        private string RecalculateActiveRoomReceivablesByPackageAfterPackageChange(
    NpgsqlConnection con,
    NpgsqlTransaction tx,
    Guid ipdApplicationFormId,
    Guid modifiedUser)
        {
            try
            {
                using (var cmd = new NpgsqlCommand(
                    "SELECT public.\"RecalculateActiveRoomReceivablesByPackageAfterTransfer\"(@ipdid, @modifieduser)",
                    con,
                    tx))
                {
                    cmd.Parameters.AddWithValue("ipdid", NpgsqlDbType.Uuid, ipdApplicationFormId);
                    cmd.Parameters.AddWithValue("modifieduser", NpgsqlDbType.Uuid, modifiedUser);

                    var result = cmd.ExecuteScalar();

                    return result?.ToString() ?? "";
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        private decimal GetPackageBookingDeposit(NpgsqlConnection npsql, NpgsqlTransaction tx, Guid? packageId)
			 {
				 if (!packageId.HasValue) return 0;
				 using (var cmd = new NpgsqlCommand("SELECT \"Get_Package_Booking_Deposit\"(@packageid)", npsql, tx))
				 {
					 cmd.Parameters.AddWithValue("packageid", NpgsqlDbType.Uuid, packageId.Value);
					 var value = cmd.ExecuteScalar();
					 return value == null || value == DBNull.Value ? 0 : Convert.ToDecimal(value);
				 }
			 }

			 private string GetPackageName(NpgsqlConnection npsql, NpgsqlTransaction tx, Guid? packageId)
			 {
				 if (!packageId.HasValue) return "None";
				 using (var cmd = new NpgsqlCommand("SELECT \"Get_Package_Name\"(@packageid)", npsql, tx))
				 {
					 cmd.Parameters.AddWithValue("packageid", NpgsqlDbType.Uuid, packageId.Value);
					 return (cmd.ExecuteScalar() ?? "").ToString();
				 }
			 }

			 private string GetPackageRefundPolicySummary(NpgsqlConnection npsql, NpgsqlTransaction tx, Guid? packageId)
			 {
				 if (!packageId.HasValue) return "No package refund policy";
				 using (var cmd = new NpgsqlCommand("SELECT \"Get_Package_Refund_Policy_Summary\"(@packageid)", npsql, tx))
				 {
					 cmd.Parameters.AddWithValue("packageid", NpgsqlDbType.Uuid, packageId.Value);
					 return (cmd.ExecuteScalar() ?? "No package refund policy").ToString();
				 }
			 }

			 private void InsertReceivableAdjustment(NpgsqlConnection npsql, NpgsqlTransaction tx, Guid tenantId, Guid patientId, Guid ipdId, Guid? packageId, string receivableFor, decimal amount, string remarks, Guid userId)
			 {
				 string dayPrefix = DateTime.Now.ToString("yyyyMMdd");
				 int maxSeq = 0;
				 using (var seqCmd = new NpgsqlCommand("SELECT \"Get_Receivable_Day_Max_Sequence\"(@dayprefix)", npsql, tx))
				 {
					 seqCmd.Parameters.AddWithValue("dayprefix", NpgsqlDbType.Varchar, dayPrefix);
					 maxSeq = Convert.ToInt32(seqCmd.ExecuteScalar() ?? 0);
				 }

				 using (var insCmd = new NpgsqlCommand("SELECT \"Insert_Receivable_Adjustment\"(@receivableid,@tenantid,@receivableno,@patientid,@ipdid,@receivablefor,@packageid,@amount,@remarks,@createduser,@paymentstatus)", npsql, tx))
				 {
					 insCmd.Parameters.AddWithValue("receivableid", NpgsqlDbType.Uuid, Guid.NewGuid());
					 insCmd.Parameters.AddWithValue("tenantid", NpgsqlDbType.Uuid, tenantId);
					 insCmd.Parameters.AddWithValue("receivableno", NpgsqlDbType.Varchar, dayPrefix + "-" + (maxSeq + 1).ToString("00000"));
					 insCmd.Parameters.AddWithValue("patientid", NpgsqlDbType.Uuid, patientId);
					 insCmd.Parameters.AddWithValue("ipdid", NpgsqlDbType.Uuid, ipdId);
					 insCmd.Parameters.AddWithValue("receivablefor", NpgsqlDbType.Varchar, receivableFor);
					 insCmd.Parameters.AddWithValue("packageid", NpgsqlDbType.Uuid, (object)packageId ?? DBNull.Value);
					 insCmd.Parameters.AddWithValue("amount", NpgsqlDbType.Numeric, amount);
					 insCmd.Parameters.AddWithValue("remarks", NpgsqlDbType.Varchar, remarks.Length > 256 ? remarks.Substring(0, 256) : remarks);
					 insCmd.Parameters.AddWithValue("createduser", NpgsqlDbType.Uuid, userId);
					 insCmd.Parameters.AddWithValue("paymentstatus", NpgsqlDbType.Varchar, amount < 0 ? "Refund Pending" : "Pending");
					 insCmd.ExecuteNonQuery();
				 }
			 }
public virtual string  Remove_IPD_Application_Form(string id,string loginUserID)
			  { 
				  String ResponseMessage="";
					try{ 
							using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
							{
								npsql.Open();
								using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"Remove_IPD_Application_Form\"(@pvar_ipdapplicationformid,@pvar_modifieduser)", npsql))
								{
										dbCommand.CommandType = CommandType.Text;
										dbCommand.Parameters.AddWithValue("pvar_ipdapplicationformid",(object)id??DBNull.Value);
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
        public virtual JObject Added_IPD_Application_Form(
            string tenantid,
            string patientname,
            string bookingstatus,
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
                        "SELECT * FROM \"Added_IPD_Application_Form\"(" +
                        "@pvar_tenantid," +
                        "@pvar_patientname," +
                        "@pvar_bookingstatus," +
                        "@pvar_verifiedstatus," +
                        "@pvar_pagesize," +
                        "@pvar_pagenumber," +
                        "@pvar_searchterm," +
                        "@pvar_sort_fields," +
                        "@pvar_createddate_automatonfrom," +
                        "@pvar_createddate_automatonto," +
                        "@pvar_bookingnumber,@pvar_workflowstatus,@pvar_financialstatus,@pvar_paymentmethod" +
                        ")", npsql))
                    {
                        dbCommand.CommandType = CommandType.Text;

                        dbCommand.Parameters.AddWithValue("pvar_tenantid", (object)tenantid ?? DBNull.Value);
                        dbCommand.Parameters.AddWithValue("pvar_patientname", (object)patientname ?? DBNull.Value);
                        dbCommand.Parameters.AddWithValue("pvar_bookingstatus", (object)bookingstatus ?? DBNull.Value);
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


        public virtual System.Data.DataTable get_all_IPDApplicationForm(string tenantid,string searchterm="", int? pagesize=1000, int? pagenumber=0)
			  { 

				    DataTable dataTable = new DataTable();
					DataSet dataSet = new DataSet();

					try{
 
							using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
							{
								npsql.Open();
								using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"get_all_IPDApplicationForm\"(@pvar_tenantid)", npsql))
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
public virtual System.Data.DataTable count_of_IPDApplicationForm(string tenantid
,string patientname, string bookingstatus
)
			  { 
					DataTable dataTable = new DataTable();
                DataSet dataSet = new DataSet(); 

					try{
 
							using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
							{
								npsql.Open();
								using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"count_of_IPDApplicationForm\"(@pvar_tenantid,@pvar_patientname,@pvar_bookingstatus)", npsql))
								{
									dbCommand.CommandType = CommandType.Text;
									dbCommand.Parameters.AddWithValue("pvar_tenantid",(object)tenantid??DBNull.Value);
dbCommand.Parameters.AddWithValue("pvar_patientname",(object)patientname??DBNull.Value);
						dbCommand.Parameters.AddWithValue("pvar_bookingstatus", (object)bookingstatus ?? DBNull.Value);
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

		public virtual System.Data.DataTable count_of_IPDApplicationForm_bookingstatus(string tenantid
, string patientname
, string bookingstatus
)
		{
			DataTable dataTable = new DataTable();
			DataSet dataSet = new DataSet();

			try
			{

				using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
				{
					npsql.Open();
					using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"count_of_IPDApplicationForm_bookingstatus\"(@pvar_tenantid,@pvar_patientname,@pvar_bookingstatus)", npsql))
					{
						dbCommand.CommandType = CommandType.Text;
						dbCommand.Parameters.AddWithValue("pvar_tenantid", (object)tenantid ?? DBNull.Value);
						dbCommand.Parameters.AddWithValue("pvar_patientname", (object)patientname ?? DBNull.Value);
						dbCommand.Parameters.AddWithValue("pvar_bookingstatus", (object)bookingstatus ?? DBNull.Value);

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
        public virtual JObject IPD_Application_Forms_for_Review(
    string tenantid,
    string patientname,
    string bookingstatus,
    string verifiedstatus,
    string createddate_automatonfrom = "",
    string createddate_automatonto = "",
    int? pagesize = 1000,
    int? pagenumber = 0,
    string searchterm = "",
    string sort_fields = "")
        {
            object dalResponse = null;

            try
            {
                using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
                {
                    using (var dbCommand = new NpgsqlCommand(
      "SELECT * FROM \"IPD_Application_Forms_for_Review\"(" +
      "@pvar_tenantid," +
      "@pvar_patientname," +
      "@pvar_bookingstatus," +
      "@pvar_verifiedstatus," +
      "@pvar_pagesize," +
      "@pvar_pagenumber," +
      "@pvar_searchterm," +
      "@pvar_sort_fields," +
      "@pvar_createddate_automatonfrom," +
      "@pvar_createddate_automatonto" +
      ")", npsql))
                    {
                        dbCommand.CommandType = CommandType.Text;

                        dbCommand.Parameters.AddWithValue("pvar_tenantid", (object)tenantid ?? DBNull.Value);
                        dbCommand.Parameters.AddWithValue("pvar_patientname", (object)patientname ?? DBNull.Value);
                        dbCommand.Parameters.AddWithValue("pvar_bookingstatus", (object)bookingstatus ?? DBNull.Value);
                        dbCommand.Parameters.AddWithValue("pvar_verifiedstatus", (object)verifiedstatus ?? DBNull.Value);

                      
                        dbCommand.Parameters.AddWithValue("pvar_pagesize", (object)pagesize ?? DBNull.Value);
                        dbCommand.Parameters.AddWithValue("pvar_pagenumber", (object)pagenumber ?? DBNull.Value);
                        dbCommand.Parameters.AddWithValue("pvar_searchterm", (object)searchterm ?? DBNull.Value);

                        if (!string.IsNullOrWhiteSpace(sort_fields) && sort_fields.Length > 2)
                            dbCommand.Parameters.AddWithValue("pvar_sort_fields", NpgsqlDbType.Json, sort_fields);
                        else
                            dbCommand.Parameters.AddWithValue("pvar_sort_fields", NpgsqlDbType.Json, DBNull.Value);

                        dbCommand.Parameters.AddWithValue("pvar_createddate_automatonfrom",
    (object)createddate_automatonfrom ?? DBNull.Value);

                        dbCommand.Parameters.AddWithValue("pvar_createddate_automatonto",
                            (object)createddate_automatonto ?? DBNull.Value);

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


        public virtual JObject Approved_IPD_Application_Forms(string tenantid
,string patientname, string bookingstatus
, int? pagesize=1000 , int? pagenumber=0,string searchterm="",string  sort_fields="")
			  { 
				  object dalResponse = null;
			
					try{
 
							using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
							{
								
								using (var dbCommand = new NpgsqlCommand("SELECT * FROM\"Approved_IPD_Application_Forms\"(@pvar_tenantid,@pvar_patientname,@pvar_bookingstatus,@pvar_pagesize,@pvar_pagenumber,@pvar_searchterm,@pvar_sort_fields)", npsql))
								{
									dbCommand.CommandType = CommandType.Text;
									dbCommand.Parameters.AddWithValue("pvar_tenantid",(object)tenantid??DBNull.Value);
dbCommand.Parameters.AddWithValue("pvar_patientname",(object)patientname??DBNull.Value);
						dbCommand.Parameters.AddWithValue("pvar_bookingstatus", (object)bookingstatus ?? DBNull.Value);
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
			   
			 
public virtual System.Data.DataTable getById_allinfo_IPDApplicationForm(string IPDApplicationFormid)
			 {
				DataSet dataSet=new DataSet();
				DataTable dataTable = new DataTable();
				try{
					     
						using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
						{
							npsql.Open();
							using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"getById_sp_all_IPDApplicationForm\"(@pvar_ipdapplicationformid)", npsql))
							{
								dbCommand.CommandType = CommandType.Text;
								dbCommand.Parameters.AddWithValue("pvar_ipdapplicationformid",(object)IPDApplicationFormid??DBNull.Value);
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
			  public virtual string  verify_IPDApplicationForm(IPDApplicationFormReviewModel model)
			 { 
				 String ResponseMessage="";
					try{
						 
							 
							using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
							{
								npsql.Open();
								using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"verify_IPDApplicationForm\"(@pvar_ipdapplicationformid,@pvar_verifiedby,@pvar_verifiedstatus,@pvar_reviewcomments)", npsql))
								{
										dbCommand.CommandType = CommandType.Text;
										 dbCommand.Parameters.AddWithValue("pvar_ipdapplicationformid",(object)model.IPDApplicationFormid??DBNull.Value);
										dbCommand.Parameters.AddWithValue("pvar_verifiedby",(object)model.verifiedby??DBNull.Value);
										dbCommand.Parameters.AddWithValue("pvar_verifiedstatus",(object)model.verifiedstatus??DBNull.Value);
										dbCommand.Parameters.AddWithValue("pvar_reviewcomments",(object)model.reviewcomments??DBNull.Value); 
											
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
public virtual System.Data.DataTable lookup_IPDApplicationForm_patientname(String tenantid,string searchterm="", int? pagesize=1000, int? pagenumber=0)
							        {
                                            DataSet dataSet = new DataSet();
									        DataTable dataTable=new DataTable();
									        try{

                                        		        using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
					                                    {
						                                    npsql.Open();
						                                    using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"lookup_IPDApplicationForm_patientname\"(@pvar_tenantid,@pvar_searchterm,@pvar_pagesize,@pvar_pagenumber)", npsql))
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
public virtual System.Data.DataTable lookup_IPDApplicationForm_countryoforigin()
							        {
                                            DataSet dataSet = new DataSet();
									        DataTable dataTable=new DataTable();
									        try{

                                        		        using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
					                                    {
						                                    npsql.Open();
						                                    using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"lookup_IPDApplicationForm_countryoforigin\"()", npsql))
						                                    {
                                            
                                                                  
																
																 
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
public virtual System.Data.DataTable lookup_IPDApplicationForm_passportissuingcountry()
							        {
                                            DataSet dataSet = new DataSet();
									        DataTable dataTable=new DataTable();
									        try{

                                        		        using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
					                                    {
						                                    npsql.Open();
						                                    using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"lookup_IPDApplicationForm_passportissuingcountry\"()", npsql))
						                                    {
                                            
                                                                  
																
																 
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
public virtual System.Data.DataTable lookup_IPDApplicationForm_visaissuedcountry()
							        {
                                            DataSet dataSet = new DataSet();
									        DataTable dataTable=new DataTable();
									        try{

                                        		        using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
					                                    {
						                                    npsql.Open();
						                                    using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"lookup_IPDApplicationForm_visaissuedcountry\"()", npsql))
						                                    {
                                            
                                                                  
																
																 
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
public virtual System.Data.DataTable lookup_IPDApplicationForm_consentform(String tenantid)
							        {
                                            DataSet dataSet = new DataSet();
									        DataTable dataTable=new DataTable();
									        try{

                                        		        using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
					                                    {
						                                    npsql.Open();
						                                    using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"lookup_IPDApplicationForm_consentform\"(@pvar_tenantid)", npsql))
						                                    {
                                            
                                                                dbCommand.Parameters.AddWithValue("pvar_tenantid",(object)tenantid??DBNull.Value);  
																
																 
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
public virtual System.Data.DataTable lookup_IPDApplicationForm_packagename(String tenantid)
							        {
                                            DataSet dataSet = new DataSet();
									        DataTable dataTable=new DataTable();
									        try{

                                        		        using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
					                                    {
						                                    npsql.Open();
						                                    using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"lookup_IPDApplicationForm_packagename\"(@pvar_tenantid)", npsql))
						                                    {
                                            
                                                                dbCommand.Parameters.AddWithValue("pvar_tenantid",(object)tenantid??DBNull.Value);  
																
																 
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
public virtual System.Data.DataTable lookup_IPDApplicationForm_medicalinfo_medicalconditionname(string searchterm="", int? pagesize=1000, int? pagenumber=0)
							        {
                                            DataSet dataSet = new DataSet();
									        DataTable dataTable=new DataTable();
									        try{

                                        		        using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
					                                    {
						                                    npsql.Open();
						                                    using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"lookup_IPDApplicationForm_medicalinfo_medicalconditionname\"(@pvar_searchterm,@pvar_pagesize,@pvar_pagenumber)", npsql))
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
public virtual System.Data.DataTable lookup_IPDApplicationForm_roompreference_roomtype(String tenantid)
							        {
                                            DataSet dataSet = new DataSet();
									        DataTable dataTable=new DataTable();
									        try{

                                        		        using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
					                                    {
						                                    npsql.Open();
						                                    using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"lookup_IPDApplicationForm_roompreference_roomtype\"(@pvar_tenantid)", npsql))
						                                    {
                                            
                                                                dbCommand.Parameters.AddWithValue("pvar_tenantid",(object)tenantid??DBNull.Value);  
																
																 
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
public virtual DataTable lookup_IPDApplicationForm_attendantroompreference_roomtypeatt(string tenantid)
{
    return LookupPreferenceRoomType("lookup_IPDApplicationForm_attendantroompreference_roomtypeatt", tenantid);
}

private DataTable LookupPreferenceRoomType(string functionName, string tenantid)
{
    var table = new DataTable();
    using (var npsql = new NpgsqlConnection(db_connectionstring))
    {
        npsql.Open();
        using (var command = new NpgsqlCommand($"SELECT * FROM \"{functionName}\"(@pvar_tenantid)", npsql))
        {
            command.Parameters.AddWithValue("pvar_tenantid", (object)tenantid ?? DBNull.Value);
            using (var adapter = new NpgsqlDataAdapter(command)) adapter.Fill(table);
        }
    }
    return table;
}
public virtual System.Data.DataTable lookup_IPDApplicationForm_room_roomnumber(String tenantid)
							        {
                                            DataSet dataSet = new DataSet();
									        DataTable dataTable=new DataTable();
									        try{

                                        		        using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
					                                    {
						                                    npsql.Open();
						                                    using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"lookup_IPDApplicationForm_room_roomnumber\"(@pvar_tenantid)", npsql))
						                                    {
                                            
                                                                dbCommand.Parameters.AddWithValue("pvar_tenantid",(object)tenantid??DBNull.Value);  
																
																 
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


						public virtual System.Data.DataTable lookup_change_IPDApplicationForm_consentform(string PatientConsentid)
						{
							DataTable dataTable=new DataTable();
                            DataSet dataSet=new DataSet();
							try
							{
								 	 
                                    using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
									{
										npsql.Open();
										using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"lookup_change_IPDApplicationForm_consentform\"(@pvar_patientconsentid)", npsql))
										{
											dbCommand.CommandType = CommandType.Text;
											dbCommand.Parameters.AddWithValue("pvar_patientconsentid",NpgsqlDbType.Varchar,(object)PatientConsentid??DBNull.Value);
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






		public virtual DataTable Get_IPD_BookingDeposit_Receivables(string IPDApplicationFormid)
		{
			var dataTable = new DataTable();
			if (!Guid.TryParse(IPDApplicationFormid, out var ipdGuid)) return dataTable;
			try
			{
				using (var npsql = new NpgsqlConnection(db_connectionstring))
				{
					npsql.Open();
					using (var cmd = new NpgsqlCommand("SELECT * FROM \"Get_IPD_BookingDeposit_Receivables\"(@pvar_ipdid)", npsql))
					{
						cmd.Parameters.AddWithValue("pvar_ipdid", NpgsqlDbType.Uuid, ipdGuid);
						using (var da = new NpgsqlDataAdapter(cmd))
						{
							var ds = new DataSet();
							da.Fill(ds);
							dataTable = ds.Tables[0];
						}
					}
				}
			}
			catch { throw; }
			return dataTable;
		}

		public virtual DataTable Get_All_IPD_Receivables(string IPDApplicationFormid)
		{
			var dataTable = new DataTable();
			if (!Guid.TryParse(IPDApplicationFormid, out var ipdGuid)) return dataTable;
			try
			{
				using (var npsql = new NpgsqlConnection(db_connectionstring))
				{
					npsql.Open();
					using (var cmd = new NpgsqlCommand("SELECT * FROM \"Get_All_IPD_Receivables\"(@pvar_ipdid)", npsql))
					{
						cmd.Parameters.AddWithValue("pvar_ipdid", NpgsqlDbType.Uuid, ipdGuid);
						using (var da = new NpgsqlDataAdapter(cmd))
						{
							var ds = new DataSet();
							da.Fill(ds);
							dataTable = ds.Tables[0];
						}
					}
				}
			}
			catch { throw; }
			return dataTable;
		}
        public virtual DataTable lookup_IPDApplicationForm_packagename_by_roomtype(
    Guid tenantid,
    Guid roomtypeid)
        {
            DataTable dataTable = new DataTable();

            try
            {
                using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
                {
                    npsql.Open();

                    using (var dbCommand = new NpgsqlCommand(
                        "SELECT * FROM public.lookup_ipdapplicationform_packagename_by_roomtype(@pvar_tenantid, @pvar_roomtypeid)",
                        npsql))
                    {
                        dbCommand.CommandType = CommandType.Text;

                        dbCommand.Parameters.AddWithValue("@pvar_tenantid", NpgsqlTypes.NpgsqlDbType.Uuid, tenantid);
                        dbCommand.Parameters.AddWithValue("@pvar_roomtypeid", NpgsqlTypes.NpgsqlDbType.Uuid, roomtypeid);

                        using (NpgsqlDataAdapter dataAdapter = new NpgsqlDataAdapter(dbCommand))
                        {
                            dataAdapter.Fill(dataTable);
                        }
                    }
                }
            }
            catch
            {
                throw;
            }

            return dataTable;
        }

        public virtual bool ValidateGroupCode(string tenantid, string groupcode)
        {
            try
            {
                using (var npsql = new NpgsqlConnection(db_connectionstring))
                {
                    npsql.Open();

                    using (var cmd = new NpgsqlCommand(
                        @"SELECT public.validate_group_code(@tenantid, @groupcode)", npsql))
                    {
                        cmd.Parameters.AddWithValue("tenantid", NpgsqlDbType.Varchar, tenantid);
                        cmd.Parameters.AddWithValue("groupcode", NpgsqlDbType.Varchar, groupcode);

                        return Convert.ToBoolean(cmd.ExecuteScalar());
                    }
                }
            }
            catch
            {
                throw;
            }
        }


        public virtual DataTable get_IPD_Room_Calendar_Cards(
    string tenantid,
    string fromdate = "",
    string todate = "")
        {
            var dataTable = new DataTable();
            var dataSet = new DataSet();

            using (var npsql =
                   new NpgsqlConnection(db_connectionstring))
            {
                npsql.Open();

                const string query = @"
            SELECT *
            FROM public.""get_IPD_Room_Calendar_Cards""(
                @pvar_tenantid,
                @pvar_fromdate,
                @pvar_todate
            );
        ";

                using (var dbCommand =
                       new NpgsqlCommand(query, npsql))
                {
                    dbCommand.CommandType = CommandType.Text;

                    dbCommand.Parameters.AddWithValue(
                        "pvar_tenantid",
                        NpgsqlDbType.Varchar,
                        string.IsNullOrWhiteSpace(tenantid)
                            ? DBNull.Value
                            : (object)tenantid
                    );

                    dbCommand.Parameters.AddWithValue(
                        "pvar_fromdate",
                        NpgsqlDbType.Varchar,
                        string.IsNullOrWhiteSpace(fromdate)
                            ? DBNull.Value
                            : (object)fromdate
                    );

                    dbCommand.Parameters.AddWithValue(
                        "pvar_todate",
                        NpgsqlDbType.Varchar,
                        string.IsNullOrWhiteSpace(todate)
                            ? DBNull.Value
                            : (object)todate
                    );

                    using (var dataAdapter =
                           new NpgsqlDataAdapter(dbCommand))
                    {
                        dataSet.Reset();
                        dataAdapter.Fill(dataSet);

                        if (dataSet.Tables.Count > 0)
                        {
                            dataTable = dataSet.Tables[0];
                        }
                    }
                }
            }

            AppendActiveRoomAllocations(dataTable, tenantid, fromdate, todate);
            return dataTable;
        }

        private void AppendActiveRoomAllocations(DataTable calendarTable, string tenantContext, string fromdate, string todate)
        {
            if (calendarTable == null) return;

            if (!calendarTable.Columns.Contains("bookedFor")) calendarTable.Columns.Add("bookedFor", typeof(string));
            if (!calendarTable.Columns.Contains("isManualBlock")) calendarTable.Columns.Add("isManualBlock", typeof(bool));

            DateTime? from = DateTime.TryParse(fromdate, out var parsedFrom) ? parsedFrom.Date : null;
            DateTime? toExclusive = DateTime.TryParse(todate, out var parsedTo) ? parsedTo.Date : null;

            using (var connection = new NpgsqlConnection(db_connectionstring))
            {
                connection.Open();
                var tenantIds = ResolveCalendarTenantIds(connection, tenantContext);

                using (var command = new NpgsqlCommand(@"
WITH occupancy_days AS
(
    SELECT
        ros.roomoccupancystatusid,
        ros.tenantid,
        ros.room,
        ros.ipdno,
        ros.roomallocationno,
        ros.status,
        ros.bookedfor,
        ros.bookeddate,
        ros.createddate,
        ros.bookeddate -
            (ROW_NUMBER() OVER
            (
                PARTITION BY ros.tenantid, ros.room, ros.ipdno,
                             ros.roomallocationno, LOWER(BTRIM(COALESCE(ros.status, '')))
                ORDER BY ros.bookeddate
            ))::integer AS date_group
    FROM RoomOccupancyStatus ros
    WHERE COALESCE(ros.isdeleted, false) = false
      AND LOWER(BTRIM(COALESCE(ros.status, ''))) IN ('blocked', 'booked', 'occupied')
      AND (ros.tenantid IS NULL OR ros.tenantid = ANY(@tenant_ids))
      AND (@from_date IS NULL OR ros.bookeddate >= @from_date)
      AND (@to_date IS NULL OR ros.bookeddate < @to_date)
      AND NOT EXISTS
      (
          SELECT 1
          FROM RoomAllocation matching_allocation
          WHERE COALESCE(matching_allocation.isdeleted, false) = false
            AND matching_allocation.room = ros.room
            AND LOWER(BTRIM(COALESCE(matching_allocation.status, ''))) IN ('blocked', 'booked', 'occupied')
            AND matching_allocation.fromdate::date <= ros.bookeddate
            AND matching_allocation.todate::date >= ros.bookeddate
      )
),
occupancy_ranges AS
(
    SELECT
        (ARRAY_AGG(roomoccupancystatusid ORDER BY bookeddate))[1] AS allocation_id,
        tenantid,
        room,
        ipdno,
        roomallocationno AS allocation_reference,
        status,
        MAX(COALESCE(bookedfor, '')) AS booked_for,
        MIN(bookeddate)::date AS from_date,
        MAX(bookeddate)::date AS to_date,
        MIN(createddate)::timestamp AS created_date,
        false AS is_manual_block
    FROM occupancy_days
    GROUP BY tenantid, room, ipdno, roomallocationno, status, date_group
),
candidate_rows AS
(
    SELECT
        ra.roomallocationid AS allocation_id,
        ra.tenantid,
        ra.room,
        ra.ipdno,
        ra.roomallocationno AS allocation_reference,
        ra.status,
        COALESCE(ra.bookedfor, '') AS booked_for,
        ra.fromdate::date AS from_date,
        ra.todate::date AS to_date,
        ra.createddate::timestamp AS created_date,
        COALESCE(ra.ismanuallyblocked, false) AS is_manual_block
    FROM RoomAllocation ra
    WHERE COALESCE(ra.isdeleted, false) = false
      AND LOWER(BTRIM(COALESCE(ra.status, ''))) IN ('blocked', 'booked', 'occupied')
      AND ra.room IS NOT NULL
      AND ra.fromdate IS NOT NULL
      AND ra.todate IS NOT NULL
      AND (ra.tenantid IS NULL OR ra.tenantid = ANY(@tenant_ids))
      AND (@from_date IS NULL OR ra.todate::date >= @from_date)
      AND (@to_date IS NULL OR ra.fromdate::date < @to_date)

    UNION ALL

    SELECT
        allocation_id,
        tenantid,
        room,
        ipdno,
        allocation_reference,
        status,
        booked_for,
        from_date,
        to_date,
        created_date,
        is_manual_block
    FROM occupancy_ranges
)
SELECT
    candidate.allocation_id,
    candidate.ipdno,
    candidate.room,
    room.roomtype,
    COALESCE(NULLIF(BTRIM(room.roomnumber), ''), room.roomcode) AS room_number,
    candidate.from_date,
    candidate.to_date,
    CASE LOWER(BTRIM(COALESCE(candidate.status, '')))
        WHEN 'occupied' THEN 'Occupied'
        WHEN 'booked' THEN 'Booked'
        ELSE 'Blocked'
    END AS calendar_status,
    candidate.booked_for,
    candidate.created_date,
    candidate.is_manual_block,
    COALESCE(NULLIF(BTRIM(ipd.bookingreferencenumber), ''),
             NULLIF(BTRIM(candidate.allocation_reference), ''),
             candidate.allocation_id::varchar) AS booking_reference,
    COALESCE(ipd.bookingtype, '') AS booking_type,
    COALESCE(ipd.groupcode, '') AS group_code,
    CASE
        WHEN candidate.is_manual_block THEN
            CASE
                WHEN NULLIF(BTRIM(candidate.booked_for), '') IS NULL THEN 'Blocked'
                ELSE 'Blocked: ' || SPLIT_PART(candidate.booked_for, ' | Description:', 1)
            END
        ELSE COALESCE(
            NULLIF(BTRIM(CONCAT_WS(' ', ipd.firstname, ipd.lastname)), ''),
            NULLIF(BTRIM(ipd.bookingreferencenumber), ''),
            CASE LOWER(BTRIM(COALESCE(candidate.status, '')))
                WHEN 'occupied' THEN 'Occupied'
                WHEN 'booked' THEN 'Booked'
                ELSE 'Blocked'
            END)
    END AS patient_name,
    COALESCE(ipd.gender, '') AS patient_gender,
    COALESCE(ipd.mobilenumber, '') AS patient_phone,
    COALESCE(pp.emailaddress, '') AS patient_email,
    COALESCE(ipd.generalcondition, '') AS general_condition,
    COALESCE(ipd.bookingstatus, candidate.status, '') AS booking_status,
    CASE
        WHEN candidate.is_manual_block THEN 'Manual Block'
        WHEN LOWER(BTRIM(COALESCE(candidate.booked_for, ''))) LIKE '%attendant%' THEN 'Attendant'
        ELSE 'Patient'
    END AS allotted_to
FROM candidate_rows candidate
INNER JOIN Room room
    ON room.Roomid = candidate.room
   AND COALESCE(room.isdeleted, false) = false
LEFT JOIN IPDApplicationForm ipd
    ON ipd.IPDApplicationFormid = candidate.ipdno
   AND COALESCE(ipd.isdeleted, false) = false
LEFT JOIN PatientProfile pp
    ON pp.PatientProfileid = ipd.patientname
   AND COALESCE(pp.isdeleted, false) = false
ORDER BY candidate.from_date, room_number", connection))
                {
                    command.Parameters.AddWithValue(
                        "tenant_ids",
                        NpgsqlDbType.Array | NpgsqlDbType.Uuid,
                        tenantIds
                    );
                    command.Parameters.AddWithValue("from_date", NpgsqlDbType.Date, (object)from ?? DBNull.Value);
                    command.Parameters.AddWithValue("to_date", NpgsqlDbType.Date, (object)toExclusive ?? DBNull.Value);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var allocationId = reader.GetGuid(0);
                            var applicationId = reader.IsDBNull(1) ? (Guid?)null : reader.GetGuid(1);
                            var roomId = reader.GetGuid(2);
                            var roomTypeId = reader.GetGuid(3);
                            var roomNumber = reader.IsDBNull(4) ? "" : reader.GetString(4);
                            var allocationFrom = reader.GetDateTime(5).Date;
                            var allocationTo = reader.GetDateTime(6).Date;
                            var calendarStatus = reader.IsDBNull(7) ? "Blocked" : reader.GetString(7);
                            var bookedFor = reader.IsDBNull(8) ? "" : reader.GetString(8);
                            var createdDate = reader.IsDBNull(9) ? DateTime.Now : reader.GetDateTime(9);
                            var isManualBlock = !reader.IsDBNull(10) && reader.GetBoolean(10);
                            var allottedTo = reader.IsDBNull(20) ? "Patient" : reader.GetString(20);

                            var existingRow = FindCalendarAllocationRow(
                                calendarTable,
                                allocationId,
                                applicationId,
                                roomId,
                                allocationFrom,
                                allocationTo,
                                allottedTo
                            );

                            if (existingRow != null)
                            {
                                MergeCalendarAllocationRange(
                                    existingRow,
                                    allocationFrom,
                                    allocationTo,
                                    calendarStatus,
                                    bookedFor,
                                    isManualBlock
                                );
                                continue;
                            }

                            var row = calendarTable.NewRow();
                            SetCalendarValue(row, "recordType", isManualBlock ? "BLOCKED" : "FIXED");
                            SetCalendarValue(row, "allocationId", allocationId);
                            SetCalendarValue(row, "applicationId", applicationId);
                            SetCalendarValue(row, "bookingReferenceNumber", reader.IsDBNull(11) ? "" : reader.GetString(11));
                            SetCalendarValue(row, "bookingtype", reader.IsDBNull(12) ? "" : reader.GetString(12));
                            SetCalendarValue(row, "groupcode", reader.IsDBNull(13) ? "" : reader.GetString(13));
                            SetCalendarValue(row, "patientName", reader.IsDBNull(14) ? calendarStatus : reader.GetString(14));
                            SetCalendarValue(row, "patientGender", reader.IsDBNull(15) ? "" : reader.GetString(15));
                            SetCalendarValue(row, "patientPhone", reader.IsDBNull(16) ? "" : reader.GetString(16));
                            SetCalendarValue(row, "patientEmail", reader.IsDBNull(17) ? "" : reader.GetString(17));
                            SetCalendarValue(row, "generalCondition", reader.IsDBNull(18) ? "" : reader.GetString(18));
                            SetCalendarValue(row, "flexibleWithDates", false);
                            SetCalendarValue(row, "flexibleWithRoomType", false);
                            SetCalendarValue(row, "joinWaitingList", false);
                            SetCalendarValue(row, "bookingStatus", reader.IsDBNull(19) ? calendarStatus : reader.GetString(19));
                            SetCalendarValue(row, "calendarStatus", calendarStatus);
                            SetCalendarValue(row, "allottedTo", allottedTo);
                            SetCalendarValue(row, "recordOrder", 1);
                            SetCalendarValue(row, "roomId", roomId);
                            SetCalendarValue(row, "roomTypeId", roomTypeId);
                            SetCalendarValue(row, "roomNumber", roomNumber);
                            SetCalendarValue(row, "fromDate", allocationFrom);
                            SetCalendarValue(row, "toDate", allocationTo);
                            SetCalendarValue(row, "createdDate", createdDate);
                            SetCalendarValue(row, "bookedFor", bookedFor);
                            SetCalendarValue(row, "isManualBlock", isManualBlock);
                            SetCalendarValue(row, "hasAttendant", string.Equals(allottedTo, "Attendant", StringComparison.OrdinalIgnoreCase));
                            SetCalendarValue(row, "roomPreferences", "[]");
                            SetCalendarValue(row, "attendantRoomPreferences", "[]");
                            SetCalendarValue(row, "patientDates", "[]");
                            SetCalendarValue(row, "attendant", "{}");
                            SetCalendarValue(row, "attendantDates", "[]");
                            calendarTable.Rows.Add(row);
                        }
                    }
                }
            }
        }

        private Guid[] ResolveCalendarTenantIds(NpgsqlConnection connection, string tenantContext)
        {
            var tenantParts = (tenantContext ?? "").Split('|');
            var userText = tenantParts.Length > 1 ? tenantParts[0] : "";
            var selectedTenantText = tenantParts.Length > 1 ? tenantParts[1] : tenantParts[0];

            var tenantIds = new List<Guid>();
            if (Guid.TryParse(selectedTenantText, out var selectedTenantId) && selectedTenantId != Guid.Empty)
            {
                tenantIds.Add(selectedTenantId);
            }
            else
            {
                object viewerTenantValue = null;
                if (Guid.TryParse(userText, out var userId))
                {
                    using (var viewerCommand = new NpgsqlCommand(
                        "SELECT viewertenantids FROM users WHERE usersid = @user_id",
                        connection))
                    {
                        viewerCommand.Parameters.AddWithValue("user_id", NpgsqlDbType.Uuid, userId);
                        viewerTenantValue = viewerCommand.ExecuteScalar();
                    }
                }

                if (viewerTenantValue != null && viewerTenantValue != DBNull.Value)
                {
                    foreach (var tenantValue in viewerTenantValue.ToString().Split(','))
                    {
                        if (Guid.TryParse(tenantValue.Trim(), out var viewerTenantId))
                            tenantIds.Add(viewerTenantId);
                    }
                }
                else
                {
                    using (var allTenantsCommand = new NpgsqlCommand(
                        "SELECT tenantid FROM tenant",
                        connection))
                    using (var tenantReader = allTenantsCommand.ExecuteReader())
                    {
                        while (tenantReader.Read())
                        {
                            if (!tenantReader.IsDBNull(0)) tenantIds.Add(tenantReader.GetGuid(0));
                        }
                    }
                }
            }

            tenantIds.Add(Guid.Empty);
            return tenantIds.Distinct().ToArray();
        }

        private DataRow FindCalendarAllocationRow(
            DataTable calendarTable,
            Guid allocationId,
            Guid? applicationId,
            Guid roomId,
            DateTime fromDate,
            DateTime toDate,
            string allottedTo)
        {
            foreach (DataRow row in calendarTable.Rows)
            {
                if (CalendarGuid(row, "allocationId") == allocationId) return row;
                if (!applicationId.HasValue || CalendarGuid(row, "applicationId") != applicationId.Value) continue;
                if (CalendarGuid(row, "roomId") != roomId) continue;

                var existingRole = CalendarText(row, "allottedTo");
                if (!string.IsNullOrWhiteSpace(existingRole)
                    && !string.IsNullOrWhiteSpace(allottedTo)
                    && !string.Equals(existingRole, allottedTo, StringComparison.OrdinalIgnoreCase))
                    continue;

                var existingFrom = CalendarDate(row, "fromDate");
                var existingTo = CalendarDate(row, "toDate");
                if (existingFrom.HasValue && existingTo.HasValue
                    && existingFrom.Value.Date <= toDate.Date
                    && existingTo.Value.Date >= fromDate.Date)
                    return row;
            }

            return null;
        }

        private void MergeCalendarAllocationRange(
            DataRow row,
            DateTime fromDate,
            DateTime toDate,
            string calendarStatus,
            string bookedFor,
            bool isManualBlock)
        {
            var existingFrom = CalendarDate(row, "fromDate");
            var existingTo = CalendarDate(row, "toDate");
            SetCalendarValue(row, "fromDate", existingFrom.HasValue && existingFrom.Value.Date < fromDate.Date ? existingFrom.Value.Date : fromDate.Date);
            SetCalendarValue(row, "toDate", existingTo.HasValue && existingTo.Value.Date > toDate.Date ? existingTo.Value.Date : toDate.Date);
            SetCalendarValue(row, "calendarStatus", calendarStatus);
            SetCalendarValue(row, "bookedFor", bookedFor);

            var existingManual = row.Table.Columns.Contains("isManualBlock")
                && row["isManualBlock"] != DBNull.Value
                && Convert.ToBoolean(row["isManualBlock"]);
            SetCalendarValue(row, "isManualBlock", existingManual || isManualBlock);
        }

        private static void SetCalendarValue(DataRow row, string columnName, object value)
        {
            if (!row.Table.Columns.Contains(columnName)) return;
            row[columnName] = value ?? DBNull.Value;
        }

        private static Guid CalendarGuid(DataRow row, string columnName)
        {
            if (!row.Table.Columns.Contains(columnName) || row[columnName] == DBNull.Value) return Guid.Empty;
            return Guid.TryParse(row[columnName].ToString(), out var value) ? value : Guid.Empty;
        }

        private static DateTime? CalendarDate(DataRow row, string columnName)
        {
            if (!row.Table.Columns.Contains(columnName) || row[columnName] == DBNull.Value) return null;
            return DateTime.TryParse(row[columnName].ToString(), out var value) ? value.Date : (DateTime?)null;
        }

        private static string CalendarText(DataRow row, string columnName)
        {
            if (!row.Table.Columns.Contains(columnName) || row[columnName] == DBNull.Value) return "";
            return row[columnName].ToString();
        }

        public virtual string Process_IPD_Room_Calendar_Action(
     IPDCalendarActionModel model
 )
        {
            string responseMessage = "";

            try
            {
                using (
                    NpgsqlConnection npsql =
                        new NpgsqlConnection(db_connectionstring)
                )
                {
                    npsql.Open();

                    using (
                        var dbCommand =
                            new NpgsqlCommand(
                                @"SELECT * FROM ""Process_IPD_Room_Calendar_Action""(
                            @pvar_action,
                            @pvar_items,
                            @pvar_tenantid,
                            @pvar_modifieduser,
                            @pvar_reviewcomments
                        )",
                                npsql
                            )
                    )
                    {
                        dbCommand.CommandType =
                            CommandType.Text;

                        dbCommand.Parameters.AddWithValue(
                            "pvar_action",
                            NpgsqlDbType.Varchar,
                            (object)model.action ??
                            DBNull.Value
                        );


                        /*
                         * IMPORTANT:
                         *
                         * PackageId is already inside model.items.
                         *
                         * Example:
                         *
                         * [
                         *   {
                         *      "IPDApplicationFormid": "...",
                         *      "PackageId": "...",
                         *      "room": [...]
                         *   }
                         * ]
                         */
                        string itemsJson =
                            model.items != null &&
                            model.items.Count > 0
                                ? JsonConvert.SerializeObject(
                                    model.items
                                )
                                : null;


                        dbCommand.Parameters.AddWithValue(
                            "pvar_items",
                            NpgsqlDbType.Json,
                            (object)itemsJson ??
                            DBNull.Value
                        );


                        dbCommand.Parameters.AddWithValue(
                            "pvar_tenantid",
                            NpgsqlDbType.Uuid,
                            (object)model.tenantid ??
                            DBNull.Value
                        );


                        dbCommand.Parameters.AddWithValue(
                            "pvar_modifieduser",
                            NpgsqlDbType.Uuid,
                            (object)model.modifieduser ??
                            DBNull.Value
                        );


                        dbCommand.Parameters.AddWithValue(
                            "pvar_reviewcomments",
                            NpgsqlDbType.Varchar,
                            (object)model.reviewcomments ??
                            DBNull.Value
                        );


                        /*
                         * PostgreSQL function returns returnMessage
                         * as a result column.
                         */
                        var result =
                            dbCommand.ExecuteScalar();


                        responseMessage =
                            result == null ||
                            result == DBNull.Value
                                ? ""
                                : result.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                responseMessage =
                    ex.Message;
            }

            return responseMessage;
        }


    }


			    }
