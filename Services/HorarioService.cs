using Microsoft.EntityFrameworkCore;
using ReservaCanchita.Data;
using ReservaCanchita.Models;

namespace ReservaCanchita.Services
{
    public class HorarioService
    {
        private readonly AppDbContext _context;

        public HorarioService(AppDbContext context)
        {
            _context = context;
        }

        public async Task GenerarHorariosDisponibles(int canchaId, int dias)
        {
            var cancha = await _context.Canchas
                .Include(c => c.HorariosBase)
                .FirstOrDefaultAsync(c => c.Id == canchaId);

            if (cancha == null) return;

            var hoy = DateTime.Today;

            foreach (var horarioBase in cancha.HorariosBase)
            {
                for (int i = 0; i < dias; i++)
                {
                    var fecha = hoy.AddDays(i);

                    bool yaExiste = _context.HorariosDisponibles.Any(h =>
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

    }
}
