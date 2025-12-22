using System.ComponentModel.DataAnnotations;

namespace TraderUI.Models;

public class SwapTrade : Trade
{
    public SwapTrade()
    {
        ProductType = "Swap";
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
    public string Id { get; set; } = string.Empty;
    
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

public class SwapTradeCreateRequest
{
    [Required]
    public string Counterparty { get; set; } = "Cpty1";
    
    [Required]
    public string Currency { get; set; } = "USD";
    
    [Required]
    public DateTime TradeDate { get; set; } = DateTime.Today;
    
    [Required]
    public DateTime EffectiveDate { get; set; } = DateTime.Today;
    
    [Required]
    public DateTime MaturityDate { get; set; } = DateTime.Today.AddYears(5);
    
    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal NotionalAmount { get; set; }
    
    [Required]
    public string NotionalCurrency { get; set; } = "USD";
    
    [Required]
    public SwapLegRequest Leg1 { get; set; } = new() { LegType = "FIXED", PayerReceiver = "PAY" };
    
    [Required]
    public SwapLegRequest Leg2 { get; set; } = new() { LegType = "FLOATING", PayerReceiver = "RECEIVE" };
}

public class SwapLegRequest
{
    [Required]
    public string LegType { get; set; } = "FIXED";
    
    [Required]
    public string PayerReceiver { get; set; } = "PAY";
    
    public decimal? FixedRate { get; set; }
    public string? ReferenceRate { get; set; }
    public decimal? Spread { get; set; }
    public string? ResetFrequency { get; set; }
    
    [Required]
    public string PaymentFrequency { get; set; } = "Q";
    
    [Required]
    public string DayCountConvention { get; set; } = "ACT/360";
    
    [Required]
    public string BusinessDayConvention { get; set; } = "MODFOLLOWING";
    
    [Required]
    public string PaymentCalendar { get; set; } = "NYC";
    
    public string? CompoundingMethod { get; set; }
    public string? CompoundingFrequency { get; set; }
    public string? AveragingMethod { get; set; }
    public string? AveragingFrequency { get; set; }
}
