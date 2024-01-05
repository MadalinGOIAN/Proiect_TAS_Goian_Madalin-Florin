using CSharp.Choices;

namespace Proiect_PSSC.Model;

[AsChoice]
public static partial class OrderedProducts
{
    public interface IOrderedProducts { }

    public record UnvalidatedOrderedProducts : IOrderedProducts
    {
        public UnvalidatedOrderedProducts(IReadOnlyCollection<UnvalidatedOrderedProduct> productList)
            => ProductList = productList;

        public IReadOnlyCollection<UnvalidatedOrderedProduct> ProductList { get; }
    }

    public record InvalidatedOrderedProducts : IOrderedProducts
    {
        public InvalidatedOrderedProducts(IReadOnlyCollection<UnvalidatedOrderedProduct> productList, string reason)
        {
            ProductList = productList;
            Reason = reason;
        }

        public IReadOnlyCollection<UnvalidatedOrderedProduct> ProductList { get; }
        public string Reason { get; }
    }

    public record ValidatedOrderedProducts : IOrderedProducts
    {
        public ValidatedOrderedProducts(IReadOnlyCollection<ValidatedOrderedProduct> productList)
            => ProductList = productList;

        public IReadOnlyCollection<ValidatedOrderedProduct> ProductList { get; }
    }

    public record CalculatedOrderedProducts : IOrderedProducts
    {
        public CalculatedOrderedProducts(IReadOnlyCollection<CalculatedOrderedProduct> productList)
            => ProductList = productList;

        public IReadOnlyCollection<CalculatedOrderedProduct> ProductList { get; }
    }

    public record PublishedOrderedProducts : IOrderedProducts
    {
        public PublishedOrderedProducts(IReadOnlyCollection<CalculatedOrderedProduct> productList, string csv, DateTime publishedDate)
        {
            ProductList = productList;
            PublishedDate = publishedDate;
            Csv = csv;
        }

        public IReadOnlyCollection<CalculatedOrderedProduct> ProductList { get; }
        public DateTime PublishedDate { get; }
        public string Csv { get; }
    }
}
