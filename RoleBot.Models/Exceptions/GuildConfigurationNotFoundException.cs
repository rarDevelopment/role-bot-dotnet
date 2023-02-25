namespace RoleBot.Models.Exceptions;

public class GuildConfigurationNotFoundException : Exception
{
    public GuildConfigurationNotFoundException(ulong guildId) : base($"Guild configuration not found for guild with id {guildId}") { }
}