using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using SkillBridge.Data;
using SkillBridge.Models;

namespace SkillBridge.Services.Profile
{
    public class UserProfileService : IUserProfileService
    {
        private readonly AppDbContext _context;

        public UserProfileService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<UserProfile?> GetProfileAsync(int userId)
        {
            return await _context.UserProfiles
                .Include(up => up.User)
                .FirstOrDefaultAsync(up => up.UserId == userId);
        }

        public async Task<UserProfile> CreateProfileAsync(int userId, List<string> skills)
        {
            var userProfile = new UserProfile
            {
                UserId = userId,
                Skills = JsonSerializer.Serialize(skills),
                UpdatedAt = DateTime.UtcNow
            };

            _context.UserProfiles.Add(userProfile);
            await _context.SaveChangesAsync();

            return userProfile;
        }

        public async Task<UserProfile> UpdateProfileAsync(int userId, List<string> skills)
        {
            var userProfile = await _context.UserProfiles
                .FirstOrDefaultAsync(up => up.UserId == userId);

            if (userProfile == null)
            {
                throw new InvalidOperationException($"User profile not found for user ID: {userId}");
            }

            userProfile.Skills = JsonSerializer.Serialize(skills);
            userProfile.UpdatedAt = DateTime.UtcNow;

            _context.UserProfiles.Update(userProfile);
            await _context.SaveChangesAsync();

            return userProfile;
        }

        public async Task<bool> DeleteProfileAsync(int userId)
        {
            var userProfile = await _context.UserProfiles
                .FirstOrDefaultAsync(up => up.UserId == userId);

            if (userProfile == null)
            {
                return false;
            }

            _context.UserProfiles.Remove(userProfile);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
