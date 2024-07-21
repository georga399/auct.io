using System.Security.Claims;
using Auctio.Items.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Npgsql;

namespace Auctio.Items.Controllers;
[Route("api/[controller]")]
[ApiController]
[Authorize]
public class ItemsController : ControllerBase
{
    private readonly ItemService _itemService;
    public ItemsController(ItemService itemService)
    {
        _itemService = itemService;
    }

    [HttpGet("")]
    public async Task<IActionResult> GetItems(string? username)
    {
        var items = await _itemService.GetItemsAsync(username!);
        return Ok(items);
    }
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetItem(int itemId)
    {
        var item = await _itemService.GetItem(itemId);
        return Ok(item);
    }
    [HttpGet("myitems")]
    public async Task<IActionResult> GetMyItems()
    {
        // Get items of the current user
        var userName = User.FindFirst(ClaimTypes.Name)!.Value;
        var items = await _itemService.GetItemsAsync(userName);
        return Ok(items);
    }
    [HttpPost("item")]
    public async Task<IActionResult> CreateItem([FromBody] DTOs.CreateItem item)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var userName = User.FindFirst(ClaimTypes.Name)?.Value;
        var createdItem = await _itemService.CreateItem(userId!, userName!, item.Name, 
            item.Description, item.Cost);
        return Ok(createdItem);
    }
}