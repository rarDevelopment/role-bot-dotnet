using System.Text.RegularExpressions;
using DiscordDotNetUtilities.Interfaces;
using RoleBot.BusinessLayer;
using RoleBot.Helpers;

namespace RoleBot.Commands;

public class CreateRoleCommand : InteractionModuleBase<SocketInteractionContext>
{
    private readonly IRoleBusinessLayer _roleBusinessLayer;
    private readonly RoleHelper _roleHelper;
    private readonly IDiscordFormatter _discordFormatter;
    private readonly ILogger<DiscordBot> _logger;

    public CreateRoleCommand(IRoleBusinessLayer roleBusinessLayer,
        RoleHelper roleHelper,
        IDiscordFormatter discordFormatter,
        ILogger<DiscordBot> logger)
    {
        _roleBusinessLayer = roleBusinessLayer;
        _roleHelper = roleHelper;
        _discordFormatter = discordFormatter;
        _logger = logger;
    }

    [SlashCommand("create-role", "Create a role.")]
    public async Task CreateRoleSlashCommand(
        [Summary("role", "The name of the role to add")] string roleName,
        [Summary("mentionable", "Whether or not the role should be mentionable by others")] bool isMentionable,
        [Summary("create_channel", "Creates a private role")] bool createChannelForRole = false,
        [Summary("channel_category", "Category for the created channel")] ICategoryChannel? categoryChannel = null
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
        var alreadyExists = allRoles.Any(r => string.Equals(r.Name.Replace(" ", ""), roleName.Replace(" ", ""),
            StringComparison.InvariantCultureIgnoreCase));

        if (alreadyExists)
        {
            await FollowupAsync(embed:
                _discordFormatter.BuildErrorEmbed("Role Name Already Exists",
                    "Sorry, that role name is taken.",
                    Context.User));
            return;
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
                var createdRole = await Context.Guild.CreateRoleAsync(fixedRoleName, GuildPermissions.None, null, false,
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
                    if (requestingUser.GuildPermissions.ManageChannels)
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

    private string ToTitleCase(string text)
    {
        var regex = new Regex("^M{0,3}(CM|CD|D?C{0,3})(XC|XL|L?X{0,3})(IX|IV|V?I{0,3})$");

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

    private string ToKebabCase(string text)
    {
        return text.ToLower().Replace(" ", "-");
    }
}