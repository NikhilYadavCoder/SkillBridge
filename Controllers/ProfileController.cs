using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkillBridge.DTOs.Profile;
using SkillBridge.Services.Profile;

namespace SkillBridge.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProfileController : ControllerBase
    {
        private readonly IUserProfileService _userProfileService;

        public ProfileController(IUserProfileService userProfileService)
        {
            _userProfileService = userProfileService;
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

        [HttpGet]
        public async Task<IActionResult> GetProfile()
        {
            try
            {
                var userId = GetCurrentUserId();
                var profile = await _userProfileService.GetProfileAsync(userId);

                if (profile == null)
                {
                    return NotFound(new { message = "Profile not found" });
                }

                var skills = new List<string>();
                var education = new List<string>();
                var projects = new List<string>();
                var experience = new List<string>();
                if (!string.IsNullOrWhiteSpace(profile.Skills))
                {
                    try
                    {
                        var deserialised = JsonSerializer.Deserialize<List<string>>(profile.Skills);
                        if (deserialised != null)
                        {
                            skills = deserialised;
                        }
                    }
                    catch { }
                }

                if (!string.IsNullOrWhiteSpace(profile.Education))
                {
                    try
                    {
                        var deserialised = JsonSerializer.Deserialize<List<string>>(profile.Education);
                        if (deserialised != null)
                        {
                            education = deserialised;
                        }
                    }
                    catch { }
                }

                if (!string.IsNullOrWhiteSpace(profile.Projects))
                {
                    try
                    {
                        var deserialised = JsonSerializer.Deserialize<List<string>>(profile.Projects);
                        if (deserialised != null)
                        {
                            projects = deserialised;
                        }
                    }
                    catch { }
                }

                if (!string.IsNullOrWhiteSpace(profile.Experience))
                {
                    try
                    {
                        var deserialised = JsonSerializer.Deserialize<List<string>>(profile.Experience);
                        if (deserialised != null)
                        {
                            experience = deserialised;
                        }
                    }
                    catch { }
                }

                return Ok(new
                {
                    name = profile.User?.Name ?? string.Empty,
                    email = profile.User?.Email ?? string.Empty,
                    skills,
                    education,
                    projects,
                    experience
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateProfile([FromBody] UpdateSkillsDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var userId = GetCurrentUserId();
                var existingProfile = await _userProfileService.GetProfileAsync(userId);

                if (existingProfile != null)
                {
                    return Conflict(new { message = "Profile already exists for this user" });
                }

                var profile = await _userProfileService.CreateProfileAsync(userId, dto.Skills);
                return CreatedAtAction(nameof(GetProfile), profile);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateSkillsDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var userId = GetCurrentUserId();
                var profile = await _userProfileService.UpdateProfileAsync(userId, dto.Skills);
                return Ok(new { message = "Profile updated successfully", profile });
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteProfile()
        {
            try
            {
                var userId = GetCurrentUserId();
                var deleted = await _userProfileService.DeleteProfileAsync(userId);

                if (!deleted)
                {
                    return NotFound(new { message = "Profile not found" });
                }

                return Ok(new { message = "Profile deleted successfully" });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }
    }
}
