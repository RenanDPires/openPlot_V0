using openPlot.Infrastructure.Auth.Models;

namespace openPlot.Infrastructure.Auth;

public interface IUserStore
{
    Task<UserRecord?> FindByUsernameAsync(string username, CancellationToken ct);
}
