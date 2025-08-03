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

        public async Task OnGetAsync()
        {
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

            var reserva = await _context.Reservas.Where(x => x.HorarioDisponibleId == HorarioDisponibleId).FirstOrDefaultAsync();

            if(reserva == null)
            {
                return RedirectToPage();
            }

            if(reserva.Telefono != Telefono)
            {
                return RedirectToPage();
            }

            horarioDisponible.EstaReservado = false;

            _context.Reservas.Remove(reserva);

            await _context.SaveChangesAsync();

            return RedirectToPage();
        }
    }
}
