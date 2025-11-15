using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace ReservaCanchita.Models;

public class PagoMercadoPago
{
    public int Id { get; set; }
    [MaxLength(50)] public string PreferenceId { get; set; } = string.Empty;
    [MaxLength(50)] public string ExternalReference { get; set; } = string.Empty;
    [MaxLength(50)] public string PaymentId { get; set; } = string.Empty;
    [MaxLength(20)] public string Status { get; set; } = string.Empty;
    [MaxLength(500)] public string InitPoint { get; set; } = string.Empty;
    [Precision(10, 2)] public decimal TotalAmount { get; set; }
    public DateTime DateCreated { get; set; } 
    public DateTime DateUpdated { get; set; }
}
