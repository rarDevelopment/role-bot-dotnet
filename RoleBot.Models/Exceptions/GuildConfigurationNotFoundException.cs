namespace RoleBot.Models.Exceptions;

public class GuildConfigurationNotFoundException(string guildId) : Exception($"Guild configuration not found for guild with id {guildId}");