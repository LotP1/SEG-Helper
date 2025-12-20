using System.Text.Json.Serialization;

namespace RyuBot.Services;

[JsonSerializable(typeof(IEnumerable<GitLabReleaseJsonResponse>))]
[JsonSerializable(typeof(IEnumerable<GitLabProjectPackageJsonResponse>))]
[JsonSerializable(typeof(GitLabReleaseJsonResponse[]))]
[JsonSerializable(typeof(GitLabProjectPackageJsonResponse[]))]
public partial class GitLabSerializerContexts : JsonSerializerContext;