using Bot.Interactions;
using Bot.Services;

namespace Bot.Commands.Interactions.Modules;

[RequireSEGGuildPrecondition]
public partial class CommandModule : SEGBotSlashCommandModule
{
    public VerifierService Verifier { get; set; }
}