using System;
using System.Collections.Generic;

namespace NalamVazha.Models
{
    /// <summary>
    /// Calendar-originated Direct IPD room selections. Room and room-type
    /// dependencies are re-resolved on the server before admission is saved.
    /// </summary>
    public sealed class CalendarDirectIPDSelectionModel
    {
        public List<CalendarDirectIPDRoomSegmentModel> rooms { get; set; } = new();
    }

    public sealed class CalendarDirectIPDRoomSegmentModel
    {
        public Guid roomid { get; set; }
        public Guid? roomtypeid { get; set; }
        public string roomnumber { get; set; }
        public string roomtypename { get; set; }
        public string role { get; set; }
        public DateTime fromdate { get; set; }
        public DateTime todate { get; set; }
    }
}
