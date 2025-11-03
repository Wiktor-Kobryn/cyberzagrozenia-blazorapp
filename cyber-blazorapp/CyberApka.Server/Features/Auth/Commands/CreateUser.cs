using CyberApka.Server.Data.Database;
using CyberApka.Server.Data.Entities;
using CyberApka.Shared.Requests;
using CyberApka.Shared.Responses;
using CyberApka.Shared.Results;
using Konscious.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace CyberApka.Server.Features.Auth.Commands;

public abstract class CreateUser
{
    public sealed record Command(RegisterRequest Request);

    private const int SALT_BYTES = 16;
    private const int HASH_BYTES = 32;
    private const int DEGREE_OF_PARALLELISM = 2;
    private const int MEMORY_SIZE = 1 << 16;
    private const int ITERATIONS = 3;

    public class Handler
    {
        private readonly CyberDbContext _context;

        public Handler(CyberDbContext context)
        {
            _context = context;
        }

        public async Task<CyberApkaResult<RegisterResponse>> Handle(Command command, CancellationToken ct)
        {
            var request = command.Request;

            if (await _context.Users.AnyAsync(u => u.Email == request.Email, ct))
            {
                return new CyberApkaResult<RegisterResponse>
                {
                    Success = false,
                    ErrorMessage = "Email zajęty.",
                    Data = null
                };
            }

            var salt = RandomNumberGenerator.GetBytes(SALT_BYTES);              //tworzymy losowo salt 16 bajtów
            var argon = new Argon2id(Encoding.UTF8.GetBytes(request.Password))  //tutaj parametry hashu - jak długo trwa itd
            {
                Salt = salt,
                DegreeOfParallelism = DEGREE_OF_PARALLELISM, //ile wątków może użyć
                MemorySize = MEMORY_SIZE,                    //ile pamięci zajmie operacja - tutaj 65636 KB
                Iterations = ITERATIONS                      //ile razy wykona hasha
            };

            var hash = await argon.GetBytesAsync(HASH_BYTES);

            var user = new User
            {
                Username = request.Username,
                Email = request.Email,
                Salt = salt,
                Hash = hash,
                RoleId = 3
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync(ct);

            return new CyberApkaResult<RegisterResponse>
            {
                Success = true,
                ErrorMessage = string.Empty,
                Data = new RegisterResponse() { UserId = user.Id, Message = "Git" }
            };
        }
    }
}
