using CareerHub.gRPC.Protos;
using Grpc.Core;
using System.Security.Claims;

namespace CareerHub.gRPC.Services
{
    public class GreeterService : Greeter.GreeterBase
    {
        private readonly ILogger<GreeterService> _logger;
        public GreeterService(ILogger<GreeterService> logger)
        {
            _logger = logger;
        }

        public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
        {
            var userEmail = context.GetHttpContext().User.FindFirstValue(ClaimTypes.Email);

            return Task.FromResult(new HelloReply
            {
                Message = $"Hello {(string.IsNullOrEmpty(request.Name) ? userEmail : request.Name)}"
            });
        }
    }
}