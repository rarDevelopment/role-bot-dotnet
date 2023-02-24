using DiscordDotNetUtilities.Interfaces;
using RoleBot.Helpers;

namespace RoleBot.Commands;

public class AddRoleCommand : InteractionModuleBase<SocketInteractionContext>
{
    private readonly RoleHelper _roleHelper;
    private readonly IDiscordFormatter _discordFormatter;

    public AddRoleCommand(RoleHelper roleHelper, IDiscordFormatter discordFormatter)
    {
        _roleHelper = roleHelper;
        _discordFormatter = discordFormatter;
    }

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
                _discordFormatter.BuildErrorEmbed("Invalid Action",
                    "Sorry, you need to be a valid user in a valid server to use this bot.",
                    Context.User));
            return;
        }

        if (!await _roleHelper.IsValidRole(roleToAdd, Context.Guild))
        {
            await FollowupAsync(embed:
                _discordFormatter.BuildErrorEmbed("Invalid Role",
                    "Sorry, this is not a valid role for this bot to add.",
                    requestingUser));
            return;
        }

        var canAdministrate = _roleHelper.CanAdministrate(Context.Guild, requestingUser);

        if (otherUser != null && !await canAdministrate)
        {
            await FollowupAsync(embed:
                _discordFormatter.BuildErrorEmbed("Insufficient Permissions",
                    "Sorry, you do not have the required permissions to assign roles to other users.",
                    requestingUser));
            return;
        }

        var userToAdd = otherUser ?? requestingUser;
        var roleWasAdded = await AddRoleToUser(roleToAdd, userToAdd);

        var messageToSend = roleWasAdded
            ? $"Added role {roleToAdd.Mention} for {userToAdd.Mention}"
            : $"Role was **NOT** added - {userToAdd.Mention} already has the role {roleToAdd.Mention}";
        var embedBuilder = _discordFormatter.BuildRegularEmbed("Added Role", messageToSend, Context.User);
        await FollowupAsync(embed: embedBuilder);
    }

    private static async Task<bool> AddRoleToUser(IRole roleToAdd, IGuildUser otherUser)
    {
        if (otherUser.RoleIds.Contains(roleToAdd.Id))
        {
            return false;
        }
        await otherUser.AddRoleAsync(roleToAdd);
        return true;
    }
}