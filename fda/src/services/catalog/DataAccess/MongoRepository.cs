using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace catalog.DataAccess
{
    public class MongoRepository<T> : IRepository<T>
    {
        protected readonly IMongoCollection<T> _collection;

        public MongoRepository(IMongoDatabase database, string collectionName)
        {
            _collection = database.GetCollection<T>(collectionName);
        }

        public IEnumerable<T> GetAll()
        {
            return _collection.Find(_ => true).ToList();
        }

        public T GetById(string id)
        {
            return _collection.Find(Builders<T>.Filter.Eq("_id", id)).FirstOrDefault();
        }


        public void Insert(T entity)
        {
            _collection.InsertOne(entity);
        }

        public void InsertMany(IEnumerable<T> entities)
        {
            _collection.InsertMany(entities);
        }

        public void Update(string id, T entity)
        {
            _collection.ReplaceOne(Builders<T>.Filter.Eq("_id", id), entity);
        }

        public void Delete(string id)
        {
            _collection.DeleteOne(Builders<T>.Filter.Eq("_id", id));
        }

        public IEnumerable<T> Find(Expression<Func<T, bool>> filter)
        {
            return _collection.AsQueryable().Where(filter).ToList();
        }
    }
}
