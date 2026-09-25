namespace NalamVazha.Models
{
    using System.Collections.Generic;

    /// <summary>Payload sent from Admin → WebApi for bulk assessment import.</summary>
    public class AssessmentImportPayload
    {
        public string templatename { get; set; }   // optional — omit to skip template creation
        public string tenantid     { get; set; }   // null/empty = global (shared across tenants)
        public List<AssessmentImportQuestion> questions { get; set; } = new List<AssessmentImportQuestion>();
    }

    /// <summary>One question row parsed from the Excel sheet.</summary>
    public class AssessmentImportQuestion
    {
        public string categoryname    { get; set; }
        public string subcategoryname { get; set; }
        public string questiontext    { get; set; }
        public string answertype      { get; set; }  // SingleSelect|MultiSelect|Boolean|Text|Number|Scale|MultilineText
        public string optionvalue     { get; set; }  // comma-separated option values
        public int?   scalerangemin   { get; set; }
        public int?   scalerangemax   { get; set; }
        public bool   isrequired      { get; set; } = true;
    }

    /// <summary>Result returned from WebApi back to Admin view.</summary>
    public class AssessmentImportResult
    {
        public bool   success           { get; set; }
        public string message           { get; set; }
        public int    categoriesAdded   { get; set; }
        public int    subcategoriesAdded{ get; set; }
        public int    questionsAdded    { get; set; }
        public int    questionsSkipped  { get; set; }
        public bool   templateCreated   { get; set; }
        public string templatename      { get; set; }
        public List<string> warnings    { get; set; } = new List<string>();
    }
}
