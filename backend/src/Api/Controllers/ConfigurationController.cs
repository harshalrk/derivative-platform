using Microsoft.AspNetCore.Mvc;
using Models;
using Persistence;

namespace Api.Controllers;

[ApiController]
[Route("[controller]")]
public class ConfigurationController : ControllerBase
{
    private readonly IConfigurationRepository _configRepository;
    private readonly ILogger<ConfigurationController> _logger;

    public ConfigurationController(
        IConfigurationRepository configRepository,
        ILogger<ConfigurationController> logger)
    {
        _configRepository = configRepository;
        _logger = logger;
    }

    [HttpGet("instrument-types")]
    public async Task<ActionResult<IEnumerable<InstrumentType>>> GetInstrumentTypes()
    {
        try
        {
            var types = await _configRepository.GetInstrumentTypesAsync();
            return Ok(types);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving instrument types");
            return StatusCode(500, new ErrorResponse
            {
                Code = "INTERNAL_ERROR",
                Message = "An error occurred while retrieving instrument types"
            });
        }
    }

    [HttpGet("frequencies")]
    public async Task<ActionResult<IEnumerable<Frequency>>> GetFrequencies()
    {
        try
        {
            var frequencies = await _configRepository.GetFrequenciesAsync();
            return Ok(frequencies);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving frequencies");
            return StatusCode(500, new ErrorResponse
            {
                Code = "INTERNAL_ERROR",
                Message = "An error occurred while retrieving frequencies"
            });
        }
    }

    [HttpGet("day-count-conventions")]
    public async Task<ActionResult<IEnumerable<DayCountConvention>>> GetDayCountConventions()
    {
        try
        {
            var conventions = await _configRepository.GetDayCountConventionsAsync();
            return Ok(conventions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving day count conventions");
            return StatusCode(500, new ErrorResponse
            {
                Code = "INTERNAL_ERROR",
                Message = "An error occurred while retrieving day count conventions"
            });
        }
    }

    [HttpGet("business-day-conventions")]
    public async Task<ActionResult<IEnumerable<BusinessDayConvention>>> GetBusinessDayConventions()
    {
        try
        {
            var conventions = await _configRepository.GetBusinessDayConventionsAsync();
            return Ok(conventions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving business day conventions");
            return StatusCode(500, new ErrorResponse
            {
                Code = "INTERNAL_ERROR",
                Message = "An error occurred while retrieving business day conventions"
            });
        }
    }

    [HttpGet("reference-rates")]
    public async Task<ActionResult<IEnumerable<ReferenceRate>>> GetReferenceRates()
    {
        try
        {
            var rates = await _configRepository.GetReferenceRatesAsync();
            return Ok(rates);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving reference rates");
            return StatusCode(500, new ErrorResponse
            {
                Code = "INTERNAL_ERROR",
                Message = "An error occurred while retrieving reference rates"
            });
        }
    }

    [HttpGet("payment-calendars")]
    public async Task<ActionResult<IEnumerable<PaymentCalendar>>> GetPaymentCalendars()
    {
        try
        {
            var calendars = await _configRepository.GetPaymentCalendarsAsync();
            return Ok(calendars);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving payment calendars");
            return StatusCode(500, new ErrorResponse
            {
                Code = "INTERNAL_ERROR",
                Message = "An error occurred while retrieving payment calendars"
            });
        }
    }

    [HttpGet("compounding-methods")]
    public async Task<ActionResult<IEnumerable<CompoundingMethod>>> GetCompoundingMethods()
    {
        try
        {
            var methods = await _configRepository.GetCompoundingMethodsAsync();
            return Ok(methods);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving compounding methods");
            return StatusCode(500, new ErrorResponse
            {
                Code = "INTERNAL_ERROR",
                Message = "An error occurred while retrieving compounding methods"
            });
        }
    }

    [HttpGet("averaging-methods")]
    public async Task<ActionResult<IEnumerable<AveragingMethod>>> GetAveragingMethods()
    {
        try
        {
            var methods = await _configRepository.GetAveragingMethodsAsync();
            return Ok(methods);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving averaging methods");
            return StatusCode(500, new ErrorResponse
            {
                Code = "INTERNAL_ERROR",
                Message = "An error occurred while retrieving averaging methods"
            });
        }
    }

    [HttpGet("leg-types")]
    public async Task<ActionResult<IEnumerable<LegType>>> GetLegTypes()
    {
        try
        {
            var types = await _configRepository.GetLegTypesAsync();
            return Ok(types);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving leg types");
            return StatusCode(500, new ErrorResponse
            {
                Code = "INTERNAL_ERROR",
                Message = "An error occurred while retrieving leg types"
            });
        }
    }

    [HttpGet("payer-receiver-types")]
    public async Task<ActionResult<IEnumerable<PayerReceiverType>>> GetPayerReceiverTypes()
    {
        try
        {
            var types = await _configRepository.GetPayerReceiverTypesAsync();
            return Ok(types);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving payer/receiver types");
            return StatusCode(500, new ErrorResponse
            {
                Code = "INTERNAL_ERROR",
                Message = "An error occurred while retrieving payer/receiver types"
            });
        }
    }

    [HttpPost("seed")]
    public async Task<ActionResult> SeedConfigurationData()
    {
        try
        {
            await _configRepository.SeedConfigurationDataAsync();
            return Ok(new { Message = "Configuration data seeded successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error seeding configuration data");
            return StatusCode(500, new ErrorResponse
            {
                Code = "INTERNAL_ERROR",
                Message = "An error occurred while seeding configuration data"
            });
        }
    }
}
