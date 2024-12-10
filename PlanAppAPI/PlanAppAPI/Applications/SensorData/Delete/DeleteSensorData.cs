using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;
using PlanAppAPI.Core;

namespace PlanAppAPI.Applications.SensorData.Delete;

public class DeleteSensorData
{
    public class Command:  IRequest<Result<int>>
    {
        public required int Id { get; set; }
    }
    
    public class Handler: IRequestHandler<Command, Result<int>>
    {
        private readonly DataContext _context;

        public Handler(DataContext context)
        {
            _context = context;
        }

        public async Task<Result<int>> Handle(Command request, CancellationToken cancellationToken)
        {
            var tran = await _context.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                var sensorData = await _context.SensorData
                    .AsNoTracking()
                    .SingleOrDefaultAsync(x => x.Id == request.Id && x.Status == 1, cancellationToken: cancellationToken)
                    .ConfigureAwait(false);
                
                if (sensorData == null)
                {
                    return Result<int>.Failure($"Sensor data not found with id {request.Id}");
                }
                
                sensorData.Status = 0;
                
                _context.SensorData.Update(sensorData);
                await _context.SaveChangesAsync(cancellationToken);
                await tran.CommitAsync(cancellationToken);
                return await Task.FromResult(Result<int>.Success(sensorData.Id));
            }
            catch (Exception e)
            {
                await tran.RollbackAsync(cancellationToken);
                return Result<int>.Failure("Error while deleting sensor data");
            }
        }
    }
}