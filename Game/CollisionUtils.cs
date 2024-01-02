using System.Numerics;
using ExtensionMethods;
using Raylib_cs;
using LanguageExt;
using ProtoPlat.Managers;
using static LanguageExt.Prelude;

namespace ProtoPlat;

public enum CollisionDirection
{
    UpRight,
    UpLeft,
    DownRight,
    DownLeft,
    LeftTop,
    LeftBottom,
    RightTop,
    RightBottom
}

public struct CollisionData
{
    public Vector2 ContactPoint;
    public Vector2 ContactNormal;
    public float Distance;
    public Collider2D Other;
}

public static class CollisionUtils
{
    public static Option<CollisionData> Raycast2D(Vector2 start, Vector2 end, List<Collider2D> blacklist = default!)
    {
        var direction = start.Direction(end);
        var distance = start.Distance(end);

        var nearestDistance = float.MaxValue;
        Vector2 nearestPoint = Vector2.Zero;
        Vector2 nearestNormal = Vector2.Zero;
        Option<Collider2D> nearestCollider = None;
        EntityManager.GetEntities<Collider2D>().ForEach(collider =>
        {
            if (blacklist.Contains(collider))
                return;
            
            var ray = new Ray(start.ToVector3(), direction.ToVector3());
            var result = Raylib.GetRayCollisionBox(ray, collider.Box);
            if (result.Hit)
            {
                var dist = result.Point.ToVector2().Distance(start);
                if (dist <= distance && dist < nearestDistance) {
                    nearestDistance = dist;
                    nearestPoint = result.Point.ToVector2();
                    nearestNormal = result.Normal.ToVector2().Normalized();
                    nearestCollider = collider;
                }
            }
        });

        return nearestCollider.Match(Some: other =>
            {
                return new CollisionData()
                {
                    ContactPoint = nearestPoint,
                    ContactNormal = nearestNormal,
                    Distance = nearestDistance,
                    Other = other
                };
            }, 
            None: () => Option<CollisionData>.None);
    }
}