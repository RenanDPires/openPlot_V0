using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using openPlot.Contracts.Requests;
using openPlot.Contracts.Responses;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit;

namespace openPlot.Tests.Integration;

public class AuthControllerTests : IClassFixture<AuthWebAppFactory>
{
    private readonly AuthWebAppFactory _factory;

    public AuthControllerTests(AuthWebAppFactory factory)
    {
        _factory = factory;
    }

    private HttpClient CreateClientWithCookies()
    {
        // O TestServer do factory mantém cookies nesta instância de HttpClient
        return _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
    }


    [Fact]
    public async Task Login_success_then_me_returns_user()
    {
        var client = CreateClientWithCookies();

        var loginReq = new LoginRequest { Username = "beatriz.dev", Password = "Bea@1234" };
        var loginRes = await client.PostAsJsonAsync("/api/v1/auth/login", loginReq);
        loginRes.StatusCode.Should().Be(HttpStatusCode.OK);

        // mesma instância de client -> cookie de sessão vai junto
        var meRes = await client.GetAsync("/api/v1/auth/me");
        meRes.StatusCode.Should().Be(HttpStatusCode.OK);
    }


    [Fact]
    public async Task Login_fail_wrong_user_returns_401()
    {
        var client = CreateClientWithCookies();

        var badUserReq = new LoginRequest
        {
            Username = "usuario.inexistente",
            Password = "qualquer"
        };

        var res = await client.PostAsJsonAsync("/api/v1/auth/login", badUserReq);
        res.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Login_fail_wrong_password_returns_401()
    {
        var client = CreateClientWithCookies();

        var badPassReq = new LoginRequest
        {
            Username = "tatiana.dev",
            Password = "senha-errada"
        };

        var res = await client.PostAsJsonAsync("/api/v1/auth/login", badPassReq);
        res.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
