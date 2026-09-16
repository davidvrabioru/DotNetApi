using FunBooksAndVideos.Domain.Products;

namespace FunBooksAndVideos.Tests.Domain;

public class ProductTests
{
    public static TheoryData<Product, bool> ProductsAndPhysicality => new()
    {
        { TestData.Book(), true },
        { TestData.Video(), false },
        { TestData.Membership(), false }
    };

    [Theory]
    [MemberData(nameof(ProductsAndPhysicality))]
    public void IsPhysical_IsDecidedByTheConcreteProductType(Product product, bool expected)
    {
        Assert.Equal(expected, product.IsPhysical);
    }
}
