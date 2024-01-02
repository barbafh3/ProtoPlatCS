using System.Reflection;
using LanguageExt;
using Tomlyn;
using Tomlyn.Model;

namespace ProtoPlat.Managers;

public struct WindowConfigData
{
    public string Title;
    public int Width;
    public int Height;
    public int TargetFPS;
}

public static class GameManager
{
    private static HashMap<string, object> _config;

    public static bool DrawCollisionEnabled = true;
    
    public static WindowConfigData WindowConfig { get; private set; }
    
    public static Task LoadGameConfig()
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
        var winConfig = (TomlTable)_config["window"];
        WindowConfig = new WindowConfigData()
        {
            Title = (string)winConfig["Title"],
            Width = Int32.Parse(winConfig["Width"].ToString()),
            Height = Int32.Parse(winConfig["Height"].ToString()),
            TargetFPS = Int32.Parse(winConfig["TargetFPS"].ToString()),
            
        };
        InputManager.LoadInputConfig((TomlTable)_config["input"]);

        return Task.CompletedTask;
    }

}