using AuxiliumSoftware.AuxiliumServices.Common.Configuration;
using AuxiliumSoftware.AuxiliumServices.Common.EntityFramework;
using AuxiliumSoftware.AuxiliumServices.Common.Services;
using Konscious.Security.Cryptography;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using System.Text;

namespace AuxiliumSoftware.AuxiliumServices.Common.Services.Implementations;

public class PasswordService : IPasswordService
{
    private readonly ConfigurationStructure _configuration;

    public PasswordService(
        IConfiguration configuration
        )
    {
        _configuration = new ConfigurationStructure();
        configuration.Bind(_configuration);
    }


    public string HashPassword(string password)
    {
        return HashPasswordArgon2(password);
    }

    public bool VerifyPassword(string password, string passwordHash)
    {
        if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(passwordHash))
        {
            return false;
        }

        if (passwordHash.StartsWith("$argon2", StringComparison.Ordinal))
        {
            return VerifyPasswordArgon2(password, passwordHash);
        }
        else if (passwordHash.StartsWith("$2a$") || passwordHash.StartsWith("$2b$") || passwordHash.StartsWith("$2y$"))
        {
            return VerifyPasswordBCrypt(password, passwordHash);
        }
        else
        {
            throw new ArgumentException("Only Argon2 (preferred) and BCrypt (deprecated) are supported");
        }
    }
    public string NormalisePassword(string? rawPassword, string? passwordSha512)
    {
        if (!string.IsNullOrEmpty(passwordSha512))
        {
            return passwordSha512;
        }

        if (!string.IsNullOrEmpty(rawPassword))
        {
            var bytes = SHA512.HashData(Encoding.UTF8.GetBytes(rawPassword));
            return Convert.ToHexString(bytes).ToLowerInvariant();
        }

        throw new ArgumentException("Either rawPassword or passwordSha512 must be provided");
    }

    private string HashPasswordArgon2(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(_configuration.Argon2.SaltLength);

        using var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password))
        {
            Salt = salt,
            DegreeOfParallelism = _configuration.Argon2.Parallelism,
            MemorySize = _configuration.Argon2.MemoryCost,
            Iterations = _configuration.Argon2.TimeCost
        };

        var hash = argon2.GetBytes(_configuration.Argon2.HashLength);

        var saltBase64 = Convert.ToBase64String(salt).TrimEnd('=');
        var hashBase64 = Convert.ToBase64String(hash).TrimEnd('=');

        return $"$argon2id$v=19$m={_configuration.Argon2.MemoryCost},t={_configuration.Argon2.TimeCost},p={_configuration.Argon2.Parallelism}${saltBase64}${hashBase64}";
    }

    private bool VerifyPasswordArgon2(string password, string passwordHash)
    {
        try
        {
            var parts = passwordHash.Split('$');

            if (parts.Length != 6)
            {
                return false;
            }

            var algorithm = parts[1];
            if (algorithm != "argon2id" && algorithm != "argon2i" && algorithm != "argon2d")
            {
                return false;
            }

            var paramParts = parts[3].Split(',');
            if (paramParts.Length != 3)
            {
                return false;
            }

            var memoryCost = int.Parse(paramParts[0].Split('=')[1]);
            var timeCost = int.Parse(paramParts[1].Split('=')[1]);
            var parallelism = int.Parse(paramParts[2].Split('=')[1]);

            var salt = Convert.FromBase64String(AddBase64Padding(parts[4]));
            var expectedHash = Convert.FromBase64String(AddBase64Padding(parts[5]));

            using var argon2 = algorithm switch
            {
                "argon2id" => (Argon2)new Argon2id(Encoding.UTF8.GetBytes(password)),
                "argon2i" => new Argon2i(Encoding.UTF8.GetBytes(password)),
                "argon2d" => new Argon2d(Encoding.UTF8.GetBytes(password)),
                _ => throw new ArgumentException($"Unknown Argon2 variant: {algorithm}")
            };

            argon2.Salt = salt;
            argon2.DegreeOfParallelism = parallelism;
            argon2.MemorySize = memoryCost;
            argon2.Iterations = timeCost;

            var computedHash = argon2.GetBytes(expectedHash.Length);

            return CryptographicOperations.FixedTimeEquals(expectedHash, computedHash);
        }
        catch
        {
            return false;
        }
    }

    private static string AddBase64Padding(string base64)
    {
        var padding = (4 - base64.Length % 4) % 4;
        return base64 + new string('=', padding);
    }

    private bool VerifyPasswordBCrypt(string password, string passwordHash)
    {
        try
        {
            return BCrypt.Net.BCrypt.Verify(password, passwordHash);
        }
        catch
        {
            return false;
        }
    }









    public (string rawToken, string tokenHash) GeneratePasswordSetToken()
    {
        var rawToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
        var tokenHash = HashToken(rawToken);
        return (rawToken, tokenHash);
    }

    public string HashToken(string rawToken)
    {
        return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(rawToken))).ToLowerInvariant();
    }
}
