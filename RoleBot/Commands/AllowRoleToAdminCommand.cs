using DiscordDotNetUtilities.Interfaces;
using RoleBot.BusinessLayer;
using RoleBot.Helpers;

namespace RoleBot.Commands;

public class AllowRoleToAdminCommand : InteractionModuleBase<SocketInteractionContext>
{
    private readonly IConfigurationBusinessLayer _configurationBusinessLayer;
    private readonly RoleHelper _roleHelper;
    private readonly IDiscordFormatter _discordFormatter;

    public AllowRoleToAdminCommand(IConfigurationBusinessLayer configurationBusinessLayer,
        RoleHelper roleHelper,
        IDiscordFormatter discordFormatter)
    {
        _configurationBusinessLayer = configurationBusinessLayer;
        _roleHelper = roleHelper;
        _discordFormatter = discordFormatter;
    }

    [SlashCommand("allow-role-to-admin", "Set whether or not the specified role can use the bot to create roles and channels.")]
    public async Task AllowRoleToAdminSlashCommand(
        [Summary("role", "The name of the role to manage")] IRole roleToSet,
        [Summary("set-allowed", "Whether or not the role should be allowed to use the bot to create roles and related channels")] bool setAllowed
        )
    {
        await DeferAsync();

        if (Context.User is not IGuildUser requestingUser)
        {
            await FollowupAsync(embed:
                _discordFormatter.BuildErrorEmbedWithUserFooter("Invalid Action",
                    "Sorry, you need to be a valid user in a valid server to use this bot.",
                    Context.User));
            return;
        }

        if (!await _roleHelper.CanAdministrate(Context.Guild, requestingUser))
        {
            await FollowupAsync(embed:
                _discordFormatter.BuildErrorEmbedWithUserFooter("Insufficient Permissions",
                    "Sorry, you do not have permission to manage the bot.",
                    Context.User));
            return;
        }

        var config = await _configurationBusinessLayer.GetConfiguration(Context.Guild.Id, Context.Guild.Name);
        var guildRoles = Context.Guild.Roles.Where(r => config.AllowedRoleIds.Contains(r.Id));
        var isRoleAllowed = guildRoles.Contains(roleToSet);

        if (setAllowed)
        {
            if (isRoleAllowed)
            {
                await FollowupAsync(embed:
                    _discordFormatter.BuildErrorEmbedWithUserFooter("Role Already Allowed",
                        "Sorry, this role is already allowed to manage the bot.",
                        Context.User));
                return;
            }
            await _configurationBusinessLayer.SetApprovedRole(Context.Guild.Id, Context.Guild.Name, roleToSet.Id, true);
        }
        else
        {
            if (!isRoleAllowed)
            {
                await FollowupAsync(embed:
                    _discordFormatter.BuildErrorEmbedWithUserFooter("Role Is Already Not Allowed",
                        "Sorry, this role is not allowed to manage the bot so there's nothing to change.",
                        Context.User));
                return;
            }
            await _configurationBusinessLayer.SetApprovedRole(Context.Guild.Id, Context.Guild.Name, roleToSet.Id, false);
        }

        await FollowupAsync(embed: _discordFormatter.BuildRegularEmbedWithUserFooter("Configuring Bot Permissions",
            $"The role {roleToSet.Mention} can now {(setAllowed ? "" : "no longer")} manage the bot.",
            Context.User));
    }
}