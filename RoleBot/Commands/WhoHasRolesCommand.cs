using DiscordDotNetUtilities.Interfaces;
using RoleBot.BusinessLayer;
using RoleBot.Helpers;

namespace RoleBot.Commands;

public class WhoHasRolesCommand : InteractionModuleBase<SocketInteractionContext>
{
    private readonly IRoleBusinessLayer _roleBusinessLayer;
    private readonly RoleHelper _roleHelper;
    private readonly IDiscordFormatter _discordFormatter;

    public WhoHasRolesCommand(IRoleBusinessLayer roleBusinessLayer,
        RoleHelper roleHelper,
        IDiscordFormatter discordFormatter)
    {
        _roleBusinessLayer = roleBusinessLayer;
        _roleHelper = roleHelper;
        _discordFormatter = discordFormatter;
    }

    [SlashCommand("who-has-role", "List the users who have the specified role")]
    public async Task WhoHasSlashCommand([Summary("Role", "The role to check (non-mentionable roles cannot be checked)")] IRole roleToCheck)
    {
        await DeferAsync();

        if (Context.User is not IGuildUser requestingUser)
        {
            await FollowupAsync(embed:
                _discordFormatter.BuildErrorEmbed("Invalid Action",
                    "Sorry, you need to be a valid user in a valid server to use this bot.",
                    Context.User));
            return;
        }

        var guildRoles = await _roleBusinessLayer.GetGuildRoles(Context.Guild.Id);

        if (!guildRoles.Any())
        {
            await FollowupAsync(embed:
                _discordFormatter.BuildErrorEmbed("No Roles",
                    "There are no roles configured with this bot.",
                    Context.User));
            return;
        }

        if (!await _roleHelper.IsValidRole(roleToCheck, Context.Guild))
        {
            await FollowupAsync(embed:
                _discordFormatter.BuildErrorEmbed("Invalid Role",
                    "Sorry, this is not a valid role for this bot to check. Use /list-roles to see which roles the bot can manage.",
                    requestingUser));
            return;
        }

        var members = Context.Guild.Users.Where(u => u.Roles.Contains(roleToCheck));
        var membersToDisplay = members.OrderBy(m => m.Username.ToLower()).Select(m => m.Mention);

        var embedBuilder = _discordFormatter.BuildRegularEmbed($"Users with Role: {roleToCheck.Name}",
            $"{string.Join("\n", membersToDisplay)}",
            Context.User);
        await FollowupAsync(embed: embedBuilder);
    }
}