namespace NalamVazha.DAL{
			    using System;
			    using System.Text;
			    using System.Data;
			    using System.Data.Common;
				using System.Collections.Generic;
			    using NalamVazha.Models;
			    using EncrypDecrypt;
			    using Newtonsoft.Json;
				using Newtonsoft.Json.Linq;
                using Npgsql;
				using NpgsqlTypes;
				using System.Text.RegularExpressions;

			    //This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 04/21/2026 05:37:58
			    public class ReceivableDAL
			    {
					public virtual string db_connectionstring{get;set;}
					
			 	    public ReceivableDAL(string connectionString)
				    {
						
					    db_connectionstring=connectionString;
				    }

		private void AppendIpdStatusToReceivableLookup(NpgsqlConnection npsql, DataTable dataTable)
		{
			if (dataTable == null || dataTable.Rows.Count == 0)
				return;

			var ipdIdColumn = dataTable.Columns.Contains("IPDApplicationFormid") ? "IPDApplicationFormid"
				: dataTable.Columns.Contains("ipdapplicationformid") ? "ipdapplicationformid"
				: string.Empty;

			if (string.IsNullOrEmpty(ipdIdColumn))
				return;

			if (!dataTable.Columns.Contains("bookingstatus"))
				dataTable.Columns.Add("bookingstatus", typeof(string));

			if (!dataTable.Columns.Contains("verifiedstatus"))
				dataTable.Columns.Add("verifiedstatus", typeof(string));

			using (var statusCommand = new NpgsqlCommand("SELECT * FROM \"AppendIpdStatusToReceivableLookup\"(@pvar_ipdapplicationformid)", npsql))
			{
				var ipdIdParameter = statusCommand.Parameters.Add("pvar_ipdapplicationformid", NpgsqlDbType.Uuid);
				foreach (DataRow row in dataTable.Rows)
				{
					Guid ipdId;
					if (!Guid.TryParse(Convert.ToString(row[ipdIdColumn]), out ipdId))
						continue;

					ipdIdParameter.Value = ipdId;
					using (var reader = statusCommand.ExecuteReader())
					{
						if (reader.Read())
						{
							row["bookingstatus"] = reader["bookingstatus"] == DBNull.Value ? string.Empty : Convert.ToString(reader["bookingstatus"]);
							row["verifiedstatus"] = reader["verifiedstatus"] == DBNull.Value ? string.Empty : Convert.ToString(reader["verifiedstatus"]);
						}
					}
				}
			}
		}
				  
			        
              public virtual string Add_Receivable(ReceivableModel model)
			  { 
				  String ResponseMessage="";
					 
					try{
							NormalizeReceivableAmountSign(model);
							 
                            using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
					        {
								npsql.Open();
								var isConcessionReceivable = IsConcessionReceivable(model.receivablefor);
								if (isConcessionReceivable)
								{
									return "Concession receivables are created automatically when the concession form is approved.";
								}
								if (IsDirectAdmissionIpd(npsql, model.ipdnumber) && IsDirectAdmissionRestrictedReceivableFor(model.receivablefor))
								{
									return "This receivable type is not applicable for Direct Admission IPD.";
								}
								//if (isConcessionReceivable)
								//{
								//	if (TryApplyNegativeReceivableAdjustment(npsql, model, out ResponseMessage))
								//	{
								//		return ResponseMessage;
								//	}
								//}
								//if (model.amount < 0 && TryApplyNegativeReceivableAdjustment(npsql, model, out ResponseMessage))
								//{
								//	return ResponseMessage;
								//}
						        using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"Add_Receivable\"(@pvar_receivableid,@pvar_tenantid,@pvar_receivableno,@pvar_receivabledate,@pvar_patientname,@pvar_patientvisit,@pvar_ipdnumber,@pvar_opdnumber,@pvar_receivablefor,@pvar_specifyothers,@pvar_therapy,@pvar_therapycost,@pvar_therapykit,@pvar_kitprice,@pvar_medicine,@pvar_price,@pvar_package,@pvar_room,@pvar_amount,@pvar_remarks,@pvar_createduser,@pvar_paidamount,@pvar_paymentstatus,@pvar_billingpaymentid)", npsql))
						        {
                                        dbCommand.CommandType = CommandType.Text;
						            	
								        					dbCommand.Parameters.AddWithValue("pvar_receivableid",NpgsqlDbType.Uuid,(object)model.Receivableid??DBNull.Value);
dbCommand.Parameters.AddWithValue("pvar_tenantid",NpgsqlDbType.Uuid,(object)model.tenantid??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_receivableno",NpgsqlDbType.Varchar,(object)model.receivableno??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_receivabledate",NpgsqlDbType.Date,(object)model.receivabledate??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_patientname",NpgsqlDbType.Uuid,(object)model.patientname??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_patientvisit",NpgsqlDbType.Uuid,(object)model.patientvisit??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_ipdnumber",NpgsqlDbType.Uuid,(object)model.ipdnumber??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_opdnumber",NpgsqlDbType.Uuid,(object)model.opdnumber??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_receivablefor",NpgsqlDbType.Varchar,(object)model.receivablefor??DBNull.Value);

						dbCommand.Parameters.AddWithValue("pvar_specifyothers", NpgsqlDbType.Varchar, (object)model.specifyothers ?? DBNull.Value);

						dbCommand.Parameters.AddWithValue("pvar_therapy",NpgsqlDbType.Uuid,(object)model.therapy??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_therapycost",NpgsqlDbType.Numeric,(object)model.therapycost??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_therapykit",NpgsqlDbType.Uuid,(object)model.therapykit??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_kitprice",NpgsqlDbType.Varchar,(object)model.kitprice??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_medicine",NpgsqlDbType.Uuid,(object)model.medicine??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_price",NpgsqlDbType.Numeric,(object)model.price??DBNull.Value);

					 
						dbCommand.Parameters.AddWithValue("pvar_package",NpgsqlDbType.Uuid,(object)model.package??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_room",NpgsqlDbType.Uuid,(object)model.room??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_amount",NpgsqlDbType.Numeric,(object)model.amount??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_remarks",NpgsqlDbType.Varchar,(object)model.remarks??DBNull.Value);
dbCommand.Parameters.AddWithValue("pvar_createduser",NpgsqlDbType.Uuid,(object)model.createduser??DBNull.Value);


						dbCommand.Parameters.AddWithValue("pvar_paidamount", NpgsqlDbType.Numeric, (object)model.paidamount ?? DBNull.Value);

						dbCommand.Parameters.AddWithValue("pvar_paymentstatus", NpgsqlDbType.Varchar, (object)model.paymentstatus ?? DBNull.Value);

						dbCommand.Parameters.AddWithValue("pvar_billingpaymentid", NpgsqlDbType.Uuid, (object)model.billingpaymentid ?? DBNull.Value);

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

		private bool TryApplyNegativeReceivableAdjustment(NpgsqlConnection npsql, ReceivableModel model, out string responseMessage)
		{
			responseMessage = "";
			using (var transaction = npsql.BeginTransaction())
			{
				try
				{
					var receivableForMatches = GetReceivableForMatches(model.receivablefor);
					Guid? existingReceivableId = null;
					decimal existingAmount = 0;
					decimal paidAmount = 0;
					decimal balanceAmount = 0;

					using (var findCmd = new NpgsqlCommand("SELECT * FROM \"Find_Negative_Adjustment_Receivable\"(@receivablefor_values,@selected_receivablefor,@patientname,@tenantid,@ipdnumber,@opdnumber)", npsql, transaction))
					{
						findCmd.Parameters.AddWithValue("receivablefor_values", NpgsqlDbType.Array | NpgsqlDbType.Text, receivableForMatches);
						findCmd.Parameters.AddWithValue("selected_receivablefor", NpgsqlDbType.Varchar, (object)model.receivablefor ?? DBNull.Value);
						findCmd.Parameters.AddWithValue("patientname", NpgsqlDbType.Uuid, model.patientname);
						findCmd.Parameters.AddWithValue("tenantid", NpgsqlDbType.Uuid, (object)model.tenantid ?? DBNull.Value);
						findCmd.Parameters.AddWithValue("ipdnumber", NpgsqlDbType.Uuid, (object)model.ipdnumber ?? DBNull.Value);
						findCmd.Parameters.AddWithValue("opdnumber", NpgsqlDbType.Uuid, (object)model.opdnumber ?? DBNull.Value);

						using (var reader = findCmd.ExecuteReader())
						{
							if (reader.Read())
							{
								existingReceivableId = reader.GetGuid(0);
								existingAmount = reader.GetDecimal(1);
								paidAmount = reader.GetDecimal(2);
								balanceAmount = reader.GetDecimal(3);
							}
						}
					}

					if (!existingReceivableId.HasValue)
					{
						transaction.Rollback();
						return false;
					}

					var adjustmentAmount = Math.Min(Math.Abs(model.amount), balanceAmount);
					var adjustedAmount = existingAmount - adjustmentAmount;
					var adjustedStatus = adjustedAmount <= paidAmount ? "Paid" : "Pending";

					using (var updateCmd = new NpgsqlCommand("SELECT \"Apply_Negative_Adjustment_Receivable\"(@receivableid,@amount,@paymentstatus,@remarks,@modifieduser)", npsql, transaction))
					{
						updateCmd.Parameters.AddWithValue("amount", NpgsqlDbType.Numeric, adjustedAmount);
						updateCmd.Parameters.AddWithValue("paymentstatus", NpgsqlDbType.Varchar, adjustedStatus);
						updateCmd.Parameters.AddWithValue("remarks", NpgsqlDbType.Varchar, (object)model.remarks ?? DBNull.Value);
						updateCmd.Parameters.AddWithValue("modifieduser", NpgsqlDbType.Uuid, (object)model.createduser ?? DBNull.Value);
						updateCmd.Parameters.AddWithValue("receivableid", NpgsqlDbType.Uuid, existingReceivableId.Value);
						updateCmd.ExecuteNonQuery();
					}

					using (var conditionCmd = new NpgsqlCommand("SELECT \"Mark_ReceivableConditions_NonMandatory\"(@receivablefor_values,@tenantid,@modifieduser)", npsql, transaction))
					{
						conditionCmd.Parameters.AddWithValue("modifieduser", NpgsqlDbType.Uuid, (object)model.createduser ?? DBNull.Value);
						conditionCmd.Parameters.AddWithValue("receivablefor_values", NpgsqlDbType.Array | NpgsqlDbType.Text, receivableForMatches);
						conditionCmd.Parameters.AddWithValue("tenantid", NpgsqlDbType.Uuid, (object)model.tenantid ?? DBNull.Value);
						conditionCmd.ExecuteNonQuery();
					}

					transaction.Commit();
					responseMessage = "201.1";
					return true;
				}
				catch
				{
					transaction.Rollback();
					throw;
				}
			}
		}

		private string[] GetReceivableForMatches(string selectedReceivableFor)
		{
			var selected = (selectedReceivableFor ?? string.Empty).Trim();
			var values = new List<string>();
			void Add(string value)
			{
				var normalized = (value ?? string.Empty).Trim().ToLowerInvariant();
				if (normalized.Length > 0 && !values.Contains(normalized))
					values.Add(normalized);
			}

			Add(selected);

			switch (selected.ToLowerInvariant())
			{
				case "concession":
				case "discount":
				case "cancellation refund":
					Add("Room");
					Add("Therapy");
					Add("Medicine");
					Add("Therapy Kit");
					Add("IPD Booking Deposit");
					Add("IPD Booking Deposit - Patient");
					Add("IPD Booking Deposit - Attendant");
					Add("IP Rounds");
					Add("Pre-Admission Consultation");
					break;
				case "ipd booking deposit":
				case "ipd booking deposit - patient":
				case "ipd booking advance":
					Add("IPD Booking Deposit");
					Add("IPD Booking Deposit - Patient");
					Add("IPD Booking Deposit - Attendant");
					break;
				case "ipd screening":
					Add("IPD Screening Fee");
					break;
				case "ipd consultation":
					Add("IP Rounds");
					break;
				case "ipd - misc":
				case "others":
					Add("Room");
					break;
				default:
					break;
			}

			return values.ToArray();
		}

		private void NormalizeReceivableAmountSign(ReceivableModel model)
		{
			if (model == null || string.IsNullOrWhiteSpace(model.receivablefor) || model.amount == 0)
				return;

			var receivableFor = model.receivablefor.Trim();
			if (receivableFor.Equals("Discount", StringComparison.OrdinalIgnoreCase)
				|| receivableFor.Equals("Cancellation Refund", StringComparison.OrdinalIgnoreCase)
				|| receivableFor.Equals("Concession", StringComparison.OrdinalIgnoreCase))
			{
				model.amount = -Math.Abs(model.amount);
			}
			else if (receivableFor.Equals("Penalty", StringComparison.OrdinalIgnoreCase))
			{
				model.amount = Math.Abs(model.amount);
			}
		}

		private bool IsConcessionReceivable(string receivableFor)
		{
			return string.Equals((receivableFor ?? string.Empty).Trim(), "Concession", StringComparison.OrdinalIgnoreCase);
		}

		private bool IsDirectAdmissionRestrictedReceivableFor(string receivableFor)
		{
			var value = Regex.Replace(receivableFor ?? string.Empty, @"\s+", " ").Trim().ToLowerInvariant();
			return value.Contains("ipd screening")|| value.Contains("opd")
				|| value.Contains("online op")
				|| value.Contains("op follow")
				|| value.Contains("op new")
				|| value.Contains("op consultation")
				
				;
		}

		private bool IsDirectAdmissionIpd(NpgsqlConnection npsql, Guid? ipdnumber)
		{
			if (!ipdnumber.HasValue)
				return false;

			using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"IsDirectAdmissionIpd\"(@pvar_ipdapplicationformid)", npsql))
			{
				dbCommand.Parameters.AddWithValue("pvar_ipdapplicationformid", NpgsqlDbType.Uuid, ipdnumber.Value);
				return Convert.ToBoolean(dbCommand.ExecuteScalar());
			}
		}

        private bool ValidateConcessionAmount(NpgsqlConnection npsql, ReceivableModel model, out string responseMessage)
        {
            responseMessage = "";

            if (!model.ipdnumber.HasValue || model.patientname == Guid.Empty)
            {
                responseMessage = "Approved concession can be applied only for an IPD patient.";
                return false;
            }

            // Accept a positive UI/API value, but keep concessions negative in
            // the receivable ledger so the billing calculation deducts it.
            if (model.amount > 0)
            {
                model.amount = -Math.Abs(model.amount);
            }

            if (model.amount >= 0)
            {
                responseMessage = "Concession amount must be greater than zero.";
                return false;
            }

            var approvedAmount = GetApprovedConcessionAmount(
                npsql,
                model.tenantid,
                model.patientname,
                model.ipdnumber
            );

            // Approved concession should exist and be negative
            if (approvedAmount >= 0)
            {
                responseMessage = "No approved concession amount is available for this IPD.";
                return false;
            }

            // Compare absolute values
            if (Math.Abs(model.amount) > Math.Abs(approvedAmount))
            {
                responseMessage = "Concession amount cannot exceed the approved concession amount.";
                return false;
            }

            return true;
        }

        private decimal GetApprovedConcessionAmount(NpgsqlConnection npsql, Guid? tenantid, Guid? patientname, Guid? ipdnumber)
		{
			using (var cmd = new NpgsqlCommand("SELECT * FROM \"get_Approved_Concession_Amount\"(@pvar_tenantid,@pvar_patientname,@pvar_ipdnumber)", npsql))
			{
				cmd.Parameters.AddWithValue("pvar_tenantid", NpgsqlDbType.Uuid, (object)tenantid ?? DBNull.Value);
				cmd.Parameters.AddWithValue("pvar_patientname", NpgsqlDbType.Uuid, (object)patientname ?? DBNull.Value);
				cmd.Parameters.AddWithValue("pvar_ipdnumber", NpgsqlDbType.Uuid, (object)ipdnumber ?? DBNull.Value);
				return Convert.ToDecimal(cmd.ExecuteScalar() ?? 0);
			}
		}
		public virtual System.Data.DataTable get_Approved_Concession_Amount(
	String tenantid,
	String patientname,
	String ipdnumber)
		{
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add("approvedconcessionamount", typeof(decimal));

			try
			{
				string tenantIdValue = tenantid;

				if (!string.IsNullOrWhiteSpace(tenantid) && tenantid.Contains("|"))
				{
					var tenantParts = tenantid.Split('|');
					tenantIdValue = tenantParts.Length > 1 ? tenantParts[1] : tenantParts[0];
				}

				if (!TryParseNullableGuid(tenantIdValue, out var tenantGuid)
					|| !TryParseNullableGuid(patientname, out var patientGuid)
					|| !TryParseNullableGuid(ipdnumber, out var ipdGuid)
					|| !tenantGuid.HasValue
					|| !patientGuid.HasValue
					|| !ipdGuid.HasValue)
				{
					dataTable.Rows.Add(0m);
					return dataTable;
				}

				using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
				{
					npsql.Open();

					decimal amount = GetApprovedConcessionAmount(
						npsql,
						tenantGuid.Value,
						patientGuid.Value,
						ipdGuid.Value);

					dataTable.Rows.Add(amount);
				}
			}
			catch
			{
				throw;
			}

			return dataTable;
		}
		private bool TryParseNullableGuid(string value, out Guid? guid)
		{
			guid = null;
			if (string.IsNullOrWhiteSpace(value))
			{
				return true;
			}
			if (Guid.TryParse(value, out var parsed))
			{
				guid = parsed;
				return true;
			}
			return false;
		}
public virtual ReceivableModel getById_Receivable(string Receivableid)
									 {
										DataTable dataTable = new DataTable();
										DataSet dataSet = new DataSet();
										try{
												 
												using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
												{
													npsql.Open();
													using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"getById_sp_Receivable\"(@pvar_receivableid)", npsql))
													{
														dbCommand.CommandType = CommandType.Text;
														dbCommand.Parameters.AddWithValue("pvar_receivableid",(object)Receivableid??DBNull.Value);
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
											return ModelConverter.ConvertDataRowToModel<ReceivableModel>(row);
										}
										else
										{
											return null;
										}
									 }
			 public virtual string  Update_Receivable(ReceivableModel model)
			 { 
				 String ResponseMessage="";
					try{
							NormalizeReceivableAmountSign(model);
						 	 
							using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
							{
								npsql.Open();
								using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"Update_Receivable\"(@pvar_receivableid,@pvar_tenantid,@pvar_receivableno,@pvar_receivabledate,@pvar_patientname,@pvar_patientvisit,@pvar_ipdnumber,@pvar_opdnumber,@pvar_receivablefor,@pvar_specifyothers,@pvar_therapy,@pvar_therapycost,@pvar_therapykit,@pvar_kitprice,@pvar_medicine,@pvar_price,@pvar_package,@pvar_room,@pvar_amount,@pvar_remarks,@pvar_modifieduser)", npsql))
								{
										dbCommand.CommandType = CommandType.Text;
															dbCommand.Parameters.AddWithValue("pvar_receivableid",NpgsqlDbType.Uuid,(object)model.Receivableid??DBNull.Value);
dbCommand.Parameters.AddWithValue("pvar_tenantid",NpgsqlDbType.Uuid,(object)model.tenantid??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_receivableno",NpgsqlDbType.Varchar,(object)model.receivableno??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_receivabledate",NpgsqlDbType.Date,(object)model.receivabledate??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_patientname",NpgsqlDbType.Uuid,(object)model.patientname??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_patientvisit",NpgsqlDbType.Uuid,(object)model.patientvisit??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_ipdnumber",NpgsqlDbType.Uuid,(object)model.ipdnumber??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_opdnumber",NpgsqlDbType.Uuid,(object)model.opdnumber??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_receivablefor",NpgsqlDbType.Varchar,(object)model.receivablefor??DBNull.Value);

						dbCommand.Parameters.AddWithValue("pvar_specifyothers", NpgsqlDbType.Varchar, (object)model.specifyothers ?? DBNull.Value);

						dbCommand.Parameters.AddWithValue("pvar_therapy",NpgsqlDbType.Uuid,(object)model.therapy??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_therapycost",NpgsqlDbType.Numeric,(object)model.therapycost??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_therapykit",NpgsqlDbType.Uuid,(object)model.therapykit??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_kitprice",NpgsqlDbType.Varchar,(object)model.kitprice??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_medicine",NpgsqlDbType.Uuid,(object)model.medicine??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_price",NpgsqlDbType.Numeric,(object)model.price??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_package",NpgsqlDbType.Uuid,(object)model.package??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_room",NpgsqlDbType.Uuid,(object)model.room??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_amount",NpgsqlDbType.Numeric,(object)model.amount??DBNull.Value);

dbCommand.Parameters.AddWithValue("pvar_remarks",NpgsqlDbType.Varchar,(object)model.remarks??DBNull.Value);
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
public virtual string  Remove_Receivable(string id,string loginUserID)
			  { 
				  String ResponseMessage="";
					try{ 
							using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
							{
								npsql.Open();
								using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"Remove_Receivable\"(@pvar_receivableid,@pvar_modifieduser)", npsql))
								{
										dbCommand.CommandType = CommandType.Text;
										dbCommand.Parameters.AddWithValue("pvar_receivableid",(object)id??DBNull.Value);
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
public virtual JObject Receivables(string tenantid
,string receivabledate_automatonfrom
,string receivabledate_automatonto
,string patientname
,string ipdnumber
,string opdnumber
,string receivablefor
, int? pagesize=1000 , int? pagenumber=0,string searchterm="",string  sort_fields="")
			  { 
				  object dalResponse = null;
			
					try{
 
							using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
							{
								
								using (var dbCommand = new NpgsqlCommand("SELECT * FROM\"Receivables\"(@pvar_tenantid,@pvar_receivabledate_automatonfrom,@pvar_receivabledate_automatonto,@pvar_patientname,@pvar_ipdnumber,@pvar_opdnumber,@pvar_receivablefor,@pvar_pagesize,@pvar_pagenumber,@pvar_searchterm,@pvar_sort_fields)", npsql))
								{
									dbCommand.CommandType = CommandType.Text;
									dbCommand.Parameters.AddWithValue("pvar_tenantid",(object)tenantid??DBNull.Value);
dbCommand.Parameters.AddWithValue("pvar_receivabledate_automatonfrom",(object)receivabledate_automatonfrom??DBNull.Value);
dbCommand.Parameters.AddWithValue("pvar_receivabledate_automatonto",(object)receivabledate_automatonto??DBNull.Value);
dbCommand.Parameters.AddWithValue("pvar_patientname",(object)patientname??DBNull.Value);
dbCommand.Parameters.AddWithValue("pvar_ipdnumber",(object)ipdnumber??DBNull.Value);
dbCommand.Parameters.AddWithValue("pvar_opdnumber",(object)opdnumber??DBNull.Value);
dbCommand.Parameters.AddWithValue("pvar_receivablefor",(object)receivablefor??DBNull.Value);

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
			   
			 
public virtual System.Data.DataTable get_all_Receivable(string tenantid,string searchterm="", int? pagesize=1000, int? pagenumber=0)
			  { 

				    DataTable dataTable = new DataTable();
					DataSet dataSet = new DataSet();

					try{
 
							using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
							{
								npsql.Open();
								using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"get_all_Receivable\"(@pvar_tenantid)", npsql))
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
public virtual System.Data.DataTable getById_allinfo_Receivable(string Receivableid)
			 {
				DataSet dataSet=new DataSet();
				DataTable dataTable = new DataTable();
				try{
					     
						using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
						{
							npsql.Open();
							using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"getById_sp_all_Receivable\"(@pvar_receivableid)", npsql))
							{
								dbCommand.CommandType = CommandType.Text;
								dbCommand.Parameters.AddWithValue("pvar_receivableid",(object)Receivableid??DBNull.Value);
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
			  
public virtual System.Data.DataTable lookup_Receivable_patientname(String tenantid,string searchterm="", int? pagesize=1000, int? pagenumber=0)
							        {
                                            DataSet dataSet = new DataSet();
									        DataTable dataTable=new DataTable();
									        try{

                                        		        using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
					                                    {
						                                    npsql.Open();
						                                    using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"lookup_Receivable_patientname\"(@pvar_tenantid,@pvar_searchterm,@pvar_pagesize,@pvar_pagenumber)", npsql))
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
public virtual System.Data.DataTable lookup_Receivable_patientvisit(String tenantid,String patientname,string searchterm="", int? pagesize=1000, int? pagenumber=0)
							        {
                                            DataSet dataSet = new DataSet();
									        DataTable dataTable=new DataTable();
									        try{

                                        		        using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
					                                    {
						                                    npsql.Open();
						                                    using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"lookup_Receivable_patientvisit\"(@pvar_tenantid,@pvar_patientname,@pvar_searchterm,@pvar_pagesize,@pvar_pagenumber)", npsql))
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
public virtual System.Data.DataTable lookup_Receivable_ipdnumber(String tenantid,String patientname,string searchterm="", int? pagesize=1000, int? pagenumber=0)
							        {
                                            DataSet dataSet = new DataSet();
									        DataTable dataTable=new DataTable();
									        try{

                                        		        using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
					                                    {
						                                    npsql.Open();
						                                    using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"lookup_Receivable_ipdnumber\"(@pvar_tenantid,@pvar_patientname,@pvar_searchterm,@pvar_pagesize,@pvar_pagenumber)", npsql))
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
																	AppendIpdStatusToReceivableLookup(npsql, dataTable);
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
public virtual System.Data.DataTable lookup_Receivable_opdnumber(String tenantid,String patientname,string searchterm="", int? pagesize=1000, int? pagenumber=0)
							        {
                                            DataSet dataSet = new DataSet();
									        DataTable dataTable=new DataTable();
									        try{

                                        		        using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
					                                    {
						                                    npsql.Open();
						                                    using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"lookup_Receivable_opdnumber\"(@pvar_tenantid,@pvar_patientname,@pvar_searchterm,@pvar_pagesize,@pvar_pagenumber)", npsql))
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
public virtual System.Data.DataTable lookup_Receivable_therapy(String tenantid)
							        {
                                            DataSet dataSet = new DataSet();
									        DataTable dataTable=new DataTable();
									        try{

                                        		        using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
					                                    {
						                                    npsql.Open();
						                                    using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"lookup_Receivable_therapy\"(@pvar_tenantid)", npsql))
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
public virtual System.Data.DataTable lookup_Receivable_therapykit(String tenantid)
							        {
                                            DataSet dataSet = new DataSet();
									        DataTable dataTable=new DataTable();
									        try{

                                        		        using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
					                                    {
						                                    npsql.Open();
						                                    using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"lookup_Receivable_therapykit\"(@pvar_tenantid)", npsql))
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
public virtual System.Data.DataTable lookup_Receivable_medicine(String tenantid,string searchterm="", int? pagesize=1000, int? pagenumber=0)
							        {
                                            DataSet dataSet = new DataSet();
									        DataTable dataTable=new DataTable();
									        try{

                                        		        using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
					                                    {
						                                    npsql.Open();
						                                    using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"lookup_Receivable_medicine\"(@pvar_tenantid,@pvar_searchterm,@pvar_pagesize,@pvar_pagenumber)", npsql))
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
public virtual System.Data.DataTable lookup_Receivable_package(String tenantid)
							        {
                                            DataSet dataSet = new DataSet();
									        DataTable dataTable=new DataTable();
									        try{

                                        		        using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
					                                    {
						                                    npsql.Open();
						                                    using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"lookup_Receivable_package\"(@pvar_tenantid)", npsql))
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
public virtual System.Data.DataTable lookup_Receivable_room(String tenantid)
							        {
                                            DataSet dataSet = new DataSet();
									        DataTable dataTable=new DataTable();
									        try{

                                        		        using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
					                                    {
						                                    npsql.Open();
						                                    using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"lookup_Receivable_room\"(@pvar_tenantid)", npsql))
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


						public virtual System.Data.DataTable lookup_change_Receivable_medicine(string Medicineid)
						{
							DataTable dataTable=new DataTable();
                            DataSet dataSet=new DataSet();
							try
							{
								 	 
                                    using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
									{
										npsql.Open();
										using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"lookup_change_Receivable_medicine\"(@pvar_medicineid)", npsql))
										{
											dbCommand.CommandType = CommandType.Text;
											dbCommand.Parameters.AddWithValue("pvar_medicineid",NpgsqlDbType.Varchar,(object)Medicineid??DBNull.Value);
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

						public virtual System.Data.DataTable lookup_change_Receivable_therapykit(string TherapyKitid)
						{
							DataTable dataTable=new DataTable();
                            DataSet dataSet=new DataSet();
							try
							{
								 	 
                                    using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
									{
										npsql.Open();
										using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"lookup_change_Receivable_therapykit\"(@pvar_therapykitid)", npsql))
										{
											dbCommand.CommandType = CommandType.Text;
											dbCommand.Parameters.AddWithValue("pvar_therapykitid",NpgsqlDbType.Varchar,(object)TherapyKitid??DBNull.Value);
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

						public virtual System.Data.DataTable lookup_change_Receivable_therapy(string Therapiesid)
						{
							DataTable dataTable=new DataTable();
                            DataSet dataSet=new DataSet();
							try
							{
								 	 
                                    using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
									{
										npsql.Open();
										using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"lookup_change_Receivable_therapy\"(@pvar_therapiesid)", npsql))
										{
											dbCommand.CommandType = CommandType.Text;
											dbCommand.Parameters.AddWithValue("pvar_therapiesid",NpgsqlDbType.Varchar,(object)Therapiesid??DBNull.Value);
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


		/// <summary>
		/// <summary>
		/// Calls create_room_receivables() per calendar day for [fromDate, toDate).
		/// Idempotent: deletes previous AllotRoom rows for same room+IPD+remarks before inserting.
		/// </summary>
		public int CreateRoomReceivables(
			Guid     ipdFormId,
			Guid     roomId,
			DateTime fromDate,
			DateTime toDate,
			decimal  costPerDay,
			bool     isAttendant,
			Guid?    tenantId,
			Guid     createdBy,
			string   remarks = null)
		{
			using var conn = new NpgsqlConnection(db_connectionstring);
			conn.Open();

			using var cmd = new NpgsqlCommand(
				"SELECT create_room_receivables(" +
				"@p_ipdformid, @p_roomid, @p_fromdate, @p_todate, " +
				"@p_costperday, @p_isattendant, @p_tenantid, @p_createdby, @p_remarks_override)", conn);

			cmd.Parameters.Add("p_ipdformid",        NpgsqlDbType.Uuid).Value    = ipdFormId;
			cmd.Parameters.Add("p_roomid",           NpgsqlDbType.Uuid).Value    = roomId;
			cmd.Parameters.Add("p_fromdate",         NpgsqlDbType.Date).Value    = fromDate.Date;
			cmd.Parameters.Add("p_todate",           NpgsqlDbType.Date).Value    = toDate.Date;
			cmd.Parameters.Add("p_costperday",       NpgsqlDbType.Numeric).Value = costPerDay;
			cmd.Parameters.Add("p_isattendant",      NpgsqlDbType.Boolean).Value = isAttendant;
			cmd.Parameters.Add("p_tenantid",         NpgsqlDbType.Uuid).Value    =
				tenantId.HasValue ? (object)tenantId.Value : DBNull.Value;
			cmd.Parameters.Add("p_createdby",        NpgsqlDbType.Uuid).Value    = createdBy;
			cmd.Parameters.Add("p_remarks_override", NpgsqlDbType.Varchar).Value =
				string.IsNullOrWhiteSpace(remarks) ? (object)DBNull.Value : remarks;

			var scalar = cmd.ExecuteScalar();
			return scalar != null && scalar != DBNull.Value ? Convert.ToInt32(scalar) : 0;
		}

		public int InvalidateRoomReceivablesForTransfer(
			Guid ipdFormId,
			bool isAttendant,
			DateTime fromDate,
			Guid modifiedBy)
		{
			using var conn = new NpgsqlConnection(db_connectionstring);
			conn.Open();

			using var cmd = new NpgsqlCommand("SELECT \"InvalidateRoomReceivablesForTransfer\"(@p_ipdformid,@p_fromdate,@p_isattendant,@p_modifiedby)", conn);

			cmd.Parameters.Add("p_ipdformid", NpgsqlDbType.Uuid).Value = ipdFormId;
			cmd.Parameters.Add("p_isattendant", NpgsqlDbType.Boolean).Value = isAttendant;
			cmd.Parameters.Add("p_fromdate", NpgsqlDbType.Date).Value = fromDate.Date;
			cmd.Parameters.Add("p_modifiedby", NpgsqlDbType.Uuid).Value = modifiedBy;

			var scalar = cmd.ExecuteScalar();
			return scalar != null && scalar != DBNull.Value ? Convert.ToInt32(scalar) : 0;
		}

        //   public string ZeroOutAttendantReceivablesForCombinedTransfer(
        //Guid ipdFormId,
        //DateTime fromDate,
        //DateTime toDate,
        //Guid modifiedBy)
        //   {
        //       try
        //       {
        //           using (var con = new NpgsqlConnection(db_connectionstring))
        //           {
        //               con.Open();

        //               string sql = @"
        //           UPDATE receivable
        //           SET
        //               amount = 0,
        //               attendantcostperday = 0,
        //               paymentstatus =
        //                   CASE
        //                       WHEN COALESCE(paidamount, 0) > 0 THEN 'Paid'
        //                       ELSE 'Waived'
        //                   END,
        //               remarks = 'TransferRoom:AttendantWaivedForSharedRoom',
        //               modifieduser = @modifiedBy,
        //               modifieddate = NOW(),
        //               isdeleted = false
        //           WHERE ipdnumber = @ipdFormId
        //             AND receivabledate::date BETWEEN @fromDate::date AND @toDate::date
        //             AND LOWER(COALESCE(receivablefor, '')) = 'room'

        //             -- Only old / pure attendant rows should be waived
        //             AND (
        //                   LOWER(COALESCE(remarks, '')) = 'allotroom:attendant'
        //                OR LOWER(COALESCE(remarks, '')) = 'transferroom:attendant'
        //             )

        //             -- Safety: do not touch combined patient+attendant rows
        //             AND COALESCE(remarks, '') NOT ILIKE '%Patient%';
        //       ";

        //               using (var cmd = new NpgsqlCommand(sql, con))
        //               {
        //                   cmd.Parameters.AddWithValue("@ipdFormId", NpgsqlTypes.NpgsqlDbType.Uuid, ipdFormId);
        //                   cmd.Parameters.AddWithValue("@fromDate", NpgsqlTypes.NpgsqlDbType.Date, fromDate.Date);
        //                   cmd.Parameters.AddWithValue("@toDate", NpgsqlTypes.NpgsqlDbType.Date, toDate.Date);
        //                   cmd.Parameters.AddWithValue("@modifiedBy", NpgsqlTypes.NpgsqlDbType.Uuid, modifiedBy);

        //                   cmd.ExecuteNonQuery();
        //               }
        //           }

        //           return "201.1";
        //       }
        //       catch (Exception ex)
        //       {
        //           return ex.Message;
        //       }
        //   }
        public string UpdateRoomReceivableAmountForTransfer(
      Guid ipdFormId,
      Guid roomId,
      DateTime fromDate,
      DateTime toDate,
      decimal newCostPerDay,
      bool isAttendant,
      Guid modifiedBy,
      string remarks)
        {
            string ResponseMessage = "";

            try
            {
                using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
                {
                    npsql.Open();

                    using (var dbCommand = new NpgsqlCommand(
                        "SELECT * FROM \"UpdateRoomReceivableAmountForTransfer\"(" +
                        "@pvar_ipdapplicationformid," +
                        "@pvar_roomid," +
                        "@pvar_fromdate," +
                        "@pvar_todate," +
                        "@pvar_newcostperday," +
                        "@pvar_isattendant," +
                        "@pvar_modifiedby," +
                        "@pvar_remarks" +
                        ")", npsql))
                    {
                        dbCommand.CommandType = CommandType.Text;

                        dbCommand.Parameters.AddWithValue(
                            "pvar_ipdapplicationformid",
                            NpgsqlDbType.Uuid,
                            ipdFormId
                        );

                        dbCommand.Parameters.AddWithValue(
                            "pvar_roomid",
                            NpgsqlDbType.Uuid,
                            roomId
                        );

                        dbCommand.Parameters.AddWithValue(
                            "pvar_fromdate",
                            NpgsqlDbType.Date,
                            fromDate.Date
                        );

                        dbCommand.Parameters.AddWithValue(
                            "pvar_todate",
                            NpgsqlDbType.Date,
                            toDate.Date
                        );

                        dbCommand.Parameters.AddWithValue(
                            "pvar_newcostperday",
                            NpgsqlDbType.Numeric,
                            newCostPerDay
                        );

                        dbCommand.Parameters.AddWithValue(
                            "pvar_isattendant",
                            NpgsqlDbType.Boolean,
                            isAttendant
                        );

                        dbCommand.Parameters.AddWithValue(
                            "pvar_modifiedby",
                            NpgsqlDbType.Uuid,
                            modifiedBy
                        );

                        dbCommand.Parameters.AddWithValue(
                            "pvar_remarks",
                            NpgsqlDbType.Varchar,
                            (object)remarks ?? DBNull.Value
                        );

                        NpgsqlParameter outParm = new NpgsqlParameter("@returnMessage", NpgsqlDbType.Varchar)
                        {
                            Direction = ParameterDirection.Output
                        };
                        dbCommand.Parameters.Add(outParm);

                        dbCommand.ExecuteNonQuery();

                        ResponseMessage = outParm.Value == null || outParm.Value == DBNull.Value
                            ? ""
                            : outParm.Value.ToString();

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
        /// <summary>
        /// Creates a single package-cost receivable (one entry, not per-day).
        /// Package cost is looked up from TreatmentPackage in the DB (not trusted from client).
        /// Idempotent: deletes existing AllotRoom:Package row for same IPD form first.
        /// </summary>
        public int CreatePackageReceivable(
			Guid  ipdFormId,
			Guid  packageId,
			Guid? tenantId,
			Guid  createdBy)
		{
			using var conn = new NpgsqlConnection(db_connectionstring);
			conn.Open();

			using var cmd = new NpgsqlCommand(
				"SELECT create_package_receivable(" +
				"@p_ipdformid, @p_packageid, @p_tenantid, @p_createdby)", conn);

			cmd.Parameters.Add("p_ipdformid", NpgsqlDbType.Uuid).Value = ipdFormId;
			cmd.Parameters.Add("p_packageid", NpgsqlDbType.Uuid).Value = packageId;
			cmd.Parameters.Add("p_tenantid",  NpgsqlDbType.Uuid).Value =
				tenantId.HasValue ? (object)tenantId.Value : DBNull.Value;
			cmd.Parameters.Add("p_createdby", NpgsqlDbType.Uuid).Value = createdBy;

			var scalar = cmd.ExecuteScalar();
			return scalar != null && scalar != DBNull.Value ? Convert.ToInt32(scalar) : 0;
		}

		/// <summary>
		/// Creates the 'IPD Screening Fee' receivable for the given IPD form.
		/// Amount is resolved from People_clinicaltaskinfo for the 'IP Screening' task
		/// (linked ClinicalAppointment practitioner → any configured doctor → receivableconditions fallback).
		/// Idempotent: preserves an existing pending or paid screening-fee receivable.
		/// </summary>
		public int CreateScreeningFeeReceivable(Guid ipdFormId, Guid? tenantId, Guid createdBy)
		{
			using var conn = new NpgsqlConnection(db_connectionstring);
			conn.Open();

			using var cmd = new NpgsqlCommand(
				"SELECT ensure_screening_fee_receivable(@p_ipdformid, @p_tenantid, @p_createdby)", conn);

			cmd.Parameters.Add("p_ipdformid", NpgsqlDbType.Uuid).Value = ipdFormId;
			cmd.Parameters.Add("p_tenantid",  NpgsqlDbType.Uuid).Value =
				tenantId.HasValue ? (object)tenantId.Value : DBNull.Value;
			cmd.Parameters.Add("p_createdby", NpgsqlDbType.Uuid).Value = createdBy;

			var scalar = cmd.ExecuteScalar();
			return scalar != null && scalar != DBNull.Value ? Convert.ToInt32(scalar) : 0;
		}

		public int CreateIPDAppointmentFeeReceivable(Guid ipdFormId, Guid? tenantId, Guid patientId, Guid practitionerId, string taskType, Guid createdBy)
		{
			if (string.IsNullOrWhiteSpace(taskType)) return 0;

			using var conn = new NpgsqlConnection(db_connectionstring);
			conn.Open();

			decimal feeAmount = 0;
			using (var feeCmd = new NpgsqlCommand("SELECT \"Get_IPD_Appointment_Fee_Amount\"(@peopleid,@tasktype)", conn))
			{
				feeCmd.Parameters.AddWithValue("peopleid", NpgsqlDbType.Uuid, practitionerId);
				feeCmd.Parameters.AddWithValue("tasktype", NpgsqlDbType.Varchar, taskType);
				var feeObj = feeCmd.ExecuteScalar();
				if (feeObj != null && feeObj != DBNull.Value)
					feeAmount = Convert.ToDecimal(feeObj);
			}

			if (feeAmount <= 0) return 0;

			var isPreAdmissionConsultation = string.Equals(taskType.Trim(), "IP New", StringComparison.OrdinalIgnoreCase);
			var receivableFor = isPreAdmissionConsultation ? "Pre-Admission Consultation" : taskType.Trim();
			var autoRemark = "Auto:ClinicalAppointment:" + taskType.Trim();

			// Retried requests must not create a second financial row for the same stage.
			using (var existingCmd = new NpgsqlCommand(
				"SELECT \"IPD_Appointment_Fee_Receivable_Exists\"(@pvar_ipdnumber,@pvar_receivablefor)", conn))
			{
				existingCmd.Parameters.AddWithValue("pvar_ipdnumber", NpgsqlDbType.Uuid, ipdFormId);
				existingCmd.Parameters.AddWithValue("pvar_receivablefor", NpgsqlDbType.Varchar, receivableFor);
				var existingResult = existingCmd.ExecuteScalar();
				if (existingResult != null && existingResult != DBNull.Value && Convert.ToBoolean(existingResult)) return 1;
			}

			var receivable = new ReceivableModel
			{
				Receivableid = Guid.NewGuid(),
				tenantid = tenantId,
				receivabledate = DateTime.Now.Date,
				patientname = patientId,
				ipdnumber = ipdFormId,
				receivablefor = receivableFor,
				amount = feeAmount,
				remarks = autoRemark,
				createduser = createdBy,
				paidamount = 0,
				paymentstatus = "Pending",
				craftmyapp_actionmethodname = "Add_Receivable"
			};

			var message = Add_Receivable(receivable);
			return (message ?? "").Replace("\"", "").Contains("201.1") ? 1 : 0;
		}

		/// <summary>
		/// Creates the 'IPD Admission Fees' receivable for the given IPD form.
		/// Amount is resolved from MiscellaneousFee (feetype='Admission') with receivableconditions fallback.
		/// Idempotent: deletes existing Auto:AdmissionConfirmed:AdmissionFees row first.
		/// </summary>
		public int CreateAdmissionFeeReceivable(Guid ipdFormId, Guid? tenantId, Guid createdBy)
		{
			using var conn = new NpgsqlConnection(db_connectionstring);
			conn.Open();

			using var cmd = new NpgsqlCommand(
				"SELECT create_admission_fee_receivable(@p_ipdformid, @p_tenantid, @p_createdby)", conn);

			cmd.Parameters.Add("p_ipdformid", NpgsqlDbType.Uuid).Value = ipdFormId;
			cmd.Parameters.Add("p_tenantid",  NpgsqlDbType.Uuid).Value =
				tenantId.HasValue ? (object)tenantId.Value : DBNull.Value;
			cmd.Parameters.Add("p_createdby", NpgsqlDbType.Uuid).Value = createdBy;

			var scalar = cmd.ExecuteScalar();
			return scalar != null && scalar != DBNull.Value ? Convert.ToInt32(scalar) : 0;
		}

		public int CreateProvisionalBookingDepositReceivables(Guid ipdFormId, Guid? tenantId, Guid createdBy)
		{
			using var conn = new NpgsqlConnection(db_connectionstring);
			conn.Open();

			using var cmd = new NpgsqlCommand(
				"SELECT create_provisional_booking_deposit_receivables(@p_ipdformid, @p_tenantid, @p_createdby)", conn);

			cmd.Parameters.Add("p_ipdformid", NpgsqlDbType.Uuid).Value = ipdFormId;
			cmd.Parameters.Add("p_tenantid",  NpgsqlDbType.Uuid).Value =
				tenantId.HasValue ? (object)tenantId.Value : DBNull.Value;
			cmd.Parameters.Add("p_createdby", NpgsqlDbType.Uuid).Value = createdBy;

			var scalar = cmd.ExecuteScalar();
			return scalar != null && scalar != DBNull.Value ? Convert.ToInt32(scalar) : 0;
		}

		/// <summary>
		/// Creates the attendant booking deposit receivable via the
		/// create_attendant_booking_deposit_receivable stored procedure.
		/// Reads Room.attendantbookingdeposit and resolves patientname internally.
		/// Idempotent: soft-deletes any existing attendant deposit rows before inserting.
		/// </summary>
		public void CreateAttendantBookingDepositReceivable(
			Guid  ipdFormId,
			Guid  roomId,
			Guid? tenantId,
			Guid  createdBy)
		{
			using var conn = new NpgsqlConnection(db_connectionstring);
			conn.Open();

			using var cmd = new NpgsqlCommand(
				"SELECT public.create_attendant_booking_deposit_receivable(@p_ipdformid, @p_roomid, @p_tenantid, @p_createdby)",
				conn);

			cmd.Parameters.Add("p_ipdformid", NpgsqlDbType.Uuid).Value = ipdFormId;
			cmd.Parameters.Add("p_roomid",    NpgsqlDbType.Uuid).Value = roomId;
			cmd.Parameters.Add("p_tenantid",  NpgsqlDbType.Uuid).Value =
				tenantId.HasValue ? (object)tenantId.Value : DBNull.Value;
			cmd.Parameters.Add("p_createdby", NpgsqlDbType.Uuid).Value = createdBy;

			cmd.ExecuteScalar();
		}

		public int CreateOPDConsultationFeeReceivable(Guid opdFormId, Guid? tenantId, Guid createdBy, Guid? taskId = null)
		{
			using var conn = new NpgsqlConnection(db_connectionstring);
			conn.Open();

			using var cmd = new NpgsqlCommand(
				"SELECT create_opd_consultation_fee_receivable(@p_opdformid, @p_tenantid, @p_createdby, @p_taskid)", conn);

			cmd.Parameters.Add("p_opdformid", NpgsqlDbType.Uuid).Value = opdFormId;
			cmd.Parameters.Add("p_tenantid", NpgsqlDbType.Uuid).Value =
				tenantId.HasValue ? (object)tenantId.Value : DBNull.Value;
			cmd.Parameters.Add("p_createdby", NpgsqlDbType.Uuid).Value = createdBy;
			cmd.Parameters.Add("p_taskid", NpgsqlDbType.Uuid).Value =
				taskId.HasValue ? (object)taskId.Value : DBNull.Value;

			var scalar = cmd.ExecuteScalar();
			return scalar != null && scalar != DBNull.Value ? Convert.ToInt32(scalar) : 0;
		}

        public string RecalculateActiveRoomReceivablesByPackageAfterRoomChange(
Guid ipdFormId,
Guid modifiedBy)
        {
            return RecalculateActiveRoomReceivablesByPackageAfterTransfer(
                ipdFormId: ipdFormId,
                modifiedBy: modifiedBy
            );
        }
        public string RecalculateActiveRoomReceivablesByPackageAfterTransfer(
         Guid ipdFormId,
         Guid modifiedBy)
        {
            string ResponseMessage = "";

            try
            {
                using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
                {
                    npsql.Open();

                    using (var dbCommand = new NpgsqlCommand(
                        "SELECT * FROM \"RecalculateActiveRoomReceivablesByPackageAfterTransfer\"(" +
                        "@pvar_ipdapplicationformid," +
                        "@pvar_modifieduser" +
                        ")", npsql))
                    {
                        dbCommand.CommandType = CommandType.Text;

                        dbCommand.Parameters.AddWithValue(
                            "pvar_ipdapplicationformid",
                            NpgsqlDbType.Uuid,
                            ipdFormId
                        );

                        dbCommand.Parameters.AddWithValue(
                            "pvar_modifieduser",
                            NpgsqlDbType.Uuid,
                            modifiedBy
                        );

                        NpgsqlParameter outParm = new NpgsqlParameter("@returnMessage", NpgsqlDbType.Varchar)
                        {
                            Direction = ParameterDirection.Output
                        };
                        dbCommand.Parameters.Add(outParm);

                        dbCommand.ExecuteNonQuery();

                        ResponseMessage = outParm.Value == null || outParm.Value == DBNull.Value
                            ? ""
                            : outParm.Value.ToString();

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
        public string SyncRoomReceivablesAfterTransferSimplePackageReprice(
      Guid ipdFormId,
      DateTime fromDate,
      DateTime toDate,
      Guid newRoomId,
      decimal newCostPerDay,
      string transferRole,
      Guid modifiedBy)
        {
            try
            {
                /*
                    Step 1:
                    Existing transfer sync.

                    This will:
                    - mark old transferred-out room/date as inactive/waived
                    - update active transfer room receivables
                    - insert missing receivable rows for new transferred room/date
                */
                string syncMessage = SyncRoomReceivablesAfterTransferNoDelete(
                    ipdFormId: ipdFormId,
                    fromDate: fromDate,
                    toDate: toDate,
                    newRoomId: newRoomId,
                    newCostPerDay: newCostPerDay,
                    transferRole: transferRole,
                    modifiedBy: modifiedBy
                );

                if ((syncMessage ?? "").Replace("\"", "") != "201.1")
                {
                    return syncMessage;
                }

                /*
                    Step 2:
                    Recalculate package room cost for all active room receivables.
                */
                string repriceMessage = RecalculateActiveRoomReceivablesByPackageAfterTransfer(
                    ipdFormId: ipdFormId,
                    modifiedBy: modifiedBy
                );

                return repriceMessage;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        public string SyncRoomReceivablesAfterTransferNoDelete(
       Guid ipdFormId,
       DateTime fromDate,
       DateTime toDate,
       Guid newRoomId,
       decimal newCostPerDay,
       string transferRole,
       Guid modifiedBy)
        {
            string ResponseMessage = "";

            try
            {
                using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
                {
                    npsql.Open();

                    using (var dbCommand = new NpgsqlCommand(
                        "SELECT * FROM \"SyncRoomReceivablesAfterTransferNoDelete\"(" +
                        "@pvar_ipdapplicationformid," +
                        "@pvar_fromdate," +
                        "@pvar_todate," +
                        "@pvar_newroomid," +
                        "@pvar_newcostperday," +
                        "@pvar_transferrole," +
                        "@pvar_modifieduser" +
                        ")", npsql))
                    {
                        dbCommand.CommandType = CommandType.Text;

                        dbCommand.Parameters.AddWithValue(
                            "pvar_ipdapplicationformid",
                            NpgsqlDbType.Uuid,
                            ipdFormId
                        );

                        dbCommand.Parameters.AddWithValue(
                            "pvar_fromdate",
                            NpgsqlDbType.Date,
                            fromDate.Date
                        );

                        dbCommand.Parameters.AddWithValue(
                            "pvar_todate",
                            NpgsqlDbType.Date,
                            toDate.Date
                        );

                        dbCommand.Parameters.AddWithValue(
                            "pvar_newroomid",
                            NpgsqlDbType.Uuid,
                            newRoomId
                        );

                        dbCommand.Parameters.AddWithValue(
                            "pvar_newcostperday",
                            NpgsqlDbType.Numeric,
                            newCostPerDay
                        );

                        dbCommand.Parameters.AddWithValue(
                            "pvar_transferrole",
                            NpgsqlDbType.Varchar,
                            (object)transferRole ?? DBNull.Value
                        );

                        dbCommand.Parameters.AddWithValue(
                            "pvar_modifieduser",
                            NpgsqlDbType.Uuid,
                            modifiedBy
                        );

                        NpgsqlParameter outParm = new NpgsqlParameter("@returnMessage", NpgsqlDbType.Varchar)
                        {
                            Direction = ParameterDirection.Output
                        };
                        dbCommand.Parameters.Add(outParm);

                        dbCommand.ExecuteNonQuery();

                        ResponseMessage = outParm.Value == null || outParm.Value == DBNull.Value
                            ? ""
                            : outParm.Value.ToString();

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

    }


}
