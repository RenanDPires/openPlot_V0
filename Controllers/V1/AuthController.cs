using Microsoft.AspNetCore.Mvc;
using openPlot.Application.Auth;
using openPlot.Contracts.Requests;
using openPlot.Contracts.Responses;
using openPlot.Middleware;
using openPlot.Web.Session;

namespace openPlot.Controllers.V1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/auth")]
public sealed class AuthController : ControllerBase
{
    private readonly IAuthService _auth;
    private readonly ISessionUserService _session;

    public AuthController(IAuthService auth, ISessionUserService session)
    {
        _auth = auth;
        _session = session;
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginRequest req, CancellationToken ct)
    {
        var (ok, resp, error) = await _auth.AuthenticateAsync(req, ct);
        if (!ok || resp is null)
            return Problem(statusCode: 401, title: "Falha de autenticação", detail: error);

        _session.SetCurrentUser(resp);
        return Ok(resp); // devolvemos o perfil + sessionId
    }

    [HttpPost("logout")]
    [RequireUser] // OBRIGATÓRIO para bloquear sem sessão
    public IActionResult Logout()
    {
        var user = _session.GetCurrentUser();
        if (user is null)
            return Unauthorized(new { error = "Sessão expirada ou inexistente." });

        _session.Clear();
        return Ok(new { message = "Sessão encerrada" });
    }


    // Exemplo de endpoint protegido por sessão (pré-Keycloak)
    [HttpGet("me")]
    [RequireUser]
    public IActionResult Me()
    {
        var user = _session.GetCurrentUser();
        return Ok(user);
    }
}
