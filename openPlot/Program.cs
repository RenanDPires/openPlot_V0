using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
// ===== namespaces das camadas =====
using openPlot.Application.Config;
using openPlot.BuildingBlocks.Extensions;
using openPlot.Application.Auth;
using openPlot.BuildingBlocks.Options;
using openPlot.Infrastructure.Auth;
using openPlot.Web.Session;

var builder = WebApplication.CreateBuilder(args);

// -------------------- Services --------------------
builder.Services.AddControllers()
    .AddJsonOptions(o =>
    {
        o.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
        o.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
    });
builder.Services.AddAppOptions(builder.Configuration);


// API Versioning (exige os pacotes Microsoft.AspNetCore.Mvc.Versioning* instalados)
builder.Services.AddApiVersioning(options =>
{
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.ReportApiVersions = true;
});
builder.Services.AddVersionedApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Session + Auth Mock
builder.Services.Configure<SessionOptionsEx>(builder.Configuration.GetSection("Session"));
builder.Services.Configure<AuthOptions>(builder.Configuration.GetSection("Auth"));

// Retorno de arquivos de configuração / necessidade de autenticacao
builder.Services.Configure<ConfigOptions>(builder.Configuration.GetSection("Configs"));
builder.Services.AddScoped<IConfigManifestService, ConfigManifestService>();


builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(o =>
{
    var session = builder.Configuration.GetSection("Session").Get<SessionOptionsEx>() ?? new SessionOptionsEx();
    o.IdleTimeout = TimeSpan.FromMinutes(session.IdleTimeoutMinutes);
    o.Cookie.Name = session.CookieName;
    o.Cookie.HttpOnly = true;
    o.Cookie.IsEssential = true;
    o.Cookie.SameSite = session.CookieSameSite.Equals("None", StringComparison.OrdinalIgnoreCase)
        ? SameSiteMode.None : SameSiteMode.Lax;
    o.Cookie.SecurePolicy = session.CookieSecurePolicy.Equals("Always", StringComparison.OrdinalIgnoreCase)
        ? CookieSecurePolicy.Always : CookieSecurePolicy.None;
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ISessionUserService, SessionUserService>();
builder.Services.AddScoped<IUserStore, JsonUserStore>();
builder.Services.AddScoped<IAuthService, AuthService>();

var app = builder.Build();

// -------------------- Pipeline --------------------
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
        foreach (var d in provider.ApiVersionDescriptions)
            options.SwaggerEndpoint($"/swagger/{d.GroupName}/swagger.json", d.GroupName.ToUpperInvariant());
    });
}

app.UseHttpsRedirection();

app.UseRouting();
app.UseSession();         // << necessário para persistir usuário logado
app.UseAuthorization();
app.MapControllers();

app.Run();

public partial class Program { }
