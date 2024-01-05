namespace Proiect_PSSC.Model;

public record Product
{
    public ProductCode ProductCode { get; init; }
    public Price Price { get; init; }
}
