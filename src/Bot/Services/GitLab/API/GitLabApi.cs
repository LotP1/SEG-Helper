using System.Net;
using System.Net.Http.Headers;
using System.Text.Json.Serialization.Metadata;
using Octokit;
using RyuBot.Services.Helpers;

namespace RyuBot.Services;

public static class GitLabApi
{
    public static Task<GitLabReleaseJsonResponse> GetLatestReleaseAsync(HttpClient httpClient, long projectId) 
        => GetReleaseAsync(httpClient, projectId, "permalink/latest");

    public static Task<GitLabReleaseJsonResponse> GetReleaseAsync(HttpClient httpClient, long projectId, string tagName) =>
        httpClient.ReadContentAsJsonAsync(
            CreateRequest(HttpMethod.Get,
                $"api/v4/projects/{projectId}/releases/{tagName}"),
            GitLabSerializerContexts.Default.GitLabReleaseJsonResponse
        );
    
    public static async Task<bool> DeletePackageAsync(
        HttpClient http, long projectId, long packageId)
    {
        var resp = await http.SendAsync(
            CreateRequest(HttpMethod.Delete,
                $"api/v4/projects/{projectId}/packages/{packageId}")
        );

        return resp.StatusCode is HttpStatusCode.NoContent;
    }

    public static Task<GitLabProjectPackageJsonResponse> FindMatchingPackageAsync(
        HttpClient httpClient, long projectId, 
        Func<GitLabProjectPackageJsonResponse, bool> matcher)
    {
        var p = PaginatedEndpoint<GitLabProjectPackageJsonResponse>.Builder(httpClient)
            .WithBaseUrl($"api/v4/projects/{projectId}/packages")
            .WithJsonContentParser(GitLabSerializerContexts.Default.IEnumerableGitLabProjectPackageJsonResponse)
            .WithPerPageCount(100)
            .WithQueryStringParameters(
                QueryParameters.Sort("desc"),
                QueryParameters.OrderBy("created_at"),
                ("package_type", "generic")
            ).Build();

        return p.FindOneAsync(predicate: matcher,
            onNonSuccess: _ => Error(LogSource.Bot, "Target project has the package registry disabled.")
        );
    }

    private static HttpRequestMessage CreateRequest(HttpMethod method, string uri)
        => new HttpRequestMessage(method, $"{Config.GitLabAuth.InstanceUrl.TrimEnd('/')}/{uri.TrimStart('/')}").Apply(m =>
        {
            m.Headers.Authorization = AuthenticationHeaderValue.Parse($"Bearer {Config.GitLabAuth.AccessToken}");
        });

    public static async Task<T> ReadContentAsJsonAsync<T>(
        this HttpClient httpClient,
        HttpRequestMessage message,
        JsonTypeInfo<T> typeInfo)
    {
        var response = await httpClient.SendAsync(message);

        return JsonSerializer.Deserialize(await response.Content.ReadAsStringAsync(), typeInfo);
    }
}