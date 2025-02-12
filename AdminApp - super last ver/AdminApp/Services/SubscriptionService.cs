using AdminApp.DB;
using AdminApp.Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AdminApp.Services
{
    public class SubscriptionService
    {
        private readonly IMongoCollection<Subscription> _subscriptions;
    
        public SubscriptionService()
        {
            string connectionString = "mongodb+srv://Admin:strongpassword@cluster0.lrajj.mongodb.net";
            string databaseName = "GymDatabase";
            var subscriptionDbManager = new DatabaseManager(connectionString, databaseName);
            _subscriptions = subscriptionDbManager.GetCollection<Subscription>("subscriptions");
        }
    
        public async Task<List<Subscription>> GetAllSubscriptionsAsync()
        {
            return await _subscriptions.Find(_ => true).ToListAsync();
        }
    
        public async Task<Subscription?> GetSubscriptionByIdAsync(string id)
        {
            return await _subscriptions.Find(s => s.Id == id).FirstOrDefaultAsync();
        }
    
        public async Task<Subscription?> GetSubscriptionByUserIdAsync(string userId)
        {
            var filter = Builders<Subscription>.Filter.Eq(s => s.UserId, userId);
            return await _subscriptions.Find(filter).FirstOrDefaultAsync();
        }
    
        public async Task<bool> InsertSubscriptionAsync(Subscription subscription)
        {
            try
            {
                subscription.CreatedAt = DateTime.UtcNow;
                subscription.UpdatedAt = DateTime.UtcNow;
                await _subscriptions.InsertOneAsync(subscription);
                return true;
            }
            catch
            {
                return false;
            }
        }
    
        public async Task<bool> UpdateSubscriptionAsync(Subscription subscription)
        {
            try
            {
                subscription.UpdatedAt = DateTime.UtcNow;
                var result = await _subscriptions.ReplaceOneAsync(s => s.Id == subscription.Id, subscription);
                return result.IsAcknowledged && result.ModifiedCount > 0;
            }
            catch
            {
                return false;
            }
        }
    
        public async Task<bool> DeleteSubscriptionAsync(string id)
        {
            try
            {
                var result = await _subscriptions.DeleteOneAsync(s => s.Id == id);
                return result.IsAcknowledged && result.DeletedCount > 0;
            }
            catch
            {
                return false;
            }
        }
    }
}
