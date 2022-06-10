using App.Sharedo;
using App.Sharedo.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace App.Modules
{
    [ApiController]
    [Route("/api/time")]
    public class TimeController : ControllerBase
    {
        private readonly ITimeApi _time;
        private readonly IUserApi _user;

        public TimeController(ITimeApi time, IUserApi user)
        {
            _time = time;
            _user = user;
        }        

        [HttpGet("categories")]
        [Authorize]
        public async Task<IActionResult> GetCategories()
        {
            var categories = await _time.GetCategories();
            return new JsonResult(categories);
        }

        [HttpGet("capture/{category}/{workItemId}")]
        [Authorize]
        public async Task<IActionResult> GetCapture(string category, Guid workItemId)
        {
            var capture = await _time.GetCapture(category, workItemId);
            return new JsonResult(capture);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> SendTime(TimeEntry entry)
        {
            var userId = await _user.GetCurrentUserId();
            
            await _time.SendTime(entry.ToApiDto(userId));

            return Ok();
        }

        public class TimeEntrySegment
        {
            public Guid Id{ get; set; }
            public string SegmentValue{ get; set; }
            public Guid? TimeCodeId{ get; set; }
        }

        public class TimeEntry
        {
            public Guid WorkItemId{ get; set; }
            public string CategorySystemName{ get; set; }
            public int Seconds{ get; set; }
            public List<TimeEntrySegment> Segments{ get; set; }

            public TimeEntry()
            {
                Segments = new List<TimeEntrySegment>();
            }

            public TimeEntryDto ToApiDto(Guid userId)
            {
                var endDateTime = DateTime.UtcNow;
                var startDateTime = endDateTime.AddSeconds(-Seconds);
                
                var segments = Segments.Select(seg => new TimeEntrySegmentDto
                {
                    Id = seg.Id,
                    SegmentValue = seg.SegmentValue,
                    TimeCodeId = seg.TimeCodeId
                }).ToList();

                return new TimeEntryDto
                {
                    SharedoId = WorkItemId,
                    OdsId = userId,
                    StartDateTime = startDateTime,
                    EndDateTime = endDateTime,
                    DurationSeconds = Seconds,
                    BillingNotes = "Created from time entry API sample on github",
                    RegenerateBillingNotes = false,
                    CreatedBySystem = false,
                    TimeCodeCategorySystemName = CategorySystemName,
                    IsAutomatic = false,
                    Segments = segments
                };
            }

        }
    }
}