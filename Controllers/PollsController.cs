using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PollSurveyBuilder.API.Data;
using PollSurveyBuilder.API.DTOs;
using PollSurveyBuilder.API.Models;
using Microsoft.AspNetCore.SignalR;
using PollSurveyBuilder.API.Hubs;

namespace PollSurveyBuilder.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PollsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IHubContext<PollHub> _hubContext;

        public PollsController(
            ApplicationDbContext context,
            IHubContext<PollHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        private string GenerateCode()
        {
            return Guid.NewGuid()
                .ToString("N")
                .Substring(0, 8)
                .ToUpper();
        }

        // POST: api/polls
        [HttpPost]
        [HttpPost]
        public async Task<IActionResult> CreatePoll(CreatePollDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var cleanedOptions = dto.Options
                .Where(option => !string.IsNullOrWhiteSpace(option))
                .Select(option => option.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            if (cleanedOptions.Count < 2 || cleanedOptions.Count > 6)
            {
                return BadRequest(
                    "A poll must contain between 2 and 6 unique, non-empty options."
                );
            }

            var poll = new Poll
            {
                Code = GenerateCode(),
                Question = dto.Question.Trim(),
                ExpiresAt = dto.ExpiresAt,
                CreatorToken = Guid.NewGuid().ToString("N")
            };

            int order = 1;

            foreach (var option in cleanedOptions)
            {
                poll.Options.Add(new PollOption
                {
                    Text = option,
                    DisplayOrder = order++
                });
            }

            _context.Polls.Add(poll);

            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetPoll),
                new { code = poll.Code },
                new
                {
                    poll.Code,
                    poll.Question,
                    poll.ExpiresAt,
                    poll.CreatorToken
                });
        }

        // GET: api/polls/ABCDEFGH
        [HttpGet("{code}")]
        public async Task<IActionResult> GetPoll(string code)
        {
            Console.WriteLine($"Incoming code: {code}");

            var poll = await _context.Polls
                .Include(p => p.Options)
                .FirstOrDefaultAsync(p => p.Code == code);

            if (poll == null)
                return NotFound();

            if (poll.IsClosed)
                return BadRequest("Poll is closed.");

            if (poll.ExpiresAt.HasValue && poll.ExpiresAt < DateTime.UtcNow)
                return BadRequest("Poll has expired.");

            return Ok(new
            {
                poll.Code,
                poll.Question,
                poll.ExpiresAt,
                Options = poll.Options
                    .OrderBy(o => o.DisplayOrder)
                    .Select(o => new
                    {
                        o.Id,
                        o.Text,
                        o.DisplayOrder
                    })
            });
        }


        // POST: api/polls/{code}/vote
        [HttpPost("{code}/vote")]
        public async Task<IActionResult> Vote(string code, VoteDto dto)
        {
            var poll = await _context.Polls
                .Include(p => p.Options)
                .Include(p => p.Votes)
                .FirstOrDefaultAsync(p => p.Code == code);

            if (poll == null)
                return NotFound();

            if (poll.IsClosed)
                return BadRequest("Poll is closed.");

            if (poll.ExpiresAt.HasValue && poll.ExpiresAt < DateTime.UtcNow)
                return BadRequest("Poll has expired.");

            if (!poll.Options.Any(o => o.Id == dto.OptionId))
                return BadRequest("Invalid option.");

            bool alreadyVoted = poll.Votes.Any(v => v.VoterToken == dto.VoterToken);

            if (alreadyVoted)
                return BadRequest("This voter has already voted.");

            var vote = new Vote
            {
                PollId = poll.Id,
                PollOptionId = dto.OptionId,
                VoterToken = dto.VoterToken
            };

            _context.Votes.Add(vote);

            await _context.SaveChangesAsync();

            var updatedResults = await _context.PollOptions
                .Where(o => o.PollId == poll.Id)
                .OrderBy(o => o.DisplayOrder)
                .Select(o => new
                {
                    o.Id,
                    o.Text,
                    Votes = o.Votes.Count
                })
                .ToListAsync();

            await _hubContext.Clients
                .Group(code.ToUpper())
                .SendAsync("ReceiveResults", new
                {
                    poll.Code,
                    poll.Question,
                    Results = updatedResults
                });

            return Ok(new
            {
                message = "Vote recorded successfully."
            });


        }

        // GET: api/polls/{code}/results
        [HttpGet("{code}/results")]
        public async Task<ActionResult<PollResultDto>> GetResults(string code)
        {
            var poll = await _context.Polls
                .Include(p => p.Options)
                    .ThenInclude(o => o.Votes)
                .FirstOrDefaultAsync(p => p.Code == code);

            if (poll == null)
                return NotFound();

            var result = new PollResultDto
            {
                Question = poll.Question,
                Results = poll.Options
                    .OrderBy(o => o.DisplayOrder)
                    .Select(o => new OptionResultDto
                    {
                        Id = o.Id,
                        Text = o.Text,
                        Votes = o.Votes.Count
                    })
                    .ToList()
            };

            return Ok(result);
        }

        // POST: api/polls/{code}/close
        [HttpPost("{code}/close")]
        public async Task<IActionResult> ClosePoll(
            string code,
            ClosePollDto dto)
        {
            var poll = await _context.Polls
                .FirstOrDefaultAsync(p => p.Code == code);

            if (poll == null)
                return NotFound("Poll not found.");

            if (poll.CreatorToken != dto.CreatorToken)
                return Unauthorized(
                    "Only the poll creator can close this poll."
                );

            if (poll.IsClosed)
                return BadRequest("Poll is already closed.");

            poll.IsClosed = true;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Poll closed."
            });
        }


    }
}