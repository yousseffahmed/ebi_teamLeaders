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
            var connectionString = "mongodb+srv://maramhossama:marmar123@cluster0.qyrbfln.mongodb.net/";
            var mongoClient = new MongoClient(connectionString);
            var database = mongoClient.GetDatabase("ebi");
            var collection = database.GetCollection<BsonDocument>("banks");


            //var connectionString = "mongodb+srv://maramhossama:marmar123@cluster0.qyrbfln.mongodb.net/";
            //var mongoClient = new MongoClient(connectionString);
            //var database = mongoClient.GetDatabase("ebi");
            //var collection = database.GetCollection<BsonDocument>("banks");


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
            var connectionString = "mongodb+srv://maramhossama:marmar123@cluster0.qyrbfln.mongodb.net/";
            var mongoClient = new MongoClient(connectionString);
            var database = mongoClient.GetDatabase("ebi");
            var collection = database.GetCollection<BankInfo>("banks"); // Use BankInfo as the document type

            var filter = Builders<BankInfo>.Filter.Empty; // Use BankInfo for the filter type
            var sortDefinition = Builders<BankInfo>.Sort.Ascending(b => b.Name); // Assuming "Name" is a property in your BankInfo class

            var banks = collection.Find(filter).Sort(sortDefinition).ToEnumerable(); // Map to BankInfo objects

            return banks;
        }



        public void DeleteBank(string id)
        {
            var connectionString = "mongodb+srv://maramhossama:marmar123@cluster0.qyrbfln.mongodb.net/";
            var mongoClient = new MongoClient(connectionString);
            var database = mongoClient.GetDatabase("ebi");
            var collection = database.GetCollection<BsonDocument>("banks");

            var filter = Builders<BsonDocument>.Filter.Eq("_id", ObjectId.Parse(id));
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


        public IActionResult OnPostUpdate(string id, string name)
        {
            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(name))
            {
                return NotFound();
            }
            var connectionString = "mongodb+srv://maramhossama:marmar123@cluster0.qyrbfln.mongodb.net/";
            var mongoClient = new MongoClient(connectionString);
            var database = mongoClient.GetDatabase("ebi");
            var collection = database.GetCollection<BankInfo>("banks");
            //var connectionString = "mongodb+srv://maramhossama:marmar123@cluster0.qyrbfln.mongodb.net/";
            //var mongoClient = new MongoClient(connectionString);
            //var database = mongoClient.GetDatabase("ebi");
            //var collection = database.GetCollection<BankInfo>("banks");

            var filter = Builders<BankInfo>.Filter.Eq("_id", ObjectId.Parse(id));

            // Use the Update.Set method to update the "Name" field
            var update = Builders<BankInfo>.Update.Set("Name", name);

            // Use UpdateOptions to specify upsert as false to prevent inserting a new document
            var options = new UpdateOptions { IsUpsert = false };

            // Perform the update
            collection.UpdateOne(filter, update, options);

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
