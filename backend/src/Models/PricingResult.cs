namespace Models;

public class PricingResult
{
    public string TradeId { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Currency { get; set; } = string.Empty;
}