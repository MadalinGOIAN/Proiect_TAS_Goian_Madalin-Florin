using Proiect_PSSC.Model;

namespace Proiect_PSSC;

public static class ProductRepository
{
    public static List<Product> AvailableProducts = new()
    {
        new Product { ProductCode = new ProductCode("11111"), Price = new Price(9.99M) },
        new Product { ProductCode = new ProductCode("22222"), Price = new Price(14.49M) },
        new Product { ProductCode = new ProductCode("33333"), Price = new Price(2.09M) },
        new Product { ProductCode = new ProductCode("44444"), Price = new Price(3.29M) },
        new Product { ProductCode = new ProductCode("55555"), Price = new Price(4.24M) },
        new Product { ProductCode = new ProductCode("66666"), Price = new Price(8.99M) },
        new Product { ProductCode = new ProductCode("77777"), Price = new Price(14.32M) },
        new Product { ProductCode = new ProductCode("88888"), Price = new Price(49.99M) },
        new Product { ProductCode = new ProductCode("99999"), Price = new Price(4.26M) },
        new Product { ProductCode = new ProductCode("12345"), Price = new Price(17.89M) },
        new Product { ProductCode = new ProductCode("12121"), Price = new Price(1.99M) },
        new Product { ProductCode = new ProductCode("10101"), Price = new Price(5.45M) },
        new Product { ProductCode = new ProductCode("00000"), Price = new Price(0.99M) },
        new Product { ProductCode = new ProductCode("00001"), Price = new Price(8.99M) },
        new Product { ProductCode = new ProductCode("00002"), Price = new Price(10.65M) },
        new Product { ProductCode = new ProductCode("00003"), Price = new Price(23.59M) }
    };
}
