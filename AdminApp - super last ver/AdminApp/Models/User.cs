using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.ComponentModel.DataAnnotations;

namespace AdminApp.Models
{
    [BsonIgnoreExtraElements]  // Игнорируем поля (например, subscription), которых нет в модели
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("username")]
        [Required]
        public required string Username { get; set; }

        [BsonElement("email")]
        [Required]
        [EmailAddress]
        public required string Email { get; set; }

        [BsonElement("password")]
        [Required]
        public required string Password { get; set; }

        [BsonElement("firstName")]
        [Required]
        public required string FirstName { get; set; }

        [BsonElement("middleName")]
        public string? MiddleName { get; set; }

        [BsonElement("lastName")]
        [Required]
        public required string LastName { get; set; }

        [BsonElement("gender")]
        [Required]
        public required string Gender { get; set; }

        [BsonElement("phone")]
        [Required]
        [Phone]
        public required string Phone { get; set; }

        [BsonElement("address")]
        public string? Address { get; set; }

        [BsonElement("role")]
        [Required]
        public required string Role { get; set; }

        [BsonElement("dateOfBirth")]
        public DateTime DateOfBirth { get; set; }

        // Новое поле для хранения баланса пользователя
        [BsonElement("balance")]
        public double Balance { get; set; } = 0;

        // В админ-приложении, например, храним только план и дату завершения,
        // без детального объекта "subscription"
        [BsonElement("subscriptionPlan")]
        public string? SubscriptionPlan { get; set; }

        [BsonElement("subscriptionEndDate")]
        public DateTime? SubscriptionEndDate { get; set; }

        [BsonElement("createdAt")]
        [Required]
        public DateTime CreatedAt { get; set; }

        [BsonElement("updatedAt")]
        [Required]
        public DateTime UpdatedAt { get; set; }
        
        [BsonElement("isInGym")]
        public bool IsInGym { get; set; } = false;

        [BsonElement("visitCount")]
        public int VisitCount { get; set; } = 0;
    }
}
