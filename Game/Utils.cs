using LanguageExt;
using Newtonsoft.Json;
using static LanguageExt.Prelude;

namespace ProtoPlat;

public static class Utils
{
    public static JsonTileMap LoadTileMapJson(string path)
    {
        try
        {
            var jsonString = File.ReadAllText(path);
            var jsonData = JsonConvert.DeserializeObject<JsonTileMap>(jsonString);
            if (jsonData == null)
                throw new Exception("Tile map json conversion failed");
            return jsonData;
        }
        catch (Exception e)
        {
            GameLogger.Log(LogLevel.ERROR, $"{e.Message}");
            Environment.Exit(1);
            return null;
        }
    }
}