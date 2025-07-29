using System.ComponentModel.DataAnnotations;

namespace ReservaCanchita.Models;

public class Horario
{
    public int Id { get; set; }

    [Required]
    public int CanchaId { get; set; }

    [Required]
    public TimeSpan HoraInicio { get; set; }

    [Required]
    public TimeSpan HoraFin { get; set; }

    public ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();
}
