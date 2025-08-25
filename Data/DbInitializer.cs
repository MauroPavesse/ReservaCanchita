using ReservaCanchita.Models;

namespace ReservaCanchita.Data;

public static class DbInitializer
{
    public static void Seed(AppDbContext context)
    {
        var configuracionesIniciales = new List<Configuracion>
        {
            new Configuracion { Campo = "NombreEmpresa", ValorString = "", ValorNumerico = 0 },
            new Configuracion { Campo = "NumeroContacto", ValorString = "", ValorNumerico = 0 },
            new Configuracion { Campo = "LogoEmpresa", ValorString = "logo.png", ValorNumerico = 0 },
            new Configuracion { Campo = "ValorReserva", ValorString = "", ValorNumerico = 0 },
            new Configuracion { Campo = "MuestraValorReserva", ValorString = "", ValorNumerico = 0},
            new Configuracion { Campo = "HoraToleranciaCancelarReserva", ValorString = "", ValorNumerico = 0},
            new Configuracion { Campo = "TrabajaConToleranciaCancelarReserva", ValorString = "", ValorNumerico = 0}
        };

        foreach (var config in configuracionesIniciales)
        {
            if (!context.Configuraciones.Any(c => c.Campo == config.Campo))
            {
                context.Configuraciones.Add(config);
            }
        }

        context.SaveChanges();
    }
}