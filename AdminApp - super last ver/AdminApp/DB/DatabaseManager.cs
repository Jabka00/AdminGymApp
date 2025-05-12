using MongoDB.Driver;
using System;
using System.Threading.Tasks;

namespace AdminApp.DB
{
    public class DatabaseManager
    {
        private readonly IMongoDatabase _database;
        private readonly MongoClient _client;
    
        public DatabaseManager(string connectionString, string databaseName)
        {
            _client = new MongoClient(connectionString);
            _database = _client.GetDatabase(databaseName);
        }
    
        public IMongoCollection<T> GetCollection<T>(string collectionName)
        {
            return _database.GetCollection<T>(collectionName);
        }
    
        public async Task ConnectAsync()
        {
            try
            {
                await _database.ListCollectionNamesAsync();
                Console.WriteLine("Connected to MongoDB successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error connecting to MongoDB: {ex.Message}");
                throw;
            }
        }
    }
}