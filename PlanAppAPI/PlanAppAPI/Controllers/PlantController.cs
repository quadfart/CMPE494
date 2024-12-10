using Microsoft.AspNetCore.Mvc;
using PlanAppAPI.Applications.Plant;
using PlanAppAPI.Core;

namespace PlanAppAPI.Controllers;

[Produces("application/json")]
[Route("api/[controller]/[action]")]
public class PlantController(IServiceProvider services) : BaseApiController(services)
{ 
    [HttpGet("{id}")]
    public async Task<ActionResult> PlantByIdAsync([FromRoute] int id)
    {
        return HandleResult(await Mediator.Send(new PlantGetById.Command { Id = id }));
    }
    
    [HttpGet("{name}")]
    public async Task<ActionResult> PlantByNameAsync([FromRoute] string name)
    {
        return HandleResult(await Mediator.Send(new PlantGetByName.Command { Name = name }));
    }
}