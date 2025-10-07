using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;
using Microsoft.EntityFrameworkCore;
using ReservaCanchita.Data;

namespace ReservaCanchita.Services.WhatsApp
{
    public class WhatsAppServicio
    {
        private readonly HttpClient _httpClient;
        private readonly string _phoneNumberId;
        private readonly string _accessToken;
        private readonly AppDbContext _context;

        public WhatsAppServicio(HttpClient httpClient, IConfiguration config, AppDbContext context)
        {
            _httpClient = httpClient;
            _phoneNumberId = config["WhatsApp:PhoneNumberId"]!;
            _accessToken = config["WhatsApp:AccessToken"]!;
            _context = context;
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
                to = to,
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

        public async Task ConfirmarReservaAsync(string? from)
        {
            var reserva = await _context.Reservas.FirstAsync(x => x.Estado == 2 && x.Telefono == from && x.Fecha.Date == DateTime.Now.Date);
            reserva.Estado = 1;
            _context.Reservas.Update(reserva);
            await _context.SaveChangesAsync();
            await SendMessageAsync(from!, "✅ Tu reserva fue confirmada.");
        }

        public async Task CancelarReservaAsync(string? from)
        {
            var reserva = await _context.Reservas.FirstOrDefaultAsync(x => x.Estado == 2 && x.Telefono == from && x.Fecha.Date == DateTime.Now.Date);

            if (reserva != null)
            {
                reserva.Estado = 3;
                _context.Reservas.Update(reserva);
                await _context.SaveChangesAsync();
                await SendMessageAsync(from!, "❌ Tu reserva fue cancelada.");
            }
        }
    }
}
