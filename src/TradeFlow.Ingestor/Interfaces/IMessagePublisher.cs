namespace TradeFlow.Ingestor.Interfaces;

public interface IMessagePublisher
{
    void Publish(string message);
}
