using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Xml.Linq;

namespace teamLeadersEBI.Pages
{
    public class AddModel : PageModel
    {
        public AdminModel.BankInfo bankInfo =  new AdminModel.BankInfo();

       // public String name = "";
        public void OnGet()
        {

        }




        public void OnPost()
        {
            // Convert the value to a string and create a BsonDocument
            var name = Request.Form["name"].ToString();
            var document = new BsonDocument
    {
        {"name", name}
    };

            var connectionString = "mongodb+srv://maramhossama:marmar123@cluster0.qyrbfln.mongodb.net/";
            var mongoClient = new MongoClient(connectionString);
            var database = mongoClient.GetDatabase("ebi");
            var collection = database.GetCollection<BsonDocument>("banks");

            try
            {
                // Insert the BsonDocument into the collection
                collection.InsertOne(document);
                Console.WriteLine("Document inserted successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error inserting document: {ex.Message}");
            }
        }



    }
}
