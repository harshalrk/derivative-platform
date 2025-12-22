using Marten.Events.Aggregation;
using Marten.Events.Projections;
using Models.Events;
using Models.ReadModels;

namespace Persistence.Projections;

public class TradeProjection : SingleStreamProjection<TradeReadModel>
{
    public TradeReadModel Create(SwapTradeCreated @event)
    {
        return new TradeReadModel
        {
            Id = @event.TradeId,
            Counterparty = @event.Counterparty,
            EffectiveDate = @event.EffectiveDate,
            MaturityDate = @event.MaturityDate,
            NotionalAmount = @event.NotionalAmount,
            NotionalCurrency = @event.NotionalCurrency,
            TradeDate = @event.TradeDate,
            BookedBy = @event.BookedBy,
            IsCancelled = false,
            CreatedAt = @event.Timestamp,
            Leg1 = @event.Leg1,
            Leg2 = @event.Leg2
        };
    }

    public void Apply(SwapTradeUpdated @event, TradeReadModel model)
    {
        model.Counterparty = @event.Counterparty ?? model.Counterparty;
        if (@event.EffectiveDate.HasValue) model.EffectiveDate = @event.EffectiveDate.Value;
        if (@event.MaturityDate.HasValue) model.MaturityDate = @event.MaturityDate.Value;
        if (@event.NotionalAmount.HasValue) model.NotionalAmount = @event.NotionalAmount.Value;
        if (@event.Leg1 != null) model.Leg1 = @event.Leg1;
        if (@event.Leg2 != null) model.Leg2 = @event.Leg2;
        model.UpdatedAt = @event.Timestamp;
    }

    public void Apply(TradePriced @event, TradeReadModel model)
    {
        model.Npv = @event.Npv;
        model.UpdatedAt = @event.Timestamp;
    }

    public void Apply(TradeCancelled @event, TradeReadModel model)
    {
        model.IsCancelled = true;
        model.CancellationReason = @event.Reason;
        model.UpdatedAt = @event.Timestamp;
    }
}
