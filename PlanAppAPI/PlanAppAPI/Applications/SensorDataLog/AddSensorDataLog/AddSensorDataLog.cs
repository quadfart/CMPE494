using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;
using PlanAppAPI.Core;
using PlanAppAPI.Applications.SensorDataLog.AddSensorDataLog.Dtos;

namespace PlanAppAPI.Applications.SensorDataLog.AddSensorDataLog
{
    public class Handler : IRequestHandler<Command, Result<SensorDataLogViewModel>>
    {
        private readonly DataContext _context;
        private readonly ILogger<Handler> _logger;

        public Handler(DataContext context, ILogger<Handler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Result<SensorDataLogViewModel>> Handle(Command request, CancellationToken cancellationToken)
        {
            using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

            try
            {
                // Step 1: Find SensorData by SensorSerialNumber
                var sensorData = await _context.SensorData
                    .FirstOrDefaultAsync(sd => sd.SensorSerialNumber == request.SensorSerialNumber, cancellationToken);

                if (sensorData == null)
                {
                    _logger.LogWarning("SensorData not found for serial number: {SerialNumber}", request.SensorSerialNumber);
                    return Result<SensorDataLogViewModel>.Failure("SensorData not found for the provided serial number.");
                }

                // Step 2: Create SensorDataLog
                var sensorDataLog = new Domain.Tables.SensorDataLog
                {
                    SensorDataId = sensorData.Id, // Link SensorDataId
                    Temperature = request.Temperature,
                    Moisture = request.Moisture,
                    SoilMoisture = request.SoilMoisture,
                    Timestamp = DateTime.UtcNow // Automatically set the timestamp
                };

                // Step 3: Add SensorDataLog to the database
                _context.SensorDataLogs.Add(sensorDataLog);

                // Step 4: Save changes and commit the transaction
                await _context.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);

                // Step 5: Map the result to ViewModel
                var viewModel = new SensorDataLogViewModel
                {
                    Id = sensorDataLog.Id,
                    SensorSerialNumber = sensorData.SensorSerialNumber, // Get SensorSerialNumber from SensorData
                    Temperature = sensorDataLog.Temperature,
                    Moisture = sensorDataLog.Moisture,
                    SoilMoisture = sensorDataLog.SoilMoisture,
                    Timestamp = sensorDataLog.Timestamp
                };

                return Result<SensorDataLogViewModel>.Success(viewModel); // Return the ViewModel
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while adding SensorDataLog");
                await transaction.RollbackAsync(cancellationToken);
                return Result<SensorDataLogViewModel>.Failure("An error occurred while adding the sensor data log.");
            }
        }
    }
}

namespace PlanAppAPI.Applications.SensorDataLog.AddSensorDataLog
{
    public class Command : IRequest<Result<SensorDataLogViewModel>>
    {
        public string SensorSerialNumber { get; set; } = default!;
        public int Temperature { get; set; }
        public int Moisture { get; set; }
        public int SoilMoisture { get; set; }
    }
}
