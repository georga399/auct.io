using Auctio.Identity.Services;
using Auctio.Shared.Masstransit;
using MassTransit;
using Microsoft.AspNetCore.Mvc;

namespace Auctio.Identity.Controllers;
[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly IdentityProvider _identityProvider;
    public AuthController(IdentityProvider identityProvider, IPublishEndpoint publishEndpoint)
    {
        _identityProvider = identityProvider;
        _publishEndpoint = publishEndpoint;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] DTOs.AuthModel authModel)
    {
        var res = await _identityProvider.Login(authModel.Username, authModel.Password);
        if(res is null)
            return NotFound();
        return Ok(new {access_token = res});
    }
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] DTOs.AuthModel authModel)
    { 
        var res = await _identityProvider.Register(authModel.Username, authModel.Password);
        if(res is null)
            return BadRequest();
        await _publishEndpoint.Publish<CreateUser>(res);
        return Ok(res);
    }
}