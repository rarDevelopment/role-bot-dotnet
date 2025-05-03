using DiscordDotNetUtilities.Interfaces;
using DiscordColor = Discord.Color;

namespace RoleBot.Commands;

public class CheckColorCommand(IDiscordFormatter discordFormatter)
    : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("check-name-color", "See the hex code of your current name color.")]
    public async Task CheckColorSlashCommand(
        [Summary("User", "The user for whom to check the name color")] IGuildUser? otherUser = null
    )
    {
        await DeferAsync();

        if (Context.User is not IGuildUser requestingUser)
        {
            await FollowupAsync(embed:
                discordFormatter.BuildErrorEmbedWithUserFooter("Invalid Action",
                    "Sorry, you need to be a valid user in a valid server to use this bot.",
                    Context.User));
            return;
        }
        var userToCheck = otherUser ?? requestingUser;

        var guildRoles = Context.Guild.Roles;
        var userRoles = guildRoles
            .Where(r => userToCheck.RoleIds.Contains(r.Id))
            .OrderByDescending(r => r.Position)
            .ToList();

        var colorRole = userRoles.FirstOrDefault(r => r.Color != DiscordColor.Default && !r.IsEveryone);

        if (colorRole == null)
        {
            await FollowupAsync(embed:
                discordFormatter.BuildErrorEmbedWithUserFooter("No Name Color",
                    $"{userToCheck.Mention} does not have a name color.",
                    Context.User));
            return;
        }

        var hex = colorRole.Color.ToString();

        await FollowupAsync(embed:
            discordFormatter.BuildRegularEmbedWithUserFooter("Name Color",
                $"{userToCheck.Mention} has a name color of {hex} because of role {colorRole.Mention}.",
                Context.User));
    }
}