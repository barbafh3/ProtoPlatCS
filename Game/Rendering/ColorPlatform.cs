using ProtoPlat.Interfaces;
using Raylib_cs;

namespace ProtoPlat;

public class ColorPlatform : Entity2D, IDraw
{
    public int DrawLayer { get; set; } = 5;
    public Rectangle Rect;
    public Color Color;

    public Collider2D? Collider;

    public void Draw()
    {
        Raylib.DrawRectangleRec(Rect, Color);
    }
}