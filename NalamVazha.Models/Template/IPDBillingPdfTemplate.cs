using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace NalamVazha.Models.Template
{
    public class IPDBillingPdfTemplate
    {
        private class BillingRoomStatementRow
        {
            public string AllottedTo { get; set; } = "-";
            public string RoomNo { get; set; } = "-";
            public string ChargeSource { get; set; } = "";
            public DateTime? FromDate { get; set; }
            public DateTime? ToDate { get; set; }
            public int Days { get; set; } = 1;
            public decimal CostPerDay { get; set; }
            public decimal LineTotal { get; set; }
        }

        private static readonly string[] BillingDateFormats = new[]
        {
            "dd/MM/yyyy", "d/M/yyyy", "dd-MM-yyyy", "d-M-yyyy",
            "yyyy-MM-dd", "yyyy-MM-dd HH:mm:ss", "MM/dd/yyyy HH:mm:ss",
            "dd/MM/yyyy HH:mm:ss", "d/M/yyyy HH:mm:ss",
            "dd-MM-yyyy HH:mm:ss", "d-M-yyyy HH:mm:ss"
        };

        private static bool TryParseBillingDate(string value, out DateTime date)
        {
            value = (value ?? "").Trim();
            if (DateTime.TryParseExact(value, BillingDateFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
            {
                date = date.Date;
                return true;
            }

            if (DateTime.TryParse(value, CultureInfo.GetCultureInfo("en-IN"), DateTimeStyles.None, out date))
            {
                date = date.Date;
                return true;
            }

            return false;
        }

        private static string GetDynamicString(object item, string name)
        {
            if (item is JObject jobj && jobj.TryGetValue(name, StringComparison.OrdinalIgnoreCase, out var token))
                return token?.ToString() ?? "";

            if (item is IDictionary<string, object> dict && dict.TryGetValue(name, out var value))
                return value?.ToString() ?? "";

            var prop = item?.GetType().GetProperty(name);
            return prop?.GetValue(item)?.ToString() ?? "";
        }

        private static decimal GetDynamicDecimal(object item, string name)
        {
            var raw = GetDynamicString(item, name);
            return decimal.TryParse(raw, NumberStyles.Any, CultureInfo.InvariantCulture, out var value)
                ? value
                : 0m;
        }

        private static string FormatBillingDate(DateTime? date)
        {
            return date.HasValue ? date.Value.ToString("dd/MM/yyyy") : "-";
        }

        private static string FormatBillingDateTime(string value)
        {
            value = (value ?? "").Trim();
            if (string.IsNullOrWhiteSpace(value))
                return "-";

            if (DateTime.TryParse(value, CultureInfo.GetCultureInfo("en-IN"), DateTimeStyles.None, out var date))
                return date.ToString("dd/MM/yyyy HH:mm");

            return value;
        }

        private static decimal GetRoomCost(JToken room, string role)
        {
            var patientCpd = (decimal?)room["costperday"] ?? 0m;
            var attendantCpd = (decimal?)room["attendantcostperday"] ?? 0m;
            return role.Equals("Attendant", StringComparison.OrdinalIgnoreCase) ? attendantCpd : patientCpd;
        }

        private static string GetRoomChargeSource(string remarks)
        {
            remarks = (remarks ?? "").Trim();
            var separatorIndex = remarks.IndexOf(':');
            return separatorIndex > 0 ? remarks.Substring(0, separatorIndex) : remarks;
        }

        private static string GetReceivableRoomNumber(object receivable)
        {
            var roomNumber = GetDynamicString(receivable, "roomnumber").Trim();
            if (!string.IsNullOrWhiteSpace(roomNumber) && roomNumber != "-" && !Guid.TryParse(roomNumber, out _))
                return roomNumber;

            roomNumber = GetDynamicString(receivable, "room_master").Trim();
            if (!string.IsNullOrWhiteSpace(roomNumber) && roomNumber != "-" && !Guid.TryParse(roomNumber, out _))
                return roomNumber;

            roomNumber = GetDynamicString(receivable, "room").Trim();
            return Guid.TryParse(roomNumber, out _) ? "" : roomNumber;
        }

        private static string ResolveRoomNumber(JArray roomDetails, string role, DateTime date, decimal amount)
        {
            string fallback = "-";

            foreach (var room in roomDetails)
            {
                var roomRole = (room["allottedto"]?.ToString() ?? "").Trim();
                if (!roomRole.Equals(role, StringComparison.OrdinalIgnoreCase)) continue;

                var roomNo = (room["roomnumber"]?.ToString() ?? "").Trim();
                if (!string.IsNullOrWhiteSpace(roomNo) && roomNo != "-")
                    fallback = roomNo;

                if (!TryParseBillingDate(room["fromdate"]?.ToString(), out var fromDate) ||
                    !TryParseBillingDate(room["todate"]?.ToString(), out var toDate)) continue;

                if (date < fromDate || date > toDate) continue;

                if (GetRoomCost(room, role) == amount)
                    return string.IsNullOrWhiteSpace(roomNo) ? "-" : roomNo;
            }

            if (fallback == "-")
            {
                foreach (var room in roomDetails)
                {
                    var roomNo = (room["roomnumber"]?.ToString() ?? "").Trim();
                    if (!string.IsNullOrWhiteSpace(roomNo) && roomNo != "-")
                        return roomNo;
                }
            }

            return fallback;
        }

        public static string BuildFooterHtml()
        {
            return @"
<!DOCTYPE html>
<html>
<head>
<meta charset='utf-8' />
<style>
html, body { margin:0; padding:0; }
body { font-family: Arial, Helvetica, sans-serif; }

.footer-band {
    margin: 0 26px;
    height:30px;
    line-height:30px;
    background:#1a4a36;
    color:#ffffff;
    text-align:center;
    -webkit-print-color-adjust: exact;
    print-color-adjust: exact;
}
.page-no {
    font-size:10px;
    font-weight:bold;
}
</style>
</head>
<body onload='subst()'>
  <div class='footer-band'>
    <span class='page-no'>Page No: <span class='page'></span> of <span class='topage'></span></span>
  </div>
<script>
function subst() {
    var vars = {};
    var x = document.location.search.substring(1).split('&');
    for (var i in x) {
        var z = x[i].split('=', 2);
        vars[z[0]] = decodeURIComponent(z[1]);
    }
    var classes = ['page', 'frompage', 'topage', 'webpage', 'section', 'subsection'];
    for (var i in classes) {
        var els = document.getElementsByClassName(classes[i]);
        for (var j = 0; j < els.length; ++j) {
            els[j].textContent = vars[classes[i]];
        }
    }
}
</script>
</body>
</html>";
        }

        private static int GetBillableOccupantCount(string allottedTo)
        {
            return 1;
        }

        private static List<BillingRoomStatementRow> BuildRoomRowsFromBackend(JArray roomDetails, int fallbackDays)
        {
            var rows = new List<BillingRoomStatementRow>();

            foreach (var room in roomDetails)
            {
                var role = (room["allottedto"]?.ToString() ?? "-").Trim();

                DateTime fromDate = DateTime.MinValue;
                DateTime toDate = DateTime.MinValue;

                bool hasFromDate = TryParseBillingDate(room["fromdate"]?.ToString(), out fromDate);
                bool hasToDate = TryParseBillingDate(room["todate"]?.ToString(), out toDate);
                bool hasDates = hasFromDate && hasToDate;

                int rowDays = hasDates
                    ? Math.Max(1, (toDate.Date - fromDate.Date).Days + 1)
                    : fallbackDays;

                decimal costPerDay = GetRoomCost(room, role);

                string roomNo = (room["roomnumber"]?.ToString() ?? "").Trim();

                if (string.IsNullOrWhiteSpace(roomNo) || roomNo == "-")
                    roomNo = (room["roomno"]?.ToString() ?? "-").Trim();

                rows.Add(new BillingRoomStatementRow
                {
                    AllottedTo = string.IsNullOrWhiteSpace(role) ? "-" : role,
                    RoomNo = string.IsNullOrWhiteSpace(roomNo) ? "-" : roomNo,
                    FromDate = hasDates ? fromDate : (DateTime?)null,
                    ToDate = hasDates ? toDate : (DateTime?)null,
                    Days = rowDays,
                    CostPerDay = costPerDay,
                    LineTotal = costPerDay * rowDays
                });
            }

            return rows
                .OrderBy(x => x.FromDate)
                .ThenBy(x => x.AllottedTo)
                .ToList();
        }
        private static List<BillingRoomStatementRow> BuildRoomStatementRows(List<dynamic> receivables, JArray roomDetails, int fallbackDays)
        {
            var dailyRows = new List<BillingRoomStatementRow>();

            if (receivables != null)
            {
                foreach (var receivable in receivables)
                {
                    var receivableFor = GetDynamicString(receivable, "receivablefor");
                    if (!receivableFor.Equals("Room", StringComparison.OrdinalIgnoreCase)) continue;

                    DateTime chargeDate;
                    if (!TryParseBillingDate(GetDynamicString(receivable, "receivabledate"), out chargeDate)) continue;

                    var amount = GetDynamicDecimal(receivable, "amount");
                    if (amount <= 0) continue;

                    var remarks = GetDynamicString(receivable, "remarks");
                    var role = remarks.IndexOf("Attendant", StringComparison.OrdinalIgnoreCase) >= 0 ? "Attendant" : "Patient";
                    var receivableRoomNo = GetReceivableRoomNumber(receivable);

                    dailyRows.Add(new BillingRoomStatementRow
                    {
                        AllottedTo = role,
                        RoomNo = string.IsNullOrWhiteSpace(receivableRoomNo)
                            ? ResolveRoomNumber(roomDetails, role, chargeDate, amount)
                            : receivableRoomNo,
                        ChargeSource = GetRoomChargeSource(remarks),
                        FromDate = chargeDate,
                        ToDate = chargeDate,
                        Days = 1,
                        CostPerDay = amount,
                        LineTotal = amount
                    });
                }
            }

            if (dailyRows.Count > 0)
            {
                var grouped = new List<BillingRoomStatementRow>();
                foreach (var row in dailyRows.OrderBy(x => x.AllottedTo).ThenBy(x => x.RoomNo).ThenBy(x => x.CostPerDay).ThenBy(x => x.ChargeSource).ThenBy(x => x.FromDate))
                {
                    var previous = grouped.LastOrDefault();
                    if (previous != null &&
                        previous.AllottedTo.Equals(row.AllottedTo, StringComparison.OrdinalIgnoreCase) &&
                        previous.RoomNo.Equals(row.RoomNo, StringComparison.OrdinalIgnoreCase) &&
                        previous.ChargeSource.Equals(row.ChargeSource, StringComparison.OrdinalIgnoreCase) &&
                        previous.CostPerDay == row.CostPerDay &&
                        previous.ToDate.HasValue &&
                        row.FromDate.HasValue &&
                        previous.ToDate.Value.AddDays(1) == row.FromDate.Value)
                    {
                        previous.ToDate = row.ToDate;
                        previous.Days += 1;
                        previous.LineTotal += row.LineTotal;
                    }
                    else
                    {
                        grouped.Add(row);
                    }
                }

                return grouped.OrderBy(x => x.FromDate).ThenBy(x => x.AllottedTo).ToList();
            }

            var fallbackRows = new List<BillingRoomStatementRow>();
            foreach (var room in roomDetails)
            {
                var role = (room["allottedto"]?.ToString() ?? "").Trim();
                var cost = GetRoomCost(room, role);
                DateTime fromDate = DateTime.MinValue;
                DateTime toDate = DateTime.MinValue;
                var hasDates = TryParseBillingDate(room["fromdate"]?.ToString(), out fromDate) &&
                               TryParseBillingDate(room["todate"]?.ToString(), out toDate);
                var rowDays = hasDates ? Math.Max(1, (toDate - fromDate).Days + 1) : fallbackDays;

                fallbackRows.Add(new BillingRoomStatementRow
                {
                    AllottedTo = string.IsNullOrWhiteSpace(role) ? "-" : role,
                    RoomNo = (room["roomnumber"]?.ToString() ?? "-").Trim(),
                    FromDate = hasDates ? fromDate : (DateTime?)null,
                    ToDate = hasDates ? toDate : (DateTime?)null,
                    Days = rowDays,
                    CostPerDay = cost,
                    LineTotal = cost * rowDays
                });
            }

            return fallbackRows;
        }
        private static string GetBillingItemDescription(object receivable)
        {
            var receivableFor = GetDynamicString(receivable, "receivablefor");
            var remarks = GetDynamicString(receivable, "remarks");

            if (!receivableFor.Equals("Room", StringComparison.OrdinalIgnoreCase))
            {
                return string.IsNullOrWhiteSpace(receivableFor) ? "-" : receivableFor;
            }

            if (remarks.IndexOf("AttendantWaivedForSharedRoom", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return "Room (Attendant - Waived)";
            }

            bool hasPatient = remarks.IndexOf("Patient", StringComparison.OrdinalIgnoreCase) >= 0;
            bool hasAttendant = remarks.IndexOf("Attendant", StringComparison.OrdinalIgnoreCase) >= 0;

            if (hasPatient && hasAttendant)
            {
                return "Room (Patient + Attendant)";
            }

            if (hasAttendant)
            {
                return "Room (Attendant)";
            }

            if (hasPatient)
            {
                return "Room (Patient)";
            }

            return "Room";
        }

        private static bool IsRefundPayment(object payment)
        {
            var receivableFor = GetDynamicString(payment, "receivablefor");
            var paymentStatus = GetDynamicString(payment, "paymentstatus");
            var refundStatus = GetDynamicString(payment, "refundstatus");

            return receivableFor.IndexOf("refund", StringComparison.OrdinalIgnoreCase) >= 0
                || paymentStatus.IndexOf("refund", StringComparison.OrdinalIgnoreCase) >= 0
                || !string.IsNullOrWhiteSpace(refundStatus)
                || GetDynamicDecimal(payment, "refundedamount") > 0m;
        }

        private static string GetPaymentDateDisplay(object payment)
        {
            var rawDate = GetDynamicString(payment, "paymentdate");
            if (string.IsNullOrWhiteSpace(rawDate))
                return "-";

            return DateTime.TryParse(rawDate, CultureInfo.GetCultureInfo("en-IN"), DateTimeStyles.None, out var parsedDate)
                ? parsedDate.ToString("dd/MM/yyyy")
                : rawDate;
        }

        private static decimal GetRefundAmount(object payment)
        {
            var refundedAmount = GetDynamicDecimal(payment, "refundedamount");
            if (refundedAmount != 0m)
                return Math.Abs(refundedAmount);

            var amount = GetDynamicDecimal(payment, "amount");
            if (amount != 0m)
                return Math.Abs(amount);

            return Math.Abs(GetDynamicDecimal(payment, "receivedamount"));
        }

        public static string BuildHtml(
            NalamVazha.Models.IPDBillingSummaryModel summary,
            List<dynamic> receivables,
            List<dynamic> payments,
            string generatedOn,
            string logoUrl)
        {
            string bookingRef = summary?.bookingreferencenumber ?? "-";
            string tenantName = Convert.ToString(summary?.tenantname ?? "Hospital");
            bool isEstimatedBill = summary != null
                && string.Equals(summary.bookingstatus, "Admitted", StringComparison.OrdinalIgnoreCase);

            decimal grandTotal = 0m;
            decimal billingItemsTotal = 0m;
            decimal totalReceivablePaid = 0m;

            if (receivables != null)
            {
                foreach (var r in receivables)
                {
                    try
                    {
                        var receivableAmount = Convert.ToDecimal(r.amount);
                        grandTotal += receivableAmount;
                        var itemDescription = GetBillingItemDescription(r);
                        if (!itemDescription.Equals("Discount", StringComparison.OrdinalIgnoreCase)
                            && !itemDescription.Equals("Concession", StringComparison.OrdinalIgnoreCase))
                        {
                            billingItemsTotal += receivableAmount;
                        }
                    }
                    catch { }
                    try { totalReceivablePaid += Convert.ToDecimal(r.paidamount); } catch { }
                }
            }
            decimal totalPaymentsMade = 0m;
            var receivedPayments = payments == null
                ? new List<dynamic>()
                : payments.Where(p => !IsRefundPayment(p)).ToList();
            var refundPayments = payments == null
                ? new List<dynamic>()
                : payments.Where(IsRefundPayment).ToList();

            if (payments != null)
            {
                foreach (var p in payments)
                {
                    try
                    {
                        totalPaymentsMade += Convert.ToDecimal(p.receivedamount);
                    }
                    catch
                    {
                        try
                        {
                            totalPaymentsMade += Convert.ToDecimal(p.amount);
                        }
                        catch { }
                    }
                }
            }
            var roomDetails = new JArray();

            if (summary != null && !string.IsNullOrWhiteSpace(summary.blocked_room_details_json))
            {
                try
                {
                    roomDetails = JArray.Parse(summary.blocked_room_details_json);
                }
                catch
                {
                    roomDetails = new JArray();
                }
            }

            var fallbackDays = (summary?.daysofstay ?? 1) > 0 ? (summary?.daysofstay ?? 1) : 1;
            var roomStatementRows = BuildRoomRowsFromBackend(roomDetails, fallbackDays);
            var roomGrandTotal = roomStatementRows.Sum(x => x.LineTotal);
            var hasReceivables = receivables != null && receivables.Count > 0;
            if (!hasReceivables)
            {
                grandTotal = roomGrandTotal;
            }
            var displayedItemsTotal = hasReceivables ? billingItemsTotal : roomGrandTotal;

            decimal totalReceived = totalPaymentsMade > 0 ? totalPaymentsMade : totalReceivablePaid;

            decimal totalExtraPaid = 0m;
            decimal totalBalance = 0m;

            if (totalReceived > grandTotal)
            {
                totalExtraPaid = totalReceived - grandTotal;
                totalBalance = 0m;
            }
            else
            {
                totalExtraPaid = 0m;
                totalBalance = grandTotal - totalReceived;
            }

            var sb = new StringBuilder();

            sb.Append(@"
<!DOCTYPE html>
<html>
<head>
<meta charset='utf-8' />
<style>

body{
    font-family:Arial,Helvetica,sans-serif;
    font-size:10px;
    color:#111827;
    margin:0;
    padding:0;
}

.page{
    padding:22px 26px;
}

.header-table{
    width:100%;
    border-collapse:collapse;
    margin-bottom:18px;
    background:#1a4a36;
}

.header-table td{
    padding:12px 18px;
    color:#ffffff;
}

.header-title{
    font-size:15px;
    font-weight:bold;
}

.bill-estimated-label {
    display: inline-block;
    margin-left: 8px;
    padding: 3px 9px;
    border-radius: 999px;
    background: #fef3c7;
    color: #92400e;
    font-size: 10px;
    font-weight: 700;
    vertical-align: middle;
}

.header-subtitle{
    font-size:9px;
    margin-top:2px;
}

.header-ref{
    text-align:right;
    font-size:12px;
    font-weight:bold;
}

.tenant-table{
    width:100%;
    border-collapse:collapse;
    margin-bottom:16px;
}

.logo-cell{
    width:80px;
}

.logo-img{
    width:70px;
    height:70px;
}

.tenant-name{
    font-size:18px;
    font-weight:bold;
    text-transform:uppercase;
}

.info-main{
    width:100%;
    border-collapse:collapse;
    border:1px solid #e0e0e0;
    margin-bottom:14px;
}

.info-box{
    width:50%;
    padding:10px 14px;
    vertical-align:top;
    border-right:1px solid #e0e0e0;
}

.info-box-last{
    width:50%;
    padding:10px 14px;
    vertical-align:top;
}

.section-title{
    font-size:10.5px;
    font-weight:bold;
    color:#2d5f44;
    padding-bottom:8px;
}

.inner-info{
    width:100%;
    border-collapse:collapse;
}

.inner-info td{
    border:none;
    padding:3px 0;
}

.label{
    width:125px;
    color:#6b7280;
}

.value{
    font-weight:bold;
}

.section-heading{
    font-size:10.5px;
    font-weight:bold;
    color:#2d5f44;
    margin-top:14px;
    margin-bottom:8px;
}

.payment-title{
    font-size:10.5px;
    font-weight:bold;
    margin-top:14px;
    margin-bottom:6px;
}

.data-table{
    width:100%;
    border-collapse:collapse;
    margin-bottom:12px;
}

.data-table th{
    background:#f0f7f4;
    color:#1a4a36;
    padding:7px 8px;
    text-align:left;
    border:1px solid #d1e7dd;
    font-size:9px;
}

.data-table td{
    padding:6px 8px;
    border:1px solid #e0e0e0;
    font-size:9.5px;
}

.data-table tfoot td{
    background:#f0f7f4;
    font-weight:bold;
}

.text-right{
    text-align:right;
}

.total-label{
    text-align:right;
    font-weight:bold;
}

.green-text{
    color:#166534;
    font-weight:bold;
}

.red-text{
    color:#b91c1c;
    font-weight:bold;
}

.empty-cell{
    text-align:center;
    color:#777;
    font-style:italic;
    padding:14px;
}
.bill-ipd-badge{
    display:inline-block;
    background:#2d5f44;
    border:1px solid #d9e6de;
    border-radius:4px;
    padding:5px 12px;
    color:#ffffff;
    font-size:12px;
    font-weight:700;
    white-space:nowrap;

    -webkit-print-color-adjust:exact;
    print-color-adjust:exact;
}
thead {
    display: table-header-group;
}

tfoot {
    display: table-row-group;
}

tr {
    page-break-inside: avoid;
}

table {
    page-break-inside: auto;
}

.section-heading {
    page-break-after: avoid;
}

.data-table {
    page-break-before: auto;
}
</style>
</head>

<body>
<div class='page'>
");

            sb.Append($@"

<table class='header-table'>
<tr>
<td>
    <div class='header-title'>IPD Billing Statement{(isEstimatedBill ? "<span class='bill-estimated-label'>Estimated Billing Summary</span>" : "")}</div>
    <div class='header-subtitle'>Patient Admission & Billing Summary</div>
</td>
<td class='header-ref'>
    <span class='bill-ipd-badge'>
        {bookingRef}
    </span>
</td>
</tr>
</table>

");

            sb.Append("<table class='tenant-table'><tr>");

            if (!string.IsNullOrWhiteSpace(logoUrl))
            {
                try
                {
                    if (System.IO.File.Exists(logoUrl))
                    {
                        byte[] logoBytes = System.IO.File.ReadAllBytes(logoUrl);

                        string extension = System.IO.Path
                            .GetExtension(logoUrl)
                            .TrimStart('.')
                            .ToLowerInvariant();

                        string mimeType;

                        switch (extension)
                        {
                            case "jpg":
                            case "jpeg":
                                mimeType = "image/jpeg";
                                break;

                            case "svg":
                                mimeType = "image/svg+xml";
                                break;

                            case "webp":
                                mimeType = "image/webp";
                                break;

                            default:
                                mimeType = "image/png";
                                break;
                        }

                        string base64Logo = Convert.ToBase64String(logoBytes);

                        string logoSource =
                            "data:" + mimeType + ";base64," + base64Logo;

                        sb.Append($@"
<td class='logo-cell'>
    <img src='{logoSource}' class='logo-img' alt='Organization Logo' />
</td>");
                    }
                }
                catch
                {
                    // PDF will continue without the logo.
                }
            }
            sb.Append($"<td><div class='tenant-name'>{tenantName}</div></td>");

            sb.Append("</tr></table>");

            string daysOfStay = "-";

            if (summary != null && summary.daysofstay.HasValue && summary.daysofstay.Value > 0)
            {
                daysOfStay = summary.daysofstay.Value + " day(s)";
            }

            sb.Append($@"

<table class='info-main'>
<tr>

<td class='info-box'>
    <div class='section-title'>Patient Details</div>

    <table class='inner-info'>
        <tr>
            <td class='label'>Patient Name</td>
            <td class='value'>{summary?.patientfullname ?? "-"}</td>
        </tr>

        <tr>
            <td class='label'>Mobile</td>
            <td class='value'>{summary?.mobilenumber ?? "-"}</td>
        </tr>

        <tr>
            <td class='label'>Days of Stay</td>
            <td class='value'>{daysOfStay}</td>
        </tr>
    </table>
</td>

<td class='info-box-last'>
    <div class='section-title'>Admission Details</div>

    <table class='inner-info'>
        <tr>
            <td class='label'>Booking Status</td>
            <td class='value'>{summary?.bookingstatus ?? "-"}</td>
        </tr>

        <tr>
            <td class='label'>Admitted Date</td>
            <td class='value'>{FormatBillingDateTime(summary?.estimatedarrival)}</td>
        </tr>
 {(!string.IsNullOrWhiteSpace(summary?.groupcode) ? $@"
        <tr>
            <td class='label'>Group Code</td>
            <td class='value'>{summary?.groupcode}</td>
        </tr>" : "")}
    </table>
    </table>
</td>

</tr>
</table>

");

            //            sb.Append($@"

            //<div class='payment-title'>PAYMENT INFORMATION</div>

            //<table class='data-table'>
            //<thead>
            //<tr>
            //<th>Currency</th>
            //<th>Grand Total</th>
            //<th>Fund Received</th>
            //<th>Net Balance</th>
            //</tr>
            //</thead>

            //<tbody>
            //<tr>
            //<td>INR</td>
            //<td>₹ {grandTotal:N2}</td>
            //<td>₹ {totalReceived:N2}</td>
            //<td class='red-text'>₹ {totalBalance:N2}</td>
            //</tr>
            //</tbody>
            //</table>

            //");
            if (roomStatementRows.Count > 0)
            {
                sb.Append(@"

<div class='section-heading'>Room / Accommodation Details</div>

<table class='data-table'>
<thead>
<tr>
<th style='width:50px;'>#</th>
<th>Allotted To</th>
<th>Room No</th>
<th>From Date</th>
<th>To Date</th>
<th class='text-right'>Days</th>
<th class='text-right'>Cost / Day (₹)</th>
<th class='text-right'>Line Total (₹)</th>
</tr>
</thead>
<tbody>
");

                int roomIdx = 1;

                foreach (var room in roomStatementRows)
                {
                    sb.Append($@"
<tr>
<td>{roomIdx}</td>
<td>{room.AllottedTo}</td>
<td>{room.RoomNo}</td>
<td>{FormatBillingDate(room.FromDate)}</td>
<td>{FormatBillingDate(room.ToDate)}</td>
<td class='text-right'>{room.Days}</td>
<td class='text-right'>{room.CostPerDay:N2}</td>
<td class='text-right'>{room.LineTotal:N2}</td>
</tr>
");

                    roomIdx++;
                }

                sb.Append($@"
</tbody>
<tfoot>
<tr>
<td colspan='7' class='total-label'>Total Room Cost</td>
<td class='text-right'>₹ {roomGrandTotal:N2}</td>
</tr>
</tfoot>
</table>
");
            }

            sb.Append(@"

<div class='section-heading'>Items Description</div>

<table class='data-table'>
<thead>
<tr>
<th style='width:50px;'>#</th>
<th style='width:180px;'>Item No</th>
<th style='width:120px;'>Date</th>
<th>Description</th>
<th style='width:140px;' class='text-right'>Amount (₹)</th>
</tr>
</thead>

<tbody>

");

            if (receivables == null || receivables.Count == 0)
            {
                sb.Append("<tr><td colspan='5' class='empty-cell'>No billing items found for this IPD.</td></tr>");
            }
            else
            {
                int i = 1;

                foreach (var r in receivables)
                {
                    var itemDescription = GetBillingItemDescription(r);
                    if (itemDescription.Equals("Discount", StringComparison.OrdinalIgnoreCase)
                        || itemDescription.Equals("Concession", StringComparison.OrdinalIgnoreCase))
                        continue;

                    decimal amount = 0m;
                    try { amount = Convert.ToDecimal(r.amount); } catch { }

                    sb.Append($@"

<tr>
<td>{i}</td>
<td>{Convert.ToString(r.receivableno)}</td>
<td>{Convert.ToString(r.receivabledate)}</td>
<td>{itemDescription}</td>
<td class='text-right'>{amount:N2}</td>
</tr>

");

                    i++;
                }
            }

            sb.Append($@"

</tbody>

<tfoot>
<tr>
<td colspan='4' class='total-label'>Total (INR)</td>
<td class='text-right'>₹ {displayedItemsTotal:N2}</td>
</tr>
</tfoot>

</table>

");

            foreach (var adjustmentName in new[] { "Billing Adjustments" })
            {
                var adjustmentRows = receivables == null
                    ? new List<dynamic>()
                    : receivables.Where(r =>
                        GetBillingItemDescription(r).Equals("Discount", StringComparison.OrdinalIgnoreCase)
                        || GetBillingItemDescription(r).Equals("Concession", StringComparison.OrdinalIgnoreCase)).ToList();
                if (adjustmentRows.Count == 0) continue;

                decimal totalAdjustmentAmount = 0m;

                sb.Append($@"
<div class='section-heading'>{adjustmentName}</div>
<table class='data-table'>
<thead><tr>
<th>Item No</th><th>Date</th><th>Description</th><th class='text-right'>Amount (₹)</th>
</tr></thead><tbody>");

                foreach (var adjustment in adjustmentRows)
                {
                    decimal adjustmentAmount = 0m;
                    try { adjustmentAmount = Convert.ToDecimal(adjustment.amount); } catch { }
                    totalAdjustmentAmount += Math.Abs(adjustmentAmount);
                    sb.Append($@"<tr>
<td>{Convert.ToString(adjustment.receivableno)}</td>
<td>{Convert.ToString(adjustment.receivabledate)}</td>
<td>{GetBillingItemDescription(adjustment)}</td>
<td class='text-right'>{Math.Abs(adjustmentAmount):N2}</td>
</tr>");
                }

                sb.Append($@"</tbody><tfoot><tr>
<td colspan='3' class='total-label'>Total Billing Adjustments</td>
<td class='text-right'>₹ {totalAdjustmentAmount:N2}</td>
</tr></tfoot></table>");
            }

            sb.Append(@"

<div class='section-heading'>Payment Summary</div>

<table class='data-table'>

<thead>
<tr>
<th style='width:60px;'>#</th>
<th>Received Date</th>
<th>Received Mode</th>
<th>Currency</th>
<th class='text-right'>Received Amount (₹)</th>
</tr>
</thead>

<tbody>

");

            if (receivedPayments.Count == 0)
            {
                sb.Append("<tr><td colspan='5' class='empty-cell'>No payments found.</td></tr>");
            }
            else
            {
                int j = 1;

                foreach (var p in receivedPayments)
                {
                    decimal receivedAmount = 0m;
                    string receivedDate = "-";
                    string receivedMode = "-";
                    string currency = "-";
                    try
                    {
                        receivedAmount = Convert.ToDecimal(p.receivedamount);
                    }
                    catch { }

                    try
                    {
                        var rawDate = Convert.ToString(p.paymentdate) ?? "";
                        DateTime parsedDate = DateTime.MinValue;
                        if (!string.IsNullOrWhiteSpace(rawDate) &&
                            DateTime.TryParse(rawDate, CultureInfo.GetCultureInfo("en-IN"), DateTimeStyles.None, out parsedDate))
                            receivedDate = parsedDate.ToString("dd/MM/yyyy");
                        else
                            receivedDate = string.IsNullOrWhiteSpace(rawDate) ? "-" : rawDate;
                    }
                    catch { }

                    try
                    {
                        receivedMode = Convert.ToString(p.paymentmode);
                    }
                    catch { }
                    try
                    {
                        currency = Convert.ToString(p.currency);
                    }
                    catch { }

                    sb.Append($@"
<tr>
    <td>{j}</td>
    <td>{receivedDate}</td>
    <td>{receivedMode}</td>
 <td>{currency}</td>
    <td class='text-right'>{receivedAmount:N2}</td>
</tr>");

                    j++;
                }
            }

            sb.Append($@"

</tbody>

<tfoot>

<tr>
    <td colspan='4' class='total-label'>Total Received</td>
    <td class='text-right green-text'>₹ {totalReceived:N2}</td>
</tr>

<tr>
    <td colspan='4' class='total-label'>Grand Total</td>
    <td class='text-right'>₹ {grandTotal:N2}</td>
</tr>

<tr>
    <td colspan='4' class='total-label'>Total Extra Paid</td>
    <td class='text-right' style='color:#2563eb;font-weight:700;'>₹ {totalExtraPaid:N2}</td>
</tr>

<tr>
  <td colspan='4' class='total-label'>Total Balance</td>
    <td class='text-right red-text'>₹ {totalBalance:N2}</td>
</tr>
<tr>
  <td colspan='4' class='total-label'>Amount in Words</td>
  <td class='text-right' style='white-space:normal;overflow-wrap:anywhere;line-height:1.4;'>{System.Net.WebUtility.HtmlEncode(summary?.amountinwords ?? "")}</td>
</tr>

</tfoot>

</table>

");

            if (refundPayments.Count > 0)
            {
                sb.Append(@"
<div class='section-heading'>Refund Summary</div>
<table class='data-table'>
<thead>
<tr>
<th style='width:60px;'>#</th>
<th>Refund Date</th>
<th>Refund Mode</th>
<th>Currency</th>
<th>Reference / Status</th>
<th class='text-right'>Refunded Amount</th>
</tr>
</thead>
<tbody>");

                int refundIndex = 1;
                foreach (var refund in refundPayments)
                {
                    var refundMode = GetDynamicString(refund, "refundmode");
                    if (string.IsNullOrWhiteSpace(refundMode))
                        refundMode = GetDynamicString(refund, "paymentmode");

                    var refundReference = GetDynamicString(refund, "refundreferencenumber");
                    if (string.IsNullOrWhiteSpace(refundReference))
                        refundReference = GetDynamicString(refund, "transactionreference");

                    var refundStatus = GetDynamicString(refund, "refundstatus");
                    if (string.IsNullOrWhiteSpace(refundStatus))
                        refundStatus = GetDynamicString(refund, "paymentstatus");

                    var referenceAndStatus = string.Join(" / ",
                        new[] { refundReference, refundStatus }.Where(v => !string.IsNullOrWhiteSpace(v)));

                    sb.Append($@"
<tr>
<td>{refundIndex}</td>
<td>{System.Net.WebUtility.HtmlEncode(GetPaymentDateDisplay(refund))}</td>
<td>{System.Net.WebUtility.HtmlEncode(string.IsNullOrWhiteSpace(refundMode) ? "-" : refundMode)}</td>
<td>{System.Net.WebUtility.HtmlEncode(string.IsNullOrWhiteSpace(GetDynamicString(refund, "currency")) ? "-" : GetDynamicString(refund, "currency"))}</td>
<td>{System.Net.WebUtility.HtmlEncode(string.IsNullOrWhiteSpace(referenceAndStatus) ? "-" : referenceAndStatus)}</td>
<td class='text-right'>{GetRefundAmount(refund):N2}</td>
</tr>");
                    refundIndex++;
                }

                sb.Append($@"
</tbody>
<tfoot>
<tr>
<td colspan='5' class='total-label'>Total Refunded</td>
<td class='text-right'>{refundPayments.Sum(GetRefundAmount):N2}</td>
</tr>
</tfoot>
</table>");
            }

            sb.Append(@"
</div>
</body>
</html>");

            return sb.ToString();
        }
    }
}
