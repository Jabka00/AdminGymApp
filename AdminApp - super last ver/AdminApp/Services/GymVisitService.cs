using AdminApp.DB;
using AdminApp.Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdminApp.Services
{
    public class GymVisitService
    {
        private readonly IMongoCollection<GymVisit> _gymVisits;
        private readonly IMongoCollection<User> _users;

        public GymVisitService(DatabaseManager dbManager)
        {
            _gymVisits = dbManager.GetCollection<GymVisit>("gymVisits");
            _users = dbManager.GetCollection<User>("users");
        }

        // Получить активную (без check-out) запись для пользователя
        public async Task<GymVisit?> GetActiveVisitByUserIdAsync(string userId)
        {
            var filter = Builders<GymVisit>.Filter.Eq(gv => gv.UserId, userId) &
                         Builders<GymVisit>.Filter.Eq(gv => gv.CheckOutTime, null);
            return await _gymVisits.Find(filter).FirstOrDefaultAsync();
        }

        // Функция регистрации входа (CheckIn)
        public async Task<(bool success, string message)> CheckInUserAsync(string userId)
        {
            var activeVisit = await GetActiveVisitByUserIdAsync(userId);
            if (activeVisit != null)
            {
                return (false, "Пользователь уже находится в зале.");
            }

            var user = await _users.Find(u => u.Id == userId).FirstOrDefaultAsync();
            if (user == null)
            {
                return (false, "Пользователь не найден.");
            }

            string gender = user.Gender.ToLower();
            if (gender != "male" && gender != "female")
                gender = "male";

            var activeVisitsForGender = await _gymVisits.Find(gv => gv.Gender.ToLower() == gender && gv.CheckOutTime == null).ToListAsync();
            var occupiedLockers = activeVisitsForGender.Select(gv => gv.LockerNumber).ToHashSet();

            int? lockerNumber = null;
            for (int num = 1; num <= 35; num += 2)
            {
                if (!occupiedLockers.Contains(num))
                {
                    lockerNumber = num;
                    break;
                }
            }
            if (lockerNumber == null)
            {
                for (int num = 2; num <= 35; num += 2)
                {
                    if (!occupiedLockers.Contains(num))
                    {
                        lockerNumber = num;
                        break;
                    }
                }
            }
            if (lockerNumber == null)
            {
                return (false, "Нет свободных шкафчиков.");
            }

            var gymVisit = new GymVisit
            {
                UserId = userId,
                Gender = gender,
                LockerNumber = lockerNumber.Value,
                CheckInTime = DateTime.UtcNow,
                CheckOutTime = null,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            try
            {
                await _gymVisits.InsertOneAsync(gymVisit);
                var update = Builders<User>.Update.Set(u => u.IsInGym, true);
                await _users.UpdateOneAsync(u => u.Id == userId, update);
                return (true, $"Пользователь зачекинен. Номер шкафчика: {lockerNumber.Value}");
            }
            catch (Exception ex)
            {
                return (false, $"Ошибка при входе: {ex.Message}");
            }
        }

        // Функция регистрации выхода (CheckOut)
        public async Task<(bool success, string message)> CheckOutUserAsync(string userId)
        {
            var activeVisit = await GetActiveVisitByUserIdAsync(userId);
            if (activeVisit == null)
            {
                return (false, "Пользователь не числится в зале.");
            }

            var updateVisit = Builders<GymVisit>.Update
                .Set(gv => gv.CheckOutTime, DateTime.UtcNow)
                .Set(gv => gv.UpdatedAt, DateTime.UtcNow);
            var resultVisit = await _gymVisits.UpdateOneAsync(gv => gv.Id == activeVisit.Id, updateVisit);

            var user = await _users.Find(u => u.Id == userId).FirstOrDefaultAsync();
            if (user == null)
            {
                return (false, "Пользователь не найден.");
            }
            var newVisitCount = user.VisitCount + 1;
            var updateUser = Builders<User>.Update
                .Set(u => u.IsInGym, false)
                .Set(u => u.VisitCount, newVisitCount)
                .Set(u => u.UpdatedAt, DateTime.UtcNow);
            var resultUser = await _users.UpdateOneAsync(u => u.Id == userId, updateUser);

            if (resultVisit.IsAcknowledged && resultUser.IsAcknowledged)
            {
                return (true, "Пользователь выписан из зала.");
            }
            else
            {
                return (false, "Ошибка при выписке пользователя.");
            }
        }
        public async Task<List<GymVisit>> GetAllGymVisitsAsync()
        {
            return await _gymVisits.Find(_ => true).ToListAsync();
        }

        // Получить историю посещений для пользователя
        public async Task<List<GymVisit>> GetVisitHistoryAsync(string userId)
        {
            var filter = Builders<GymVisit>.Filter.Eq(gv => gv.UserId, userId);
            return await _gymVisits.Find(filter)
                                     .SortByDescending(gv => gv.CheckInTime)
                                     .ToListAsync();
        }
    }
}
