using Crm.Models;
using Crm.DataAccess;
using MongoDB.Driver;

namespace Crm.DataAccess
{
    public class UserProfileRepository : MongoRepository<UserProfile>
    {
        public UserProfileRepository(IMongoDatabase database) 
            : base(database, "userprofiles")
        {
        }

        /// <summary>
        /// Get user profile by UserId (reference to Authentication service)
        /// </summary>
        public UserProfile? GetByUserId(string userId)
        {
            var filter = Builders<UserProfile>.Filter.Eq(p => p.UserId, userId);
            return _collection.Find(filter).FirstOrDefault();
        }

        /// <summary>
        /// Get user profiles by role
        /// </summary>
        public List<UserProfile> GetByRole(UserRole role)
        {
            var filter = Builders<UserProfile>.Filter.Eq(p => p.Role, role);
            return _collection.Find(filter).ToList();
        }

        /// <summary>
        /// Get user profiles by email (for lookup purposes)
        /// </summary>
        public UserProfile? GetByEmail(string email)
        {
            var filter = Builders<UserProfile>.Filter.Eq(p => p.Email, email);
            return _collection.Find(filter).FirstOrDefault();
        }

        /// <summary>
        /// Update user profile and set UpdatedAt timestamp
        /// </summary>
        public bool UpdateUserProfile(string id, UserProfile userProfile)
        {
            userProfile.UpdatedAt = DateTime.UtcNow;
            
            var filter = Builders<UserProfile>.Filter.Eq(p => p.Id, id);
            var result = _collection.ReplaceOne(filter, userProfile);
            
            return result.ModifiedCount > 0;
        }

        /// <summary>
        /// Search user profiles by name (first name or last name)
        /// </summary>
        public List<UserProfile> SearchByName(string searchTerm)
        {
            var filter = Builders<UserProfile>.Filter.Or(
                Builders<UserProfile>.Filter.Regex(p => p.FirstName, new MongoDB.Bson.BsonRegularExpression(searchTerm, "i")),
                Builders<UserProfile>.Filter.Regex(p => p.LastName, new MongoDB.Bson.BsonRegularExpression(searchTerm, "i"))
            );
            
            return _collection.Find(filter).ToList();
        }
    }
}