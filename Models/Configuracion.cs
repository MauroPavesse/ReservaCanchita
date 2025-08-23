namespace ReservaCanchita.Models;

public class Configuracion
{
    public int Id { get; set; }
    public string Campo { get; set; } = string.Empty;
    public string ValorString { get; set; } = string.Empty;
    public decimal ValorNumerico { get; set; }
}
