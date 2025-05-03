using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using RoleBot.Models;

namespace RoleBot.DataLayer.SchemaModels;

[BsonIgnoreExtraElements]
public class ConfigurationEntity
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    [BsonElement("guildId")]
    public string GuildId { get; set; }

    [BsonElement("guildName")]
    public string GuildName { get; set; }

    [BsonElement("allowedRoleIds")]
    public List<string> AllowedRoleIds { get; set; }
    [BsonElement("newUserRole")]
    public string? NewUserRole { get; set; }
    [BsonElement("enableColorChoosing")]
    public bool EnableColorChoosing { get; set; }

    public Configuration ToDomain()
    {
        return new Configuration
        {
            GuildId = GuildId,
            GuildName = GuildName,
            AllowedRoleIds = AllowedRoleIds.Select(r => Convert.ToUInt64(r)).ToList(),
            NewUserRole = Convert.ToUInt64(NewUserRole),
            EnableColorChoosing = EnableColorChoosing
        };
    }
}