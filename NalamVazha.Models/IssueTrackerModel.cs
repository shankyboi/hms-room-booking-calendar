namespace NalamVazha.Models
{
    using System;

    public class IssueTrackerModel
    {
        public Guid?   Issuetrackerid { get; set; }
        public string  entityname     { get; set; }
        public string  filename       { get; set; }
        public string  pageurl        { get; set; }
        public string  issuetext      { get; set; }
        public string  priority       { get; set; }   // Low | Medium | High | Critical
        public string  status         { get; set; }   // Open | In Progress | Resolved | Closed
        public string  reporterdesc   { get; set; }
        public DateTime? createddate  { get; set; }
        public DateTime? modifieddate { get; set; }
        public bool?   isdeleted      { get; set; }
        /// <summary>Full base64 JPEG data URI – populated only on Add and Get_Screenshot.</summary>
        public string  screenshotdata { get; set; }
        /// <summary>True when a screenshot was attached – populated in list queries (avoids loading the large blob).</summary>
        public bool?   hasscreenshot  { get; set; }
    }
}
