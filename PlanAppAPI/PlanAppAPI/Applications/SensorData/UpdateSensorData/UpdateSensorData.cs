using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;
using PlanAppAPI.Applications.SensorData.AddSensorData.Dtos;
using PlanAppAPI.Applications.SensorData.BaseDto;
using PlanAppAPI.Applications.SensorData.UpdateSensorData.Dtos;
using PlanAppAPI.Core;

namespace PlanAppAPI.Applications.SensorData.UpdateSensorData;

public class UpdateSensorData
{
    public class Command:  IRequest<Result<SensorDataViewModel>>
    {
        public required UpdateSensorDataRequestModel UpdateSensorDataRequest { get; set; }
    }

    public class Handler : IRequestHandler<Command, Result<SensorDataViewModel>>
    {
        private readonly DataContext _context;
        private readonly ILogger<UpdateSensorData> _logger;

        public Handler(DataContext context, ILogger<UpdateSensorData> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Result<SensorDataViewModel>> Handle(Command request, CancellationToken cancellationToken)
        {
            var tran = await _context.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                var sensorDataLog = await _context.SensorDataLogs
                    .OrderByDescending(x => x.Id)
                    .Include(x => x.SensorData)
                    .AsNoTracking()
                    .LastOrDefaultAsync(x => x.SensorDataId == request.UpdateSensorDataRequest.Id, cancellationToken)
                    .ConfigureAwait(false);

                if (sensorDataLog == null)
                {
                    return Result<SensorDataViewModel>.Failure($"Sensor data not found with id {request.UpdateSensorDataRequest.Id}");
                }

                if (sensorDataLog.SensorData == null || sensorDataLog.SensorData.Status == 0)
                {
                    throw new Exception("Sensor data not found");
                }
                
                var updatedSensorData = new Domain.Tables.SensorData
                {
                    Id = sensorDataLog.SensorData.Id,
                    Status = 1
                };
                
                var updatedSensorDataLog = new Domain.Tables.SensorDataLog
                {
                    SensorDataId = sensorDataLog.SensorDataId,
                    Temperature = request.UpdateSensorDataRequest.Temperature,
                    Moisture = request.UpdateSensorDataRequest.Moisture,
                    Timestamp = DateTime.UtcNow
                };
                
                if (request.UpdateSensorDataRequest.UserId != null && request.UpdateSensorDataRequest.UserId != 0)
                {
                    updatedSensorData.UserId = request.UpdateSensorDataRequest.UserId;
                }
                
                if (request.UpdateSensorDataRequest.PlantId != null && request.UpdateSensorDataRequest.PlantId != 0)
                {
                    updatedSensorData.PlantId = request.UpdateSensorDataRequest.PlantId;
                }
                
                sensorDataLog.SensorData = updatedSensorData;
                _context.SensorData.Update(sensorDataLog.SensorData);
                await _context.SaveChangesAsync(cancellationToken);
                
                _context.SensorDataLogs.Add(updatedSensorDataLog);
                await _context.SaveChangesAsync(cancellationToken);
                
                await tran.CommitAsync(cancellationToken);
                return await Task.FromResult(Result<SensorDataViewModel>.Success(new SensorDataViewModel
                {
                    Id = sensorDataLog.SensorData.Id
                }));
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Error while updating sensor data {request.UpdateSensorDataRequest.Id}");
                await tran.RollbackAsync(cancellationToken);
                return Result<SensorDataViewModel>.Failure($"Error while updatind sensor data {request.UpdateSensorDataRequest.Id}: {e.Message}");
            }
        }
    }
}