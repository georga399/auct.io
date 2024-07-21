using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace Auctio.Items.Services;
public class ItemCacheService
{
    private readonly IDistributedCache _cache;
    private readonly TimeSpan _cacheDuration = TimeSpan.FromMinutes(30);

    public ItemCacheService(IDistributedCache cache)
    {
        _cache = cache;
    }

    private string GetCacheKey(int itemId) => $"Item_{itemId}";

    public async Task<DTOs.Item?> GetItemAsync(int itemId)
    {
        var cachedItem = await _cache.GetStringAsync(GetCacheKey(itemId));
        return cachedItem == null ? null : JsonSerializer.Deserialize<DTOs.Item>(cachedItem)!;
    }

    public async Task SetItemAsync(DTOs.Item item)
    {
        var options = new DistributedCacheEntryOptions().SetAbsoluteExpiration(_cacheDuration);
        await _cache.SetStringAsync(GetCacheKey(item.Id), JsonSerializer.Serialize(item), options);
    }

    public async Task RemoveItemAsync(int itemId)
    {
        await _cache.RemoveAsync(GetCacheKey(itemId));
    }

    public async Task UpdateItemAsync(DTOs.Item item)
    {
        await SetItemAsync(item);
    }
}