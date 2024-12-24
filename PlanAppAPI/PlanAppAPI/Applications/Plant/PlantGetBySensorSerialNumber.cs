using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;
using PlanAppAPI.Core;

namespace PlanAppAPI.Applications.Plant
{
    public class PlantGetBySensorSerialNumber
    {
        public class Command : IRequest<Result<Domain.Tables.Plant>>
        {
            public required string SensorSerialNumber { get; set; }
        }

        public class Handler : IRequestHandler<Command, Result<Domain.Tables.Plant>>
        {
            private readonly DataContext _context;

            public Handler(DataContext context)
            {
                _context = context;
            }

            public async Task<Result<Domain.Tables.Plant>> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    // Query to find the plant by SensorSerialNumber through the SensorData table
                    var plant = await _context.SensorData
                        .Where(x => x.SensorSerialNumber == request.SensorSerialNumber)
                        .Include(x => x.Plant) // Include the Plant table to get plant details
                        .Select(x => x.Plant) // Only select the Plant related to the SensorData
                        .FirstOrDefaultAsync(cancellationToken)
                        .ConfigureAwait(false);

                    // Check if plant is found
                    if (plant == null)
                    {
                        return Result<Domain.Tables.Plant>.Failure("Plant not found for the given SensorSerialNumber.");
                    }

                    // Return success with the plant data
                    return Result<Domain.Tables.Plant>.Success(plant);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    return Result<Domain.Tables.Plant>.Failure("An error occurred while retrieving the plant.");
                }
            }
        }
    }
}
