using System.Numerics;
using Raylib_cs;

namespace ExtensionMethods;

public static class VectorExtensions
{
    // Vector2 extensions
    public static float Distance(this Vector2 va, Vector2 vb) => (float)Math.Sqrt(Math.Pow(vb.X - va.X, 2) + Math.Pow(vb.Y - va.Y, 2));
    public static Vector2 Direction(this Vector2 va, Vector2 vb) => new(vb.X - va.X, vb.Y - va.Y);
    public static Vector2 Normalized(this Vector2 v) => new(v.X / v.Length(), v.Y / v.Length());
    public static Vector3 ToVector3(this Vector2 v, bool yAsZ = false) => yAsZ ? new(v.X, 0, v.Y) : new(v.X, v.Y, 0);
    
    // Vector3 extensions
    public static Vector2 ToVector2(this Vector3 v, bool zAsY = false) => zAsY ? new(v.X, v.Z) : new(v.X, v.Y);

}

public static class GeneralTypeExtensions
{
    // Float Extensions
    public static Vector2 Divide(this float f, Vector2 v) => new(f / v.X, f / v.Y);
    

    // Rectangle extensions
    public static float Top(this Rectangle rect) => rect.Y;
    public static float Bottom(this Rectangle rect) => rect.Y + rect.Height;
    public static float Left(this Rectangle rect) => rect.X;
    public static float Right(this Rectangle rect) => rect.X + rect.Width;
    public static Vector2 Center(this Rectangle Rect) => new (Rect.X + (Rect.Width / 2), Rect.Y + (Rect.Height / 2));
}