using DiscordDotNetUtilities.Interfaces;
using RoleBot.BusinessLayer;
using RoleBot.Helpers;

namespace RoleBot.Commands;

public class ListRolesCommand : InteractionModuleBase<SocketInteractionContext>
{
    private readonly IRoleBusinessLayer _roleBusinessLayer;
    private readonly RoleHelper _roleHelper;
    private readonly IDiscordFormatter _discordFormatter;

    public ListRolesCommand(IRoleBusinessLayer roleBusinessLayer,
        RoleHelper roleHelper,
        IDiscordFormatter discordFormatter)
    {
        _roleBusinessLayer = roleBusinessLayer;
        _roleHelper = roleHelper;
        _discordFormatter = discordFormatter;
    }

    [SlashCommand("list-roles", "List your roles and the available roles")]
    public async Task ListRolesSlashCommand()
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

        var validRoles = _roleHelper.GetValidRoles(Context.Guild, guildRoles);
        var rolesYouHave = new List<IRole>();
        var rolesAvailable = new List<IRole>();

        foreach (var role in validRoles)
        {
            if (requestingUser.RoleIds.Contains(role.Id))
            {
                rolesYouHave.Add(role);
            }
            else
            {
                rolesAvailable.Add(role);
            }
        }

        var rolesYouHaveField = new EmbedFieldBuilder
        {
            Name = "YOUR ROLES",
            Value = string.Join(", ", rolesYouHave),
            IsInline = false
        };

        var rolesAvailableField = new EmbedFieldBuilder
        {
            Name = "AVAILABLE ROLES",
            Value = string.Join(", ", rolesAvailable),
            IsInline = false
        };

        var embedBuilder = _discordFormatter.BuildRegularEmbed("Roles List",
            "",
            Context.User,
            new List<EmbedFieldBuilder>
        {
            rolesYouHaveField, rolesAvailableField
        });
        await FollowupAsync(embed: embedBuilder);
    }
}