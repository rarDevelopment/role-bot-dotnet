using MongoDB.Bson;
using MongoDB.Driver;
using RoleBot.DataLayer.SchemaModels;
using RoleBot.Models;
using RoleBot.Models.Exceptions;

namespace RoleBot.DataLayer;

public class RoleDataLayer : IRoleDataLayer
{
    private readonly IMongoCollection<GuildRoleEntity> _roleCollection;
    public RoleDataLayer(DatabaseSettings databaseSettings)
    {
        var connectionString = $"mongodb+srv://{databaseSettings.User}:{databaseSettings.Password}@{databaseSettings.Cluster}.mongodb.net/{databaseSettings.Name}?w=majority";
        var client = new MongoClient(connectionString);
        var database = client.GetDatabase(databaseSettings.Name);
        _roleCollection = database.GetCollection<GuildRoleEntity>("role");
    }

    public async Task<IReadOnlyCollection<GuildRole>> GetRolesForGuild(string guildId)
    {
        var filter = Builders<GuildRoleEntity>.Filter.Eq("guildId", guildId);

        var guildRoles = await _roleCollection.Find(filter).ToListAsync();
        return guildRoles.Select(x => x.ToDomain()).ToList();
    }

    public async Task<GuildRole?> GetRole(string guildId, ulong roleId)
    {
        var filter = new BsonDocument
        {
            {
                "guildId", new BsonDocument
                {
                    {"$eq", guildId}
                }
            },
            {
                "roleId", new BsonDocument
                {
                    {"$eq", roleId.ToString()}
                }
            }
        };

        var guildRole = await _roleCollection.Find(filter).FirstOrDefaultAsync();
        return guildRole?.ToDomain();
    }

    public async Task SaveRole(string guildId, ulong roleId)
    {
        var existingRole = await GetRole(guildId, roleId);
        if (existingRole != null)
        {
            return;
        }

        await _roleCollection.InsertOneAsync(new GuildRoleEntity { GuildId = guildId, RoleId = roleId.ToString() });

        var insertedRole = await GetRole(guildId, roleId);
        if (insertedRole == null)
        {
            throw new InsertFailedException(guildId, roleId);
        }
    }

    public async Task DeleteRole(string guildId, ulong roleId)
    {
        var filter = new BsonDocument
        {
            {
                "guildId", new BsonDocument
                {
                    {"$eq", guildId}
                }
            },
            {
                "roleId", new BsonDocument
                {
                    {"$eq", roleId.ToString()}
                }
            }
        };

        var result = await _roleCollection.DeleteOneAsync(filter);
        if (result.DeletedCount != 1)
        {
            throw new DeleteFailedException(guildId, roleId);
        }
    }
}