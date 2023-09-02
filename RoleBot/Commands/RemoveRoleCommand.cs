using DiscordDotNetUtilities.Interfaces;
using RoleBot.Helpers;

namespace RoleBot.Commands;

public class RemoveRoleCommand : InteractionModuleBase<SocketInteractionContext>
{
    private readonly RoleHelper _roleHelper;
    private readonly IDiscordFormatter _discordFormatter;

    public RemoveRoleCommand(RoleHelper roleHelper, IDiscordFormatter discordFormatter)
    {
        _roleHelper = roleHelper;
        _discordFormatter = discordFormatter;
    }

    [SlashCommand("remove-role", "Remove a role from yourself or another user.")]
    public async Task RemoveRoleSlashCommand(
        [Summary("Role", "The role to remove")] IRole roleToRemove,
        [Summary("User", "The user for whom to remove the role")] IGuildUser? otherUser = null
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

        if (!await _roleHelper.IsValidRole(roleToRemove, Context.Guild))
        {
            await FollowupAsync(embed:
                _discordFormatter.BuildErrorEmbedWithUserFooter("Invalid Role",
                    "Sorry, this is not a valid role for this bot to remove.",
                    requestingUser));
            return;
        }

        if (otherUser != null && !await _roleHelper.CanAdministrate(Context.Guild, requestingUser))
        {
            await FollowupAsync(embed:
                _discordFormatter.BuildErrorEmbedWithUserFooter("Insufficient Permissions",
                    "Sorry, you do not have the required permissions to remove roles from other users.",
                    requestingUser));
            return;
        }

        var userToRemove = otherUser ?? requestingUser;
        var roleWasRemoved = await RemoveRoleFromUser(roleToRemove, userToRemove);

        var messageToSend = roleWasRemoved
            ? $"Removed role {roleToRemove.Mention} for {userToRemove.Mention}"
            : $"Role was **NOT** removed - {userToRemove.Mention} does not have the role {roleToRemove.Mention}";
        var embedBuilder = _discordFormatter.BuildRegularEmbedWithUserFooter("Removed Role", messageToSend, Context.User);
        await FollowupAsync(embed: embedBuilder);
    }

    private static async Task<bool> RemoveRoleFromUser(IRole roleToRemove, IGuildUser otherUser)
    {
        if (!otherUser.RoleIds.Contains(roleToRemove.Id))
        {
            return false;
        }
        await otherUser.RemoveRoleAsync(roleToRemove);
        return true;
    }
}