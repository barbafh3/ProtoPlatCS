using System.ComponentModel;
using System.Numerics;
using ExtensionMethods;
using ProtoPlat.Interfaces;
using ProtoPlat.Managers;
using Raylib_cs;

namespace ProtoPlat;


public class KinematicBody2D : Entity2D, IUpdate, IFixedUpdate
{
    public Vector2 Velocity = Vector2.Zero;
    public Collider2D Collider;

    protected int _fallCount = 0;

    public bool FacingRight = true;

    public KinematicBody2D(Collider2D collider, string name = "KinematicBody2D") : base(name)
    {
        if (collider == null)
            GameLogger.Log(LogLevel.ERROR,
                "KinematicBody2D requires a Collision2D child. Collisions for this entity will not work.");
        AddChild(collider);
        Collider = collider;
        collider.Parent = this;
    }

    public KinematicBody2D(Collider2D collider, Vector2 position, string name = "KinematicBody2D") : base(name)
    {
        Position = position;
        AddChild(collider);
        Collider = collider;
        collider.Parent = this;
    }

    public override void Start()
    {
        GetChild<Collider2D>().IfNone(() =>
        {
            GameLogger.Log(LogLevel.ERROR, "KinematicBody2D requires a Collision2D child.");
            Environment.Exit(1);
        });
        Name = "KinematicBody2D";
    }

    public virtual void Update(float delta)
    {
        Collider.IsColliding = false;
        
        if(!Collider.CollisionDirections.ContainsValue(CollisionDirection.FromBelow))
            Velocity.Y += Math.Min(1, (_fallCount / Constants.FPS) * Constants.Gravity);
    }

    public virtual void FixedUpdate(float delta)
    {
    }

    protected override void RepositionChildren()
    {
        _fallCount++;
        
        base.RepositionChildren();

        Collider.Rect.X = Position.X + Collider.Offset.X;
        Collider.Rect.Y = Position.Y + Collider.Offset.Y;
    }

    public void MoveBody()
    {
        // Raycast Right
        CheckRayCollision(Collider.Rect.Center(), new(20, 0));
        // Raycast Left
        CheckRayCollision(Collider.Rect.Center(), new(-20, 0));
        // Raycast Bottom
        CheckRayCollision(Collider.Rect.Center(), new(0, 30));

        Position += Velocity;
        RepositionChildren();
        
        EntityManager.CheckCollision(Collider).ForEach(otherCollider =>
        {
            if (CheckCollisionDirection(otherCollider))
            {
                RevertMovement(otherCollider);
                RepositionChildren();
            }
        });
    }

    private void CheckRayCollision(Vector2 origin, Vector2 direction)
    {
        var collisionResults = new List<RayCollision>();
        var distance = origin.Distance(origin + direction);
        EntityManager.GetEntities<Collider2D>().ForEach(collider =>
        {
            if (collider != Collider)
            {
                var ray = new Ray
                {
                    Position = origin.ToVector3(),
                    Direction = direction.ToVector3()
                };
                var result = Raylib.GetRayCollisionBox(ray, collider.Rect.ToBoundingBox());
                collisionResults.Add(result);
            }
        });

        var nearestDistance = float.MaxValue;
        var nearestPoint = Vector2.Zero;
        var nearestNormal = Vector2.Zero;
        foreach (var result in collisionResults)
        {
            var dist = origin.Distance(result.Point.ToVector2());
            if (dist < nearestDistance)
            {
                nearestDistance = dist;
                nearestPoint = result.Point.ToVector2();
                nearestNormal = result.Normal.ToVector2().Normalized();
            }
        }

        if (nearestDistance < distance)
        {
            Raylib.DrawCircleV(nearestPoint, 5, Color.RED);
            var newNormal = new Vector2(15, 15) * nearestNormal;
            var lineEnd = nearestPoint.Plus(newNormal);
            Raylib.DrawLineV(nearestPoint, lineEnd, Color.YELLOW);
        }
    }


    protected bool CheckCollisionDirection(Collider2D otherCollider)
    {
        var bottomCollision = otherCollider.Rect.Bottom() - Collider.Rect.Y;
        var topCollision = Collider.Rect.Bottom() - otherCollider.Rect.Y;
        var leftCollision = Collider.Rect.Right() - otherCollider.Rect.X;
        var rightCollision = otherCollider.Rect.Right() - Collider.Rect.X;

        var notColliding = new List<CollisionDirection>();
        
        if (topCollision < bottomCollision && topCollision < leftCollision && topCollision < rightCollision )
        {                           
            Collider.CollisionDirections.Add(otherCollider, CollisionDirection.FromBelow);
            return true;
        }
        notColliding.Add(CollisionDirection.FromBelow);
        
        if (bottomCollision < topCollision && bottomCollision < leftCollision && bottomCollision < rightCollision)                        
        {
            Collider.CollisionDirections.Add(otherCollider, CollisionDirection.FromAbove);
            return true;
        }
        notColliding.Add(CollisionDirection.FromAbove);
        
        if (leftCollision < rightCollision && leftCollision < topCollision && leftCollision < bottomCollision)
        {
            Collider.CollisionDirections.Add(otherCollider, CollisionDirection.FromRight);
            return true;
        }
        notColliding.Add(CollisionDirection.FromRight);
        
        if (rightCollision < leftCollision && rightCollision < topCollision && rightCollision < bottomCollision )
        {
            Collider.CollisionDirections.Add(otherCollider, CollisionDirection.FromLeft);
            return true;
        }
        notColliding.Add(CollisionDirection.FromLeft);
        
        Collider
            .CollisionDirections
            .Where(kv => kv.Key == otherCollider && notColliding.Contains(kv.Value))
            .ToList()
            .ForEach(kv => Collider.CollisionDirections.Remove(kv.Key));

        return false;
    }

    void RevertMovement(Collider2D otherCollider)
    {
        var direction = Collider.CollisionDirections[otherCollider];
        switch (direction)
        {
            case CollisionDirection.FromLeft:
                Position.X = otherCollider.Parent.Position.X + otherCollider.Rect.Width - Collider.Offset.X;
                Velocity.X = 0f;
                break;
            case CollisionDirection.FromRight:
                Position.X = otherCollider.Parent.Position.X - Collider.Rect.Width - Collider.Offset.X;
                Velocity.X = 0f;
                break;
            case CollisionDirection.FromAbove:
                Position.Y = otherCollider.Parent.Position.Y + Collider.Rect.Height - Collider.Offset.Y;
                Velocity.Y = 0f;
                break;
            case CollisionDirection.FromBelow:
                Position.Y = otherCollider.Parent.Position.Y - Collider.Rect.Height - Collider.Offset.Y;
                Velocity.Y = 0f;
                break;
        }
    }
}
