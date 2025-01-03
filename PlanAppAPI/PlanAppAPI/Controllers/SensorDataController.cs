using MediatR;
using Microsoft.AspNetCore.Mvc;
using PlanAppAPI.Applications.Plant;
using PlanAppAPI.Applications.SensorData.AddSensorData;
using PlanAppAPI.Applications.SensorData.AddSensorData.Dtos;
using PlanAppAPI.Applications.SensorData.Delete;
using PlanAppAPI.Applications.SensorData.DiseasePrediction;
using PlanAppAPI.Applications.SensorData.GetLogBySensorDataId;
using PlanAppAPI.Applications.SensorData.List;
using PlanAppAPI.Applications.SensorData.SensorDataById;
using PlanAppAPI.Applications.SensorData.SensorDataById.Dtos;
using PlanAppAPI.Applications.SensorData.SensorDataGetByUserId;
using PlanAppAPI.Applications.SensorData.UpdateSensorData;
using PlanAppAPI.Applications.SensorData.UpdateSensorData.Dtos;
using PlanAppAPI.Core;

namespace PlanAppAPI.Controllers;

[Produces("application/json")]
[Route("api/[controller]/[action]")]
public class SensorDataController(IMediator services) : BaseApiController(services)
{
    [HttpPost]
    public async Task<ActionResult> AddSensorDataAsync([FromBody] AddSensorDataRequestModel request)
    {
        return HandleResult(await Mediator.Send(new AddSensorData.Command { AddSensorDataRequest = request }));
    }
    
    [HttpPut]
    public async Task<ActionResult> UpdateSensorDataAsync([FromBody] UpdateSensorDataRequestModel request)
    {
        return HandleResult(await Mediator.Send(new UpdateSensorData.Command { UpdateSensorDataRequest = request }));
    }
    
    [HttpGet]
    public async Task<ActionResult> ListSensorDataAsync()
    {
        return HandleResult(await Mediator.Send(new ListSensorData.Command{}));
    }
    
    [HttpGet("{id}")]
    public async Task<ActionResult> SensorDataByIdAsync([FromRoute] int id)
    {
        var request = new SensorDataGetByIdRequestModel { Id = id };
        return HandleResult(await Mediator.Send(new SensorDataGetById.Command { SensorData = request }));
    }
    
    [HttpGet("{id}")]
    public async Task<ActionResult> SensorDataLogByIdAsync([FromRoute] int id)
    {
        return HandleResult(await Mediator.Send(new GetLogById.Command { Id = id }));
    }
    
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteSensorDataAsync([FromRoute] int id)
    {
        return HandleResult(await Mediator.Send(new DeleteSensorData.Command { Id = id }));
    }

    [HttpGet("user/{userId}")]
    public async Task<ActionResult> GetSensorDataByUserIdAsync([FromRoute] string userId)
    {
        return HandleResult(await Mediator.Send(new SensorDataGetByUserId.Command { UserId = userId }));
    }
    [HttpPost]
    public async Task<ActionResult> DiseasePredictionAsync(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No File!");

        var result = await Mediator.Send(new DiseasePrediction.Command { File = file });

        return HandleResult(result);
    }
}
