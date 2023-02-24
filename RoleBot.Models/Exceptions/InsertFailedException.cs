namespace RoleBot.Models.Exceptions;

public class InsertFailedException : Exception
{
    public InsertFailedException(ulong guildId, ulong roleId) : base($"Failed to insert role with id {roleId} from guild {guildId}") { }
}