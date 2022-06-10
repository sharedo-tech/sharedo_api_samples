namespace App.Sharedo.Models
{
    public class TimeCode
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public List<TimeCode> Children { get; set; }

        public TimeCode()
        {
            Children = new List<TimeCode>();
        }
    }
}

