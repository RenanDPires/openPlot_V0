using openPlot.Contracts.Requests;
using openPlot.Contracts.Responses;
using openPlot.Infrastructure.Auth;

namespace openPlot.Application.Auth;

public sealed class AuthService : IAuthService
{
    private readonly IUserStore _store;

    public AuthService(IUserStore store) => _store = store;

    public async Task<(bool ok, LoginResponse? resp, string? error)> AuthenticateAsync(LoginRequest request, CancellationToken ct)
    {
        var user = await _store.FindByUsernameAsync(request.Username, ct);
        if (user is null) return (false, null, "Usuário não encontrado.");

        // DEMO: comparação direta. Em produção: Hash + Timing-safe compare
        if (!string.Equals(user.password, request.Password, StringComparison.Ordinal))
            return (false, null, "Senha inválida.");

        var sessionId = Guid.NewGuid().ToString("N");

        var resp = new LoginResponse
        {
            Sub = user.sub,
            Username = user.username,
            Roles = user.roles,
            Email = user.email,
            Claims = user.claims,
            SessionId = sessionId
        };

        return (true, resp, null);
    }
}
