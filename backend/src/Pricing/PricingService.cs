using Models;

namespace Pricing;

public interface IPricingService
{
    Task<PricingResult> PriceTradeAsync(SwapTrade trade, int seed);
}

public class PricingService : IPricingService
{
    public Task<PricingResult> PriceTradeAsync(SwapTrade trade, int seed)
    {
        // Deterministic pricing using the provided seed
        var random = new Random(seed);
        
        // Generate NPV variation between -5% and +5% of the notional amount
        var variation = random.NextDouble() * 0.1 - 0.05; // -0.05 to 0.05
        var npv = trade.NotionalAmount * (decimal)variation;
        
        // Round to 2 decimal places for display
        npv = Math.Round(npv, 2);
        
        var result = new PricingResult
        {
            TradeId = trade.Id,
            Price = npv,
            Currency = trade.NotionalCurrency
        };
        
        return Task.FromResult(result);
    }
}