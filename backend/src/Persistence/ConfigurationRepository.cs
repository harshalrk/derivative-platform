using Marten;
using Models;

namespace Persistence;

public interface IConfigurationRepository
{
    Task<IEnumerable<InstrumentType>> GetInstrumentTypesAsync();
    Task<IEnumerable<Frequency>> GetFrequenciesAsync();
    Task<IEnumerable<DayCountConvention>> GetDayCountConventionsAsync();
    Task<IEnumerable<BusinessDayConvention>> GetBusinessDayConventionsAsync();
    Task<IEnumerable<ReferenceRate>> GetReferenceRatesAsync();
    Task<IEnumerable<PaymentCalendar>> GetPaymentCalendarsAsync();
    Task<IEnumerable<CompoundingMethod>> GetCompoundingMethodsAsync();
    Task<IEnumerable<AveragingMethod>> GetAveragingMethodsAsync();
    Task<IEnumerable<LegType>> GetLegTypesAsync();
    Task<IEnumerable<PayerReceiverType>> GetPayerReceiverTypesAsync();
    Task SeedConfigurationDataAsync();
}

public class ConfigurationRepository : IConfigurationRepository
{
    private readonly IDocumentStore _documentStore;

    public ConfigurationRepository(IDocumentStore documentStore)
    {
        _documentStore = documentStore;
    }

    public async Task<IEnumerable<InstrumentType>> GetInstrumentTypesAsync()
    {
        await using var session = _documentStore.QuerySession();
        return await session.Query<InstrumentType>().ToListAsync();
    }

    public async Task<IEnumerable<Frequency>> GetFrequenciesAsync()
    {
        await using var session = _documentStore.QuerySession();
        return await session.Query<Frequency>().ToListAsync();
    }

    public async Task<IEnumerable<DayCountConvention>> GetDayCountConventionsAsync()
    {
        await using var session = _documentStore.QuerySession();
        return await session.Query<DayCountConvention>().ToListAsync();
    }

    public async Task<IEnumerable<BusinessDayConvention>> GetBusinessDayConventionsAsync()
    {
        await using var session = _documentStore.QuerySession();
        return await session.Query<BusinessDayConvention>().ToListAsync();
    }

    public async Task<IEnumerable<ReferenceRate>> GetReferenceRatesAsync()
    {
        await using var session = _documentStore.QuerySession();
        return await session.Query<ReferenceRate>().ToListAsync();
    }

    public async Task<IEnumerable<PaymentCalendar>> GetPaymentCalendarsAsync()
    {
        await using var session = _documentStore.QuerySession();
        return await session.Query<PaymentCalendar>().ToListAsync();
    }

    public async Task<IEnumerable<CompoundingMethod>> GetCompoundingMethodsAsync()
    {
        await using var session = _documentStore.QuerySession();
        return await session.Query<CompoundingMethod>().ToListAsync();
    }

    public async Task<IEnumerable<AveragingMethod>> GetAveragingMethodsAsync()
    {
        await using var session = _documentStore.QuerySession();
        return await session.Query<AveragingMethod>().ToListAsync();
    }

    public async Task<IEnumerable<LegType>> GetLegTypesAsync()
    {
        await using var session = _documentStore.QuerySession();
        return await session.Query<LegType>().ToListAsync();
    }

    public async Task<IEnumerable<PayerReceiverType>> GetPayerReceiverTypesAsync()
    {
        await using var session = _documentStore.QuerySession();
        return await session.Query<PayerReceiverType>().ToListAsync();
    }

    public async Task SeedConfigurationDataAsync()
    {
        await using var session = _documentStore.LightweightSession();

        // Check if already seeded
        var existingTypes = await session.Query<InstrumentType>().AnyAsync();
        if (existingTypes) return;

        // Instrument Types
        var instrumentTypes = new[]
        {
            new InstrumentType { Name = "Swap", Description = "Interest Rate Swap" },
            new InstrumentType { Name = "FX", Description = "Foreign Exchange" },
            new InstrumentType { Name = "FXOption", Description = "FX Option" }
        };
        session.Store(instrumentTypes);

        // Frequencies
        var frequencies = new[]
        {
            new Frequency { Name = "Daily", Code = "D" },
            new Frequency { Name = "Weekly", Code = "W" },
            new Frequency { Name = "Monthly", Code = "M" },
            new Frequency { Name = "Quarterly", Code = "Q" },
            new Frequency { Name = "Semi-Annual", Code = "S" },
            new Frequency { Name = "Annual", Code = "A" }
        };
        session.Store(frequencies);

        // Day Count Conventions
        var dayCountConventions = new[]
        {
            new DayCountConvention { Name = "30/360", Code = "30/360", Description = "30/360 day count" },
            new DayCountConvention { Name = "ACT/360", Code = "ACT/360", Description = "Actual/360" },
            new DayCountConvention { Name = "ACT/365", Code = "ACT/365", Description = "Actual/365" },
            new DayCountConvention { Name = "ACT/ACT", Code = "ACT/ACT", Description = "Actual/Actual" }
        };
        session.Store(dayCountConventions);

        // Business Day Conventions
        var businessDayConventions = new[]
        {
            new BusinessDayConvention { Name = "Following", Code = "FOLLOWING", Description = "Following business day" },
            new BusinessDayConvention { Name = "Modified Following", Code = "MODFOLLOWING", Description = "Modified following business day" },
            new BusinessDayConvention { Name = "Preceding", Code = "PRECEDING", Description = "Preceding business day" },
            new BusinessDayConvention { Name = "Unadjusted", Code = "UNADJUSTED", Description = "No adjustment" }
        };
        session.Store(businessDayConventions);

        // Reference Rates
        var referenceRates = new[]
        {
            new ReferenceRate { Name = "SOFR", Code = "SOFR", Currency = "USD", Description = "Secured Overnight Financing Rate" },
            new ReferenceRate { Name = "LIBOR USD 3M", Code = "USD-LIBOR-3M", Currency = "USD", Description = "USD LIBOR 3 Month" },
            new ReferenceRate { Name = "EURIBOR 3M", Code = "EUR-EURIBOR-3M", Currency = "EUR", Description = "EURIBOR 3 Month" },
            new ReferenceRate { Name = "SONIA", Code = "SONIA", Currency = "GBP", Description = "Sterling Overnight Index Average" },
            new ReferenceRate { Name = "TONAR", Code = "TONAR", Currency = "JPY", Description = "Tokyo Overnight Average Rate" }
        };
        session.Store(referenceRates);

        // Payment Calendars
        var paymentCalendars = new[]
        {
            new PaymentCalendar { Name = "New York", Code = "NYC", Description = "New York business days" },
            new PaymentCalendar { Name = "London", Code = "LON", Description = "London business days" },
            new PaymentCalendar { Name = "TARGET", Code = "TGT", Description = "TARGET (European) calendar" },
            new PaymentCalendar { Name = "Tokyo", Code = "TKO", Description = "Tokyo business days" }
        };
        session.Store(paymentCalendars);

        // Compounding Methods
        var compoundingMethods = new[]
        {
            new CompoundingMethod { Name = "None", Code = "NONE", Description = "No compounding" },
            new CompoundingMethod { Name = "Flat", Code = "FLAT", Description = "Flat compounding" },
            new CompoundingMethod { Name = "Straight", Code = "STRAIGHT", Description = "Straight compounding" },
            new CompoundingMethod { Name = "Spread Exclusive", Code = "SPREAD_EXCLUSIVE", Description = "Compounding excluding spread" }
        };
        session.Store(compoundingMethods);

        // Averaging Methods
        var averagingMethods = new[]
        {
            new AveragingMethod { Name = "None", Code = "NONE", Description = "No averaging" },
            new AveragingMethod { Name = "Weighted", Code = "WEIGHTED", Description = "Weighted average" },
            new AveragingMethod { Name = "Unweighted", Code = "UNWEIGHTED", Description = "Unweighted average" }
        };
        session.Store(averagingMethods);

        // Leg Types
        var legTypes = new[]
        {
            new LegType { Name = "Fixed", Code = "FIXED" },
            new LegType { Name = "Floating", Code = "FLOATING" }
        };
        session.Store(legTypes);

        // Payer Receiver Types
        var payerReceiverTypes = new[]
        {
            new PayerReceiverType { Name = "Pay", Code = "PAY" },
            new PayerReceiverType { Name = "Receive", Code = "RECEIVE" }
        };
        session.Store(payerReceiverTypes);

        await session.SaveChangesAsync();
    }
}
