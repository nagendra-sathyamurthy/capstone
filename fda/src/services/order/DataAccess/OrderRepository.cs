using Order.Models;
using MongoDB.Driver;

namespace Order.DataAccess
{
    public class OrderRepository : MongoRepository<Models.Order>
    {
        public OrderRepository(IMongoDatabase database) 
            : base(database, "Orders")
        {
        }

        public IEnumerable<Models.Order> GetByRestaurantId(string restaurantId)
        {
            return _collection.Find(order => order.RestaurantId == restaurantId).ToList();
        }

        public IEnumerable<Models.Order> GetByCustomerId(string customerId)
        {
            return _collection.Find(order => order.CustomerId == customerId).ToList();
        }

        public IEnumerable<Models.Order> GetByStatus(string restaurantId, OrderStatus status)
        {
            return _collection.Find(order => 
                order.RestaurantId == restaurantId && 
                order.Status == status).ToList();
        }

        public IEnumerable<Models.Order> GetPendingOrders(string restaurantId)
        {
            return _collection.Find(order => 
                order.RestaurantId == restaurantId && 
                order.Status == OrderStatus.Pending).ToList();
        }

        public IEnumerable<Models.Order> GetReadyForPickup(string restaurantId)
        {
            return _collection.Find(order => 
                order.RestaurantId == restaurantId && 
                order.Status == OrderStatus.ReadyForPickup).ToList();
        }

        public void UpdateOrderStatus(string id, OrderStatus status)
        {
            var filter = Builders<Models.Order>.Filter.Eq(o => o.Id, id);
            var update = Builders<Models.Order>.Update
                .Set(o => o.Status, status)
                .Set(o => o.UpdatedAt, DateTime.UtcNow);
            
            _collection.UpdateOne(filter, update);
        }

        public void UpdatePackaging(string id, PackagingDetails packaging)
        {
            var filter = Builders<Models.Order>.Filter.Eq(o => o.Id, id);
            var update = Builders<Models.Order>.Update
                .Set(o => o.Packaging, packaging)
                .Set(o => o.UpdatedAt, DateTime.UtcNow);
            
            _collection.UpdateOne(filter, update);
        }

        public void SetHandoverOTP(string id, string otp)
        {
            var filter = Builders<Models.Order>.Filter.Eq(o => o.Id, id);
            var update = Builders<Models.Order>.Update
                .Set(o => o.HandoverOTP, otp)
                .Set(o => o.HandoverOTPGeneratedAt, DateTime.UtcNow)
                .Set(o => o.UpdatedAt, DateTime.UtcNow);
            
            _collection.UpdateOne(filter, update);
        }

        public void CompleteHandover(string id, string operatorId)
        {
            var filter = Builders<Models.Order>.Filter.Eq(o => o.Id, id);
            var update = Builders<Models.Order>.Update
                .Set(o => o.IsHandedOver, true)
                .Set(o => o.HandoverTime, DateTime.UtcNow)
                .Set(o => o.HandoverBy, operatorId)
                .Set(o => o.Status, OrderStatus.OutForDelivery)
                .Set(o => o.UpdatedAt, DateTime.UtcNow);
            
            _collection.UpdateOne(filter, update);
        }

        public new void Update(string id, Models.Order entity)
        {
            entity.UpdatedAt = DateTime.UtcNow;
            base.Update(id, entity);
        }
    }
}
