using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using RoleBot.Models;

namespace RoleBot.DataLayer.SchemaModels
{
    public class GuildRoleEntity
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("guildId")]
        public string GuildId { get; set; }

        [BsonElement("roleId")]
        public string RoleId { get; set; }

        public GuildRole ToDomain()
        {
            return new GuildRole
            {
                GuildId = Convert.ToUInt64(GuildId),
                RoleId = Convert.ToUInt64(RoleId)
            };
        }
    }
}
