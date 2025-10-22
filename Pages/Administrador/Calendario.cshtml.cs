using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ReservaCanchita.Data;
using ReservaCanchita.Models;
using ReservaCanchita.Services.HorariosDisponibles;

namespace ReservaCanchita.Pages.Administrador
{
    public class CalendarioModel : PageModel
    {
        private readonly AppDbContext _context;
        private HorarioDisponibleServicio horarioDisponibleServicio;

        public CalendarioModel(AppDbContext context, HorarioDisponibleServicio horarioDisponibleServicio)
        {
            _context = context;
            this.horarioDisponibleServicio = horarioDisponibleServicio;
        }

        public List<HorarioDisponible> HorariosDisponibles { get; set; } = new List<HorarioDisponible>();
        public List<object> EventosCalendario { get; set; } = new List<object>();
        [BindProperty]
        public int HorarioDisponibleId { get; set; }
        [BindProperty]
        public DateTime FechaAEliminar { get; set; }
        [BindProperty]
        public List<Tuple<string, int>> DatosHistograma { get; set; } = new List<Tuple<string, int>>();

        public async Task OnGetAsync()
        {
            /*DateTime fechaInicio = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
            DateTime fechaFin = fechaInicio.AddMonths(1).AddDays(-1);
            HorariosDisponibles = await horarioDisponibleServicio.ObtenerDesdeHasta(fechaInicio, fechaFin);
            //HorariosDisponibles = await _context.HorariosDisponibles.ToListAsync();

            var eventos = acomodarCalendario();
            EventosCalendario = eventos;*/
        }

        private List<object> acomodarCalendario()
        {
            var eventos = new List<object>();

            DatosHistograma = new List<Tuple<string, int>>();
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

                DatosHistograma.Add(new Tuple<string, int>(horarioDisponibleFecha.First().Fecha.ToString("dd/MM/yy"), horarioDisponibleFecha.Count(x => x.EstaReservado)));
            }

            return eventos;
        }

        public async Task<JsonResult> OnGetEventosPorMes(int mes, int anio)
        {
            DateTime fechaInicio = new DateTime(anio, mes+1, 1);
            DateTime fechaFin = fechaInicio.AddMonths(1).AddDays(-1);
            HorariosDisponibles = await horarioDisponibleServicio.ObtenerDesdeHasta(fechaInicio, fechaFin);

            var eventos = acomodarCalendario();
            

            return new JsonResult(eventos);
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

        public async Task<JsonResult> OnGetDatosHistogramaAsync(int mes, int anio)
        {
            DateTime fechaInicio = new DateTime(anio, mes+1, 1);
            DateTime fechaFin = fechaInicio.AddMonths(1).AddDays(-1);

            // Puedes reutilizar tu servicio
            var horarios = await horarioDisponibleServicio.ObtenerDesdeHasta(fechaInicio, fechaFin);

            // Agrupar por cancha o por día, tú decides
            // Aquí un ejemplo agrupando por día:
            var datos = horarios
                .GroupBy(h => h.Fecha.Date)
                .Select(g => new {
                    Fecha = g.Key.ToString("dd/MM"),
                    Cantidad = g.Where(x => x.EstaReservado).Count()
                })
                .OrderBy(x => x.Fecha)
                .ToList();

            int max = horarios.GroupBy(h => h.Fecha.Date).ToList().First().Count();

            return new JsonResult(new
            {
                valores = datos,
                max = max
            });
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
