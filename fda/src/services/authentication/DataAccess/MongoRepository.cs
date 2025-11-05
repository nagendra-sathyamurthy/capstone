using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MongoDB.Bson;
using System.Reflection;

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
            // Check if the entity has an Id property with BsonId attribute
            var idProperty = typeof(T).GetProperties()
                .FirstOrDefault(p => p.GetCustomAttribute<MongoDB.Bson.Serialization.Attributes.BsonIdAttribute>() != null);
            
            if (idProperty != null)
            {
                var filter = Builders<T>.Filter.Eq(idProperty.Name, ObjectId.Parse(id));
                return await _collection.Find(filter).FirstOrDefaultAsync();
            }
            
            // Fallback to Email if no BsonId found
            var emailFilter = Builders<T>.Filter.Eq("Email", id);
            return await _collection.Find(emailFilter).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _collection.Find(Builders<T>.Filter.Empty).ToListAsync();
        }

        public async Task<IEnumerable<T>> FindAsync(FilterDefinition<T> filter)
        {
            return await _collection.Find(filter).ToListAsync();
        }

        public async Task<T> FindOneAsync(FilterDefinition<T> filter)
        {
            return await _collection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            return await _collection.Find(predicate).ToListAsync();
        }

        public async Task<T> FindOneAsync(Expression<Func<T, bool>> predicate)
        {
            return await _collection.Find(predicate).FirstOrDefaultAsync();
        }

        public async Task AddAsync(T entity)
        {
            await _collection.InsertOneAsync(entity);
        }

        public async Task UpdateAsync(T entity)
        {
            // Get the Id property value
            var idProperty = typeof(T).GetProperties()
                .FirstOrDefault(p => p.GetCustomAttribute<MongoDB.Bson.Serialization.Attributes.BsonIdAttribute>() != null);
            
            if (idProperty != null)
            {
                var idValue = idProperty.GetValue(entity);
                if (idValue != null)
                {
                    var filter = Builders<T>.Filter.Eq("_id", ObjectId.Parse(idValue.ToString()));
                    await _collection.ReplaceOneAsync(filter, entity);
                    return;
                }
            }
            
            // If no BsonId property found, throw an exception
            throw new InvalidOperationException("Entity must have a property marked with BsonId attribute for updates");
        }

        public async Task DeleteAsync(string id)
        {
            var filter = Builders<T>.Filter.Eq("_id", ObjectId.Parse(id));
            await _collection.DeleteOneAsync(filter);
        }

        public async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate)
        {
            var count = await _collection.CountDocumentsAsync(predicate);
            return count > 0;
        }
    }
}