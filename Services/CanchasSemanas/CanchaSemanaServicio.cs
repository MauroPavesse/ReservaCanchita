using Microsoft.EntityFrameworkCore;
using ReservaCanchita.Data;
using ReservaCanchita.Models;

namespace ReservaCanchita.Services.CanchasSemanas
{
    public class CanchaSemanaServicio
    {
        private readonly AppDbContext appDbContext;

        public CanchaSemanaServicio(AppDbContext appDbContext)
        {
            this.appDbContext = appDbContext;
        }

        public async Task<List<CanchaSemana>> ObtenerTodo(BuscadorEntrada buscador)
        {
            try
            {
                var query = appDbContext.CanchasSemanas.AsNoTracking();

                var includes = new List<Func<IQueryable<CanchaSemana>, IQueryable<CanchaSemana>>>();

                foreach (var incluye in buscador.TablasIncluidas)
                {
                    switch (incluye)
                    {
                        case "Cancha":
                            includes.Add(q => q.Include(x => x.Cancha));
                            break;
                        case "Cancha_HorarioBase":
                            includes.Add(q => q.Include(x => x.Cancha.HorariosBase));
                            break;
                        case "Cancha_HorarioDisponible":
                            includes.Add(q => q.Include(x => x.Cancha.HorariosDisponibles));
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
                    var canchaFiltro = buscador.Filtros.SingleOrDefault(x => x.Campo == "CanchaId");
                    if (canchaFiltro != null)
                    {
                        query = query.Where(x => x.CanchaId == Convert.ToInt32(canchaFiltro.Valor));
                    }

                    var canchasFiltro = buscador.Filtros.SingleOrDefault(x => x.Campo == "CanchasIds");
                    if (canchasFiltro != null)
                    {
                        query = query.Where(x => canchasFiltro.Ids.Contains(x.CanchaId));
                    }
                }

                return await query.ToListAsync();
            }
            catch
            {
                throw;
            }
        }

        public async Task<CanchaSemana> Guardar(CanchaSemana input)
        {
            try
            {
                CanchaSemana canchaSemana = new CanchaSemana();
                if (input.Id == 0)
                {
                    canchaSemana = appDbContext.CanchasSemanas.Add(input).Entity;
                }
                else
                {
                    canchaSemana = appDbContext.CanchasSemanas.Update(input).Entity;
                }
                await appDbContext.SaveChangesAsync();
                return canchaSemana;
            }
            catch
            {
                throw;
            }
        }

        public async Task<CanchaSemana?> Eliminar(CanchaSemana canchaSemana)
        {
            try
            {
                appDbContext.CanchasSemanas.Remove(canchaSemana);
                await appDbContext.SaveChangesAsync();
                return canchaSemana;
            }
            catch
            {
                throw;
            }
        }
    }
}
