using System.Text.Json.Serialization;

namespace RyuBot.Services;

public class GitLabProjectPackageJsonResponse
{
    [JsonPropertyName("id")] public long Id { get; set; }

    [JsonPropertyName("name")] public string Name { get; set; } = null!;

    [JsonPropertyName("version")] public string Version { get; set; } = null!;

    [JsonPropertyName("package_type")] public string PackageType { get; set; } = null!;

    [JsonPropertyName("created_at")] public DateTimeOffset CreatedAt { get; set; }
}