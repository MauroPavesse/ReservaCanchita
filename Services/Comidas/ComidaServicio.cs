using Microsoft.EntityFrameworkCore;
using ReservaCanchita.Data;
using ReservaCanchita.Models;

namespace ReservaCanchita.Services.Comidas
{
    public class ComidaServicio
    {
        private readonly AppDbContext appDbContext;

        public ComidaServicio(AppDbContext appDbContext)
        {
            this.appDbContext = appDbContext;
        }

        public async Task<List<Comida>> ObtenerTodo(BuscadorEntrada buscador)
        {
            try
            {
                var query = appDbContext.Comidas.AsNoTracking();

                var includes = new List<Func<IQueryable<Comida>, IQueryable<Comida>>>();

                foreach (var incluye in buscador.TablasIncluidas)
                {
                    switch (incluye)
                    {
                        case "ComidaCategoria":
                            includes.Add(q => q.Include(x => x.ComidaCategoria));
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
                    var comidaCategoriaFiltro = buscador.Filtros.SingleOrDefault(x => x.Campo == "ComidaCategoriaId");
                    if (comidaCategoriaFiltro != null)
                    {
                        query = query.Where(x => x.ComidaCategoriaId == Convert.ToInt32(comidaCategoriaFiltro.Valor));
                    }

                    var comidasCategoriasFiltro = buscador.Filtros.SingleOrDefault(x => x.Campo == "ComidasCategoriasIds");
                    if (comidasCategoriasFiltro != null)
                    {
                        query = query.Where(x => comidasCategoriasFiltro.Ids.Contains(Convert.ToInt32(x.ComidaCategoriaId)));
                    }

                    var nombreFiltro = buscador.Filtros.SingleOrDefault(x => x.Campo == "Nombre");
                    if (nombreFiltro != null)
                    {
                        query = query.Where(x => x.Nombre.Equals(nombreFiltro.Valor, StringComparison.OrdinalIgnoreCase));
                    }
                }

                return await query.ToListAsync();
            }
            catch
            {
                throw;
            }
        }

        public async Task<Comida> Guardar(Comida input)
        {
            try
            {
                Comida comida = new Comida();
                if (input.Id == 0)
                {
                    comida = appDbContext.Comidas.Add(input).Entity;
                }
                else
                {
                    comida = appDbContext.Comidas.Update(input).Entity;
                }
                await appDbContext.SaveChangesAsync();
                return comida;
            }
            catch
            {
                throw;
            }
        }

        public async Task<Comida?> Eliminar(Comida comida)
        {
            try
            {
                appDbContext.Comidas.Remove(comida);
                await appDbContext.SaveChangesAsync();
                return comida;
            }
            catch
            {
                throw;
            }
        }
    }
}
