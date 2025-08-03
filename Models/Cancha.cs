using System.ComponentModel.DataAnnotations;

namespace ReservaCanchita.Models;

public class Cancha
{
    public int Id { get; set; }

    [Required]
    public string NombreCancha { get; set; } = string.Empty;

    public ICollection<HorarioBase> HorariosBase { get; set; } = new List<HorarioBase>();
    public ICollection<HorarioDisponible> HorariosDisponibles { get; set; } = new List<HorarioDisponible>();
    public ICollection<CanchaSemana> CanchasSemanas { get; set; } = new List<CanchaSemana>();
}
