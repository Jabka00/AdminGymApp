using AdminApp.DB;
using AdminApp.Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AdminApp.Services
{
    public class TrainingService
    {
        private readonly IMongoCollection<Training> _trainings;
        private readonly DatabaseManager _dbManager;
        private readonly TrainerService _trainerService;

        public TrainingService(DatabaseManager dbManager, TrainerService trainerService)
        {
            _dbManager = dbManager;
            _trainings = _dbManager.GetCollection<Training>("trainings");
            _trainerService = trainerService;
        }

        public async Task<List<Training>> GetAllTrainingsAsync()
        {
            return await _trainings.Find(_ => true).ToListAsync();
        }

        public async Task<Training?> GetTrainingByIdAsync(string id)
        {
            return await _trainings.Find(t => t.Id == id).FirstOrDefaultAsync();
        }

        public async Task<Training?> GetTrainingByTitleAsync(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Название тренировки не должно быть пустым.", nameof(title));

            var filter = Builders<Training>.Filter.Eq(t => t.Title, title);
            return await _trainings.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<bool> InsertTrainingAsync(Training training)
        {
            try
            {
                training.CreatedAt = DateTime.UtcNow;
                training.UpdatedAt = DateTime.UtcNow;
                training.EnrolledUsers = new List<string>();
                await _trainings.InsertOneAsync(training);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdateTrainingAsync(Training training)
        {
            try
            {
                training.UpdatedAt = DateTime.UtcNow;
                var result = await _trainings.ReplaceOneAsync(t => t.Id == training.Id, training);
                return result.IsAcknowledged && result.ModifiedCount > 0;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteTrainingAsync(string id)
        {
            try
            {
                var result = await _trainings.DeleteOneAsync(t => t.Id == id);
                return result.IsAcknowledged && result.DeletedCount > 0;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> EnrollUserAsync(string trainingId, string userId)
        {
            try
            {
                var training = await GetTrainingByIdAsync(trainingId);
                if (training == null || training.EnrolledUsers == null)
                    return false;

                // Проверка вместимости
                if (training.EnrolledUsers.Count >= training.Capacity)
                    return false;

                // Получение тренера
                var trainer = await _trainerService.GetTrainerByIdAsync(training.TrainerId);
                if (trainer == null)
                    return false;

                // Проверка рабочего дня тренера
                var trainingDay = training.Schedule.DayOfWeek;
                if (!trainer.WorkingDays.Contains(trainingDay))
                    return false;

                // Запись пользователя
                var update = Builders<Training>.Update
                    .AddToSet(t => t.EnrolledUsers, userId)
                    .Set(t => t.UpdatedAt, DateTime.UtcNow);

                var result = await _trainings.UpdateOneAsync(t => t.Id == trainingId, update);
                return result.IsAcknowledged && result.ModifiedCount > 0;
            }
            catch
            {
                return false;
            }
        }
    }
}
