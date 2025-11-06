using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Crm.Models
{
    /// <summary>
    /// Customer model focused on CRM-specific data.
    /// For complete user profile information, use UserProfile model.
    /// This model is for customer relationship management specific to business interactions.
    /// </summary>
    public class Customer
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public string? Id { get; set; }

        /// <summary>
        /// Reference to the UserProfile ID for complete user information
        /// </summary>
        [Required]
        public string UserProfileId { get; set; } = string.Empty;

        /// <summary>
        /// Reference to the UserAccount ID in Authentication service
        /// </summary>
        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Phone]
        public string PhoneNumber { get; set; } = string.Empty;

        // CRM-specific fields
        public CustomerStatus Status { get; set; } = CustomerStatus.Active;
        public CustomerTier Tier { get; set; } = CustomerTier.Standard;
        public DateTime? LastPurchaseDate { get; set; }
        public decimal TotalSpent { get; set; } = 0;
        public int OrderCount { get; set; } = 0;
        public double AverageOrderValue { get; set; } = 0;
        public List<string> Tags { get; set; } = new List<string>();
        public string? PreferredContactMethod { get; set; }
        public bool MarketingOptIn { get; set; } = true;
        public string? AssignedSalesRep { get; set; }
        public string? Notes { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }

    public enum CustomerStatus
    {
        Active,
        Inactive,
        Suspended,
        Potential,
        VIP
    }

    public enum CustomerTier
    {
        Bronze,
        Silver,
        Gold,
        Platinum,
        Standard
    }
}