using Grpc.Net.Client;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace gRPC.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GreetController : ControllerBase
    {
        private readonly ILogger<GreetController> _logger;

        public GreetController(ILogger<GreetController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public void Get()
        {
            // The port number(5001) must match the port of the gRPC server.
            using var channel = GrpcChannel.ForAddress("https://localhost:5001");
            var client = new Greeter.GreeterClient(channel);
            var reply = client.SayHello(new HelloRequest { Name = "<Your Name Here>" });
            _logger.LogInformation(reply.Message);
        }
    }
}
