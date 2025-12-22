using Marten.Events.Aggregation;
using Models.Events;
using Models.Aggregates;

namespace Persistence.Projections;

// Read model for efficient querying
public class SwapTradeReadModel
{
    public string Id { get; set; } = string.Empty;
    public string Counterparty { get; set; } = string.Empty;
    public DateTime EffectiveDate { get; set; }
    public DateTime MaturityDate { get; set; }
    public decimal NotionalAmount { get; set; }
    public string NotionalCurrency { get; set; } = string.Empty;
    public DateTime TradeDate { get; set; }
    public string BookedBy { get; set; } = string.Empty;
    public decimal? Npv { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsCancelled { get; set; }
    public string? CancellationReason { get; set; }
    public DateTime? LastModified { get; set; }
    public SwapLegData? Leg1 { get; set; }
    public SwapLegData? Leg2 { get; set; }
}

// Projection for SwapTradeReadModel
public class SwapTradeProjection : SingleStreamProjection<SwapTradeReadModel>
{
    public SwapTradeProjection()
    {
        ProjectEvent<SwapTradeCreated>((model, @event) =>
        {
            model.Id = @event.TradeId;
            model.Counterparty = @event.Counterparty;
            model.EffectiveDate = @event.EffectiveDate;
            model.MaturityDate = @event.MaturityDate;
            model.NotionalAmount = @event.NotionalAmount;
            model.NotionalCurrency = @event.NotionalCurrency;
            model.TradeDate = @event.TradeDate;
            model.BookedBy = @event.BookedBy;
            model.CreatedAt = @event.Timestamp;
            model.LastModified = @event.Timestamp;
            model.Leg1 = @event.Leg1;
            model.Leg2 = @event.Leg2;
        });

        ProjectEvent<SwapTradeUpdated>((model, @event) =>
        {
            if (@event.Counterparty != null) model.Counterparty = @event.Counterparty;
            if (@event.EffectiveDate.HasValue) model.EffectiveDate = @event.EffectiveDate.Value;
            if (@event.MaturityDate.HasValue) model.MaturityDate = @event.MaturityDate.Value;
            if (@event.NotionalAmount.HasValue) model.NotionalAmount = @event.NotionalAmount.Value;
            if (@event.Leg1 != null) model.Leg1 = @event.Leg1;
            if (@event.Leg2 != null) model.Leg2 = @event.Leg2;
            model.LastModified = @event.Timestamp;
        });

        ProjectEvent<TradePriced>((model, @event) =>
        {
            model.Npv = @event.Npv;
            model.LastModified = @event.Timestamp;
        });

        ProjectEvent<TradeCancelled>((model, @event) =>
        {
            model.IsCancelled = true;
            model.CancellationReason = @event.Reason;
            model.LastModified = @event.Timestamp;
        });
    }
}
