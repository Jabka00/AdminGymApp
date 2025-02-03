using AdminApp.DB;
using AdminApp.Models;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AdminApp.Services
{
    public class UserService
    {
        private readonly IMongoCollection<User> _users;

        public UserService(DatabaseManager dbManager)
        {
            _users = dbManager.GetCollection<User>("users");
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            return await _users.Find(_ => true).ToListAsync();
        }

        public async Task<User?> GetUserByIdAsync(string id)
        {
            return await _users.Find(u => u.Id == id).FirstOrDefaultAsync();
        }

        public async Task<bool> InsertUserAsync(User user)
        {
            try
            {
                user.CreatedAt = DateTime.UtcNow;
                user.UpdatedAt = DateTime.UtcNow;
                await _users.InsertOneAsync(user);
                return true;
            }
            catch
            {
                // Добавьте дополнительную обработку ошибок при необходимости
                return false;
            }
        }

        public async Task<bool> UpdateUserAsync(User user)
        {
            try
            {
                user.UpdatedAt = DateTime.UtcNow;
                var result = await _users.ReplaceOneAsync(u => u.Id == user.Id, user);
                return result.IsAcknowledged && result.ModifiedCount > 0;
            }
            catch
            {
                // Добавьте дополнительную обработку ошибок при необходимости
                return false;
            }
        }

        public async Task<bool> DeleteUserAsync(string id)
        {
            try
            {
                var result = await _users.DeleteOneAsync(u => u.Id == id);
                return result.IsAcknowledged && result.DeletedCount > 0;
            }
            catch
            {
                // Добавьте дополнительную обработку ошибок при необходимости
                return false;
            }
        }
    }
}
