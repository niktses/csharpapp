using System.Text.Json.Serialization;

namespace CSharpApp.Core.Dtos;

public sealed class UpdateCategoryDto
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = null!;
}
