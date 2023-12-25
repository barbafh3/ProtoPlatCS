using System.Reflection;
using LanguageExt;
using Tomlyn;
using Tomlyn.Model;

namespace ProtoPlat.Managers;

public static class GameManager
{
    private static HashMap<string, object> _config;

    public static bool DrawCollisionEnabled = false;
    
    public static TomlTable WindowConfig { get; private set; }
    
    public static void LoadGameConfig()
    {
        var toml = "";
        try
        {
            var assembly = Assembly.GetExecutingAssembly();

            var stream = assembly.GetManifestResourceStream("ProtoPlat.Project.toml");
            var reader = new StreamReader(stream);
            toml = reader.ReadToEnd();
        }
        catch (Exception e)
        {
            GameLogger.Log(LogLevel.ERROR, e.Message);
            Environment.Exit(1);
        }

        _config = Toml.ToModel(toml).ToHashMap();
        WindowConfig = (TomlTable)_config["window"];
        InputManager.LoadInputConfig((TomlTable)_config["input"]);
    }

}