using Domain.Tables;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;
using PlanAppAPI.Applications.SensorData.BaseDto;
using PlanAppAPI.Applications.SensorData.SensorDataById.Dtos;
using PlanAppAPI.Core;

namespace PlanAppAPI.Applications.SensorData.SensorDataById;

public class SensorDataGetById
{
    public class Command:  IRequest<Result<Domain.Tables.SensorData>>
    {
        public required SensorDataGetByIdRequestModel SensorData { get; set; }
    }
    
    public class Handler: IRequestHandler<Command, Result<Domain.Tables.SensorData>>
    {
        private readonly DataContext _context;

        public Handler(DataContext context)
        {
            _context = context;
        }

        public async Task<Result<Domain.Tables.SensorData>> Handle(Command request, CancellationToken cancellationToken)
        {
            try
            {
               var sensorData = await _context.SensorData
                   .Include(x => x.Plant)
                   .Include(x=>x.Diseases)
                   .SingleOrDefaultAsync(x => x.Id == request.SensorData.Id && x.Status == 1, cancellationToken: cancellationToken)
                   .ConfigureAwait(false);

               return await Task.FromResult(Result<Domain.Tables.SensorData>.Success(sensorData));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}