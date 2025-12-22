namespace Models;

public class BulkPricingRequest
{
    public List<string> TradeIds { get; set; } = new();
    public int? Seed { get; set; }
}

public class TradePricingResult
{
    public required string TradeId { get; set; }
    public decimal Price { get; set; }
    public required string Currency { get; set; }
}

public class BulkPricingResult
{
    public List<TradePricingResult> Results { get; set; } = new();
}
