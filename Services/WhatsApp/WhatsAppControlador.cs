using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace ReservaCanchita.Services.WhatsApp
{
    [ApiController]
    [Route("api/whatsapp/webhook")]
    public class WhatsAppControlador : ControllerBase
    {
        private readonly WhatsAppServicio _whatsapp;
        private readonly string? _verifyToken;

        public WhatsAppControlador(WhatsAppServicio whatsapp, IConfiguration config)
        {
            _whatsapp = whatsapp;
            _verifyToken = config["WhatsApp:VerifyToken"];
        }

        // GET para verificación
        [HttpGet]
        public IActionResult Verify(
            [FromQuery(Name = "hub.mode")] string mode,
            [FromQuery(Name = "hub.challenge")] string challenge,
            [FromQuery(Name = "hub.verify_token")] string token)
        {
            if (mode == "subscribe" && token == _verifyToken)
            {
                return Ok(challenge);
            }
            return Unauthorized();
        }

        // POST para recibir botones
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] JsonElement body)
        {
            var entries = body.GetProperty("entry");
            foreach (var entry in entries.EnumerateArray())
            {
                var changes = entry.GetProperty("changes");
                foreach (var change in changes.EnumerateArray())
                {
                    var value = change.GetProperty("value");
                    if (!value.TryGetProperty("messages", out var messages)) continue;

                    foreach (var message in messages.EnumerateArray())
                    {
                        var from = message.GetProperty("from").GetString();

                        // Caso 1: botones interactivos
                        if (message.TryGetProperty("interactive", out var interactive))
                        {
                            var buttonTitle = interactive.GetProperty("button_reply").GetProperty("title").GetString();
                            var buttonId = interactive.GetProperty("button_reply").GetProperty("id").GetString();

                            if (buttonTitle == "Confirmar")
                                await _whatsapp.ConfirmarReservaAsync(from);
                            else if (buttonTitle == "Cancelar")
                                await _whatsapp.CancelarReservaAsync(from);
                        }
                        // Caso 2: botones simples
                        else if (message.TryGetProperty("button", out var button))
                        {
                            var buttonText = button.GetProperty("text").GetString();
                            var buttonPayload = button.GetProperty("payload").GetString();

                            if (buttonText == "Confirmar")
                                await _whatsapp.ConfirmarReservaAsync(from);
                            else if (buttonText == "Cancelar")
                                await _whatsapp.CancelarReservaAsync(from);
                        }
                    }
                }
            }

            return Ok();
        }
    }
}
