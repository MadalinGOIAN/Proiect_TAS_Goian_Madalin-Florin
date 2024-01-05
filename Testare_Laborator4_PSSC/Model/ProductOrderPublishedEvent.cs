using CSharp.Choices;

namespace Proiect_PSSC.Model;

[AsChoice]
public static partial class ProductOrderPublishedEvent
{
    public interface IProductOrderPublishedEvent { }

    public record ProductOrderPublishedSucceededEvent : IProductOrderPublishedEvent
    {
        public string Csv { get; }
        public DateTime PublishedDate { get; }

        internal ProductOrderPublishedSucceededEvent(string csv, DateTime publishedDate)
        {
            Csv = csv;
            PublishedDate = publishedDate;
        }
    }

    public record ProductOrderPublishedFailedEvent : IProductOrderPublishedEvent
    {
        public string Reason { get; }

        internal ProductOrderPublishedFailedEvent(string reason) => Reason = reason;
    }
}
