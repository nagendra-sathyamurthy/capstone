using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace catalog.Models
{
    public class Restaurant
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [Required]
        public string OwnerId { get; set; } = string.Empty; // User account ID of restaurant owner

        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Description { get; set; }

        [Required]
        public Address Address { get; set; } = new Address();

        [Required]
        public ContactInformation ContactInfo { get; set; } = new ContactInformation();

        [Required]
        public BusinessHours Timings { get; set; } = new BusinessHours();

        public bool IsActive { get; set; } = true;

        [StringLength(500)]
        public string? ImageUrl { get; set; }

        [StringLength(100)]
        public string? CuisineType { get; set; }

        [Range(0, 5)]
        public decimal Rating { get; set; } = 0;

        public int TotalReviews { get; set; } = 0;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }

    public class Address
    {
        [Required]
        [StringLength(500)]
        public string Street { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string City { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string State { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string ZipCode { get; set; } = string.Empty;

        [StringLength(100)]
        public string Country { get; set; } = "USA";

        public string? Landmark { get; set; }
    }

    public class ContactInformation
    {
        [Required]
        [Phone]
        public string Phone { get; set; } = string.Empty;

        [EmailAddress]
        public string? Email { get; set; }

        [StringLength(200)]
        public string? Website { get; set; }
    }

    public class BusinessHours
    {
        public DayTimings Monday { get; set; } = new DayTimings();
        public DayTimings Tuesday { get; set; } = new DayTimings();
        public DayTimings Wednesday { get; set; } = new DayTimings();
        public DayTimings Thursday { get; set; } = new DayTimings();
        public DayTimings Friday { get; set; } = new DayTimings();
        public DayTimings Saturday { get; set; } = new DayTimings();
        public DayTimings Sunday { get; set; } = new DayTimings();
    }

    public class DayTimings
    {
        public bool IsOpen { get; set; } = true;
        public string OpenTime { get; set; } = "09:00"; // Format: "HH:mm"
        public string CloseTime { get; set; } = "22:00"; // Format: "HH:mm"
    }
}
