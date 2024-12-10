using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;
using PlanAppAPI.Core;

namespace PlanAppAPI.Applications.Plant;

public class PlantGetByName
{
    public class Command:  IRequest<Result<Domain.Tables.Plant>>
    {
        public required string Name { get; set; }
    }
    
    public class Handler: IRequestHandler<Command, Result<Domain.Tables.Plant>>
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
                var plant = await _context.Plants
                    .Include(x => x.Diseases)
                    .SingleOrDefaultAsync(x => x.ScientificName == request.Name, cancellationToken: cancellationToken)
                    .ConfigureAwait(false);

                return await Task.FromResult(Result<Domain.Tables.Plant>.Success(plant));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}