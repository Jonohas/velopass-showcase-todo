using JV.ResultUtilities;
using JV.ResultUtilities.ValidationPipeline;
using Shared.Domain;

namespace Shared.S3ObjectStorage.Domain.Folder;

public class Folder : Entity<Guid>
{
  public string Name { get; private set; }
  public Guid? ParentId { get; private set; }

  public Folder(string name, Guid? parentId)
  {
    Id = Guid.CreateVersion7();
    Name = name;
    ParentId = parentId;
  }

  // Navigation properties
  public Folder? ParentFolder { get; set; }
  public ICollection<Folder> ChildFolders { get; set; } = new List<Folder>();
  public ICollection<File.File> Files { get; set; } = new List<File.File>();

  public static Result<Folder> Create(string name, Guid? parentId = null)
  {
    var newFolder = new Folder(name, parentId);

    return new ValidationPipeline<Folder>()
      .Validate(newFolder);
  }

  public Result Update(string name, Guid? parentId = null)
  {
    if (string.IsNullOrWhiteSpace(name))
      return Result.Error(ValidationKeys.Folder.NameRequired);

    Name = name;
    ParentId = parentId;

    return Result.Ok();
  }
}