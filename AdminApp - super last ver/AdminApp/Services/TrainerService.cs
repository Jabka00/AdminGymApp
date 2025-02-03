using AdminApp.DB;
using AdminApp.Models;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AdminApp.Services
{
    public class TrainerService
    {
        private readonly IMongoCollection<Trainer> _trainers;

        public TrainerService(DatabaseManager dbManager)
        {
            _trainers = dbManager.GetCollection<Trainer>("trainers");
        }

        public async Task<List<Trainer>> GetAllTrainersAsync()
        {
            return await _trainers.Find(_ => true).ToListAsync();
        }

        public async Task<Trainer?> GetTrainerByIdAsync(string id)
        {
            return await _trainers.Find(t => t.Id == id).FirstOrDefaultAsync();
        }

        public async Task<bool> InsertTrainerAsync(Trainer trainer)
        {
            try
            {
                trainer.CreatedAt = DateTime.UtcNow;
                trainer.UpdatedAt = DateTime.UtcNow;
                await _trainers.InsertOneAsync(trainer);
                return true;
            }
            catch
            {
                // Добавьте дополнительную обработку ошибок при необходимости
                return false;
            }
        }

        public async Task<bool> UpdateTrainerAsync(Trainer trainer)
        {
            try
            {
                trainer.UpdatedAt = DateTime.UtcNow;
                var result = await _trainers.ReplaceOneAsync(t => t.Id == trainer.Id, trainer);
                return result.IsAcknowledged && result.ModifiedCount > 0;
            }
            catch
            {
                // Добавьте дополнительную обработку ошибок при необходимости
                return false;
            }
        }

        public async Task<bool> DeleteTrainerAsync(string id)
        {
            try
            {
                var result = await _trainers.DeleteOneAsync(t => t.Id == id);
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
