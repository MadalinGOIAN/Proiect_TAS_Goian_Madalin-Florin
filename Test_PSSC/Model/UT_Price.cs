using Proiect_PSSC.Model;
using Proiect_PSSC.Model.Exceptions;

namespace Test_PSSC.Model;

[TestFixture]
public class UT_Price
{
    private Price price;

    [SetUp]
    public void Setup()
    {
        price = new Price(3.49M);
    }
    
    [Test]
    [TestCase(0.01)]
    [TestCase(0.99)]
    [TestCase(10)]
    [TestCase(10.5)]
    [TestCase(10001.89)]
    public void Test_ContructorWithValidParameterCreatesObject_Success(decimal value)
    {
        var price = new Price(value);
        Assert.AreEqual(price.Value, value);
    }

    [Test]
    [TestCase(0)]
    [TestCase(-0.99)]
    [TestCase(-10)]
    [TestCase(-10.5)]
    [TestCase(-10001.89)]
    public void Test_ContructorWithInvalidParameter_ThrowsException(decimal value)
    {
        Assert.Throws<InvalidPriceException>(() => { var p = new Price(value); });
    }

    [Test]
    public void Test_ToStringOutputFormat_Success()
    {
        Assert.AreEqual(price.ToString(), "3.49");
    }
    
    [Test]
    [TestCase("string")]
    [TestCase("0x2b")]
    [TestCase("312569E-5")]
    [TestCase("true")]
    [TestCase("712UL")]
    [TestCase("0")]
    [TestCase("-0.99")]
    [TestCase("-10")]
    [TestCase("-50.01")]
    [TestCase("-10001.89")]
    public void Test_TryParsePriceWithInvalidTypeOrValue_Fails(string stringValue)
    {
        Price failedParsedPrice;

        Assert.IsFalse(Price.TryParsePrice(stringValue, out failedParsedPrice));
        Assert.IsNull(failedParsedPrice);
    }

    [Test]
    [TestCase("0.1")]
    [TestCase("0.99")]
    [TestCase("10")]
    [TestCase("50.01")]
    [TestCase("10001.89")]
    public void Test_TryParsePriceWithValidValue_Success(string stringValue)
    {
        Price parsedPrice;

        Assert.IsTrue(Price.TryParsePrice(stringValue, out parsedPrice));
        Assert.AreEqual(parsedPrice.Value, decimal.Parse(stringValue));
    }
}