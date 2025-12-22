using System.ComponentModel.DataAnnotations;

namespace Models;

public class SwapTrade : Trade
{
    public SwapTrade()
    {
        ProductType = "Swap";
        Id = Guid.NewGuid().ToString();
    }
    
    [Required]
    public DateTime EffectiveDate { get; set; }
    
    [Required]
    public DateTime MaturityDate { get; set; }
    
    [Required]
    public decimal NotionalAmount { get; set; }
    
    [Required]
    public string NotionalCurrency { get; set; } = string.Empty;
    
    public SwapLeg? Leg1 { get; set; }
    public SwapLeg? Leg2 { get; set; }
}

public class SwapLeg
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    
    [Required]
    public string LegType { get; set; } = string.Empty; // FIXED or FLOATING
    
    [Required]
    public string PayerReceiver { get; set; } = string.Empty; // PAY or RECEIVE
    
    // Fixed leg fields
    public decimal? FixedRate { get; set; }
    
    // Floating leg fields
    public string? ReferenceRate { get; set; }
    public decimal? Spread { get; set; }
    public string? ResetFrequency { get; set; }
    
    // Common fields
    [Required]
    public string PaymentFrequency { get; set; } = string.Empty;
    
    [Required]
    public string DayCountConvention { get; set; } = string.Empty;
    
    [Required]
    public string BusinessDayConvention { get; set; } = string.Empty;
    
    [Required]
    public string PaymentCalendar { get; set; } = string.Empty;
    
    // Optional compounding
    public string? CompoundingMethod { get; set; }
    public string? CompoundingFrequency { get; set; }
    
    // Optional averaging
    public string? AveragingMethod { get; set; }
    public string? AveragingFrequency { get; set; }
}

public class SwapTradeCreateRequest
{
    [Required]
    public string Counterparty { get; set; } = string.Empty;
    
    [Required]
    public DateTime TradeDate { get; set; }
    
    [Required]
    public DateTime EffectiveDate { get; set; }
    
    [Required]
    public DateTime MaturityDate { get; set; }
    
    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal NotionalAmount { get; set; }
    
    [Required]
    public string NotionalCurrency { get; set; } = string.Empty;
    
    [Required]
    public SwapLegRequest Leg1 { get; set; } = new();
    
    [Required]
    public SwapLegRequest Leg2 { get; set; } = new();
}

public class SwapLegRequest
{
    [Required]
    public string LegType { get; set; } = string.Empty;
    
    [Required]
    public string PayerReceiver { get; set; } = string.Empty;
    
    public decimal? FixedRate { get; set; }
    public string? ReferenceRate { get; set; }
    public decimal? Spread { get; set; }
    public string? ResetFrequency { get; set; }
    
    [Required]
    public string PaymentFrequency { get; set; } = string.Empty;
    
    [Required]
    public string DayCountConvention { get; set; } = string.Empty;
    
    [Required]
    public string BusinessDayConvention { get; set; } = string.Empty;
    
    [Required]
    public string PaymentCalendar { get; set; } = string.Empty;
    
    public string? CompoundingMethod { get; set; }
    public string? CompoundingFrequency { get; set; }
    public string? AveragingMethod { get; set; }
    public string? AveragingFrequency { get; set; }
}
