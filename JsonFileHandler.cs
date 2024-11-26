using System.Text.Json;

public class JsonFileHandler
{
    public static List<house> LoadHouses(string filePath)
    {
        try
        {
            if (!File.Exists(filePath))
            {
                return new List<house>(); // If the file doesn't exist, return an empty list.
            }

            string jsonContent = File.ReadAllText(filePath); // Read the file's content.
            return JsonSerializer.Deserialize<List<house>>(jsonContent) ?? new List<house>(); // Deserialize JSON into a list of houses.
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error while reading the file: {ex.Message}");
            return new List<house>();
        }
    }

    public static void SaveHouses(string filePath, List<house> Houses)
    {
        try
        {
            var jsonString = JsonSerializer.Serialize(Houses, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(filePath, jsonString); // Save the serialized JSON to the file.
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error while saving the file: {ex.Message}");
        }
    }

    public static List<house> DeserializeHouses(string jsonContent)
    {
        try
        {
            return JsonSerializer.Deserialize<List<house>>(jsonContent); // Deserialize JSON string into a list of houses.
        }
        catch
        {
            return null; // Return null if deserialization fails.
        }
    }

    public static bool IsValidhouseData(List<house> Houses)
    {
        // Check if the list is not null and all houses have valid data (ID > 0 and a non-empty name).
        return Houses != null && Houses.All(h => h.Id > 0 && !string.IsNullOrWhiteSpace(h.Name));
    }

    public static void CopyFile(string sourceFilePath, string destinationFilePath)
    {
        try
        {
            File.Copy(sourceFilePath, destinationFilePath, true); // Copy the file to the destination.
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error while copying the file: {ex.Message}");
        }
    }
}
