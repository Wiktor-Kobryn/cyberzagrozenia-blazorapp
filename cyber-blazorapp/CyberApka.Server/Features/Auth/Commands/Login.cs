using CyberApka.Server.Data.Database;
using CyberApka.Server.Data.Entities;
using CyberApka.Shared.Requests;
using CyberApka.Shared.Responses;
using CyberApka.Shared.Results;
using Konscious.Security.Cryptography;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace CyberApka.Server.Features.Auth.Commands;

public abstract class Login
{
    public record Command(LoginRequest Request) : IRequest<CyberApkaResult<LoginResponse>>;

    private const int SALT_BYTES = 16;
    private const int HASH_BYTES = 32;
    private const int DEGREE_OF_PARALLELISM = 2;
    private const int MEMORY_SIZE = 1 << 16;
    private const int ITERATIONS = 3;

    public class Handler(CyberDbContext context) : IRequestHandler<Command, CyberApkaResult<LoginResponse>>
    {
        private readonly CyberDbContext _context = context;

        public async Task<CyberApkaResult<LoginResponse>> Handle(Command command, CancellationToken ct)
        {
            try
            {
                var request = command.Request;

                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email == request.Email, ct);

                if (user == null)
                {
                    return CyberApkaResult<LoginResponse>.Failure("Incorrect email or password.");
                }

                var argon = new Argon2id(Encoding.UTF8.GetBytes(request.Password))
                {
                    Salt = user.Salt,
                    DegreeOfParallelism = DEGREE_OF_PARALLELISM,
                    MemorySize = MEMORY_SIZE,
                    Iterations = ITERATIONS
                };

                var computedHash = await argon.GetBytesAsync(HASH_BYTES);

                if(CryptographicOperations.FixedTimeEquals(computedHash, user.Hash) == false)
                {
                    return CyberApkaResult<LoginResponse>.Failure("Incorrect email or password");
                }

                var response = new LoginResponse()
                {
                    RefreshToken = "aaa",
                    AccessToken = "bbb",
                    ExpiresAt = DateTime.UtcNow.AddDays(1)
                };

               return CyberApkaResult<LoginResponse>.Success(response);
            }
            catch (Exception ex)
            {
                return CyberApkaResult<LoginResponse>.Failure(ex.Message);
            }
        }
    }
}
