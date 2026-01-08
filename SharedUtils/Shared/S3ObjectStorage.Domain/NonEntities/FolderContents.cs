using Domain_File = Shared.S3ObjectStorage.Domain.File.File;

namespace Shared.S3ObjectStorage.Domain.NonEntities;

public class FolderContents
{
  public string Name { get; set; } = default!;
  public Guid Id { get; set; }
  public bool IsFolder { get; set; }
  public long? Size { get; set; }

  public static FolderContents Map(Folder.Folder folder)
  {
    return new FolderContents
    {
      Name = folder.Name,
      Id = folder.Id,
      IsFolder = true
    };
  }

  public static FolderContents Map(Domain_File file)
  {
    return new FolderContents
    {
      Name = file.FileName,
      Id = file.Id,
      IsFolder = false,
      Size = file.FileSize
    };
  }
}