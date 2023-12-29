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
    private static List<(DebugType, dynamic)> _itemList = new();

    public static void ClearDebugItems() => _itemList = new();

    public static void QueuDebugItem(DebugType debugType, dynamic args) => _itemList.Add((debugType, args));
    
    public static void DrawDebugItems()
    {
        foreach (var (debugType, args) in _itemList)
        {
            switch (debugType)
            {
                case DebugType.LineV:
                    Raylib.DrawLineV(args.start, args.end, args.color);
                    break;
                case DebugType.Circle:
                    Raylib.DrawCircleV(args.position, args.radius, args.color);
                    break;
                case DebugType.BoundingBox:
                    Raylib.DrawBoundingBox(args.box, args.color);
                    break;
            }
        }
    }
}