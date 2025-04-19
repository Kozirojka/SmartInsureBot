namespace Insurance.App.Abstract;


public interface IReceiverService
{
    Task ReceiveAsync(CancellationToken stoppingToken);
}