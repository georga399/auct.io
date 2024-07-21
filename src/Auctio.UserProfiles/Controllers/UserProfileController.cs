using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Auctio.UserProfiles.Controllers;
[Route("api/[controller]")]
[ApiController]
[Authorize]
public class UserProfileController : ControllerBase
{
    private readonly Services.UserProfileService _service;
    public UserProfileController(Services.UserProfileService service) 
    {
        _service = service;
    }
    [HttpGet("{username}")]
    public async Task<IActionResult> GetUserInfo(string username)
    {
        var user = await _service.GetAsync(username);
        if(user is null)
            return NotFound();
        return Ok(user);
    }
    [HttpPost("set-bio")]
    public async Task<IActionResult> SetBio([FromBody] DTOs.SetBioDTO dto)
    {
        var username = User.FindFirst(ClaimTypes.Name)!.Value;
        Console.WriteLine($"Username: {username}");
        await _service.SetBio(username, dto.bio);
        return Ok(); 
    }
}