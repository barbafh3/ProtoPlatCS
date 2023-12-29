using System.Numerics;
using ExtensionMethods;
using ProtoPlat.Interfaces;
using ProtoPlat.Managers;
using Raylib_cs;
using LanguageExt;
using LanguageExt.UnsafeValueAccess;
using static LanguageExt.Prelude;

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

        var colliderSize = Collider.Box.Max - Collider.Box.Min;
        Collider.Box.Min.X = Position.X + Collider.Offset.X;
        Collider.Box.Min.Y = Position.Y + Collider.Offset.Y;
        Collider.Box.Max = Collider.Box.Min + colliderSize;
    }

    public void MoveBody()
    {
        Position += Velocity;
        RepositionChildren();
        
        // Raycast Right
        Collider.CheckRayCollision(Collider.Box.Center().ToVector2(), new(20, 0))
            .IfSome(collider =>
            {
                Collider.CollisionDirections.TryAdd(collider, CollisionDirection.FromRight);
                RevertMovement(collider);
                RepositionChildren();
            });
        // Raycast Left
        Collider.CheckRayCollision(Collider.Box.Center().ToVector2(), new(-20, 0))
            .IfSome(collider =>
            {
                Collider.CollisionDirections.TryAdd(collider, CollisionDirection.FromLeft);
                RevertMovement(collider);
                RepositionChildren();
            });
        // Raycast Bottom
        Collider.CheckRayCollision(Collider.Box.Center().ToVector2(), new(0, 30))
            .IfSome(collider =>
            {
                Collider.CollisionDirections.TryAdd(collider, CollisionDirection.FromBelow);
                RevertMovement(collider);
                RepositionChildren();
            });
        // Raycast Top
        Collider.CheckRayCollision(Collider.Box.Center().ToVector2(), new(0, -30))
            .IfSome(collider =>
            {
                GameLogger.Log(LogLevel.INFO, $"Collided with {collider.Parent.Name}");     
                Collider.CollisionDirections.TryAdd(collider, CollisionDirection.FromAbove);
                RevertMovement(collider);
                RepositionChildren();
            });
        
        GameLogger.Log(LogLevel.INFO, "End move");
        GameLogger.Log(LogLevel.INFO, "--- move");
        // EntityManager.GetEntities<Collider2D>().ForEach(collider =>
        // {
        //     if (Collider.CheckCollision(collider))
        //     {
        //         if (CheckCollisionDirection(collider))
        //         {
        //             RevertMovement(collider);
        //             RepositionChildren();
        //         }
        //     }
        // });
    }


    protected bool CheckCollisionDirection(Collider2D otherCollider)
    {
        var bottomCollision = otherCollider.Box.Max.Y - Collider.Box.Min.Y;
        var topCollision = Collider.Box.Max.Y - otherCollider.Box.Min.Y;
        var leftCollision = Collider.Box.Max.X - otherCollider.Box.Min.X;
        var rightCollision = otherCollider.Box.Max.X - Collider.Box.Min.X;

        var notColliding = new List<CollisionDirection>();
        
        if (topCollision < bottomCollision && topCollision < leftCollision && topCollision < rightCollision )
        {                           
            Collider.CollisionDirections.TryAdd(otherCollider, CollisionDirection.FromBelow);
            return true;
        }
        notColliding.Add(CollisionDirection.FromBelow);
        
        if (bottomCollision < topCollision && bottomCollision < leftCollision && bottomCollision < rightCollision)                        
        {
            Collider.CollisionDirections.TryAdd(otherCollider, CollisionDirection.FromAbove);
            return true;
        }
        notColliding.Add(CollisionDirection.FromAbove);
        
        if (leftCollision < rightCollision && leftCollision < topCollision && leftCollision < bottomCollision)
        {
            Collider.CollisionDirections.TryAdd(otherCollider, CollisionDirection.FromRight);
            return true;
        }
        notColliding.Add(CollisionDirection.FromRight);
        
        if (rightCollision < leftCollision && rightCollision < topCollision && rightCollision < bottomCollision )
        {
            Collider.CollisionDirections.TryAdd(otherCollider, CollisionDirection.FromLeft);
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
        Vector2 newPos;
        switch (direction)
        {
            case CollisionDirection.FromLeft:
                newPos.X = otherCollider.Box.Min.X + otherCollider.Box.Size().X - Collider.Offset.X;
                Position.X = newPos.X;
                Velocity.X = 0f;
                break;
            case CollisionDirection.FromRight:
                newPos.X = otherCollider.Box.Min.X - Collider.Box.Size().X - Collider.Offset.X;
                Position.X = newPos.X;
                Velocity.X = 0f;
                break;
            case CollisionDirection.FromAbove:
                newPos.Y = otherCollider.Box.Min.Y + Collider.Box.Size().Y - Collider.Offset.Y;
                Position.Y = newPos.Y;
                Velocity.Y = 0f;
                break;
            case CollisionDirection.FromBelow:
                newPos.Y = otherCollider.Box.Min.Y - Collider.Box.Size().Y - Collider.Offset.Y;
                Position.Y = newPos.Y;
                Velocity.Y = 0f;
                break;
        }
    }
}
