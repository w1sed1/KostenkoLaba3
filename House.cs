using System.Text.Json.Serialization;

public class house
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("location")]
    public string Location { get; set; }

    [JsonPropertyName("CountRooms")]
    public int CountRooms { get; set; }

    [JsonPropertyName("Price")]
    public decimal Price { get; set; }

}


