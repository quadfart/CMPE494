using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;
using PlanAppAPI.Core;

namespace PlanAppAPI.Applications.SensorData.List;

public class ListSensorData
{
    public class Command:  IRequest<Result<List<Domain.Tables.SensorData>>>
    {
    }
    
    public class Handler: IRequestHandler<Command, Result<List<Domain.Tables.SensorData>>>
    {
        private readonly DataContext _context;

        public Handler(DataContext context)
        {
            _context = context;
        }

        public async Task<Result<List<Domain.Tables.SensorData>>> Handle(Command request, CancellationToken cancellationToken)
        {
            try
            {
                var sensorData = await _context.SensorData
                    .Where(x => x.Status == 1)
                    .Include(x => x.Plant)
                    .Include(x=>x.Diseases)
                    .ToListAsync(cancellationToken: cancellationToken)
                    .ConfigureAwait(false);

                return await Task.FromResult(Result<List<Domain.Tables.SensorData>>.Success(sensorData));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}