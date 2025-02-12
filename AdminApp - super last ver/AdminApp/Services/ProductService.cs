using AdminApp.DB;
using AdminApp.Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AdminApp.Services
{
    public class ProductService
    {
        private readonly IMongoCollection<Product> _products;
    
        public ProductService(DatabaseManager dbManager)
        {
            _products = dbManager.GetCollection<Product>("products");
        }
    
        public async Task<List<Product>> GetAllProductsAsync()
        {
            return await _products.Find(_ => true).ToListAsync();
        }
    
        public async Task<Product?> GetProductByIdAsync(string id)
        {
            return await _products.Find(p => p.Id == id).FirstOrDefaultAsync();
        }
    
        public async Task<bool> InsertProductAsync(Product product)
        {
            try
            {
                product.CreatedAt = DateTime.UtcNow;
                product.UpdatedAt = DateTime.UtcNow;
                await _products.InsertOneAsync(product);
                return true;
            }
            catch
            {
                return false;
            }
        }
    
        public async Task<bool> UpdateProductAsync(Product product)
        {
            try
            {
                product.UpdatedAt = DateTime.UtcNow;
                var result = await _products.ReplaceOneAsync(p => p.Id == product.Id, product);
                return result.IsAcknowledged && result.ModifiedCount > 0;
            }
            catch
            {
                return false;
            }
        }
    
        public async Task<bool> DeleteProductAsync(string id)
        {
            try
            {
                var result = await _products.DeleteOneAsync(p => p.Id == id);
                return result.IsAcknowledged && result.DeletedCount > 0;
            }
            catch
            {
                return false;
            }
        }
    }
}