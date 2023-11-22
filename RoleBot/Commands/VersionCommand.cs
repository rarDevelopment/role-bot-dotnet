using DiscordDotNetUtilities.Interfaces;
using RoleBot.Models;

namespace RoleBot.Commands;

public class VersionCommand(VersionSettings versionSettings, IDiscordFormatter discordFormatter)
    : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("version", "Get the current version number of the bot.")]
    public async Task VersionSlashCommand()
    {
        await RespondAsync(embed: discordFormatter.BuildRegularEmbedWithUserFooter("Bot Version",
            $"RoleBot is at version **{versionSettings.VersionNumber}**",
            Context.User));
    }
}