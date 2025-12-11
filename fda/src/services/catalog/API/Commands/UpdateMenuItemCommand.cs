using Catalog.Models;
using Catalog.DataAccess;

namespace Catalog.API.Commands
{
    /// <summary>
    /// Command for updating an existing menu item
    /// </summary>
    public class UpdateMenuItemCommand : ICommand<MenuItem>
    {
        private readonly string _menuItemId;
        private readonly MenuItem _updatedMenuItem;
        private readonly MenuRepository _menuRepository;

        public UpdateMenuItemCommand(
            string menuItemId,
            MenuItem updatedMenuItem,
            MenuRepository menuRepository)
        {
            _menuItemId = menuItemId;
            _updatedMenuItem = updatedMenuItem;
            _menuRepository = menuRepository;
        }

        public async Task<MenuItem> ExecuteAsync()
        {
            // Validate inputs
            if (string.IsNullOrEmpty(_menuItemId))
            {
                throw new ArgumentException("Menu item ID cannot be null or empty", nameof(_menuItemId));
            }

            if (_updatedMenuItem == null)
            {
                throw new ArgumentException("Updated menu item data cannot be null", nameof(_updatedMenuItem));
            }

            // Validate required fields
            if (string.IsNullOrEmpty(_updatedMenuItem.Name))
            {
                throw new ArgumentException("Menu item name is required", nameof(_updatedMenuItem.Name));
            }

            if (string.IsNullOrEmpty(_updatedMenuItem.Description))
            {
                throw new ArgumentException("Description is required", nameof(_updatedMenuItem.Description));
            }

            if (_updatedMenuItem.PreparationTimeMinutes <= 0)
            {
                throw new ArgumentException("Preparation time must be greater than 0", nameof(_updatedMenuItem.PreparationTimeMinutes));
            }

            if (string.IsNullOrEmpty(_updatedMenuItem.PackagingSize))
            {
                throw new ArgumentException("Packaging size is required", nameof(_updatedMenuItem.PackagingSize));
            }

            if (string.IsNullOrEmpty(_updatedMenuItem.UnitOfMeasure))
            {
                throw new ArgumentException("Unit of measure is required", nameof(_updatedMenuItem.UnitOfMeasure));
            }

            if (_updatedMenuItem.PricePerUOM <= 0)
            {
                throw new ArgumentException("Price must be greater than 0", nameof(_updatedMenuItem.PricePerUOM));
            }

            if (string.IsNullOrEmpty(_updatedMenuItem.RestaurantId))
            {
                throw new ArgumentException("Restaurant ID is required", nameof(_updatedMenuItem.RestaurantId));
            }

            // Get existing menu item
            var existingMenuItem = await _menuRepository.GetByIdAsync(_menuItemId);
            if (existingMenuItem == null)
            {
                throw new InvalidOperationException($"Menu item with ID {_menuItemId} not found");
            }

            // Update fields (preserve MenuId and CreatedAt)
            existingMenuItem.Name = _updatedMenuItem.Name;
            existingMenuItem.Description = _updatedMenuItem.Description;
            existingMenuItem.PreparationTimeMinutes = _updatedMenuItem.PreparationTimeMinutes;
            existingMenuItem.PackagingSize = _updatedMenuItem.PackagingSize;
            existingMenuItem.UnitOfMeasure = _updatedMenuItem.UnitOfMeasure;
            existingMenuItem.PricePerUOM = _updatedMenuItem.PricePerUOM;
            existingMenuItem.RestaurantId = _updatedMenuItem.RestaurantId;
            existingMenuItem.ItemImage = _updatedMenuItem.ItemImage;
            existingMenuItem.IsAvailable = _updatedMenuItem.IsAvailable;
            existingMenuItem.UpdatedAt = DateTime.UtcNow;

            // Update in database
            await _menuRepository.UpdateAsync(existingMenuItem);

            return existingMenuItem;
        }
    }
}
