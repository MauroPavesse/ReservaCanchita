using ReservaCanchita.Models;

namespace ReservaCanchita.Data;

public static class DbInitializer
{
    public static void Seed(AppDbContext context)
    {
        var configuracionesIniciales = new List<Configuracion>
        {
            new Configuracion { Id = 1, Campo = "NombreEmpresa", ValorString = "", ValorNumerico = 0 },
            new Configuracion { Id = 2, Campo = "NumeroContacto", ValorString = "", ValorNumerico = 0 },
            new Configuracion { Id = 3, Campo = "LogoEmpresa", ValorString = "logo.png", ValorNumerico = 0 },
            new Configuracion { Id = 4, Campo = "ValorReserva", ValorString = "", ValorNumerico = 0 },
            new Configuracion { Id = 5, Campo = "MuestraValorReserva", ValorString = "", ValorNumerico = 0},
            new Configuracion { Id = 6, Campo = "HoraToleranciaCancelarReserva", ValorString = "", ValorNumerico = 0},
            new Configuracion { Id = 7, Campo = "TrabajaConToleranciaCancelarReserva", ValorString = "", ValorNumerico = 0},
            new Configuracion { Id = 8, Campo = "TrabajaConWhatsapp", ValorString = "", ValorNumerico = 0},
            new Configuracion { Id = 9, Campo = "VersionIncluyeWhatsapp", ValorString = "", ValorNumerico = 0},
            new Configuracion { Id = 10, Campo = "TrabajaConMenuComidas", ValorString = "", ValorNumerico = 0},
            new Configuracion { Id = 11, Campo = "VersionIncluyeMenuComidas", ValorString = "", ValorNumerico = 0},
            new Configuracion { Id = 12, Campo = "VersionIncluyeMercadoPago", ValorString = "", ValorNumerico = 0},
            new Configuracion { Id = 13, Campo = "TrabajaConSena", ValorString = "", ValorNumerico = 0},
            new Configuracion { Id = 14, Campo = "AccessTokenMp", ValorString = "", ValorNumerico = 0},
            new Configuracion { Id = 15, Campo = "AliasMp", ValorString = "", ValorNumerico = 0}
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