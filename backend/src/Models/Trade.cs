using System.ComponentModel.DataAnnotations;

namespace Models;

public abstract class Trade
{
    public string Id { get; set; } = string.Empty;
    
    [Required]
    public string ProductType { get; set; } = string.Empty;
    
    [Required]
    public string Counterparty { get; set; } = string.Empty;
    
    [Required]
    public DateTime TradeDate { get; set; } = DateTime.UtcNow.Date;
    
    [Required]
    public string BookedBy { get; set; } = string.Empty;
    
    public decimal? Npv { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
