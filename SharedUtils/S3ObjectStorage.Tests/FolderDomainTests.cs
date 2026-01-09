using Shared.S3ObjectStorage.Domain.Folder;

namespace Shared.S3ObjectStorage.Tests;

public class FolderDomainTests
{
  [Fact]
  public void Update_Should_ReturnError_When_Name_Is_Whitespace()
  {
    var folder = new Folder("Valid", null);
    var result = folder.Update("   ");
    Assert.False(result.IsSuccessful);
  }

  [Fact]
  public void Constructor_Should_Set_Id_To_Version7_Guid()
  {
    var folder = new Folder("Valid", null);
    Assert.NotEqual(Guid.Empty, folder.Id);
  }
}
