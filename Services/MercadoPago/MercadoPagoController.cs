using MercadoPago.Client.Payment;
using MercadoPago.Resource.Payment;
using Microsoft.AspNetCore.Mvc;
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
            string type = body?.type;
            string id = body?.data?.id;

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
