using Microsoft.AspNetCore.SignalR;

namespace Api.Hubs;

public class TradesHub : Hub
{
    public async Task TradeCreated(object trade)
    {
        await Clients.All.SendAsync("TradeCreated", trade);
    }

    public async Task TradeUpdated(object trade)
    {
        await Clients.All.SendAsync("TradeUpdated", trade);
    }

    public override async Task OnConnectedAsync()
    {
        await base.OnConnectedAsync();
        Console.WriteLine($"Client connected: {Context.ConnectionId}");
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await base.OnDisconnectedAsync(exception);
        Console.WriteLine($"Client disconnected: {Context.ConnectionId}");
    }
}
