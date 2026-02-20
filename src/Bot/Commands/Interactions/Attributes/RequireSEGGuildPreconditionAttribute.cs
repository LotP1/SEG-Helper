using Discord.Interactions;

namespace Bot.Commands.Interactions;

public class RequireSEGGuildPreconditionAttribute : PreconditionAttribute
{
    public override Task<PreconditionResult> CheckRequirementsAsync(
        IInteractionContext context,
        ICommandInfo commandInfo,
        IServiceProvider services)
    {
        if (context.Guild is null)
            return Task.FromResult(PreconditionResult.FromError("This command must be run in a guild."));

        return Task.FromResult(
            context.Guild.Id is not 1344396769630617600
                ? PreconditionResult.FromError(
                    $"This command can only be run in Switch Emulation Group.")
                : PreconditionResult.FromSuccess()
        );
    }
}