using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;
using PlanAppAPI.Core;

namespace PlanAppAPI.Applications.SensorData.SensorDataGetByUserId;

public class SensorDataGetByUserId
{
    public class Command : IRequest<Result<List<Domain.Tables.SensorData>>>
    {
        public required string UserId { get; set; }
    }

    public class Handler : IRequestHandler<Command, Result<List<Domain.Tables.SensorData>>>
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
                var userId = int.Parse(request.UserId); // Parse if UserId is passed as a string.
                var sensorDataList = await _context.SensorData
                    .Include(x => x.Plant)
                    .ThenInclude(x => x.Diseases)
                    .Where(x => x.UserId == userId && x.Status == 1)
                    .ToListAsync(cancellationToken);

                return Result<List<Domain.Tables.SensorData>>.Success(sensorDataList);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}