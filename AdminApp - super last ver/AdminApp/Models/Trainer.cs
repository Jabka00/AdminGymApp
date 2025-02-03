using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AdminApp.Models
{
    [BsonIgnoreExtraElements] // Игнорируем лишние поля, если они есть
    public class Trainer
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("fullName")]
        [Required]
        public required string Name { get; set; }

        [BsonElement("workingDays")]
        [Required]
        public List<DayOfWeek> WorkingDays { get; set; } = new List<DayOfWeek>();

        /// <summary>
        /// Цена за одну индивидуальную тренировку
        /// </summary>
        [BsonElement("trainingPrice")]
        [Required]
        public double TrainingPrice { get; set; }

        /// <summary>
        /// Баланс тренера (например, сколько денег ему уже начислено).
        /// </summary>
        [BsonElement("balance")]
        [Required]
        public double Balance { get; set; } = 0.0;

        [BsonElement("createdAt")]
        [Required]
        public DateTime CreatedAt { get; set; }

        [BsonElement("updatedAt")]
        [Required]
        public DateTime UpdatedAt { get; set; }
    }
}