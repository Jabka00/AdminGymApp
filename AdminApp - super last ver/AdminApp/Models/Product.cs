using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace AdminApp.Models
{
    [BsonIgnoreExtraElements]
    public class Product
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
    
        [BsonElement("name")]
        public string Name { get; set; }
    
        [BsonElement("description")]
        public string? Description { get; set; }
    
        [BsonElement("price")]
        public double Price { get; set; }
    
        [BsonElement("quantity")]
        public int Quantity { get; set; }
        
        [BsonElement("category")]
        public string Category { get; set; }
    
        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; }
    
        [BsonElement("updatedAt")]
        public DateTime UpdatedAt { get; set; }
    }
}