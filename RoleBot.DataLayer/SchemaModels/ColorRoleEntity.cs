using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using RoleBot.Models;

namespace RoleBot.DataLayer.SchemaModels;

internal class ColorRoleEntity
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    [BsonElement("guildId")]
    public string GuildId { get; set; }

    [BsonElement("roleId")]
    public string RoleId { get; set; }
    [BsonElement("userId")]
    public string UserId { get; set; }

    public ColorRole ToDomain()
    {
        return new ColorRole
        {
            GuildId = GuildId,
            RoleId = RoleId,
            UserId = UserId
        };
    }
}