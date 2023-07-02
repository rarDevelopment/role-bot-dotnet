using DiscordDotNetUtilities.Interfaces;
using RoleBot.Models;

namespace RoleBot.Commands;

public class VersionCommand : InteractionModuleBase<SocketInteractionContext>
{
    private readonly VersionSettings _versionSettings;
    private readonly IDiscordFormatter _discordFormatter;

    public VersionCommand(VersionSettings versionSettings, IDiscordFormatter discordFormatter)
    {
        _versionSettings = versionSettings;
        _discordFormatter = discordFormatter;
    }

    [SlashCommand("version", "Get the current version number of the bot.")]
    public async Task VersionSlashCommand()
    {
        await RespondAsync(embed: _discordFormatter.BuildRegularEmbed("Bot Version",
            $"RoleBot is at version **{_versionSettings.VersionNumber}**",
            Context.User));
    }
}