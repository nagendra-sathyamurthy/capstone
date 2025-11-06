using System.Text.Json;
using System.Text;

namespace catalog.Testing
{
    public class MenuApiTestScript
    {
        private static readonly HttpClient httpClient = new HttpClient();
        private const string baseUrl = "http://localhost:5002";

        public static async Task RunTestsAsync()
        {
            Console.WriteLine("=== Food Delivery Menu API Testing ===\n");

            try
            {
                // Test 1: Health Check
                await TestHealthCheck();

                // Test 2: Get Available Categories
                await TestGetCategories();

                // Test 3: Get Available Cuisines  
                await TestGetCuisines();

                // Test 4: Get Menu by Category
                await TestGetMenuByCategory("Main Course");

                // Test 5: Get Vegetarian Items
                await TestGetVegetarianItems();

                // Test 6: Get Quick Preparation Items
                await TestGetQuickItems();

                // Test 7: Search Menu Items
                await TestSearchMenuItems("chicken");

                // Test 8: Filter by Price Range
                await TestFilterByPriceRange(10, 20);

                // Test 9: Get Menu Statistics
                await TestGetMenuStatistics();

                Console.WriteLine("\n=== All Tests Completed Successfully! ===");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nError during testing: {ex.Message}");
                Console.WriteLine("Make sure the Catalog service is running on http://localhost:5002");
            }
        }

        private static async Task TestHealthCheck()
        {
            Console.WriteLine("1. Testing Health Check...");
            var response = await httpClient.GetAsync($"{baseUrl}/api/health");
            var content = await response.Content.ReadAsStringAsync();
            
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("✅ Health Check: Service is healthy");
                var healthData = JsonSerializer.Deserialize<JsonElement>(content);
                Console.WriteLine($"   Service: {healthData.GetProperty("service").GetString()}");
                Console.WriteLine($"   Version: {healthData.GetProperty("version").GetString()}");
            }
            else
            {
                Console.WriteLine($"❌ Health Check Failed: {response.StatusCode}");
            }
            Console.WriteLine();
        }

        private static async Task TestGetCategories()
        {
            Console.WriteLine("2. Testing Get Categories...");
            var response = await httpClient.GetAsync($"{baseUrl}/api/menu/categories");
            
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var categories = JsonSerializer.Deserialize<string[]>(content);
                Console.WriteLine($"✅ Found {categories?.Length} categories:");
                if (categories != null)
                {
                    foreach (var category in categories)
                    {
                        Console.WriteLine($"   - {category}");
                    }
                }
            }  
            else
            {
                Console.WriteLine($"❌ Get Categories Failed: {response.StatusCode}");
            }
            Console.WriteLine();
        }

        private static async Task TestGetCuisines()
        {
            Console.WriteLine("3. Testing Get Cuisines...");
            var response = await httpClient.GetAsync($"{baseUrl}/api/menu/cuisines");
            
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var cuisines = JsonSerializer.Deserialize<string[]>(content);
                Console.WriteLine($"✅ Found {cuisines?.Length} cuisines:");
                if (cuisines != null)
                {
                    foreach (var cuisine in cuisines)
                    {
                        Console.WriteLine($"   - {cuisine}");
                    }
                }
            }
            else
            {
                Console.WriteLine($"❌ Get Cuisines Failed: {response.StatusCode}");
            }
            Console.WriteLine();
        }

        private static async Task TestGetMenuByCategory(string category)
        {
            Console.WriteLine($"4. Testing Get Menu by Category: {category}...");
            var response = await httpClient.GetAsync($"{baseUrl}/api/menu/category/{Uri.EscapeDataString(category)}");
            
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var menuItems = JsonSerializer.Deserialize<JsonElement[]>(content);
                Console.WriteLine($"✅ Found {menuItems?.Length} items in {category}:");
                if (menuItems != null)
                {
                    foreach (var item in menuItems.Take(3)) // Show first 3 items
                    {
                        Console.WriteLine($"   - {item.GetProperty("name").GetString()} (${item.GetProperty("pricePerUOM").GetDecimal()})");
                    }
                    if (menuItems.Length > 3)
                    {
                        Console.WriteLine($"   ... and {menuItems.Length - 3} more items");
                    }
                }
            }
            else
            {
                Console.WriteLine($"❌ Get Menu by Category Failed: {response.StatusCode}");
            }
            Console.WriteLine();
        }

        private static async Task TestGetVegetarianItems()
        {
            Console.WriteLine("5. Testing Get Vegetarian Items...");
            var response = await httpClient.GetAsync($"{baseUrl}/api/menu/vegetarian");
            
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var menuItems = JsonSerializer.Deserialize<JsonElement[]>(content);
                Console.WriteLine($"✅ Found {menuItems?.Length} vegetarian items:");
                if (menuItems != null)
                {
                    foreach (var item in menuItems.Take(3))
                    {
                        Console.WriteLine($"   - {item.GetProperty("name").GetString()} ({item.GetProperty("cuisine").GetString()})");
                    }
                }
            }
            else
            {
                Console.WriteLine($"❌ Get Vegetarian Items Failed: {response.StatusCode}");
            }
            Console.WriteLine();
        }

        private static async Task TestGetQuickItems()
        {
            Console.WriteLine("6. Testing Get Quick Preparation Items (≤30 min)...");
            var response = await httpClient.GetAsync($"{baseUrl}/api/menu/quick?maxMinutes=30");
            
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var menuItems = JsonSerializer.Deserialize<JsonElement[]>(content);
                Console.WriteLine($"✅ Found {menuItems?.Length} quick preparation items:");
                if (menuItems != null)
                {
                    foreach (var item in menuItems.Take(3))
                    {
                        Console.WriteLine($"   - {item.GetProperty("name").GetString()} ({item.GetProperty("preparationTimeMinutes").GetInt32()} min)");
                    }
                }
            }
            else
            {
                Console.WriteLine($"❌ Get Quick Items Failed: {response.StatusCode}");
            }
            Console.WriteLine();
        }

        private static async Task TestSearchMenuItems(string searchTerm)
        {
            Console.WriteLine($"7. Testing Search Menu Items: '{searchTerm}'...");
            var response = await httpClient.GetAsync($"{baseUrl}/api/menu/search?searchTerm={Uri.EscapeDataString(searchTerm)}");
            
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var menuItems = JsonSerializer.Deserialize<JsonElement[]>(content);
                Console.WriteLine($"✅ Found {menuItems?.Length} items matching '{searchTerm}':");
                if (menuItems != null)
                {
                    foreach (var item in menuItems)
                    {
                        Console.WriteLine($"   - {item.GetProperty("name").GetString()} (${item.GetProperty("pricePerUOM").GetDecimal()})");
                    }
                }
            }
            else
            {
                Console.WriteLine($"❌ Search Menu Items Failed: {response.StatusCode}");
            }
            Console.WriteLine();
        }

        private static async Task TestFilterByPriceRange(decimal minPrice, decimal maxPrice)
        {
            Console.WriteLine($"8. Testing Filter by Price Range: ${minPrice} - ${maxPrice}...");
            var response = await httpClient.GetAsync($"{baseUrl}/api/menu/price-range?minPrice={minPrice}&maxPrice={maxPrice}");
            
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var menuItems = JsonSerializer.Deserialize<JsonElement[]>(content);
                Console.WriteLine($"✅ Found {menuItems?.Length} items in price range ${minPrice} - ${maxPrice}:");
                if (menuItems != null)
                {
                    foreach (var item in menuItems.Take(3))
                    {
                        Console.WriteLine($"   - {item.GetProperty("name").GetString()} (${item.GetProperty("pricePerUOM").GetDecimal()})");
                    }
                }
            }
            else
            {
                Console.WriteLine($"❌ Filter by Price Range Failed: {response.StatusCode}");
            }
            Console.WriteLine();
        }

        private static async Task TestGetMenuStatistics()
        {
            Console.WriteLine("9. Testing Get Menu Statistics...");
            var response = await httpClient.GetAsync($"{baseUrl}/api/seed/status");
            
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var stats = JsonSerializer.Deserialize<JsonElement>(content);
                Console.WriteLine("✅ Menu Statistics:");
                Console.WriteLine($"   Total Menu Items: {stats.GetProperty("totalMenuItems").GetInt32()}");
                Console.WriteLine($"   Available Items: {stats.GetProperty("availableMenuItems").GetInt32()}");
                Console.WriteLine($"   Categories: {stats.GetProperty("categoriesCount").GetInt32()}");
                Console.WriteLine($"   Cuisines: {stats.GetProperty("cuisinesCount").GetInt32()}");
                
                if (stats.TryGetProperty("dietaryOptions", out var dietary))
                {
                    Console.WriteLine("   Dietary Options:");
                    Console.WriteLine($"     - Vegetarian: {dietary.GetProperty("vegetarian").GetInt32()}");
                    Console.WriteLine($"     - Vegan: {dietary.GetProperty("vegan").GetInt32()}");
                    Console.WriteLine($"     - Gluten-Free: {dietary.GetProperty("glutenFree").GetInt32()}");
                }
            }
            else
            {
                Console.WriteLine($"❌ Get Menu Statistics Failed: {response.StatusCode}");
            }
            Console.WriteLine();
        }

        public static async Task Main(string[] args)
        {
            await RunTestsAsync();
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}