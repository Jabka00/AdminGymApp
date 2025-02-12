using AdminApp.DB;
using AdminApp.Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AdminApp.Services
{
    public class PurchaseService
    {
        private readonly IMongoCollection<Purchase> _purchases;
    
        public PurchaseService(DatabaseManager dbManager)
        {
            _purchases = dbManager.GetCollection<Purchase>("purchases");
        }
    
        public async Task<List<Purchase>> GetAllPurchasesAsync()
        {
            return await _purchases.Find(_ => true).ToListAsync();
        }
    
        public async Task<List<Purchase>> GetPurchasesByUserIdAsync(string userId)
        {
            var filter = Builders<Purchase>.Filter.Eq(p => p.UserId, userId);
            return await _purchases.Find(filter).SortByDescending(p => p.PurchaseDate).ToListAsync();
        }
    
        public async Task<bool> InsertPurchaseAsync(Purchase purchase)
        {
            try
            {
                purchase.CreatedAt = DateTime.UtcNow;
                purchase.UpdatedAt = DateTime.UtcNow;
                await _purchases.InsertOneAsync(purchase);
                return true;
            }
            catch
            {
                return false;
            }
        }
    
        public async Task<bool> DeletePurchaseAsync(string id)
        {
            try
            {
                var result = await _purchases.DeleteOneAsync(p => p.Id == id);
                return result.IsAcknowledged && result.DeletedCount > 0;
            }
            catch
            {
                return false;
            }
        }
    }
}