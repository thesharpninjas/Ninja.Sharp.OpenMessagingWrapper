namespace Ninja.Sharp.OpenMessagingMiddleware.Interfaces
{
    /// <summary>
    /// Da capire quali parametri sono trasversali a tutti i messaggi
    /// </summary>
    public class Message
    {
        public string Id { get; set; } = string.Empty;
        public string GroupId { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
    }

    public interface IMessageConsumer
    {
        Task ConsumeAsync(Message message);
        Task ConsumeAsync<T>(T message); // Da capire se serve e quali parametri aggiungere
        // Eventuali altri overload? Rendiamoli tutti non obbligatori?
    }
}
