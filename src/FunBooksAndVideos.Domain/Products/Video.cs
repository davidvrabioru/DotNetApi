namespace FunBooksAndVideos.Domain.Products;

public class Video : Product
{
    public Video(string name, decimal price, int durationMinutes) : base(name, price)
    {
        DurationMinutes = durationMinutes;
    }

    public int DurationMinutes { get; set; }

    public override bool IsPhysical => false;
}
