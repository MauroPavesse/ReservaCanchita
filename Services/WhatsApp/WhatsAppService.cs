using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;

namespace ReservaCanchita.Services.WhatsApp
{
    public class WhatsAppService
    {
        private readonly HttpClient _httpClient;
        private readonly string _phoneNumberId;
        private readonly string _accessToken;

        public WhatsAppService(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _phoneNumberId = config["WhatsApp:PhoneNumberId"]!;
            _accessToken = config["WhatsApp:AccessToken"]!;
        }

        public async Task<bool> SendMessageAsync(string to, string message)
        {
            var url = $"https://graph.facebook.com/v20.0/{_phoneNumberId}/messages";

            var payload = new
            {
                messaging_product = "whatsapp",
                to = to,
                type = "text",
                text = new { body = message }
            };

            var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
            request.Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

            var response = await _httpClient.SendAsync(request);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> SendReservaTemplateAsync(string to, string nombre, string fecha)
        {
            var url = $"https://graph.facebook.com/v20.0/{_phoneNumberId}/messages";

            var payload = new
            {
                messaging_product = "whatsapp",
                to = to, // Número completo ej: "5493511234567"
                type = "template",
                template = new
                {
                    name = "confirmar_reserva_cancha",
                    language = new { code = "es_AR" },
                    components = new object[]
                    {
                        new {
                            type = "body",
                            parameters = new object[]
                            {
                                new { type = "text", text = nombre },
                                new { type = "text", text = fecha }
                            }
                        }
                    }
                }
            };

            var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Headers.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _accessToken);
            request.Content = new StringContent(
                System.Text.Json.JsonSerializer.Serialize(payload),
                Encoding.UTF8,
                "application/json");

            var response = await _httpClient.SendAsync(request);
            return response.IsSuccessStatusCode;
        }

        public async Task ConfirmarReservaAsync()
        {

        }

        public async Task CancelarReservaAsync()
        {

        }
    }
}
