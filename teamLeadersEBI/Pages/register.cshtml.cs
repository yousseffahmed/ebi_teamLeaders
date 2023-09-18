using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MongoDB.Bson;
using MongoDB.Driver;


namespace teamLeadersEBI.Pages
{
    public class textModel : PageModel
    {
        public bool hasData = false;
        public String fullName = "";
        public int nationalID;
        public String bank = "";
        public String title = "";
        public String reportingMang = "";
        public String essay = "";
        byte[] pdfData;
        public String referal;

        public IFormFile PdfFile { get; set; }

        public void OnGet()
        {
            ViewData["Banks"] = getBanks();
        }

        public void OnPost()
        {
            var connectionString = "mongodb://localhost:27017";
            var mongoClient = new MongoClient(connectionString);
            var database = mongoClient.GetDatabase("ebiDB");
            var collection = database.GetCollection<BsonDocument>("regForm");
            fullName = Request.Form["fullName"];
            if (int.TryParse(Request.Form["nationalID"], out int parsedNationalID))
            {
                nationalID = parsedNationalID;
            }
            else
            {
                Console.WriteLine("error");
            }
            bank = Request.Form["bank"];
            title = Request.Form["title"];
            reportingMang = Request.Form["repManger"];
            essay = Request.Form["essay"];
            referal = Request.Form["referal"];
            PdfFile = Request.Form.Files["pdfFile"];

            if (PdfFile != null && PdfFile.Length > 0)
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    PdfFile.CopyTo(ms);
                    pdfData = ms.ToArray();
                }
            }
            if (Exists())
            {
                ViewData["ErrorMessage"] = "A matching National ID already exists.";
            }
            var document = new BsonDocument
            {
                { "fullName",  fullName},
                { "natID", nationalID },
                { "bank",  bank},
                { "title", title },
                { "repMang", reportingMang },
                { "essay", essay },
                { "referal", referal }
            };
            try
            {
                collection.InsertOne(document);
                Console.WriteLine("Document inserted successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error inserting document: {ex.Message}");
            }
        }

        public bool Exists()
        {
            var connectionString = "mongodb://localhost:27017";
            var mongoClient = new MongoClient(connectionString);
            var database = mongoClient.GetDatabase("ebiDB");
            var collection = database.GetCollection<BsonDocument>("regForm");

            var filter = Builders<BsonDocument>.Filter.Empty;

            var cursor = collection.Find(filter).ToCursor();

            foreach (var document in cursor.ToEnumerable())
            {
                var natID = document["natID"].AsInt32;
                if (natID.Equals(nationalID))
                {
                    return true;
                }
            }
            return false;
        }
        public string[] getBanks()
        {
            var connectionString = "mongodb://localhost:27017";
            var mongoClient = new MongoClient(connectionString);
            var database = mongoClient.GetDatabase("Banks");
            var collection = database.GetCollection<BsonDocument>("Banks");

            List<string> banksList = new List<string>();

            var filter = Builders<BsonDocument>.Filter.Empty;
            var cursor = collection.Find(filter).ToCursor();

            foreach (var document in cursor.ToEnumerable())
            {
                var bankName = document["name"].AsString;
                banksList.Add(bankName);
            }

            return banksList.ToArray();
        }
    }
}
