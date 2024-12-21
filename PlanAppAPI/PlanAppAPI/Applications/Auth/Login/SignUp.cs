using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;
using PlanAppAPI.Core;
using PlanAppAPI.DTOs;
using Microsoft.Extensions.Logging;

namespace PlanAppAPI.Applications.Auth.SignUp
{
    public class SignUp
    {
        public class Command : IRequest<Result<string>>
        {
            public required SignUpDto SignUpDto { get; set; }
        }

        public class Handler : IRequestHandler<Command, Result<string>>
        {
            private readonly DataContext _context;
            private readonly ILogger<SignUp> _logger;

            public Handler(DataContext context, ILogger<SignUp> logger)
            {
                _context = context;
                _logger = logger;
            }

            public async Task<Result<string>> Handle(Command request, CancellationToken cancellationToken)
            {
                var tran = await _context.Database.BeginTransactionAsync(cancellationToken);
                try
                {
                    // Check if the user already exists by email or username
                    if (await _context.Users.AnyAsync(u => u.Email == request.SignUpDto.Email, cancellationToken))
                        return Result<string>.Failure("Email already exists");

                    if (await _context.Users.AnyAsync(u => u.Name == request.SignUpDto.Name, cancellationToken))
                        return Result<string>.Failure("Username already exists");

                    // Create the new user entity
                    var user = new Domain.Tables.User
                    {
                        Name = request.SignUpDto.Name,
                        Email = request.SignUpDto.Email,
                        Password = request.SignUpDto.Password // In a real-world scenario, ensure you hash the password!
                    };

                    // Add user to the database
                    _context.Users.Add(user);
                    await _context.SaveChangesAsync(cancellationToken);

                    await tran.CommitAsync(cancellationToken);

                    // Return success message
                    return Result<string>.Success("User created successfully");
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Error while creating user");
                    await tran.RollbackAsync(cancellationToken);
                    return Result<string>.Failure("Error while creating user");
                }
            }
        }
    }
}
