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
	using System.Collections.Generic;
	using System.Linq;

	//This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:43:29
	public class BillingPaymentDAL
	{
		public virtual string db_connectionstring { get; set; }

		public BillingPaymentDAL(string connectionString)
		{

			db_connectionstring = connectionString;
		}
		public virtual string Add_Billing_Payment(BillingPaymentModel model)
		{
			String ResponseMessage = "";

			try
			{

				using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
				{
					npsql.Open();
					using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"Add_Billing_Payment\"(@pvar_billingpaymentid,@pvar_tenantid,@pvar_paymentdate,@pvar_patientname,@pvar_patientvisit,@pvar_ipdnumber,@pvar_opdnumber,@pvar_receivablefor,@pvar_therapy,@pvar_therapycost,@pvar_therapykit,@pvar_kitprice,@pvar_medicine,@pvar_price,@pvar_room,@pvar_receivedamount,@pvar_currency,@pvar_conversionrate,@pvar_amount,@pvar_remarks,@pvar_paymentmode,@pvar_transactionreference,@pvar_bankname,@pvar_chequedddate,@pvar_paymentstatus,@pvar_collectedby,@pvar_refundmode,@pvar_refundedamount,@pvar_refundedby,@pvar_refundreferencenumber,@pvar_refundbankname,@pvar_refundreason,@pvar_refundstatus,@pvar_counterid,@pvar_createduser)", npsql))
					{
						dbCommand.CommandType = CommandType.Text;

						dbCommand.Parameters.AddWithValue("pvar_billingpaymentid", NpgsqlDbType.Uuid, (object)model.BillingPaymentid ?? DBNull.Value);
						dbCommand.Parameters.AddWithValue("pvar_tenantid", NpgsqlDbType.Uuid, (object)model.tenantid ?? DBNull.Value);

						dbCommand.Parameters.AddWithValue("pvar_paymentdate", NpgsqlDbType.Date, (object)model.paymentdate ?? DBNull.Value);

						dbCommand.Parameters.AddWithValue("pvar_patientname", NpgsqlDbType.Uuid, (object)model.patientname ?? DBNull.Value);

						dbCommand.Parameters.AddWithValue("pvar_patientvisit", NpgsqlDbType.Uuid, (object)model.patientvisit ?? DBNull.Value);

						dbCommand.Parameters.AddWithValue("pvar_ipdnumber", NpgsqlDbType.Uuid, (object)model.ipdnumber ?? DBNull.Value);

						dbCommand.Parameters.AddWithValue("pvar_opdnumber", NpgsqlDbType.Uuid, (object)model.opdnumber ?? DBNull.Value);

						dbCommand.Parameters.AddWithValue("pvar_receivablefor", NpgsqlDbType.Varchar, (object)model.receivablefor ?? DBNull.Value);

						dbCommand.Parameters.AddWithValue("pvar_therapy", NpgsqlDbType.Uuid, (object)model.therapy ?? DBNull.Value);

						dbCommand.Parameters.AddWithValue("pvar_therapycost", NpgsqlDbType.Numeric, (object)model.therapycost ?? DBNull.Value);

						dbCommand.Parameters.AddWithValue("pvar_therapykit", NpgsqlDbType.Uuid, (object)model.therapykit ?? DBNull.Value);

						dbCommand.Parameters.AddWithValue("pvar_kitprice", NpgsqlDbType.Varchar, (object)model.kitprice ?? DBNull.Value);

						dbCommand.Parameters.AddWithValue("pvar_medicine", NpgsqlDbType.Uuid, (object)model.medicine ?? DBNull.Value);

						dbCommand.Parameters.AddWithValue("pvar_price", NpgsqlDbType.Numeric, (object)model.price ?? DBNull.Value);

						dbCommand.Parameters.AddWithValue("pvar_room", NpgsqlDbType.Uuid, (object)model.room ?? DBNull.Value);

						dbCommand.Parameters.AddWithValue("pvar_receivedamount", NpgsqlDbType.Numeric, (object)model.receivedamount ?? DBNull.Value);

						dbCommand.Parameters.AddWithValue("pvar_currency", NpgsqlDbType.Varchar, string.IsNullOrWhiteSpace(model.currency) ? "INR" : model.currency);

						dbCommand.Parameters.AddWithValue("pvar_conversionrate", NpgsqlDbType.Numeric, (object)model.conversionrate ?? DBNull.Value);

						dbCommand.Parameters.AddWithValue("pvar_amount", NpgsqlDbType.Numeric, (object)model.amount ?? DBNull.Value);

						dbCommand.Parameters.AddWithValue("pvar_remarks", NpgsqlDbType.Varchar, (object)model.remarks ?? DBNull.Value);

						dbCommand.Parameters.AddWithValue("pvar_paymentmode", NpgsqlDbType.Varchar, (object)model.paymentmode ?? DBNull.Value);

						dbCommand.Parameters.AddWithValue("pvar_transactionreference", NpgsqlDbType.Varchar, (object)model.transactionreference ?? DBNull.Value);

						dbCommand.Parameters.AddWithValue("pvar_bankname", NpgsqlDbType.Varchar, (object)model.bankname ?? DBNull.Value);

						dbCommand.Parameters.AddWithValue("pvar_chequedddate", NpgsqlDbType.Date, (object)model.chequedddate ?? DBNull.Value);

						dbCommand.Parameters.AddWithValue("pvar_paymentstatus", NpgsqlDbType.Varchar, (object)model.paymentstatus ?? DBNull.Value);

						dbCommand.Parameters.AddWithValue("pvar_collectedby", NpgsqlDbType.Uuid, (object)model.collectedby ?? DBNull.Value);

						dbCommand.Parameters.AddWithValue("pvar_refundmode", NpgsqlDbType.Varchar, (object)model.refundmode ?? DBNull.Value);

						dbCommand.Parameters.AddWithValue("pvar_refundedamount", NpgsqlDbType.Numeric, (object)model.refundedamount ?? DBNull.Value);

						dbCommand.Parameters.AddWithValue("pvar_refundedby", NpgsqlDbType.Uuid, (object)model.refundedby ?? DBNull.Value);

						dbCommand.Parameters.AddWithValue("pvar_refundreferencenumber", NpgsqlDbType.Varchar, (object)model.refundreferencenumber ?? DBNull.Value);

						dbCommand.Parameters.AddWithValue("pvar_refundbankname", NpgsqlDbType.Varchar, (object)model.refundbankname ?? DBNull.Value);

						dbCommand.Parameters.AddWithValue("pvar_refundreason", NpgsqlDbType.Varchar, (object)model.refundreason ?? DBNull.Value);

						dbCommand.Parameters.AddWithValue("pvar_refundstatus", NpgsqlDbType.Varchar, (object)model.refundstatus ?? DBNull.Value);

						dbCommand.Parameters.AddWithValue("pvar_counterid", NpgsqlDbType.Varchar, (object)model.counterid ?? DBNull.Value);
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

		//public virtual string Add_Billing_Payment(BillingPaymentModel model)
		//{
		//	String ResponseMessage = "";

		//	try
		//	{

		//		using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
		//		{
		//			npsql.Open();
		//			using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"Add_Billing_Payment\"(@pvar_billingpaymentid,@pvar_tenantid,@pvar_receiptno,@pvar_receivablefor,@pvar_patientname,@pvar_patientvisit,@pvar_ipdnumber,@pvar_opdnumber,@pvar_currency,@pvar_receivedamount,@pvar_conversionrate,@pvar_amount,@pvar_paymentmode,@pvar_transactionreference,@pvar_bankname,@pvar_chequedddate,@pvar_paymentstatus,@pvar_collectedby,@pvar_refundmode,@pvar_refundedamount,@pvar_refundedby,@pvar_refundreferencenumber,@pvar_refundbankname,@pvar_refundreason,@pvar_refundstatus,@pvar_counterid,@pvar_remarks,@pvar_createduser)", npsql))
		//			{
		//				dbCommand.CommandType = CommandType.Text;

		//				dbCommand.Parameters.AddWithValue("pvar_billingpaymentid", NpgsqlDbType.Uuid, (object)model.BillingPaymentid ?? DBNull.Value);
		//				dbCommand.Parameters.AddWithValue("pvar_tenantid", NpgsqlDbType.Uuid, (object)model.tenantid ?? DBNull.Value);

		//				dbCommand.Parameters.AddWithValue("pvar_receiptno", NpgsqlDbType.Varchar, (object)model.receiptno ?? DBNull.Value);

		//				dbCommand.Parameters.AddWithValue("pvar_receivablefor", NpgsqlDbType.Varchar, (object)model.receivablefor ?? DBNull.Value);

		//				dbCommand.Parameters.AddWithValue("pvar_patientname", NpgsqlDbType.Uuid, (object)model.patientname ?? DBNull.Value);

		//				dbCommand.Parameters.AddWithValue("pvar_patientvisit", NpgsqlDbType.Uuid, (object)model.patientvisit ?? DBNull.Value);

		//				dbCommand.Parameters.AddWithValue("pvar_ipdnumber", NpgsqlDbType.Uuid, (object)model.ipdnumber ?? DBNull.Value);

		//				dbCommand.Parameters.AddWithValue("pvar_opdnumber", NpgsqlDbType.Uuid, (object)model.opdnumber ?? DBNull.Value);

		//				dbCommand.Parameters.AddWithValue("pvar_currency", NpgsqlDbType.Varchar, string.IsNullOrWhiteSpace(model.currency) ? "INR" : model.currency);

		//				dbCommand.Parameters.AddWithValue("pvar_receivedamount", NpgsqlDbType.Numeric, (object)model.receivedamount ?? DBNull.Value);

		//				dbCommand.Parameters.AddWithValue("pvar_conversionrate", NpgsqlDbType.Numeric, (object)model.conversionrate ?? DBNull.Value);

		//				dbCommand.Parameters.AddWithValue("pvar_amount", NpgsqlDbType.Numeric, (object)model.amount ?? DBNull.Value);

		//				dbCommand.Parameters.AddWithValue("pvar_paymentmode", NpgsqlDbType.Varchar, (object)model.paymentmode ?? DBNull.Value);

		//				dbCommand.Parameters.AddWithValue("pvar_transactionreference", NpgsqlDbType.Varchar, (object)model.transactionreference ?? DBNull.Value);

		//				dbCommand.Parameters.AddWithValue("pvar_bankname", NpgsqlDbType.Varchar, (object)model.bankname ?? DBNull.Value);

		//				dbCommand.Parameters.AddWithValue("pvar_chequedddate", NpgsqlDbType.Date, (object)model.chequedddate ?? DBNull.Value);

		//				dbCommand.Parameters.AddWithValue("pvar_paymentstatus", NpgsqlDbType.Varchar, (object)model.paymentstatus ?? DBNull.Value);

		//				dbCommand.Parameters.AddWithValue("pvar_collectedby", NpgsqlDbType.Uuid, (object)model.collectedby ?? DBNull.Value);

		//				dbCommand.Parameters.AddWithValue("pvar_refundmode", NpgsqlDbType.Varchar, (object)model.refundmode ?? DBNull.Value);

		//				dbCommand.Parameters.AddWithValue("pvar_refundedamount", NpgsqlDbType.Numeric, (object)model.refundedamount ?? DBNull.Value);

		//				dbCommand.Parameters.AddWithValue("pvar_refundedby", NpgsqlDbType.Uuid, (object)model.refundedby ?? DBNull.Value);

		//				dbCommand.Parameters.AddWithValue("pvar_refundreferencenumber", NpgsqlDbType.Varchar, (object)model.refundreferencenumber ?? DBNull.Value);

		//				dbCommand.Parameters.AddWithValue("pvar_refundbankname", NpgsqlDbType.Varchar, (object)model.refundbankname ?? DBNull.Value);

		//				dbCommand.Parameters.AddWithValue("pvar_refundreason", NpgsqlDbType.Varchar, (object)model.refundreason ?? DBNull.Value);

		//				dbCommand.Parameters.AddWithValue("pvar_refundstatus", NpgsqlDbType.Varchar, (object)model.refundstatus ?? DBNull.Value);

		//				dbCommand.Parameters.AddWithValue("pvar_counterid", NpgsqlDbType.Varchar, (object)model.counterid ?? DBNull.Value);

		//				dbCommand.Parameters.AddWithValue("pvar_remarks", NpgsqlDbType.Varchar, (object)model.remarks ?? DBNull.Value);
		//				dbCommand.Parameters.AddWithValue("pvar_createduser", NpgsqlDbType.Uuid, (object)model.createduser ?? DBNull.Value);


		//				NpgsqlParameter outParm = new NpgsqlParameter("pvar_returnMessage", NpgsqlDbType.Varchar)
		//				{
		//					Direction = ParameterDirection.Output
		//				};
		//				dbCommand.Parameters.Add(outParm);

		//				dbCommand.ExecuteNonQuery();
		//				ResponseMessage = outParm.Value.ToString();
		//				if (dbCommand.Connection.State != ConnectionState.Closed)
		//				{
		//					dbCommand.Connection.Dispose();
		//				}

		//			}
		//			npsql.Close();
		//		}


		//	}
		//	catch (Exception ex)
		//	{
		//		ResponseMessage = ex.Message;
		//		Console.WriteLine(ex);
		//	}

		//	return ResponseMessage;

		//}

		public virtual string Add_Billing_Payment_old(BillingPaymentModel model)
		{
			String ResponseMessage = "";

			try
			{

				using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
				{
					npsql.Open();
					using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"Add_Billing_Payment\"(@pvar_billingpaymentid,@pvar_tenantid,@pvar_receiptno,@pvar_receivablefor,@pvar_patientvisit,@pvar_patientname,@pvar_amount,@pvar_paymentmode,@pvar_transactionreference,@pvar_paymentstatus,@pvar_refundmode,@pvar_refundedamount,@pvar_refundedby,@pvar_refundreason,@pvar_collectedby,@pvar_counterid,@pvar_remarks,@pvar_createduser)", npsql))
					{
						dbCommand.CommandType = CommandType.Text;

						dbCommand.Parameters.AddWithValue("pvar_billingpaymentid", NpgsqlDbType.Uuid, (object)model.BillingPaymentid ?? DBNull.Value);
						dbCommand.Parameters.AddWithValue("pvar_tenantid", NpgsqlDbType.Uuid, (object)model.tenantid ?? DBNull.Value);

						dbCommand.Parameters.AddWithValue("pvar_receiptno", NpgsqlDbType.Varchar, (object)model.receiptno ?? DBNull.Value);

						dbCommand.Parameters.AddWithValue("pvar_receivablefor", NpgsqlDbType.Varchar, (object)model.receivablefor ?? DBNull.Value);

						dbCommand.Parameters.AddWithValue("pvar_patientvisit", NpgsqlDbType.Uuid, (object)model.patientvisit ?? DBNull.Value);

						dbCommand.Parameters.AddWithValue("pvar_patientname", NpgsqlDbType.Uuid, (object)model.patientname ?? DBNull.Value);

						dbCommand.Parameters.AddWithValue("pvar_amount", NpgsqlDbType.Numeric, (object)model.amount ?? DBNull.Value);

						dbCommand.Parameters.AddWithValue("pvar_paymentmode", NpgsqlDbType.Varchar, (object)model.paymentmode ?? DBNull.Value);

						dbCommand.Parameters.AddWithValue("pvar_transactionreference", NpgsqlDbType.Varchar, (object)model.transactionreference ?? DBNull.Value);

						dbCommand.Parameters.AddWithValue("pvar_paymentstatus", NpgsqlDbType.Varchar, (object)model.paymentstatus ?? DBNull.Value);
						dbCommand.Parameters.AddWithValue("pvar_refundmode", NpgsqlDbType.Varchar, (object)model.refundmode ?? DBNull.Value);

						dbCommand.Parameters.AddWithValue("pvar_refundedamount", NpgsqlDbType.Numeric, (object)model.refundedamount ?? DBNull.Value);

						dbCommand.Parameters.AddWithValue("pvar_refundedby", NpgsqlDbType.Uuid, (object)model.refundedby ?? DBNull.Value);

						dbCommand.Parameters.AddWithValue("pvar_refundreason", NpgsqlDbType.Varchar, (object)model.refundreason ?? DBNull.Value);

						dbCommand.Parameters.AddWithValue("pvar_collectedby", NpgsqlDbType.Uuid, (object)model.collectedby ?? DBNull.Value);

						dbCommand.Parameters.AddWithValue("pvar_counterid", NpgsqlDbType.Varchar, (object)model.counterid ?? DBNull.Value);

						dbCommand.Parameters.AddWithValue("pvar_remarks", NpgsqlDbType.Varchar, (object)model.remarks ?? DBNull.Value);
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
		public virtual BillingPaymentModel getById_BillingPayment(string BillingPaymentid)
		{
			DataTable dataTable = new DataTable();
			DataSet dataSet = new DataSet();
			try
			{

				using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
				{
					npsql.Open();
					using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"getById_sp_BillingPayment\"(@pvar_billingpaymentid)", npsql))
					{
						dbCommand.CommandType = CommandType.Text;
						dbCommand.Parameters.AddWithValue("pvar_billingpaymentid", (object)BillingPaymentid ?? DBNull.Value);
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
				return ModelConverter.ConvertDataRowToModel<BillingPaymentModel>(row);
			}
			else
			{
				return null;
			}
		}

		public virtual string Update_Billing_Payment(BillingPaymentModel model)
		{
			String ResponseMessage = "";
			try
			{

				using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
				{
					npsql.Open();
					using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"Update_Billing_Payment\"(@pvar_billingpaymentid,@pvar_tenantid,@pvar_paymentdate,@pvar_patientname,@pvar_patientvisit,@pvar_ipdnumber,@pvar_opdnumber,@pvar_receivablefor,@pvar_therapy,@pvar_therapycost,@pvar_therapykit,@pvar_kitprice,@pvar_medicine,@pvar_price,@pvar_room,@pvar_receivedamount,@pvar_currency,@pvar_conversionrate,@pvar_amount,@pvar_remarks,@pvar_paymentmode,@pvar_transactionreference,@pvar_bankname,@pvar_chequedddate,@pvar_paymentstatus,@pvar_collectedby,@pvar_refundmode,@pvar_refundedamount,@pvar_refundedby,@pvar_refundreferencenumber,@pvar_refundbankname,@pvar_refundreason,@pvar_refundstatus,@pvar_counterid,@pvar_modifieduser)", npsql))
					{
						dbCommand.CommandType = CommandType.Text;
						dbCommand.Parameters.AddWithValue("pvar_billingpaymentid", NpgsqlDbType.Uuid, (object)model.BillingPaymentid ?? DBNull.Value);
						dbCommand.Parameters.AddWithValue("pvar_tenantid", NpgsqlDbType.Uuid, (object)model.tenantid ?? DBNull.Value);

						dbCommand.Parameters.AddWithValue("pvar_paymentdate", NpgsqlDbType.Date, (object)model.paymentdate ?? DBNull.Value);

						dbCommand.Parameters.AddWithValue("pvar_patientname", NpgsqlDbType.Uuid, (object)model.patientname ?? DBNull.Value);

						dbCommand.Parameters.AddWithValue("pvar_patientvisit", NpgsqlDbType.Uuid, (object)model.patientvisit ?? DBNull.Value);

						dbCommand.Parameters.AddWithValue("pvar_ipdnumber", NpgsqlDbType.Uuid, (object)model.ipdnumber ?? DBNull.Value);

						dbCommand.Parameters.AddWithValue("pvar_opdnumber", NpgsqlDbType.Uuid, (object)model.opdnumber ?? DBNull.Value);

						dbCommand.Parameters.AddWithValue("pvar_receivablefor", NpgsqlDbType.Varchar, (object)model.receivablefor ?? DBNull.Value);

						dbCommand.Parameters.AddWithValue("pvar_therapy", NpgsqlDbType.Uuid, (object)model.therapy ?? DBNull.Value);

						dbCommand.Parameters.AddWithValue("pvar_therapycost", NpgsqlDbType.Numeric, (object)model.therapycost ?? DBNull.Value);

						dbCommand.Parameters.AddWithValue("pvar_therapykit", NpgsqlDbType.Uuid, (object)model.therapykit ?? DBNull.Value);

						dbCommand.Parameters.AddWithValue("pvar_kitprice", NpgsqlDbType.Varchar, (object)model.kitprice ?? DBNull.Value);

						dbCommand.Parameters.AddWithValue("pvar_medicine", NpgsqlDbType.Uuid, (object)model.medicine ?? DBNull.Value);

						dbCommand.Parameters.AddWithValue("pvar_price", NpgsqlDbType.Numeric, (object)model.price ?? DBNull.Value);

						dbCommand.Parameters.AddWithValue("pvar_room", NpgsqlDbType.Uuid, (object)model.room ?? DBNull.Value);

						dbCommand.Parameters.AddWithValue("pvar_receivedamount", NpgsqlDbType.Numeric, (object)model.receivedamount ?? DBNull.Value);

						dbCommand.Parameters.AddWithValue("pvar_currency", NpgsqlDbType.Varchar, string.IsNullOrWhiteSpace(model.currency) ? "INR" : model.currency);

						dbCommand.Parameters.AddWithValue("pvar_conversionrate", NpgsqlDbType.Numeric, (object)model.conversionrate ?? DBNull.Value);

						dbCommand.Parameters.AddWithValue("pvar_amount", NpgsqlDbType.Numeric, (object)model.amount ?? DBNull.Value);

						dbCommand.Parameters.AddWithValue("pvar_remarks", NpgsqlDbType.Varchar, (object)model.remarks ?? DBNull.Value);

						dbCommand.Parameters.AddWithValue("pvar_paymentmode", NpgsqlDbType.Varchar, (object)model.paymentmode ?? DBNull.Value);

						dbCommand.Parameters.AddWithValue("pvar_transactionreference", NpgsqlDbType.Varchar, (object)model.transactionreference ?? DBNull.Value);

						dbCommand.Parameters.AddWithValue("pvar_bankname", NpgsqlDbType.Varchar, (object)model.bankname ?? DBNull.Value);

						dbCommand.Parameters.AddWithValue("pvar_chequedddate", NpgsqlDbType.Date, (object)model.chequedddate ?? DBNull.Value);

						dbCommand.Parameters.AddWithValue("pvar_paymentstatus", NpgsqlDbType.Varchar, (object)model.paymentstatus ?? DBNull.Value);

						dbCommand.Parameters.AddWithValue("pvar_collectedby", NpgsqlDbType.Uuid, (object)model.collectedby ?? DBNull.Value);

						dbCommand.Parameters.AddWithValue("pvar_refundmode", NpgsqlDbType.Varchar, (object)model.refundmode ?? DBNull.Value);

						dbCommand.Parameters.AddWithValue("pvar_refundedamount", NpgsqlDbType.Numeric, (object)model.refundedamount ?? DBNull.Value);

						dbCommand.Parameters.AddWithValue("pvar_refundedby", NpgsqlDbType.Uuid, (object)model.refundedby ?? DBNull.Value);

						dbCommand.Parameters.AddWithValue("pvar_refundreferencenumber", NpgsqlDbType.Varchar, (object)model.refundreferencenumber ?? DBNull.Value);

						dbCommand.Parameters.AddWithValue("pvar_refundbankname", NpgsqlDbType.Varchar, (object)model.refundbankname ?? DBNull.Value);

						dbCommand.Parameters.AddWithValue("pvar_refundreason", NpgsqlDbType.Varchar, (object)model.refundreason ?? DBNull.Value);

						dbCommand.Parameters.AddWithValue("pvar_refundstatus", NpgsqlDbType.Varchar, (object)model.refundstatus ?? DBNull.Value);

						dbCommand.Parameters.AddWithValue("pvar_counterid", NpgsqlDbType.Varchar, (object)model.counterid ?? DBNull.Value);
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

		//public virtual string Update_Billing_Payment(BillingPaymentModel model)
		//{
		//	String ResponseMessage = "";
		//	try
		//	{

		//		using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
		//		{
		//			npsql.Open();
		//			using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"Update_Billing_Payment\"(@pvar_billingpaymentid,@pvar_tenantid,@pvar_receiptno,@pvar_receivablefor,@pvar_patientname,@pvar_patientvisit,@pvar_ipdnumber,@pvar_opdnumber,@pvar_currency,@pvar_receivedamount,@pvar_conversionrate,@pvar_amount,@pvar_paymentmode,@pvar_transactionreference,@pvar_bankname,@pvar_chequedddate,@pvar_paymentstatus,@pvar_collectedby,@pvar_refundmode,@pvar_refundedamount,@pvar_refundedby,@pvar_refundreferencenumber,@pvar_refundbankname,@pvar_refundreason,@pvar_refundstatus,@pvar_counterid,@pvar_remarks,@pvar_modifieduser)", npsql))
		//			{
		//				dbCommand.CommandType = CommandType.Text;
		//				dbCommand.Parameters.AddWithValue("pvar_billingpaymentid", NpgsqlDbType.Uuid, (object)model.BillingPaymentid ?? DBNull.Value);
		//				dbCommand.Parameters.AddWithValue("pvar_tenantid", NpgsqlDbType.Uuid, (object)model.tenantid ?? DBNull.Value);

		//				dbCommand.Parameters.AddWithValue("pvar_receiptno", NpgsqlDbType.Varchar, (object)model.receiptno ?? DBNull.Value);

		//				dbCommand.Parameters.AddWithValue("pvar_receivablefor", NpgsqlDbType.Varchar, (object)model.receivablefor ?? DBNull.Value);

		//				dbCommand.Parameters.AddWithValue("pvar_patientname", NpgsqlDbType.Uuid, (object)model.patientname ?? DBNull.Value);

		//				dbCommand.Parameters.AddWithValue("pvar_patientvisit", NpgsqlDbType.Uuid, (object)model.patientvisit ?? DBNull.Value);

		//				dbCommand.Parameters.AddWithValue("pvar_ipdnumber", NpgsqlDbType.Uuid, (object)model.ipdnumber ?? DBNull.Value);

		//				dbCommand.Parameters.AddWithValue("pvar_opdnumber", NpgsqlDbType.Uuid, (object)model.opdnumber ?? DBNull.Value);

		//				dbCommand.Parameters.AddWithValue("pvar_currency", NpgsqlDbType.Varchar, string.IsNullOrWhiteSpace(model.currency) ? "INR" : model.currency);

		//				dbCommand.Parameters.AddWithValue("pvar_receivedamount", NpgsqlDbType.Numeric, (object)model.receivedamount ?? DBNull.Value);

		//				dbCommand.Parameters.AddWithValue("pvar_conversionrate", NpgsqlDbType.Numeric, (object)model.conversionrate ?? DBNull.Value);

		//				dbCommand.Parameters.AddWithValue("pvar_amount", NpgsqlDbType.Numeric, (object)model.amount ?? DBNull.Value);

		//				dbCommand.Parameters.AddWithValue("pvar_paymentmode", NpgsqlDbType.Varchar, (object)model.paymentmode ?? DBNull.Value);

		//				dbCommand.Parameters.AddWithValue("pvar_transactionreference", NpgsqlDbType.Varchar, (object)model.transactionreference ?? DBNull.Value);

		//				dbCommand.Parameters.AddWithValue("pvar_bankname", NpgsqlDbType.Varchar, (object)model.bankname ?? DBNull.Value);

		//				dbCommand.Parameters.AddWithValue("pvar_chequedddate", NpgsqlDbType.Date, (object)model.chequedddate ?? DBNull.Value);

		//				dbCommand.Parameters.AddWithValue("pvar_paymentstatus", NpgsqlDbType.Varchar, (object)model.paymentstatus ?? DBNull.Value);

		//				dbCommand.Parameters.AddWithValue("pvar_collectedby", NpgsqlDbType.Uuid, (object)model.collectedby ?? DBNull.Value);

		//				dbCommand.Parameters.AddWithValue("pvar_refundmode", NpgsqlDbType.Varchar, (object)model.refundmode ?? DBNull.Value);

		//				dbCommand.Parameters.AddWithValue("pvar_refundedamount", NpgsqlDbType.Numeric, (object)model.refundedamount ?? DBNull.Value);

		//				dbCommand.Parameters.AddWithValue("pvar_refundedby", NpgsqlDbType.Uuid, (object)model.refundedby ?? DBNull.Value);

		//				dbCommand.Parameters.AddWithValue("pvar_refundreferencenumber", NpgsqlDbType.Varchar, (object)model.refundreferencenumber ?? DBNull.Value);

		//				dbCommand.Parameters.AddWithValue("pvar_refundbankname", NpgsqlDbType.Varchar, (object)model.refundbankname ?? DBNull.Value);

		//				dbCommand.Parameters.AddWithValue("pvar_refundreason", NpgsqlDbType.Varchar, (object)model.refundreason ?? DBNull.Value);

		//				dbCommand.Parameters.AddWithValue("pvar_refundstatus", NpgsqlDbType.Varchar, (object)model.refundstatus ?? DBNull.Value);

		//				dbCommand.Parameters.AddWithValue("pvar_counterid", NpgsqlDbType.Varchar, (object)model.counterid ?? DBNull.Value);

		//				dbCommand.Parameters.AddWithValue("pvar_remarks", NpgsqlDbType.Varchar, (object)model.remarks ?? DBNull.Value);
		//				dbCommand.Parameters.AddWithValue("pvar_modifieduser", NpgsqlDbType.Uuid, model.modifieduser);

		//				NpgsqlParameter outParm = new NpgsqlParameter("@returnMessage", NpgsqlDbType.Varchar)
		//				{
		//					Direction = ParameterDirection.Output
		//				};
		//				dbCommand.Parameters.Add(outParm);

		//				dbCommand.ExecuteNonQuery();
		//				ResponseMessage = outParm.Value.ToString();
		//				if (dbCommand.Connection.State != ConnectionState.Closed)
		//				{
		//					dbCommand.Connection.Dispose();
		//				}

		//			}
		//			npsql.Close();
		//		}

		//	}
		//	catch (Exception ex)
		//	{
		//		ResponseMessage = ex.Message;
		//	}

		//	return ResponseMessage;

		//}
		public virtual string Update_Billing_Payment_(BillingPaymentModel model)
		{
			String ResponseMessage = "";
			try
			{

				using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
				{
					npsql.Open();
					using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"Update_Billing_Payment\"(@pvar_billingpaymentid,@pvar_tenantid,@pvar_receiptno,@pvar_receivablefor,@pvar_patientvisit,@pvar_patientname,@pvar_amount,@pvar_paymentmode,@pvar_transactionreference,@pvar_paymentstatus,@pvar_refundmode,@pvar_refundedamount,@pvar_refundedby,@pvar_refundreason,@pvar_collectedby,@pvar_counterid,@pvar_remarks,@pvar_modifieduser)", npsql))
					{
						dbCommand.CommandType = CommandType.Text;
						dbCommand.Parameters.AddWithValue("pvar_billingpaymentid", NpgsqlDbType.Uuid, (object)model.BillingPaymentid ?? DBNull.Value);
						dbCommand.Parameters.AddWithValue("pvar_tenantid", NpgsqlDbType.Uuid, (object)model.tenantid ?? DBNull.Value);

						dbCommand.Parameters.AddWithValue("pvar_receiptno", NpgsqlDbType.Varchar, (object)model.receiptno ?? DBNull.Value);

						dbCommand.Parameters.AddWithValue("pvar_receivablefor", NpgsqlDbType.Varchar, (object)model.receivablefor ?? DBNull.Value);

						dbCommand.Parameters.AddWithValue("pvar_patientvisit", NpgsqlDbType.Uuid, (object)model.patientvisit ?? DBNull.Value);

						dbCommand.Parameters.AddWithValue("pvar_patientname", NpgsqlDbType.Uuid, (object)model.patientname ?? DBNull.Value);

						dbCommand.Parameters.AddWithValue("pvar_amount", NpgsqlDbType.Numeric, (object)model.amount ?? DBNull.Value);

						dbCommand.Parameters.AddWithValue("pvar_paymentmode", NpgsqlDbType.Varchar, (object)model.paymentmode ?? DBNull.Value);

						dbCommand.Parameters.AddWithValue("pvar_transactionreference", NpgsqlDbType.Varchar, (object)model.transactionreference ?? DBNull.Value);

						dbCommand.Parameters.AddWithValue("pvar_paymentstatus", NpgsqlDbType.Varchar, (object)model.paymentstatus ?? DBNull.Value);
						dbCommand.Parameters.AddWithValue("pvar_refundmode", NpgsqlDbType.Varchar, (object)model.refundmode ?? DBNull.Value);

						dbCommand.Parameters.AddWithValue("pvar_refundedamount", NpgsqlDbType.Numeric, (object)model.refundedamount ?? DBNull.Value);

						dbCommand.Parameters.AddWithValue("pvar_refundedby", NpgsqlDbType.Uuid, (object)model.refundedby ?? DBNull.Value);

						dbCommand.Parameters.AddWithValue("pvar_refundreason", NpgsqlDbType.Varchar, (object)model.refundreason ?? DBNull.Value);

						dbCommand.Parameters.AddWithValue("pvar_collectedby", NpgsqlDbType.Uuid, (object)model.collectedby ?? DBNull.Value);

						dbCommand.Parameters.AddWithValue("pvar_counterid", NpgsqlDbType.Varchar, (object)model.counterid ?? DBNull.Value);

						dbCommand.Parameters.AddWithValue("pvar_remarks", NpgsqlDbType.Varchar, (object)model.remarks ?? DBNull.Value);
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
		public virtual string Remove_Billing_Payment(string id, string loginUserID)
		{
			String ResponseMessage = "";
			try
			{
				using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
				{
					npsql.Open();
					using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"Remove_Billing_Payment\"(@pvar_billingpaymentid,@pvar_modifieduser)", npsql))
					{
						dbCommand.CommandType = CommandType.Text;
						dbCommand.Parameters.AddWithValue("pvar_billingpaymentid", (object)id ?? DBNull.Value);
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
		public virtual System.Data.DataTable lookup_BillingPayment_refundedby(String tenantid)
		{
			DataSet dataSet = new DataSet();
			DataTable dataTable = new DataTable();
			try
			{

				using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
				{
					npsql.Open();
					using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"lookup_BillingPayment_refundedby\"(@pvar_tenantid)", npsql))
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

		//		public virtual JObject Billing_Payment_List(string tenantid
		//, string receivablefor
		//, string patientname
		//, string ipdnumber
		//, string opdnumber
		//, string currency
		//, string paymentmode
		//, string paymentstatus
		//, string collectedby
		//, string refundstatus
		//, int? pagesize = 1000, int? pagenumber = 0, string searchterm = "", string sort_fields = "")
		//		{
		//			object dalResponse = null;

		//			try
		//			{

		//				using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
		//				{

		//					using (var dbCommand = new NpgsqlCommand("SELECT * FROM\"Billing_Payment_List\"(@pvar_tenantid,@pvar_receivablefor,@pvar_patientname,@pvar_ipdnumber,@pvar_opdnumber,@pvar_currency,@pvar_paymentmode,@pvar_paymentstatus,@pvar_collectedby,@pvar_refundstatus,@pvar_pagesize,@pvar_pagenumber,@pvar_searchterm,@pvar_sort_fields)", npsql))
		//					{
		//						dbCommand.CommandType = CommandType.Text;
		//						dbCommand.Parameters.AddWithValue("pvar_tenantid", (object)tenantid ?? DBNull.Value);
		//						dbCommand.Parameters.AddWithValue("pvar_receivablefor", (object)receivablefor ?? DBNull.Value);
		//						dbCommand.Parameters.AddWithValue("pvar_patientname", (object)patientname ?? DBNull.Value);
		//						dbCommand.Parameters.AddWithValue("pvar_ipdnumber", (object)ipdnumber ?? DBNull.Value);
		//						dbCommand.Parameters.AddWithValue("pvar_opdnumber", (object)opdnumber ?? DBNull.Value);
		//						dbCommand.Parameters.AddWithValue("pvar_currency", (object)currency ?? DBNull.Value);
		//						dbCommand.Parameters.AddWithValue("pvar_paymentmode", (object)paymentmode ?? DBNull.Value);
		//						dbCommand.Parameters.AddWithValue("pvar_paymentstatus", (object)paymentstatus ?? DBNull.Value);
		//						dbCommand.Parameters.AddWithValue("pvar_collectedby", (object)collectedby ?? DBNull.Value);
		//						dbCommand.Parameters.AddWithValue("pvar_refundstatus", (object)refundstatus ?? DBNull.Value);

		//						dbCommand.Parameters.AddWithValue("pvar_pagesize", (object)pagesize ?? DBNull.Value);
		//						dbCommand.Parameters.AddWithValue("pvar_pagenumber", (object)pagenumber ?? DBNull.Value);

		//						dbCommand.Parameters.AddWithValue("pvar_searchterm", (object)searchterm ?? DBNull.Value);
		//						if (sort_fields != null && sort_fields.Length > 2)
		//							dbCommand.Parameters.AddWithValue("pvar_sort_fields", NpgsqlDbType.Json, sort_fields);
		//						else
		//							dbCommand.Parameters.AddWithValue("pvar_sort_fields", NpgsqlDbType.Json, DBNull.Value);


		//						npsql.Open();
		//						dalResponse = dbCommand.ExecuteScalar();
		//						npsql.Close();

		//					}

		//				}



		//			}
		//			catch
		//			{
		//				throw;
		//			}


		//			return JObject.Parse(dalResponse.ToString());




		//		}

		public virtual JObject Billing_Payment_List(string tenantid
, string paymentdate_automatonfrom
, string paymentdate_automatonto
, string patientname
, string receivablefor
, int? pagesize = 1000, int? pagenumber = 0, string searchterm = "", string sort_fields = "")
		{
			object dalResponse = null;

			try
			{

				using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
				{

					using (var dbCommand = new NpgsqlCommand("SELECT * FROM\"Billing_Payment_List\"(@pvar_tenantid,@pvar_paymentdate_automatonfrom,@pvar_paymentdate_automatonto,@pvar_patientname,@pvar_receivablefor,@pvar_pagesize,@pvar_pagenumber,@pvar_searchterm,@pvar_sort_fields)", npsql))
					{
						dbCommand.CommandType = CommandType.Text;
						dbCommand.Parameters.AddWithValue("pvar_tenantid", (object)tenantid ?? DBNull.Value);
						dbCommand.Parameters.AddWithValue("pvar_paymentdate_automatonfrom", (object)paymentdate_automatonfrom ?? DBNull.Value);
						dbCommand.Parameters.AddWithValue("pvar_paymentdate_automatonto", (object)paymentdate_automatonto ?? DBNull.Value);
						dbCommand.Parameters.AddWithValue("pvar_patientname", (object)patientname ?? DBNull.Value);
						dbCommand.Parameters.AddWithValue("pvar_receivablefor", (object)receivablefor ?? DBNull.Value);

						dbCommand.Parameters.AddWithValue("pvar_pagesize", (object)pagesize ?? DBNull.Value);
						dbCommand.Parameters.AddWithValue("pvar_pagenumber", (object)pagenumber ?? DBNull.Value);

						dbCommand.Parameters.AddWithValue("pvar_searchterm", (object)searchterm ?? DBNull.Value);
						if (sort_fields != null && sort_fields.Length > 2)
							dbCommand.Parameters.AddWithValue("pvar_sort_fields", NpgsqlDbType.Json, sort_fields);
						else
							dbCommand.Parameters.AddWithValue("pvar_sort_fields", NpgsqlDbType.Json, DBNull.Value);


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

		public virtual System.Data.DataTable Get_HealthSeeker_Payments(Guid? tenantid,Guid patientid,string paymentdate_automatonfrom,string paymentdate_automatonto,int? pagesize = 200,int? pagenumber = 0)
		{
			var dataSet = new DataSet();
			var dataTable = new DataTable();
			using (var npsql = new NpgsqlConnection(db_connectionstring))
			{
				npsql.Open();
				using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"Get_HealthSeeker_Payments\"(@pvar_tenantid,@pvar_patientid,@pvar_paymentdate_automatonfrom,@pvar_paymentdate_automatonto,@pvar_pagesize,@pvar_pagenumber)", npsql))
				{
					dbCommand.CommandType = CommandType.Text;
					dbCommand.Parameters.AddWithValue("pvar_tenantid", NpgsqlDbType.Uuid, (object)tenantid ?? DBNull.Value);
					dbCommand.Parameters.AddWithValue("pvar_patientid", NpgsqlDbType.Uuid, patientid);
					dbCommand.Parameters.AddWithValue("pvar_paymentdate_automatonfrom", NpgsqlDbType.Varchar, (object)paymentdate_automatonfrom ?? DBNull.Value);
					dbCommand.Parameters.AddWithValue("pvar_paymentdate_automatonto", NpgsqlDbType.Varchar, (object)paymentdate_automatonto ?? DBNull.Value);
					dbCommand.Parameters.AddWithValue("pvar_pagesize", NpgsqlDbType.Integer, (object)pagesize ?? DBNull.Value);
					dbCommand.Parameters.AddWithValue("pvar_pagenumber", NpgsqlDbType.Integer, (object)pagenumber ?? DBNull.Value);
					using (var dataAdapter = new NpgsqlDataAdapter(dbCommand))
					{
						dataSet.Reset();
						dataAdapter.Fill(dataSet);
						dataTable = dataSet.Tables[0];
					}
				}
			}
			return dataTable;
		}


		public virtual System.Data.DataTable get_all_BillingPayment(string tenantid, string searchterm = "", int? pagesize = 1000, int? pagenumber = 0)
		{

			DataTable dataTable = new DataTable();
			DataSet dataSet = new DataSet();

			try
			{

				using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
				{
					npsql.Open();
					using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"get_all_BillingPayment\"(@pvar_tenantid)", npsql))
					{
						dbCommand.CommandType = CommandType.Text;
						dbCommand.Parameters.AddWithValue("pvar_tenantid", (object)tenantid ?? DBNull.Value);
						dbCommand.Parameters.AddWithValue("pvar_searchterm", (object)searchterm ?? DBNull.Value);
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


			}
			catch
			{
				throw;
			}
			return dataTable;




		}
		public virtual System.Data.DataTable getById_allinfo_BillingPayment(string BillingPaymentid)
		{
			DataSet dataSet = new DataSet();
			DataTable dataTable = new DataTable();
			try
			{

				using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
				{
					npsql.Open();
					using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"getById_sp_all_BillingPayment\"(@pvar_billingpaymentid)", npsql))
					{
						dbCommand.CommandType = CommandType.Text;
						dbCommand.Parameters.AddWithValue("pvar_billingpaymentid", (object)BillingPaymentid ?? DBNull.Value);
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

		public virtual System.Data.DataTable lookup_BillingPayment_patientvisit(String tenantid, String patientname, string searchterm = "", int? pagesize = 1000, int? pagenumber = 0)
		{
			DataSet dataSet = new DataSet();
			DataTable dataTable = new DataTable();
			try
			{

				using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
				{
					npsql.Open();
					using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"lookup_BillingPayment_patientvisit\"(@pvar_tenantid,@pvar_patientname,@pvar_searchterm,@pvar_pagesize,@pvar_pagenumber)", npsql))
					{

						dbCommand.Parameters.AddWithValue("pvar_tenantid", (object)tenantid ?? DBNull.Value); dbCommand.Parameters.AddWithValue("pvar_patientname", (object)patientname ?? DBNull.Value);
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

		public virtual System.Data.DataTable lookup_BillingPayment_patientname(String tenantid, string searchterm = "", int? pagesize = 1000, int? pagenumber = 0)
		{
			DataSet dataSet = new DataSet();
			DataTable dataTable = new DataTable();
			try
			{

				using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
				{
					npsql.Open();
					using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"lookup_BillingPayment_patientname\"(@pvar_tenantid,@pvar_searchterm,@pvar_pagesize,@pvar_pagenumber)", npsql))
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
		public virtual System.Data.DataTable lookup_BillingPayment_collectedby(String tenantid)
		{
			DataSet dataSet = new DataSet();
			DataTable dataTable = new DataTable();
			try
			{

				using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
				{
					npsql.Open();
					using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"lookup_BillingPayment_collectedby\"(@pvar_tenantid)", npsql))
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


		public virtual System.Data.DataTable lookup_change_BillingPayment_patientvisit(string PatientVisitid)
		{
			DataTable dataTable = new DataTable();
			DataSet dataSet = new DataSet();
			try
			{

				using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
				{
					npsql.Open();
					using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"lookup_change_BillingPayment_patientvisit\"(@pvar_patientvisitid)", npsql))
					{
						dbCommand.CommandType = CommandType.Text;
						dbCommand.Parameters.AddWithValue("pvar_patientvisitid", NpgsqlDbType.Varchar, (object)PatientVisitid ?? DBNull.Value);
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







		/// <summary>Get all billing payments for an IPD application (by IPDApplicationFormid).</summary>
		public virtual System.Data.DataTable Get_Billing_Payments_For_IPD(string IPDApplicationFormid, string tenantid)
		{
			DataTable dataTable = new DataTable();
			DataSet dataSet = new DataSet();
			try
			{
				using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
				{
					npsql.Open();
					using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"Get_Billing_Payments_For_IPD\"(@pvar_ipdid)", npsql))
					{
						dbCommand.CommandType = System.Data.CommandType.Text;
						dbCommand.Parameters.AddWithValue("pvar_ipdid", NpgsqlTypes.NpgsqlDbType.Varchar, (object)IPDApplicationFormid ?? DBNull.Value);
						using (NpgsqlDataAdapter dataAdapter = new NpgsqlDataAdapter(dbCommand))
						{
							dataSet.Reset();
							dataAdapter.Fill(dataSet);
							if (dataSet.Tables.Count > 0) dataTable = dataSet.Tables[0];
						}
					}
					npsql.Close();
				}
			}
			catch { throw; }
			return dataTable;
		}


		/// <summary>Returns individual BillingPayment rows for an OPD form (by opdnumber).</summary>
		public virtual System.Data.DataTable Get_Billing_Payments_For_OPD(string OPDFormid)
		{
			DataTable dataTable = new DataTable();
			DataSet dataSet = new DataSet();
			try
			{
				using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
				{
					npsql.Open();
					using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"Get_Billing_Payments_For_OPD\"(@pvar_opdid)", npsql))
					{
						dbCommand.CommandType = System.Data.CommandType.Text;
						dbCommand.Parameters.AddWithValue("pvar_opdid", NpgsqlTypes.NpgsqlDbType.Varchar, (object)OPDFormid ?? DBNull.Value);
						using (NpgsqlDataAdapter dataAdapter = new NpgsqlDataAdapter(dbCommand))
						{
							dataSet.Reset();
							dataAdapter.Fill(dataSet);
							if (dataSet.Tables.Count > 0) dataTable = dataSet.Tables[0];
						}
					}
					npsql.Close();
				}
			}
			catch { throw; }
			return dataTable;
		}

		/// <summary>Returns total billed, total paid, and balance for an OPD form by looking up BillingPayment.opdnumber.</summary>
		public virtual System.Data.DataTable Get_OPD_Billing_Summary_By_OPDForm(string OPDFormid)
		{
			DataTable dataTable = new DataTable();
			DataSet dataSet = new DataSet();
			try
			{
				using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
				{
					npsql.Open();
					using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"Get_OPD_Billing_Summary_By_OPDForm\"(@pvar_opdformid)", npsql))
					{
						dbCommand.CommandType = CommandType.Text;
						Guid opdformID = Guid.Parse(OPDFormid);
						dbCommand.Parameters.AddWithValue("pvar_opdformid", NpgsqlDbType.Uuid, opdformID);
						using (NpgsqlDataAdapter dataAdapter = new NpgsqlDataAdapter(dbCommand))
						{
							dataSet.Reset();
							dataAdapter.Fill(dataSet);
							if (dataSet.Tables.Count > 0) dataTable = dataSet.Tables[0];
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

		/// <summary>Returns a billing summary for an OPD patient visit (consultation + additional charges, paid vs balance).</summary>
		public virtual System.Data.DataTable Get_OPD_Billing_Summary(string PatientVisitid)
		{
			DataTable dataTable = new DataTable();
			DataSet dataSet = new DataSet();
			try
			{
				using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
				{
					npsql.Open();
					using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"Get_OPD_Billing_Summary\"(@pvar_patientvisitid)", npsql))
					{
						dbCommand.CommandType = CommandType.Text;
						dbCommand.Parameters.AddWithValue("pvar_patientvisitid", NpgsqlTypes.NpgsqlDbType.Uuid, (object)PatientVisitid ?? DBNull.Value);
						using (NpgsqlDataAdapter dataAdapter = new NpgsqlDataAdapter(dbCommand))
						{
							dataSet.Reset();
							dataAdapter.Fill(dataSet);
							if (dataSet.Tables.Count > 0) dataTable = dataSet.Tables[0];
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

		public virtual System.Data.DataTable lookup_BillingPayment_ipdnumber(String tenantid, String patientname, string searchterm = "", int? pagesize = 1000, int? pagenumber = 0)
		{
			DataSet dataSet = new DataSet();
			DataTable dataTable = new DataTable();
			try
			{

				using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
				{
					npsql.Open();
					using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"lookup_BillingPayment_ipdnumber\"(@pvar_tenantid,@pvar_patientname,@pvar_searchterm,@pvar_pagesize,@pvar_pagenumber)", npsql))
					{

						dbCommand.Parameters.AddWithValue("pvar_tenantid", (object)tenantid ?? DBNull.Value); dbCommand.Parameters.AddWithValue("pvar_patientname", (object)patientname ?? DBNull.Value);
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
		public virtual System.Data.DataTable lookup_BillingPayment_opdnumber(String tenantid, String patientname, string searchterm = "", int? pagesize = 1000, int? pagenumber = 0)
		{
			DataSet dataSet = new DataSet();
			DataTable dataTable = new DataTable();
			try
			{

				using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
				{
					npsql.Open();
					using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"lookup_BillingPayment_opdnumber\"(@pvar_tenantid,@pvar_patientname,@pvar_searchterm,@pvar_pagesize,@pvar_pagenumber)", npsql))
					{

						dbCommand.Parameters.AddWithValue("pvar_tenantid", (object)tenantid ?? DBNull.Value); dbCommand.Parameters.AddWithValue("pvar_patientname", (object)patientname ?? DBNull.Value);
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
		public virtual System.Data.DataTable lookup_BillingPayment_therapy(String tenantid)
		{
			DataSet dataSet = new DataSet();
			DataTable dataTable = new DataTable();
			try
			{

				using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
				{
					npsql.Open();
					using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"lookup_BillingPayment_therapy\"(@pvar_tenantid)", npsql))
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
		public virtual System.Data.DataTable lookup_BillingPayment_therapykit(String tenantid)
		{
			DataSet dataSet = new DataSet();
			DataTable dataTable = new DataTable();
			try
			{

				using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
				{
					npsql.Open();
					using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"lookup_BillingPayment_therapykit\"(@pvar_tenantid)", npsql))
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
		public virtual System.Data.DataTable lookup_BillingPayment_medicine(String tenantid, string searchterm = "", int? pagesize = 1000, int? pagenumber = 0)
		{
			DataSet dataSet = new DataSet();
			DataTable dataTable = new DataTable();
			try
			{

				using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
				{
					npsql.Open();
					using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"lookup_BillingPayment_medicine\"(@pvar_tenantid,@pvar_searchterm,@pvar_pagesize,@pvar_pagenumber)", npsql))
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
		public virtual System.Data.DataTable lookup_BillingPayment_room(String tenantid)
		{
			DataSet dataSet = new DataSet();
			DataTable dataTable = new DataTable();
			try
			{

				using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
				{
					npsql.Open();
					using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"lookup_BillingPayment_room\"(@pvar_tenantid)", npsql))
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
		public virtual System.Data.DataTable lookup_change_BillingPayment_medicine(string Medicineid)
		{
			DataTable dataTable = new DataTable();
			DataSet dataSet = new DataSet();
			try
			{

				using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
				{
					npsql.Open();
					using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"lookup_change_BillingPayment_medicine\"(@pvar_medicineid)", npsql))
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

		public virtual System.Data.DataTable lookup_change_BillingPayment_therapykit(string TherapyKitid)
		{
			DataTable dataTable = new DataTable();
			DataSet dataSet = new DataSet();
			try
			{

				using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
				{
					npsql.Open();
					using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"lookup_change_BillingPayment_therapykit\"(@pvar_therapykitid)", npsql))
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

		public virtual System.Data.DataTable lookup_change_BillingPayment_therapy(string Therapiesid)
		{
			DataTable dataTable = new DataTable();
			DataSet dataSet = new DataSet();
			try
			{

				using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
				{
					npsql.Open();
					using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"lookup_change_BillingPayment_therapy\"(@pvar_therapiesid)", npsql))
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

		public virtual DataTable Get_Unified_Pending_Receivables(Guid patientID, Guid? ipdNo, string type)
		{
			var dataSet = new DataSet();
			var dataTable = new DataTable();
			try
			{
				using (var npsql = new NpgsqlConnection(db_connectionstring))
				{
					npsql.Open();
					if (ipdNo.HasValue && string.Equals(type, "IPD", StringComparison.OrdinalIgnoreCase))
					{
						ApplyOutstandingIPDDiscounts(npsql, ipdNo.Value);
					}
					using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"Get_Unified_Pending_Receivables_IPD\"(@pvar_patientid,@pvar_ipdno,@pvar_type)", npsql))
					{
						dbCommand.CommandType = CommandType.Text;
						dbCommand.Parameters.AddWithValue("pvar_patientid", NpgsqlDbType.Uuid, patientID);
						dbCommand.Parameters.AddWithValue("pvar_ipdno", NpgsqlDbType.Uuid, (object)ipdNo ?? DBNull.Value);
						dbCommand.Parameters.AddWithValue("pvar_type", NpgsqlDbType.Varchar, string.IsNullOrWhiteSpace(type) ? "OPD" : type.ToUpperInvariant());
						using (var dataAdapter = new NpgsqlDataAdapter(dbCommand))
						{
							dataSet.Reset();
							dataAdapter.Fill(dataSet);
							dataTable = dataSet.Tables[0];
						}
					}

					// The legacy database function returns only positive payable rows.  IPD
					// discounts are stored as negative receivables, so omitting them makes both
					// front-desk collection and the health-seeker Razorpay order too high.
					// Merge outstanding credits into the same result so every consumer can net
					// the complete ledger.  Keep this compatibility merge even after the SQL
					// function is upgraded; the receivable-id check prevents duplicates.
					if (ipdNo.HasValue && string.Equals(type, "IPD", StringComparison.OrdinalIgnoreCase))
					{
						MergeOutstandingIPDCredits(dataTable, npsql, ipdNo.Value);
						MergeAdjustedIPDBookingDeposits(dataTable, npsql, patientID, ipdNo.Value);
						MergeOutstandingIPDOtherReceivables(dataTable, npsql, patientID, ipdNo.Value);
					}
				}
			}
			catch
			{
				throw;
			}
			return dataTable;
		}

		private static void ApplyOutstandingIPDDiscounts(NpgsqlConnection connection, Guid ipdNo)
		{
			using (var command = new NpgsqlCommand(
				"SELECT \"Apply_Outstanding_IPD_Discounts\"(@pvar_ipdid)", connection))
			{
				command.Parameters.AddWithValue("pvar_ipdid", NpgsqlDbType.Uuid, ipdNo);
				command.ExecuteNonQuery();
			}
		}

		private static void MergeOutstandingIPDCredits(DataTable target, NpgsqlConnection connection, Guid ipdNo)
		{
			if (target == null || target.Columns.Count == 0) return;

			var existingIds = new HashSet<Guid>();
			foreach (DataRow row in target.Rows)
			{
				if (Guid.TryParse(Convert.ToString(GetColumnValue(row, "Receivableid")), out var id))
					existingIds.Add(id);
			}

			using (var command = new NpgsqlCommand(
				"SELECT * FROM \"Get_Outstanding_IPD_Credits\"(@pvar_ipdid)", connection))
			{
				command.Parameters.AddWithValue("pvar_ipdid", NpgsqlDbType.Uuid, ipdNo);
				using (var reader = command.ExecuteReader())
				{
					while (reader.Read())
					{
						var receivableId = reader.GetGuid(reader.GetOrdinal("receivableid"));
						if (existingIds.Contains(receivableId)) continue;

						var row = target.NewRow();
						SetColumnValue(row, "Receivableid", receivableId);
						SetColumnValue(row, "receivabledate", reader["receivabledate"]);
						SetColumnValue(row, "receivablefor", reader["receivablefor"]);
						SetColumnValue(row, "billdetails", reader["remarks"]);
						SetColumnValue(row, "remarks", reader["remarks"]);
						SetColumnValue(row, "category", "IPD");
						SetColumnValue(row, "amount", reader["amount"]);
						SetColumnValue(row, "paidamount", reader["paidamount"]);
						SetColumnValue(row, "balance", reader["balance"]);
						SetColumnValue(row, "ismandatory", reader["ismandatory"]);
						target.Rows.Add(row);
						existingIds.Add(receivableId);
					}
				}
			}
		}

		private static void MergeOutstandingIPDOtherReceivables(
			DataTable target,
			NpgsqlConnection connection,
			Guid patientId,
			Guid ipdNo)
		{
			if (target == null || target.Columns.Count == 0) return;

			var existingRows = new Dictionary<Guid, DataRow>();
			foreach (DataRow row in target.Rows)
			{
				if (Guid.TryParse(Convert.ToString(GetColumnValue(row, "Receivableid")), out var id))
					existingRows[id] = row;
			}

			using (var command = new NpgsqlCommand(
				"SELECT * FROM \"Get_Outstanding_IPD_Other_Receivables\"(@pvar_patientid,@pvar_ipdid)",
				connection))
			{
				command.Parameters.AddWithValue("pvar_ipdid", NpgsqlDbType.Uuid, ipdNo);
				command.Parameters.AddWithValue("pvar_patientid", NpgsqlDbType.Uuid, patientId);
				using (var reader = command.ExecuteReader())
				{
					while (reader.Read())
					{
						var receivableId = reader.GetGuid(reader.GetOrdinal("receivableid"));
						var displayReceivableFor = Convert.ToString(reader["displayreceivablefor"]);

						if (existingRows.TryGetValue(receivableId, out var existingRow))
						{
							SetColumnValue(existingRow, "receivablefor", displayReceivableFor);
							continue;
						}

						var row = target.NewRow();
						SetColumnValue(row, "Receivableid", receivableId);
						SetColumnValue(row, "receivabledate", reader["receivabledate"]);
						SetColumnValue(row, "receivablefor", displayReceivableFor);
						SetColumnValue(row, "billdetails", reader["remarks"]);
						SetColumnValue(row, "remarks", reader["remarks"]);
						SetColumnValue(row, "category", "IPD");
						SetColumnValue(row, "amount", reader["amount"]);
						SetColumnValue(row, "paidamount", reader["paidamount"]);
						SetColumnValue(row, "balance", reader["balance"]);
						SetColumnValue(row, "ismandatory", reader["ismandatory"]);
						target.Rows.Add(row);
						existingRows[receivableId] = row;
					}
				}
			}
		}

		/// <summary>
		/// The legacy pending-receivables function omits a booking deposit as soon
		/// as a discount has fully adjusted it.  Keep that zero-balance ledger row
		/// in the collection summary so gross charges and adjustments reconcile to
		/// the billing statement (for example 100 deposit + 300 room - 200 discount).
		/// </summary>
		private static void MergeAdjustedIPDBookingDeposits(
			DataTable target,
			NpgsqlConnection connection,
			Guid patientId,
			Guid ipdNo)
		{
			if (target == null || target.Columns.Count == 0) return;

			var existingIds = new HashSet<Guid>();
			foreach (DataRow row in target.Rows)
			{
				if (Guid.TryParse(Convert.ToString(GetColumnValue(row, "Receivableid")), out var id))
					existingIds.Add(id);
			}

			using (var command = new NpgsqlCommand(
				"SELECT * FROM \"Get_Adjusted_IPD_Booking_Deposits\"(@pvar_patientid,@pvar_ipdid)",
				connection))
			{
				command.Parameters.AddWithValue("pvar_ipdid", NpgsqlDbType.Uuid, ipdNo);
				command.Parameters.AddWithValue("pvar_patientid", NpgsqlDbType.Uuid, patientId);
				using (var reader = command.ExecuteReader())
				{
					while (reader.Read())
					{
						var receivableId = reader.GetGuid(reader.GetOrdinal("receivableid"));
						if (existingIds.Contains(receivableId)) continue;

						var row = target.NewRow();
						SetColumnValue(row, "Receivableid", receivableId);
						SetColumnValue(row, "receivabledate", reader["receivabledate"]);
						SetColumnValue(row, "receivablefor", reader["receivablefor"]);
						SetColumnValue(row, "billdetails", reader["remarks"]);
						SetColumnValue(row, "remarks", reader["remarks"]);
						SetColumnValue(row, "category", "IPD");
						SetColumnValue(row, "amount", reader["amount"]);
						SetColumnValue(row, "paidamount", reader["paidamount"]);
						SetColumnValue(row, "balance", reader["balance"]);
						SetColumnValue(row, "ismandatory", reader["ismandatory"]);
						target.Rows.Add(row);
						existingIds.Add(receivableId);
					}
				}
			}
		}

		private static object GetColumnValue(DataRow row, string columnName)
		{
			var column = row.Table.Columns.Cast<DataColumn>()
				.FirstOrDefault(x => string.Equals(x.ColumnName, columnName, StringComparison.OrdinalIgnoreCase));
			return column == null ? null : row[column];
		}

		private static void SetColumnValue(DataRow row, string columnName, object value)
		{
			var column = row.Table.Columns.Cast<DataColumn>()
				.FirstOrDefault(x => string.Equals(x.ColumnName, columnName, StringComparison.OrdinalIgnoreCase));
			if (column != null)
				row[column] = value ?? DBNull.Value;
		}

		private static bool IsRazorpayPaymentMode(string paymentMode)
		{
			return !string.IsNullOrWhiteSpace(paymentMode)
				&& paymentMode.IndexOf("razor", StringComparison.OrdinalIgnoreCase) >= 0;
		}

		private static bool IsSuccessfulUnifiedCollection(UnifiedPaymentPostModel model)
		{
			if (model == null || (model.receivedamount ?? 0) <= 0) return false;
			var status = string.IsNullOrWhiteSpace(model.paymentstatus) ? "Success" : model.paymentstatus;
			return string.Equals(status, "Success", StringComparison.OrdinalIgnoreCase)
				|| string.Equals(status, "Paid", StringComparison.OrdinalIgnoreCase)
				|| string.Equals(status, "Completed", StringComparison.OrdinalIgnoreCase);
		}

		public virtual string Apply_Unified_Receivable_Payments(List<UnifiedPaymentInputModel> payments, Guid modifiedUser)
		{
			if (payments == null || payments.Count == 0) return "No payment rows submitted";
			try
			{
				using (var npsql = new NpgsqlConnection(db_connectionstring))
				{
					npsql.Open();
					using (var transaction = npsql.BeginTransaction())
					{
						try
						{
							foreach (var item in payments.Where(x => x != null && x.PayNow > 0))
							{
								using (var cmd = new NpgsqlCommand("SELECT \"Apply_Unified_Receivable_Payment\"(@pvar_receivableid,@pvar_paynow,@pvar_modifieduser)", npsql, transaction))
								{
									cmd.Parameters.AddWithValue("pvar_paynow", NpgsqlDbType.Numeric, item.PayNow);
									cmd.Parameters.AddWithValue("pvar_modifieduser", NpgsqlDbType.Uuid, modifiedUser);
									cmd.Parameters.AddWithValue("pvar_receivableid", NpgsqlDbType.Uuid, item.Receivableid);
									cmd.ExecuteNonQuery();
								}
							}

							transaction.Commit();
							return "201.1";
						}
						catch (Exception exTrans)
						{
							transaction.Rollback();
							return exTrans.Message;
						}
					}
				}
			}
			catch (Exception ex)
			{
				return ex.Message;
			}
		}
        public List<UnifiedRoomCostSummaryModel> GetUnifiedRoomCostSummary(Guid ipdFormId)
        {
            var result = new List<UnifiedRoomCostSummaryModel>();

            using (var conn = new NpgsqlConnection(db_connectionstring))
            {
                conn.Open();

                using (var cmd = new NpgsqlCommand(@"
            SELECT
                r.receivabledate::date AS receivabledate,
                CASE
                    WHEN COALESCE(r.remarks, '') ILIKE '%Patient, Attendant%' THEN 'Patient + Attendant'
                    WHEN COALESCE(r.remarks, '') ILIKE '%Attendant%' THEN 'Attendant'
                    WHEN COALESCE(r.remarks, '') ILIKE '%Patient%' THEN 'Patient'
                    ELSE ''
                END AS allottedto,
                COALESCE(rm.roomnumber, '') AS roomno,
                COALESCE(r.amount, 0) AS costperday,
                COALESCE(r.amount, 0) AS lineitemtotal,
                COALESCE(r.paidamount, 0) AS paidamount,
                GREATEST(COALESCE(r.amount, 0) - COALESCE(r.paidamount, 0), 0) AS balanceamount,
                GREATEST(COALESCE(r.paidamount, 0) - COALESCE(r.amount, 0), 0) AS creditamount,
                CASE
                    WHEN COALESCE(r.paidamount, 0) >= COALESCE(r.amount, 0) THEN 'Paid'
                    WHEN COALESCE(r.paidamount, 0) > 0 THEN 'Partially Paid'
                    ELSE 'Pending'
                END AS status
            FROM receivable r
            LEFT JOIN room rm
                ON rm.roomid = r.room
            WHERE r.ipdnumber = @ipdFormId
              AND LOWER(COALESCE(r.receivablefor, '')) = 'room'
              AND COALESCE(r.isdeleted, false) = false
            ORDER BY r.receivabledate::date, r.remarks;
        ", conn))
                {
                    cmd.Parameters.AddWithValue("@ipdFormId", NpgsqlDbType.Uuid, ipdFormId);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            result.Add(new UnifiedRoomCostSummaryModel
                            {
                                ReceivableDate = Convert.ToDateTime(reader["receivabledate"]),
                                AllottedTo = Convert.ToString(reader["allottedto"]) ?? "",
                                RoomNo = Convert.ToString(reader["roomno"]) ?? "",
                                CostPerDay = reader["costperday"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["costperday"]),
                                LineItemTotal = reader["lineitemtotal"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["lineitemtotal"]),
                                PaidAmount = reader["paidamount"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["paidamount"]),
                                BalanceAmount = reader["balanceamount"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["balanceamount"]),
                                CreditAmount = reader["creditamount"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["creditamount"]),
                                Status = Convert.ToString(reader["status"]) ?? ""
                            });
                        }
                    }
                }
            }

            return result;
        }
        public virtual string Add_Unified_Payment_Collection(UnifiedPaymentPostModel model, Guid createdUser)
		{
			if (model == null) return "No payment payload submitted";
			try
			{
				var commonAmount = model.receivedamount ?? 0;
				if (commonAmount <= 0) return "Amount should be greater than zero";

				using (var npsql = new NpgsqlConnection(db_connectionstring))
				{
					npsql.Open();
					using (var transaction = npsql.BeginTransaction())
					{
						try
						{
							// Prevent race in receipt number generation inside Add_Billing_Payment.
							using (var lockCmd = new NpgsqlCommand("LOCK TABLE billingpayment IN EXCLUSIVE MODE; LOCK TABLE receivable IN EXCLUSIVE MODE; LOCK TABLE paymentrequest IN SHARE ROW EXCLUSIVE MODE;", npsql, transaction))
							{
								lockCmd.ExecuteNonQuery();
							}

							// Fetch pending receivables within locked transaction (mandatory first, then by date)
							var pendingItems = new System.Collections.Generic.List<(Guid id, decimal amount, decimal paid, string rxfor)>();
							var ptype = (model.Type ?? "OPD").ToUpperInvariant();
							using (var fetchCmd = new NpgsqlCommand("SELECT * FROM \"Get_Unified_Collection_Pending_Receivables\"(@ft,@fi,@fp)", npsql, transaction))
							{
								fetchCmd.Parameters.AddWithValue("ft", NpgsqlDbType.Varchar, ptype);
								fetchCmd.Parameters.AddWithValue("fi", NpgsqlDbType.Uuid, (object)model.IPDNo ?? DBNull.Value);
								fetchCmd.Parameters.AddWithValue("fp", NpgsqlDbType.Uuid, model.PatientID);
								using (var rdr = fetchCmd.ExecuteReader())
									while (rdr.Read()) pendingItems.Add((rdr.GetGuid(0), rdr.GetDecimal(1), rdr.GetDecimal(2), rdr.GetString(3)));
							}

							var selectedReceivableIds = new HashSet<Guid>(
								(model.Payments ?? new List<UnifiedPaymentInputModel>())
									.Where(x => x != null && x.PayNow > 0 && x.Receivableid != Guid.Empty)
									.Select(x => x.Receivableid));
							var touchesBookingDeposit = pendingItems.Any(x =>
								string.Equals(x.rxfor, "IPD Booking Deposit", StringComparison.OrdinalIgnoreCase)
								&& (selectedReceivableIds.Count == 0 || selectedReceivableIds.Contains(x.id)));
							if (ptype == "IPD"
								&& model.IPDNo.HasValue
								&& touchesBookingDeposit
								&& IsSuccessfulUnifiedCollection(model)
								&& !IsRazorpayPaymentMode(model.paymentmode))
							{
								using (var activeCmd = new NpgsqlCommand("SELECT \"BillingPayment_Has_Active_Razorpay_Request\"(@pvar_paymentmarker,@pvar_paymenttype,@pvar_activeafter)", npsql, transaction))
								{
									activeCmd.Parameters.AddWithValue("pvar_paymentmarker", NpgsqlDbType.Varchar, $"IPD:{model.IPDNo.Value}:%");
									activeCmd.Parameters.AddWithValue("pvar_paymenttype", NpgsqlDbType.Varchar, "Booking Deposit");
									activeCmd.Parameters.AddWithValue("pvar_activeafter", NpgsqlDbType.Timestamp, DateTime.Now.AddMinutes(-15));
									if (Convert.ToBoolean(activeCmd.ExecuteScalar()))
									{
										throw new Exception("A Razorpay booking-deposit payment is already in progress for this IPD. Please ask the health seeker to close/cancel the payment popup or wait for it to expire before receiving payment at the front desk.");
									}
								}
							}

							var derivedReceivableFor = pendingItems.Count > 0 ? pendingItems[0].rxfor : "Unified Payment Collection";
							var billingPaymentId = Guid.NewGuid();

							var paymentModel = new BillingPaymentModel
							{
								BillingPaymentid = billingPaymentId,
								tenantid = model.tenantid,
								paymentdate = DateTime.Now.Date,
								patientname = model.PatientID,
								ipdnumber = model.IPDNo,
								opdnumber = model.opdnumber,
								receivablefor = derivedReceivableFor,
								currency = string.IsNullOrWhiteSpace(model.currency) ? "INR" : model.currency,
								receivedamount = commonAmount,
								conversionrate = model.conversionrate ?? 1,
								amount = commonAmount,
								paymentmode = model.paymentmode,
								transactionreference = model.transactionreference,
								bankname = model.bankname,
								chequedddate = model.chequedddate,
								paymentstatus = string.IsNullOrWhiteSpace(model.paymentstatus) ? "Success" : model.paymentstatus,
								collectedby = model.collectedby,
								counterid = model.counterid,
								remarks = model.remarks,
								createduser = createdUser
							};

							using (var fnCmd = new NpgsqlCommand("SELECT * FROM \"Add_Billing_Payment\"(@pvar_billingpaymentid,@pvar_tenantid,@pvar_paymentdate,@pvar_patientname,@pvar_patientvisit,@pvar_ipdnumber,@pvar_opdnumber,@pvar_receivablefor,@pvar_therapy,@pvar_therapycost,@pvar_therapykit,@pvar_kitprice,@pvar_medicine,@pvar_price,@pvar_room,@pvar_receivedamount,@pvar_currency,@pvar_conversionrate,@pvar_amount,@pvar_remarks,@pvar_paymentmode,@pvar_transactionreference,@pvar_bankname,@pvar_chequedddate,@pvar_paymentstatus,@pvar_collectedby,@pvar_refundmode,@pvar_refundedamount,@pvar_refundedby,@pvar_refundreferencenumber,@pvar_refundbankname,@pvar_refundreason,@pvar_refundstatus,@pvar_counterid,@pvar_createduser)", npsql, transaction))
							{
								fnCmd.CommandType = CommandType.Text;
								fnCmd.Parameters.AddWithValue("pvar_billingpaymentid", NpgsqlDbType.Uuid, (object)paymentModel.BillingPaymentid ?? DBNull.Value);
								fnCmd.Parameters.AddWithValue("pvar_tenantid", NpgsqlDbType.Uuid, (object)paymentModel.tenantid ?? DBNull.Value);
								fnCmd.Parameters.AddWithValue("pvar_paymentdate", NpgsqlDbType.Date, paymentModel.paymentdate);
								fnCmd.Parameters.AddWithValue("pvar_patientname", NpgsqlDbType.Uuid, (object)paymentModel.patientname ?? DBNull.Value);
								fnCmd.Parameters.AddWithValue("pvar_patientvisit", NpgsqlDbType.Uuid, (object)paymentModel.patientvisit ?? DBNull.Value);
								fnCmd.Parameters.AddWithValue("pvar_ipdnumber", NpgsqlDbType.Uuid, (object)paymentModel.ipdnumber ?? DBNull.Value);
								fnCmd.Parameters.AddWithValue("pvar_opdnumber", NpgsqlDbType.Uuid, (object)paymentModel.opdnumber ?? DBNull.Value);
								fnCmd.Parameters.AddWithValue("pvar_receivablefor", NpgsqlDbType.Varchar, (object)paymentModel.receivablefor ?? DBNull.Value);
								fnCmd.Parameters.AddWithValue("pvar_therapy", NpgsqlDbType.Uuid, (object)paymentModel.therapy ?? DBNull.Value);
								fnCmd.Parameters.AddWithValue("pvar_therapycost", NpgsqlDbType.Numeric, (object)paymentModel.therapycost ?? DBNull.Value);
								fnCmd.Parameters.AddWithValue("pvar_therapykit", NpgsqlDbType.Uuid, (object)paymentModel.therapykit ?? DBNull.Value);
								fnCmd.Parameters.AddWithValue("pvar_kitprice", NpgsqlDbType.Varchar, (object)paymentModel.kitprice ?? DBNull.Value);
								fnCmd.Parameters.AddWithValue("pvar_medicine", NpgsqlDbType.Uuid, (object)paymentModel.medicine ?? DBNull.Value);
								fnCmd.Parameters.AddWithValue("pvar_price", NpgsqlDbType.Numeric, (object)paymentModel.price ?? DBNull.Value);
								fnCmd.Parameters.AddWithValue("pvar_room", NpgsqlDbType.Uuid, (object)paymentModel.room ?? DBNull.Value);
								fnCmd.Parameters.AddWithValue("pvar_receivedamount", NpgsqlDbType.Numeric, (object)paymentModel.receivedamount ?? DBNull.Value);
								fnCmd.Parameters.AddWithValue("pvar_currency", NpgsqlDbType.Varchar, (object)paymentModel.currency ?? DBNull.Value);
								fnCmd.Parameters.AddWithValue("pvar_conversionrate", NpgsqlDbType.Numeric, (object)paymentModel.conversionrate ?? DBNull.Value);
								fnCmd.Parameters.AddWithValue("pvar_amount", NpgsqlDbType.Numeric, paymentModel.amount);
								fnCmd.Parameters.AddWithValue("pvar_remarks", NpgsqlDbType.Varchar, (object)paymentModel.remarks ?? DBNull.Value);
								fnCmd.Parameters.AddWithValue("pvar_paymentmode", NpgsqlDbType.Varchar, (object)paymentModel.paymentmode ?? DBNull.Value);
								fnCmd.Parameters.AddWithValue("pvar_transactionreference", NpgsqlDbType.Varchar, (object)paymentModel.transactionreference ?? DBNull.Value);
								fnCmd.Parameters.AddWithValue("pvar_bankname", NpgsqlDbType.Varchar, (object)paymentModel.bankname ?? DBNull.Value);
								fnCmd.Parameters.AddWithValue("pvar_chequedddate", NpgsqlDbType.Date, (object)paymentModel.chequedddate ?? DBNull.Value);
								fnCmd.Parameters.AddWithValue("pvar_paymentstatus", NpgsqlDbType.Varchar, (object)paymentModel.paymentstatus ?? DBNull.Value);
								fnCmd.Parameters.AddWithValue("pvar_collectedby", NpgsqlDbType.Uuid, (object)paymentModel.collectedby ?? DBNull.Value);
								fnCmd.Parameters.AddWithValue("pvar_refundmode", NpgsqlDbType.Varchar, (object)paymentModel.refundmode ?? DBNull.Value);
								fnCmd.Parameters.AddWithValue("pvar_refundedamount", NpgsqlDbType.Numeric, (object)paymentModel.refundedamount ?? DBNull.Value);
								fnCmd.Parameters.AddWithValue("pvar_refundedby", NpgsqlDbType.Uuid, (object)paymentModel.refundedby ?? DBNull.Value);
								fnCmd.Parameters.AddWithValue("pvar_refundreferencenumber", NpgsqlDbType.Varchar, (object)paymentModel.refundreferencenumber ?? DBNull.Value);
								fnCmd.Parameters.AddWithValue("pvar_refundbankname", NpgsqlDbType.Varchar, (object)paymentModel.refundbankname ?? DBNull.Value);
								fnCmd.Parameters.AddWithValue("pvar_refundreason", NpgsqlDbType.Varchar, (object)paymentModel.refundreason ?? DBNull.Value);
								fnCmd.Parameters.AddWithValue("pvar_refundstatus", NpgsqlDbType.Varchar, (object)paymentModel.refundstatus ?? DBNull.Value);
								fnCmd.Parameters.AddWithValue("pvar_counterid", NpgsqlDbType.Varchar, (object)paymentModel.counterid ?? DBNull.Value);
								fnCmd.Parameters.AddWithValue("pvar_createduser", NpgsqlDbType.Uuid, createdUser);

								var outParm = new NpgsqlParameter("pvar_returnMessage", NpgsqlDbType.Varchar) { Direction = ParameterDirection.Output };
								fnCmd.Parameters.Add(outParm);
								fnCmd.ExecuteNonQuery();
								var message = Convert.ToString(outParm.Value ?? "");
								if (!(message ?? string.Empty).Contains("201.1"))
								{
									throw new Exception(message);
								}
							}

							// Waterfall: distribute payment across pending receivables
							decimal remaining = commonAmount;
							foreach (var (rxId, rxAmount, rxPaid, _) in pendingItems)
							{
								if (remaining <= 0m) break;
								decimal rxBalance = rxAmount - rxPaid;
								if (rxBalance <= 0m) continue;
								decimal applying = Math.Min(remaining, rxBalance);
								decimal newPaid = rxPaid + applying;
								bool fullyPaid = (newPaid + 0.009m) >= rxAmount;
								using (var updCmd = new NpgsqlCommand(
									"SELECT \"Update_Unified_Collection_Receivable\"(@paid,@status,@bpid,@rxid)",
									npsql, transaction))
								{
									updCmd.Parameters.AddWithValue("paid", NpgsqlDbType.Numeric, newPaid);
									updCmd.Parameters.AddWithValue("status", NpgsqlDbType.Varchar, fullyPaid ? "Paid" : "Partially Paid");
									updCmd.Parameters.AddWithValue("bpid", NpgsqlDbType.Uuid, billingPaymentId);
									updCmd.Parameters.AddWithValue("rxid", NpgsqlDbType.Uuid, rxId);
									updCmd.ExecuteNonQuery();
								}
								remaining -= applying;
							}

							transaction.Commit();
							return "201.1";
						}
						catch (Exception exTrans)
						{
							transaction.Rollback();
							return exTrans.Message;
						}
					}
				}
			}
			catch (Exception ex)
			{
				return ex.Message;
			}
		}

		public virtual void UpdateOPDConsultationFeeReceivable(Guid opdFormId, decimal paidAmount, Guid billingPaymentId)
		{
			if (paidAmount <= 0) throw new ArgumentException("Paid amount must be greater than zero.", nameof(paidAmount));

			using var conn = new NpgsqlConnection(db_connectionstring);
			conn.Open();
			using (var dbCommand = new NpgsqlCommand("SELECT \"Update_OPD_Consultation_Fee_Receivable\"(@pvar_opdformid,@pvar_paidamount,@pvar_billingpaymentid)", conn))
			{
				dbCommand.CommandType = CommandType.Text;
				dbCommand.Parameters.AddWithValue("pvar_opdformid", NpgsqlDbType.Uuid, opdFormId);
				dbCommand.Parameters.AddWithValue("pvar_paidamount", NpgsqlDbType.Numeric, paidAmount);
				dbCommand.Parameters.AddWithValue("pvar_billingpaymentid", NpgsqlDbType.Uuid, billingPaymentId);
				dbCommand.ExecuteNonQuery();
			}
		}

		public virtual void ApplyIPDRazorpayReceivablePayment(Guid ipdFormId, decimal paidAmount, Guid billingPaymentId, Guid? modifiedUser)
		{
			if (paidAmount <= 0) return;

			using var conn = new NpgsqlConnection(db_connectionstring);
			conn.Open();
			using var transaction = conn.BeginTransaction();
			try
			{
				using (var lockCmd = new NpgsqlCommand("LOCK TABLE receivable IN EXCLUSIVE MODE;", conn, transaction))
				{
					lockCmd.ExecuteNonQuery();
				}

				var pendingItems = new System.Collections.Generic.List<(Guid id, decimal amount, decimal paid)>();
				using (var fetchCmd = new NpgsqlCommand("SELECT * FROM \"Get_IPD_Razorpay_Pending_Receivables\"(@ipdid)", conn, transaction))
				{
					fetchCmd.Parameters.AddWithValue("ipdid", NpgsqlDbType.Uuid, ipdFormId);
					using var rdr = fetchCmd.ExecuteReader();
					while (rdr.Read())
						pendingItems.Add((rdr.GetGuid(0), rdr.GetDecimal(1), rdr.GetDecimal(2)));
				}

				decimal remaining = paidAmount;
				foreach (var (receivableId, receivableAmount, alreadyPaid) in pendingItems)
				{
					if (remaining <= 0) break;

					decimal balance = receivableAmount - alreadyPaid;
					if (balance <= 0) continue;

					decimal applying = Math.Min(remaining, balance);
					decimal newPaid = alreadyPaid + applying;
					bool fullyPaid = (newPaid + 0.009m) >= receivableAmount;

					using var updateCmd = new NpgsqlCommand("SELECT \"Update_IPD_Razorpay_Receivable_Payment\"(@receivableid,@paid,@status,@billingpaymentid,@modifieduser)", conn, transaction);
					updateCmd.Parameters.AddWithValue("paid", NpgsqlDbType.Numeric, newPaid);
					updateCmd.Parameters.AddWithValue("status", NpgsqlDbType.Varchar, fullyPaid ? "Paid" : "Partially Paid");
					updateCmd.Parameters.AddWithValue("billingpaymentid", NpgsqlDbType.Uuid, billingPaymentId);
					updateCmd.Parameters.AddWithValue("modifieduser", NpgsqlDbType.Uuid, (object)modifiedUser ?? DBNull.Value);
					updateCmd.Parameters.AddWithValue("receivableid", NpgsqlDbType.Uuid, receivableId);
					updateCmd.ExecuteNonQuery();

					remaining -= applying;
				}

				transaction.Commit();
			}
			catch
			{
				transaction.Rollback();
				throw;
			}
		}

		public virtual void ApplyIPDBalanceRazorpayReceivablePayment(Guid ipdFormId, decimal paidAmount, Guid billingPaymentId, Guid? modifiedUser)
		{
			if (paidAmount <= 0) return;

			using var conn = new NpgsqlConnection(db_connectionstring);
			conn.Open();
			using var cmd = new NpgsqlCommand("SELECT \"Apply_IPD_Balance_Razorpay_Receivable_Payment\"(@ipdid,@paid,@billingpaymentid,@modifieduser)", conn);
			cmd.Parameters.AddWithValue("modifieduser", NpgsqlDbType.Uuid, (object)modifiedUser ?? DBNull.Value);
			cmd.Parameters.AddWithValue("billingpaymentid", NpgsqlDbType.Uuid, billingPaymentId);
			cmd.Parameters.AddWithValue("paid", NpgsqlDbType.Numeric, paidAmount);
			cmd.Parameters.AddWithValue("ipdid", NpgsqlDbType.Uuid, ipdFormId);
			cmd.ExecuteNonQuery();
		}

		public virtual void ApplySelectedIPDBalanceRazorpayReceivablePayment(List<UnifiedPaymentInputModel> payments, Guid billingPaymentId, Guid? modifiedUser)
		{
			if (payments == null || payments.Count == 0) return;

			using var conn = new NpgsqlConnection(db_connectionstring);
			conn.Open();
			using var transaction = conn.BeginTransaction();
			try
			{
				using (var lockCmd = new NpgsqlCommand("LOCK TABLE receivable IN EXCLUSIVE MODE;", conn, transaction))
				{
					lockCmd.ExecuteNonQuery();
				}

				foreach (var item in payments.Where(x => x != null && x.Receivableid != Guid.Empty && x.PayNow > 0))
				{
					using var cmd = new NpgsqlCommand("SELECT \"Apply_Selected_IPD_Balance_Razorpay_Receivable_Payment\"(@receivableid,@paid,@billingpaymentid,@modifieduser)", conn, transaction);
					cmd.Parameters.AddWithValue("modifieduser", NpgsqlDbType.Uuid, (object)modifiedUser ?? DBNull.Value);
					cmd.Parameters.AddWithValue("billingpaymentid", NpgsqlDbType.Uuid, billingPaymentId);
					cmd.Parameters.AddWithValue("paid", NpgsqlDbType.Numeric, item.PayNow);
					cmd.Parameters.AddWithValue("receivableid", NpgsqlDbType.Uuid, item.Receivableid);
					cmd.ExecuteNonQuery();
				}

				transaction.Commit();
			}
			catch
			{
				transaction.Rollback();
				throw;
			}
		}

		public virtual void MarkIPDRazorpayReceivablesFailed(Guid ipdFormId, Guid? modifiedUser)
		{
			using var conn = new NpgsqlConnection(db_connectionstring);
			conn.Open();
			using var cmd = new NpgsqlCommand("SELECT \"Mark_IPD_Razorpay_Receivables_Failed\"(@ipdid,@modifieduser)", conn);
			cmd.Parameters.AddWithValue("modifieduser", NpgsqlDbType.Uuid, (object)modifiedUser ?? DBNull.Value);
			cmd.Parameters.AddWithValue("ipdid", NpgsqlDbType.Uuid, ipdFormId);
			cmd.ExecuteNonQuery();
		}


	}
}
