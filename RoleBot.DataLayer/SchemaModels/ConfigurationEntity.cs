﻿using MongoDB.Bson;
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
    public ulong GuildId { get; set; }

    [BsonElement("guildName")]
    public string GuildName { get; set; }

    [BsonElement("allowedRoleIds")]
    public List<string> AllowedRoleIds { get; set; }

    public Configuration ToDomain()
    {
        return new Configuration
        {
            GuildId = GuildId,
            GuildName = GuildName,
            AllowedRoleIds = AllowedRoleIds.Select(r => Convert.ToUInt64(r)).ToList()
        };
    }
}