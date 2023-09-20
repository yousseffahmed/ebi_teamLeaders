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
        public AdminModel.BankInfo bankInfo =  new AdminModel.BankInfo();

        public IActionResult OnPost()
        {
            var name = Request.Form["name"].ToString();
            var document = new BsonDocument
            {
                {"name", name}
            };
            var connectionString = "mongodb://localhost:27017";
            var mongoClient = new MongoClient(connectionString);
            var database = mongoClient.GetDatabase("ebiDB");
            var collection = database.GetCollection<BsonDocument>("Banks");

            try
            {
                collection.InsertOne(document);
                Console.WriteLine("Document inserted successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error inserting document: {ex.Message}");
            }
            return RedirectToPage("Admin");
        }


        public void OnGet()
        {
            var banks = GetBanks();
            ViewData["Banks"] = banks;
        }

        public IEnumerable<BankInfo> GetBanks()
        {
            var connectionString = "mongodb://localhost:27017";
            var mongoClient = new MongoClient(connectionString);
            var database = mongoClient.GetDatabase("ebiDB");
            var collection = database.GetCollection<BankInfo>("Banks");
            var filter = Builders<BankInfo>.Filter.Empty;
            var sortDefinition = Builders<BankInfo>.Sort.Ascending(b => b.Name);
            var banks = collection.Find(filter).Sort(sortDefinition).ToList();
            return banks;
        }


        public void DeleteBank(string id)
        {
            var connectionString = "mongodb://localhost:27017";
            var mongoClient = new MongoClient(connectionString);
            var database = mongoClient.GetDatabase("ebiDB");
            var collection = database.GetCollection<BankInfo>("Banks");

            var filter = Builders<BankInfo>.Filter.Eq("_id", ObjectId.Parse(id));
            collection.DeleteOne(filter);
        }


        public IActionResult OnPostDelete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            DeleteBank(id); 

            return RedirectToPage("/Admin");
        }



        public class BankInfo
        {
            [BsonId]
            [BsonRepresentation(BsonType.ObjectId)]
            public string Id { get; set; }

            [BsonElement("name")] 
            public string Name { get; set; }
        }
    }
}
