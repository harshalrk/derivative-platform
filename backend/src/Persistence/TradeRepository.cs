using Marten;
using Models;
using Models.Events;
using Models.Aggregates;
using Models.ReadModels;

namespace Persistence;

public interface ITradeRepository
{
    Task<IEnumerable<TradeReadModel>> GetTradesByUserAsync(string bookedBy);
    Task<TradeReadModel?> GetTradeByIdAsync(string tradeId);
    Task<string> CreateTradeAsync(SwapTrade swapTrade);
    Task<bool> UpdateTradeAsync(string tradeId, SwapTrade updatedTrade);
    Task<bool> PriceTradeAsync(string tradeId, decimal npv);
    Task<bool> CancelTradeAsync(string tradeId, string reason);
}

public interface ISessionRepository
{
    Task<UserSession> CreateSessionAsync(string userName);
    Task<UserSession?> GetSessionAsync(string sessionId);
    Task<IEnumerable<UserSession>> GetActiveSessionsAsync();
    Task<bool> UpdateLastAccessedAsync(string sessionId);
    Task<bool> DeleteSessionAsync(string sessionId);
}

public class TradeRepository : ITradeRepository
{
    private readonly IDocumentSession _session;

    public TradeRepository(IDocumentSession session)
    {
        _session = session;
    }

    public async Task<IEnumerable<TradeReadModel>> GetTradesByUserAsync(string bookedBy)
    {
        // Query from read model projection for fast reads - no event replay needed!
        return await _session.Query<TradeReadModel>()
            .Where(t => t.BookedBy == bookedBy && !t.IsCancelled)
            .OrderByDescending(t => t.TradeDate)
            .ToListAsync();
    }

    public async Task<TradeReadModel?> GetTradeByIdAsync(string tradeId)
    {
        // Fast indexed lookup - return null if cancelled
        var readModel = await _session.LoadAsync<TradeReadModel>(tradeId);
        return readModel?.IsCancelled == true ? null : readModel;
    }

    public async Task<string> CreateTradeAsync(SwapTrade swapTrade)
    {
        var tradeId = swapTrade.Id;
        var streamKey = $"trade-{tradeId}";
        
        var @event = new SwapTradeCreated
        {
            TradeId = tradeId,
            Counterparty = swapTrade.Counterparty,
            EffectiveDate = swapTrade.EffectiveDate,
            MaturityDate = swapTrade.MaturityDate,
            NotionalAmount = swapTrade.NotionalAmount,
            NotionalCurrency = swapTrade.NotionalCurrency,
            TradeDate = swapTrade.TradeDate,
            BookedBy = swapTrade.BookedBy,
            UserId = swapTrade.BookedBy,
            Timestamp = DateTime.UtcNow,
            Leg1 = MapSwapLeg(swapTrade.Leg1),
            Leg2 = MapSwapLeg(swapTrade.Leg2)
        };

        _session.Events.StartStream<SwapTradeAggregate>(streamKey, @event);
        await _session.SaveChangesAsync();
        
        return streamKey; // Return full stream key instead of just GUID
    }

    public async Task<bool> UpdateTradeAsync(string tradeId, SwapTrade updatedTrade)
    {
        var existing = await _session.Events.AggregateStreamAsync<SwapTradeAggregate>(tradeId);
        if (existing == null || existing.IsCancelled) return false;

        var @event = new SwapTradeUpdated
        {
            TradeId = tradeId,
            Counterparty = updatedTrade.Counterparty,
            EffectiveDate = updatedTrade.EffectiveDate,
            MaturityDate = updatedTrade.MaturityDate,
            NotionalAmount = updatedTrade.NotionalAmount,
            Leg1 = MapSwapLeg(updatedTrade.Leg1),
            Leg2 = MapSwapLeg(updatedTrade.Leg2),
            UserId = updatedTrade.BookedBy,
            Timestamp = DateTime.UtcNow
        };

        _session.Events.Append(tradeId, @event);
        await _session.SaveChangesAsync();
        
        return true;
    }

    public async Task<bool> PriceTradeAsync(string tradeId, decimal npv)
    {
        var existing = await _session.Events.AggregateStreamAsync<SwapTradeAggregate>(tradeId);
        if (existing == null || existing.IsCancelled) return false;

        var @event = new TradePriced
        {
            TradeId = tradeId,
            Npv = npv,
            PricingDate = DateTime.UtcNow,
            UserId = "system",
            Timestamp = DateTime.UtcNow
        };

        _session.Events.Append(tradeId, @event);
        await _session.SaveChangesAsync();
        
        return true;
    }

    public async Task<bool> CancelTradeAsync(string tradeId, string reason)
    {
        var existing = await _session.Events.AggregateStreamAsync<SwapTradeAggregate>(tradeId);
        if (existing == null || existing.IsCancelled) return false;

        var @event = new TradeCancelled
        {
            TradeId = tradeId,
            Reason = reason,
            UserId = existing.BookedBy,
            Timestamp = DateTime.UtcNow
        };

        _session.Events.Append(tradeId, @event);
        await _session.SaveChangesAsync();
        
        return true;
    }
    
    private SwapLegData MapSwapLeg(SwapLeg? leg)
    {
        if (leg == null) return new SwapLegData();
        
        return new SwapLegData
        {
            LegType = leg.LegType,
            PayerReceiver = leg.PayerReceiver,
            FixedRate = leg.FixedRate,
            ReferenceRate = leg.ReferenceRate,
            Spread = leg.Spread,
            ResetFrequency = leg.ResetFrequency,
            PaymentFrequency = leg.PaymentFrequency,
            DayCountConvention = leg.DayCountConvention,
            BusinessDayConvention = leg.BusinessDayConvention,
            PaymentCalendar = leg.PaymentCalendar,
            CompoundingMethod = leg.CompoundingMethod,
            CompoundingFrequency = leg.CompoundingFrequency,
            AveragingMethod = leg.AveragingMethod,
            AveragingFrequency = leg.AveragingFrequency
        };
    }
}

public class SessionRepository : ISessionRepository
{
    private readonly IDocumentSession _session;

    public SessionRepository(IDocumentSession session)
    {
        _session = session;
    }

    public async Task<UserSession> CreateSessionAsync(string userName)
    {
        // Check if user session already exists
        var existingSession = await _session.Query<UserSession>()
            .Where(s => s.UserName == userName)
            .FirstOrDefaultAsync();

        if (existingSession != null)
        {
            // Update last accessed time for existing session
            existingSession.LastAccessedAt = DateTime.UtcNow;
            _session.Store(existingSession);
            await _session.SaveChangesAsync();
            return existingSession;
        }

        // Create new session with username as ID for simplicity
        var userSession = new UserSession
        {
            Id = userName, // Use username as session ID
            UserName = userName,
            CreatedAt = DateTime.UtcNow,
            LastAccessedAt = DateTime.UtcNow
        };

        _session.Store(userSession);
        await _session.SaveChangesAsync();
        return userSession;
    }

    public async Task<UserSession?> GetSessionAsync(string sessionId)
    {
        return await _session.LoadAsync<UserSession>(sessionId);
    }

    public async Task<IEnumerable<UserSession>> GetActiveSessionsAsync()
    {
        return await _session.Query<UserSession>()
            .OrderByDescending(s => s.LastAccessedAt)
            .ToListAsync();
    }

    public async Task<bool> UpdateLastAccessedAsync(string sessionId)
    {
        var userSession = await _session.LoadAsync<UserSession>(sessionId);
        if (userSession == null) return false;

        userSession.LastAccessedAt = DateTime.UtcNow;
        _session.Store(userSession);
        await _session.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteSessionAsync(string sessionId)
    {
        var userSession = await _session.LoadAsync<UserSession>(sessionId);
        if (userSession == null) return false;

        _session.Delete<UserSession>(sessionId);
        await _session.SaveChangesAsync();
        return true;
    }
}