using DiscordDotNetUtilities.Interfaces;
using RoleBot.BusinessLayer;
using RoleBot.Helpers;

namespace RoleBot.Commands;

public class ManageRoleCommand : InteractionModuleBase<SocketInteractionContext>
{
    private readonly IRoleBusinessLayer _roleBusinessLayer;
    private readonly RoleHelper _roleHelper;
    private readonly IDiscordFormatter _discordFormatter;

    public ManageRoleCommand(IRoleBusinessLayer roleBusinessLayer,
        RoleHelper roleHelper,
        IDiscordFormatter discordFormatter)
    {
        _roleBusinessLayer = roleBusinessLayer;
        _roleHelper = roleHelper;
        _discordFormatter = discordFormatter;
    }

    [SlashCommand("manage-role", "Link or unlink a role so that the bot can manage it.")]
    public async Task ManageRoleSlashCommand(
        [Summary("role", "The name of the role to manage")] IRole role,
        [Summary("set-manageable", "Whether or not the role should be manageable by the bot")] bool setManageable
        )
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

        if (!await _roleHelper.CanAdministrate(Context.Guild, requestingUser))
        {
            await FollowupAsync(embed:
                _discordFormatter.BuildErrorEmbed("Insufficient Permissions",
                    "Sorry, you do not have permission to manage roles with the bot.",
                    Context.User));
            return;
        }

        var validRoles = await _roleBusinessLayer.GetGuildRoles(Context.Guild.Id);
        var isRoleManageable = validRoles.Any(r => r.RoleId == role.Id);
        if (setManageable)
        {
            if (isRoleManageable)
            {
                await FollowupAsync(embed:
                    _discordFormatter.BuildErrorEmbed("Role Already Manageable",
                        "Sorry, this role is already manageable by the bot.",
                        Context.User));
                return;
            }

            await _roleBusinessLayer.SaveRole(Context.Guild.Id, role.Id);
        }
        else
        {
            if (!isRoleManageable)
            {
                await FollowupAsync(embed:
                    _discordFormatter.BuildErrorEmbed("Role Is Not Manageable",
                        "Sorry, this role is not manageable by the bot so there's nothing to change.",
                        Context.User));
                return;
            }
            await _roleBusinessLayer.DeleteRole(Context.Guild.Id, role.Id);
        }

        await FollowupAsync(embed: _discordFormatter.BuildRegularEmbed("Configuring a Role",
            $"The role {role.Mention} can now {(setManageable ? "" : "no longer")} be managed by the bot.",
            Context.User));
    }
}