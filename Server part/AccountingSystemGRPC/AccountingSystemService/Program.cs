using AccountingSystemService;
using AccountingSystemService.DataCollections;
using AccountingSystemService.Helpers;
using AccountingSystemService.Interfaces;
using AccountingSystemService.Middleware;
using AccountingSystemService.Realization;
using AccountingSystemService.Services;
using AccountingSystemService.Validators;

using BdClasses;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

using NLog;

using System.Diagnostics;
using System.Net;
using System.Text;

try
{
    Trace.Listeners.Add(new NLogTraceListener());

    Trace.TraceInformation("========================= Ќачат запуск службы =========================");

    var builder = WebApplication.CreateBuilder(args);

    // Add services to the container.
    builder.Services.AddGrpc(options =>
    {
        options.MaxSendMessageSize = 100 * 1024 * 1024;
        options.MaxReceiveMessageSize = 100 * 1024 * 1024;
    });

    builder.Services.AddSingleton(provider => new CustomTokenValidator(AuthServiceHelper.JwtKey));

    builder.WebHost.ConfigureKestrel(options =>
    {
        // явно слушаем IPv4 на порту 5001 (gRPC)
        options.Listen(IPAddress.Any, 5001, configure => configure.Protocols = HttpProtocols.Http2);

        // явно слушаем IPv4 на порту 5002 (Web API)
        options.Listen(IPAddress.Any, 5002);
    });

    builder.Services.AddDbContext<ConstructionContext>(DbContextHelper.ProcessOptionsBuilder);

    builder.Services.AddTransient<IErrorHandler>(x => new ErrorHandlerA());

    builder.Services.AddSingleton<UsersCollection>();

    builder.Services.AddSingleton<ObjectCollection>();

    var validationParameters = new TokenValidationParameters()
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(AuthServiceHelper.JwtKey)),
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = false,
    };

    builder.Services.AddAuthentication(x =>
    {
        x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    }).AddJwtBearer(o =>
    {
        o.RequireHttpsMetadata = false;
        o.SaveToken = true;
        o.TokenValidationParameters = validationParameters;
    });

    builder.Services.AddAuthorization();

    Trace.TraceInformation("========================= «арегистрированы все сервисы =========================");

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    app.MapGrpcService<AccountingService>();

    app.MapGrpcService<AuthentificatingService>();

    app.UseMiddleware<TokenValidationMiddleware>();

    app.UseAuthentication();

    app.UseAuthorization();

    app.Run();
}
catch (Exception e)
{
    Trace.TraceError($"ќшибка при запуске службы -> {e.Message}");
}

