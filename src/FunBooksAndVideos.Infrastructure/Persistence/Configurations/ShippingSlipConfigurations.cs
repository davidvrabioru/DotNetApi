using FunBooksAndVideos.Domain.Shipping;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FunBooksAndVideos.Infrastructure.Persistence.Configurations;

public class ShippingSlipConfiguration : AuditableEntityConfiguration<ShippingSlip>
{
    protected override void ConfigureEntity(EntityTypeBuilder<ShippingSlip> builder)
    {
        builder.ToTable("ShippingSlips");

        builder.HasOne(slip => slip.PurchaseOrder)
            .WithOne()
            .HasForeignKey<ShippingSlip>(slip => slip.PurchaseOrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(slip => slip.Lines)
            .WithOne()
            .HasForeignKey(line => line.ShippingSlipId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class ShippingSlipLineConfiguration : IEntityTypeConfiguration<ShippingSlipLine>
{
    public void Configure(EntityTypeBuilder<ShippingSlipLine> builder)
    {
        builder.ToTable("ShippingSlipLines");
        builder.Property(line => line.ProductName).HasMaxLength(200).IsRequired();
    }
}
