using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Authentication.DataAccess
{
    public class MongoRepository<T> : IRepository<T>
    {
        private readonly IMongoCollection<T> _collection;

        public MongoRepository(IMongoDatabase database, string collectionName)
        {
            _collection = database.GetCollection<T>(collectionName);
        }

        public async Task<T> GetByIdAsync(string id)
        {
            var filter = Builders<T>.Filter.Eq("Email", id);
            return await _collection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _collection.Find(Builders<T>.Filter.Empty).ToListAsync();
        }

        public async Task AddAsync(T entity)
        {
            await _collection.InsertOneAsync(entity);
        }

        public async Task UpdateAsync(T entity)
        {
            // Implement update logic based on your entity's key
        }

        public async Task DeleteAsync(string id)
        {
            var filter = Builders<T>.Filter.Eq("email", id);
            await _collection.DeleteOneAsync(filter);
        }
    }
}