using System.ComponentModel.DataAnnotations;

namespace Models;

public class PricingRequest
{
    [Required]
    public string TradeId { get; set; } = string.Empty;
    
    [Required]
    public int Seed { get; set; }
}