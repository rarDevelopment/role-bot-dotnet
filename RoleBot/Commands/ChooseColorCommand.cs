using System.Drawing;
using DiscordDotNetUtilities.Interfaces;
using RoleBot.BusinessLayer;
using RoleBot.Helpers;
using Color = System.Drawing.Color;

namespace RoleBot.Commands;

public class ChooseColorCommand(IColorRoleBusinessLayer colorRoleBusinessLayer,
        IConfigurationBusinessLayer configurationBusinessLayer,
        RoleHelper roleHelper,
        IDiscordFormatter discordFormatter,
        ILogger<DiscordBot> logger)
    : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("choose-color", "Choose a color.")]
    public async Task ChooseColorSlashCommand(
        [Summary("color", "Color hex code")] string colorHexCode,
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

        if (string.IsNullOrEmpty(colorHexCode))
        {
            await FollowupAsync(embed:
                discordFormatter.BuildErrorEmbedWithUserFooter("Invalid Color Specified",
                    "You need to specify a hex code color.",
                    Context.User));
            return;
        }

        var color = GetColorFromHexCode(colorHexCode);

        if (color == null)
        {
            await FollowupAsync(embed:
                discordFormatter.BuildErrorEmbedWithUserFooter("Invalid Color Specified",
                    $"Sorry, the hex code {colorHexCode} is not a valid hex color.",
                    Context.User));
            return;
        }

        try
        {
            var roleColor = new Discord.Color(color.Value.R, color.Value.G, color.Value.B);

            var userToAdd = otherUser ?? requestingUser;
            var userId = userToAdd.Id.ToString();

            var roleName = userToAdd.Username;

            var existingColorRole =
                await colorRoleBusinessLayer.GetColorRole(Context.Guild.Id.ToString(), userId);

            IRole? roleToAdd;

            if (existingColorRole != null)
            {
                try
                {
                    roleToAdd = await Context.Guild.GetRoleAsync(Convert.ToUInt64(existingColorRole.RoleId));
                }
                catch (Exception ex) when (ex is Discord.Net.HttpException && ex.Message.Contains("Unknown Role"))
                {
                    roleToAdd = null;
                }

                if (roleToAdd != null)
                {
                    await roleToAdd.ModifyAsync(x =>
                    {
                        x.Color = roleColor;
                        x.Name = roleName;
                    });
                }
                else
                {
                    roleToAdd = await CreateRole(roleName, roleColor, userId);
                }
            }
            else
            {
                roleToAdd = await CreateRole(roleName, roleColor, userId);
            }

            if (roleToAdd == null)
            {
                await FollowupAsync(embed:
                    discordFormatter.BuildErrorEmbedWithUserFooter("Failed to Create Role",
                        $"There was an error.",
                        Context.User));
                return;
            }

            var roleWasAdded = await AddRoleToUser(roleToAdd, userToAdd);

            var messageToSend = roleWasAdded
                ? $"Added color {roleToAdd.Mention} for {userToAdd.Mention}"
                : $"Color was **NOT** added - {userToAdd.Mention} already has the color role {roleToAdd.Mention}";

            var guildRoles = Context.Guild.Roles;
            var userRoles = guildRoles
                .Where(r => userToAdd.RoleIds.Contains(r.Id))
                .OrderBy(r => r.Position);
            var addedRole = guildRoles.FirstOrDefault(r => r.Id == roleToAdd.Id);

            var rolesWithColorsAboveNewRole = userRoles
                .Where(r => r.Color != Color.Black && r.Position > (addedRole?.Position ?? 0) && r.Position != 0)
                .ToList();


            if (rolesWithColorsAboveNewRole.Any())
            {
                messageToSend += "\n**Note:** The following role(s) may override this new color due to priority order:\n";
                foreach (var role in rolesWithColorsAboveNewRole)
                {
                    messageToSend += $"{role.Mention}\n";
                }
            }

            var embedBuilder = discordFormatter.BuildRegularEmbedWithUserFooter("Added Color Role", messageToSend, Context.User);
            await FollowupAsync(embed: embedBuilder);
        }
        catch (Exception ex)
        {
            logger.LogError("Error creating color role: {0}", ex.Message);
            await FollowupAsync(embed:
                discordFormatter.BuildErrorEmbedWithUserFooter("Error Creating Color Role",
                    "Sorry, there was an error creating that role.",
                    Context.User));
        }
    }

    private async Task<IRole?> CreateRole(string roleName, Discord.Color roleColor, string userId)
    {
        var roleToAdd = await Context.Guild.CreateRoleAsync(roleName, GuildPermissions.None, roleColor);
        await colorRoleBusinessLayer.SaveRole(Context.Guild.Id.ToString(), roleToAdd.Id.ToString(), userId);
        return roleToAdd;
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
    private static Color? GetColorFromHexCode(string colorHexCode)
    {
        if (!colorHexCode.StartsWith("#"))
        {
            colorHexCode = $"#{colorHexCode}";
        }

        var colorConverter = new ColorConverter();
        if (!colorConverter.IsValid(colorHexCode))
        {
            return null;
        }
        var color = (Color?)colorConverter.ConvertFromString(colorHexCode);
        return color;
    }
}