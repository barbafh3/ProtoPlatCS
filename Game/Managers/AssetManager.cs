using LanguageExt;
using Raylib_cs;
using static LanguageExt.Prelude;

namespace ProtoPlat.Managers;

// public class AssetManager : Singleton<AssetManager>
public static class AssetManager
{
    private static readonly Dictionary<string, Texture2D> AtlasList = new();

    public static void LoadAtlas(string atlasName, string atlasPath)
    {
        if (!File.Exists(atlasPath))
        {
            GameLogger.Log(LogLevel.ERROR, $"Texture file could not be found on path '{atlasPath}'");
            Environment.Exit(1);
        }

        var atlas = Raylib.LoadTexture(atlasPath);
        AtlasList.Add(atlasName, atlas);
    }
    
    public static void LoadAtlasList(Dictionary<string, string> atlasList)
    {
        foreach (var (name, path) in atlasList)
        {
            if (!File.Exists(path))
            {
                GameLogger.Log(LogLevel.ERROR, $"Texture file could not be found on path '{path}'");
                Environment.Exit(1);
            }

            var atlas = Raylib.LoadTexture(path);
            AtlasList.Add(name, atlas);
        }
    }
    
    public static void LoadAtlasList(Dictionary<string, Texture2D> atlasList)
    {
        foreach (var (name, texture) in atlasList)
        {
            GameLogger.Log(LogLevel.INFO, $"Loading texture {name}");
            AtlasList.Add(name, texture);
        }
    }

    public static Option<Texture2D> GetTextureAtlas(string atlasName)
    {
        if (AtlasList.Keys.Contains(atlasName))
            return AtlasList[atlasName];
        return None;
    }
}