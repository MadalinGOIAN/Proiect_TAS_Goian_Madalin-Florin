using Moq;
using Proiect_PSSC;
using Proiect_PSSC.Model;
using System.Text;
using static LanguageExt.Prelude;
using static Proiect_PSSC.Model.OrderedProducts;
using static Proiect_PSSC.Model.ProductOrderPublishedEvent;

namespace Test_PSSC;

[TestFixture]
public class UT_OrderProductsWorkflow
{
    private OrderProductsWorkflow workflow;
    private OrderProductsCommand command;
    private IProductOrderPublishedEvent result;
    private Mock<IProductValidator> productValidatorMock;

    private readonly List<UnvalidatedOrderedProduct> dummyUnvalidatedProductList = new()
    {
        new UnvalidatedOrderedProduct("00000", "2"),
        new UnvalidatedOrderedProduct("22222", "3"),
        new UnvalidatedOrderedProduct("00001", "2"),
        new UnvalidatedOrderedProduct("00002", "3")
    };

    private readonly List<CalculatedOrderedProduct> dummyCalculatedProductList = new()
    {
        new CalculatedOrderedProduct(new ProductCode("00000"), new Quantity(2), new Price(1.98m)),
        new CalculatedOrderedProduct(new ProductCode("22222"), new Quantity(3), new Price(43.47m)),
        new CalculatedOrderedProduct(new ProductCode("00001"), new Quantity(2), new Price(17.98m)),
        new CalculatedOrderedProduct(new ProductCode("00002"), new Quantity(3), new Price(31.95m))
    };

    [SetUp]
    public void Setup()
    {
        workflow = new OrderProductsWorkflow();
        productValidatorMock = new Mock<IProductValidator>();

        productValidatorMock.Setup(validator => validator.CheckProductExists(It.IsAny<ProductCode>()))
                            .Returns(TryAsync(() => Task.FromResult(true)));
        productValidatorMock.Setup(validator => validator.CheckProductIsInStock(It.IsAny<Quantity>()))
                            .Returns(TryAsync(() => Task.FromResult(true)));
    }

    [Test]
    public async Task Test_ExecuteWorkflowReceivesUnvalidatedWithValidCodeAndQuantity_ReturnsPublishSucceededEvent()
    {
        // Arrange
        command = new(dummyUnvalidatedProductList);
        CalculatedOrderedProducts calculatedProducts = new(dummyCalculatedProductList);

        StringBuilder expectedCsv = new();
        calculatedProducts.ProductList.Aggregate(expectedCsv, (export, product)
            => export.AppendLine($"{product.ProductCode.Value}, {product.Quantity.Value}, {product.TotalPrice.Value}"));

        // Act
        result = await workflow.ExecuteAsync(command,
                                             productValidatorMock.Object.CheckProductExists,
                                             productValidatorMock.Object.CheckProductIsInStock);

        ProductOrderPublishedSucceededEvent? succeeded = result as ProductOrderPublishedSucceededEvent;

        // Assert
        Assert.AreEqual(succeeded.Csv, expectedCsv.ToString());
        Assert.AreEqual(succeeded.PublishedDate.Hour, DateTime.Now.Hour);
        Assert.AreEqual(succeeded.PublishedDate.Minute, DateTime.Now.Minute);
        Assert.AreEqual(succeeded.PublishedDate.Second, DateTime.Now.Second);
    }

    [Test]
    [TestCase("22")]
    [TestCase("")]
    [TestCase("1111111")]
    [TestCase("ooooo")]
    [TestCase("9999d")]
    public async Task Test_ExecuteWorkflowReceivesUnvalidatedWithInvalidCode_ReturnsPublishFailedEvent(string invalidValue)
    {
        // Arrange
        List<UnvalidatedOrderedProduct> dummyUnvalidatedProductListWithInvalidCode = new()
        {
            new UnvalidatedOrderedProduct("00000", "2"),
            new UnvalidatedOrderedProduct(invalidValue, "3")
        };

        command = new(dummyUnvalidatedProductListWithInvalidCode);
        string expectedReason = $"Invalid product code ({invalidValue})";

        // Act
        result = await workflow.ExecuteAsync(command,
                                             productValidatorMock.Object.CheckProductExists,
                                             productValidatorMock.Object.CheckProductIsInStock);

        ProductOrderPublishedFailedEvent? failed = result as ProductOrderPublishedFailedEvent;

        // Assert
        Assert.AreEqual(failed.Reason, expectedReason);
    }

    [Test]
    [TestCase("0")]
    [TestCase("")]
    [TestCase("-5")]
    [TestCase("1o")]
    [TestCase("-99d")]
    public async Task Test_ExecuteWorkflowReceivesUnvalidatedWithInvalidQuantity_ReturnsPublishFailedEvent(string invalidValue)
    {
        // Arrange
        List<UnvalidatedOrderedProduct> dummyUnvalidatedProductListWithInvalidQuantity = new()
        {
            new UnvalidatedOrderedProduct("00000", "2"),
            new UnvalidatedOrderedProduct("00001", invalidValue)
        };

        command = new(dummyUnvalidatedProductListWithInvalidQuantity);
        string expectedReason = $"Invalid quantity ({invalidValue})";

        // Act
        result = await workflow.ExecuteAsync(command,
                                             productValidatorMock.Object.CheckProductExists,
                                             productValidatorMock.Object.CheckProductIsInStock);

        ProductOrderPublishedFailedEvent? failed = result as ProductOrderPublishedFailedEvent;

        // Assert
        Assert.AreEqual(failed.Reason, expectedReason);
    }
}
