using AccountingSystemService;
using AccountingSystemService.Helpers;
using AccountingSystemService.Interfaces;
using AccountingSystemService.Middleware;
using AccountingSystemService.Realization;
using AccountingSystemService.Services;
using AccountingSystemService.Validators;

using BdClasses;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

using NLog;

using System.Diagnostics;
using System.Text;

Trace.Listeners.Add(new NLogTraceListener());

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddGrpc(options =>
{
    options.MaxSendMessageSize = 100 * 1024 * 1024;
    options.MaxReceiveMessageSize = 100 * 1024 * 1024;
});

builder.Services.AddSingleton(provider => new CustomTokenValidator(AuthServiceHelper.JwtKey));

builder.Services.AddDbContext<ConstructionContext>();

builder.Services.AddTransient<IErrorHandler>(x=> new ErrorHandlerA());

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

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<AccountingService>();

app.MapGrpcService<AuthentificatingService>();

app.UseMiddleware<TokenValidationMiddleware>();

app.UseAuthentication();

app.UseAuthorization();

app.Run();
