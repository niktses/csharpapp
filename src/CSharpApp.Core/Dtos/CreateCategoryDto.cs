using System.Text.Json.Serialization;

namespace CSharpApp.Core.Dtos;

public sealed class CreateCategoryDto
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = null!;

    [JsonPropertyName("image")]
    public string Image { get; set; } = null!;
}
