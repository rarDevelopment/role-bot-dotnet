using MongoDB.Driver;
using RoleBot.DataLayer.SchemaModels;
using RoleBot.Models;

namespace RoleBot.DataLayer;

public class ConfigurationDataLayer : IConfigurationDataLayer
{
    private readonly IMongoCollection<ConfigurationEntity> _configurationCollection;
    public ConfigurationDataLayer(DatabaseSettings databaseSettings)
    {
        var connectionString = $"mongodb+srv://{databaseSettings.User}:{databaseSettings.Password}@{databaseSettings.Cluster}.mongodb.net/{databaseSettings.Name}?w=majority";
        var client = new MongoClient(connectionString);
        var database = client.GetDatabase(databaseSettings.Name);
        _configurationCollection = database.GetCollection<ConfigurationEntity>("configuration");
    }

    public async Task<Configuration> GetConfigurationForGuild(string guildId, string guildName)
    {
        var filter = Builders<ConfigurationEntity>.Filter.Eq("guildId", guildId);
        var guildConfig = await _configurationCollection.Find(filter).FirstOrDefaultAsync();
        if (guildConfig != null)
        {
            return guildConfig.ToDomain();
        }

        await InitGuildConfiguration(guildId, guildName);

        guildConfig = await _configurationCollection.Find(filter).FirstOrDefaultAsync();
        return guildConfig.ToDomain();
    }

    public async Task<bool> AddAllowedRoleId(string guildId, string guildName, ulong roleId)
    {
        var existingConfig = await GetConfigurationForGuild(guildId, guildName);
        var filter = Builders<ConfigurationEntity>.Filter.Eq("guildId", guildId);
        var updatedAllowedRoleIds = existingConfig.AllowedRoleIds;
        updatedAllowedRoleIds.Add(roleId);
        var updatedAllowedRoleIdStrings = updatedAllowedRoleIds.Select(r => r.ToString()); //TODO: remove this when changing to numbers
        var update = Builders<ConfigurationEntity>.Update.Set(config => config.AllowedRoleIds, updatedAllowedRoleIdStrings);
        var updateResult = await _configurationCollection.UpdateOneAsync(filter, update);
        return updateResult.MatchedCount == 1;
    }

    public async Task<bool> RemoveAllowedRoleId(string guildId, string guildName, ulong roleId)
    {
        var existingConfig = await GetConfigurationForGuild(guildId, guildName);
        var filter = Builders<ConfigurationEntity>.Filter.Eq("guildId", guildId);
        var updatedAllowedRoleIds = existingConfig.AllowedRoleIds;
        updatedAllowedRoleIds.Remove(roleId);
        var updatedAllowedRoleIdStrings = updatedAllowedRoleIds.Select(r => r.ToString()); //TODO: remove this when changing to numbers
        var update = Builders<ConfigurationEntity>.Update.Set(config => config.AllowedRoleIds, updatedAllowedRoleIdStrings);
        var updateResult = await _configurationCollection.UpdateOneAsync(filter, update);
        return updateResult.MatchedCount == 1;
    }

    public async Task<bool> SetNewUserRole(string guildId, ulong? roleId)
    {
        var filter = Builders<ConfigurationEntity>.Filter.Eq("guildId", guildId);
        var update = Builders<ConfigurationEntity>.Update.Set(config => config.NewUserRole, roleId?.ToString() ?? null);
        var updateResult = await _configurationCollection.UpdateOneAsync(filter, update);
        return updateResult.ModifiedCount == 1 || updateResult.MatchedCount == 1;
    }

    public async Task<bool> SetEnableColorChoosing(string guildId, bool isEnabled)
    {
        var filter = Builders<ConfigurationEntity>.Filter.Eq("guildId", guildId);
        var update = Builders<ConfigurationEntity>.Update.Set(config => config.EnableColorChoosing, isEnabled);
        var updateResult = await _configurationCollection.UpdateOneAsync(filter, update);
        return updateResult.ModifiedCount == 1 || updateResult.MatchedCount == 1;
    }

    private async Task InitGuildConfiguration(string guildId, string guildName)
    {
        await _configurationCollection.InsertOneAsync(new ConfigurationEntity
        {
            GuildId = guildId,
            GuildName = guildName,
            AllowedRoleIds = [],
            EnableColorChoosing = false
        });
    }
}