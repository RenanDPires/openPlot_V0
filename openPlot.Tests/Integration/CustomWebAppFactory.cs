using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Moq;

using openPlot.Application.Abstractions;
using openPlot.Application.Commands;
using openPlot.Contracts.Responses;

namespace openPlot.Tests.Integration;

public class CustomWebAppFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // 1) Tenta achar o registro existente; se não houver, segue sem falhar
            var descriptor = services.FirstOrDefault(sd =>
                sd.ServiceType == typeof(ICommandHandler<SubmitSearchCommand, SubmitSearchResponse>));
            if (descriptor is not null)
                services.Remove(descriptor);

            // 2) Registra o mock do handler para os testes
            var mock = new Mock<ICommandHandler<SubmitSearchCommand, SubmitSearchResponse>>();
            mock.Setup(h => h.Handle(It.IsAny<SubmitSearchCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((SubmitSearchCommand c, CancellationToken _) => new SubmitSearchResponse
                {
                    RequestId = c.RequestId,
                    Username = c.Payload.Username,
                    ConfigVersion = c.Payload.ConfigVersion,
                    Terminais = c.Payload.Terminais,
                    Mode = c.Mode,
                    Agg = c.Agg,
                    SelectRate = c.SelectRate
                });

            services.AddScoped(_ => mock.Object);
        });
    }
}
