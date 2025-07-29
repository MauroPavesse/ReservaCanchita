using System.ComponentModel.DataAnnotations;

namespace ReservaCanchita.Models;

public class Cancha
{
    public int Id { get; set; }

    [Required]
    public string NombreCancha { get; set; } = string.Empty;

    public ICollection<Horario> Horarios = new List<Horario>();
}
