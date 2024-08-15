using Newtonsoft.Json;

namespace RareCrewTask.Models
{
    public class EmployeeTimeTrackModel
    {
        public string Id { get; set; }
        public string EmployeeName { get; set; }
        [JsonProperty("StarTimeUtc")]
        public DateTime? StartTimeUtc { get; set; }
        public DateTime? EndTimeUtc { get; set; }
        public string EntryNotes { get; set; }
        public DateTime? DeletedOn { get; set; }
        public double TotalTimeWorked { get; set; }
    }
}
