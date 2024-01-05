using LanguageExt;
using Proiect_PSSC.Model;
using Proiect_PSSC.Model.Exceptions;

namespace Test_PSSC.Model;

[TestFixture]
public class UT_ProductCode
{
    private ProductCode productCode;

    [SetUp]
    public void Setup()
    {
        productCode = new ProductCode("00000");
    }

    [Test]
    [TestCase("11111")]
    [TestCase("01010")]
    [TestCase("99999")]
    [TestCase("12345")]
    [TestCase("00001")]
    public void Test_ContructorWithValidPatternCreatesObject_Success(string value)
    {
        var productCode = new ProductCode(value);
        Assert.AreEqual(productCode.Value, value);
    }
    
    [Test]
    [TestCase("111111")]
    [TestCase("0101010101")]
    [TestCase("9d9d9")]
    [TestCase("ooooo")]
    [TestCase("l1l*{}")]
    public void Test_ContructorWithInvalidPattern_ThrowsException(string value)
    {
        Assert.Throws<InvalidProductCodeException>(() => { var p = new ProductCode(value); });
    }

    [Test]
    public void Test_ToStringOutputFormat_Success()
    {
        Assert.AreEqual(productCode.ToString(), "00000");
    }

    [Test]
    [TestCase("111111")]
    [TestCase("0101010101")]
    [TestCase("9d9d9")]
    [TestCase("ooooo")]
    [TestCase("l1l*{}")]
    public void Test_TryParseWithInvalidPattern_ReturnsNone(string stringValue)
    {
        Option<ProductCode> productCode = ProductCode.TryParse(stringValue);
        Assert.IsTrue(productCode.IsNone);
    }
    
    [Test]
    [TestCase("11111")]
    [TestCase("01010")]
    [TestCase("99999")]
    [TestCase("12345")]
    [TestCase("00001")]
    public void Test_TryParseWithValidPattern_ReturnsSome(string stringValue)
    {
        Option<ProductCode> productCode = ProductCode.TryParse(stringValue);

        Assert.IsTrue(productCode.IsSome);
        productCode.IfSome(p => Assert.AreEqual(p.Value, stringValue));
    }
}
