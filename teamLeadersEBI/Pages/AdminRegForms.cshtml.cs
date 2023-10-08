using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using MongoDB.Driver;
using static teamLeadersEBI.Pages.AdminModel;

namespace teamLeadersEBI.Pages
{
    public class AdminRegFormsModel : PageModel
    {
        public void OnGet()
        {
            var forms = GetForms();
            var bankNames = GetUniqueBankNames();
            ViewData["Forms"] = forms;
            ViewData["BankNames"] = bankNames;
        }
        public IEnumerable<FormInfo> GetForms()
        {
            var connectionString = "mongodb+srv://maramhossama:marmar123@cluster0.qyrbfln.mongodb.net/";
            var mongoClient = new MongoClient(connectionString);
            var database = mongoClient.GetDatabase("ebi");
            var collection = database.GetCollection<FormInfo>("regForm"); // Use BankInfo as the document type

            var filter = Builders<FormInfo>.Filter.Empty; // Use BankInfo for the filter type
            var sortDefinition = Builders<FormInfo>.Sort.Ascending(b => b.FName); // Assuming "Name" is a property in your BankInfo class

            var forms = collection.Find(filter).Sort(sortDefinition).ToEnumerable(); // Map to BankInfo objects

            return forms;
        }
        public class FormInfo
        {
            [BsonId]
            [BsonRepresentation(BsonType.ObjectId)]
            public string Id { get; set; }

            [BsonElement("fullName")]
            public string FName { get; set; }

            [BsonElement("natID")]
            public string NID { get; set; }

            [BsonElement("bank")]
            public string Bank { get; set; }

            [BsonElement("title")]
            public string Title { get; set; }

            [BsonElement("repMang")]
            public string RepM { get; set; }

            [BsonElement("essay")]
            public string Essay { get; set; }

            [BsonElement("referal")]
            public string Referral { get; set; }

            [BsonElement("status")]
            public string FormStatus { get; set; }

            [BsonElement("reason")]
            public string StatusReason { get; set; }

            [BsonElement("date")]
            [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
            public DateTime Date { get; set; }
        }
        public List<string> GetUniqueBankNames()
        {
            var connectionString = "mongodb+srv://maramhossama:marmar123@cluster0.qyrbfln.mongodb.net/";
            var mongoClient = new MongoClient(connectionString);
            var database = mongoClient.GetDatabase("ebi");
            var collection = database.GetCollection<FormInfo>("regForm");

            var distinctBankNames = collection.Distinct<string>("bank", FilterDefinition<FormInfo>.Empty).ToList();

            return distinctBankNames;
        }
    }
}
