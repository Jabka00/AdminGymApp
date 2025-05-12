using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AdminApp.Models
{
    [BsonIgnoreExtraElements]
    public class Training
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("title")]
        [Required]
        public required string Title { get; set; }

        [BsonElement("description")]
        public string? Description { get; set; }

        [BsonElement("type")]
        public string? Type { get; set; }

        [BsonElement("schedule")]
        public DateTime Schedule { get; set; }

        [BsonElement("duration")]
        public TimeSpan Duration { get; set; }

        [BsonElement("trainerId")]
        public string? TrainerId { get; set; }

        [BsonElement("capacity")]
        public int Capacity { get; set; }

        [BsonElement("enrolledUsers")]
        public List<string>? EnrolledUsers { get; set; }

        /// <summary>
        /// </summary>
        [BsonElement("groupPrice")]
        public double? GroupPrice { get; set; }

        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; }

        [BsonElement("updatedAt")]
        public DateTime UpdatedAt { get; set; }
    }
}