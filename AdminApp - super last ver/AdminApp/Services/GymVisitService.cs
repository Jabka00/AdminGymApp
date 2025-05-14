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

        public async Task<GymVisit?> GetActiveVisitByUserIdAsync(string userId)
        {
            var filter = Builders<GymVisit>.Filter.Eq(gv => gv.UserId, userId) &
                         Builders<GymVisit>.Filter.Eq(gv => gv.CheckOutTime, null);
            return await _gymVisits.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<(bool success, string message)> CheckInUserAsync(string userId)
        {
            var activeVisit = await GetActiveVisitByUserIdAsync(userId);
            if (activeVisit != null)
            {
                return (false, "User is already checked in.");
            }

            var user = await _users.Find(u => u.Id == userId).FirstOrDefaultAsync();
            if (user == null)
            {
                return (false, "User not found.");
            }

            string gender = user.Gender.ToLower();
            if (gender != "male" && gender != "female")
                gender = "male";

            var activeVisitsForGender = await _gymVisits
                .Find(gv => gv.Gender.ToLower() == gender && gv.CheckOutTime == null)
                .ToListAsync();

            var occupiedLockers = activeVisitsForGender
                .Select(gv => gv.LockerNumber)
                .ToHashSet();

            int? lockerNumber = null;
            // Try odd-numbered lockers first
            for (int num = 1; num <= 35; num += 2)
            {
                if (!occupiedLockers.Contains(num))
                {
                    lockerNumber = num;
                    break;
                }
            }
            // Then try even-numbered lockers
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
                return (false, "No free lockers available.");
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

                return (true, $"User checked in. Locker number: {lockerNumber.Value}");
            }
            catch (Exception ex)
            {
                return (false, $"Error during check-in: {ex.Message}");
            }
        }

        public async Task<(bool success, string message)> CheckOutUserAsync(string userId)
        {
            var activeVisit = await GetActiveVisitByUserIdAsync(userId);
            if (activeVisit == null)
            {
                return (false, "User is not currently checked in.");
            }

            var updateVisit = Builders<GymVisit>.Update
                .Set(gv => gv.CheckOutTime, DateTime.UtcNow)
                .Set(gv => gv.UpdatedAt, DateTime.UtcNow);
            var resultVisit = await _gymVisits.UpdateOneAsync(gv => gv.Id == activeVisit.Id, updateVisit);

            var user = await _users.Find(u => u.Id == userId).FirstOrDefaultAsync();
            if (user == null)
            {
                return (false, "User not found.");
            }

            int newVisitCount = user.VisitCount + 1;
            var updateUser = Builders<User>.Update
                .Set(u => u.IsInGym, false)
                .Set(u => u.VisitCount, newVisitCount)
                .Set(u => u.UpdatedAt, DateTime.UtcNow);
            var resultUser = await _users.UpdateOneAsync(u => u.Id == userId, updateUser);

            if (resultVisit.IsAcknowledged && resultUser.IsAcknowledged)
            {
                return (true, "User checked out successfully.");
            }
            else
            {
                return (false, "Error during check-out.");
            }
        }

        public async Task<List<GymVisit>> GetAllGymVisitsAsync()
        {
            return await _gymVisits.Find(_ => true).ToListAsync();
        }

        public async Task<List<GymVisit>> GetVisitHistoryAsync(string userId)
        {
            var filter = Builders<GymVisit>.Filter.Eq(gv => gv.UserId, userId);
            return await _gymVisits
                         .Find(filter)
                         .SortByDescending(gv => gv.CheckInTime)
                         .ToListAsync();
        }
    }
}