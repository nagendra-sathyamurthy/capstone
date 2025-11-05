using Crm.Models;
using Crm.DataAccess;
using MongoDB.Driver;
namespace Crm.DataAccess
{
    public class CustomerRepository : MongoRepository<Customer>
    {
        public CustomerRepository(IMongoDatabase database)
            : base(database, "Customers")
        {
        }

        public void InsertMany(IEnumerable<Customer> customers)
        {
            // Use the underlying collection to insert many customers
            _collection.InsertMany(customers);
        }
    }
}
