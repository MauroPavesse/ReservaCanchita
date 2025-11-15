using Microsoft.EntityFrameworkCore;
using ReservaCanchita.Models;

namespace ReservaCanchita.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    public DbSet<Cuenta> Cuentas { get; set; }
    public DbSet<Cancha> Canchas { get; set; }
    public DbSet<HorarioBase> HorariosBase { get; set; }
    public DbSet<HorarioDisponible> HorariosDisponibles { get; set; }
    public DbSet<Reserva> Reservas { get; set; }
    public DbSet<CanchaSemana> CanchasSemanas { get; set; }
    public DbSet<Configuracion> Configuraciones { get; set; }
    public DbSet<ComidaCategoria> ComidasCategorias { get; set; }
    public DbSet<Comida> Comidas { get; set; }
    public DbSet<PagoMercadoPago> PagosMercadoPago { get; set; }
}
