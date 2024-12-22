using MediatR;
using Microsoft.AspNetCore.Mvc;
using PlanAppAPI.Applications.Plant;
using PlanAppAPI.Core;

namespace PlanAppAPI.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]/[action]")]
    public class PlantController : BaseApiController
    {
        public PlantController(IMediator services) : base(services) { }

        // Existing methods...

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

        // New method to get Plant by SensorSerialNumber
        [HttpGet("{sensorSerialNumber}")]
        public async Task<ActionResult> PlantBySensorSerialNumberAsync([FromRoute] string sensorSerialNumber)
        {
            var result = await Mediator.Send(new PlantGetBySensorSerialNumber.Command { SensorSerialNumber = sensorSerialNumber });

            // If the result is successful, return the plant data
            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }

            // If not, return an error message
            return BadRequest(result.Error);
        }
    }
}