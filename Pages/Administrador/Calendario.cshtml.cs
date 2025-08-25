using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ReservaCanchita.Data;
using ReservaCanchita.Models;

namespace ReservaCanchita.Pages.Administrador
{
    public class CalendarioModel : PageModel
    {
        private readonly AppDbContext _context;

        public CalendarioModel(AppDbContext context)
        {
            _context = context;
        }

        public List<HorarioDisponible> HorariosDisponibles { get; set; } = new List<HorarioDisponible>();
        public List<object> EventosCalendario { get; set; } = new List<object>();
        [BindProperty]
        public int HorarioDisponibleId { get; set; }
        [BindProperty]
        public DateTime FechaAEliminar { get; set; }

        public async Task OnGetAsync()
        {
            HorariosDisponibles = await _context.HorariosDisponibles.ToListAsync();

            var eventos = new List<object>();

            foreach (var horarioDisponibleFecha in HorariosDisponibles.GroupBy(x => x.Fecha.Date))
            {
                var color = horarioDisponibleFecha.Count(x => x.EstaReservado) == 0 ? "green"
                    : horarioDisponibleFecha.Count(x => x.EstaReservado) < horarioDisponibleFecha.Count() ? "orange"
                    : "red";

                eventos.Add(new
                {
                    title = "",
                    start = horarioDisponibleFecha.Key.ToString("yyyy-MM-dd"),
                    allDay = true,
                    display = "background",
                    color = color
                });
            }

            EventosCalendario = eventos;
        }

        public JsonResult OnGetHorariosPorFecha(DateTime fecha)
        {
            var horarios = _context.HorariosDisponibles.Include(x => x.Cancha)
                .Where(x => x.Fecha.Date == fecha.Date)
                .AsEnumerable() // ejecuta la consulta
                .GroupBy(x => x.CanchaId)
                .Select(g => new {
                    CanchaId = g.Key,
                    Cancha = g.First().Cancha.NombreCancha,
                    Horarios = g.OrderBy(x => x.HoraInicio).Select(x => new {
                        x.Id,
                        HoraInicio = x.HoraInicio.ToString(@"hh\:mm"),
                        HoraFin = x.HoraFin.ToString(@"hh\:mm"),
                        Reservado = x.EstaReservado
                    }),
                });

            return new JsonResult(horarios);
        }

        public async Task<IActionResult> OnPostEliminarReservaAsync()
        {
            var horarioDisponible = await _context.HorariosDisponibles
                .Include(x => x.Reserva)
                .Where(x => x.Id == HorarioDisponibleId)
                .FirstAsync();

            if (horarioDisponible == null)
            {
                return NotFound();
            }

            horarioDisponible.EstaReservado = false;
            if(horarioDisponible.Reserva != null)
                _context.Reservas.Remove(horarioDisponible.Reserva!);
            
            await _context.SaveChangesAsync();

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostEliminarHorariosDiaAsync()
        {
            var horariosDelDia = await _context.HorariosDisponibles
                .Where(x => x.Fecha.Date == FechaAEliminar.Date)
                .ToListAsync();

            _context.HorariosDisponibles.RemoveRange(horariosDelDia);
            await _context.SaveChangesAsync();

            return RedirectToPage();
        }

        public async Task<JsonResult> OnGetDetalleReservaAsync(int id)
        {
            var horario = await _context.HorariosDisponibles
                .Include(h => h.Reserva)
                .Where(h => h.Id == id)
                .FirstAsync();

            return new JsonResult(new
            {
                Cliente = horario.Reserva!.NombreCliente,
                Telefono = horario.Reserva.Telefono
            });
        }
    }
}
