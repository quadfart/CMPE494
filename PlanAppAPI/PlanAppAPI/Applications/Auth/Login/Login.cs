using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;
using PlanAppAPI.Core;
using PlanAppAPI.DTOs;

namespace PlanAppAPI.Applications.Auth.Login;

public static class Login
{
    public class Command: IRequest<Result<AuthDto>>
    {
        public required LoginDto LoginDto { get; set; }
    }
    
    public class Handler : IRequestHandler<Command, Result<AuthDto>>
    {
        private readonly DataContext _context;

        public Handler(DataContext context)
        {
            _context = context;
        }

        public async Task<Result<AuthDto>> Handle(Command request, CancellationToken cancellationToken)
        {
            var user = await _context.Users.SingleOrDefaultAsync(x => x.Email == request.LoginDto.Email, cancellationToken: cancellationToken);
            if (user == null)
            {
                return Result<AuthDto>.Failure("User not found");
            }

            if (user.Password != request.LoginDto.Password)
            {
                return Result<AuthDto>.Failure("Invalid password");
            }

            var response = new AuthDto
            {
                Id = user.Id,
                Email = user.Email,
                Name = user.Name
            };
            return await Task.FromResult(Result<AuthDto>.Success(response));
        }
    }
}