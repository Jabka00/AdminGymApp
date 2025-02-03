using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace AdminApp.Models
{
    [BsonIgnoreExtraElements]
    public class Subscription
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        
        [BsonElement("userId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string UserId { get; set; }
        
        [BsonElement("durationMonths")]
        public int DurationMonths { get; set; }
        
        [BsonElement("pricePerMonth")]
        public double PricePerMonth { get; set; }
        
        [BsonElement("totalPrice")]
        public double TotalPrice { get; set; }
        
        [BsonElement("startDate")]
        public DateTime StartDate { get; set; }
        
        [BsonElement("endDate")]
        public DateTime EndDate { get; set; }
        
        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; }
        
        [BsonElement("updatedAt")]
        public DateTime UpdatedAt { get; set; }
    }
}