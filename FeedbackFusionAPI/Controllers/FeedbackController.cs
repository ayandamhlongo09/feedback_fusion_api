using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FeedbackFusionAPI.Context;
using FeedbackFusionAPI.DTOs;
using FeedbackFusionAPI.Model;

namespace FeedbackFusionAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FeedbackController : ControllerBase
    {
        private readonly MyDbContext _context;

        public FeedbackController(MyDbContext context)
        {
            _context = context;
        }

        // GET: api/Feedback/user/{userId}
        [HttpGet("{userId}")]
        public async Task<ActionResult<IEnumerable<FeedbackDto>>> GetFeedbacksByUserId(string userId)
        {
            try
            {
                var feedbacks = await _context.Feedbacks
                    .Where(f => f.UserId == userId)
                    .Select(f => new FeedbackDto
                    {
                        Id = f.Id,
                        RatingSuccessful = f.RatingSuccessful,
                        FeedbackComment = f.Comment,
                        TimeStamp = f.CreatedAt
                    })
                    .ToListAsync();

                if (feedbacks.Count == 0)
                {
                    return NotFound("No feedbacks found for the user.");
                }

                return Ok(new { feedbacks });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");
            }
        }

        // POST: api/Feedback
        [HttpPost]
        public async Task<ActionResult<IEnumerable<FeedbackDto>>> PostFeedback(CreateFeedbackDto createFeedbackDto)
        {
            try
            {
                var feedback = new Feedback
                {
                    UserId = createFeedbackDto.UserId,
                    RatingSuccessful = createFeedbackDto.RatingSuccessful,
                    Comment = createFeedbackDto.FeedbackComment,
                    CreatedAt = DateTime.UtcNow // Ensure CreatedAt is set to UTC time
                };

                _context.Feedbacks.Add(feedback);
                await _context.SaveChangesAsync();

                var allFeedbacks = await _context.Feedbacks
                    .Where(f => f.UserId == createFeedbackDto.UserId)
                    .Select(f => new FeedbackDto
                    {
                        Id = f.Id,
                        RatingSuccessful = f.RatingSuccessful,
                        FeedbackComment = f.Comment,
                        TimeStamp = f.CreatedAt
                    })
                    .ToListAsync();

                return CreatedAtAction(nameof(GetFeedbacksByUserId), new { userId = feedback.UserId }, new { feedback = allFeedbacks });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");
            }
        }
    }
}
