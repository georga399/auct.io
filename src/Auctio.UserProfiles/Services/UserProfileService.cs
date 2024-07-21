namespace Auctio.UserProfiles.Services;

public class UserProfileService
{
    private readonly UserProfileCacheService _cacheService;
    private readonly Repositories.UserProfileRepository _repository;
    public UserProfileService(UserProfileCacheService cacheService, 
        Repositories.UserProfileRepository repository)
    {
        _cacheService = cacheService;
        _repository = repository;
    }
    public async Task<DTOs.UserProfile?> GetAsync(string username)
    {
        var cachedProfile = await _cacheService.GetUserProfileAsync(username);
        if (cachedProfile != null)
        {
            return cachedProfile;
        }
        var userProfile = await _repository.GetUser(username);
        if (userProfile == null)
        {
            return null;
        }
        await _cacheService.SetUserProfileAsync(userProfile);
        return userProfile;
    }
    public async Task SetBio(string username, string bio)
    {
        var userProfile = await _repository.SetBio(username, bio);
        if(userProfile is null)
            return;
        await _cacheService.SetUserProfileAsync(userProfile!);
    }
}