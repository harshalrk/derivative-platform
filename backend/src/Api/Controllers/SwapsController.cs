using Microsoft.AspNetCore.Mvc;
using Models;
using Models.Aggregates;
using Models.ReadModels;
using Persistence;
using System.Text;
using System.Text.Json;
using RabbitMQ.Client;

namespace Api.Controllers;

[ApiController]
[Route("[controller]")]
public class SwapsController : ControllerBase
{
    private readonly ITradeRepository _tradeRepository;
    private readonly ILogger<SwapsController> _logger;

    public SwapsController(
        ITradeRepository tradeRepository,
        ILogger<SwapsController> logger)
    {
        _tradeRepository = tradeRepository;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<SwapTradeResponse>>> GetSwaps([FromQuery] string bookedBy)
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
            var swaps = await _tradeRepository.GetTradesByUserAsync(bookedBy);
            var responses = swaps.Select(MapToResponse);
            return Ok(responses);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving swaps for user {BookedBy}", bookedBy);
            return StatusCode(500, new ErrorResponse
            {
                Code = "INTERNAL_ERROR",
                Message = "An error occurred while retrieving swaps"
            });
        }
    }

    [HttpPost]
    public async Task<ActionResult<SwapTradeResponse>> CreateSwap([FromBody] SwapTradeCreateRequest request, [FromQuery] string bookedBy)
    {
        if (string.IsNullOrWhiteSpace(bookedBy))
        {
            return BadRequest(new ErrorResponse
            {
                Code = "MISSING_PARAMETER",
                Message = "bookedBy parameter is required"
            });
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(new ErrorResponse
            {
                Code = "VALIDATION_ERROR",
                Message = string.Join("; ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage))
            });
        }

        try
        {
            var swap = new SwapTrade
            {
                TradeDate = request.TradeDate,
                BookedBy = bookedBy,
                Counterparty = request.Counterparty,
                EffectiveDate = request.EffectiveDate,
                MaturityDate = request.MaturityDate,
                NotionalAmount = request.NotionalAmount,
                NotionalCurrency = request.NotionalCurrency,
                Leg1 = new SwapLeg
                {
                    LegType = request.Leg1.LegType,
                    PayerReceiver = request.Leg1.PayerReceiver,
                    FixedRate = request.Leg1.FixedRate,
                    ReferenceRate = request.Leg1.ReferenceRate,
                    Spread = request.Leg1.Spread,
                    ResetFrequency = request.Leg1.ResetFrequency,
                    PaymentFrequency = request.Leg1.PaymentFrequency,
                    DayCountConvention = request.Leg1.DayCountConvention,
                    BusinessDayConvention = request.Leg1.BusinessDayConvention,
                    PaymentCalendar = request.Leg1.PaymentCalendar,
                    CompoundingMethod = request.Leg1.CompoundingMethod,
                    CompoundingFrequency = request.Leg1.CompoundingFrequency,
                    AveragingMethod = request.Leg1.AveragingMethod,
                    AveragingFrequency = request.Leg1.AveragingFrequency
                },
                Leg2 = new SwapLeg
                {
                    LegType = request.Leg2.LegType,
                    PayerReceiver = request.Leg2.PayerReceiver,
                    FixedRate = request.Leg2.FixedRate,
                    ReferenceRate = request.Leg2.ReferenceRate,
                    Spread = request.Leg2.Spread,
                    ResetFrequency = request.Leg2.ResetFrequency,
                    PaymentFrequency = request.Leg2.PaymentFrequency,
                    DayCountConvention = request.Leg2.DayCountConvention,
                    BusinessDayConvention = request.Leg2.BusinessDayConvention,
                    PaymentCalendar = request.Leg2.PaymentCalendar,
                    CompoundingMethod = request.Leg2.CompoundingMethod,
                    CompoundingFrequency = request.Leg2.CompoundingFrequency,
                    AveragingMethod = request.Leg2.AveragingMethod,
                    AveragingFrequency = request.Leg2.AveragingFrequency
                }
            };

            var swapId = await _tradeRepository.CreateTradeAsync(swap);
            _logger.LogInformation("Created trade with ID: {SwapId}", swapId);
            
            // Build response directly from the data we just created - no need to wait for async projection
            var createdSwap = new TradeReadModel
            {
                Id = swapId,
                Counterparty = swap.Counterparty,
                EffectiveDate = swap.EffectiveDate,
                MaturityDate = swap.MaturityDate,
                NotionalAmount = swap.NotionalAmount,
                NotionalCurrency = swap.NotionalCurrency,
                TradeDate = swap.TradeDate,
                BookedBy = swap.BookedBy,
                IsCancelled = false,
                CreatedAt = DateTime.UtcNow,
                Leg1 = MapToSwapLegData(swap.Leg1),
                Leg2 = MapToSwapLegData(swap.Leg2)
            };
            
            // Publish trade creation event to RabbitMQ
            try
            {
                PublishTradeEvent(createdSwap, "created");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to publish trade creation event for {TradeId}", swapId);
                // Don't fail the request if event publishing fails
            }
            
            var response = MapToResponse(createdSwap);
            return CreatedAtAction(nameof(CreateSwap), new { id = swapId }, response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating swap trade");
            return StatusCode(500, new ErrorResponse
            {
                Code = "INTERNAL_ERROR",
                Message = "An error occurred while creating the swap trade"
            });
        }
    }
    
    [HttpPut("{id}")]
    public async Task<ActionResult<SwapTradeResponse>> UpdateSwap(string id, [FromBody] SwapTradeCreateRequest request, [FromQuery] string bookedBy)
    {
        if (string.IsNullOrWhiteSpace(bookedBy))
        {
            return BadRequest(new ErrorResponse
            {
                Code = "MISSING_PARAMETER",
                Message = "bookedBy parameter is required"
            });
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(new ErrorResponse
            {
                Code = "VALIDATION_ERROR",
                Message = string.Join("; ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage))
            });
        }

        try
        {
            var updatedSwap = new SwapTrade
            {
                Id = id,
                TradeDate = request.TradeDate,
                BookedBy = bookedBy,
                Counterparty = request.Counterparty,
                EffectiveDate = request.EffectiveDate,
                MaturityDate = request.MaturityDate,
                NotionalAmount = request.NotionalAmount,
                NotionalCurrency = request.NotionalCurrency,
                Leg1 = new SwapLeg
                {
                    LegType = request.Leg1.LegType,
                    PayerReceiver = request.Leg1.PayerReceiver,
                    FixedRate = request.Leg1.FixedRate,
                    ReferenceRate = request.Leg1.ReferenceRate,
                    Spread = request.Leg1.Spread,
                    ResetFrequency = request.Leg1.ResetFrequency,
                    PaymentFrequency = request.Leg1.PaymentFrequency,
                    DayCountConvention = request.Leg1.DayCountConvention,
                    BusinessDayConvention = request.Leg1.BusinessDayConvention,
                    PaymentCalendar = request.Leg1.PaymentCalendar,
                    CompoundingMethod = request.Leg1.CompoundingMethod,
                    CompoundingFrequency = request.Leg1.CompoundingFrequency,
                    AveragingMethod = request.Leg1.AveragingMethod,
                    AveragingFrequency = request.Leg1.AveragingFrequency
                },
                Leg2 = new SwapLeg
                {
                    LegType = request.Leg2.LegType,
                    PayerReceiver = request.Leg2.PayerReceiver,
                    FixedRate = request.Leg2.FixedRate,
                    ReferenceRate = request.Leg2.ReferenceRate,
                    Spread = request.Leg2.Spread,
                    ResetFrequency = request.Leg2.ResetFrequency,
                    PaymentFrequency = request.Leg2.PaymentFrequency,
                    DayCountConvention = request.Leg2.DayCountConvention,
                    BusinessDayConvention = request.Leg2.BusinessDayConvention,
                    PaymentCalendar = request.Leg2.PaymentCalendar,
                    CompoundingMethod = request.Leg2.CompoundingMethod,
                    CompoundingFrequency = request.Leg2.CompoundingFrequency,
                    AveragingMethod = request.Leg2.AveragingMethod,
                    AveragingFrequency = request.Leg2.AveragingFrequency
                }
            };

            var success = await _tradeRepository.UpdateTradeAsync(id, updatedSwap);
            
            if (!success)
            {
                return NotFound(new ErrorResponse
                {
                    Code = "NOT_FOUND",
                    Message = $"Swap trade with ID {id} not found"
                });
            }
            
            var updatedAggregate = await _tradeRepository.GetTradeByIdAsync(id);
            if (updatedAggregate == null)
            {
                return StatusCode(500, new ErrorResponse
                {
                    Code = "INTERNAL_ERROR",
                    Message = "Swap updated but could not be retrieved"
                });
            }
            
            // Publish trade update event to RabbitMQ
            try
            {
                PublishTradeEvent(updatedAggregate, "updated");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to publish trade update event for {TradeId}", id);
                // Don't fail the request if event publishing fails
            }
            
            var response = MapToResponse(updatedAggregate);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating swap trade {TradeId}", id);
            return StatusCode(500, new ErrorResponse
            {
                Code = "INTERNAL_ERROR",
                Message = "An error occurred while updating the swap trade"
            });
        }
    }
    
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteSwap(string id, [FromQuery] string bookedBy)
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
            var trade = await _tradeRepository.GetTradeByIdAsync(id);
            if (trade == null)
            {
                return NotFound(new ErrorResponse
                {
                    Code = "NOT_FOUND",
                    Message = $"Swap trade with ID {id} not found"
                });
            }

            var success = await _tradeRepository.CancelTradeAsync(id, "DELETED");
            
            if (!success)
            {
                return StatusCode(500, new ErrorResponse
                {
                    Code = "INTERNAL_ERROR",
                    Message = "Failed to delete the swap trade"
                });
            }

            _logger.LogInformation("Deleted swap trade {TradeId} by user {BookedBy}", id, bookedBy);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting swap trade {TradeId}", id);
            return StatusCode(500, new ErrorResponse
            {
                Code = "INTERNAL_ERROR",
                Message = "An error occurred while deleting the swap trade"
            });
        }
    }
    
    private void PublishTradeEvent(TradeReadModel aggregate, string eventType)
    {
        var factory = new ConnectionFactory { Uri = new Uri("amqp://trader:trader123@rabbitmq:5672") };
        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();
        
        var queueName = $"trade-{eventType}";
        channel.QueueDeclare(
            queue: queueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null);
        
        var tradeData = new
        {
            id = aggregate.Id,
            productType = "Swap",
            counterparty = aggregate.Counterparty,
            tradeDate = aggregate.TradeDate,
            bookedBy = aggregate.BookedBy,
            npv = aggregate.Npv,
            createdAt = aggregate.CreatedAt
        };
        
        var message = JsonSerializer.Serialize(tradeData);
        var body = Encoding.UTF8.GetBytes(message);
        
        channel.BasicPublish(
            exchange: string.Empty,
            routingKey: queueName,
            basicProperties: null,
            body: body);
        
        _logger.LogInformation("Published trade {EventType} event for {TradeId}", eventType, aggregate.Id);
    }
    
    private SwapTradeResponse MapToResponse(TradeReadModel trade)
    {
        return new SwapTradeResponse
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
            CreatedAt = trade.CreatedAt,
            Leg1 = trade.Leg1,
            Leg2 = trade.Leg2
        };
    }
    
    private Models.Events.SwapLegData MapToSwapLegData(SwapLeg leg)
    {
        return new Models.Events.SwapLegData
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

public class SwapTradeResponse
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
    public DateTime CreatedAt { get; set; }
    public Models.Events.SwapLegData? Leg1 { get; set; }
    public Models.Events.SwapLegData? Leg2 { get; set; }
}
