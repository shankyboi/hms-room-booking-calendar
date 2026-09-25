namespace Admin.Controllers
{
    using System;
    using System.Collections.Generic;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Hosting;
    using NalamVazha.Models;

    /// <summary>
    /// Development-only entry points for capturing real application views with
    /// fictional data. These routes intentionally do not exist outside Development.
    /// </summary>
    public class ScreenshotDemoController : Controller
    {
        private readonly IWebHostEnvironment _environment;

        public ScreenshotDemoController(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        private IActionResult DemoView(string viewPath, object model)
        {
            if (!_environment.IsDevelopment())
                return NotFound();

            HttpContext.Session.SetString("NalamVazhachoosedtenantid", "88e95f5c-1b1d-4f63-8914-6c512e4821c8");
            HttpContext.Session.SetString("NalamVazhaloginUserID", "32b0a34e-fd02-4881-a724-d9048617b3fd");
            HttpContext.Session.SetString("NalamVazhausersid", "32b0a34e-fd02-4881-a724-d9048617b3fd");
            HttpContext.Session.SetString("NalamVazhauserrole", "Frontdesk Admin");
            HttpContext.Session.SetString("firstname", "Aarav");
            HttpContext.Session.SetString("NalamVazharole_JSON",
                "[" +
                "{\"controllername\":\"Finance\",\"viewname\":\"list\",\"actionmethodname\":\"get_Finance_List\"}," +
                "{\"controllername\":\"Finance\",\"viewname\":\"detail\",\"actionmethodname\":\"Detail_Finance\"}," +
                "{\"controllername\":\"Receivable\",\"viewname\":\"list\",\"actionmethodname\":\"get_Receivables\"}," +
                "{\"controllername\":\"Receivable\",\"viewname\":\"create\",\"actionmethodname\":\"Add_Receivable\"}," +
                "{\"controllername\":\"Receivable\",\"viewname\":\"detail\",\"actionmethodname\":\"View_Receivable\"}," +
                "{\"controllername\":\"BillingPayment\",\"viewname\":\"list\",\"actionmethodname\":\"get_Billing_Payment_List\"}," +
                "{\"controllername\":\"Assessment\",\"viewname\":\"create\",\"actionmethodname\":\"Add_Assessment\"}," +
                "{\"controllername\":\"IPDApplicationForm\",\"viewname\":\"create\",\"actionmethodname\":\"Add_IPD_Application_Form\"}" +
                "]");

            return View(viewPath, model);
        }

        public IActionResult IPDApplication()
        {
            return DemoView("~/Views/IPDApplicationForm/Add_IPD_Application_Form.cshtml", new IPDApplicationFormModel());
        }

        public IActionResult Assessment()
        {
            return DemoView("~/Views/Assessment/Add_Assessment.cshtml", new AssessmentModel
            {
                patientname = Guid.Parse("1b9cf15a-5670-4cd1-981e-93e83890d73a"),
                doctorname = Guid.Parse("4d14f231-7bb8-4c1d-8e23-b0cb5a113d07"),
                assessmentdate = DateTime.Today,
                taskname = "IP Screening"
            });
        }

        public IActionResult PatientDashboard()
        {
            return DemoView("~/Views/PatientDashboard/Index.cshtml", null);
        }

        public IActionResult FrontDesk()
        {
            return DemoView("~/Views/FrontDesk/Dashboard.cshtml", null);
        }

        public IActionResult FinanceReports()
        {
            return DemoView("~/Views/Finance/Finance_List.cshtml", new FinanceModel());
        }

        public IActionResult Receivables()
        {
            return DemoView("~/Views/Receivable/Receivables.cshtml", new ReceivableModel());
        }

        public IActionResult BillGeneration()
        {
            var patientId = Guid.Parse("1b9cf15a-5670-4cd1-981e-93e83890d73a");
            var ipdId = Guid.Parse("9ccf0324-eb08-4c68-8e0d-2855335820e9");
            var bills = new List<UnifiedReceivableItemModel>
            {
                new UnifiedReceivableItemModel { Receivableid = Guid.Parse("0a5d1111-9f21-4f59-a7b4-123456789001"), receivabledate = "03/09/2026", receivablefor = "Package", billdetails = "Naturopathy Rejuvenation Programme – 7 days", category = "Treatment", amount = 42000m, paidamount = 15000m, balance = 27000m, ismandatory = true },
                new UnifiedReceivableItemModel { Receivableid = Guid.Parse("0a5d1111-9f21-4f59-a7b4-123456789002"), receivabledate = "03/09/2026", receivablefor = "Room", billdetails = "Panchakarma Suite P1 (03/09/2026)", category = "Accommodation", amount = 6500m, paidamount = 0m, balance = 6500m, ismandatory = true },
                new UnifiedReceivableItemModel { Receivableid = Guid.Parse("0a5d1111-9f21-4f59-a7b4-123456789003"), receivabledate = "03/09/2026", receivablefor = "Consultation", billdetails = "Initial physician consultation", category = "Clinical", amount = 1200m, paidamount = 1200m, balance = 0m, ismandatory = true },
                new UnifiedReceivableItemModel { Receivableid = Guid.Parse("0a5d1111-9f21-4f59-a7b4-123456789004"), receivabledate = "03/09/2026", receivablefor = "Therapy Kit", billdetails = "Wellness therapy kit", category = "Other Charges", amount = 1850m, paidamount = 0m, balance = 1850m, ismandatory = false }
            };
            var model = new UnifiedPaymentCollectionViewModel
            {
                PatientID = patientId,
                IPDNo = ipdId,
                Type = "IPD",
                Bills = bills,
                RoomCostSummary = new List<UnifiedRoomCostSummaryModel>
                {
                    new UnifiedRoomCostSummaryModel { ReceivableDate = DateTime.Today, AllottedTo = "Aarav Menon", RoomNo = "P1", CostPerDay = 6500m, LineItemTotal = 6500m, PaidAmount = 0m, BalanceAmount = 6500m, Status = "Due" }
                }
            };
            foreach (var bill in bills)
            {
                model.Payments.Add(new UnifiedPaymentInputModel
                {
                    Receivableid = bill.Receivableid,
                    ReceivableFor = bill.receivablefor,
                    Balance = bill.balance,
                    IsMandatory = bill.ismandatory,
                    PayNow = 0m
                });
            }
            ViewBag.BookingStatus = "Admitted";
            return DemoView("~/Views/BillingPayment/UnifiedPayment.cshtml", model);
        }

        public IActionResult PaymentGateway()
        {
            ViewBag.BillingSummary = new IPDBillingSummaryModel
            {
                tenantname = "Nalam Nature Cure Hospital",
                bookingreferencenumber = "IPD-20984",
                patientfullname = "Aarav Menon",
                bookingstatus = "Provisional Confirmed",
                totalamount = 42000m,
                advanceamount = 15000m,
                remainingbalance = 27000m,
                currencycode = "INR"
            };
            return DemoView("~/Views/IPDApplicationForm/Initiate_Payment.cshtml", new IPDPaymentDetailsModel
            {
                IPDApplicationFormid = "9ccf0324-eb08-4c68-8e0d-2855335820e9",
                bookingreferencenumber = "IPD-20984",
                firstname = "Aarav",
                lastname = "Menon",
                mobilenumber = "+91 98765 43210",
                packagename_master = "Naturopathy Rejuvenation Programme",
                bookingdepositamount = 15000m,
                advanceamount = 15000m,
                totalamount = 42000m,
                daysofstay = 7,
                accommodationtype = "Panchakarma Suite",
                bookingstatus = "Provisional Confirmed"
            });
        }
    }
}
