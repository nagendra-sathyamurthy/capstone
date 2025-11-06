using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Authentication.Models
{
    /// <summary>
    /// Simplified UserAccount model containing only authentication-related data.
    /// User profile information has been moved to the CRM service.
    /// </summary>
    public class UserAccount
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MinLength(6)]
        public string Password { get; set; } = string.Empty;

        [Required]
        public UserRole Role { get; set; }

        public string Organization { get; set; } = string.Empty;
        public List<string> Permissions { get; set; } = new List<string>();

        public bool IsActive { get; set; } = true;
        public bool IsEmailVerified { get; set; } = false;
        public int InvalidLogins { get; set; }
        public DateTime? LastLoginTime { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }

    public enum UserRole
    {
        Customer,
        Biller,
        Operator,
        Worker,
        DeliveryAgent,
        Developer,
        Tester,
        NetworkAdmin,
        DatabaseAdmin
    }
}