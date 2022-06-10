namespace App.Sharedo.Models
{
    public class CaptureSegment
    {
        public Guid Id{ get; set; }
        public string Name{ get; set; }
        public string CaptureType{ get; set; }
        public bool IsMandatoryForEntry{ get; set; }
        public bool IsMandatoryForSubmission{ get; set; }
        public List<TimeCode> TimeCodes{ get; set; }

        public CaptureSegment()
        {
            TimeCodes = new List<TimeCode>();
        }
    }
}

