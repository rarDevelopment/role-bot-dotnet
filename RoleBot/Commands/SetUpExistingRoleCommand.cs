using DiscordDotNetUtilities.Interfaces;
using RoleBot.BusinessLayer;
using RoleBot.Helpers;

namespace RoleBot.Commands;

public class SetUpExistingRoleCommand(IRoleBusinessLayer roleBusinessLayer,
        RoleHelper roleHelper,
        IDiscordFormatter discordFormatter)
    : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("set-existing-role", "Allow or disallow the bot to control an existing role.")]
    public async Task SetUpExistingRoleSlashCommand(
        [Summary("role", "The role to allow or disallow")] IRole role,
        [Summary("controllable", "Whether or not the role should be controllable by the bot")] bool setControllable
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
                discordFormatter.BuildErrorEmbedWithUserFooter("Insufficient Permissions",
                    "Sorry, you do not have permission to control roles with the bot.",
                    Context.User));
            return;
        }

        var validRoles = await roleBusinessLayer.GetGuildRoles(Context.Guild.Id);
        var isRoleControllable = validRoles.Any(r => r.RoleId == role.Id);
        if (setControllable)
        {
            if (isRoleControllable)
            {
                await FollowupAsync(embed:
                    discordFormatter.BuildErrorEmbedWithUserFooter("Role Already Controllable",
                        "Sorry, this role is already controllable by the bot.",
                        Context.User));
                return;
            }

            await roleBusinessLayer.SaveRole(Context.Guild.Id, role.Id);
        }
        else
        {
            if (!isRoleControllable)
            {
                await FollowupAsync(embed:
                    discordFormatter.BuildErrorEmbedWithUserFooter("Role Is Not Controllable",
                        "Sorry, this role is not controllable by the bot so there's nothing to change.",
                        Context.User));
                return;
            }
            await roleBusinessLayer.DeleteRole(Context.Guild.Id, role.Id);
        }

        await FollowupAsync(embed: discordFormatter.BuildRegularEmbedWithUserFooter("Role Updated",
            $"Updated Role: **{role.Name}**\n" +
            $"Can The Bot Control This Role: **{(setControllable ? "Yes" : "No")}**",
            Context.User));
    }
}