namespace Shared.Extensions.Tests;

public class StringExtensionsTests
{
  [Fact]
  public void ToSnakeCase_ReturnsNull_WhenInputIsNull()
  {
    string? input = null;
    var result = input!.ToSnakeCase();
    Assert.Null(result);
  }

  [Fact]
  public void ToSnakeCase_ReturnsEmpty_WhenInputIsEmpty()
  {
    var result = "".ToSnakeCase();
    Assert.Equal(string.Empty, result);
  }

  [Theory]
  [InlineData("hello", "hello")]
  [InlineData("Hello", "hello")]
  [InlineData("HelloWorld", "hello_world")]
  [InlineData("helloWorld", "hello_world")]
  [InlineData("already_snake_case", "already_snake_case")]
  [InlineData("Hello World", "hello__world")] // space -> '_' and uppercase W adds another '_'
  [InlineData("JSONData", "j_s_o_n_data")] // every uppercase triggers '_'
  [InlineData("File123Name", "file123_name")] // digits preserved; uppercase before Name adds '_'
  public void ToSnakeCase_Converts_AsExpected(string input, string expected)
  {
    var result = input.ToSnakeCase();
    Assert.Equal(expected, result);
  }

  [Fact]
  public void FromSnakeCase_ReturnsNull_WhenInputIsNull()
  {
    string? input = null;
    var result = input!.FromSnakeCase();
    Assert.Null(result);
  }

  [Fact]
  public void FromSnakeCase_ReturnsEmpty_WhenInputIsEmpty()
  {
    var result = "".FromSnakeCase();
    Assert.Equal(string.Empty, result);
  }

  [Theory]
  [InlineData("hello", "Hello")]
  [InlineData("hello_world", "Hello World")]
  [InlineData("my_snake_case_string", "My Snake Case String")]
  [InlineData("already_snake", "Already Snake")]
  [InlineData("multiple__underscores", "Multiple Underscores")]
  [InlineData("ends_with_underscore_", "Ends With Underscore")]
  [InlineData("_starts_with_underscore", "Starts With Underscore")]
  [InlineData("j_s_o_n_data", "JSONData")]
  public void FromSnakeCase_Converts_AsExpected(string input, string expected)
  {
    var result = input.FromSnakeCase();
    Assert.Equal(expected, result);
  }
}