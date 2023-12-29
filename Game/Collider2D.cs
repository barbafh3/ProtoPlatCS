using System.Numerics;
using ExtensionMethods;
using LanguageExt;
using LanguageExt.UnsafeValueAccess;
using static LanguageExt.Prelude;
using ProtoPlat.Interfaces;
using ProtoPlat.Managers;
using Raylib_cs;

namespace ProtoPlat;

public enum CollisionDirection
{
    FromAbove,
    FromBelow,
    FromLeft,
    FromRight
}

public class Collider2D : GameEntity, IUpdate, IDraw
{
    public new string Name = "Collider2D";
    
    public delegate void OnCollisionEventHandler(Collider2D other);

    public event OnCollisionEventHandler OnCollision;
    
    public new Entity2D Parent;

    public bool Enabled = true;
    public int DrawLayer { get; set; } = 99;
    public BoundingBox Box;
    public Vector2 Offset;
    public List<string> Layers = new();
    public List<string> LayerMasks = new();
    public bool Static = false;
    public Dictionary<Collider2D, CollisionDirection> CollisionDirections = new();

    public Collider2D(BoundingBox box, Vector2 offset, string name = "Collider2D") : base(name)
    {
        Box = box;
        Offset = offset;
    }

    public void Draw()
    {
        if (GameManager.DrawCollisionEnabled)
        {
            Raylib.DrawBoundingBox(
                Box,
                CollisionDirections.Count > 0 ? new Color(255, 0, 0, 170) : new Color(0, 0, 255, 170)
                );
        }
    }

    public void Update(float delta)
    {
        CollisionDirections = new();
    }

    public void AddDirection(Collider2D collider, CollisionDirection direction)
    {
        if (Static)
            GameLogger.Log(LogLevel.INFO, "Changing collision on static");
        CollisionDirections.Add(collider, direction);
    }

    public Option<Collider2D> CheckRayCollision(Vector2 origin, Vector2 direction)
    {
        if (!Enabled)
            return None;
        
        var end = origin + direction;
        var distance = origin.Distance(end);

        var nearestDistance = float.MaxValue;
        Option<Vector2> nearestPoint = None;
        Option<Vector2> nearestNormal = None;
        Option<Collider2D> nearestCollider = None;
        // var nearestNormal = Vector2.Zero;
        EntityManager.GetEntities<Collider2D>().ForEach(collider =>
        {
            if (this != collider && collider.Static)
            {
                var ray = new Ray(origin.ToVector3(), direction.ToVector3());
                var result = Raylib.GetRayCollisionBox(ray, collider.Box);
                if (result.Hit)
                {
                    var dist = result.Point.ToVector2().Distance(origin);
                    if (dist <= distance && dist < nearestDistance) {
                        nearestDistance = dist;
                        nearestPoint = result.Point.ToVector2();
                        nearestNormal = result.Normal.ToVector2().Normalized();
                        nearestCollider = collider;
                    }
                }
            }
        });
        
        nearestCollider.IfSome(col => col.AddDirection(this, CollisionDirection.FromAbove));

        if (GameManager.DrawCollisionEnabled)
        {
            nearestPoint.IfSome(
                point =>
                {
                    Raylib.DrawCircleV(point, 3, Color.RED);
                    nearestNormal.IfSome(
                        normal =>
                        {
                            var endPos = point + (new Vector2(15, 15) * normal);
                            Raylib.DrawLineV(point, endPos, Color.YELLOW);
                        });
                });
        }

        return nearestCollider;
    }
}