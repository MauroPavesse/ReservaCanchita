using System.ComponentModel.DataAnnotations;

namespace ReservaCanchita.Models;

public class Cuenta
{
    public int Id { get; set; }

    [Required]
    public string Usuario { get; set; } = string.Empty;

    [Required]
    public string Clave { get; set; } = string.Empty;

    [Required]
    public int CuentaTipoId { get; set; }
}
