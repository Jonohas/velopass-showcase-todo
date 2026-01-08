using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using File = Shared.S3ObjectStorage.Domain.File.File;

namespace Shared.S3ObjectStorage.EF.EntityTypeConfigurations;

public class FileConfiguration : IEntityTypeConfiguration<File>
{
  public void Configure(EntityTypeBuilder<File> builder)
  {
    builder.ToTable($"{Constants.TablePrefix}_file");

    builder.HasKey(a => a.Id);
    builder.Property(a => a.Id).ValueGeneratedNever();

    builder.HasOne(file => file.ParentFolder)
      .WithMany(folder => folder.Files)
      .HasForeignKey(a => a.ParentId)
      .OnDelete(DeleteBehavior.SetNull);

    builder.HasIndex(a => a.ParentId);
  }
}