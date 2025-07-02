using PicPayClone.Models;
using System.Text.Json;

namespace PicPayClone.Services.External
{
    public class NotificationService : INotificationService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(HttpClient httpClient, ILogger<NotificationService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task SendNotificationAsync(User recipient, User sender, decimal amount)
        {
            try
            {
                var notificationData = new
                {
                    email = recipient.Email,
                    message = $"Você recebeu uma transferência de R${amount:F2} de {sender.FullName}."
                };

                var content = new StringContent(
                    JsonSerializer.Serialize(notificationData),
                    System.Text.Encoding.UTF8,
                    "application/json");

                var response = await _httpClient.PostAsync("https://util.devi.tools/api/v1/notify", content);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Falha ao enviar notificação para {Email}. Status: {StatusCode}",
                        recipient.Email, response.StatusCode);
                }
                else
                {
                    _logger.LogInformation("Notificação enviada com sucesso para {Email}", recipient.Email);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao enviar notificação para {Email}", recipient.Email);
            }
        }
    }
}