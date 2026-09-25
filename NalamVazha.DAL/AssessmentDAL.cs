namespace NalamVazha.DAL{
			    using System;
			    using System.Text;
			    using System.Linq;
			    using System.Data;
			    using System.Data.Common;
			    using NalamVazha.Models;
			    using EncrypDecrypt;
			    using Newtonsoft.Json;
				using Newtonsoft.Json.Linq;
                using Npgsql;
				using NpgsqlTypes;
	using System.Text.RegularExpressions;

	//This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/23/2026 16:41:43
				 public class AssessmentDAL
	{
					public virtual string db_connectionstring{get;set;}

					private static bool IsSuccessfulAssessmentSave(string response)
					{
						return !string.IsNullOrWhiteSpace(response)
							&& response.Replace("\"", "").Contains("201.1");
					}

					private void AppendAssessmentHistory(
						NpgsqlConnection connection,
						NpgsqlTransaction transaction,
						AssessmentModel model,
						Guid actionBy,
						string defaultActionType)
					{
						if (!model.Assessmentid.HasValue || model.Assessmentid.Value == Guid.Empty)
							throw new InvalidOperationException("Assessmentid is required to create assessment history.");
						if (!model.tenantid.HasValue || model.tenantid.Value == Guid.Empty)
							throw new InvalidOperationException("Tenantid is required to create assessment history.");

						var actionType = string.IsNullOrWhiteSpace(model.historyactiontype)
							? defaultActionType
							: model.historyactiontype.Trim();
						var workflowStage = string.IsNullOrWhiteSpace(model.workflowstage)
							? (model.ipdform.HasValue ? "IPD_ASSESSMENT" : "OPD_ASSESSMENT")
							: model.workflowstage.Trim();
						var snapshot = JsonConvert.SerializeObject(model);

						using (var historyCommand = new NpgsqlCommand(
							"SELECT * FROM \"Append_Assessment_History\"("
							+ "@pvar_assessmenthistoryid,@pvar_assessmentid,@pvar_tenantid,"
							+ "@pvar_patientname,@pvar_patientvisit,@pvar_ipdform,@pvar_opdform,"
							+ "@pvar_actiontype,@pvar_workflowstage,@pvar_assessmentdate,"
							+ "@pvar_questionnairetemplate,@pvar_doctorname,@pvar_assessedby,"
							+ "@pvar_assessmentnotes,@pvar_eligibleforfinaladmission,@pvar_taskname,"
							+ "@pvar_assessment_snapshot,@pvar_actionby,@pvar_actionbyrole,@pvar_reviewnotes)",
							connection,
							transaction))
						{
							historyCommand.Parameters.AddWithValue("pvar_assessmenthistoryid", NpgsqlDbType.Uuid, Guid.NewGuid());
							historyCommand.Parameters.AddWithValue("pvar_assessmentid", NpgsqlDbType.Uuid, model.Assessmentid.Value);
							historyCommand.Parameters.AddWithValue("pvar_tenantid", NpgsqlDbType.Uuid, model.tenantid.Value);
							historyCommand.Parameters.AddWithValue("pvar_patientname", NpgsqlDbType.Uuid, model.patientname);
							historyCommand.Parameters.AddWithValue("pvar_patientvisit", NpgsqlDbType.Uuid, (object)model.patientvisit ?? DBNull.Value);
							historyCommand.Parameters.AddWithValue("pvar_ipdform", NpgsqlDbType.Uuid, (object)model.ipdform ?? DBNull.Value);
							historyCommand.Parameters.AddWithValue("pvar_opdform", NpgsqlDbType.Uuid, (object)model.opdform ?? DBNull.Value);
							historyCommand.Parameters.AddWithValue("pvar_actiontype", NpgsqlDbType.Varchar, actionType);
							historyCommand.Parameters.AddWithValue("pvar_workflowstage", NpgsqlDbType.Varchar, workflowStage);
							historyCommand.Parameters.AddWithValue("pvar_assessmentdate", NpgsqlDbType.Timestamp, model.assessmentdate);
							historyCommand.Parameters.AddWithValue("pvar_questionnairetemplate", NpgsqlDbType.Uuid, model.questionnairetemplate);
							historyCommand.Parameters.AddWithValue("pvar_doctorname", NpgsqlDbType.Uuid, (object)model.doctorname ?? DBNull.Value);
							historyCommand.Parameters.AddWithValue("pvar_assessedby", NpgsqlDbType.Varchar, (object)model.assessedby ?? DBNull.Value);
							historyCommand.Parameters.AddWithValue("pvar_assessmentnotes", NpgsqlDbType.Text, (object)model.assessmentnotes ?? DBNull.Value);
							historyCommand.Parameters.AddWithValue("pvar_eligibleforfinaladmission", NpgsqlDbType.Varchar, (object)model.eligibleforfinaladmission ?? DBNull.Value);
							historyCommand.Parameters.AddWithValue("pvar_taskname", NpgsqlDbType.Varchar, (object)model.taskname ?? DBNull.Value);
							historyCommand.Parameters.AddWithValue("pvar_assessment_snapshot", NpgsqlDbType.Jsonb, snapshot);
							historyCommand.Parameters.AddWithValue("pvar_actionby", NpgsqlDbType.Uuid, actionBy);
							historyCommand.Parameters.AddWithValue("pvar_actionbyrole", NpgsqlDbType.Varchar, (object)model.actionbyrole ?? DBNull.Value);
							historyCommand.Parameters.AddWithValue("pvar_reviewnotes", NpgsqlDbType.Text, (object)model.reviewnotes ?? DBNull.Value);
							var historyResponse = historyCommand.ExecuteScalar()?.ToString() ?? "";
							if (!IsSuccessfulAssessmentSave(historyResponse))
								throw new InvalidOperationException(historyResponse);
						}
					}

					public virtual DataTable GetAssessmentHistory(
						Guid? assessmentId = null,
						Guid? ipdFormId = null,
						Guid? opdFormId = null)
					{
						var result = new DataTable();
						using (var connection = new NpgsqlConnection(db_connectionstring))
						using (var command = new NpgsqlCommand(
							"SELECT * FROM \"Get_Assessment_History\"("
							+ "@pvar_assessmentid,@pvar_ipdform,@pvar_opdform)",
							connection))
						{
							command.Parameters.AddWithValue("pvar_assessmentid", NpgsqlDbType.Uuid, (object)assessmentId ?? DBNull.Value);
							command.Parameters.AddWithValue("pvar_ipdform", NpgsqlDbType.Uuid, (object)ipdFormId ?? DBNull.Value);
							command.Parameters.AddWithValue("pvar_opdform", NpgsqlDbType.Uuid, (object)opdFormId ?? DBNull.Value);
							connection.Open();
							using (var adapter = new NpgsqlDataAdapter(command))
								adapter.Fill(result);
						}
						return result;
					}

					public virtual bool CanSaveOPDAssessment(Guid? opdFormId, Guid userId)
					{
						if (!opdFormId.HasValue || opdFormId.Value == Guid.Empty) return true;
						using (var connection = new NpgsqlConnection(db_connectionstring))
						using (var command = new NpgsqlCommand("SELECT * FROM \"Can_Save_OPD_Assessment\"(@pvar_opdformid,@pvar_userid)", connection))
						{
							command.Parameters.AddWithValue("pvar_opdformid", NpgsqlDbType.Uuid, opdFormId.Value);
							command.Parameters.AddWithValue("pvar_userid", NpgsqlDbType.Uuid, userId);
							connection.Open();
							var result = command.ExecuteScalar();
							return result != null && result != DBNull.Value && (bool)result;
						}
					}

					public virtual string AdvanceOPDAssessmentStatus(Guid? opdFormId, Guid userId)
					{
						if (!opdFormId.HasValue || opdFormId.Value == Guid.Empty) return "201.1";
						using (var connection = new NpgsqlConnection(db_connectionstring))
						using (var command = new NpgsqlCommand("SELECT * FROM \"Advance_OPD_Assessment_Status\"(@pvar_opdformid,@pvar_userid)", connection))
						{
							command.Parameters.AddWithValue("pvar_opdformid", NpgsqlDbType.Uuid, opdFormId.Value);
							command.Parameters.AddWithValue("pvar_userid", NpgsqlDbType.Uuid, userId);
							connection.Open();
							return command.ExecuteScalar()?.ToString() ?? "OPD assessment status update failed.";
						}
					}

					public virtual string SaveOPDAssessmentDraftStatus(Guid? opdFormId, Guid userId)
					{
						if (!opdFormId.HasValue || opdFormId.Value == Guid.Empty) return "201.1";
						using (var connection = new NpgsqlConnection(db_connectionstring))
						using (var command = new NpgsqlCommand("SELECT * FROM \"Save_OPD_Assessment_Draft_Status\"(@pvar_opdformid,@pvar_userid)", connection))
						{
							command.Parameters.AddWithValue("pvar_opdformid", NpgsqlDbType.Uuid, opdFormId.Value);
							command.Parameters.AddWithValue("pvar_userid", NpgsqlDbType.Uuid, userId);
							connection.Open();
							return command.ExecuteScalar()?.ToString() ?? "OPD assessment draft status update failed.";
						}
					}
					
			 	    public AssessmentDAL(string connectionString)
				    {
						
					    db_connectionstring=connectionString;
				    }
				  
			        public virtual System.Data.DataTable getById_assessmentquestions(string Assessmentid)
			 {
					DataTable dataTable=new DataTable();
					DataSet dataSet=new DataSet();
					try
					{
						  
							using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
							{
								npsql.Open();
								using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"getById_sp_Assessment_assessmentquestions\"(@pvar_assessmentid)", npsql))
								{
									dbCommand.CommandType = CommandType.Text;
									dbCommand.Parameters.AddWithValue("pvar_assessmentid",(object)Assessmentid??DBNull.Value);
								
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
// ── Patient Answers ─────────────────────────────────────────────────────────
        public virtual string Save_patientanswers(string assessmentid, string answersJson)
        {
            string responseMessage = "";
            try
            {
                using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
                {
                    npsql.Open();
                    using (var dbCommand = new NpgsqlCommand(
                        "SELECT * FROM \"Save_Assessment_patientanswers\"(@pvar_assessmentid,@pvar_patientanswers)", npsql))
                    {
                        dbCommand.CommandType = CommandType.Text;
                        dbCommand.Parameters.AddWithValue("pvar_assessmentid",   NpgsqlDbType.Uuid,    new Guid(assessmentid));
                        dbCommand.Parameters.AddWithValue("pvar_patientanswers", NpgsqlDbType.Json,
                            string.IsNullOrWhiteSpace(answersJson) ? (object)DBNull.Value : answersJson);

                        NpgsqlParameter outParm = new NpgsqlParameter("pvar_returnMessage", NpgsqlDbType.Varchar)
                            { Direction = ParameterDirection.Output };
                        dbCommand.Parameters.Add(outParm);
                        dbCommand.ExecuteNonQuery();
                        responseMessage = outParm.Value?.ToString() ?? "";
                        if (dbCommand.Connection.State != ConnectionState.Closed)
                            dbCommand.Connection.Dispose();
                    }
                    npsql.Close();
                }
            }
            catch (Exception ex) { responseMessage = ex.Message; Console.WriteLine(ex); }
            return responseMessage;
        }

        public virtual System.Data.DataTable getById_patientanswers(string Assessmentid)
        {
            DataTable dataTable = new DataTable();
            DataSet dataSet = new DataSet();
            try
            {
                using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
                {
                    npsql.Open();
                    using (var dbCommand = new NpgsqlCommand(
                        "SELECT * FROM \"getById_sp_Assessment_patientanswers\"(@pvar_assessmentid)", npsql))
                    {
                        dbCommand.CommandType = CommandType.Text;
                        dbCommand.Parameters.AddWithValue("pvar_assessmentid", (object)Assessmentid ?? DBNull.Value);
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

        public virtual System.Data.DataTable getLatestPreviousPatientAnswers(
            string patientname, string questionnairetemplate, string taskname = "", string opdform = "", string ipdform = "")
        {
            if (!Guid.TryParse(patientname, out var patientId)
                || !Guid.TryParse(questionnairetemplate, out var templateId))
                return new DataTable();

            Guid currentOpdId;
            var hasCurrentOpd = Guid.TryParse(opdform, out currentOpdId) && currentOpdId != Guid.Empty;
            Guid currentIpdId;
            var hasCurrentIpd = Guid.TryParse(ipdform, out currentIpdId) && currentIpdId != Guid.Empty;
            using (var connection = new NpgsqlConnection(db_connectionstring))
            using (var command = new NpgsqlCommand(
                "SELECT * FROM \"Get_Latest_Previous_Assessment_PatientAnswers\"(@pvar_patientname,@pvar_questionnairetemplate,@pvar_taskname,@pvar_opdform,@pvar_ipdform)",
                connection))
            {
                command.Parameters.AddWithValue("pvar_patientname", NpgsqlDbType.Uuid, patientId);
                command.Parameters.AddWithValue("pvar_questionnairetemplate", NpgsqlDbType.Uuid, templateId);
                command.Parameters.AddWithValue("pvar_taskname", NpgsqlDbType.Varchar, (object)taskname ?? string.Empty);
                command.Parameters.AddWithValue("pvar_opdform", NpgsqlDbType.Uuid,
                    hasCurrentOpd ? (object)currentOpdId : DBNull.Value);
                command.Parameters.AddWithValue("pvar_ipdform", NpgsqlDbType.Uuid,
                    hasCurrentIpd ? (object)currentIpdId : DBNull.Value);
                var result = new DataTable();
                using (var adapter = new NpgsqlDataAdapter(command))
                    adapter.Fill(result);
                return result;
            }
        }

        public virtual System.Data.DataTable getLatestAssessmentForPrefill(
            string patientname, string taskname = "", string opdform = "", string ipdform = "")
        {
            if (!Guid.TryParse(patientname, out var patientId) || patientId == Guid.Empty)
                return new DataTable();

            Guid currentOpdId;
            var hasCurrentOpd = Guid.TryParse(opdform, out currentOpdId) && currentOpdId != Guid.Empty;
            Guid currentIpdId;
            var hasCurrentIpd = Guid.TryParse(ipdform, out currentIpdId) && currentIpdId != Guid.Empty;

            using (var connection = new NpgsqlConnection(db_connectionstring))
            using (var command = new NpgsqlCommand(
                "SELECT * FROM \"Get_Latest_Assessment_For_Prefill\"(@pvar_patientname,@pvar_taskname,@pvar_opdform,@pvar_ipdform)",
                connection))
            {
                command.Parameters.AddWithValue("pvar_patientname", NpgsqlDbType.Uuid, patientId);
                command.Parameters.AddWithValue("pvar_taskname", NpgsqlDbType.Varchar, (object)taskname ?? string.Empty);
                command.Parameters.AddWithValue("pvar_opdform", NpgsqlDbType.Uuid,
                    hasCurrentOpd ? (object)currentOpdId : DBNull.Value);
                command.Parameters.AddWithValue("pvar_ipdform", NpgsqlDbType.Uuid,
                    hasCurrentIpd ? (object)currentIpdId : DBNull.Value);
                var result = new DataTable();
                using (var adapter = new NpgsqlDataAdapter(command))
                    adapter.Fill(result);
                return result;
            }
        }
        // ────────────────────────────────────────────────────────────────────────────

        public virtual System.Data.DataTable getAssessments_by_ipdform(string ipdapplicationformid)
        {
            DataTable dataTable = new DataTable();
            DataSet dataSet = new DataSet();
            try
            {
                using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
                {
                    npsql.Open();
                    using (var dbCommand = new NpgsqlCommand(
                        "SELECT * FROM \"getAssessments_by_ipdform\"(@pvar_ipdapplicationformid)", npsql))
                    {
                        dbCommand.CommandType = CommandType.Text;
                        dbCommand.Parameters.AddWithValue("pvar_ipdapplicationformid", (object)ipdapplicationformid ?? DBNull.Value);
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

		public virtual System.Data.DataTable Get_IPD_Assessment_Task_State(string ipdapplicationformid)
		{
			var table = new DataTable();
			using (var connection = new NpgsqlConnection(db_connectionstring))
			{
				connection.Open();
				using (var command = new NpgsqlCommand(
					"SELECT * FROM \"Get_IPD_Assessment_Task_State\"(@pvar_ipdapplicationformid)", connection))
				{
					command.Parameters.AddWithValue("pvar_ipdapplicationformid", NpgsqlDbType.Uuid, new Guid(ipdapplicationformid));
					using (var adapter = new NpgsqlDataAdapter(command)) adapter.Fill(table);
				}
			}
			return table;
		}

        public virtual System.Data.DataTable get_Assessments_for_IPD(string ipdapplicationformid)
        {
            DataTable dataTable = new DataTable();
            DataSet dataSet = new DataSet();
            try
            {
                using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
                {
                    npsql.Open();
                    using (var dbCommand = new NpgsqlCommand(
                        "SELECT * FROM \"get_Assessments_for_IPD\"(@pvar_ipdapplicationformid)", npsql))
                    {
                        dbCommand.CommandType = CommandType.Text;
                        dbCommand.Parameters.AddWithValue("pvar_ipdapplicationformid", (object)ipdapplicationformid ?? DBNull.Value);
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

public virtual System.Data.DataTable prefill_Assessment_assessmentquestions(string questionnairetemplate)
						{
							DataTable dataTable=new DataTable();
							DataSet dataSet=new DataSet();
							if (!Guid.TryParse(questionnairetemplate, out Guid questionnaireTemplateId))
								return dataTable;
							try
							{
								  

									using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
									{
										npsql.Open();
										using (var dbCommand = new NpgsqlCommand(
											"SELECT * FROM \"prefill_Assessment_assessmentquestions\"(@pvar_questionnairetemplate)", npsql))
										{
											dbCommand.CommandType = CommandType.Text;
											dbCommand.Parameters.AddWithValue("pvar_questionnairetemplate", NpgsqlDbType.Uuid, questionnaireTemplateId);
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

		private static bool PreservesIpdBookingStatus(string taskName)
		{
			var normalizedTaskName = new string((taskName ?? "").Where(char.IsLetterOrDigit).Select(char.ToLowerInvariant).ToArray());
			return string.Equals(taskName?.Trim(), "Feedbackform", StringComparison.OrdinalIgnoreCase)
				|| normalizedTaskName.Contains("dischargechecklist");
		}

		private static Tuple<string, DateTime?> CaptureIpdBookingStatus(
			NpgsqlConnection connection, NpgsqlTransaction transaction, AssessmentModel model)
		{
			if (!PreservesIpdBookingStatus(model?.taskname) || !model.ipdform.HasValue || model.ipdform.Value == Guid.Empty)
				return null;

			using (var command = new NpgsqlCommand(
				"SELECT * FROM \"Get_IPD_Booking_Status_For_Assessment\"(@pvar_ipdapplicationformid)",
				connection, transaction))
			{
				command.Parameters.AddWithValue("pvar_ipdapplicationformid", NpgsqlDbType.Uuid, model.ipdform.Value);
				using (var reader = command.ExecuteReader())
				{
					if (!reader.Read()) return null;
					var status = reader.IsDBNull(0) ? null : reader.GetString(0);
					var statusDate = reader.IsDBNull(1) ? (DateTime?)null : reader.GetDateTime(1);
					return Tuple.Create(status, statusDate);
				}
			}
		}

		private static void RestoreIpdBookingStatus(
			NpgsqlConnection connection, NpgsqlTransaction transaction, AssessmentModel model,
			Tuple<string, DateTime?> originalStatus)
		{
			if (originalStatus == null || !model.ipdform.HasValue) return;
			using (var command = new NpgsqlCommand(
				"SELECT * FROM \"Restore_IPD_Booking_Status_For_Assessment\"(@pvar_ipdapplicationformid,@pvar_bookingstatus,@pvar_bookingstatusdate)",
				connection, transaction))
			{
				command.Parameters.AddWithValue("pvar_ipdapplicationformid", NpgsqlDbType.Uuid, model.ipdform.Value);
				command.Parameters.AddWithValue("pvar_bookingstatus", NpgsqlDbType.Varchar, (object)originalStatus.Item1 ?? DBNull.Value);
				command.Parameters.AddWithValue("pvar_bookingstatusdate", NpgsqlDbType.Timestamp, (object)originalStatus.Item2 ?? DBNull.Value);
				command.ExecuteNonQuery();
			}
		}

              public virtual string Add_Assessment(AssessmentModel model)
			  { 
				  String ResponseMessage="";
					 
					try{
							 
                            using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
					        {
						        npsql.Open();
								using (var transaction = npsql.BeginTransaction())
								{
								var originalIpdStatus = CaptureIpdBookingStatus(npsql, transaction, model);
						        using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"Add_Assessment\"(@pvar_assessmentid,@pvar_tenantid,@pvar_patientname,@pvar_patientvisit,@pvar_assessedby,@pvar_doctorname,@pvar_ipdform,@pvar_opdform,@pvar_assessmentdate,@pvar_questionnairetemplate,@pvar_assessmentnotes,@pvar_assessmentquestions,@pvar_eligibleforfinaladmission,@pvar_createduser,@pvar_taskname)", npsql))
						        {
										dbCommand.Transaction = transaction;
                                        dbCommand.CommandType = CommandType.Text;
						            	
								        					dbCommand.Parameters.AddWithValue("pvar_assessmentid",NpgsqlDbType.Uuid,(object)model.Assessmentid??DBNull.Value);
dbCommand.Parameters.AddWithValue("pvar_tenantid",NpgsqlDbType.Uuid,(object)model.tenantid??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_patientname",NpgsqlDbType.Uuid,(object)model.patientname??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_patientvisit",NpgsqlDbType.Uuid,(object)model.patientvisit??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_assessedby",NpgsqlDbType.Varchar,(object)model.assessedby??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_doctorname",NpgsqlDbType.Uuid,(object)model.doctorname??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_ipdform",NpgsqlDbType.Uuid,(object)model.ipdform??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_opdform",NpgsqlDbType.Uuid,(object)model.opdform??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_assessmentdate",NpgsqlDbType.Timestamp,(object)model.assessmentdate??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_questionnairetemplate",NpgsqlDbType.Uuid,(object)model.questionnairetemplate??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_assessmentnotes",NpgsqlDbType.Varchar,(object)model.assessmentnotes??DBNull.Value);
if(model.assessmentquestions !=null  && model.assessmentquestions.Count >0)
dbCommand.Parameters.AddWithValue("pvar_assessmentquestions",NpgsqlDbType.Json,JsonConvert.SerializeObject(model.assessmentquestions));
else
dbCommand.Parameters.AddWithValue("pvar_assessmentquestions",NpgsqlDbType.Json,DBNull.Value);
						dbCommand.Parameters.AddWithValue("pvar_eligibleforfinaladmission", NpgsqlDbType.Varchar, (object)model.eligibleforfinaladmission ?? DBNull.Value);

						dbCommand.Parameters.AddWithValue("pvar_createduser",NpgsqlDbType.Uuid,(object)model.createduser??DBNull.Value);
						dbCommand.Parameters.AddWithValue("pvar_taskname", NpgsqlDbType.Varchar, (object)model.taskname ?? DBNull.Value);

						


										NpgsqlParameter outParm = new NpgsqlParameter("pvar_returnMessage", NpgsqlDbType.Varchar)
                                        {
                                             Direction = ParameterDirection.Output
                                        };
                                        dbCommand.Parameters.Add(outParm);

                                        dbCommand.ExecuteNonQuery();
								        ResponseMessage = outParm.Value.ToString();
							        }
								RestoreIpdBookingStatus(npsql, transaction, model, originalIpdStatus);
								if (IsSuccessfulAssessmentSave(ResponseMessage))
								{
									AppendAssessmentHistory(npsql, transaction, model, model.createduser.Value, "CREATED");
									transaction.Commit();
								}
								else
								{
									transaction.Rollback();
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
public virtual AssessmentModel getById_Assessment(string Assessmentid)
									 {
										DataTable dataTable = new DataTable();
										DataSet dataSet = new DataSet();
										try{
												 
												using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
												{
													npsql.Open();
													using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"getById_sp_Assessment\"(@pvar_assessmentid)", npsql))
													{
														dbCommand.CommandType = CommandType.Text;
														dbCommand.Parameters.AddWithValue("pvar_assessmentid",(object)Assessmentid??DBNull.Value);
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
											return ModelConverter.ConvertDataRowToModel<AssessmentModel>(row);
										}
										else
										{
											return null;
										}
									 }
			 public virtual string  Update_Assessment(AssessmentModel model)
			 { 
				 String ResponseMessage="";
					try{
						 	 
							using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
							{
								npsql.Open();
								using (var transaction = npsql.BeginTransaction())
								{
								var originalIpdStatus = CaptureIpdBookingStatus(npsql, transaction, model);
								using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"Update_Assessment\"(@pvar_assessmentid,@pvar_tenantid,@pvar_patientname,@pvar_patientvisit,@pvar_assessedby,@pvar_doctorname,@pvar_ipdform,@pvar_opdform,@pvar_assessmentdate,@pvar_questionnairetemplate,@pvar_assessmentnotes,@pvar_assessmentquestions,@pvar_modifieduser,@pvar_eligibleforfinaladmission,@pvar_taskname)", npsql))
								{
										dbCommand.Transaction = transaction;
										dbCommand.CommandType = CommandType.Text;
															dbCommand.Parameters.AddWithValue("pvar_assessmentid",NpgsqlDbType.Uuid,(object)model.Assessmentid??DBNull.Value);
dbCommand.Parameters.AddWithValue("pvar_tenantid",NpgsqlDbType.Uuid,(object)model.tenantid??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_patientname",NpgsqlDbType.Uuid,(object)model.patientname??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_patientvisit",NpgsqlDbType.Uuid,(object)model.patientvisit??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_assessedby",NpgsqlDbType.Varchar,(object)model.assessedby??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_doctorname",NpgsqlDbType.Uuid,(object)model.doctorname??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_ipdform",NpgsqlDbType.Uuid,(object)model.ipdform??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_opdform",NpgsqlDbType.Uuid,(object)model.opdform??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_assessmentdate",NpgsqlDbType.Timestamp,(object)model.assessmentdate??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_questionnairetemplate",NpgsqlDbType.Uuid,(object)model.questionnairetemplate??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_assessmentnotes",NpgsqlDbType.Varchar,(object)model.assessmentnotes??DBNull.Value);
if(model.assessmentquestions !=null  && model.assessmentquestions.Count >0)
dbCommand.Parameters.AddWithValue("pvar_assessmentquestions",NpgsqlDbType.Json,JsonConvert.SerializeObject(model.assessmentquestions));
else
dbCommand.Parameters.AddWithValue("pvar_assessmentquestions",NpgsqlDbType.Json,DBNull.Value);
dbCommand.Parameters.AddWithValue("pvar_modifieduser",NpgsqlDbType.Uuid,model.modifieduser);
						dbCommand.Parameters.AddWithValue("pvar_eligibleforfinaladmission", NpgsqlDbType.Varchar, (object)model.eligibleforfinaladmission ?? DBNull.Value);
						dbCommand.Parameters.AddWithValue("pvar_taskname", NpgsqlDbType.Varchar, (object)model.taskname ?? DBNull.Value);

						NpgsqlParameter outParm = new NpgsqlParameter("@returnMessage", NpgsqlDbType.Varchar)
										{
											 Direction = ParameterDirection.Output
										};
										dbCommand.Parameters.Add(outParm);

										dbCommand.ExecuteNonQuery();
										ResponseMessage = outParm.Value.ToString();
								}
								RestoreIpdBookingStatus(npsql, transaction, model, originalIpdStatus);
								if (IsSuccessfulAssessmentSave(ResponseMessage))
								{
									AppendAssessmentHistory(npsql, transaction, model, model.modifieduser.Value, "UPDATED");
									transaction.Commit();
								}
								else
								{
									transaction.Rollback();
								}
								}
								npsql.Close();
							}		 

					}catch(Exception ex){
						ResponseMessage=ex.Message;
					}
					
					return ResponseMessage;

			   }
public virtual string  Remove_Assessment(string id,string loginUserID)
			  { 
				  String ResponseMessage="";
					try{ 
							using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
							{
								npsql.Open();
								using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"Remove_Assessment\"(@pvar_assessmentid,@pvar_modifieduser)", npsql))
								{
										dbCommand.CommandType = CommandType.Text;
										dbCommand.Parameters.AddWithValue("pvar_assessmentid",(object)id??DBNull.Value);
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
public virtual JObject Assessment_List(string tenantid
,string patientname
,string patientvisit
,string assessmentdate_automatonfrom
,string assessmentdate_automatonto
, int? pagesize=1000 , int? pagenumber=0,string searchterm="",string  sort_fields="")
			  { 
				  object dalResponse = null;
			
					try{
 
							using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
							{
								
								using (var dbCommand = new NpgsqlCommand("SELECT * FROM\"Assessment_List\"(@pvar_tenantid,@pvar_patientname,@pvar_patientvisit,@pvar_assessmentdate_automatonfrom,@pvar_assessmentdate_automatonto,@pvar_pagesize,@pvar_pagenumber,@pvar_searchterm,@pvar_sort_fields)", npsql))
								{
									dbCommand.CommandType = CommandType.Text;
									dbCommand.Parameters.AddWithValue("pvar_tenantid",(object)tenantid??DBNull.Value);
dbCommand.Parameters.AddWithValue("pvar_patientname",(object)patientname??DBNull.Value);
dbCommand.Parameters.AddWithValue("pvar_patientvisit",(object)patientvisit??DBNull.Value);
dbCommand.Parameters.AddWithValue("pvar_assessmentdate_automatonfrom",(object)assessmentdate_automatonfrom??DBNull.Value);
dbCommand.Parameters.AddWithValue("pvar_assessmentdate_automatonto",(object)assessmentdate_automatonto??DBNull.Value);

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
			   
public virtual System.Data.DataTable Patient_Profile_Assessments(string tenantid, string patientname)
			  {
				    DataTable dataTable = new DataTable();

					try{
							using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
							{
								using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"Patient_Profile_Assessments\"(@pvar_tenantid,@pvar_patientname)", npsql))
								{
									dbCommand.CommandType = CommandType.Text;
									dbCommand.Parameters.AddWithValue("pvar_tenantid",(object)tenantid??DBNull.Value);
									dbCommand.Parameters.AddWithValue("pvar_patientname",(object)patientname??DBNull.Value);
									npsql.Open();
									using (NpgsqlDataAdapter dataAdapter = new NpgsqlDataAdapter(dbCommand))
									{
										dataAdapter.Fill(dataTable);
									}
									npsql.Close();
								}
							}
					}catch{
						throw;
					}

					return dataTable;
			  }
			   
			 
public virtual System.Data.DataTable get_all_Assessment(string tenantid,string searchterm="", int? pagesize=1000, int? pagenumber=0)
			  { 

				    DataTable dataTable = new DataTable();
					DataSet dataSet = new DataSet();

					try{
 
							using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
							{
								npsql.Open();
								using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"get_all_Assessment\"(@pvar_tenantid)", npsql))
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
public virtual string getAssessmentByOpdForm(string opdformid)
			  {
					try {
						using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
						{
							npsql.Open();
							using (var dbCommand = new NpgsqlCommand(
								"SELECT * FROM \"Get_Assessment_By_OPDForm\"(@pvar_opdform)", npsql))
							{
								dbCommand.CommandType = CommandType.Text;
								dbCommand.Parameters.AddWithValue("pvar_opdform", NpgsqlTypes.NpgsqlDbType.Uuid, new Guid(opdformid));
								var result = dbCommand.ExecuteScalar();
								if (dbCommand.Connection.State != System.Data.ConnectionState.Closed)
									dbCommand.Connection.Dispose();
								return result?.ToString() ?? "";
							}
						}
					} catch { return ""; }
			  }

private void SetAssessmentPatientNameFallback(NpgsqlConnection npsql, DataTable dataTable, string Assessmentid)
{
	if (dataTable == null || dataTable.Rows.Count == 0)
		return;

	if (!dataTable.Columns.Contains("patientname"))
		dataTable.Columns.Add("patientname", typeof(string));

	var patientName = Convert.ToString(dataTable.Rows[0]["patientname"])?.Trim();
	if (!string.IsNullOrWhiteSpace(patientName) && patientName.Trim() != "-")
		return;

	using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"Get_Assessment_Patient_Display_Name\"(@pvar_assessmentid)", npsql))
	{
		dbCommand.CommandType = CommandType.Text;
		dbCommand.Parameters.AddWithValue("pvar_assessmentid", (object)Assessmentid ?? DBNull.Value);
		var fallbackName = Convert.ToString(dbCommand.ExecuteScalar())?.Trim();
		if (!string.IsNullOrWhiteSpace(fallbackName))
			dataTable.Rows[0]["patientname"] = fallbackName;
	}
}

public virtual System.Data.DataTable getById_allinfo_Assessment(string Assessmentid)
			 {
				DataSet dataSet=new DataSet();
				DataTable dataTable = new DataTable();
				try{
					     
						using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
						{
							npsql.Open();
							using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"getById_sp_all_Assessment\"(@pvar_assessmentid)", npsql))
							{
								dbCommand.CommandType = CommandType.Text;
								dbCommand.Parameters.AddWithValue("pvar_assessmentid",(object)Assessmentid??DBNull.Value);
								using (NpgsqlDataAdapter dataAdapter = new NpgsqlDataAdapter(dbCommand))
								{
									dataSet.Reset();
									dataAdapter.Fill(dataSet);
									dataTable = dataSet.Tables[0];
									SetAssessmentPatientNameFallback(npsql, dataTable, Assessmentid);
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
			  
public virtual System.Data.DataTable lookup_Assessment_patientname(String tenantid,string searchterm="", int? pagesize=1000, int? pagenumber=0)
							        {
                                            DataSet dataSet = new DataSet();
									        DataTable dataTable=new DataTable();
									        try{

                                        		        using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
					                                    {
						                                    npsql.Open();
						                                    using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"lookup_Assessment_patientname\"(@pvar_tenantid,@pvar_searchterm,@pvar_pagesize,@pvar_pagenumber)", npsql))
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
public virtual System.Data.DataTable lookup_Assessment_patientvisit(String tenantid,String patientname,string searchterm="", int? pagesize=1000, int? pagenumber=0)
							        {
                                            DataSet dataSet = new DataSet();
									        DataTable dataTable=new DataTable();
									        try{

                                        		        using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
					                                    {
						                                    npsql.Open();
						                                    using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"lookup_Assessment_patientvisit\"(@pvar_tenantid,@pvar_patientname,@pvar_searchterm,@pvar_pagesize,@pvar_pagenumber)", npsql))
						                                    {
                                            
                                                                dbCommand.Parameters.AddWithValue("pvar_tenantid",(object)tenantid??DBNull.Value);dbCommand.Parameters.AddWithValue("pvar_patientname",(object)patientname??DBNull.Value);  
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
public virtual System.Data.DataTable lookup_Assessment_doctorname(String tenantid,string searchterm="", int? pagesize=1000, int? pagenumber=0)
							        {
                                            DataSet dataSet = new DataSet();
									        DataTable dataTable=new DataTable();
									        try{

                                        		        using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
					                                    {
						                                    npsql.Open();
						                                    using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"lookup_Assessment_doctorname\"(@pvar_tenantid,@pvar_searchterm,@pvar_pagesize,@pvar_pagenumber)", npsql))
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
public virtual System.Data.DataTable lookup_Assessment_ipdform(String tenantid,String patientname,string searchterm="", int? pagesize=1000, int? pagenumber=0)
							        {
                                            DataSet dataSet = new DataSet();
									        DataTable dataTable=new DataTable();
									        try{

                                        		        using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
					                                    {
						                                    npsql.Open();
						                                    using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"lookup_Assessment_ipdform\"(@pvar_tenantid,@pvar_patientname,@pvar_searchterm,@pvar_pagesize,@pvar_pagenumber)", npsql))
						                                    {
                                            
                                                                dbCommand.Parameters.AddWithValue("pvar_tenantid",(object)tenantid??DBNull.Value);dbCommand.Parameters.AddWithValue("pvar_patientname",(object)patientname??DBNull.Value);  
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
public virtual System.Data.DataTable lookup_Assessment_opdform(String tenantid,String patientname,String doctorname,string searchterm="", int? pagesize=1000, int? pagenumber=0)
							        {
                                            DataSet dataSet = new DataSet();
									        DataTable dataTable=new DataTable();
									        try{

                                        		        using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
					                                    {
						                                    npsql.Open();
						                                    using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"lookup_Assessment_opdform\"(@pvar_tenantid,@pvar_patientname,@pvar_preferreddoctor,@pvar_searchterm,@pvar_pagesize,@pvar_pagenumber)", npsql))
						                                    {
                                            
                                                                dbCommand.Parameters.AddWithValue("pvar_tenantid",(object)tenantid??DBNull.Value);dbCommand.Parameters.AddWithValue("pvar_patientname",(object)patientname??DBNull.Value);dbCommand.Parameters.AddWithValue("pvar_preferreddoctor",(object)doctorname??DBNull.Value);  
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
public virtual System.Data.DataTable get_Assessment_suggested_templates(String ipdapplicationformid, String userrole, String taskname = "")
                                {
                                    DataSet dataSet = new DataSet();
                                    DataTable dataTable = new DataTable();
                                    try
                                    {
                                        using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
                                        {
                                            npsql.Open();
                                            using (var dbCommand = new NpgsqlCommand("SELECT * FROM get_Assessment_suggested_templates(@p_ipd, @p_role, @p_taskname)", npsql))
                                            {
                                                dbCommand.Parameters.AddWithValue("p_ipd", string.IsNullOrWhiteSpace(ipdapplicationformid) ? (object)DBNull.Value : new Guid(ipdapplicationformid));
                                                dbCommand.Parameters.AddWithValue("p_role", (object)(userrole ?? "") ?? DBNull.Value);
                                                dbCommand.Parameters.AddWithValue("p_taskname", (object)(taskname ?? "") ?? DBNull.Value);
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
                                    catch { throw; }
                                    return dataTable;
                                }
public virtual System.Data.DataTable get_opd_assessmenttemplates(String opdformid, String taskname)
                                {
                                    DataSet dataSet = new DataSet();
                                    DataTable dataTable = new DataTable();
                                    try
                                    {
                                        using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
                                        {
                                            npsql.Open();
                                            using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"get_opd_assessmenttemplates\"(@pvar_opdformid,@pvar_taskname)", npsql))
                                            {
                                                dbCommand.Parameters.AddWithValue("pvar_opdformid", NpgsqlDbType.Uuid, string.IsNullOrWhiteSpace(opdformid) ? (object)DBNull.Value : new Guid(opdformid));
                                                dbCommand.Parameters.AddWithValue("pvar_taskname", NpgsqlDbType.Varchar, (object)taskname ?? DBNull.Value);
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
                                    catch { throw; }
                                    return dataTable;
                                }
public virtual System.Data.DataTable lookup_Assessment_questionnairetemplate(String tenantid)
							        {
                                            DataSet dataSet = new DataSet();
									        DataTable dataTable=new DataTable();
									        try{

                                        		        using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
					                                    {
						                                    npsql.Open();
						                                    using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"lookup_Assessment_questionnairetemplate\"(@pvar_tenantid)", npsql))
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
public virtual System.Data.DataTable lookup_Assessment_assessmentquestions_questions(String tenantid)
							        {
                                            DataSet dataSet = new DataSet();
									        DataTable dataTable=new DataTable();
									        try{

                                        		        using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
					                                    {
						                                    npsql.Open();
						                                    using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"lookup_Assessment_assessmentquestions_questions\"(@pvar_tenantid)", npsql))
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
public virtual System.Data.DataTable lookup_Assessment_assessmentquestions_questioncategory(String tenantid)
							        {
                                            DataSet dataSet = new DataSet();
									        DataTable dataTable=new DataTable();
									        try{

                                        		        using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
					                                    {
						                                    npsql.Open();
						                                    using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"lookup_Assessment_assessmentquestions_questioncategory\"(@pvar_tenantid)", npsql))
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
public virtual System.Data.DataTable lookup_Assessment_assessmentquestions_questionsub(String tenantid,String questioncategory)
							        {
                                            DataSet dataSet = new DataSet();
									        DataTable dataTable=new DataTable();
									        try{

                                        		        using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
					                                    {
						                                    npsql.Open();
						                                    using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"lookup_Assessment_assessmentquestions_questionsub\"(@pvar_tenantid,@pvar_questioncategoryname)", npsql))
						                                    {
                                            
                                                                dbCommand.Parameters.AddWithValue("pvar_tenantid",(object)tenantid??DBNull.Value);dbCommand.Parameters.AddWithValue("pvar_questioncategoryname",(object)questioncategory??DBNull.Value);  
																
																 
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
public virtual System.Data.DataTable lookup_Assessment_assessmentquestions_question(String tenantid,String questioncategory,String questionsub)
							        {
                                            DataSet dataSet = new DataSet();
									        DataTable dataTable=new DataTable();
									        try{

                                        		        using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
					                                    {
						                                    npsql.Open();
						                                    using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"lookup_Assessment_assessmentquestions_question\"(@pvar_tenantid,@pvar_questioncategory,@pvar_questionsubcategory)", npsql))
						                                    {
                                            
                                                                dbCommand.Parameters.AddWithValue("pvar_tenantid",(object)tenantid??DBNull.Value);dbCommand.Parameters.AddWithValue("pvar_questioncategory",(object)questioncategory??DBNull.Value);dbCommand.Parameters.AddWithValue("pvar_questionsubcategory",(object)questionsub??DBNull.Value);  
																
																 
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


public virtual System.Data.DataTable lookup_change_assessmentquestions_Assessment_question(string AssessmentQuestionid)
						{
							DataTable dataTable=new DataTable();
                            DataSet dataSet=new DataSet();
								try
								{
								 
                                         using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
                                        {
	                                        npsql.Open();
	                                        using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"lookup_change_assessmentquestions_Assessment_question\"(@pvar_assessmentquestionid)", npsql))
	                                        {
		                                        dbCommand.CommandType = CommandType.Text;
		                                        dbCommand.Parameters.AddWithValue("pvar_assessmentquestionid",NpgsqlDbType.Varchar,(object)AssessmentQuestionid??DBNull.Value);
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





			    }


			    }
