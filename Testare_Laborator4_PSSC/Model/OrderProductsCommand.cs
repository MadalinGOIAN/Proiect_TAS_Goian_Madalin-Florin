namespace Proiect_PSSC.Model;

public record OrderProductsCommand
{
    public OrderProductsCommand(IReadOnlyCollection<UnvalidatedOrderedProduct> inputOrderedProducts)
        => InputOrderedProducts = inputOrderedProducts;

    public IReadOnlyCollection<UnvalidatedOrderedProduct> InputOrderedProducts { get; }
}
