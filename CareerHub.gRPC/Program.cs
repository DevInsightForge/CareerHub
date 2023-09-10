using CareerHub.gRPC.Services;
using CareerHub.gRPC;
using CareerHub.Infrastructure;
using Serilog;
using CareerHub.Application;

var builder = WebApplication.CreateBuilder(args);

// Add Serilog to container
builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

builder.Services.AddApplicationServices()
    .AddgRPCServices(builder.Configuration)
    .AddInfrastructureServices(builder.Configuration);

var app = builder.Build();

app.MapGrpcReflectionService()
    .AllowAnonymous();

app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client.")
    .AllowAnonymous();

app.MapGrpcService<GreeterService>();
app.MapGrpcService<AuthenticationService>();

app.Run();
