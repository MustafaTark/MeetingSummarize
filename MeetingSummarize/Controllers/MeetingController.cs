using System;
using System.Linq;
using System.Threading.Tasks;
using MeetingSummarize.Data;
using MeetingSummarize.Models;
using MeetingSummarize.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MeetingSummarize.Controllers
{
    public class MeetingController : Controller
    {
        private readonly AppDbContext _db;
        private readonly ISummarizationService _summarizationService;

        public MeetingController(AppDbContext db, ISummarizationService summarizationService)
        {
            _db = db;
            _summarizationService = summarizationService;
        }

        public async Task<IActionResult> Index()
        {
            var meetings = await _db.Meetings.OrderByDescending(m => m.StartTime).ToListAsync();
            return View(meetings);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Meeting model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            model.Id = Guid.NewGuid();
            model.StartTime = DateTimeOffset.UtcNow;
            _db.Meetings.Add(model);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Join), new { id = model.Id });
        }

        public async Task<IActionResult> Join(string id)
        {
            if (!Guid.TryParse(id, out var meetingId)) return NotFound();
            var meeting = await _db.Meetings.FindAsync(meetingId);
            if (meeting == null) return NotFound();
            ViewBag.MeetingId = meeting.Id.ToString();
            ViewBag.Title = meeting.Title;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> End(string id)
        {
            if (!Guid.TryParse(id, out var meetingId)) return NotFound();
            var meeting = await _db.Meetings.FindAsync(meetingId);
            if (meeting == null) return NotFound();

            meeting.EndTime = DateTimeOffset.UtcNow;

            // For demo, use transcript already on entity; in real app collect from stream aggregator
            var (summary, decisions, actions) = await _summarizationService.SummarizeAsync(meeting.Transcript ?? string.Empty);
            meeting.Summary = summary;
            meeting.Decisions = decisions;
            meeting.Actions = actions;
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}


