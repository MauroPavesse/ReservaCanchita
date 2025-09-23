using Microsoft.EntityFrameworkCore;

namespace ReservaCanchita.Models
{
    public class Comida
    {
        public int Id { get; set; }
        public int? ComidaCategoriaId { get; set; }
        public ComidaCategoria? ComidaCategoria { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; } = string.Empty;
        [Precision(10, 2)]
        public decimal Importe { get; set; }
        public string Imagen { get; set; } = string.Empty;
    }
}
