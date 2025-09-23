using Microsoft.EntityFrameworkCore;
using ReservaCanchita.Data;
using ReservaCanchita.Models;

namespace ReservaCanchita.Services.ComidasCategorias
{
    public class ComidaCategoriaService
    {
        private readonly AppDbContext appDbContext;

        public ComidaCategoriaService(AppDbContext appDbContext)
        {
            this.appDbContext = appDbContext;
        }

        public async Task<ComidaCategoria> Guardar(ComidaCategoria input)
        {
            try
            {
                var comidaCategoria = new ComidaCategoria();
                if (input.Id == 0)
                {
                    comidaCategoria = appDbContext.ComidasCategorias.Add(input).Entity;
                }
                else
                {
                    comidaCategoria = appDbContext.ComidasCategorias.Update(input).Entity;
                }
                await appDbContext.SaveChangesAsync();
                return comidaCategoria;
            }
            catch
            {
                throw;
            }
        }

        public async Task<List<ComidaCategoria>> ObtenerTodo()
        {
            try
            {
                return await appDbContext.ComidasCategorias.Include(x => x.Comidas).ToListAsync();
            }
            catch
            {
                throw;
            }
        }
    }
}
