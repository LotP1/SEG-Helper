using Discord.Interactions;

namespace RyuBot.Commands.Interactions.Modules;

public partial class GitLabModule
{
    [SlashCommand("nuke-version",
        "Nuke a Canary version from the GitLab, deleting its release, package registry assets, and tag.")]
    [RequireBotOwnerPrecondition]
    public async Task<RuntimeResult> NukeVersionAsync(
        [Summary("version", "The version to delete.")]
        string version)
    {
        await DeferAsync(ephemeral: true);

        version = version.Replace("Canary-", string.Empty);
        try
        {
            GitLab.DeleteCanaryTag(version);

            await GitLab.DeleteCanaryPackageAsync(await GitLab.GetCanaryPackageAsync(version));
        }
        catch (Exception e)
        {
            Error(e);
            return BadRequest("Something went wrong. Check the logs for more detail.");
        }

        return Ok($"Deleted Canary v{version}.");
    }
}