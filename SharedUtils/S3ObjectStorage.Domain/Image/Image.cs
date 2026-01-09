using Shared.Domain;

namespace Shared.S3ObjectStorage.Domain.Image;

public class Image : Entity<Guid>
{
  public Guid OriginalFileId { get; set; } // Original file (unaltered)
  public Guid? ThumbnailFileId { get; set; } // Smallest size (e.g., 20px)
  public Guid? SmallFileId { get; set; } // Small version (e.g., 200px)
  public Guid? MediumFileId { get; set; } // Medium version (e.g., 400px)
  public Guid? LargeFileId { get; set; } // Large version (e.g., 600px)
  public Guid? ExtraLargeFileId { get; set; } // Extra-large version (e.g., 800px)
  public Guid? XXLargeFileId { get; set; } // Even larger version (e.g., 1000px)
  public Guid? MaxFileId { get; set; } // Maximum size version (e.g., 1200px)

  public File.File OriginalFile { get; set; }
  public File.File? ThumbnailFile { get; set; }
  public File.File? SmallFile { get; set; }
  public File.File? MediumFile { get; set; }
  public File.File? LargeFile { get; set; }
  public File.File? ExtraLargeFile { get; set; }
  public File.File? XXLargeFile { get; set; }
  public File.File? MaxFile { get; set; }

  public Image()
  {
    Id = Guid.CreateVersion7();
  }
}