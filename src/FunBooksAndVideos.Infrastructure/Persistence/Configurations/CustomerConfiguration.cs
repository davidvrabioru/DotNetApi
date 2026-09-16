using FunBooksAndVideos.Domain.Customers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FunBooksAndVideos.Infrastructure.Persistence.Configurations;

public class CustomerConfiguration : AuditableEntityConfiguration<Customer>
{
    protected override void ConfigureEntity(EntityTypeBuilder<Customer> builder)
    {
        builder.ToTable("Customers");

        builder.Property(customer => customer.Name).HasMaxLength(200).IsRequired();
        builder.Property(customer => customer.Email).HasMaxLength(320).IsRequired();
        builder.HasIndex(customer => customer.Email).IsUnique();
        builder.Property(customer => customer.Membership).HasConversion<string>().HasMaxLength(20);
    }
}
