using Proiect_PSSC.Model.Exceptions;
using LanguageExt;
using static LanguageExt.Prelude;

namespace Proiect_PSSC.Model;

public record Quantity
{
    public int Value { get; }

    public Quantity(int value)
    {
        if (IsValid(value))
            Value = value;
        else throw new InvalidQuantityException($"{value} is an invalid quantity.");
    }

    public override string ToString() => Value.ToString();

    public static Option<Quantity> TryParse(string stringQuantity)
    {
        if (int.TryParse(stringQuantity, out int numericQuantity) && IsValid(numericQuantity))
            return Some(new Quantity(numericQuantity));

        return None;
    }

    private static bool IsValid(int numericQuantity) => numericQuantity > 0;
}
