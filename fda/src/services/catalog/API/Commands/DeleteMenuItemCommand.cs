using Catalog.Models;
using Catalog.DataAccess;

namespace Catalog.API.Commands
{
    /// <summary>
    /// Command for deleting a menu item
    /// </summary>
    public class DeleteMenuItemCommand : ICommand
    {
        private readonly string _menuItemId;
        private readonly MenuRepository _menuRepository;
        private readonly bool _softDelete;

        public DeleteMenuItemCommand(
            string menuItemId,
            MenuRepository menuRepository,
            bool softDelete = true)
        {
            _menuItemId = menuItemId;
            _menuRepository = menuRepository;
            _softDelete = softDelete;
        }

        public async Task ExecuteAsync()
        {
            // Validate inputs
            if (string.IsNullOrEmpty(_menuItemId))
            {
                throw new ArgumentException("Menu item ID cannot be null or empty", nameof(_menuItemId));
            }

            // Get existing menu item
            var menuItem = await _menuRepository.GetByIdAsync(_menuItemId);
            if (menuItem == null)
            {
                throw new InvalidOperationException($"Menu item with ID {_menuItemId} not found");
            }

            if (_softDelete)
            {
                // Soft delete - mark as unavailable
                menuItem.IsAvailable = false;
                menuItem.UpdatedAt = DateTime.UtcNow;
                await _menuRepository.UpdateAsync(menuItem);
            }
            else
            {
                // Hard delete - remove from database
                await _menuRepository.DeleteAsync(_menuItemId);
            }
        }
    }
}
