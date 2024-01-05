using Proiect_PSSC.Model.Exceptions;
using LanguageExt;
using System.Text.RegularExpressions;
using static LanguageExt.Prelude;

namespace Proiect_PSSC.Model;

public record ProductCode
{
    private static readonly Regex ValidPattern = new("^[0-9]{5}$");
    public string Value { get; }

    public ProductCode(string value)
    {
        if (IsValid(value))
            Value = value;
        else throw new InvalidProductCodeException("The product code needs to have 5 digits!");
    }

    private static bool IsValid(string stringValue) => ValidPattern.IsMatch(stringValue);

    public override string ToString() => Value;

    public static Option<ProductCode> TryParse(string stringValue)
    {
        if (IsValid(stringValue))
            return Some(new ProductCode(stringValue));

        return None;
    }
}
