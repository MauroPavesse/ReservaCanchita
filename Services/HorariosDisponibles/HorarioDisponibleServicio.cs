using Microsoft.EntityFrameworkCore;
using ReservaCanchita.Data;
using ReservaCanchita.Models;

namespace ReservaCanchita.Services.HorariosDisponibles
{
    public class HorarioDisponibleServicio
    {
        private readonly AppDbContext _context;

        public HorarioDisponibleServicio(AppDbContext context)
        {
            _context = context;
        }

        public async Task GenerarHorariosDisponibles(int canchaId, int dias, int diaInicio = 0)
        {
            var cancha = await _context.Canchas
                .Include(c => c.HorariosBase)
                .FirstOrDefaultAsync(c => c.Id == canchaId);

            if (cancha == null) return;

            var hoy = DateTime.Today;

            foreach (var horarioBase in cancha.HorariosBase)
            {
                for (int i = diaInicio; i < dias; i++)
                {
                    var fecha = hoy.AddDays(i);

                    bool yaExiste = await _context.HorariosDisponibles.AnyAsync(h =>
                        h.CanchaId == canchaId &&
                        h.Fecha == fecha &&
                        h.HoraInicio == horarioBase.HoraInicio &&
                        h.HoraFin == horarioBase.HoraFin
                    );

                    if (!yaExiste)
                    {
                        var horarioDisponible = new HorarioDisponible
                        {
                            CanchaId = canchaId,
                            Fecha = fecha,
                            HoraInicio = horarioBase.HoraInicio,
                            HoraFin = horarioBase.HoraFin
                        };

                        _context.HorariosDisponibles.Add(horarioDisponible);
                    }
                }
            }

            await _context.SaveChangesAsync();
        }

        public async Task<List<HorarioDisponible>> ObtenerDesdeHasta(DateTime fechaDesde, DateTime fechaHasta)
        {
            var horariosDisponibles = await _context.HorariosDisponibles
                .Where(x => x.Fecha.Date >= fechaDesde.Date && x.Fecha.Date <= fechaHasta.Date)
                .ToListAsync();

            return horariosDisponibles;
        }
    }
}
