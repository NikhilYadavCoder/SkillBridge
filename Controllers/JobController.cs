using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkillBridge.DTOs.Job;
using SkillBridge.Services.Job;
using SkillBridge.Services.Profile;
using SkillBridge.Services.AI;

namespace SkillBridge.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class JobController : ControllerBase
    {
        private readonly IUserProfileService _userProfileService;
        private readonly IJobAnalysisService _jobAnalysisService;
        private readonly IRoadmapService _roadmapService;

        public JobController(
            IUserProfileService userProfileService,
            IJobAnalysisService jobAnalysisService,
            IRoadmapService roadmapService)
        {
            _userProfileService = userProfileService;
            _jobAnalysisService = jobAnalysisService;
            _roadmapService = roadmapService;
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

        [HttpPost("analyze")]
        public async Task<IActionResult> Analyze([FromBody] JobAnalysisRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var userId = GetCurrentUserId();
                var profile = await _userProfileService.GetProfileAsync(userId);

                if (profile == null || string.IsNullOrWhiteSpace(profile.Skills))
                {
                    return NotFound(new { message = "User profile or skills not found" });
                }

                List<string>? skills;
                try
                {
                    skills = JsonSerializer.Deserialize<List<string>>(profile.Skills);
                }
                catch
                {
                    return StatusCode(StatusCodes.Status500InternalServerError,
                        new { message = "Failed to read stored skills for this user" });
                }

                skills ??= new List<string>();

                var result = await _jobAnalysisService.AnalyzeAsync(
                    skills,
                    request.Role,
                    request.PageNumber,
                    request.PageSize);

                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (FileNotFoundException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "An error occurred while analyzing jobs", error = ex.Message });
            }
        }

        [HttpPost("roadmap")]
        public async Task<IActionResult> GenerateRoadmap([FromBody] List<string> skills)
        {
            var result = await _roadmapService.GenerateRoadmapAsync(skills ?? new List<string>());
            return Ok(result);
        }
    }
}
