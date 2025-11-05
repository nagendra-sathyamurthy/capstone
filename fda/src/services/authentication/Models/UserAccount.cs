using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Authentication.Models
{
    public class UserAccount
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MinLength(6)]
        public string Password { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        [Phone]
        public string PhoneNumber { get; set; }

        [Required]
        public UserRole Role { get; set; }

        [Required]
        public string Organization { get; set; }

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

    public class Address
    {
        public string Street { get; set; }
        public string? Apartment { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
    }

    public class EmployeeInfo
    {
        public string EmployeeId { get; set; }
        public string Position { get; set; }
        public string Department { get; set; }
        public DateTime HireDate { get; set; }
        public string? ShiftSchedule { get; set; }
        public decimal? HourlyWage { get; set; }
        public List<string>? Certifications { get; set; }
    }

    public class BusinessInfo
    {
        public string RestaurantName { get; set; }
        public string BusinessLicense { get; set; }
        public string TaxId { get; set; }
        public Address Address { get; set; }
        
        // Payment information for Biller (who receives payments)
        public string UpiId { get; set; }
        public string BankAccountNumber { get; set; }
        public string BankIfscCode { get; set; }
        public string BankAccountHolderName { get; set; }
        public bool IsUpiVerified { get; set; } = false;
        public DateTime? UpiVerificationDate { get; set; }
    }

    public class DeliveryInfo
    {
        public string EmployeeId { get; set; }
        public string VehicleType { get; set; }
        public string LicensePlate { get; set; }
        public string DriversLicense { get; set; }
        public string InsurancePolicy { get; set; }
        public List<string> ServiceAreas { get; set; } = new List<string>();
        public string WorkingHours { get; set; }
        public double AverageRating { get; set; } = 0.0;
    }

    public class TechInfo
    {
        public string EmployeeId { get; set; }
        public string Specialization { get; set; }
        public string Department { get; set; }
        public int SecurityClearance { get; set; }
        public List<string> AccessPermissions { get; set; } = new List<string>();
    }
}