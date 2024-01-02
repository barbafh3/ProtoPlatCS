using System.Numerics;
using ProtoPlat.Interfaces;
using Raylib_cs;

namespace ProtoPlat;

public enum DebugType
{
    LineV,
    Circle,
    BoundingBox,
}

public static class VisualDebugger
{
    private static List<(Vector2, Vector2, Color)> _lineVQueue = new();
    private static List<(Vector2, float, Color)> _circleVList = new();
    private static List<(BoundingBox, Color)> _boundingBoxList = new();

    public static void ClearDebugItems()
    {
        _lineVQueue = new();
        _circleVList = new();
        _boundingBoxList = new();
    }


    public static void QueueLineV(Vector2 start, Vector2 end, Color color) => _lineVQueue.Add((start, end, color));
    public static void QueueCircleV(Vector2 position, float radius, Color color) => _circleVList.Add((position, radius, color));
    public static void QueueBoundingBox(BoundingBox box, Color color) => _boundingBoxList.Add((box, color));
    
    public static void DrawDebugItems()
    {
        foreach (var (box, color) in _boundingBoxList)
        {
            Raylib.DrawBoundingBox(box, color);
        }
        foreach (var (position, radius, color) in _circleVList)
        {
            Raylib.DrawCircleV(position, radius, color);
        }
        foreach (var (start, end, color) in _lineVQueue)
        {
            Raylib.DrawLineV(start, end, color);
        }
    }
}