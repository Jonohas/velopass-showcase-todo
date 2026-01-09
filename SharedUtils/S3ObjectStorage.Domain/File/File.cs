using Shared.Domain;

namespace Shared.S3ObjectStorage.Domain.File;

public class File : Entity<Guid>
{
  public string Bucket { get; set; } = default!;
  public string FileName { get; set; } = default!;
  public string Key { get; set; } = default!;
  public long FileSize { get; set; } // in bytes
  public string MimeType { get; set; } = default!;
  public Guid? ParentId { get; set; }

  public File()
  {
    Id = Guid.CreateVersion7();
  }

  // Navigation properties
  public Folder.Folder? ParentFolder { get; set; }
}