using System.Numerics;
using ProtoPlat.Interfaces;
using ProtoPlat.Managers;
using Raylib_cs;

namespace ProtoPlat;

public class Sprite2D : Entity2D, IDraw
{
    public int DrawLayer { get; set; }
    public Rectangle AtlasRect { get; set; }
    public string AtlasName { get; set; } = null!;

    public void Draw()
    {
        // GameLogger.Log(LogLevel.INFO, Position.ToString());
        AssetManager
            .GetTextureAtlas(AtlasName)
            .Match(
                Some: atlas => Raylib.DrawTextureRec(atlas, AtlasRect, Position, Color.WHITE), 
                None: () => Console.WriteLine("Failed to get atlas texture"));
    }
}