using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using System;
using System.Collections.Generic;

namespace MongoDBDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            MongoCRUD db = new MongoCRUD("AddressBook");

            //PersonModel person = new PersonModel
            //{
            //    FirstName = "Kaustubh",
            //    LastName = "Patil"
            //,
            //    PrimaryAddress = new AddressModel
            //    {
            //        City = "Bangalore",
            //        StreetAddress = "roop"
            //    }
            //};

            //db.InsertRecord("Users", person);

            //var recs = db.LoadRecords<PersonModel>("Users");
            var rec = db.LoadRecodById<PersonModel>("Users", new Guid("315d2ede-4853-48bd-8648-6ec38717e513"));

            Console.WriteLine($"{rec.Id}:{rec.FirstName}:{rec.LastName} ");
            if (rec.PrimaryAddress != null)
            {
                Console.WriteLine(rec.PrimaryAddress.City);
            }
            rec.DateOfBirth = new DateTime(1982, 4, 6, 0, 0, 0, DateTimeKind.Utc);
            db.UpsertRecords<PersonModel>("Users", new Guid("315d2ede-4853-48bd-8648-6ec38717e513"), rec);

            Console.WriteLine($"{rec.Id}:{rec.FirstName}:{rec.LastName} ");
            if (rec.DateOfBirth != null)
            {
                Console.WriteLine(rec.DateOfBirth);
            }

            db.DeleteRecord<PersonModel>("Users", rec.Id);
            //foreach (var rec in oneRec)
            //{
            //    Console.WriteLine($"{rec.Id}:{rec.FirstName}:{rec.LastName} ");
            //    if(rec.PrimaryAddress != null)
            //    {
            //        Console.WriteLine(rec.PrimaryAddress.City);
            //    }
            //}
            Console.ReadLine();
        }
    }

    public class PersonModel
    {
        [BsonId]
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public AddressModel PrimaryAddress { get; set; }
        [BsonElement("dob")]
        public DateTime DateOfBirth { get; set; }
    }

    public class AddressModel
    {
        public string StreetAddress { get; set; }
        public string City { get; set; }
    }

    public class MongoCRUD
    {
        private IMongoDatabase db;
        public MongoCRUD(string database)
        {
            var client = new MongoClient();
            db = client.GetDatabase(database);
        }
        public void InsertRecord<T>(string table, T record)
        {
            var collection = db.GetCollection<T>(table);
            collection.InsertOne(record);
            
        }
        public List<T> LoadRecords<T>(string table)
        {
            var collection = db.GetCollection<T>(table);
            return collection.Find(new BsonDocument()).ToList();
        }

        public T LoadRecodById<T>(string table, Guid id)
        {
            var collection = db.GetCollection<T>(table);
            var filter = Builders<T>.Filter.Eq("_id", id);
            return collection.Find(filter).First();
        }

        public void UpsertRecords<T>(string table, Guid id, T record)
        {
            var collection = db.GetCollection<T>(table);
            var result = collection.ReplaceOne(new BsonDocument("_id", id), record, new ReplaceOptions { IsUpsert = true });
        }

        public void DeleteRecord<T>(string table, Guid id)
        {
            var collection = db.GetCollection<T>(table);
            var filter = Builders<T>.Filter.Eq("_id", id);
            collection.DeleteOne(filter);
        }

    }
}
