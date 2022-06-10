namespace App.Sharedo.Models
{
    public class TimeEntryDto
    {
        public Guid SharedoId { get; set; }
        public Guid OdsId{ get; set; }
        public DateTime StartDateTime{ get; set; }
        public DateTime EndDateTime{ get; set; }
        public int DurationSeconds{ get; set; }
        public string BillingNotes{ get; set; }
        public bool RegenerateBillingNotes{ get; set; }
        public bool CreatedBySystem{ get; set; }
        public string TimeCodeCategorySystemName{ get; set; }
        public bool IsAutomatic{ get; set; }
        public List<TimeEntrySegmentDto> Segments{ get; set; }

        public TimeEntryDto()
        {
            Segments = new List<TimeEntrySegmentDto>();
        }
    }
}