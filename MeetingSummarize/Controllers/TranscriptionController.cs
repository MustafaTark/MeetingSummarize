using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using MeetingSummarize.Data;
using MeetingSummarize.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MeetingSummarize.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TranscriptionController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly ITranscriptionService _transcriptionService;

        public TranscriptionController(AppDbContext db, ITranscriptionService transcriptionService)
        {
            _db = db;
            _transcriptionService = transcriptionService;
        }

        [HttpGet("{meetingId}")]
        public async Task<IActionResult> GetTranscript(string meetingId)
        {
            if (!Guid.TryParse(meetingId, out var id)) return BadRequest("Invalid meeting id");
            var meeting = await _db.Meetings.AsNoTracking().FirstOrDefaultAsync(m => m.Id == id);
            if (meeting == null) return NotFound();
            return Ok(new { transcript = meeting.Transcript ?? string.Empty });
        }

        [HttpPost("{meetingId}/chunk")]
        public async Task<IActionResult> UploadChunk(string meetingId, IFormFile audio)
        {
            if (!Guid.TryParse(meetingId, out var id)) return BadRequest("Invalid meeting id");
            var meeting = await _db.Meetings.FirstOrDefaultAsync(m => m.Id == new Guid(meetingId));
            if (meeting == null) return NotFound();
            if (audio == null || audio.Length == 0) return BadRequest("No audio file");

            await using var stream = audio.OpenReadStream();
            var resultJson = await _transcriptionService.TranscribeChunkAsync(stream, audio.FileName, audio.ContentType);

            // Try parse text from Whisper JSON { text: "..." }
            string textToAppend = string.Empty;
            try
            {
                using var doc = JsonDocument.Parse(resultJson);
                if (doc.RootElement.TryGetProperty("text", out var textProp))
                {
                    textToAppend = textProp.GetString() ?? string.Empty;
                }
            }
            catch
            {
                // Fallback: append raw
                textToAppend = resultJson;
            }

            if (!string.IsNullOrWhiteSpace(textToAppend))
            {
                var sb = new StringBuilder(meeting.Transcript ?? string.Empty);
                if (sb.Length > 0) sb.AppendLine();
                sb.Append(textToAppend);
                meeting.Transcript = sb.ToString();
                await _db.SaveChangesAsync();
            }

            return Ok(new { ok = true, appended = textToAppend?.Length ?? 0 });
        }
    }
}


