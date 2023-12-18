using ProtoPlat.Interfaces;
using Raylib_cs;

namespace ProtoPlat;

public class ColorPlatform : Entity2D, IDraw
{
    public int DrawLayer { get; set; } = 0;
    public Rectangle Rect;
    public Color Color;

    public Collision2D Collision;

    public void Draw()
    {
        Raylib.DrawRectangleRec(Rect, Color);
    }
}