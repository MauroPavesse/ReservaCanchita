using Mapster;
using Microsoft.EntityFrameworkCore;
using ReservaCanchita.Data;
using ReservaCanchita.Models;
using ReservaCanchita.Services.Configuraciones.Dtos;

namespace ReservaCanchita.Services.Configuraciones
{
    public class ConfiguracionServicio
    {
        private readonly AppDbContext _context;

        public ConfiguracionServicio(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ItemConfiguracionOutput> ObtenerConfiguracion(string campo)
        {
            var configuracion = await _context.Configuraciones
                .FirstAsync(c => c.Campo == campo);

            return configuracion.Adapt<ItemConfiguracionOutput>();
        }

        public async Task<List<ItemConfiguracionOutput>> ObtenerConfiguraciones(List<string> campos)
        {
            var configuraciones = new List<Configuracion>();
            if (campos.Any())
            {
                configuraciones = await _context.Configuraciones
                    .Where(c => campos.Contains(c.Campo))
                    .ToListAsync();
            }
            else
            {
                configuraciones = await _context.Configuraciones
                    .ToListAsync();
            }

            return configuraciones.Adapt<List<ItemConfiguracionOutput>>();
        }

        public async Task GuardarConfiguraciones(List<Configuracion> inputs)
        {
            foreach(var input in inputs)
            {
                _context.Configuraciones.Update(input);
            }
            await _context.SaveChangesAsync();
        }
    }
}
