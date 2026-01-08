using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shared.S3ObjectStorage.Domain.Folder;

namespace Shared.S3ObjectStorage.EF.EntityTypeConfigurations;

public class FolderConfiguration : IEntityTypeConfiguration<Folder>
{
  public void Configure(EntityTypeBuilder<Folder> builder)
  {
    builder.ToTable($"{Constants.TablePrefix}_folder");

    builder.HasKey(a => a.Id);
    builder.Property(a => a.Id).ValueGeneratedNever();

    builder.Property(a => a.Name)
      .IsRequired()
      .HasMaxLength(255);

    // Configure self-referencing relationship
    builder.HasOne(f => f.ParentFolder)
      .WithMany(f => f.ChildFolders)
      .HasForeignKey(f => f.ParentId)
      .OnDelete(DeleteBehavior.Restrict); // Prevent cascade delete to avoid orphaned folders

    // Optional: Add index on ParentId for better query performance
    builder.HasIndex(f => f.ParentId);
  }
}