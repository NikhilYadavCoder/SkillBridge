using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SkillBridge.Data;
using SkillBridge.DTOs.Resume;
using SkillBridge.Services.Resume;

namespace SkillBridge.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ResumeController : ControllerBase
    {
        private readonly IResumeService _resumeService;
        private readonly AppDbContext _context;

        public ResumeController(IResumeService resumeService, AppDbContext context)
        {
            _resumeService = resumeService;
            _context = context;
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
            {
                throw new UnauthorizedAccessException("User ID not found in token");
            }

            return userId;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadResume(IFormFile file)
        {
            try
            {
                if (file == null)
                {
                    return BadRequest(new { message = "No file provided" });
                }

                var userId = GetCurrentUserId();

                // Parse resume into structured data
                var result = await _resumeService.ExtractAndParseResumeAsync(file);
                var structuredData = result.StructuredData ?? new ResumeParsedDto();

                // Load user with profile
                var user = await _context.Users
                    .Include(u => u.UserProfile)
                    .FirstOrDefaultAsync(u => u.Id == userId);

                if (user == null)
                {
                    return NotFound(new { message = "User not found" });
                }

                var skills = structuredData.skills ?? new List<string>();
                var education = structuredData.education ?? new List<string>();
                var projects = structuredData.projects ?? new List<string>();
                var experience = structuredData.experience ?? new List<string>();

                // ALWAYS overwrite profile: assign a new UserProfile or replace its data
                if (user.UserProfile == null)
                {
                    user.UserProfile = new Models.UserProfile
                    {
                        UserId = user.Id,
                        Skills = JsonSerializer.Serialize(skills),
                        Education = JsonSerializer.Serialize(education),
                        Projects = JsonSerializer.Serialize(projects),
                        Experience = JsonSerializer.Serialize(experience),
                        UpdatedAt = DateTime.UtcNow
                    };
                }
                else
                {
                    user.UserProfile.Skills = JsonSerializer.Serialize(skills);
                    user.UserProfile.Education = JsonSerializer.Serialize(education);
                    user.UserProfile.Projects = JsonSerializer.Serialize(projects);
                    user.UserProfile.Experience = JsonSerializer.Serialize(experience);
                    user.UserProfile.UpdatedAt = DateTime.UtcNow;
                }

                await _context.SaveChangesAsync();

                return Ok(new { message = "Profile replaced successfully" });
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "An error occurred while processing the PDF", error = ex.Message });
            }
        }
    }
}
