namespace RoleBot.Models.Exceptions;

public class InsertFailedException(string guildId, ulong roleId) : Exception($"Failed to insert role with id {roleId} from guild {guildId}");