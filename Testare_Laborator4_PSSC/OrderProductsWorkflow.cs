using Proiect_PSSC.Model;
using LanguageExt;
using static Proiect_PSSC.Model.OrderedProducts;
using static Proiect_PSSC.Model.ProductOrderPublishedEvent;
using static Proiect_PSSC.OrderedProductsOperations;

namespace Proiect_PSSC;

public class OrderProductsWorkflow
{
    public async Task<IProductOrderPublishedEvent> ExecuteAsync(OrderProductsCommand command,
                                                                Func<ProductCode, TryAsync<bool>> checkProductExists,
                                                                Func<Quantity, TryAsync<bool>> checkProductIsInStock)
    {
        UnvalidatedOrderedProducts unvalidatedOrderedProducts = new(command.InputOrderedProducts);
        IOrderedProducts products = await ValidateOrderedProducts(checkProductExists,
                                                                  checkProductIsInStock,
                                                                  unvalidatedOrderedProducts);
        products = CalculateOrderedProducts(products);
        products = PublishOrderedProducts(products);

        return products.Match(
            whenUnvalidatedOrderedProducts: unvalidatedProducts
                => new ProductOrderPublishedFailedEvent("Unexpected unvalidated state") as IProductOrderPublishedEvent,
            whenInvalidatedOrderedProducts: invalidProducts
                => new ProductOrderPublishedFailedEvent(invalidProducts.Reason),
            whenValidatedOrderedProducts: validatedProducts
                => new ProductOrderPublishedFailedEvent("Unexpected validated state"),
            whenCalculatedOrderedProducts: calculatedProducts
                => new ProductOrderPublishedFailedEvent("Unexpected calculated state"),
            whenPublishedOrderedProducts: publishedProducts
                => new ProductOrderPublishedSucceededEvent(publishedProducts.Csv, publishedProducts.PublishedDate));
    }
}
