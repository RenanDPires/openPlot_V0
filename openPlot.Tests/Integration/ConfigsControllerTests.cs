using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

using openPlot.Contracts.Requests; // LoginRequest

namespace openPlot.Tests.Integration;

public sealed class ConfigsControllerTests : IClassFixture<ConfigsWebAppFactory>
{
    private readonly ConfigsWebAppFactory _factory;

    public ConfigsControllerTests(ConfigsWebAppFactory factory)
    {
        _factory = factory;
        _factory.ClearTempXmlDir(); // zera a pasta temporária no início de cada teste
    }

    private HttpClient CreateClient() =>
        _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });

    private static async Task LoginAsync(HttpClient client, string user = "beatriz.dev", string pass = "Bea@1234")
    {
        var req = new LoginRequest { Username = user, Password = pass };
        var res = await client.PostAsJsonAsync("/api/v1/auth/login", req);
        res.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task manifest_and_meta_should_return_401_when_unauthenticated()
    {
        var client = CreateClient();

        var r1 = await client.GetAsync("/api/v1/configs/manifest");
        r1.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

        var r2 = await client.GetAsync("/api/v1/configs/manifest/meta");
        r2.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task manifest_and_meta_should_return_empty_when_no_xml()
    {
        var client = CreateClient();
        await LoginAsync(client);

        var manifest = await client.GetFromJsonAsync<string[]>("/api/v1/configs/manifest");
        manifest.Should().NotBeNull();
        manifest!.Should().BeEmpty();

        var metaRes = await client.GetAsync("/api/v1/configs/manifest/meta");
        metaRes.StatusCode.Should().Be(HttpStatusCode.OK);

        using var meta = JsonDocument.Parse(await metaRes.Content.ReadAsStringAsync());
        meta.RootElement.GetProperty("count").GetInt32().Should().Be(0);
        meta.RootElement.GetProperty("files").EnumerateArray().Should().BeEmpty();
    }

    [Fact]
    public async Task manifest_and_meta_should_list_only_xml_sorted()
    {
        // Arrange: cria alguns arquivos na pasta temporária usada pela API de teste
        _factory.SeedXmlFiles("B_file.xml", "A_file.xml", "ignore.txt");

        var client = CreateClient();
        await LoginAsync(client);

        // /manifest → array simples
        var list = await client.GetFromJsonAsync<string[]>("/api/v1/configs/manifest");
        list.Should().Equal("A_file.xml", "B_file.xml"); // ordenado e apenas .xml

        // /manifest/meta → objeto com count e files[]
        var metaRes = await client.GetAsync("/api/v1/configs/manifest/meta");
        metaRes.StatusCode.Should().Be(HttpStatusCode.OK);

        using var meta = JsonDocument.Parse(await metaRes.Content.ReadAsStringAsync());
        meta.RootElement.GetProperty("count").GetInt32().Should().Be(2);
        var files = meta.RootElement.GetProperty("files").EnumerateArray().Select(e => e.GetString()).ToArray();
        files.Should().Equal("A_file.xml", "B_file.xml");
    }
}
