using MediatR;
using Persistence;
using PlanAppAPI.Applications.SensorData.AddSensorData.Dtos;
using PlanAppAPI.Applications.SensorData.BaseDto;
using PlanAppAPI.Core;
using PlanAppAPI.DTOs;

namespace PlanAppAPI.Applications.SensorData.AddSensorData;

public class AddSensorData
{
    public class Command : IRequest<Result<SensorDataViewModel>>
    {
        public required AddSensorDataRequestModel AddSensorDataRequest { get; set; }
    }

    public class Handler : IRequestHandler<Command, Result<SensorDataViewModel>>
    {
        private readonly DataContext _context;
        private readonly ILogger<AddSensorData> _logger;

        public Handler(DataContext context, ILogger<AddSensorData> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Result<SensorDataViewModel>> Handle(Command request, CancellationToken cancellationToken)
        {
            var tran = await _context.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                // Validate if the user exists
                var user = await _context.Users
                    .FindAsync(new object[] { request.AddSensorDataRequest.UserId }, cancellationToken);

                if (user == null)
                {
                    return Result<SensorDataViewModel>.Failure("User not found");
                }

                // Create the SensorData record
                var sensorData = new Domain.Tables.SensorData
                {
                    Status = 1,
                    UserId = request.AddSensorDataRequest.UserId,
                    SensorSerialNumber = request.AddSensorDataRequest.SensorSerialNumber
                };
                _context.SensorData.Add(sensorData);
                await _context.SaveChangesAsync(cancellationToken);

                // Create the SensorDataLog record
                var sensorDataLog = new Domain.Tables.SensorDataLog
                {
                    Temperature = request.AddSensorDataRequest.Temperature,
                    Moisture = request.AddSensorDataRequest.Moisture,
                    Timestamp = DateTime.UtcNow,
                    SensorDataId = sensorData.Id,
                };

                _context.SensorDataLogs.Add(sensorDataLog);
                await _context.SaveChangesAsync(cancellationToken);

                // Commit the transaction
                await tran.CommitAsync(cancellationToken);

                return await Task.FromResult(Result<SensorDataViewModel>.Success(new SensorDataViewModel
                {
                    Id = sensorData.Id
                }));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while adding sensor data");
                await tran.RollbackAsync(cancellationToken);
                return Result<SensorDataViewModel>.Failure("Error while adding sensor data");
            }
        }
    }
}
