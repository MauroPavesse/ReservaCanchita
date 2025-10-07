namespace ReservaCanchita.Services
{
    public class BuscadorEntrada
    {
        public int Id { get; set; } = 0;
        public List<FiltroBuscadorEntrada> Filtros { get; set; } = new List<FiltroBuscadorEntrada>();
        public List<string> TablasIncluidas { get; set; } = new List<string>();
    }

    public class FiltroBuscadorEntrada
    {
        public string Campo { get; set; } = "";
        public string Valor { get; set; } = "";
        public List<int> Ids { get; set; } = new List<int>();
    }
}
