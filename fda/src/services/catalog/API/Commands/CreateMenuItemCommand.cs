using catalog.Models;
using catalog.DataAccess;

namespace Catalog.API.Commands
{
    /// <summary>
    /// Command for creating a new menu item
    /// </summary>
    public class CreateMenuItemCommand : ICommand<MenuItem>
    {
        private readonly MenuItem _menuItem;
        private readonly MenuItemRepository _menuItemRepository;

        public CreateMenuItemCommand(MenuItem menuItem, MenuItemRepository menuItemRepository)
        {
            _menuItem = menuItem;
            _menuItemRepository = menuItemRepository;
        }

        public async Task<MenuItem> ExecuteAsync()
        {
            if (_menuItem == null)
            {
                throw new ArgumentNullException(nameof(_menuItem), "Menu item cannot be null");
            }

            // Validate required fields
            ValidateMenuItem();

            // Set timestamps
            _menuItem.CreatedAt = DateTime.UtcNow;
            _menuItem.UpdatedAt = DateTime.UtcNow;

            await _menuItemRepository.AddAsync(_menuItem);
            return _menuItem;
        }

        private void ValidateMenuItem()
        {
            if (string.IsNullOrEmpty(_menuItem.Name))
            {
                throw new ArgumentException("Menu item name is required");
            }

            if (string.IsNullOrEmpty(_menuItem.Description))
            {
                throw new ArgumentException("Menu item description is required");
            }

            if (_menuItem.PreparationTimeMinutes <= 0)
            {
                throw new ArgumentException("Preparation time must be greater than zero");
            }

            if (string.IsNullOrEmpty(_menuItem.PackagingSize))
            {
                throw new ArgumentException("Packaging size is required");
            }

            if (string.IsNullOrEmpty(_menuItem.UnitOfMeasure))
            {
                throw new ArgumentException("Unit of measure is required");
            }

            if (_menuItem.PricePerUOM <= 0)
            {
                throw new ArgumentException("Price must be greater than zero");
            }
        }
    }
}
