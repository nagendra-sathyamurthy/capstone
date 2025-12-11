using catalog.Models;
using catalog.DataAccess;

namespace Catalog.API.Commands
{
    /// <summary>
    /// Command for updating menu item availability
    /// </summary>
    public class UpdateMenuItemAvailabilityCommand : ICommand<MenuItem>
    {
        private readonly string _menuItemId;
        private readonly bool _isAvailable;
        private readonly MenuItemRepository _menuItemRepository;

        public UpdateMenuItemAvailabilityCommand(string menuItemId, bool isAvailable, MenuItemRepository menuItemRepository)
        {
            _menuItemId = menuItemId;
            _isAvailable = isAvailable;
            _menuItemRepository = menuItemRepository;
        }

        public async Task<MenuItem> ExecuteAsync()
        {
            if (string.IsNullOrEmpty(_menuItemId))
            {
                throw new ArgumentException("Menu item ID cannot be null or empty", nameof(_menuItemId));
            }

            var menuItem = await _menuItemRepository.GetByIdAsync(_menuItemId);
            if (menuItem == null)
            {
                throw new InvalidOperationException($"Menu item with ID {_menuItemId} not found");
            }

            menuItem.IsAvailable = _isAvailable;
            menuItem.UpdatedAt = DateTime.UtcNow;

            await _menuItemRepository.UpdateAsync(_menuItemId, menuItem);
            return menuItem;
        }
    }
}
