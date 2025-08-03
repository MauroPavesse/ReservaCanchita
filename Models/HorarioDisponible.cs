namespace ReservaCanchita.Models
{
    public class HorarioDisponible
    {
        public int Id { get; set; }
        public int CanchaId { get; set; }
        public DateTime Fecha { get; set; }
        public TimeSpan HoraInicio { get; set; }
        public TimeSpan HoraFin { get; set; }

        public bool EstaReservado { get; set; } = false;

        public Cancha Cancha { get; set; } = new Cancha();
        public Reserva? Reserva { get; set; }
    }
}
