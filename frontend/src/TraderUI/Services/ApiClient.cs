using TraderUI.Models;
using System.Text;
using System.Text.Json;

namespace TraderUI.Services;

public class ApiClient
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions;

    public ApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }

    public async Task<ApiResponse<SessionResponse>> SetSessionAsync(string name)
    {
        try
        {
            var request = new { Name = name };
            var json = JsonSerializer.Serialize(request, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            var response = await _httpClient.PostAsync("/session", content);
            var responseContent = await response.Content.ReadAsStringAsync();
            
            if (response.IsSuccessStatusCode)
            {
                var result = JsonSerializer.Deserialize<SessionResponse>(responseContent, _jsonOptions);
                return ApiResponse<SessionResponse>.Success(result!);
            }
            
            var error = JsonSerializer.Deserialize<ErrorResponse>(responseContent, _jsonOptions);
            return ApiResponse<SessionResponse>.Error(error?.Message ?? "Unknown error");
        }
        catch (Exception ex)
        {
            return ApiResponse<SessionResponse>.Error($"Network error: {ex.Message}");
        }
    }

    public async Task<ApiResponse<IEnumerable<SessionResponse>>> GetSessionsAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("/session");
            var responseContent = await response.Content.ReadAsStringAsync();
            
            if (response.IsSuccessStatusCode)
            {
                var result = JsonSerializer.Deserialize<IEnumerable<SessionResponse>>(responseContent, _jsonOptions);
                return ApiResponse<IEnumerable<SessionResponse>>.Success(result!);
            }
            
            var error = JsonSerializer.Deserialize<ErrorResponse>(responseContent, _jsonOptions);
            return ApiResponse<IEnumerable<SessionResponse>>.Error(error?.Message ?? "Unknown error");
        }
        catch (Exception ex)
        {
            return ApiResponse<IEnumerable<SessionResponse>>.Error($"Network error: {ex.Message}");
        }
    }

    public async Task<ApiResponse<bool>> DeleteSwapAsync(string tradeId, string bookedBy)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"/swaps/{Uri.EscapeDataString(tradeId)}?bookedBy={Uri.EscapeDataString(bookedBy)}");
            
            if (response.IsSuccessStatusCode)
            {
                return ApiResponse<bool>.Success(true);
            }
            
            var responseContent = await response.Content.ReadAsStringAsync();
            var error = JsonSerializer.Deserialize<ErrorResponse>(responseContent, _jsonOptions);
            return ApiResponse<bool>.Error(error?.Message ?? "Failed to delete trade");
        }
        catch (Exception ex)
        {
            return ApiResponse<bool>.Error($"Network error: {ex.Message}");
        }
    }

    public async Task<ApiResponse<bool>> DeleteSessionAsync(string sessionId)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"/session/{Uri.EscapeDataString(sessionId)}");
            
            if (response.IsSuccessStatusCode)
            {
                return ApiResponse<bool>.Success(true);
            }
            
            var responseContent = await response.Content.ReadAsStringAsync();
            var error = JsonSerializer.Deserialize<ErrorResponse>(responseContent, _jsonOptions);
            return ApiResponse<bool>.Error(error?.Message ?? "Unknown error");
        }
        catch (Exception ex)
        {
            return ApiResponse<bool>.Error($"Network error: {ex.Message}");
        }
    }

    public async Task<ApiResponse<IEnumerable<TradeResponse>>> GetTradesAsync(string bookedBy)
    {
        try
        {
            Console.WriteLine($"[ApiClient] GetTradesAsync called for bookedBy: {bookedBy}");
            var response = await _httpClient.GetAsync($"/trades?bookedBy={Uri.EscapeDataString(bookedBy)}");
            var responseContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"[ApiClient] Response status: {response.StatusCode}");
            Console.WriteLine($"[ApiClient] Response content: {responseContent}");
            
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("[ApiClient] Attempting to deserialize to IEnumerable<TradeResponse>");
                var result = JsonSerializer.Deserialize<IEnumerable<TradeResponse>>(responseContent, _jsonOptions);
                Console.WriteLine($"[ApiClient] Deserialization successful, count: {result?.Count() ?? 0}");
                return ApiResponse<IEnumerable<TradeResponse>>.Success(result!);
            }
            
            var error = JsonSerializer.Deserialize<ErrorResponse>(responseContent, _jsonOptions);
            return ApiResponse<IEnumerable<TradeResponse>>.Error(error?.Message ?? "Unknown error");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ApiClient] Exception in GetTradesAsync: {ex.GetType().Name} - {ex.Message}");
            Console.WriteLine($"[ApiClient] Stack trace: {ex.StackTrace}");
            return ApiResponse<IEnumerable<TradeResponse>>.Error($"Network error: {ex.Message}");
        }
    }

    public async Task<ApiResponse<IEnumerable<SwapTrade>>> GetSwapsAsync(string bookedBy)
    {
        try
        {
            var response = await _httpClient.GetAsync($"/swaps?bookedBy={Uri.EscapeDataString(bookedBy)}");
            var responseContent = await response.Content.ReadAsStringAsync();
            
            if (response.IsSuccessStatusCode)
            {
                var result = JsonSerializer.Deserialize<IEnumerable<SwapTrade>>(responseContent, _jsonOptions);
                return ApiResponse<IEnumerable<SwapTrade>>.Success(result!);
            }
            
            var error = JsonSerializer.Deserialize<ErrorResponse>(responseContent, _jsonOptions);
            return ApiResponse<IEnumerable<SwapTrade>>.Error(error?.Message ?? "Unknown error");
        }
        catch (Exception ex)
        {
            return ApiResponse<IEnumerable<SwapTrade>>.Error($"Network error: {ex.Message}");
        }
    }

    public async Task<ApiResponse<PricingResult>> PriceTradeAsync(string tradeId, int seed)
    {
        try
        {
            var request = new { TradeId = tradeId, Seed = seed };
            var json = JsonSerializer.Serialize(request, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            var response = await _httpClient.PostAsync($"/pricing/{tradeId}", content);
            var responseContent = await response.Content.ReadAsStringAsync();
            
            if (response.IsSuccessStatusCode)
            {
                var result = JsonSerializer.Deserialize<PricingResult>(responseContent, _jsonOptions);
                return ApiResponse<PricingResult>.Success(result!);
            }
            
            var error = JsonSerializer.Deserialize<ErrorResponse>(responseContent, _jsonOptions);
            return ApiResponse<PricingResult>.Error(error?.Message ?? "Unknown error");
        }
        catch (Exception ex)
        {
            return ApiResponse<PricingResult>.Error($"Network error: {ex.Message}");
        }
    }

    public async Task<ApiResponse<BulkPricingResult>> PriceTradesAsync(List<string> tradeIds, int? seed = null)
    {
        try
        {
            var request = new { TradeIds = tradeIds, Seed = seed };
            var json = JsonSerializer.Serialize(request, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            var response = await _httpClient.PostAsync("/pricing/bulk", content);
            var responseContent = await response.Content.ReadAsStringAsync();
            
            if (response.IsSuccessStatusCode)
            {
                var result = JsonSerializer.Deserialize<BulkPricingResult>(responseContent, _jsonOptions);
                return ApiResponse<BulkPricingResult>.Success(result!);
            }
            
            var error = JsonSerializer.Deserialize<ErrorResponse>(responseContent, _jsonOptions);
            return ApiResponse<BulkPricingResult>.Error(error?.Message ?? "Unknown error");
        }
        catch (Exception ex)
        {
            return ApiResponse<BulkPricingResult>.Error($"Network error: {ex.Message}");
        }
    }

    // Configuration endpoints
    public async Task<ApiResponse<IEnumerable<InstrumentType>>> GetInstrumentTypesAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("/configuration/instrument-types");
            var responseContent = await response.Content.ReadAsStringAsync();
            
            if (response.IsSuccessStatusCode)
            {
                var result = JsonSerializer.Deserialize<IEnumerable<InstrumentType>>(responseContent, _jsonOptions);
                return ApiResponse<IEnumerable<InstrumentType>>.Success(result!);
            }
            
            var error = JsonSerializer.Deserialize<ErrorResponse>(responseContent, _jsonOptions);
            return ApiResponse<IEnumerable<InstrumentType>>.Error(error?.Message ?? "Unknown error");
        }
        catch (Exception ex)
        {
            return ApiResponse<IEnumerable<InstrumentType>>.Error($"Network error: {ex.Message}");
        }
    }

    public async Task<ApiResponse<IEnumerable<Frequency>>> GetFrequenciesAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("/configuration/frequencies");
            var responseContent = await response.Content.ReadAsStringAsync();
            
            if (response.IsSuccessStatusCode)
            {
                var result = JsonSerializer.Deserialize<IEnumerable<Frequency>>(responseContent, _jsonOptions);
                return ApiResponse<IEnumerable<Frequency>>.Success(result!);
            }
            
            var error = JsonSerializer.Deserialize<ErrorResponse>(responseContent, _jsonOptions);
            return ApiResponse<IEnumerable<Frequency>>.Error(error?.Message ?? "Unknown error");
        }
        catch (Exception ex)
        {
            return ApiResponse<IEnumerable<Frequency>>.Error($"Network error: {ex.Message}");
        }
    }

    public async Task<ApiResponse<IEnumerable<DayCountConvention>>> GetDayCountConventionsAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("/configuration/day-count-conventions");
            var responseContent = await response.Content.ReadAsStringAsync();
            
            if (response.IsSuccessStatusCode)
            {
                var result = JsonSerializer.Deserialize<IEnumerable<DayCountConvention>>(responseContent, _jsonOptions);
                return ApiResponse<IEnumerable<DayCountConvention>>.Success(result!);
            }
            
            var error = JsonSerializer.Deserialize<ErrorResponse>(responseContent, _jsonOptions);
            return ApiResponse<IEnumerable<DayCountConvention>>.Error(error?.Message ?? "Unknown error");
        }
        catch (Exception ex)
        {
            return ApiResponse<IEnumerable<DayCountConvention>>.Error($"Network error: {ex.Message}");
        }
    }

    public async Task<ApiResponse<IEnumerable<BusinessDayConvention>>> GetBusinessDayConventionsAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("/configuration/business-day-conventions");
            var responseContent = await response.Content.ReadAsStringAsync();
            
            if (response.IsSuccessStatusCode)
            {
                var result = JsonSerializer.Deserialize<IEnumerable<BusinessDayConvention>>(responseContent, _jsonOptions);
                return ApiResponse<IEnumerable<BusinessDayConvention>>.Success(result!);
            }
            
            var error = JsonSerializer.Deserialize<ErrorResponse>(responseContent, _jsonOptions);
            return ApiResponse<IEnumerable<BusinessDayConvention>>.Error(error?.Message ?? "Unknown error");
        }
        catch (Exception ex)
        {
            return ApiResponse<IEnumerable<BusinessDayConvention>>.Error($"Network error: {ex.Message}");
        }
    }

    public async Task<ApiResponse<IEnumerable<ReferenceRate>>> GetReferenceRatesAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("/configuration/reference-rates");
            var responseContent = await response.Content.ReadAsStringAsync();
            
            if (response.IsSuccessStatusCode)
            {
                var result = JsonSerializer.Deserialize<IEnumerable<ReferenceRate>>(responseContent, _jsonOptions);
                return ApiResponse<IEnumerable<ReferenceRate>>.Success(result!);
            }
            
            var error = JsonSerializer.Deserialize<ErrorResponse>(responseContent, _jsonOptions);
            return ApiResponse<IEnumerable<ReferenceRate>>.Error(error?.Message ?? "Unknown error");
        }
        catch (Exception ex)
        {
            return ApiResponse<IEnumerable<ReferenceRate>>.Error($"Network error: {ex.Message}");
        }
    }

    public async Task<ApiResponse<IEnumerable<PaymentCalendar>>> GetPaymentCalendarsAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("/configuration/payment-calendars");
            var responseContent = await response.Content.ReadAsStringAsync();
            
            if (response.IsSuccessStatusCode)
            {
                var result = JsonSerializer.Deserialize<IEnumerable<PaymentCalendar>>(responseContent, _jsonOptions);
                return ApiResponse<IEnumerable<PaymentCalendar>>.Success(result!);
            }
            
            var error = JsonSerializer.Deserialize<ErrorResponse>(responseContent, _jsonOptions);
            return ApiResponse<IEnumerable<PaymentCalendar>>.Error(error?.Message ?? "Unknown error");
        }
        catch (Exception ex)
        {
            return ApiResponse<IEnumerable<PaymentCalendar>>.Error($"Network error: {ex.Message}");
        }
    }

    public async Task<ApiResponse<IEnumerable<CompoundingMethod>>> GetCompoundingMethodsAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("/configuration/compounding-methods");
            var responseContent = await response.Content.ReadAsStringAsync();
            
            if (response.IsSuccessStatusCode)
            {
                var result = JsonSerializer.Deserialize<IEnumerable<CompoundingMethod>>(responseContent, _jsonOptions);
                return ApiResponse<IEnumerable<CompoundingMethod>>.Success(result!);
            }
            
            var error = JsonSerializer.Deserialize<ErrorResponse>(responseContent, _jsonOptions);
            return ApiResponse<IEnumerable<CompoundingMethod>>.Error(error?.Message ?? "Unknown error");
        }
        catch (Exception ex)
        {
            return ApiResponse<IEnumerable<CompoundingMethod>>.Error($"Network error: {ex.Message}");
        }
    }

    public async Task<ApiResponse<IEnumerable<AveragingMethod>>> GetAveragingMethodsAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("/configuration/averaging-methods");
            var responseContent = await response.Content.ReadAsStringAsync();
            
            if (response.IsSuccessStatusCode)
            {
                var result = JsonSerializer.Deserialize<IEnumerable<AveragingMethod>>(responseContent, _jsonOptions);
                return ApiResponse<IEnumerable<AveragingMethod>>.Success(result!);
            }
            
            var error = JsonSerializer.Deserialize<ErrorResponse>(responseContent, _jsonOptions);
            return ApiResponse<IEnumerable<AveragingMethod>>.Error(error?.Message ?? "Unknown error");
        }
        catch (Exception ex)
        {
            return ApiResponse<IEnumerable<AveragingMethod>>.Error($"Network error: {ex.Message}");
        }
    }

    public async Task<ApiResponse<IEnumerable<LegType>>> GetLegTypesAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("/configuration/leg-types");
            var responseContent = await response.Content.ReadAsStringAsync();
            
            if (response.IsSuccessStatusCode)
            {
                var result = JsonSerializer.Deserialize<IEnumerable<LegType>>(responseContent, _jsonOptions);
                return ApiResponse<IEnumerable<LegType>>.Success(result!);
            }
            
            var error = JsonSerializer.Deserialize<ErrorResponse>(responseContent, _jsonOptions);
            return ApiResponse<IEnumerable<LegType>>.Error(error?.Message ?? "Unknown error");
        }
        catch (Exception ex)
        {
            return ApiResponse<IEnumerable<LegType>>.Error($"Network error: {ex.Message}");
        }
    }

    public async Task<ApiResponse<IEnumerable<PayerReceiverType>>> GetPayerReceiverTypesAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("/configuration/payer-receiver-types");
            var responseContent = await response.Content.ReadAsStringAsync();
            
            if (response.IsSuccessStatusCode)
            {
                var result = JsonSerializer.Deserialize<IEnumerable<PayerReceiverType>>(responseContent, _jsonOptions);
                return ApiResponse<IEnumerable<PayerReceiverType>>.Success(result!);
            }
            
            var error = JsonSerializer.Deserialize<ErrorResponse>(responseContent, _jsonOptions);
            return ApiResponse<IEnumerable<PayerReceiverType>>.Error(error?.Message ?? "Unknown error");
        }
        catch (Exception ex)
        {
            return ApiResponse<IEnumerable<PayerReceiverType>>.Error($"Network error: {ex.Message}");
        }
    }

    // Swap trade endpoints
    public async Task<ApiResponse<SwapTrade>> CreateSwapAsync(SwapTradeCreateRequest request, string bookedBy)
    {
        try
        {
            var json = JsonSerializer.Serialize(request, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            var response = await _httpClient.PostAsync($"/swaps?bookedBy={Uri.EscapeDataString(bookedBy)}", content);
            var responseContent = await response.Content.ReadAsStringAsync();
            
            if (response.IsSuccessStatusCode)
            {
                var result = JsonSerializer.Deserialize<SwapTrade>(responseContent, _jsonOptions);
                return ApiResponse<SwapTrade>.Success(result!);
            }
            
            var error = JsonSerializer.Deserialize<ErrorResponse>(responseContent, _jsonOptions);
            return ApiResponse<SwapTrade>.Error(error?.Message ?? "Unknown error");
        }
        catch (Exception ex)
        {
            return ApiResponse<SwapTrade>.Error($"Network error: {ex.Message}");
        }
    }

    public async Task<ApiResponse<SwapTrade>> UpdateSwapAsync(string tradeId, SwapTradeCreateRequest request, string bookedBy)
    {
        try
        {
            var json = JsonSerializer.Serialize(request, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            var response = await _httpClient.PutAsync($"/swaps/{Uri.EscapeDataString(tradeId)}?bookedBy={Uri.EscapeDataString(bookedBy)}", content);
            var responseContent = await response.Content.ReadAsStringAsync();
            
            if (response.IsSuccessStatusCode)
            {
                var result = JsonSerializer.Deserialize<SwapTrade>(responseContent, _jsonOptions);
                return ApiResponse<SwapTrade>.Success(result!);
            }
            
            var error = JsonSerializer.Deserialize<ErrorResponse>(responseContent, _jsonOptions);
            return ApiResponse<SwapTrade>.Error(error?.Message ?? "Unknown error");
        }
        catch (Exception ex)
        {
            return ApiResponse<SwapTrade>.Error($"Network error: {ex.Message}");
        }
    }
}

public class ApiResponse<T>
{
    public bool IsSuccess { get; set; }
    public T? Data { get; set; }
    public string? ErrorMessage { get; set; }

    public static ApiResponse<T> Success(T data) => new() { IsSuccess = true, Data = data };
    public static ApiResponse<T> Error(string message) => new() { IsSuccess = false, ErrorMessage = message };
}

public class SessionResponse
{
    public string Name { get; set; } = string.Empty;
    public string SessionId { get; set; } = string.Empty;
}

public class ErrorResponse
{
    public string Code { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}