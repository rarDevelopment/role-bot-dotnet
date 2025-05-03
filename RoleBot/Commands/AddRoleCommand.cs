using DiscordDotNetUtilities.Interfaces;
using RoleBot.Helpers;

namespace RoleBot.Commands;

public class AddRoleCommand(RoleHelper roleHelper, IDiscordFormatter discordFormatter) : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("add-role", "Add a role to yourself or another user.")]
    public async Task AddRoleSlashCommand(
        [Summary("Role", "The role to add")] IRole roleToAdd,
        [Summary("User", "The user for whom to add the role")] IGuildUser? otherUser = null
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

        if (!await roleHelper.IsValidRole(roleToAdd, Context.Guild))
        {
            await FollowupAsync(embed:
                discordFormatter.BuildErrorEmbedWithUserFooter("Invalid Role",
                    "Sorry, this is not a valid role for this bot to add.",
                    requestingUser));
            return;
        }

        var canAdministrate = roleHelper.CanAdministrate(Context.Guild, requestingUser);

        if (otherUser != null && otherUser.Id != requestingUser.Id && !await canAdministrate)
        {
            await FollowupAsync(embed:
                discordFormatter.BuildErrorEmbedWithUserFooter("Insufficient Permissions",
                    "Sorry, you do not have the required permissions to assign roles to other users.",
                    requestingUser));
            return;
        }

        var userToAdd = otherUser ?? requestingUser;
        var roleWasAdded = await AddRoleToUser(roleToAdd, userToAdd);

        var messageToSend = roleWasAdded
            ? $"Added role {roleToAdd.Mention} for {userToAdd.Mention}"
            : $"Role was **NOT** added - {userToAdd.Mention} already has the role {roleToAdd.Mention}";
        var embedBuilder = discordFormatter.BuildRegularEmbedWithUserFooter("Added Role", messageToSend, Context.User);
        await FollowupAsync(embed: embedBuilder);
    }

    private static async Task<bool> AddRoleToUser(IRole roleToAdd, IGuildUser userToAdd)
    {
        if (userToAdd.RoleIds.Contains(roleToAdd.Id))
        {
            return false;
        }
        await userToAdd.AddRoleAsync(roleToAdd);
        return true;
    }
}