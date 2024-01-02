using System.Numerics;
using ExtensionMethods;
using LanguageExt;
using LanguageExt.UnsafeValueAccess;
using static LanguageExt.Prelude;
using ProtoPlat.Interfaces;
using ProtoPlat.Managers;
using Raylib_cs;

namespace ProtoPlat;


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
            VisualDebugger.QueueBoundingBox(
                 Box,
                 CollisionDirections.Count > 0 ? new Color(255, 0, 0, 170) : new Color(0, 0, 255, 170)
                );
        }
    }

    public void Update(float delta)
    {
        CollisionDirections = new();
    }

    public Option<CollisionData> CheckRayCollision(Vector2 start, Vector2 end)
    {
        if (!Enabled)
            return None;
        
        var direction = start.Direction(end);
        var distance = start.Distance(end);

        var nearestDistance = float.MaxValue;
        Vector2 nearestPoint = Vector2.Zero;
        Vector2 nearestNormal = Vector2.Zero;
        Option<Collider2D> nearestCollider = None;
        EntityManager.GetEntities<Collider2D>().ForEach(collider =>
        {
            if (this != collider && collider.Static)
            {
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
            }
        });

        return nearestCollider.Match(Some: other =>
            new CollisionData()
            {
                ContactPoint = nearestPoint,
                ContactNormal = nearestNormal,
                Distance = nearestDistance,
                Other = other 
            }, 
            None: () => Option<CollisionData>.None);

    }
    
    protected bool CheckCollisionDirection(Collider2D otherCollider)
    {
        var bottomCollision = otherCollider.Box.Max.Y - Box.Min.Y;
        var topCollision = Box.Max.Y - otherCollider.Box.Min.Y;
        var leftCollision = Box.Max.X - otherCollider.Box.Min.X;
        var rightCollision = otherCollider.Box.Max.X - Box.Min.X;

        var notColliding = new List<CollisionDirection>();
        
        if (topCollision < bottomCollision && topCollision < leftCollision && topCollision < rightCollision )
        {                           
            CollisionDirections.TryAdd(otherCollider, CollisionDirection.DownLeft);
            return true;
        }
        notColliding.Add(CollisionDirection.DownLeft);
        
        if (bottomCollision < topCollision && bottomCollision < leftCollision && bottomCollision < rightCollision)                        
        {
            CollisionDirections.TryAdd(otherCollider, CollisionDirection.UpRight);
            return true;
        }
        notColliding.Add(CollisionDirection.UpRight);
        
        if (leftCollision < rightCollision && leftCollision < topCollision && leftCollision < bottomCollision)
        {
            CollisionDirections.TryAdd(otherCollider, CollisionDirection.RightTop);
            return true;
        }
        notColliding.Add(CollisionDirection.RightTop);
        
        if (rightCollision < leftCollision && rightCollision < topCollision && rightCollision < bottomCollision )
        {
            CollisionDirections.TryAdd(otherCollider, CollisionDirection.LeftTop);
            return true;
        }
        notColliding.Add(CollisionDirection.LeftTop);
        
        CollisionDirections
            .Where(kv => kv.Key == otherCollider && notColliding.Contains(kv.Value))
            .ToList()
            .ForEach(kv => CollisionDirections.Remove(kv.Key));

        return false;
    }
}