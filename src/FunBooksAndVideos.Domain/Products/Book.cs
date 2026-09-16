namespace FunBooksAndVideos.Domain.Products;

public class Book : Product
{
    public Book(string name, decimal price, string author) : base(name, price)
    {
        Author = author;
    }

    public string Author { get; set; }

    public override bool IsPhysical => true;
}
