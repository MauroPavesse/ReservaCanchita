using MercadoPago.Client.Payment;
using MercadoPago.Resource.Payment;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReservaCanchita.Data;
using ReservaCanchita.Shared;

namespace ReservaCanchita.Services.MercadoPago;

[ApiController]
[Route("api/mercadopago")]
public class MercadoPagoController : ControllerBase
{
    private readonly AppDbContext _context;

    public MercadoPagoController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost("notificaciones")]
    public async Task<IActionResult> Notificacion([FromBody] dynamic body)
    {
        try
        {
            string type = body.type;
            string id = body.data.id;

            if (type == "payment")
            {
                var client = new PaymentClient();
                Payment payment = await client.GetAsync(long.Parse(id));

                int reservaId = int.Parse(payment.ExternalReference);

                var reserva = await _context.Reservas.FindAsync(reservaId);
                if (reserva != null)
                {
                    switch (payment.Status)
                    {
                        case "approved":
                            reserva.Estado = (int)ReservaEstadosEnum.RESERVA_ESTADOS.RESERVADO;
                            break;
                        case "rejected":
                            reserva.Estado = (int)ReservaEstadosEnum.RESERVA_ESTADOS.DISPONIBLE;
                            break;
                        case "in_process":
                        case "pending":
                            reserva.Estado = (int)ReservaEstadosEnum.RESERVA_ESTADOS.EN_CONFIRMACION;
                            break;
                    }

                    var pagoMercadoPago = await _context.PagosMercadoPago.FirstAsync(x => x.ExternalReference == reservaId.ToString());
                    pagoMercadoPago.Status = payment.Status;
                    pagoMercadoPago.PaymentId = payment.Id.ToString() ?? "";
                    pagoMercadoPago.DateUpdated = payment.DateLastUpdated ?? DateTime.UtcNow;
                    pagoMercadoPago.TotalAmount = payment.TransactionAmount ?? 0m;

                    await _context.SaveChangesAsync();
                }
            }

            return Ok();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error en webhook MercadoPago: {ex.Message}");
            return BadRequest();
        }
    }
}
