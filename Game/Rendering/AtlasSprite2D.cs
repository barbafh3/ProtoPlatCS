using System.Numerics;
using ProtoPlat.Interfaces;
using ProtoPlat.Managers;
using Raylib_cs;

namespace ProtoPlat;

public class AtlasSprite2D : Entity2D, IDraw
{
    public int DrawLayer { get; set; }
    public string AtlasName { get; set; }
    public Rectangle Rect { get; set; }
    public float Scale { get; set; }

    public AtlasSprite2D(string atlasName, Rectangle rect, float scale, int drawLayer, string name = "AtlasSprite2D") : base(name)
    {
        AtlasName = atlasName;
        Rect = rect;
        DrawLayer = drawLayer;
        Scale = scale;
    }

    public void Draw()
    {
        var src = Rect;
        var dest = new Rectangle
        {
            X = Position.X,
            Y = Position.Y,
            Width = Rect.Width,
            Height = Rect.Height
        };
        AssetManager
            .GetTextureAtlas(AtlasName)
            .Match(
                Some: atlas => Raylib.DrawTexturePro(atlas, src, dest, Vector2.Zero, 0f, Color.WHITE), 
                None: () => GameLogger.Log(LogLevel.ERROR, "Failed to get atlas texture"));
    }
}