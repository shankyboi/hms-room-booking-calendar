namespace NalamVazha.Models
{
    using System;
    using System.Collections.Generic;

    // UI schema + values for Holistic History (screening) linked to an IPD form.
    // Note: IPDFormid is intentionally NOT part of this model; the controller passes it separately.
    public class HolisticHistoryModel
    {
        public Guid? HolisticHistoryid { get; set; }
        public Guid? tenantid { get; set; }

        // Answers payload (encoded as JSON string). Stored as jsonb in DB, but we keep UI clean.
        public string? holisticanswers { get; set; }

        public string? verifiedstatus { get; set; }
        public Guid? verifiedby { get; set; }

        // Filled from API token in WebApi controller.
        public Guid? createduser { get; set; }
        public Guid? modifieduser { get; set; }

        // Dynamic UI schema (derived from Holistic History.xlsx "Screening" sheet).
        public List<HolisticHistorySectionModel> sections { get; set; } = new List<HolisticHistorySectionModel>();
    }

    public class HolisticHistorySectionModel
    {
        public string? title { get; set; }
        public List<HolisticHistoryFieldModel> fields { get; set; } = new List<HolisticHistoryFieldModel>();
    }

    public class HolisticHistoryFieldModel
    {
        // Used as HTML input name and as key in holisticanswers JSON.
        public string? fieldId { get; set; }

        public string? label { get; set; }

        // checkbox | radio | text
        public string? inputType { get; set; }

        // For checkbox/radio.
        public List<string> options { get; set; } = new List<string>();

        // Extra grouping from the sheet (like "Indicators" / "Habits & Context").
        public string? group { get; set; }
    }
}

