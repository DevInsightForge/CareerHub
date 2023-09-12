using CareerHub.gRPC.Services;
using CareerHub.gRPC;
using CareerHub.Infrastructure;
using Serilog;
using CareerHub.Application;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// Add Serilog to container
builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

builder.Services.AddApplicationServices(configuration)
    .AddgRPCServices(configuration)
    .AddInfrastructureServices(configuration);

var app = builder.Build();

app.MapGrpcReflectionService()
    .AllowAnonymous();

app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client.")
    .AllowAnonymous();

app.MapGrpcService<GreeterService>();
app.MapGrpcService<AuthenticationService>();

app.Run();
