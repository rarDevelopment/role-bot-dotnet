using System.Drawing;
using DiscordDotNetUtilities.Interfaces;
using RoleBot.BusinessLayer;
using RoleBot.Helpers;
using Color = System.Drawing.Color;
using DiscordColor = Discord.Color;

namespace RoleBot.Commands;

public class ChooseExistingColorRoleCommand(IColorRoleBusinessLayer colorRoleBusinessLayer,
        IConfigurationBusinessLayer configurationBusinessLayer,
        RoleHelper roleHelper,
        IDiscordFormatter discordFormatter,
        ILogger<DiscordBot> logger)
    : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("set-existing-color-role", "Set an existing role for your color.")]
    public async Task ChooseExistingColorRoleSlashCommand(
        [Summary("Role", "Your existing color role")] IRole? role = null,
        [Summary("User", "The user for whom to add the role")]
        IGuildUser? otherUser = null
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

        var configuration =
            await configurationBusinessLayer.GetConfiguration(Context.Guild.Id.ToString(), Context.Guild.Name);

        var canAdministrate = await roleHelper.CanAdministrate(Context.Guild, requestingUser);

        if (!configuration.EnableColorChoosing && !canAdministrate)
        {
            await FollowupAsync(embed:
                discordFormatter.BuildErrorEmbedWithUserFooter("Color NOT Set",
                    configuration.EnableColorChoosing
                        ? "You do not have permission to set colors."
                        : "Color choosing is not enabled.",
                    Context.User));
            return;
        }

        if (otherUser != null && otherUser.Id != requestingUser.Id && !canAdministrate)
        {
            await FollowupAsync(embed:
                discordFormatter.BuildErrorEmbedWithUserFooter("Color NOT Set",
                    $"You do not have permission to set colors for other users.",
                    Context.User));
            return;
        }

        try
        {
            var userToAdd = otherUser ?? requestingUser;
            var userId = userToAdd.Id.ToString();

            //if role is null, get their existing color role (which would be the lowest Position role they have)
            var guildRoles = Context.Guild.Roles;
            var userRoles = guildRoles
                .Where(r => userToAdd.RoleIds.Contains(r.Id))
                .OrderByDescending(r => r.Position)
                .ToList();

            role ??= userRoles.FirstOrDefault(r => r.Color != DiscordColor.DarkBlue && !r.IsEveryone);

            if (role == null)
            {
                await FollowupAsync(embed:
                    discordFormatter.BuildErrorEmbedWithUserFooter("Color Role NOT Set",
                        $"The user {userToAdd.Mention} does not have any roles with a color.",
                        Context.User));
                return;
            }

            var userHasRole = userToAdd.RoleIds.Contains(role.Id);
            if (!userHasRole)
            {
                await FollowupAsync(embed:
                    discordFormatter.BuildErrorEmbedWithUserFooter("Color Role NOT Set",
                        $"The user {userToAdd.Mention} does not have the role {role.Mention}.",
                        Context.User));
                return;
            }

            await colorRoleBusinessLayer.SaveRole(Context.Guild.Id.ToString(), role.Id.ToString(), userId);

            var messageToSend = $"Role {role.Mention} was set as the color role for {userToAdd.Mention}";

            if (role.Color == DiscordColor.Default)
            {
                messageToSend += "\n**Note:** This role does not have a color.";
            }
            else
            {
                var color = GetColorFromHexCode(role.Color.ToString());
                if (color != null)
                {
                    messageToSend += $"\nThe color for this role is {FixColorForHexCode(role.Color.ToString())}";
                }
            }

            var addedRole = guildRoles.FirstOrDefault(r => r.Id == role.Id);

            var rolesWithColorsAboveNewRole = userRoles
                .Where(r => r.Color != DiscordColor.Default && r.Position > (addedRole?.Position ?? 0) && r.Position != 0)
                .ToList();

            if (rolesWithColorsAboveNewRole.Any())
            {
                messageToSend += "\n**Note:** The following role(s) may override this new color due to priority order (highest to lowest):\n";
                foreach (var roleAbove in rolesWithColorsAboveNewRole.OrderBy(r => r.Position))
                {
                    messageToSend += $"{roleAbove.Mention}\n";
                }
            }

            var embedBuilder = discordFormatter.BuildRegularEmbedWithUserFooter("Set Existing Color Role", messageToSend, Context.User);
            await FollowupAsync(embed: embedBuilder);
        }
        catch (Exception ex)
        {
            logger.LogError("Error creating color role: {0}", ex.ToString());
            await FollowupAsync(embed:
                discordFormatter.BuildErrorEmbedWithUserFooter("Error Creating Color Role",
                    "Sorry, there was an error creating that role.",
                    Context.User));
        }
    }


    private static Color? GetColorFromHexCode(string colorHexCode)
    {
        colorHexCode = FixColorForHexCode(colorHexCode);

        var colorConverter = new ColorConverter();
        if (!colorConverter.IsValid(colorHexCode))
        {
            return null;
        }
        var color = (Color?)colorConverter.ConvertFromString(colorHexCode);
        return color;
    }

    private static string FixColorForHexCode(string colorHexCode)
    {
        if (!colorHexCode.StartsWith("#"))
        {
            colorHexCode = $"#{colorHexCode}";
        }

        return colorHexCode.ToUpper();
    }
}