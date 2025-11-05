using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Authentication.Models
{
    public class OtpCode
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("email")]
        public string? Email { get; set; }

        [BsonElement("phoneNumber")]
        public string? PhoneNumber { get; set; }

        [BsonElement("otp")]
        public string Otp { get; set; }

        [BsonElement("purpose")]
        public OtpPurpose Purpose { get; set; }

        [BsonElement("isUsed")]
        public bool IsUsed { get; set; } = false;

        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [BsonElement("expiresAt")]
        public DateTime ExpiresAt { get; set; }

        [BsonElement("attempts")]
        public int Attempts { get; set; } = 0;

        [BsonElement("maxAttempts")]
        public int MaxAttempts { get; set; } = 3;

        public bool IsValid => !IsUsed && DateTime.UtcNow <= ExpiresAt && Attempts < MaxAttempts;
    }
}