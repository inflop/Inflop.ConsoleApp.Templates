namespace ConsoleApp.Advanced.Data.Models;

/// <summary>
/// Example entity demonstrating database model.
/// </summary>
public class ExampleEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
