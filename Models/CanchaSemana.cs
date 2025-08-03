namespace ReservaCanchita.Models
{
    public class CanchaSemana
    {
        public int Id { get; set; }
        public int CanchaId { get; set; }
        public Cancha Cancha { get; set; } = new Cancha();
        public int DiaSemana { get; set; }
        public int Activo { get; set; }
    }
}
