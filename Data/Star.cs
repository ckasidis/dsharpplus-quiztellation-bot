using System.Text.Json.Serialization;

namespace Quiztellation.Data;

public class Stars {
    public List<Star> records { get; set; }
}
public class Star
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("createdTime")]
    public string CreatedTime { get; set; }

    [JsonPropertyName("fields")]
    public StarFields Fields { get; set; }
}
public class StarFields
{
    [JsonPropertyName("bayer")]
    public string Bayer { get; set; }

    [JsonPropertyName("con")]
    public string Con { get; set; }

    [JsonPropertyName("names")]
    public string Names { get; set; }

    [JsonPropertyName("level")]
    public int Level { get; set; }
}