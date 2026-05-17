using Microsoft.AspNetCore.Identity;

namespace HoraCerta.Api.Autenticacao;

public static class SenhaHasher
{
    private static readonly PasswordHasher<object> Hasher = new();

    public static string Hash(string senha)
        => Hasher.HashPassword(new object(), senha);

    public static bool Verificar(string senha, string hash)
        => Hasher.VerifyHashedPassword(new object(), hash, senha) == PasswordVerificationResult.Success;
}
