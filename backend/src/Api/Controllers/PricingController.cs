using Microsoft.AspNetCore.Mvc;
using Models;
using Models.Aggregates;
using Models.ReadModels;
using Persistence;
using Pricing;

namespace Api.Controllers;

[ApiController]
[Route("[controller]")]
public class PricingController : ControllerBase
{
    private readonly ITradeRepository _tradeRepository;
    private readonly IPricingService _pricingService;
    private readonly ILogger<PricingController> _logger;

    public PricingController(
        ITradeRepository tradeRepository,
        IPricingService pricingService,
        ILogger<PricingController> logger)
    {
        _tradeRepository = tradeRepository;
        _pricingService = pricingService;
        _logger = logger;
    }

    [HttpPost("{tradeId}")]
    public async Task<ActionResult<PricingResult>> PriceTrade(string tradeId, [FromBody] PricingRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new ErrorResponse
            {
                Code = "VALIDATION_ERROR",
                Message = "Invalid pricing request"
            });
        }

        try
        {
            var tradeAggregate = await _tradeRepository.GetTradeByIdAsync(tradeId);
            if (tradeAggregate == null)
            {
                return NotFound(new ErrorResponse
                {
                    Code = "NOT_FOUND",
                    Message = $"Trade with ID {tradeId} not found"
                });
            }

            var trade = MapToSwapTrade(tradeAggregate);
            var pricingResult = await _pricingService.PriceTradeAsync(trade, request.Seed);
            
            // Store pricing event
            await _tradeRepository.PriceTradeAsync(tradeId, pricingResult.Price);
            
            return Ok(pricingResult);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error pricing trade {TradeId}", tradeId);
            return StatusCode(500, new ErrorResponse
            {
                Code = "INTERNAL_ERROR",
                Message = "An error occurred while pricing the trade"
            });
        }
    }

    [HttpPost("bulk")]
    public async Task<ActionResult<BulkPricingResult>> PriceTrades([FromBody] BulkPricingRequest request)
    {
        if (!ModelState.IsValid || request.TradeIds == null || !request.TradeIds.Any())
        {
            return BadRequest(new ErrorResponse
            {
                Code = "VALIDATION_ERROR",
                Message = "At least one trade ID is required"
            });
        }

        try
        {
            var results = new List<TradePricingResult>();
            var seed = request.Seed ?? new Random().Next(1000, 9999);

            foreach (var tradeId in request.TradeIds)
            {
                var tradeAggregate = await _tradeRepository.GetTradeByIdAsync(tradeId);
                if (tradeAggregate != null)
                {
                    var trade = MapToSwapTrade(tradeAggregate);
                    var pricingResult = await _pricingService.PriceTradeAsync(trade, seed);
                    
                    // Store pricing event
                    await _tradeRepository.PriceTradeAsync(tradeId, pricingResult.Price);

                    results.Add(new TradePricingResult
                    {
                        TradeId = tradeId,
                        Price = pricingResult.Price,
                        Currency = pricingResult.Currency
                    });
                }
            }

            return Ok(new BulkPricingResult { Results = results });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error pricing trades");
            return StatusCode(500, new ErrorResponse
            {
                Code = "INTERNAL_ERROR",
                Message = "An error occurred while pricing trades"
            });
        }
    }
    
    private SwapTrade MapToSwapTrade(TradeReadModel trade)
    {
        return new SwapTrade
        {
            Id = trade.Id,
            Counterparty = trade.Counterparty,
            EffectiveDate = trade.EffectiveDate,
            MaturityDate = trade.MaturityDate,
            NotionalAmount = trade.NotionalAmount,
            NotionalCurrency = trade.NotionalCurrency,
            TradeDate = trade.TradeDate,
            BookedBy = trade.BookedBy,
            Npv = trade.Npv,
            CreatedAt = trade.CreatedAt
        };
    }
}
