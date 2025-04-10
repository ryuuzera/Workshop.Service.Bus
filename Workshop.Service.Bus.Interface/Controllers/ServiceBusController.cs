using Azure.Messaging.ServiceBus;
using Microsoft.AspNetCore.Mvc;

namespace Workshop.Service.Bus.Interface.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ServiceBusController : ControllerBase
    {
        private readonly ILogger<ServiceBusController> _logger;
        private readonly IConfiguration _configuration;

        public ServiceBusController(ILogger<ServiceBusController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        [HttpPost("Send")]
        public async Task<IActionResult> SendMessageAsync([FromBody] string messageContent)
        {
            try
            {
                string connectionString = _configuration["ServiceBus:ConnectionString"];
                string queueName = _configuration["ServiceBus:QueueName"];

                await using var client = new ServiceBusClient(connectionString);
                ServiceBusSender sender = client.CreateSender(queueName);

                var message = new ServiceBusMessage(messageContent);
                await sender.SendMessageAsync(message);

                _logger.LogInformation("Mensagem enviada com sucesso: {Message}", messageContent);
                return Ok(new { status = "Mensagem enviada com sucesso" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao enviar mensagem para o Service Bus");
                return StatusCode(500, "Erro ao enviar mensagem para o Service Bus");
            }
        }
    }
}
