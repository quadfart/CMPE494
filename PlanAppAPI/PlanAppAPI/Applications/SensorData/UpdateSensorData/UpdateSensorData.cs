using Domain.Tables;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;
using PlanAppAPI.Applications.SensorData.BaseDto;
using PlanAppAPI.Applications.SensorData.UpdateSensorData.Dtos;
using PlanAppAPI.Core;

namespace PlanAppAPI.Applications.SensorData.UpdateSensorData;

public class UpdateSensorData
{
    public class Command:  IRequest<Result<UpdateSensorDataResponseModel>>
    {
        public required UpdateSensorDataRequestModel UpdateSensorDataRequest { get; set; }
    }

    public class Handler : IRequestHandler<Command, Result<UpdateSensorDataResponseModel>>
    {
        private readonly DataContext _context;
        private readonly ILogger<UpdateSensorData> _logger;

        public Handler(DataContext context, ILogger<UpdateSensorData> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Result<UpdateSensorDataResponseModel>> Handle(Command request, CancellationToken cancellationToken)
{
    var tran = await _context.Database.BeginTransactionAsync(cancellationToken);
    try
    {
        var existingSensorData = await _context.SensorData
            .FirstOrDefaultAsync(x=> x.Id ==  request.UpdateSensorDataRequest.Id ,cancellationToken);

        if (existingSensorData == null)
        {
            throw new Exception("Sensor data not found");
        }

        // Update fields
        if (request.UpdateSensorDataRequest.Id != 0)
        {
            existingSensorData.Id = request.UpdateSensorDataRequest.Id;
        }
        if (request.UpdateSensorDataRequest.PlantId != null && request.UpdateSensorDataRequest.PlantId != 0)
        {
            existingSensorData.PlantId = request.UpdateSensorDataRequest.PlantId;
        }

        if (request.UpdateSensorDataRequest.DiseaseId != null && request.UpdateSensorDataRequest.DiseaseId != 0)
        {
            existingSensorData.DiseaseId = request.UpdateSensorDataRequest.DiseaseId;
        }
        
        _context.SensorData.Update(existingSensorData);

        await _context.SaveChangesAsync(cancellationToken);
        await tran.CommitAsync(cancellationToken);
        
        
        return Result<UpdateSensorDataResponseModel>.Success(new UpdateSensorDataResponseModel()
        {
            Id = existingSensorData.Id,
            SensorSerialNumber = existingSensorData.SensorSerialNumber,
            PlantId = existingSensorData.PlantId,
            UserId = existingSensorData.UserId,
        });
    }
    catch (Exception e)
    {
        _logger.LogError(e, $"Error while updating sensor data {request.UpdateSensorDataRequest.Id}");
        await tran.RollbackAsync(cancellationToken);
        return Result<UpdateSensorDataResponseModel>.Failure($"Error while updating sensor data {request.UpdateSensorDataRequest.Id}: {e.Message}");
    }
}
    }
}