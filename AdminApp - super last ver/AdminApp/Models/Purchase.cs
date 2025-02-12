using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace AdminApp.Models
{
    [BsonIgnoreExtraElements]
    public class Purchase
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
    
        [BsonElement("userId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string UserId { get; set; }
    
        [BsonElement("items")]
        public List<PurchaseItem> Items { get; set; } = new List<PurchaseItem>();
    
        [BsonElement("totalAmount")]
        public double TotalAmount { get; set; }
    
        [BsonElement("purchaseDate")]
        public DateTime PurchaseDate { get; set; }
    
        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; }
    
        [BsonElement("updatedAt")]
        public DateTime UpdatedAt { get; set; }
    }
    
    public class PurchaseItem
    {
        [BsonElement("productId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string ProductId { get; set; }
    
        [BsonElement("productName")]
        public string ProductName { get; set; }
    
        [BsonElement("quantity")]
        public int Quantity { get; set; }
    
        [BsonElement("price")]
        public double Price { get; set; }
    
        [BsonElement("subtotal")]
        public double Subtotal => Quantity * Price;
    }
}