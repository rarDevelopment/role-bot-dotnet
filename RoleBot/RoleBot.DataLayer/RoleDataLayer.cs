using MongoDB.Driver;
using RoleBot.DataLayer.SchemaModels;
using RoleBot.Models;

namespace RoleBot.DataLayer
{
    public class RoleDataLayer : IRoleDataLayer
    {
        private readonly IMongoCollection<GuildRoleEntity> _roleCollection;
        public RoleDataLayer(DatabaseSettings databaseSettings)
        {
            var connectionString = $"mongodb+srv://{databaseSettings.User}:{databaseSettings.Password}@{databaseSettings.Cluster}.mongodb.net/{databaseSettings.Name}?w=majority";
            var client = new MongoClient(connectionString);
            var database = client.GetDatabase(databaseSettings.Name);
            _roleCollection = database.GetCollection<GuildRoleEntity>("person");
        }
    }
}
