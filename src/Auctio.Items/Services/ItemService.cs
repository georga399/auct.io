using MassTransit;

namespace Auctio.Items.Services;
public class ItemService
{
    private readonly Repositories.ItemsRepository _repository;
    private readonly ItemCacheService _cacheService;
    public ItemService(Repositories.ItemsRepository repository, 
        ItemCacheService cacheService)
    {
        _repository = repository;
        _cacheService = cacheService;
    }
    public async Task<IEnumerable<DTOs.Item>> GetItemsAsync(string username)
    {
        return await _repository.GetItems(username);
    }
    public async Task<IEnumerable<DTOs.Item>> GetItemsByCursor(uint cursor = uint.MaxValue)
    {
        return await _repository.GetItemsByCursor(cursor);
    }
    public async Task<DTOs.Item?> GetItem(int id)
    {
        var cachedItem = await _cacheService.GetItemAsync(id);
        if(cachedItem is not null)
        {
            return cachedItem;
        }
        var item = await _repository.GetItem(id);
        return item;
    }
    
    public async Task<DTOs.Item?> CreateItem(string userId, string userName, 
        string itemName, string? itemDescription, double? itemCost)
    {
        var createdItem = await _repository.CreateItem(userId, userName, itemName, itemDescription, itemCost);
        return createdItem;
    }
}