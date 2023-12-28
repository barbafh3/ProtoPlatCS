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
        Collider.CollisionDirections = new();
        
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
        var collided = false;
        Raylib.DrawLineV(Collider.Rect.Center(), Collider.Rect.Center() + new Vector2(50, 0), Color.BLACK);
        EntityManager.GetEntities<Collider2D>().ForEach(collider =>
        {
            if (!collided && collider != Collider)
            {
                // var distance = 50f;
                var origin = Collider.Rect.Center();
                var mousePos = Raylib.GetMousePosition();
                // var dir = new Vector2(50, 0);
                var dir = mousePos - origin;
                var box = new BoundingBox
                {
                    Min = new Vector3(collider.Rect.X, collider.Rect.Y, 0),
                    Max = new Vector3(collider.Rect.Right(), collider.Rect.Bottom(), 0)
                };
                var ray = new Ray
                {
                    Position = origin.ToVector3(),
                    Direction = dir.ToVector3()
                };
                var result = Raylib.GetRayCollisionBox(ray, box);
                Raylib.DrawCircleV(result.Point.ToVector2(), 5, Color.RED);
                collided = true;
            }
        });
        
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


    protected bool CheckCollisionDirection(Collider2D otherCollider)
    {
        var bottomCollision = otherCollider.Rect.Bottom() - Collider.Rect.Y;
        var topCollision = Collider.Rect.Bottom() - otherCollider.Rect.Y;
        var leftCollision = Collider.Rect.Right() - otherCollider.Rect.X;
        var rightCollision = otherCollider.Rect.Right() - Collider.Rect.X;
        
        
        if (topCollision < bottomCollision && topCollision < leftCollision && topCollision < rightCollision )
        {                           
            Collider.CollisionDirections.Add(otherCollider, CollisionDirection.FromBelow);
            return true;
        }
        if (bottomCollision < topCollision && bottomCollision < leftCollision && bottomCollision < rightCollision)                        
        {
            Collider.CollisionDirections.Add(otherCollider, CollisionDirection.FromAbove);
            return true;
        }
        if (leftCollision < rightCollision && leftCollision < topCollision && leftCollision < bottomCollision)
        {
            Collider.CollisionDirections.Add(otherCollider, CollisionDirection.FromRight);
            return true;
        }
        if (rightCollision < leftCollision && rightCollision < topCollision && rightCollision < bottomCollision )
        {
            Collider.CollisionDirections.Add(otherCollider, CollisionDirection.FromLeft);
            return true;
        }

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
