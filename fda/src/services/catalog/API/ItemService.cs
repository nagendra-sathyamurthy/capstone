using catalog.Models;
using MongoDB.Driver;

namespace catalog.API
{
    public class ItemService
    {
        private readonly ItemRepository _itemRepository;

        public ItemService(IMongoClient mongoClient)
        {
            var database = mongoClient.GetDatabase("CatalogDb");
            _itemRepository = new ItemRepository(database);
        }

        public Task<List<Item>> GetAllAsync()
        {
            return Task.FromResult(_itemRepository.GetAll().ToList());
        }

        public Task<Item?> GetByIdAsync(string id)
        {
            return Task.FromResult(_itemRepository.GetById(id));
        }

        public Task CreateAsync(Item item)
        {
            _itemRepository.Insert(item);
            return Task.CompletedTask;
        }

        public Task CreateManyAsync(List<Item> items)
        {
            _itemRepository.InsertMany(items);
            return Task.CompletedTask;
        }

        public Task UpdateAsync(string id, Item item)
        {
            _itemRepository.Update(id, item);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(string id)
        {
            _itemRepository.Delete(id);
            return Task.CompletedTask;
        }
    }
}
