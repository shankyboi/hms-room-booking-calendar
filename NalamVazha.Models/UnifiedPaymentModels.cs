namespace NalamVazha.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class UnifiedReceivableItemModel
    {
        public Guid Receivableid { get; set; }
        public string receivabledate { get; set; }
        public string receivablefor { get; set; }
        public string billdetails { get; set; }
        public string remarks { get; set; }
        public string category { get; set; }
        public decimal amount { get; set; }
        public decimal paidamount { get; set; }
        public decimal balance { get; set; }
        public bool ismandatory { get; set; }
    }

    public class UnifiedPaymentInputModel
    {
        public Guid Receivableid { get; set; }
        public string ReceivableFor { get; set; }
        public decimal Balance { get; set; }
        public bool IsMandatory { get; set; }
        [Range(0, 9999999999999999d)]
        public decimal PayNow { get; set; }
    }

    public class UnifiedPaymentCollectionViewModel
    {
        public Guid PatientID { get; set; }
        public Guid? IPDNo { get; set; }
        public string Type { get; set; }
        public List<UnifiedRoomCostSummaryModel> RoomCostSummary { get; set; } = new List<UnifiedRoomCostSummaryModel>();
        public List<UnifiedReceivableItemModel> Bills { get; set; } = new List<UnifiedReceivableItemModel>();
        public List<UnifiedPaymentInputModel> Payments { get; set; } = new List<UnifiedPaymentInputModel>();
    }
    public class UnifiedRoomCostSummaryModel
    {
        public DateTime ReceivableDate { get; set; }
        public string AllottedTo { get; set; }
        public string RoomNo { get; set; }
        public decimal CostPerDay { get; set; }
        public decimal LineItemTotal { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal BalanceAmount { get; set; }
        public decimal CreditAmount { get; set; }
        public string Status { get; set; }
    }
    public class UnifiedPaymentPostModel
    {
        public Guid PatientID { get; set; }
        public Guid? IPDNo { get; set; }
        public string Type { get; set; }
        public List<UnifiedPaymentInputModel> Payments { get; set; } = new List<UnifiedPaymentInputModel>();

        public Guid? tenantid { get; set; }
        public Guid? opdnumber { get; set; }
        public string currency { get; set; }
        public decimal? receivedamount { get; set; }
        public decimal? conversionrate { get; set; }
        public string paymentmode { get; set; }
        public string receivablefor { get; set; }
        public string transactionreference { get; set; }
        public string bankname { get; set; }
        public DateTime? chequedddate { get; set; }
        public string paymentstatus { get; set; }
        public Guid? collectedby { get; set; }
        public string counterid { get; set; }
        public string remarks { get; set; }
    }
}
