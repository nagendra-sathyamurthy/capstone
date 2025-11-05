using Crm.Models;
using Crm.DataAccess;
using MongoDB.Driver;
using System.Collections.Generic;
namespace Crm.Services
{
    public class CustomerService
    {
        private readonly CustomerRepository _repository;

        public CustomerService(IMongoDatabase database)
        {
            _repository = new CustomerRepository(database);
        }

        public IEnumerable<Customer> GetAll()
        {
            return _repository.GetAll();
        }

        public Customer GetById(string id)
        {
            return _repository.GetById(id);
        }


        public void Create(Customer customer)
        {
            // Check for duplicate by email
            var existing = _repository.Find(c => c.Email == customer.Email);
            if (existing != null && existing.Any())
            {
                throw new InvalidOperationException("A customer with this email already exists.");
            }
            customer.Id = Guid.NewGuid().ToString();
            _repository.Insert(customer);
        }

        public void CreateMany(List<Customer> customers)
        {
            foreach (var customer in customers)
            {
                customer.Id = Guid.NewGuid().ToString();
            }
            _repository.InsertMany(customers);
        }

        public void Update(string id, Customer customer)
        {
            customer.Id = id;
            _repository.Update(id, customer);
        }

        public void Delete(string id)
        {
            _repository.Delete(id);
        }
    }
}
