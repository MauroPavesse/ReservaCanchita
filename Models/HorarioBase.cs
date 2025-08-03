namespace ReservaCanchita.Models;

public class HorarioBase
{
    public int Id { get; set; }
    public int CanchaId { get; set; }
    public TimeSpan HoraInicio { get; set; }
    public TimeSpan HoraFin { get; set; }

    public Cancha Cancha { get; set; } = new Cancha();
}
