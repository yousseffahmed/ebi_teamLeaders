using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Bson.Serialization.Attributes;
using teamLeadersEBI;


namespace teamLeadersEBI.Pages
{
    public class AdminModel : PageModel
    {


        public void OnGet()
        {



            // var connectionString = "mongodb://localhost:27017";
            //  var mongoClient = new MongoClient(connectionString);
            // var database = mongoClient.GetDatabase("Banks");
            // var collection = database.GetCollection<BsonDocument>("Banks");
            // ViewData["Banks"] = getBanks();

            //  var banks = getBanks();
            // Console.WriteLine($"Number of banks retrieved: {banks.Length}");
            // ViewData["Banks"] = banks;
            var banks = GetBanks();

            // Set ViewData["Banks"] with the fetched collection
            ViewData["Banks"] = banks;



        }


        


        
        public IEnumerable<BankInfo> GetBanks()
        {
            var connectionString = "mongodb://localhost:27017";
            var mongoClient = new MongoClient(connectionString);
            var database = mongoClient.GetDatabase("ebiDB");
            var collection = database.GetCollection<BankInfo>("Banks");

            var filter = Builders<BankInfo>.Filter.Empty;
            var banks = collection.Find(filter).ToList();

            return banks;
        }



        public void DeleteBank(string id)
        {
            var connectionString = "mongodb://localhost:27017";
            var mongoClient = new MongoClient(connectionString);
            var database = mongoClient.GetDatabase("ebiDB");
            var collection = database.GetCollection<BankInfo>("Banks");

            var filter = Builders<BankInfo>.Filter.Eq("_id", ObjectId.Parse(id)); // Convert the id to ObjectId
            collection.DeleteOne(filter);
        }


        public IActionResult OnPostDelete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            DeleteBank(id); // Call your DeleteBank method here to delete the record

            // Redirect to a page after successful deletion (you can customize this)
            return RedirectToPage("/Admin"); // Replace "/admin" with the desired URL
        }



        public class BankInfo
        {
            [BsonId] // Mark this property as the ObjectId (_id) field
            [BsonRepresentation(BsonType.ObjectId)]
            public string Id { get; set; }

            [BsonElement("name")] // Map the "name" field to this property
            public string Name { get; set; }
        }
    }
}
