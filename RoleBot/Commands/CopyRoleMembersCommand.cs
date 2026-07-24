using DiscordDotNetUtilities.Interfaces;
using RoleBot.BusinessLayer;
using RoleBot.Helpers;

namespace RoleBot.Commands;

public class CopyRoleMembersCommand(IRoleBusinessLayer roleBusinessLayer,
        IConfigurationBusinessLayer configurationBusinessLayer,
        RoleHelper roleHelper,
        IDiscordFormatter discordFormatter,
        ILogger<DiscordBot> logger)
    : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("copy-role-members", "Copy members of one role to another.")]
    public async Task CopyRoleMembersSlashCommand(
        [Summary("from_role", "The name of the role to copy members from")] IRole fromRole,
        [Summary("to_role", "The name of the role to copy members to")] IRole toRole
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

        if (!await roleHelper.CanAdministrate(Context.Guild, requestingUser))
        {
            await FollowupAsync(embed:
                discordFormatter.BuildErrorEmbedWithUserFooter("No Permission",
                    "You do not have permission to manage roles with this bot.",
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

        var validRoles = roleHelper.GetValidRoles(Context.Guild, guildRoles).ToList();
        if (!validRoles.Contains(fromRole) || fromRole is not SocketRole fromRoleInGuild)
        {
            await FollowupAsync(embed:
                discordFormatter.BuildErrorEmbedWithUserFooter("Invalid Role",
                    $"Sorry, the role {fromRole.Mention} is not valid for use with this bot.",
                    Context.User));
            return;
        }
        if (!validRoles.Contains(toRole) || fromRole is not SocketRole toRoleInGuild)
        {
            await FollowupAsync(embed:
                discordFormatter.BuildErrorEmbedWithUserFooter("Invalid Role",
                    $"Sorry, the role {toRole.Mention} is not valid for use with this bot.",
                    Context.User));
            return;
        }

        var fromRoleMembers = fromRoleInGuild.Members;

        var addedList = new List<SocketGuildUser>();
        var failedList = new List<SocketGuildUser>();

        foreach (var fromRoleMember in fromRoleMembers)
        {
            try
            {
                await fromRoleMember.AddRoleAsync(toRole);
                addedList.Add(fromRoleMember);
            }
            catch (Exception ex)
            {
                failedList.Add(fromRoleMember);
            }
        }

        try
        {
            var embedFields = new List<EmbedFieldBuilder>();


            if (addedList.Count > 0)
            {
                embedFields.Add(new EmbedFieldBuilder
                {
                    Name = $"Members Added to {toRole.Name}",
                    Value = $"{string.Join(", ", addedList)}",
                    IsInline = false
                });
            }

            if (failedList.Count > 0)
            {
                embedFields.Add(new EmbedFieldBuilder
                {
                    Name = $"Members Not Added to {toRole.Name}",
                    Value = $"{string.Join(", ", failedList)}",
                    IsInline = false
                });
            }

            if (embedFields.Count > 0)
            {
                var embedBuilder = discordFormatter.BuildRegularEmbedWithUserFooter("Creating a Role",
                    "",
                    Context.User,
                    embedFields);
                await FollowupAsync(embed: embedBuilder);
            }
            else
            {
                await FollowupAsync(embed:
                    discordFormatter.BuildErrorEmbedWithUserFooter("Error Copying Members to Role",
                        "Sorry, nothing happened for some reason.",
                        Context.User));
                return;
            }

        }
        catch (Exception ex)
        {
            logger.LogError("Error copying members to a role: {0}", ex.Message);
            await FollowupAsync(embed:
                discordFormatter.BuildErrorEmbedWithUserFooter("Error Copying Members to Role",
                    "Sorry, there was an error copying members to and/or from that role.",
                    Context.User));
        }
    }
}