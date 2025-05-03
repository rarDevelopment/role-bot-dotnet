namespace RoleBot.Models.Exceptions;

public class InsertFailedException : Exception
{
    public InsertFailedException(string guildId, string roleId) : base($"Failed to insert role with id {roleId} from guild {guildId}")
    { }
    public InsertFailedException(string guildId, string roleId, string userId) : base($"Failed to insert color role with id {roleId} from guild {guildId} for user {userId}")
    { }
}