using Microsoft.EntityFrameworkCore;
using ReservaCanchita.Data;
using ReservaCanchita.Models;

namespace ReservaCanchita.Services.Canchas
{
    public class CanchaServicio
    {
        private readonly AppDbContext appDbContext;

        public CanchaServicio(AppDbContext appDbContext)
        {
            this.appDbContext = appDbContext;
        }

        public async Task<List<Cancha>> ObtenerTodo(BuscadorEntrada buscador)
        {
            try
            {
                var query = appDbContext.Canchas.AsNoTracking();

                var includes = new List<Func<IQueryable<Cancha>, IQueryable<Cancha>>>();

                foreach (var incluye in buscador.TablasIncluidas)
                {
                    switch (incluye)
                    {
                        case "HorarioBase":
                            includes.Add(q => q.Include(x => x.HorariosBase));
                            break;
                        case "HorarioDisponible":
                            includes.Add(q => q.Include(x => x.HorariosDisponibles));
                            break;
                        case "HorarioDisponible_Reserva":
                            includes.Add(q => q.Include(x => x.HorariosDisponibles).ThenInclude(x => x.Reserva));
                            break;
                        case "CanchaSemana":
                            includes.Add(q => q.Include(x => x.CanchasSemanas));
                            break;
                    }
                }

                foreach (var include in includes)
                {
                    query = include(query);
                }

                if (buscador.Id > 0)
                {
                    query = query.Where(x => x.Id == buscador.Id);
                }
                else
                {
                    var nombreFiltro = buscador.Filtros.SingleOrDefault(x => x.Campo == "Nombre");
                    if(nombreFiltro != null)
                    {
                        query = query.Where(x => x.NombreCancha.Equals(nombreFiltro.Valor, StringComparison.OrdinalIgnoreCase));
                    }
                }

                return await query.ToListAsync();
            }
            catch
            {
                throw;
            }
        }

        public async Task<Cancha> Guardar(Cancha input)
        {
            try
            {
                Cancha cancha = new Cancha();
                if (input.Id == 0)
                {
                    cancha = appDbContext.Canchas.Add(input).Entity;
                }
                else
                {
                    cancha = appDbContext.Canchas.Update(input).Entity;
                }
                await appDbContext.SaveChangesAsync();
                return cancha;
            }
            catch
            {
                throw;
            }
        }

        public async Task<Cancha?> Eliminar(Cancha cancha)
        {
            try
            {
                appDbContext.Canchas.Remove(cancha);
                await appDbContext.SaveChangesAsync();
                return cancha;
            }
            catch
            {
                throw;
            }
        }
    }
}
