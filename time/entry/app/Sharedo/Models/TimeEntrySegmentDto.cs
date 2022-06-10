namespace App.Sharedo.Models
{
    public class TimeEntrySegmentDto
    {
        public Guid Id{ get; set; }
        public string SegmentValue{ get; set; }
        public Guid? TimeCodeId{ get; set; }
    }
}