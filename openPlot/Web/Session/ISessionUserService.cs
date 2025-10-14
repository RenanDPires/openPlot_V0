using openPlot.Contracts.Responses;

namespace openPlot.Web.Session;

public interface ISessionUserService
{
    void SetCurrentUser(LoginResponse user);
    LoginResponse? GetCurrentUser();
    void Clear();
}
