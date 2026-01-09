using JV.ResultUtilities.Extensions;
using JV.ResultUtilities.ValidationMessage;

namespace Shared.S3ObjectStorage.Domain;

public static class ValidationKeys
{
  public static class Folder
  {
    public static readonly ValidationKeyDefinition FolderNotFound = ValidationKeyDefinition.Create("Folder.NotFound");

    public static readonly ValidationKeyDefinition CreationFailed =
      ValidationKeyDefinition.Create("Folder.CreationFailed");

    public static readonly ValidationKeyDefinition DeletionFailed =
      ValidationKeyDefinition.Create("Folder.DeletionFailed");

    public static readonly ValidationKeyDefinition UpdateFailed = ValidationKeyDefinition.Create("Folder.UpdateFailed");

    public static readonly ValidationKeyDefinition NameRequired = ValidationKeyDefinition.Create("Folder.NameRequired");
  }
  
  public static class File
  {
    public static readonly ValidationKeyDefinition FileNotFound = ValidationKeyDefinition.Create("File.NotFound")
      .WithStringParameter("FileId");

    public static readonly ValidationKeyDefinition CreationFailed = ValidationKeyDefinition.Create("File.CreationFailed")
      .WithStringParameter("FileName");
  }
}