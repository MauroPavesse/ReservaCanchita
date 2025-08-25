namespace ReservaCanchita.Services.Configuraciones.Dtos
{
    public class ItemConfiguracionOutput
    {
        public int Id { get; set; }
        public string Campo { get; set; } = "";
        public string ValorString { get; set; } = "";
        public decimal ValorNumerico { get; set; }
    }
}
