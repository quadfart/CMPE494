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
            .LastOrDefaultAsync(x => x.SensorDataId == request.UpdateSensorDataRequest.Id, cancellationToken);

        if (sensorDataLog == null || sensorDataLog.SensorData == null || sensorDataLog.SensorData.Status == 0)
        {
            throw new Exception("Sensor data not found");
        }

        var existingSensorData = await _context.SensorData
            .FirstOrDefaultAsync(x => x.Id == sensorDataLog.SensorData.Id, cancellationToken);

        if (existingSensorData == null)
        {
            throw new Exception("Sensor data not found");
        }

        // Update fields

        if (request.UpdateSensorDataRequest.PlantId != null && request.UpdateSensorDataRequest.PlantId != 0)
        {
            existingSensorData.PlantId = request.UpdateSensorDataRequest.PlantId;
        }

        if (request.UpdateSensorDataRequest.DiseaseId != null && request.UpdateSensorDataRequest.DiseaseId != 0)
        {
            existingSensorData.DiseaseId = request.UpdateSensorDataRequest.DiseaseId;
        }

        existingSensorData.Status = 1;

        var updatedSensorDataLog = new Domain.Tables.SensorDataLog
        {
            SensorDataId = sensorDataLog.SensorDataId,
            Timestamp = DateTime.UtcNow
        };

        _context.SensorData.Update(existingSensorData);
        _context.SensorDataLogs.Add(updatedSensorDataLog);

        await _context.SaveChangesAsync(cancellationToken);
        await tran.CommitAsync(cancellationToken);

        return Result<SensorDataViewModel>.Success(new SensorDataViewModel
        {
            Id = existingSensorData.Id
        });
    }
    catch (Exception e)
    {
        _logger.LogError(e, $"Error while updating sensor data {request.UpdateSensorDataRequest.Id}");
        await tran.RollbackAsync(cancellationToken);
        return Result<SensorDataViewModel>.Failure($"Error while updating sensor data {request.UpdateSensorDataRequest.Id}: {e.Message}");
    }
}
    }
}