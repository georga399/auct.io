using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace Auctio.UserProfiles.Services;
public class UserProfileCacheService
{
    private readonly IDistributedCache _cache;
    private readonly TimeSpan _cacheDuration = TimeSpan.FromMinutes(30);

    public UserProfileCacheService(IDistributedCache cache)
    {
        _cache = cache;
    }

    public async Task<DTOs.UserProfile?> GetUserProfileAsync(string userId)
    {
        var cachedProfile = await _cache.GetStringAsync(userId);
        return cachedProfile == null ? null : JsonSerializer.Deserialize<DTOs.UserProfile>(cachedProfile);
    }

    public async Task SetUserProfileAsync(DTOs.UserProfile profile)
    {
        var options = new DistributedCacheEntryOptions().SetAbsoluteExpiration(_cacheDuration);
        await _cache.SetStringAsync(profile.UserName, JsonSerializer.Serialize(profile), options);
    }

    public async Task RemoveUserProfileAsync(string username)
    {
        await _cache.RemoveAsync(username);
    }
}