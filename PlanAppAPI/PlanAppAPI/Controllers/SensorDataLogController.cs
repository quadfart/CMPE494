using MediatR;
using Microsoft.AspNetCore.Mvc;
using PlanAppAPI.Applications.SensorDataLog.AddSensorDataLog;
using PlanAppAPI.Applications.SensorDataLog.AddSensorDataLog.Dtos;
using PlanAppAPI.Core;

namespace PlanAppAPI.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]/[action]")]
    public class SensorDataLogController : BaseApiController
    {
        public SensorDataLogController(IMediator services) : base(services)
        {
        }

        // POST api/sensordatalog/add
        [HttpPost]
        public async Task<ActionResult<SensorDataLogViewModel>> AddSensorDataLogAsync([FromBody] AddSensorDataLogRequestModel requestModel)
        {
            // Send the command to the handler through MediatR
            var result = await Mediator.Send(new Command
            {
                SensorSerialNumber = requestModel.SensorSerialNumber,
                Temperature = requestModel.Temperature,
                Moisture = requestModel.Moisture,
                SoilMoisture = requestModel.SoilMoisture
            });

            if (result.IsSuccess)
            {
                // Return the sensor data log as a view model
                var sensorDataLog = result.Value;

                var viewModel = new SensorDataLogViewModel
                {
                    Id = sensorDataLog.Id,
                    SensorSerialNumber = sensorDataLog.SensorSerialNumber, // Make sure the correct property is used
                    Temperature = sensorDataLog.Temperature,
                    Moisture = sensorDataLog.Moisture,
                    SoilMoisture = sensorDataLog.SoilMoisture,
                    Timestamp = sensorDataLog.Timestamp
                };

                return Ok(viewModel); // Return the view model as the response
            }

            // If failure, return the error message
            return BadRequest(result.Error);
        }
    }
}