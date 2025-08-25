using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ReservaCanchita.Data;
using ReservaCanchita.Models;

namespace ReservaCanchita.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly AppDbContext _context;

        public IndexModel(ILogger<IndexModel> logger, AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public List<HorarioDisponible> HorariosDisponibles { get; set; } = new List<HorarioDisponible>();
        [TempData]
        public string MensajeError { get; set; } = string.Empty;

        public async Task OnGetAsync()
        {
            var _ = MensajeError;
            HorariosDisponibles = await _context.HorariosDisponibles
                .Where(x => x.Fecha.Date >= DateTime.Now.Date)
                .ToListAsync();
        }

        public async Task<IActionResult> OnPostReservarCanchaAsync(int HorarioDisponibleId, string NombreCliente, string Telefono)
        {
            var horarioDisponible = await _context.HorariosDisponibles.Where(x => x.Id == HorarioDisponibleId).FirstAsync();

            if (horarioDisponible.EstaReservado)
            {
                return RedirectToPage();
            }

            _context.Reservas.Add(new Reserva()
            {
                NombreCliente = NombreCliente,
                Telefono = Telefono,
                Fecha = DateTime.Now,
                Estado = 1,
                HorarioDisponible = horarioDisponible
            });

            horarioDisponible.EstaReservado = true;

            await _context.SaveChangesAsync();

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostEliminarReservaAsync(int HorarioDisponibleId, string Telefono)
        {
            var horarioDisponible = await _context.HorariosDisponibles.Where(x => x.Id == HorarioDisponibleId).FirstAsync();

            if (!horarioDisponible.EstaReservado)
            {
                return RedirectToPage();
            }

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
