namespace NalamVazha.Models
{
    using System;

    public class ManualRoomAvailabilityModel
    {
        public Guid? allocationid { get; set; }
        public Guid? tenantid { get; set; }
        public Guid room { get; set; }
        public DateTime fromdate { get; set; }
        public DateTime todate { get; set; }
        public string status { get; set; }
        public string blockedfor { get; set; }
        public string description { get; set; }
    }
}
