using Discord.Interactions;

namespace RyuBot.Commands.Interactions.Modules;

public partial class GitLabModule
{
    [SlashCommand("nuke-version",
        "Nuke at least one Canary version(s) from the GitLab, deleting its release, package, and RC repo tag.")]
    [RequireBotOwnerPrecondition]
    public async Task<RuntimeResult> NukeVersionAsync(
        [Summary("version", "The version(s) to delete. If multiple, separate by a single space.")]
        string versionFormat)
    {
        await DeferAsync(ephemeral: true);

        var versions = versionFormat.Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Select(x => x.Replace("Canary-", string.Empty));

        List<string> successfullyDeleted = [];
        List<string> deletionFailed = [];

        foreach (var version in versions)
        {
            try
            {
                GitLab.DeleteCanaryTag(version);

                if (await GitLab.GetCanaryPackageAsync(version).Then(GitLab.DeleteCanaryPackageAsync))
                    successfullyDeleted.Add(version);
                else
                    deletionFailed.Add(version);
            }
            catch (Exception e)
            {
                Error(LogSource.Module, $"Failed to properly delete Canary v{version}", e);
                deletionFailed.Add(version);
            }
        }

        return successfullyDeleted.Count switch
        {
            0 => BadRequest("Something went wrong. Check the logs for more detail."),
            1 => Ok(String(sb =>
            {
                sb.Append("Deleted Canary v");
                sb.Append(successfullyDeleted.First());
                sb.Append('.').AppendLine().AppendLine();

                if (deletionFailed.Count > 0)
                {
                    sb.AppendLine("Some versions failed to delete: ");
                    sb.Append(deletionFailed.FormatCollection(x => Format.Code(x), prefix: "[", suffix: "]"));
                }
            })),
            _ => Ok(String(sb =>
            {
                sb.AppendLine("Deleted the following canary versions: ");
                sb.AppendLine(successfullyDeleted.FormatCollection(x => Format.Code(x), prefix: "[", suffix: "]"));

                if (deletionFailed.Count > 0)
                {
                    sb.AppendLine("Some versions failed to delete: ");
                    sb.Append(deletionFailed.FormatCollection(x => Format.Code(x), prefix: "[", suffix: "]"));
                }
            }))
        };
    }
}