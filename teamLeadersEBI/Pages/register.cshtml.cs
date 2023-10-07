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
        public String nationalID;
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
            var connectionString = "mongodb+srv://maramhossama:marmar123@cluster0.qyrbfln.mongodb.net/";
            var mongoClient = new MongoClient(connectionString);
            var database = mongoClient.GetDatabase("ebi");
            var collection = database.GetCollection<BsonDocument>("regForm");

            fullName = Request.Form["fullName"];
            nationalID = Request.Form["nationalID"];
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
                    Console.WriteLine($"pdfData has {pdfData.Length} bytes of data.");
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
                { "referal", referal },
                { "status", " " },
                { "reason", " " },
                { "date", getTodaysDate() }
            };

            if (pdfData != null && pdfData.Length > 0)
            {
                document.Add("pdfData", new BsonBinaryData(pdfData));
            }


            try
            {
                collection.InsertOne(document);
                Console.WriteLine("Document inserted successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error inserting document: {ex.Message}");
            }
            ViewData["Banks"] = getBanks();
        }

        public bool Exists()
        {
            var connectionString = "mongodb+srv://maramhossama:marmar123@cluster0.qyrbfln.mongodb.net/";
            var mongoClient = new MongoClient(connectionString);
            var database = mongoClient.GetDatabase("ebi");
            var collection = database.GetCollection<BsonDocument>("regForm");

            var filter = Builders<BsonDocument>.Filter.Empty;

            var cursor = collection.Find(filter).ToCursor();

            foreach (var document in cursor.ToEnumerable())
            {
                var natIDValue = document["natID"];

                if (natIDValue.BsonType == MongoDB.Bson.BsonType.Int32)
                {
                    var natID = natIDValue.AsInt32.ToString();
                    if (natID.Equals(nationalID.ToString()))
                    {
                        return true;
                    }
                }
                else if (natIDValue.BsonType == MongoDB.Bson.BsonType.String)
                {
                    var natID = natIDValue.AsString;
                    if (natID.Equals(nationalID.ToString()))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
        public string[] getBanks()
        {
            var connectionString = "mongodb+srv://maramhossama:marmar123@cluster0.qyrbfln.mongodb.net/";
            var mongoClient = new MongoClient(connectionString);
            var database = mongoClient.GetDatabase("ebi");
            var collection = database.GetCollection<BsonDocument>("banks");

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
        public DateTime getTodaysDate()
        {
            DateTime today = DateTime.Today;
            return today;
        }
    }
}
