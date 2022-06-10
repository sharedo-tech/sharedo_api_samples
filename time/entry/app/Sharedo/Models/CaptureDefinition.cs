namespace App.Sharedo.Models
{
    public class CaptureDefinition
    {
        public string SystemName { get; set; }
        public string Name { get; set; }
        public List<CaptureSegment> Segments { get; set; }

        public CaptureDefinition()
        {
            Segments = new List<CaptureSegment>();
        }
    }
}

