using MongoDB.Bson.Serialization.Attributes;

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