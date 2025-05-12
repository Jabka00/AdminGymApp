using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace AdminApp.Models
{
    [BsonIgnoreExtraElements]
    public class GymVisit
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        
        [BsonElement("userId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string UserId { get; set; }
        
        [BsonElement("gender")]
        public string Gender { get; set; }
        
        [BsonElement("lockerNumber")]
        public int LockerNumber { get; set; }
        
        [BsonElement("checkInTime")]
        public DateTime CheckInTime { get; set; }
        
        [BsonElement("checkOutTime")]
        public DateTime? CheckOutTime { get; set; }
        
        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; }
        
        [BsonElement("updatedAt")]
        public DateTime UpdatedAt { get; set; }
    }
}