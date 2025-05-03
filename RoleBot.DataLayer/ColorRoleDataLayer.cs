using MongoDB.Bson;
using MongoDB.Driver;
using RoleBot.DataLayer.SchemaModels;
using RoleBot.Models;
using RoleBot.Models.Exceptions;

namespace RoleBot.DataLayer;

public class ColorRoleDataLayer : IColorRoleDataLayer
{
    private readonly IMongoCollection<ColorRoleEntity> _roleCollection;
    public ColorRoleDataLayer(DatabaseSettings databaseSettings)
    {
        var connectionString = $"mongodb+srv://{databaseSettings.User}:{databaseSettings.Password}@{databaseSettings.Cluster}.mongodb.net/{databaseSettings.Name}?w=majority";
        var client = new MongoClient(connectionString);
        var database = client.GetDatabase(databaseSettings.Name);
        _roleCollection = database.GetCollection<ColorRoleEntity>("colorRole");
    }

    //public async Task<IReadOnlyCollection<ColorRole>> GetRolesForGuild(string guildId)
    //{
    //    var filter = Builders<ColorRoleEntity>.Filter.Eq("guildId", guildId);

    //    var guildRoles = await _roleCollection.Find(filter).ToListAsync();
    //    return guildRoles.Select(x => x.ToDomain()).ToList();
    //}

    public async Task<ColorRole?> GetColorRole(string guildId, string userId)
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
                "userId", new BsonDocument
                {
                    {"$eq", userId}
                }
            }
        };

        var colorRoleEntity = await _roleCollection.Find(filter).FirstOrDefaultAsync();
        return colorRoleEntity?.ToDomain();
    }

    public async Task<bool> SaveColorRole(string guildId, string roleId, string userId)
    {
        var existingRole = await GetColorRole(guildId, userId);
        if (existingRole != null)
        {
            var filter = Builders<ColorRoleEntity>.Filter.Eq(x => x.GuildId, guildId) &
                Builders<ColorRoleEntity>.Filter.Eq(x => x.UserId, userId);
            var update = Builders<ColorRoleEntity>.Update.Set(x => x.RoleId, roleId);
            var updateResult = await _roleCollection.UpdateOneAsync(filter, update);
            return updateResult.ModifiedCount == 1 || updateResult.MatchedCount == 1;
        }

        await _roleCollection.InsertOneAsync(new ColorRoleEntity { GuildId = guildId, RoleId = roleId, UserId = userId });

        var insertedRole = await GetColorRole(guildId, userId);
        if (insertedRole == null)
        {
            throw new InsertFailedException(guildId, roleId, userId);
        }

        return true;
    }

    public async Task DeleteColorRole(string guildId, string roleId, string userId)
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
                    {"$eq", roleId}
                }
            },
            {
                "userId", new BsonDocument
                {
                    {"$eq", userId}
                }
            }
        };

        var result = await _roleCollection.DeleteOneAsync(filter);
        if (result.DeletedCount != 1)
        {
            throw new DeleteFailedException(guildId, roleId, userId);
        }
    }
}