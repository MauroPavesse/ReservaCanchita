using Microsoft.EntityFrameworkCore;
using ReservaCanchita.Data;
using ReservaCanchita.Models;

namespace ReservaCanchita.Services.Comidas
{
    public class ComidaService
    {
        private readonly AppDbContext appDbContext;

        public ComidaService(AppDbContext appDbContext)
        {
            this.appDbContext = appDbContext;
        }

        public async Task<List<Comida>> ObtenerTodo()
        {
            try
            {
                return await appDbContext.Comidas.Include(x => x.ComidaCategoria).ToListAsync();
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

        public async Task<Comida?> ObtenerPorId(int comidaId)
        {
            try
            {
                return await appDbContext.Comidas.Include(x => x.ComidaCategoria).Where(x => x.Id == comidaId).FirstOrDefaultAsync();
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
