using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace catalog.Models
{
    public class MenuItem
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(1000)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Range(1, 300)]
        public int PreparationTimeMinutes { get; set; }

        [Required]
        [StringLength(50)]
        public string PackagingSize { get; set; } = string.Empty; // e.g., "Small", "Medium", "Large", "Family Size"

        [Required]
        [StringLength(20)]
        public string UnitOfMeasure { get; set; } = string.Empty; // e.g., "piece", "serving", "kg", "liter", "portion"

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal PricePerUOM { get; set; }

        [StringLength(50)]
        public string Category { get; set; } = string.Empty; // e.g., "Appetizer", "Main Course", "Dessert", "Beverage"

        [StringLength(50)]
        public string Cuisine { get; set; } = string.Empty; // e.g., "Italian", "Indian", "Chinese", "American"

        public List<string> Ingredients { get; set; } = new List<string>();

        public List<string> Allergens { get; set; } = new List<string>(); // e.g., "Nuts", "Dairy", "Gluten"

        public bool IsVegetarian { get; set; } = false;

        public bool IsVegan { get; set; } = false;

        public bool IsGlutenFree { get; set; } = false;

        [Range(1, 5)]
        public int SpiceLevel { get; set; } = 1; // 1 = Mild, 5 = Very Spicy

        public bool IsAvailable { get; set; } = true;

        [StringLength(500)]
        public string? ImageUrl { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Nutritional information (optional)
        public int? Calories { get; set; }
        public decimal? Protein { get; set; } // in grams
        public decimal? Carbohydrates { get; set; } // in grams
        public decimal? Fat { get; set; } // in grams
    }

    // Keep the old Item class for backward compatibility during migration
    public class Item
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [Required]
        [StringLength(100)]
        public string? Name { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }
    }
}