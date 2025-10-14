using openPlot.Contracts.Requests;
using openPlot.Contracts.Responses;

namespace openPlot.Application.Auth;

public interface IAuthService
{
    Task<(bool ok, LoginResponse? resp, string? error)> AuthenticateAsync(LoginRequest request, CancellationToken ct);
}
