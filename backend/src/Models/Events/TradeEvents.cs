namespace Models.Events;

// Base event for all trade events
public abstract class TradeEvent
{
    public string TradeId { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string UserId { get; set; } = string.Empty;
}

// Swap trade lifecycle events
public class TradePriced : TradeEvent
{
    public decimal Npv { get; set; }
    public DateTime PricingDate { get; set; }
}

public class TradeCancelled : TradeEvent
{
    public string Reason { get; set; } = string.Empty;
}

// Swap trade creation event
public class SwapTradeCreated : TradeEvent
{
    public string Counterparty { get; set; } = string.Empty;
    public DateTime EffectiveDate { get; set; }
    public DateTime MaturityDate { get; set; }
    public decimal NotionalAmount { get; set; }
    public string NotionalCurrency { get; set; } = string.Empty;
    public DateTime TradeDate { get; set; }
    public string BookedBy { get; set; } = string.Empty;
    
    // Leg details
    public SwapLegData Leg1 { get; set; } = new();
    public SwapLegData Leg2 { get; set; } = new();
}

public class SwapLegData
{
    public string LegType { get; set; } = string.Empty;
    public string PayerReceiver { get; set; } = string.Empty;
    public decimal? FixedRate { get; set; }
    public string? ReferenceRate { get; set; }
    public decimal? Spread { get; set; }
    public string? ResetFrequency { get; set; }
    public string PaymentFrequency { get; set; } = string.Empty;
    public string DayCountConvention { get; set; } = string.Empty;
    public string BusinessDayConvention { get; set; } = string.Empty;
    public string PaymentCalendar { get; set; } = string.Empty;
    public string? CompoundingMethod { get; set; }
    public string? CompoundingFrequency { get; set; }
    public string? AveragingMethod { get; set; }
    public string? AveragingFrequency { get; set; }
}

public class SwapTradeUpdated : TradeEvent
{
    public string? Counterparty { get; set; }
    public DateTime? EffectiveDate { get; set; }
    public DateTime? MaturityDate { get; set; }
    public decimal? NotionalAmount { get; set; }
    public SwapLegData? Leg1 { get; set; }
    public SwapLegData? Leg2 { get; set; }
}
