namespace RoleBot.Models.Exceptions;

public class GuildConfigurationNotFoundException(ulong guildId) : Exception($"Guild configuration not found for guild with id {guildId}");