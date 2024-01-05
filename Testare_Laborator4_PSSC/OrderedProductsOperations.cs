using Proiect_PSSC.Model;
using LanguageExt;
using System.Text;
using static Proiect_PSSC.Model.OrderedProducts;
using static LanguageExt.Prelude;

namespace Proiect_PSSC;

public static class OrderedProductsOperations
{
    public static Task<IOrderedProducts> ValidateOrderedProducts(Func<ProductCode, TryAsync<bool>> checkProductExists,
                                                                 Func<Quantity, TryAsync<bool>> checkProductIsInStock,
                                                                 UnvalidatedOrderedProducts orderedProducts)
        => orderedProducts.ProductList
                          .Select(ValidateProduct(checkProductExists, checkProductIsInStock))
                          .Aggregate(CreateEmptyValidatedProductList().ToAsync(), ReduceValidProducts)
                          .MatchAsync(Right: validatedProducts => new ValidatedOrderedProducts(validatedProducts),
                                      LeftAsync: errorMessage
                                                 => Task.FromResult(
                                                    new InvalidatedOrderedProducts(orderedProducts.ProductList, errorMessage)
                                                    as IOrderedProducts)
                          );

    private static Func<UnvalidatedOrderedProduct, EitherAsync<string, ValidatedOrderedProduct>> ValidateProduct(
        Func<ProductCode, TryAsync<bool>> checkProductExists,
        Func<Quantity, TryAsync<bool>> checkProductIsInStock)
        => (unvalidatedOrderedProduct) => ValidateProduct(checkProductExists, checkProductIsInStock, unvalidatedOrderedProduct);

    private static EitherAsync<string, ValidatedOrderedProduct> ValidateProduct(Func<ProductCode, TryAsync<bool>> checkProductExists,
                                                                                Func<Quantity, TryAsync<bool>> checkProductIsInStock,
                                                                                UnvalidatedOrderedProduct unvalidatedOrderedProduct)
    {
        return from productCode in ProductCode.TryParse(unvalidatedOrderedProduct.ProductCode)
                                          .ToEitherAsync(() => $"Invalid product code ({unvalidatedOrderedProduct.ProductCode})")
               from quantity in Quantity.TryParse(unvalidatedOrderedProduct.Quantity)
                                        .ToEitherAsync(() => $"Invalid quantity ({unvalidatedOrderedProduct.Quantity})")
               from productExists in checkProductExists(productCode)
                                     .ToEither((error) => error.ToString())
               from productIsInStock in checkProductIsInStock(quantity)
                                        .ToEither((error) => error.ToString())
               select new ValidatedOrderedProduct(productCode, quantity);
    }

    private static Either<string, List<ValidatedOrderedProduct>> CreateEmptyValidatedProductList()
        => Right(new List<ValidatedOrderedProduct>());

    private static EitherAsync<string, List<ValidatedOrderedProduct>> ReduceValidProducts(
        EitherAsync<string, List<ValidatedOrderedProduct>> acc,
        EitherAsync<string, ValidatedOrderedProduct> next)
        => from list in acc
           from nextProduct in next
           select list.AppendValidProduct(nextProduct);

    private static List<ValidatedOrderedProduct> AppendValidProduct(this List<ValidatedOrderedProduct> list,
                                                                    ValidatedOrderedProduct validProduct)
    {
        list.Add(validProduct);
        return list;
    }

    public static IOrderedProducts CalculateOrderedProducts(IOrderedProducts orderedProducts)
        => orderedProducts.Match(whenUnvalidatedOrderedProducts: unvalidatedProducts => unvalidatedProducts,
                                 whenInvalidatedOrderedProducts: invalidatedProducts => invalidatedProducts,
                                 whenCalculatedOrderedProducts: calculatedProducts => calculatedProducts,
                                 whenPublishedOrderedProducts: publishedProducts => publishedProducts,
                                 whenValidatedOrderedProducts: validatedProducts =>
    {
        var calculatedProducts = validatedProducts.ProductList.Select(validProduct =>
            new CalculatedOrderedProduct(validProduct.ProductCode,
                                         validProduct.Quantity,
                                         CalculateTotalPrice(validProduct)));

        return new CalculatedOrderedProducts(calculatedProducts.ToList().AsReadOnly());
    });

    public static IOrderedProducts PublishOrderedProducts(IOrderedProducts orderedProducts)
        => orderedProducts.Match(whenUnvalidatedOrderedProducts: unvalidatedProducts => unvalidatedProducts,
                                 whenInvalidatedOrderedProducts: invalidatedProducts => invalidatedProducts,
                                 whenPublishedOrderedProducts: publishedProducts => publishedProducts,
                                 whenValidatedOrderedProducts: validatedProducts => validatedProducts,
                                 whenCalculatedOrderedProducts: calculatedProducts =>
    {
        StringBuilder csv = new();
        calculatedProducts.ProductList.Aggregate(csv, (export, product)
            => export.AppendLine($"{product.ProductCode.Value}, {product.Quantity.Value}, {product.TotalPrice.Value}"));

        PublishedOrderedProducts publishedOrderedProducts =
            new(calculatedProducts.ProductList, csv.ToString(), DateTime.Now);

        return publishedOrderedProducts;
    });

    private static Price CalculateTotalPrice(ValidatedOrderedProduct validProduct)
        => new Price(ProductRepository.AvailableProducts.Find(product
            => product.ProductCode.Equals(validProduct.ProductCode)).Price.Value * validProduct.Quantity.Value);
}
