using Microsoft.AspNetCore.Mvc;
using Models;
using Models.ReadModels;
using Persistence;

namespace Api.Controllers;

[ApiController]
[Route("[controller]")]
public class TradesController : ControllerBase
{
    private readonly ITradeRepository _tradeRepository;
    private readonly ILogger<TradesController> _logger;

    public TradesController(
        ITradeRepository tradeRepository,
        ILogger<TradesController> logger)
    {
        _tradeRepository = tradeRepository;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TradeResponse>>> GetTrades([FromQuery] string bookedBy)
    {
        if (string.IsNullOrWhiteSpace(bookedBy))
        {
            return BadRequest(new ErrorResponse
            {
                Code = "MISSING_PARAMETER",
                Message = "bookedBy parameter is required"
            });
        }

        try
        {
            var trades = await _tradeRepository.GetTradesByUserAsync(bookedBy);
            var responses = trades.Select(MapToCommonResponse);
            return Ok(responses);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving trades for user {BookedBy}", bookedBy);
            return StatusCode(500, new ErrorResponse
            {
                Code = "INTERNAL_ERROR",
                Message = "An error occurred while retrieving trades"
            });
        }
    }
    
    private TradeResponse MapToCommonResponse(TradeReadModel trade)
    {
        return new TradeResponse
        {
            Id = trade.Id,
            ProductType = "Swap",
            Counterparty = trade.Counterparty,
            TradeDate = trade.TradeDate,
            BookedBy = trade.BookedBy,
            Npv = trade.Npv,
            CreatedAt = trade.CreatedAt
        };
    }
}

public class TradeResponse
{
    public string Id { get; set; } = string.Empty;
    public string ProductType { get; set; } = string.Empty;
    public string Counterparty { get; set; } = string.Empty;
    public DateTime TradeDate { get; set; }
    public string BookedBy { get; set; } = string.Empty;
    public decimal? Npv { get; set; }
    public DateTime CreatedAt { get; set; }
}
