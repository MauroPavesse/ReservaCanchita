using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ReservaCanchita.Data;
using ReservaCanchita.Models;

namespace ReservaCanchita.Pages.Administrador
{
    public class DashboardModel : PageModel
    {
        private readonly AppDbContext _context;

        public DashboardModel(AppDbContext context)
        {
            _context = context;
        }

        public List<Cancha> Canchas { get; set; } = new List<Cancha>();

        [BindProperty(SupportsGet = true)]
        public string MensajeError { get; set; } = string.Empty;

        public async Task OnGetAsync()
        {
            Canchas = await _context.Canchas
                .Include(x => x.HorariosBase)
                .Include(x => x.CanchasSemanas)
                .ToListAsync();
        }

        public async Task<IActionResult> OnPostAgregarHorarioBaseAsync(int CanchaId, TimeSpan HoraInicio, TimeSpan HoraFin)
        {
            if (HoraInicio >= HoraFin)
            {
                return RedirectToPage("Dashboard", new { MensajeError = "La hora de inicio debe ser anterior a la de fin." });
            }

            var cancha = await _context.Canchas.Include(c => c.HorariosBase)
                .Include(x => x.CanchasSemanas)
                .Include(x => x.HorariosDisponibles)
                .Include(x => x.CanchasSemanas)
                .FirstOrDefaultAsync(c => c.Id == CanchaId);

            if (cancha == null)
                return NotFound();

            if (cancha.HorariosBase.Any(x => !((HoraInicio < x.HoraInicio && HoraFin <= x.HoraInicio) || (HoraInicio >= x.HoraFin))))
            {
                return RedirectToPage("Dashboard", new { MensajeError = "Ya existe un rango horarios en el horario ingresado." }); ;
            }

            cancha.HorariosBase.Add(new HorarioBase
            {
                HoraInicio = HoraInicio,
                HoraFin = HoraFin
            });

            if (cancha.CanchasSemanas.Any())
            {
                for (var fecha = DateTime.Now; fecha <= DateTime.Now.AddDays(15); fecha = fecha.AddDays(1))
                {
                    var canchaSemana = cancha.CanchasSemanas.First(x => x.DiaSemana == (int)fecha.DayOfWeek);

                    if (canchaSemana.Activo == 1)
                    {
                        cancha.HorariosDisponibles.Add(new HorarioDisponible()
                        {
                            Fecha = fecha,
                            HoraInicio = HoraInicio,
                            HoraFin = HoraFin
                        });
                    }
                }
            }

            await _context.SaveChangesAsync();
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostAgregarCancha(string NombreCancha)
        {
            await _context.Canchas.AddAsync(new Cancha()
            {
                NombreCancha = NombreCancha
            });

            await _context.SaveChangesAsync();
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostAsignarDiasSemanaAsync(int CanchaId, List<int> DiasSeleccionados)
        {
            var cancha = (await _context.Canchas
                .Include(x => x.HorariosDisponibles)
                .Include(x => x.HorariosBase)
                .Include(x => x.CanchasSemanas)
                .Where(x => x.Id == CanchaId).ToListAsync()).First();

            for(int i = 0; i < 7; i++)
            {
                var canchaSemana = cancha.CanchasSemanas.FirstOrDefault(x => x.DiaSemana == i);

                int activo = DiasSeleccionados.Any(x => x == i) ? 1 : 0;

                if (canchaSemana != null)
                {
                    canchaSemana.Activo = activo;
                }
                else
                {
                    cancha.CanchasSemanas.Add(new CanchaSemana()
                    {
                        DiaSemana = i,
                        Activo = activo
                    });
                }
            }
            await _context.SaveChangesAsync();

            var horariosDisponiblesModificar = cancha.HorariosDisponibles.Where(x => x.Fecha.Date >= DateTime.Now.Date).ToList();

            for (var fecha = DateTime.Now; fecha <= DateTime.Now.AddDays(15); fecha = fecha.AddDays(1))
            {
                var canchaSemana = cancha.CanchasSemanas.First(x => x.DiaSemana == (int)fecha.DayOfWeek);

                var horariosDisponiblesFecha = cancha.HorariosDisponibles.Where(x => x.Fecha.Date == fecha.Date).ToList();

                foreach (var horarioBase in cancha.HorariosBase)
                {
                    var horarioDisponibleActual = horariosDisponiblesFecha.FirstOrDefault(x => x.HoraInicio == horarioBase.HoraInicio && x.HoraFin == horarioBase.HoraFin);

                    if (horarioDisponibleActual != null) // Existe
                    {
                        if (canchaSemana.Activo == 0) // Tengo que borrar el registro
                        {
                            _context.HorariosDisponibles.Remove(horarioDisponibleActual);
                        }
                    }
                    else // No existe
                    {
                        if (canchaSemana.Activo == 1) // Agrego
                        {
                            cancha.HorariosDisponibles.Add(new HorarioDisponible()
                            {
                                Fecha = fecha,
                                HoraInicio = horarioBase.HoraInicio,
                                HoraFin = horarioBase.HoraFin
                            });
                        }
                    }
                }
            }

            await _context.SaveChangesAsync();

            return RedirectToPage("Dashboard");
        }

        public async Task<IActionResult> OnPostEliminarHorarioBaseAsync(int HorarioBaseId, int CanchaId)
        {
            var horario = await _context.HorariosBase.FindAsync(HorarioBaseId);
            if (horario != null)
            {
                _context.HorariosBase.Remove(horario);

                var cancha = await _context.Canchas.Include(x => x.HorariosDisponibles).Where(x => x.Id == CanchaId).FirstAsync();

                var horariosDisponiblesAEliminar = cancha.HorariosDisponibles.Where(x => x.Fecha.Date >= DateTime.Now.Date && x.HoraInicio == horario.HoraInicio && x.HoraFin == horario.HoraFin).ToList();

                foreach (var horarioDisponible in horariosDisponiblesAEliminar)
                {
                    cancha.HorariosDisponibles.Remove(horarioDisponible);
                }

                await _context.SaveChangesAsync();
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostEliminarCanchaAsync(int CanchaId)
        {
            var cancha = await _context.Canchas
                .Include(x => x.HorariosBase)
                .Include(x => x.CanchasSemanas)
                .Include(x => x.HorariosDisponibles)
                .Where(x => x.Id == CanchaId).FirstAsync();

            _context.Canchas.Remove(cancha);
            await _context.SaveChangesAsync();
            return RedirectToPage();
        }
    }
}
