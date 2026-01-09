using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shared.S3ObjectStorage.Domain.Image;

namespace Shared.S3ObjectStorage.EF.EntityTypeConfigurations;

public class ImageConfiguration : IEntityTypeConfiguration<Image>
{
  public void Configure(EntityTypeBuilder<Image> builder)
  {
    builder.ToTable($"{Constants.TablePrefix}_images");

    builder.HasKey(a => a.Id);
    builder.Property(a => a.Id).ValueGeneratedNever();

    // Foreign Key: OriginalFileId (required)
    builder
      .HasOne(i => i.OriginalFile)
      .WithMany()
      .HasForeignKey(i => i.OriginalFileId)
      .OnDelete(DeleteBehavior.Restrict) // Avoid cascade delete
      .IsRequired(); // Non-nullable FK

    builder
      .HasOne(i => i.ThumbnailFile)
      .WithMany()
      .HasForeignKey(i => i.ThumbnailFileId)
      .OnDelete(DeleteBehavior.Restrict); // Avoid cascade delete

    builder
      .HasOne(i => i.SmallFile)
      .WithMany()
      .HasForeignKey(i => i.SmallFileId)
      .OnDelete(DeleteBehavior.Restrict); // Avoid cascade delete

    builder
      .HasOne(i => i.MediumFile)
      .WithMany()
      .HasForeignKey(i => i.MediumFileId)
      .OnDelete(DeleteBehavior.Restrict); // Avoid cascade delete

    builder
      .HasOne(i => i.LargeFile)
      .WithMany()
      .HasForeignKey(i => i.LargeFileId)
      .OnDelete(DeleteBehavior.Restrict); // Avoid cascade delete

    builder
      .HasOne(i => i.ExtraLargeFile)
      .WithMany()
      .HasForeignKey(i => i.ExtraLargeFileId)
      .OnDelete(DeleteBehavior.Restrict); // Avoid cascade delete

    builder
      .HasOne(i => i.XXLargeFile)
      .WithMany()
      .HasForeignKey(i => i.XXLargeFileId)
      .OnDelete(DeleteBehavior.Restrict); // Avoid cascade delete

    builder
      .HasOne(i => i.MaxFile)
      .WithMany()
      .HasForeignKey(i => i.MaxFileId)
      .OnDelete(DeleteBehavior.Restrict); // Avoid cascade delete
  }
}