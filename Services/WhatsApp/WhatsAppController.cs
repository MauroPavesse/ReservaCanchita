using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace ReservaCanchita.Services.WhatsApp
{
    [ApiController]
    [Route("api/whatsapp/webhook")]
    public class WhatsAppController : ControllerBase
    {
        private readonly WhatsAppService _whatsapp;

        public WhatsAppController(WhatsAppService whatsapp)
        {
            _whatsapp = whatsapp;
        }

        // GET para verificación
        [HttpGet]
        public IActionResult Get([FromQuery] string hub_mode, [FromQuery] string hub_verify_token, [FromQuery] string hub_challenge)
        {
            if (hub_mode == "subscribe" && hub_verify_token == "MiTokenSecreto")
                return Content(hub_challenge, "text/plain");
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
                        if (message.TryGetProperty("interactive", out var interactive))
                        {
                            var buttonTitle = interactive.GetProperty("button_reply").GetProperty("title").GetString();
                            var buttonId = interactive.GetProperty("button_reply").GetProperty("id").GetString();

                            //if (buttonTitle == "Confirmar")
                                //await _whatsapp.ConfirmarReservaAsync(buttonId, from);
                            //else if (buttonTitle == "Cancelar")
                                //await _whatsapp.CancelarReservaAsync(buttonId, from);
                        }
                    }
                }
            }

            return Ok();
        }
    }

}
