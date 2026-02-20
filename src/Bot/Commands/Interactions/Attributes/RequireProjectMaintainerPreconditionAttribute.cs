using Discord.Interactions;

namespace Bot.Commands.Interactions;

public class RequireProjectMaintainerPreconditionAttribute : PreconditionAttribute
{
    public const ulong GreemDev = 168548441939509248;
    public const ulong Keaton = 394186598071140383;
    public const ulong LotP = 341547992840404992;
    
    private static readonly ulong[] Maintainers = [ GreemDev, Keaton, LotP ];

    public override Task<PreconditionResult> CheckRequirementsAsync(
        IInteractionContext context,
        ICommandInfo commandInfo,
        IServiceProvider services) =>
        Task.FromResult(Maintainers.Contains(context.User.Id)
            ? PreconditionResult.FromSuccess()
            : PreconditionResult.FromError("Not an approved project maintainer.")
        );
}