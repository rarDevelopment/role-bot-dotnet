namespace RoleBot.Models.Exceptions;

public class DeleteFailedException : Exception
{
    public DeleteFailedException(ulong guildId, ulong roleId) : base($"Failed to delete role with id {roleId} from guild {guildId}") { }
}