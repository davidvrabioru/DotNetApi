using FunBooksAndVideos.Domain.Common;

namespace FunBooksAndVideos.Domain.Products;

public abstract class Product : AuditableEntity
{
    protected Product(string name, decimal price)
    {
        Name = name;
        Price = price;
    }

    public string Name { get; set; }
    public decimal Price { get; set; }

    public abstract bool IsPhysical { get; }
}
