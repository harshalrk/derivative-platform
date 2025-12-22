namespace Models;

public class InstrumentType
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

public class Frequency
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty; // D, W, M, Q, S, A
}

public class DayCountConvention
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty; // 30/360, ACT/360, etc.
    public string Description { get; set; } = string.Empty;
}

public class BusinessDayConvention
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty; // FOLLOWING, MODFOLLOWING, etc.
    public string Description { get; set; } = string.Empty;
}

public class ReferenceRate
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty; // SOFR, LIBOR, etc.
    public string Currency { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

public class PaymentCalendar
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty; // NYC, LON, TGT, etc.
    public string Description { get; set; } = string.Empty;
}

public class CompoundingMethod
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty; // NONE, FLAT, STRAIGHT, etc.
    public string Description { get; set; } = string.Empty;
}

public class AveragingMethod
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty; // NONE, WEIGHTED, UNWEIGHTED
    public string Description { get; set; } = string.Empty;
}

public class LegType
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty; // FIXED, FLOATING
}

public class PayerReceiverType
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty; // PAY, RECEIVE
}
