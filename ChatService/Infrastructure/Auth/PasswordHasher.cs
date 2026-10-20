using ChatService.Application.Abstractions.Auth;
using System.Security.Cryptography;
using System.Text;

namespace ChatService.Infrastructure.Auth;
internal sealed class PasswordHasher : IPasswordHasher
{
    private const int SaltSize = 32; // 256 bits for salt
    private const int KeySize = 32; // 256 bits for hash
    private const int Iterations = 100_000; // Adjust based on your system performance

    public string GenerateSalt()
    {
        byte[] salt = new byte[SaltSize];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(salt);
        return Convert.ToBase64String(salt);
    }

    public string Hash(string password, string salt)
    {
        byte[] hash = Rfc2898DeriveBytes.Pbkdf2(password, Convert.FromBase64String(salt), Iterations, HashAlgorithmName.SHA256, KeySize);

        var hashData = new PasswordHashData
        {
            Iterations = Iterations,
            Salt = salt,
            Hash = Convert.ToBase64String(hash)
        };

        return hashData.ToString();
    }

    public bool Verify(string enteredPassword, string storedPasswordHash)
    {
        var hashData = PasswordHashData.Parse(storedPasswordHash);

        byte[] enteredPasswordHash = Rfc2898DeriveBytes.Pbkdf2(enteredPassword, Convert.FromBase64String(hashData.Salt), hashData.Iterations, HashAlgorithmName.SHA256, KeySize);

        // Verify if the computed hash matches the stored hash
        return hashData.Hash == Convert.ToBase64String(enteredPasswordHash);
    }
}