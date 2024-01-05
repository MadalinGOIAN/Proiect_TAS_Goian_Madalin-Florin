using Moq;
using Proiect_PSSC;
using Proiect_PSSC.Model;
using System.Text;
using static LanguageExt.Prelude;
using static Proiect_PSSC.OrderedProductsOperations;
using static Proiect_PSSC.Model.OrderedProducts;

namespace Test_PSSC;

[TestFixture]
public class UT_OrderedProductsOperations
{
    private UnvalidatedOrderedProducts unvalidatedProducts;
    private InvalidatedOrderedProducts invalidatedProducts;
    private ValidatedOrderedProducts validatedProducts;
    private CalculatedOrderedProducts calculatedProducts;
    private PublishedOrderedProducts publishedProducts;
    IOrderedProducts result;
    private Mock<IProductValidator> productValidatorMock;

    private readonly List<UnvalidatedOrderedProduct> dummyUnvalidatedProductList = new()
    {
        new UnvalidatedOrderedProduct("00000", "2"),
        new UnvalidatedOrderedProduct("22222", "3"),
        new UnvalidatedOrderedProduct("00001", "2"),
        new UnvalidatedOrderedProduct("00002", "3")
    };

    private readonly List<ValidatedOrderedProduct> dummyValidatedProductList = new()
    {
        new ValidatedOrderedProduct(new ProductCode("00000"), new Quantity(2)),
        new ValidatedOrderedProduct(new ProductCode("22222"), new Quantity(3)),
        new ValidatedOrderedProduct(new ProductCode("00001"), new Quantity(2)),
        new ValidatedOrderedProduct(new ProductCode("00002"), new Quantity(3))
    };

    private readonly List<CalculatedOrderedProduct> dummyCalculatedProductList = new()
    {
        new CalculatedOrderedProduct(new ProductCode("00000"), new Quantity(2), new Price(1.98m)),
        new CalculatedOrderedProduct(new ProductCode("22222"), new Quantity(3), new Price(43.47m)),
        new CalculatedOrderedProduct(new ProductCode("00001"), new Quantity(2), new Price(17.98m)),
        new CalculatedOrderedProduct(new ProductCode("00002"), new Quantity(3), new Price(31.95m))
    };

    private string reason = "test";
    private DateTime publishedDate = DateTime.Now;
    private string csv = "test";

    [SetUp]
    public void Setup()
    {
        unvalidatedProducts = new(dummyUnvalidatedProductList);
        invalidatedProducts = new(dummyUnvalidatedProductList, reason);
        validatedProducts = new(dummyValidatedProductList);
        calculatedProducts = new(dummyCalculatedProductList);
        publishedProducts = new(dummyCalculatedProductList, csv, publishedDate);
        productValidatorMock = new Mock<IProductValidator>();
    }

    [Test]
    public async Task Test_ValidateOrderedProductsReceivesUnvalidatedWithValidCodeAndQuantity_ReturnsValidated()
    {
        // Arrange
        productValidatorMock.Setup(validator => validator.CheckProductExists(It.IsAny<ProductCode>()))
                            .Returns(TryAsync(() => Task.FromResult(true)));
        productValidatorMock.Setup(validator => validator.CheckProductIsInStock(It.IsAny<Quantity>()))
                            .Returns(TryAsync(() => Task.FromResult(true)));

        // Act
        result = await ValidateOrderedProducts(productValidatorMock.Object.CheckProductExists,
                                               productValidatorMock.Object.CheckProductIsInStock,
                                               unvalidatedProducts);

        ValidatedOrderedProducts? validated = result as ValidatedOrderedProducts;

        // Assert
        Assert.AreEqual(validated.ProductList, dummyValidatedProductList);
    }

    [Test]
    [TestCase("22")]
    [TestCase("")]
    [TestCase("1111111")]
    [TestCase("ooooo")]
    [TestCase("9999d")]
    public async Task Test_ValidateOrderedProductsReceivesUnvalidatedWithInvalidCode_ReturnsInvalidated(string invalidValue)
    {
        // Arrange
        productValidatorMock.Setup(validator => validator.CheckProductExists(It.IsAny<ProductCode>()))
                            .Returns(TryAsync(() => Task.FromResult(true)));
        productValidatorMock.Setup(validator => validator.CheckProductIsInStock(It.IsAny<Quantity>()))
                            .Returns(TryAsync(() => Task.FromResult(true)));

        List<UnvalidatedOrderedProduct> dummyUnvalidatedProductListWithInvalidCode = new()
        {
            new UnvalidatedOrderedProduct("00000", "2"),
            new UnvalidatedOrderedProduct(invalidValue, "3")
        };

        unvalidatedProducts = new UnvalidatedOrderedProducts(dummyUnvalidatedProductListWithInvalidCode);
        string expectedReason = $"Invalid product code ({invalidValue})";

        // Act
        result = await ValidateOrderedProducts(productValidatorMock.Object.CheckProductExists,
                                               productValidatorMock.Object.CheckProductIsInStock,
                                               unvalidatedProducts);

        InvalidatedOrderedProducts? invalidated = result as InvalidatedOrderedProducts;

        // Assert
        Assert.AreEqual(invalidated.ProductList, dummyUnvalidatedProductListWithInvalidCode);
        Assert.AreEqual(invalidated.Reason, expectedReason);
    }

    [Test]
    [TestCase("0")]
    [TestCase("")]
    [TestCase("-5")]
    [TestCase("1o")]
    [TestCase("-99d")]
    public async Task Test_ValidateOrderedProductsReceivesUnvalidatedWithInvalidQuantity_ReturnsInvalidated(string invalidValue)
    {
        // Arrange
        productValidatorMock.Setup(validator => validator.CheckProductExists(It.IsAny<ProductCode>()))
                            .Returns(TryAsync(() => Task.FromResult(true)));
        productValidatorMock.Setup(validator => validator.CheckProductIsInStock(It.IsAny<Quantity>()))
                            .Returns(TryAsync(() => Task.FromResult(true)));

        List<UnvalidatedOrderedProduct> dummyUnvalidatedProductListWithInvalidQuantity = new()
        {
            new UnvalidatedOrderedProduct("00000", "2"),
            new UnvalidatedOrderedProduct("00001", invalidValue)
        };

        unvalidatedProducts = new UnvalidatedOrderedProducts(dummyUnvalidatedProductListWithInvalidQuantity);
        string expectedReason = $"Invalid quantity ({invalidValue})";

        // Act
        result = await ValidateOrderedProducts(productValidatorMock.Object.CheckProductExists,
                                               productValidatorMock.Object.CheckProductIsInStock,
                                               unvalidatedProducts);

        InvalidatedOrderedProducts? invalidated = result as InvalidatedOrderedProducts;

        // Assert
        Assert.AreEqual(invalidated.ProductList, dummyUnvalidatedProductListWithInvalidQuantity);
        Assert.AreEqual(invalidated.Reason, expectedReason);
    }

    [Test]
    public void Test_CalculateOrderedProductsReceivesUnvalidated_ReturnsUnvalidated()
    {
        result = CalculateOrderedProducts(unvalidatedProducts);
        UnvalidatedOrderedProducts? unvalidated = result as UnvalidatedOrderedProducts;

        Assert.AreEqual(unvalidated.ProductList, dummyUnvalidatedProductList);
    }
    
    [Test]
    public void Test_CalculateOrderedProductsReceivesInvalidated_ReturnsInvalidated()
    {
        result = CalculateOrderedProducts(invalidatedProducts);
        InvalidatedOrderedProducts? invalidated = result as InvalidatedOrderedProducts;

        Assert.AreEqual(invalidated.ProductList, dummyUnvalidatedProductList);
        Assert.AreEqual(invalidated.Reason, reason);
    }
    
    [Test]
    public void Test_CalculateOrderedProductsReceivesCalculated_ReturnsCalculated()
    {
        result = CalculateOrderedProducts(calculatedProducts);
        CalculatedOrderedProducts? calculated = result as CalculatedOrderedProducts;

        Assert.AreEqual(calculated.ProductList, dummyCalculatedProductList);
    }
    
    [Test]
    public void Test_CalculateOrderedProductsReceivesPublished_ReturnsPublished()
    {
        result = CalculateOrderedProducts(publishedProducts);
        PublishedOrderedProducts? published = result as PublishedOrderedProducts;

        Assert.AreEqual(published.ProductList, dummyCalculatedProductList);
        Assert.AreEqual(published.Csv, csv);
        Assert.AreEqual(published.PublishedDate, publishedDate);
    }
    
    [Test]
    public void Test_CalculateOrderedProductsReceivesValidated_ExecutesAndReturnsCalculated()
    {
        result = CalculateOrderedProducts(validatedProducts);
        CalculatedOrderedProducts? calculated = result as CalculatedOrderedProducts;

        Assert.AreEqual(calculated.ProductList, dummyCalculatedProductList);
    }

    [Test]
    public void Test_PublishOrderedProductsReceivesUnvalidated_ReturnsUnvalidated()
    {
        result = PublishOrderedProducts(unvalidatedProducts);
        UnvalidatedOrderedProducts? unvalidated = result as UnvalidatedOrderedProducts;

        Assert.AreEqual(unvalidated.ProductList, dummyUnvalidatedProductList);
    }

    [Test]
    public void Test_PublishOrderedProductsReceivesInvalidated_ReturnsInvalidated()
    {
        result = PublishOrderedProducts(invalidatedProducts);
        InvalidatedOrderedProducts? invalidated = result as InvalidatedOrderedProducts;

        Assert.AreEqual(invalidated.ProductList, dummyUnvalidatedProductList);
        Assert.AreEqual(invalidated.Reason, reason);
    }

    [Test]
    public void Test_PublishOrderedProductsReceivesPublished_ReturnsPublished()
    {
        result = PublishOrderedProducts(publishedProducts);
        PublishedOrderedProducts? published = result as PublishedOrderedProducts;

        Assert.AreEqual(published.ProductList, dummyCalculatedProductList);
        Assert.AreEqual(published.Csv, csv);
        Assert.AreEqual(published.PublishedDate.Hour, publishedDate.Hour);
        Assert.AreEqual(published.PublishedDate.Minute, publishedDate.Minute);
        Assert.AreEqual(published.PublishedDate.Second, publishedDate.Second);
    }

    [Test]
    public void Test_PublishOrderedProductsReceivesValidated_ReturnsValidated()
    {
        result = PublishOrderedProducts(validatedProducts);
        ValidatedOrderedProducts? validated = result as ValidatedOrderedProducts;

        Assert.AreEqual(validated.ProductList, dummyValidatedProductList);
    }

    [Test]
    public void Test_PublishOrderedProductsReceivesCalculated_ExecutesAndReturnsPublished()
    {
        StringBuilder expectedCsv = new();
        calculatedProducts.ProductList.Aggregate(expectedCsv, (export, product)
            => export.AppendLine($"{product.ProductCode.Value}, {product.Quantity.Value}, {product.TotalPrice.Value}"));

        result = PublishOrderedProducts(calculatedProducts);
        PublishedOrderedProducts? published = result as PublishedOrderedProducts;

        Assert.AreEqual(published.ProductList, dummyCalculatedProductList);
        Assert.AreEqual(published.Csv, expectedCsv.ToString());
        Assert.AreEqual(published.PublishedDate.Hour, publishedDate.Hour);
        Assert.AreEqual(published.PublishedDate.Minute, publishedDate.Minute);
        Assert.AreEqual(published.PublishedDate.Second, publishedDate.Second);
    }
}
