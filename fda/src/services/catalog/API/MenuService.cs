using catalog.Models;
using catalog.DataAccess;
using MongoDB.Driver;

namespace catalog.API
{
    public class MenuService
    {
        private readonly MenuItemRepository _menuItemRepository;

        public MenuService(IMongoClient mongoClient)
        {
            var database = mongoClient.GetDatabase("CatalogDb");
            _menuItemRepository = new MenuItemRepository(database);
        }

        // Basic CRUD operations
        public Task<List<MenuItem>> GetAllMenuItemsAsync()
        {
            return Task.FromResult(_menuItemRepository.GetAll().ToList());
        }

        public Task<List<MenuItem>> GetAvailableMenuItemsAsync()
        {
            return Task.FromResult(_menuItemRepository.GetAvailableItems().ToList());
        }

        public Task<MenuItem?> GetMenuItemByIdAsync(string id)
        {
            return Task.FromResult(_menuItemRepository.GetById(id));
        }

        public Task CreateMenuItemAsync(MenuItem menuItem)
        {
            menuItem.CreatedAt = DateTime.UtcNow;
            menuItem.UpdatedAt = DateTime.UtcNow;
            _menuItemRepository.Insert(menuItem);
            return Task.CompletedTask;
        }

        public Task CreateMenuItemsAsync(List<MenuItem> menuItems)
        {
            var now = DateTime.UtcNow;
            foreach (var item in menuItems)
            {
                item.CreatedAt = now;
                item.UpdatedAt = now;
            }
            _menuItemRepository.InsertMany(menuItems);
            return Task.CompletedTask;
        }

        public Task UpdateMenuItemAsync(string id, MenuItem menuItem)
        {
            menuItem.UpdatedAt = DateTime.UtcNow;
            _menuItemRepository.Update(id, menuItem);
            return Task.CompletedTask;
        }

        public Task DeleteMenuItemAsync(string id)
        {
            _menuItemRepository.Delete(id);
            return Task.CompletedTask;
        }

        public Task UpdateAvailabilityAsync(string id, bool isAvailable)
        {
            _menuItemRepository.UpdateAvailability(id, isAvailable);
            return Task.CompletedTask;
        }

        // Food-specific query methods
        public Task<List<MenuItem>> GetMenuItemsByCategoryAsync(string category)
        {
            return Task.FromResult(_menuItemRepository.GetByCategory(category).ToList());
        }

        public Task<List<MenuItem>> GetMenuItemsByCuisineAsync(string cuisine)
        {
            return Task.FromResult(_menuItemRepository.GetByCuisine(cuisine).ToList());
        }

        public Task<List<MenuItem>> GetVegetarianMenuItemsAsync()
        {
            return Task.FromResult(_menuItemRepository.GetVegetarianItems().ToList());
        }

        public Task<List<MenuItem>> GetVeganMenuItemsAsync()
        {
            return Task.FromResult(_menuItemRepository.GetVeganItems().ToList());
        }

        public Task<List<MenuItem>> GetGlutenFreeMenuItemsAsync()
        {
            return Task.FromResult(_menuItemRepository.GetGlutenFreeItems().ToList());
        }

        public Task<List<MenuItem>> GetMenuItemsBySpiceLevelAsync(int spiceLevel)
        {
            return Task.FromResult(_menuItemRepository.GetItemsBySpiceLevel(spiceLevel).ToList());
        }

        public Task<List<MenuItem>> GetMenuItemsByPriceRangeAsync(decimal minPrice, decimal maxPrice)
        {
            return Task.FromResult(_menuItemRepository.GetItemsByPriceRange(minPrice, maxPrice).ToList());
        }

        public Task<List<MenuItem>> SearchMenuItemsByNameAsync(string searchTerm)
        {
            return Task.FromResult(_menuItemRepository.SearchByName(searchTerm).ToList());
        }

        public Task<List<MenuItem>> GetQuickMenuItemsAsync(int maxMinutes = 30)
        {
            return Task.FromResult(_menuItemRepository.GetItemsByPreparationTime(maxMinutes).ToList());
        }

        // Menu organization methods
        public Task<Dictionary<string, List<MenuItem>>> GetMenuByCategoriesAsync()
        {
            var allItems = _menuItemRepository.GetAvailableItems();
            var categorizedMenu = allItems
                .GroupBy(item => item.Category)
                .ToDictionary(g => g.Key, g => g.ToList());
            
            return Task.FromResult(categorizedMenu);
        }

        public Task<Dictionary<string, List<MenuItem>>> GetMenuByCuisinesAsync()
        {
            var allItems = _menuItemRepository.GetAvailableItems();
            var cuisineMenu = allItems
                .GroupBy(item => item.Cuisine)
                .ToDictionary(g => g.Key, g => g.ToList());
            
            return Task.FromResult(cuisineMenu);
        }

        public Task<List<string>> GetAvailableCategoriesAsync()
        {
            var categories = _menuItemRepository.GetAvailableItems()
                .Select(item => item.Category)
                .Where(category => !string.IsNullOrEmpty(category))
                .Distinct()
                .OrderBy(category => category)
                .ToList();
            
            return Task.FromResult(categories);
        }

        public Task<List<string>> GetAvailableCuisinesAsync()
        {
            var cuisines = _menuItemRepository.GetAvailableItems()
                .Select(item => item.Cuisine)
                .Where(cuisine => !string.IsNullOrEmpty(cuisine))
                .Distinct()
                .OrderBy(cuisine => cuisine)
                .ToList();
            
            return Task.FromResult(cuisines);
        }

        // Dietary filter combinations
        public Task<List<MenuItem>> GetFilteredMenuItemsAsync(
            string? category = null,
            string? cuisine = null,
            bool? isVegetarian = null,
            bool? isVegan = null,
            bool? isGlutenFree = null,
            int? maxSpiceLevel = null,
            decimal? minPrice = null,
            decimal? maxPrice = null,
            int? maxPreparationTime = null)
        {
            var items = _menuItemRepository.GetAvailableItems().AsQueryable();

            if (!string.IsNullOrEmpty(category))
                items = items.Where(item => item.Category == category);

            if (!string.IsNullOrEmpty(cuisine))
                items = items.Where(item => item.Cuisine == cuisine);

            if (isVegetarian.HasValue)
                items = items.Where(item => item.IsVegetarian == isVegetarian.Value);

            if (isVegan.HasValue)
                items = items.Where(item => item.IsVegan == isVegan.Value);

            if (isGlutenFree.HasValue)
                items = items.Where(item => item.IsGlutenFree == isGlutenFree.Value);

            if (maxSpiceLevel.HasValue)
                items = items.Where(item => item.SpiceLevel <= maxSpiceLevel.Value);

            if (minPrice.HasValue)
                items = items.Where(item => item.PricePerUOM >= minPrice.Value);

            if (maxPrice.HasValue)
                items = items.Where(item => item.PricePerUOM <= maxPrice.Value);

            if (maxPreparationTime.HasValue)
                items = items.Where(item => item.PreparationTimeMinutes <= maxPreparationTime.Value);

            return Task.FromResult(items.ToList());
        }
    }
}