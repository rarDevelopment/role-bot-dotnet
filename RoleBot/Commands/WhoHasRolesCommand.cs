using DiscordDotNetUtilities.Interfaces;
using RoleBot.BusinessLayer;
using RoleBot.Helpers;

namespace RoleBot.Commands;

public class WhoHasRolesCommand(IRoleBusinessLayer roleBusinessLayer,
        RoleHelper roleHelper,
        IDiscordFormatter discordFormatter)
    : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("who-has-role", "List the users who have the specified role")]
    public async Task WhoHasSlashCommand([Summary("Role", "The role to check (non-mentionable roles cannot be checked)")] IRole roleToCheck)
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

        var guildRoles = await roleBusinessLayer.GetGuildRoles(Context.Guild.Id.ToString());

        if (!guildRoles.Any())
        {
            await FollowupAsync(embed:
                discordFormatter.BuildErrorEmbedWithUserFooter("No Roles",
                    "There are no roles configured with this bot.",
                    Context.User));
            return;
        }

        if (!await roleHelper.IsValidRole(roleToCheck, Context.Guild))
        {
            await FollowupAsync(embed:
                discordFormatter.BuildErrorEmbedWithUserFooter("Invalid Role",
                    "Sorry, this is not a valid role for this bot to check. Use /list-roles to see which roles the bot can manage.",
                    requestingUser));
            return;
        }

        var members = Context.Guild.Users.Where(u => u.Roles.Contains(roleToCheck));
        var membersToDisplay = members.OrderBy(m => m.Username.ToLower()).Select(m => m.Mention);

        var embedBuilder = discordFormatter.BuildRegularEmbedWithUserFooter($"Users with Role: {roleToCheck.Name}",
            $"{string.Join("\n", membersToDisplay)}",
            Context.User);
        await FollowupAsync(embed: embedBuilder);
    }
}