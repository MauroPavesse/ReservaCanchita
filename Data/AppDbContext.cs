using Microsoft.EntityFrameworkCore;
using ReservaCanchita.Models;

namespace ReservaCanchita.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    public DbSet<Reserva> Reservas { get; set; }
    public DbSet<Cancha> Canchas { get; set; }
    public DbSet<Horario> Horarios { get; set; }
    public DbSet<Cuenta> Cuentas { get; set; }
}
