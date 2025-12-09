using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Order.Models
{
    public class Order
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [Required]
        public string CustomerId { get; set; } = string.Empty;

        [Required]
        public string RestaurantId { get; set; } = string.Empty;

        public string? RestaurantName { get; set; }

        [Required]
        public List<OrderItem> Items { get; set; } = new List<OrderItem>();

        [Required]
        public decimal TotalAmount { get; set; }

        public OrderStatus Status { get; set; } = OrderStatus.Pending;

        public PackagingDetails Packaging { get; set; } = new PackagingDetails();

        public string? DeliveryAgentId { get; set; }

        public string? HandoverOTP { get; set; } // OTP for delivery agent handover

        public DateTime? HandoverOTPGeneratedAt { get; set; }

        public bool IsHandedOver { get; set; } = false;

        public DateTime? HandoverTime { get; set; }

        public string? HandoverBy { get; set; } // Operator who handed over

        public DateTime? PickedUpAt { get; set; } // When delivery agent picked up

        public DateTime? DeliveredAt { get; set; } // When delivered to customer

        public string? Notes { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }

    public class OrderItem
    {
        [Required]
        public string MenuItemId { get; set; } = string.Empty;

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        [Required]
        public decimal Price { get; set; }

        public string? SpecialInstructions { get; set; }
    }

    public class PackagingDetails
    {
        public bool IncludeCutlery { get; set; } = true;

        public bool IncludeNapkins { get; set; } = true;

        public int NumberOfBags { get; set; } = 1;

        public string? SpecialPackagingNotes { get; set; }

        public bool IsFragile { get; set; } = false;

        public DateTime? PackagedAt { get; set; }

        public string? PackagedBy { get; set; } // Operator who packaged
    }

    public enum OrderStatus
    {
        Pending = 0,        // Order placed, awaiting acceptance
        Accepted = 1,       // Accepted by operator
        Declined = 2,       // Declined by operator
        Preparing = 3,      // Being prepared by workers
        ReadyForPickup = 4, // Ready for delivery agent pickup
        OutForDelivery = 5, // Handed over to delivery agent
        Delivered = 6,      // Delivered to customer
        Cancelled = 7       // Cancelled
    }

    public class HandoverRequest
    {
        [Required]
        public string OrderId { get; set; } = string.Empty;

        [Required]
        public string DeliveryAgentId { get; set; } = string.Empty;

        [Required]
        [StringLength(6, MinimumLength = 6)]
        public string OTP { get; set; } = string.Empty;

        [Required]
        public string OperatorId { get; set; } = string.Empty;
    }

    public class InventoryUpdateRequest
    {
        [Required]
        public string MenuItemId { get; set; } = string.Empty;

        [Range(0, int.MaxValue)]
        public int? QuantityAvailable { get; set; }

        public bool? IsAvailable { get; set; }

        public string? AvailableFromTime { get; set; }

        public string? AvailableToTime { get; set; }
    }
}
