using DiscordDotNetUtilities.Interfaces;
using RoleBot.BusinessLayer;
using RoleBot.Helpers;

namespace RoleBot.Commands;

public class SetUpNewUserRoleCommand(IConfigurationBusinessLayer configurationBusinessLayer,
        RoleHelper roleHelper,
        IDiscordFormatter discordFormatter)
    : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("set-new-user-role", "Set a role to be assigned to every new user.")]
    public async Task SetUpNewUserRoleSlashCommand(
        [Summary("role", "The role to assign to new users")] IRole? role = null)
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

        if (!await roleHelper.CanAdministrate(Context.Guild, requestingUser))
        {
            await FollowupAsync(embed:
                discordFormatter.BuildErrorEmbedWithUserFooter("Insufficient Permissions",
                    "Sorry, you do not have permission to control roles with the bot.",
                    Context.User));
            return;
        }

        var didUpdate = await configurationBusinessLayer.SetNewUserRole(Context.Guild.Id, Context.Guild.Name, role?.Id);

        if (didUpdate)
        {
            await FollowupAsync(embed: discordFormatter.BuildRegularEmbedWithUserFooter("Role Updated",
                role != null
                    ? $"Set the role **{role.Name}** to be assigned to new users who join the server.\n"
                    : "Removed the new user role.",
                Context.User));
        }
        else
        {
            await FollowupAsync(embed:
                discordFormatter.BuildErrorEmbedWithUserFooter("Error Setting Role",
                    "Sorry, I wasn't able to set the new user role.",
                    requestingUser));
        }
    }
}