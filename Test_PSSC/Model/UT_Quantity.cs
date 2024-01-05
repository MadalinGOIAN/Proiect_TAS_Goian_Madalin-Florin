using LanguageExt;
using Proiect_PSSC.Model;
using Proiect_PSSC.Model.Exceptions;

namespace Test_PSSC.Model;

[TestFixture]
public class UT_Quantity
{
    private Quantity quantity;

    [SetUp]
    public void Setup()
    {
        quantity = new Quantity(5);
    }

    [Test]
    [TestCase(1)]
    [TestCase(5)]
    [TestCase(10)]
    [TestCase(105)]
    [TestCase(100015)]
    public void Test_ContructorWithValidParameterCreatesObject_Success(int value)
    {
        var quantity = new Quantity(value);
        Assert.AreEqual(quantity.Value, value);
    }

    [Test]
    [TestCase(0)]
    [TestCase(-1)]
    [TestCase(-10)]
    [TestCase(-105)]
    [TestCase(-100015)]
    public void Test_ContructorWithInvalidParameter_ThrowsException(int value)
    {
        Assert.Throws<InvalidQuantityException>(() => { var q = new Quantity(value); });
    }

    [Test]
    public void Test_ToStringOutputFormat_Success()
    {
        Assert.AreEqual(quantity.ToString(), "5");
    }

    [Test]
    [TestCase("string")]
    [TestCase("999999999L")]
    [TestCase("312569E-5")]
    [TestCase("13.49d")]
    [TestCase("13.49")]
    [TestCase("0")]
    [TestCase("-1")]
    [TestCase("-10")]
    [TestCase("-105")]
    [TestCase("-1015")]
    public void Test_TryParseWithInvalidTypeOrValue_ReturnsNone(string stringValue)
    {
        Option<Quantity> quantity = Quantity.TryParse(stringValue);
        Assert.IsTrue(quantity.IsNone);
    }

    [Test]
    [TestCase("1")]
    [TestCase("5")]
    [TestCase("10")]
    [TestCase("105")]
    [TestCase("1015")]
    public void Test_TryParseWithValidValues_ReturnsSome(string stringValue)
    {
        Option<Quantity> quantity = Quantity.TryParse(stringValue);

        Assert.IsTrue(quantity.IsSome);
        quantity.IfSome(q => Assert.AreEqual(q.Value, int.Parse(stringValue)));
    }
}
