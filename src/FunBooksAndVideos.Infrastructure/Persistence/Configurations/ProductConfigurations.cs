using FunBooksAndVideos.Domain.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FunBooksAndVideos.Infrastructure.Persistence.Configurations;

public sealed class ProductConfiguration : AuditableEntityConfiguration<Product>
{
    protected override void ConfigureEntity(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Products");

        builder.HasDiscriminator<string>("ProductType")
            .HasValue<Book>(nameof(Book))
            .HasValue<Video>(nameof(Video))
            .HasValue<Membership>(nameof(Membership));

        builder.Property(product => product.Name).HasMaxLength(200).IsRequired();
        builder.Property(product => product.Price).HasPrecision(18, 2);
        builder.Ignore(product => product.IsPhysical);
    }
}

public sealed class BookConfiguration : IEntityTypeConfiguration<Book>
{
    public void Configure(EntityTypeBuilder<Book> builder) =>
        builder.Property(book => book.Author).HasMaxLength(200).IsRequired();
}

public sealed class VideoConfiguration : IEntityTypeConfiguration<Video>
{
    public void Configure(EntityTypeBuilder<Video> builder) =>
        builder.Property(video => video.DurationMinutes).IsRequired();
}

public sealed class MembershipConfiguration : IEntityTypeConfiguration<Membership>
{
    public void Configure(EntityTypeBuilder<Membership> builder) =>
        builder.Property(membership => membership.Type).HasConversion<string>().HasMaxLength(20);
}
