using System.ComponentModel.DataAnnotations;

namespace TraderUI.Models;

public enum TradeSide
{
    Buy,
    Sell
}

public abstract class Trade
{
    public Trade() { }
    
    public string Id { get; set; } = string.Empty;
    
    [Required]
    public string ProductType { get; set; } = string.Empty;
    
    [Required]
    public string Counterparty { get; set; } = string.Empty;
    
    [Required]
    public DateTime TradeDate { get; set; }
    
    [Required]
    public string BookedBy { get; set; } = string.Empty;
    
    public decimal? Npv { get; set; }
    
    public DateTime CreatedAt { get; set; }
}

public class TradeResponse
{
    public string Id { get; set; } = string.Empty;
    public string ProductType { get; set; } = string.Empty;
    public string Counterparty { get; set; } = string.Empty;
    public DateTime TradeDate { get; set; }
    public string BookedBy { get; set; } = string.Empty;
    public decimal? Npv { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class TradeCreateRequest
{
    [Required]
    public string Instrument { get; set; } = string.Empty;
    
    public TradeSide Side { get; set; }
    
    [Range(0.01, double.MaxValue)]
    public decimal Quantity { get; set; }
    
    [Range(0, double.MaxValue)]
    public decimal Price { get; set; }
    
    [Required]
    public string Currency { get; set; } = string.Empty;
    
    public DateTime TradeDate { get; set; }
}

public class PricingRequest
{
    [Required]
    public string Instrument { get; set; } = string.Empty;
    
    public TradeSide Side { get; set; }
    
    [Range(0.01, double.MaxValue)]
    public decimal Quantity { get; set; }
    
    [Required]
    public string Currency { get; set; } = string.Empty;
    
    public int? RandomSeed { get; set; }
}

public class PricingResult
{
    public decimal Price { get; set; }
    public decimal Spread { get; set; }
    public decimal BidPrice { get; set; }
    public decimal AskPrice { get; set; }
    public DateTime Timestamp { get; set; }
    public int RandomSeed { get; set; }
    public string Currency { get; set; } = string.Empty;
}

public class UserSession
{
    public string Id { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime LastAccessedAt { get; set; }
}

public class BulkPricingResult
{
    public List<TradePricingResult> Results { get; set; } = new();
}

public class TradePricingResult
{
    public string TradeId { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Currency { get; set; } = string.Empty;
}