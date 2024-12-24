using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;
using PlanAppAPI.Applications.Auth.BaseDto;
using PlanAppAPI.Core;

namespace PlanAppAPI.Applications.Auth.GetById;

public class GetById
{
    public class Command: IRequest<Result<UserViewModel>>
    {
        public required int Id { get; set; }
    }
    
    public class Handler: IRequestHandler<Command, Result<UserViewModel>>
    {
        private readonly DataContext _context;

        public Handler(DataContext context)
        {
            _context = context;
        }

        public async Task<Result<UserViewModel>> Handle(Command request, CancellationToken cancellationToken)
        {
            var user = await _context.Users
                .Include(x => x.SensorData!.Where(x => x.Status == 1))!
                .ThenInclude(x => x.Plant)
                .SingleOrDefaultAsync(x => x.Id == request.Id, cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            if (user == null) return new Result<UserViewModel>();
            var userViewModel = new UserViewModel
            {
                Id = user.Id,
                Name = user.Name,
                SensorData = user.SensorData
            };
                
            return await Task.FromResult(Result<UserViewModel>.Success(userViewModel));

        }
    }
}