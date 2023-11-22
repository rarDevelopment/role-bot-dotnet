namespace RoleBot.Models.Exceptions;

public class DeleteFailedException(ulong guildId, ulong roleId) : Exception($"Failed to delete role with id {roleId} from guild {guildId}");