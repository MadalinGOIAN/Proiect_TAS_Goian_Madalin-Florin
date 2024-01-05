using Proiect_PSSC.Model.Exceptions;

namespace Proiect_PSSC.Model;

public record Price
{
    public decimal Value { get; }

    public Price(decimal value)
    {
        if (IsValid(value))
            Value = value;
        else throw new InvalidPriceException($"{value:0.##} is an invalid Price.");
    }

    public override string ToString() => $"{Value:0.##}";

    public static bool TryParsePrice(string stringPrice, out Price Price)
    {
        bool isValid = false;
        Price = null;

        if (decimal.TryParse(stringPrice, out decimal numericPrice))
        {
            if (IsValid(numericPrice))
            {
                isValid = true;
                Price = new(numericPrice);
            }
        }

        return isValid;
    }

    private static bool IsValid(decimal numericPrice) => numericPrice > 0;
}
