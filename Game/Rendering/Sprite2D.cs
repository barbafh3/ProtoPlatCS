using System.Numerics;
using ProtoPlat.Interfaces;
using ProtoPlat.Managers;
using Raylib_cs;

namespace ProtoPlat;

public class Sprite2D : Entity2D, IDraw
{
    public int DrawLayer { get; set; }
    public string AtlasName { get; set; }
    public float Scale { get; set; }

    public Sprite2D(string atlasName, float scale, int drawLayer, string name = "Sprite2D") : base(name)
    {
        AtlasName = atlasName;
        DrawLayer = drawLayer;
        Scale = scale;
    }

    public void Draw()
    {
        AssetManager
            .GetTextureAtlas(AtlasName)
            .Match(
                Some: atlas => Raylib.DrawTextureEx(atlas, Position, 0f, Scale, Color.WHITE), 
                None: () => GameLogger.Log(LogLevel.ERROR, "Failed to get atlas texture"));
    }
}