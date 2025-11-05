using System;
using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Authentication.Models
{
    public class UserAccount
    {

    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }

        [Required]
        [MinLength(6)]
        public string Password { get; set; }

        public int InvalidLogins { get; set; }

        public DateTime LastLoginTime { get; set; }
    }
}