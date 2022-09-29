using System.Security.Claims;
using api.Entities;
using api.Repos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

[ApiController]
[Authorize]
[Route("[controller]")]
public class UserController : ControllerBase
{

    private readonly IUserRepo _userRepo;

    public UserController(IUserRepo userRepo)
    {
        _userRepo = userRepo;
    }

    [HttpGet]
    public async Task<IEnumerable<User>> Get()
    {
        var user = User.FindFirst(ClaimTypes.Email)?.Value;
        var headers = Request.Headers;
        var ip = Request.HttpContext.Connection.RemoteIpAddress;

        return await _userRepo.All();
    }

    [Route("{firstname}")]
    [HttpGet]
    public async Task<List<User>> GetByFirstname(string firstname)
    {
        return await _userRepo.FindByFirstName(firstname);
    }
}