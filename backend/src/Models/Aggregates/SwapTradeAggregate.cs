using Models.Events;

namespace Models.Aggregates;

// Swap trade aggregate that rebuilds state from events
public class SwapTradeAggregate
{
    public string Id { get; private set; } = string.Empty;
    public string Counterparty { get; private set; } = string.Empty;
    public DateTime EffectiveDate { get; private set; }
    public DateTime MaturityDate { get; private set; }
    public decimal NotionalAmount { get; private set; }
    public string NotionalCurrency { get; private set; } = string.Empty;
    public DateTime TradeDate { get; private set; }
    public string BookedBy { get; private set; } = string.Empty;
    public decimal? Npv { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public bool IsCancelled { get; private set; }
    public string? CancellationReason { get; private set; }
    
    public SwapLegData? Leg1 { get; private set; }
    public SwapLegData? Leg2 { get; private set; }

    public void Apply(SwapTradeCreated @event)
    {
        Id = @event.TradeId;
        Counterparty = @event.Counterparty;
        EffectiveDate = @event.EffectiveDate;
        MaturityDate = @event.MaturityDate;
        NotionalAmount = @event.NotionalAmount;
        NotionalCurrency = @event.NotionalCurrency;
        TradeDate = @event.TradeDate;
        BookedBy = @event.BookedBy;
        CreatedAt = @event.Timestamp;
        Leg1 = @event.Leg1;
        Leg2 = @event.Leg2;
    }

    public void Apply(SwapTradeUpdated @event)
    {
        if (@event.Counterparty != null) Counterparty = @event.Counterparty;
        if (@event.EffectiveDate.HasValue) EffectiveDate = @event.EffectiveDate.Value;
        if (@event.MaturityDate.HasValue) MaturityDate = @event.MaturityDate.Value;
        if (@event.NotionalAmount.HasValue) NotionalAmount = @event.NotionalAmount.Value;
        if (@event.Leg1 != null) Leg1 = @event.Leg1;
        if (@event.Leg2 != null) Leg2 = @event.Leg2;
    }

    public void Apply(TradePriced @event)
    {
        Npv = @event.Npv;
    }

    public void Apply(TradeCancelled @event)
    {
        IsCancelled = true;
        CancellationReason = @event.Reason;
    }
}
