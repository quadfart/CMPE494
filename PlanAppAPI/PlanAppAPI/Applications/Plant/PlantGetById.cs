using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;
using PlanAppAPI.Core;

namespace PlanAppAPI.Applications.Plant;

public class PlantGetById
{
    public class Command:  IRequest<Result<Domain.Tables.Plant>>
    {
        public required int Id { get; set; }
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
                    .SingleOrDefaultAsync(x => x.Id == request.Id, cancellationToken: cancellationToken)
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