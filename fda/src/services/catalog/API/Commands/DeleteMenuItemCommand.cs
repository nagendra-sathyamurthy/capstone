using catalog.Models;
using catalog.DataAccess;

namespace catalog.API.Commands
{
    /// <summary>
    /// Command for deleting a menu item
    /// </summary>
    public class DeleteMenuItemCommand : ICommand
    {
        private readonly string _menuItemId;
        private readonly MenuItemRepository _menuRepository;
        private readonly bool _softDelete;

        public DeleteMenuItemCommand(
            string menuItemId,
            MenuItemRepository menuRepository,
            bool softDelete = true)
        {
            _menuItemId = menuItemId;
            _menuRepository = menuRepository;
            _softDelete = softDelete;
        }

        public Task ExecuteAsync()
        {
            // Validate inputs
            if (string.IsNullOrEmpty(_menuItemId))
            {
                throw new ArgumentException("Menu item ID cannot be null or empty", nameof(_menuItemId));
            }

            // Get existing menu item
            var menuItem = _menuRepository.GetById(_menuItemId);
            if (menuItem == null)
            {
                throw new InvalidOperationException($"Menu item with ID {_menuItemId} not found");
            }

            if (_softDelete)
            {
                // Soft delete - mark as unavailable
                menuItem.IsAvailable = false;
                menuItem.UpdatedAt = DateTime.UtcNow;
                _menuRepository.Update(_menuItemId, menuItem);
            }
            else
            {
                // Hard delete - remove from database
                _menuRepository.Delete(_menuItemId);
            }
            
            return Task.CompletedTask;
        }
    }
}
