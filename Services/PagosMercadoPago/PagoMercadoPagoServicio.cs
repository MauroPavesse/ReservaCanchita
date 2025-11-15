using ReservaCanchita.Data;
using ReservaCanchita.Models;

namespace ReservaCanchita.Services.PagosMercadoPago;

public class PagoMercadoPagoServicio
{
    private readonly AppDbContext appDbContext;

    public PagoMercadoPagoServicio(AppDbContext appDbContext)
    {
        this.appDbContext = appDbContext;
    }

    public async Task<PagoMercadoPago> Guardar(PagoMercadoPago input)
    {
        try
        {
            PagoMercadoPago pagoMercadoPago = new PagoMercadoPago();
            if (input.Id == 0)
            {
                pagoMercadoPago = appDbContext.PagosMercadoPago.Add(input).Entity;
            }
            else
            {
                pagoMercadoPago = appDbContext.PagosMercadoPago.Update(input).Entity;
            }
            await appDbContext.SaveChangesAsync();
            return pagoMercadoPago;
        }
        catch
        {
            throw;
        }
    }
}
