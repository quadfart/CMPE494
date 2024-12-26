using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;
using PlanAppAPI.Applications.SensorData.BaseDto;
using PlanAppAPI.Core;

namespace PlanAppAPI.Applications.SensorData.GetLogBySensorDataId;

public class GetLogById
{
    public class Command:  IRequest<Result<List<SensorDataLogViewModel>>>
    {
        public required int Id { get; set; }
    }
    
    public class Handler: IRequestHandler<Command, Result<List<SensorDataLogViewModel>>>
    {
        private readonly DataContext _context;

        public Handler(DataContext context)
        {
            _context = context;
        }

        public async Task<Result<List<SensorDataLogViewModel>>> Handle(Command request, CancellationToken cancellationToken)
        {
            try
            {
                var sensorDataLog = await _context.SensorDataLogs
                    .Where(x => x.SensorDataId == request.Id && x.SensorData.Status == 1)
                    .Include(x => x.SensorData)
                    .ThenInclude(x => x.Plant)
                    .Select(x => new SensorDataLogViewModel
                    {
                        Id = x.SensorDataId,
                        Temperature = x.Temperature,
                        SoilMoisture = x.SoilMoisture,
                        Moisture = x.Moisture,
                        Timestamp = x.Timestamp,
                    })
                    .ToListAsync(cancellationToken: cancellationToken)
                    .ConfigureAwait(false);
                

                return await Task.FromResult(Result<List<SensorDataLogViewModel>>.Success(sensorDataLog));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}