using catalog.Models;

namespace catalog.Services
{
    public static class SampleMenuData
    {
        public static List<MenuItem> GetSampleMenuItems()
        {
            return new List<MenuItem>
            {
                // Appetizers
                new MenuItem
                {
                    Name = "Margherita Pizza Slice",
                    Description = "Classic pizza with fresh mozzarella, tomato sauce, and basil leaves",
                    PreparationTimeMinutes = 15,
                    PackagingSize = "Single Slice",
                    UnitOfMeasure = "slice",
                    PricePerUOM = 8.99m,
                    Category = "Appetizer",
                    Cuisine = "Italian",
                    Ingredients = new List<string> { "Pizza dough", "Mozzarella cheese", "Tomato sauce", "Fresh basil", "Olive oil" },
                    Allergens = new List<string> { "Gluten", "Dairy" },
                    IsVegetarian = true,
                    IsVegan = false,
                    IsGlutenFree = false,
                    SpiceLevel = 1,
                    IsAvailable = true,
                    Calories = 320,
                    Protein = 12,
                    Carbohydrates = 38,
                    Fat = 14
                },
                new MenuItem
                {
                    Name = "Chicken Wings",
                    Description = "Crispy buffalo chicken wings served with ranch dipping sauce",
                    PreparationTimeMinutes = 20,
                    PackagingSize = "6 pieces",
                    UnitOfMeasure = "serving",
                    PricePerUOM = 12.99m,
                    Category = "Appetizer",
                    Cuisine = "American",
                    Ingredients = new List<string> { "Chicken wings", "Buffalo sauce", "Celery", "Ranch dressing" },
                    Allergens = new List<string> { "Dairy" },
                    IsVegetarian = false,
                    IsVegan = false,
                    IsGlutenFree = true,
                    SpiceLevel = 3,
                    IsAvailable = true,
                    Calories = 450,
                    Protein = 28,
                    Carbohydrates = 2,
                    Fat = 36
                },

                // Main Courses
                new MenuItem
                {
                    Name = "Chicken Biryani",
                    Description = "Aromatic basmati rice cooked with tender chicken pieces and traditional Indian spices",
                    PreparationTimeMinutes = 45,
                    PackagingSize = "Large",
                    UnitOfMeasure = "serving",
                    PricePerUOM = 18.99m,
                    Category = "Main Course",
                    Cuisine = "Indian",
                    Ingredients = new List<string> { "Basmati rice", "Chicken", "Onions", "Yogurt", "Biryani spices", "Saffron" },
                    Allergens = new List<string> { "Dairy" },
                    IsVegetarian = false,
                    IsVegan = false,
                    IsGlutenFree = true,
                    SpiceLevel = 4,
                    IsAvailable = true,
                    Calories = 650,
                    Protein = 35,
                    Carbohydrates = 78,
                    Fat = 18
                },
                new MenuItem
                {
                    Name = "Vegetable Pad Thai",
                    Description = "Stir-fried rice noodles with tofu, vegetables, and tamarind-based sauce",
                    PreparationTimeMinutes = 25,
                    PackagingSize = "Regular",
                    UnitOfMeasure = "serving",
                    PricePerUOM = 14.99m,
                    Category = "Main Course",
                    Cuisine = "Thai",
                    Ingredients = new List<string> { "Rice noodles", "Tofu", "Bean sprouts", "Carrots", "Tamarind paste", "Peanuts" },
                    Allergens = new List<string> { "Nuts", "Soy" },
                    IsVegetarian = true,
                    IsVegan = true,
                    IsGlutenFree = true,
                    SpiceLevel = 2,
                    IsAvailable = true,
                    Calories = 480,
                    Protein = 18,
                    Carbohydrates = 68,
                    Fat = 16
                },
                new MenuItem
                {
                    Name = "Classic Beef Burger",
                    Description = "Juicy beef patty with lettuce, tomato, onion, and house sauce on a brioche bun",
                    PreparationTimeMinutes = 18,
                    PackagingSize = "Single",
                    UnitOfMeasure = "piece",
                    PricePerUOM = 16.99m,
                    Category = "Main Course",
                    Cuisine = "American",
                    Ingredients = new List<string> { "Beef patty", "Brioche bun", "Lettuce", "Tomato", "Onion", "House sauce" },
                    Allergens = new List<string> { "Gluten", "Dairy" },
                    IsVegetarian = false,
                    IsVegan = false,
                    IsGlutenFree = false,
                    SpiceLevel = 1,
                    IsAvailable = true,
                    Calories = 580,
                    Protein = 32,
                    Carbohydrates = 45,
                    Fat = 28
                },

                // Desserts
                new MenuItem
                {
                    Name = "Chocolate Lava Cake",
                    Description = "Warm chocolate cake with molten chocolate center, served with vanilla ice cream",
                    PreparationTimeMinutes = 12,
                    PackagingSize = "Individual",
                    UnitOfMeasure = "piece",
                    PricePerUOM = 9.99m,
                    Category = "Dessert",
                    Cuisine = "French",
                    Ingredients = new List<string> { "Dark chocolate", "Butter", "Eggs", "Flour", "Sugar", "Vanilla ice cream" },
                    Allergens = new List<string> { "Gluten", "Dairy", "Eggs" },
                    IsVegetarian = true,
                    IsVegan = false,
                    IsGlutenFree = false,
                    SpiceLevel = 1,
                    IsAvailable = true,
                    Calories = 420,
                    Protein = 8,
                    Carbohydrates = 52,
                    Fat = 22
                },

                // Beverages
                new MenuItem
                {
                    Name = "Fresh Mango Lassi",
                    Description = "Traditional Indian yogurt-based drink blended with fresh mango and cardamom",
                    PreparationTimeMinutes = 5,
                    PackagingSize = "16 oz",
                    UnitOfMeasure = "glass",
                    PricePerUOM = 5.99m,
                    Category = "Beverage",
                    Cuisine = "Indian",
                    Ingredients = new List<string> { "Fresh mango", "Yogurt", "Milk", "Sugar", "Cardamom" },
                    Allergens = new List<string> { "Dairy" },
                    IsVegetarian = true,
                    IsVegan = false,
                    IsGlutenFree = true,
                    SpiceLevel = 1,
                    IsAvailable = true,
                    Calories = 180,
                    Protein = 6,
                    Carbohydrates = 32,
                    Fat = 4
                },
                new MenuItem
                {
                    Name = "Green Smoothie",
                    Description = "Healthy blend of spinach, banana, apple, and coconut water",
                    PreparationTimeMinutes = 3,
                    PackagingSize = "12 oz",
                    UnitOfMeasure = "glass",
                    PricePerUOM = 7.99m,
                    Category = "Beverage",
                    Cuisine = "Health Food",
                    Ingredients = new List<string> { "Fresh spinach", "Banana", "Apple", "Coconut water", "Honey" },
                    Allergens = new List<string>(),
                    IsVegetarian = true,
                    IsVegan = true,
                    IsGlutenFree = true,
                    SpiceLevel = 1,
                    IsAvailable = true,
                    Calories = 140,
                    Protein = 3,
                    Carbohydrates = 35,
                    Fat = 1
                },

                // Quick Options
                new MenuItem
                {
                    Name = "Caesar Salad",
                    Description = "Crisp romaine lettuce with Caesar dressing, croutons, and parmesan cheese",
                    PreparationTimeMinutes = 8,
                    PackagingSize = "Regular",
                    UnitOfMeasure = "bowl",
                    PricePerUOM = 11.99m,
                    Category = "Salad",
                    Cuisine = "Mediterranean",
                    Ingredients = new List<string> { "Romaine lettuce", "Caesar dressing", "Croutons", "Parmesan cheese" },
                    Allergens = new List<string> { "Gluten", "Dairy", "Eggs" },
                    IsVegetarian = true,
                    IsVegan = false,
                    IsGlutenFree = false,
                    SpiceLevel = 1,
                    IsAvailable = true,
                    Calories = 250,
                    Protein = 8,
                    Carbohydrates = 15,
                    Fat = 18
                },
                new MenuItem
                {
                    Name = "Quinoa Buddha Bowl",
                    Description = "Nutritious bowl with quinoa, roasted vegetables, avocado, and tahini dressing",
                    PreparationTimeMinutes = 15,
                    PackagingSize = "Large",
                    UnitOfMeasure = "bowl",
                    PricePerUOM = 13.99m,
                    Category = "Salad",
                    Cuisine = "Health Food",
                    Ingredients = new List<string> { "Quinoa", "Roasted vegetables", "Avocado", "Chickpeas", "Tahini dressing" },
                    Allergens = new List<string> { "Sesame" },
                    IsVegetarian = true,
                    IsVegan = true,
                    IsGlutenFree = true,
                    SpiceLevel = 1,
                    IsAvailable = true,
                    Calories = 380,
                    Protein = 14,
                    Carbohydrates = 48,
                    Fat = 16
                }
            };
        }
    }
}