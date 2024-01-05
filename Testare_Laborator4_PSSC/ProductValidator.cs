using LanguageExt;
using Proiect_PSSC.Model;
using static LanguageExt.Prelude;

namespace Proiect_PSSC;

public interface IProductValidator
{
    TryAsync<bool> CheckProductExists(ProductCode productCode);
    TryAsync<bool> CheckProductIsInStock(Quantity quantity);
}

public class ProductValidator : IProductValidator
{
    public TryAsync<bool> CheckProductExists(ProductCode productCode)
        => TryAsync(() => Task.FromResult(true));
    public TryAsync<bool> CheckProductIsInStock(Quantity quantity)
        => TryAsync(() => Task.FromResult(true));
}
