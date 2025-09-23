namespace ReservaCanchita.Models
{
    public class ComidaCategoria
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Imagen { get; set; } = string.Empty;
        public ICollection<Comida> Comidas { get; set; } = new List<Comida>();
    }
}
