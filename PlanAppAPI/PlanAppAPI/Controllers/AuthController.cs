using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlanAppAPI.Applications.Auth.GetById;
using PlanAppAPI.Applications.Auth.Login;
using PlanAppAPI.Applications.Auth.SignUp;
using PlanAppAPI.Core;
using PlanAppAPI.DTOs;

namespace PlanAppAPI.Controllers;

[Produces("application/json")]
[Route("api/[controller]/[action]")]
public class AuthController(IMediator services) : BaseApiController(services)
{
    [HttpPost]
    public async Task<ActionResult<string>> LoginAsync([FromBody] LoginDto loginDto)
    {
        return HandleResult(await Mediator.Send(new Login.Command { LoginDto  = loginDto}));
    }
    
    [HttpGet("{id}")]
    public async Task<ActionResult<string>> GetUserByIdAsync([FromRoute] int id)
    {
        return HandleResult(await Mediator.Send(new GetById.Command { Id  = id}));
    }
    [HttpPost]
    public async Task<ActionResult<string>> SignUpAsync([FromBody] SignUpDto signUpDto)
    {
        var result = await Mediator.Send(new SignUp.Command { SignUpDto = signUpDto });
        return HandleResult(result);
    }
}