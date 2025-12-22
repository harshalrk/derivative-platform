namespace Models.ReadModels;

public class TradeReadModel
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
    public bool IsCancelled { get; set; }
    public string? CancellationReason { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    
    // Store legs as JSON in a single column for simplicity
    // Alternatively, could create separate Leg1/Leg2 complex type properties
    public Events.SwapLegData? Leg1 { get; set; }
    public Events.SwapLegData? Leg2 { get; set; }
}
