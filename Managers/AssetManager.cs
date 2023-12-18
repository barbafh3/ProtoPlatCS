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

    public static Option<Texture2D> GetTextureAtlas(string atlasName)
    {
        if (AtlasList.Keys.Contains(atlasName))
            return AtlasList[atlasName];
        return None;
    }
}