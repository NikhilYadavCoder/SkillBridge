using SkillBridge.Models;

namespace SkillBridge.Services.Profile
{
    public interface IUserProfileService
    {
        Task<UserProfile?> GetProfileAsync(int userId);
        Task<UserProfile> CreateProfileAsync(int userId, List<string> skills);
        Task<UserProfile> UpdateProfileAsync(int userId, List<string> skills);
        Task<bool> DeleteProfileAsync(int userId);
    }
}
