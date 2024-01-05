using Proiect_PSSC.Model;
using LanguageExt;
using Proiect_PSSC;
using static LanguageExt.Prelude;

internal class Program
{
    static async Task Main(string[] args)
    {
        var listOfPlacedProducts = ReadListOfProducts().ToArray();
        OrderProductsCommand command = new(listOfPlacedProducts);
        OrderProductsWorkflow workflow = new();
        IProductValidator productValidator = new ProductValidator();
        var result = await workflow.ExecuteAsync(
            command, productValidator.CheckProductExists, productValidator.CheckProductIsInStock);

        result.Match(
            whenProductOrderPublishedFailedEvent: _event =>
            {
                Console.WriteLine($"Publish failed: {_event.Reason}");
                return _event;
            },
            whenProductOrderPublishedSucceededEvent: _event =>
            {
                Console.Clear();
                Console.WriteLine($"Publish succeeded.");
                Console.WriteLine($"Date livrare: {Client.Name}, {Client.Adress}");
                Console.WriteLine(_event.Csv);
                return _event;
            });
    }

    private static List<UnvalidatedOrderedProduct> ReadListOfProducts()
    {
        List<UnvalidatedOrderedProduct> listOfPlacedProducts = new();

        do
        {
            var productCode = ReadValue("Product Code: ");
            if (string.IsNullOrEmpty(productCode))
                break;

            var quantity = ReadValue("Quantity: ");
            if (string.IsNullOrEmpty(quantity))
                break;

            listOfPlacedProducts.Add(new(productCode, quantity));
        } while (true);

        return listOfPlacedProducts;
    }

    private static string? ReadValue(string prompt)
    {
        Console.Write(prompt);
        return Console.ReadLine();
    }
}