using System.ComponentModel.DataAnnotations;

namespace ReservaCanchita.Models;

public class Reserva
{
    public int Id { get; set; }

    [Required]
    public string NombreCliente { get; set; } = string.Empty;

    [Required]
    public string Telefono { get; set; } = string.Empty;

    [Required]
    public DateTime Fecha { get; set; }

    [Required]
    public int HorarioDisponibleId { get; set; }

    public HorarioDisponible HorarioDisponible { get; set; } = new HorarioDisponible();

    public int Estado { get; set; }
}
