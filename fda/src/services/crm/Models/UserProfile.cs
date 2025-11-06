using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Crm.Models
{
    /// <summary>
    /// Comprehensive user profile model containing all non-authentication user data.
    /// This includes personal information, addresses, preferences, and role-specific data.
    /// Links to UserAccount in Authentication service via UserId.
    /// </summary>
    public class UserProfile
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        /// <summary>
        /// Reference to the UserAccount ID in the Authentication service
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

        [Required]
        public UserRole Role { get; set; }

        [MaxLength(200)]
        public string? Organization { get; set; }

        public List<string> Permissions { get; set; } = new List<string>();

        public DateTime? DateOfBirth { get; set; }

        public Address? Address { get; set; }

        // Customer-specific fields
        public List<string>? DietaryPreferences { get; set; }
        public List<string>? FavoriteRestaurants { get; set; }

        // Employee-specific fields
        public EmployeeInfo? EmployeeInfo { get; set; }

        // Business-specific fields (for Biller role)
        public BusinessInfo? BusinessInfo { get; set; }

        // Delivery agent-specific fields
        public DeliveryInfo? DeliveryInfo { get; set; }

        // IT technician-specific fields
        public TechInfo? TechInfo { get; set; }

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

    public class Address
    {
        [Required]
        [MaxLength(200)]
        public string Street { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? Apartment { get; set; }

        [Required]
        [MaxLength(100)]
        public string City { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string State { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string ZipCode { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? Country { get; set; } = "USA";
    }

    public class EmployeeInfo
    {
        [Required]
        [MaxLength(50)]
        public string EmployeeId { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Position { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Department { get; set; } = string.Empty;

        public DateTime HireDate { get; set; }

        [MaxLength(200)]
        public string? ShiftSchedule { get; set; }

        public decimal? HourlyWage { get; set; }

        public List<string>? Certifications { get; set; }

        [MaxLength(50)]
        public string? ManagerId { get; set; }

        public DateTime? LastPerformanceReview { get; set; }
    }

    public class BusinessInfo
    {
        [Required]
        [MaxLength(200)]
        public string RestaurantName { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string BusinessLicense { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string TaxId { get; set; } = string.Empty;

        [Required]
        public Address Address { get; set; } = new Address();
        
        // Payment information for Biller (who receives payments)
        [MaxLength(100)]
        public string? UpiId { get; set; }

        [MaxLength(50)]
        public string? BankAccountNumber { get; set; }

        [MaxLength(20)]
        public string? BankIfscCode { get; set; }

        [MaxLength(200)]
        public string? BankAccountHolderName { get; set; }

        public bool IsUpiVerified { get; set; } = false;
        public DateTime? UpiVerificationDate { get; set; }

        [MaxLength(20)]
        public string? BusinessPhone { get; set; }

        [MaxLength(100)]
        public string? BusinessEmail { get; set; }

        public List<string>? BusinessCategories { get; set; }
    }

    public class DeliveryInfo
    {
        [Required]
        [MaxLength(50)]
        public string EmployeeId { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string VehicleType { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string LicensePlate { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string DriversLicense { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? InsurancePolicy { get; set; }

        public List<string> ServiceAreas { get; set; } = new List<string>();

        [MaxLength(100)]
        public string WorkingHours { get; set; } = string.Empty;

        public double AverageRating { get; set; } = 0.0;

        public int TotalDeliveries { get; set; } = 0;

        public DateTime? LastDeliveryDate { get; set; }

        public bool IsAvailable { get; set; } = true;
    }

    public class TechInfo
    {
        [Required]
        [MaxLength(50)]
        public string EmployeeId { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Specialization { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Department { get; set; } = string.Empty;

        public int SecurityClearance { get; set; } = 1;

        public List<string> AccessPermissions { get; set; } = new List<string>();

        public List<string>? TechnicalSkills { get; set; }

        public List<string>? CurrentProjects { get; set; }

        public DateTime? LastSecurityTraining { get; set; }
    }
}