using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace Quiztellation.Data;

public class Star
{
    [BsonElement("bayer")]
    public string Bayer { get; set; }

    [BsonElement("con")]
    public string Con { get; set; }

    [BsonElement("names")]
    public List<string> Names { get; set; }

    [BsonElement("level")]
    public int Level { get; set; }
}

public class StarData
{
    public static Task<List<Star>> GetStarsByLevel(int level = 1, int count = 10)
    {
        var settings = MongoClientSettings.FromConnectionString(Environment.GetEnvironmentVariable("CONNECTION_STRING"));
        settings.ServerApi = new ServerApi(ServerApiVersion.V1);
        var client = new MongoClient(settings);
        var pipeline = new BsonDocument[]
        {
            new BsonDocument("$match",
            new BsonDocument("level",
            new BsonDocument("$eq", level))),
            new BsonDocument("$sample",
            new BsonDocument("size", count)),
            new BsonDocument("$project",
            new BsonDocument("_id", 0))
        };
        var starsCollection = client.GetDatabase("astroDB").GetCollection<BsonDocument>("stars");
        var stars = starsCollection.Aggregate<Star>(pipeline).ToListAsync();
        return stars;
    }

    public static Task<List<Star>> GetStarsByCon(string con)
    {
        var settings = MongoClientSettings.FromConnectionString(Environment.GetEnvironmentVariable("CONNECTION_STRING"));
        settings.ServerApi = new ServerApi(ServerApiVersion.V1);
        var client = new MongoClient(settings);
        var pipeline = new BsonDocument[]
        {
            new BsonDocument("$match",
            new BsonDocument("con",
            new BsonDocument("$eq", con))),
            new BsonDocument("$project",
            new BsonDocument("_id", 0))
        };
        var starsCollection = client.GetDatabase("astroDB").GetCollection<BsonDocument>("stars");
        var stars = starsCollection.Aggregate<Star>(pipeline).ToListAsync();
        return stars;
    }
}