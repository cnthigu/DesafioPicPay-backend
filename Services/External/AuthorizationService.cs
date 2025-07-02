using PicPayClone.Exceptions;
using PicPayClone.Models;
using System.Text.Json;

namespace PicPayClone.Services.External
{
    public class AuthorizationService : IAuthorizationService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<AuthorizationService> _logger;

        public AuthorizationService(HttpClient httpClient, ILogger<AuthorizationService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<bool> AuthorizeTransactionAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("https://util.devi.tools/api/v2/authorize");
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var authResult = JsonSerializer.Deserialize<AuthorizationResponse>(content,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                return authResult?.Status == "success";
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Erro ao chamar serviço de autorização");
                throw new AuthorizationServiceException("Serviço de autorização indisponível", ex);
            }
        }
    }
}