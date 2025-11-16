using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ReservaCanchita.Data;
using ReservaCanchita.Models;
using ReservaCanchita.Services.MercadoPago;
using ReservaCanchita.Services.WhatsApp;
using ReservaCanchita.Shared;

namespace ReservaCanchita.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly AppDbContext _context;
        private readonly WhatsAppServicio _whatsapp;
        private readonly MercadoPagoServicio _mpService;

        public IndexModel(ILogger<IndexModel> logger, AppDbContext context, WhatsAppServicio whatsapp, MercadoPagoServicio mpService)
        {
            _logger = logger;
            _context = context;
            _whatsapp = whatsapp;
            _mpService = mpService;
        }

        public List<HorarioDisponible> HorariosDisponibles { get; set; } = new List<HorarioDisponible>();
        public bool MuestraValorReserva { get; set; } = false;
        public string ValorReserva { get; set; } = string.Empty;
        [TempData]
        public string MensajeError { get; set; } = string.Empty;
        [TempData]
        public string MensajeSuccess { get; set; } = string.Empty;
        public string? InitPoint { get; set; }

        public async Task OnGetAsync()
        {
            var _ = MensajeError;
            var __ = MensajeSuccess;

            HorariosDisponibles = await _context.HorariosDisponibles
                .Where(x => x.Fecha.Date >= DateTime.Now.Date)
                .ToListAsync();

            var configuraciones = await _context.Configuraciones.Where(x => x.Campo == "MuestraValorReserva" || x.Campo == "ValorReserva").ToArrayAsync();
            MuestraValorReserva = configuraciones.First(x => x.Campo == "MuestraValorReserva").ValorNumerico == 1;
            ValorReserva = configuraciones.First(x => x.Campo == "ValorReserva").ValorNumerico.ToString("c");
        }

        private string NormalizarTelefono(string telefonoIngresado)
        {
            var limpio = new string(telefonoIngresado.Where(char.IsDigit).ToArray());

            if (string.IsNullOrEmpty(limpio))
                return string.Empty;

            if (limpio.StartsWith("549"))
            {
                return limpio;
            }

            if (limpio.StartsWith("54"))
            {
                return "549" + limpio.Substring(2);
            }

            return "549" + limpio;
        }

        public async Task<IActionResult> OnPostReservarCanchaAsync(int HorarioDisponibleId, string NombreCliente, string Telefono)
        {
            var horarioDisponible = await _context.HorariosDisponibles.Where(x => x.Id == HorarioDisponibleId).FirstAsync();

            if (horarioDisponible.EstaReservado)
            {
                return RedirectToPage();
            }

            if (!Telefono.StartsWith("22"))
            {
                MensajeError = "El numero de telefono ingresado no tiene el formato correcto.";
                return RedirectToPage();
            }

            Telefono = NormalizarTelefono(Telefono);

            var configuraciones = await _context.Configuraciones.Where(x => x.Campo == "TrabajaConWhatsapp").ToArrayAsync();
            var trabajaConWhatsApp = configuraciones.First(x => x.Campo == "TrabajaConWhatsapp").ValorNumerico == 1;
            var trabajaConSena = true;
            var montoSena = 1000;

            if (trabajaConWhatsApp)
            {
                string fecha = $"{horarioDisponible.Fecha:dd:MM:yy}{horarioDisponible.HoraInicio}";
                var enviado = await _whatsapp.SendReservaTemplateAsync(Telefono, NombreCliente, fecha);

                if (enviado)
                    MensajeSuccess = "Se te enviará un WhatsApp para que confirmes la reserva.";
                else
                    MensajeError = "No se pudo completar la reserva porque el numero ingresado no es correcto.";
            }
            else
            {
                if (trabajaConSena)
                {
                    var reservaActual = _context.Reservas.Add(new Reserva()
                    {
                        NombreCliente = NombreCliente,
                        Telefono = Telefono,
                        Fecha = DateTime.Now,
                        Estado = (int)ReservaEstadosEnum.RESERVA_ESTADOS.EN_CONFIRMACION,
                        HorarioDisponible = horarioDisponible
                    });

                    horarioDisponible.EstaReservado = true;

                    await _context.SaveChangesAsync();

                    var successUrl = Url.Page(
                        "/MercadoPago/Exito",
                        null,
                        null,
                        Request.Scheme,
                        Request.Host.ToUriComponent()
                    );
                    var failureUrl = Url.Page(
                        "/MercadoPago/Error",
                        null,
                        null,
                        Request.Scheme,
                        Request.Host.ToUriComponent()
                    );

                    var pendingUrl = Url.Page(
                        "/MercadoPago/Pendiente",
                        null,
                        null,
                        Request.Scheme,
                        Request.Host.ToUriComponent()
                    );

                    var notificationUrl = Url.Page(
                        "/api/mercadopago/notificaciones",
                        null,
                        null,
                        Request.Scheme,
                        Request.Host.ToUriComponent()
                    );

                    var response = await _mpService.CrearLinkPagoAsync(successUrl, failureUrl, pendingUrl, notificationUrl, montoSena, reservaActual.Entity.Id, Request);

                    if (response.Item1 && response.Item2 != null)
                    {
                        return Redirect(response.Item2);
                    }
                    else
                    {
                        MensajeError = response.Item2!;
                    }
                }
                else
                {
                    _context.Reservas.Add(new Reserva()
                    {
                        NombreCliente = NombreCliente,
                        Telefono = Telefono,
                        Fecha = DateTime.Now,
                        Estado = (int)ReservaEstadosEnum.RESERVA_ESTADOS.RESERVADO,
                        HorarioDisponible = horarioDisponible
                    });

                    horarioDisponible.EstaReservado = true;

                    await _context.SaveChangesAsync();

                    MensajeSuccess = "Reserva confirmada correctamente.";
                }
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostEliminarReservaAsync(int HorarioDisponibleId, string Telefono)
        {
            var horarioDisponible = await _context.HorariosDisponibles.Where(x => x.Id == HorarioDisponibleId).FirstAsync();

            if (!horarioDisponible.EstaReservado)
            {
                return RedirectToPage();
            }

            if (!Telefono.StartsWith("22"))
            {
                MensajeError = "El numero de telefono ingresado no tiene el formato correcto.";
                return RedirectToPage();
            }

            Telefono = NormalizarTelefono(Telefono);

            var configuraciones = await _context.Configuraciones.Where(x => x.Campo == "TrabajaConToleranciaCancelarReserva" || x.Campo == "HoraToleranciaCancelarReserva" || x.Campo == "NumeroContacto").ToListAsync();
            var trabajaConToleranciaCancelarReserva = configuraciones.FirstOrDefault(x => x.Campo == "TrabajaConToleranciaCancelarReserva");
            if(trabajaConToleranciaCancelarReserva != null && trabajaConToleranciaCancelarReserva.ValorNumerico == 1)
            {
                DateTime horaInicioReserva = new DateTime(horarioDisponible.Fecha.Year, horarioDisponible.Fecha.Month, horarioDisponible.Fecha.Day, horarioDisponible.HoraInicio.Hours, horarioDisponible.HoraInicio.Minutes, horarioDisponible.HoraInicio.Seconds);
                DateTime ahoraMismo = DateTime.Now;
                var toleranciaConfig = configuraciones.First(x => x.Campo == "HoraToleranciaCancelarReserva");
                TimeSpan tolerancia = TimeSpan.FromHours(Convert.ToDouble(toleranciaConfig.ValorNumerico));
                TimeSpan diferencia = horaInicioReserva - ahoraMismo;

                if (diferencia.TotalHours <= tolerancia.TotalHours && diferencia.TotalSeconds > 0)
                {
                    var contactoConfig = configuraciones.First(x => x.Campo == "NumeroContacto");
                    MensajeError = $"No puede cancelar la reserva, debe cancelar {Convert.ToInt32(toleranciaConfig.ValorNumerico)} hora/s antes. Comuniquese al: {contactoConfig.ValorString}";
                    return RedirectToPage();
                }
            }

            var reserva = await _context.Reservas.Where(x => x.HorarioDisponibleId == HorarioDisponibleId).FirstOrDefaultAsync();

            if(reserva == null)
            {
                return RedirectToPage();
            }

            if(reserva.Telefono != Telefono)
            {
                MensajeError = $"El telefono de la reserva no coincide con el ingresado";
                return RedirectToPage();
            }

            horarioDisponible.EstaReservado = false;

            _context.Reservas.Remove(reserva);

            await _context.SaveChangesAsync();

            return RedirectToPage();
        }
    }
}
