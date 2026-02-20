using Bot.Services;
using Discord.Interactions;

namespace Bot.Commands.Interactions.Modules;

public partial class CommandModule
{
    private const string Both = "Both";
    private const string Ryujinx = "Ryujinx";
    private const string Yuzu = "Yuzu";
    private static readonly string[] Options = [Both, Ryujinx, Yuzu];
    
    private const ulong RyujinxRoleId = 1473745250735362141;
    private const ulong YuzuRoleId = 1473745295912341658;
    
    [SlashCommand("emulator", "Sets your emulator roles.")]
    public async Task<RuntimeResult> SetEmulatorAsync(
        [Summary(description: "The type of emulator you work with.")]
        [Autocomplete(typeof(EmulatorAutocompleter))]
        string type)
    {
        if (Context.User is not SocketGuildUser member) return None();

        // if (member.HasRole(VerifierService.VerifiedSwitchOwnerRoleId))
        //     return BadRequest("You are already verified.");

        await DeferAsync(true);

        try
        {
            if (member.HasRole(RyujinxRoleId))
                await member.RemoveRoleAsync(RyujinxRoleId);
            
            if (member.HasRole(YuzuRoleId))
                await member.RemoveRoleAsync(YuzuRoleId);
            
            if (type != Yuzu)
                await member.AddRoleAsync(RyujinxRoleId);
            
            if (type != Ryujinx)
                await member.AddRoleAsync(YuzuRoleId);

            return Ok($"Success! Your roles have been updated.");
        }
        catch (Exception e)
        {
            await Verifier.SendVerificationModlogErrorMessageAsync("emulator", member, e);
            return BadRequest(
                "An internal error occurred when processing this command. It has been forwarded to the developer.");
        }
    }
    
    public class EmulatorAutocompleter : AutocompleteHandler
    {
        public override Task<AutocompletionResult> GenerateSuggestionsAsync(
            IInteractionContext context,
            IAutocompleteInteraction autocompleteInteraction,
            IParameterInfo parameter,
            IServiceProvider services)
        {
            foreach (var option in autocompleteInteraction.Data.Options)
            {
                var userValue = option.Value?.ToString();

                if (!option.Focused || string.Empty.Equals(userValue)) continue;

                var results = Options.Where(x => x.ContainsAnyIgnoreCase(userValue))
                    .Take(5).ToArray();

                if (results.Length > 0)
                    return Task.FromResult(AutocompletionResult.FromSuccess(
                        results.Select(it => new AutocompleteResult(it, it))
                    ));
            }

            return Task.FromResult(AutocompletionResult.FromSuccess(
                Options.Select(it => new AutocompleteResult(it, it))
            ));
        }
    }
}