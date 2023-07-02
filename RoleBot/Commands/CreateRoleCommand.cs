﻿using System.Drawing;
using System.Text.RegularExpressions;
using DiscordDotNetUtilities.Interfaces;
using RoleBot.BusinessLayer;
using RoleBot.Helpers;
using Color = System.Drawing.Color;

namespace RoleBot.Commands;

public class CreateRoleCommand : InteractionModuleBase<SocketInteractionContext>
{
    private readonly IRoleBusinessLayer _roleBusinessLayer;
    private readonly IConfigurationBusinessLayer _configurationBusinessLayer;
    private readonly RoleHelper _roleHelper;
    private readonly IDiscordFormatter _discordFormatter;
    private readonly ILogger<DiscordBot> _logger;

    public CreateRoleCommand(IRoleBusinessLayer roleBusinessLayer,
        IConfigurationBusinessLayer configurationBusinessLayer,
        RoleHelper roleHelper,
        IDiscordFormatter discordFormatter,
        ILogger<DiscordBot> logger)
    {
        _roleBusinessLayer = roleBusinessLayer;
        _configurationBusinessLayer = configurationBusinessLayer;
        _roleHelper = roleHelper;
        _discordFormatter = discordFormatter;
        _logger = logger;
    }

    [SlashCommand("create-role", "Create a role.")]
    public async Task CreateRoleSlashCommand(
        [Summary("role", "The name of the role to add")] string roleName,
        [Summary("mentionable", "Whether or not the role should be mentionable by others")] bool isMentionable,
        [Summary("create_channel", "Creates a private channel associated with the new role")] bool createChannelForRole = false,
        [Summary("channel_category", "Category for the created channel")] ICategoryChannel? categoryChannel = null,
        [Summary("color", "Color hex code for the role")] string? colorHexCode = null
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

        var allRoles = Context.Guild.Roles;
        var existingRole = allRoles.FirstOrDefault(r => string.Equals(r.Name.Replace(" ", ""), roleName.Replace(" ", ""),
            StringComparison.InvariantCultureIgnoreCase));

        if (existingRole != null)
        {
            await FollowupAsync(embed:
                _discordFormatter.BuildErrorEmbed("Role With That Name Already Exists",
                    $"Sorry, the role {existingRole.Mention} already exists.",
                    Context.User));
            return;
        }

        Color? color = null;
        if (!string.IsNullOrEmpty(colorHexCode))
        {
            color = GetColorFromHexCode(colorHexCode);
            if (color == null)
            {
                await FollowupAsync(embed:
                _discordFormatter.BuildErrorEmbed("Invalid Color Specified",
                    $"Sorry, the hex code {colorHexCode} is not a valid hex color.",
                    Context.User));
                return;
            }
        }

        var fixedRoleName = ToTitleCase(roleName);

        try
        {
            var embedFields = new List<EmbedFieldBuilder>();

            if (!await _roleHelper.CanAdministrate(Context.Guild, requestingUser))
            {
                embedFields.Add(new EmbedFieldBuilder
                {
                    Name = "Role NOT Created",
                    Value = "No permission to create roles",
                    IsInline = false
                });
            }
            else
            {
                Discord.Color? roleColor = null;
                if (color != null)
                {
                    roleColor = new Discord.Color(color.Value.R, color.Value.G, color.Value.B);
                }

                var createdRole = await Context.Guild.CreateRoleAsync(fixedRoleName, GuildPermissions.None, roleColor, false,
                    isMentionable);
                await _roleBusinessLayer.SaveRole(Context.Guild.Id, createdRole.Id);

                embedFields.Add(new EmbedFieldBuilder
                {
                    Name = "Role Created",
                    Value = $"{createdRole.Mention}",
                    IsInline = false
                });

                if (createChannelForRole)
                {
                    if (requestingUser.GuildPermissions.ManageChannels
                        || await _configurationBusinessLayer.HasApprovedRole(Context.Guild.Id, Context.Guild.Name, requestingUser.RoleIds))
                    {
                        var channelName = ToKebabCase(roleName);

                        void ChannelPropertiesAction(TextChannelProperties c)
                        {
                            c.CategoryId = categoryChannel?.Id;
                            c.Topic = $"Discuss {fixedRoleName}";
                            c.PermissionOverwrites = new Optional<IEnumerable<Overwrite>>(new List<Overwrite>
                            {
                                new(createdRole.Id,
                                    PermissionTarget.Role,
                                    new OverwritePermissions(viewChannel: PermValue.Allow)),
                                new(Context.Guild.Id,
                                    PermissionTarget.Role,
                                    new OverwritePermissions(viewChannel: PermValue.Deny)),
                            });
                        }

                        var createdChannel =
                            await Context.Guild.CreateTextChannelAsync(channelName, ChannelPropertiesAction);

                        embedFields.Add(new EmbedFieldBuilder
                        {
                            Name = "Channel Created",
                            Value = $"{createdChannel.Mention} created.\nUse /add-role to add the role to access this channel.",
                            IsInline = false
                        });
                    }
                    else
                    {
                        embedFields.Add(new EmbedFieldBuilder
                        {
                            Name = "Channel NOT Created",
                            Value = "No permission to create channels.",
                            IsInline = false
                        });
                    }
                }
            }

            var embedBuilder = _discordFormatter.BuildRegularEmbed("Creating a Role",
                "",
                Context.User,
                embedFields);
            await FollowupAsync(embed: embedBuilder);
        }
        catch (Exception ex)
        {
            _logger.LogError("Error creating role: {0}", ex.Message);
            await FollowupAsync(embed:
                _discordFormatter.BuildErrorEmbed("Error Creating Role",
                    "Sorry, there was an error creating that role.",
                    Context.User));
        }
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

    private static string ToTitleCase(string text)
    {
        var regex = new Regex("^M{0,3}(CM|CD|D?C{0,3})(XC|XL|L?X{0,3})(IX|IV|V?I{0,3})$", RegexOptions.IgnoreCase);

        var fixedTextPieces = text
            .ToLower()
            .Split(" ")
            .Select(s =>
            {
                if (regex.IsMatch(s))
                {
                    return s.ToUpper();
                }
                return s[..1].ToUpper() + s[1..].ToLower();
            });

        return string.Join(" ", fixedTextPieces);
    }

    private static string ToKebabCase(string text)
    {
        return text.ToLower().Replace(" ", "-");
    }
}