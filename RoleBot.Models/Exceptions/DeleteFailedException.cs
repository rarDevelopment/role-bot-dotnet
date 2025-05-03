namespace RoleBot.Models.Exceptions;

public class DeleteFailedException : Exception
{
    public DeleteFailedException(string guildId, string roleId) : base($"Failed to delete role with id {roleId} from guild {guildId}")
    {
    }
    public DeleteFailedException(string guildId, string roleId, string userId) : base($"Failed to delete color role with id {roleId} from guild {guildId} for user {userId}")
    {
    }
}