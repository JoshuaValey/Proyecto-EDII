using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APIApp.Services
{
    public class MongoDataServices
    {
        private MongoServer server;
        private string database { get; set; }
        public MongoDataServices(string connectionString)
        {
            MongoClient client = new MongoClient(connectionString);
            server = client.GetServer();
        }

        public string search(string databaseName, string collectionName, string query)
        {
            var db = server.GetDatabase(databaseName);
            var collection = db.GetCollection(collectionName);
            BsonDocument bsonDoc = MongoDB.Bson.Serialization.BsonSerializer.Deserialize<BsonDocument>(query);

            var result = collection.FindOne(new QueryDocument(bsonDoc));
            if (result != null)
            {
                return result.ToJson();
            }
            else
            {
                return "";
            }
        }
    }
}
