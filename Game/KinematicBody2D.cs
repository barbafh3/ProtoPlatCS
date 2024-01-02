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

    protected int _fallCount = 100;
    protected Vector2 RaycastRange => (Collider.Box.Size().ToVector2() / 2).Minus(1);
    protected float RaycastOffset = 5f;

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
        if(!Collider.CollisionDirections.ContainsValue(CollisionDirection.DownLeft))
            Velocity.Y += Math.Min(1, (_fallCount / GameManager.WindowConfig.TargetFPS) * Constants.Gravity);
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

        // var oldBox = Collider.Box;
        Position += Velocity;
        RepositionChildren();
        
        CheckCollision(); 
        
        // // Raycast Right
        // Collider.CheckRayCollision(Collider.Box.Center().ToVector2(), Collider.Box.Center().ToVector2() + new Vector2(20, 0))
        //     .IfSome(data =>
        //     {
        //         Collider.CollisionDirections.TryAdd(data.Other, CollisionDirection.FromRight);
        //         RevertMovement(data.Other);
        //         RepositionChildren();
        //     });
        // // Raycast Left
        // Collider.CheckRayCollision(Collider.Box.Center().ToVector2(), Collider.Box.Center().ToVector2() + new Vector2(-20, 0))
        //     .IfSome(data =>
        //     {
        //         Collider.CollisionDirections.TryAdd(data.Other, CollisionDirection.FromLeft);
        //         RevertMovement(data.Other);
        //         RepositionChildren();
        //     });
        // // Raycast Bottom
        // Collider.CheckRayCollision(Collider.Box.Center().ToVector2(), Collider.Box.Center().ToVector2() + new Vector2(0, 30))
        //     .IfSome(data =>
        //     {
        //         Collider.CollisionDirections.TryAdd(data.Other, CollisionDirection.FromBelow);
        //         RevertMovement(data.Other);
        //         RepositionChildren();
        //     });
        // // Raycast Top
        // Collider.CheckRayCollision(Collider.Box.Center().ToVector2(), Collider.Box.Center().ToVector2() + new Vector2(0, -30))
        //     .IfSome(data =>
        //     {
        //         Collider.CollisionDirections.TryAdd(data.Other, CollisionDirection.FromAbove);
        //         RevertMovement(data.Other);
        //         RepositionChildren();
        //     });
        
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

    protected void CheckCollision()
    {
        // BOTTOM SIDE COLLISION
        var downLeftOrigin = new Vector2(Collider.Box.Min.X + RaycastOffset, Collider.Box.Center().Y);
        var downLeftDest = downLeftOrigin + new Vector2(0, RaycastRange.Y);
        var downLeftRaycast = CollisionUtils.Raycast2D(downLeftOrigin, downLeftDest, new(){Collider});
        var downRightOrigin = new Vector2(Collider.Box.Max.X - RaycastOffset, Collider.Box.Center().Y);
        var downRightDest = downRightOrigin + new Vector2(0, RaycastRange.Y);
        var downRightRaycast = CollisionUtils.Raycast2D(downRightOrigin, downRightDest, new(){Collider});
        downLeftRaycast.Match(
            Some: collision =>
            {
                Collider.CollisionDirections.TryAdd(collision.Other, CollisionDirection.DownLeft);
                RevertMovement(collision.Other, CollisionDirection.DownLeft);
                VisualDebugger.QueueCircleV(collision.ContactPoint, 3, Color.RED);
                VisualDebugger.QueueLineV(downLeftOrigin, collision.ContactPoint, Color.YELLOW);
            },
            None: () => downRightRaycast.IfSome(
                collision =>
                {
                    Collider.CollisionDirections.TryAdd(collision.Other, CollisionDirection.DownRight);
                    RevertMovement(collision.Other, CollisionDirection.DownRight);
                    VisualDebugger.QueueCircleV(collision.ContactPoint, 3, Color.RED);
                    VisualDebugger.QueueLineV(downRightOrigin, collision.ContactPoint, Color.YELLOW);
                }));
        
        // TOP SIDE COLLISION
        var upLeftOrigin = new Vector2(Collider.Box.Min.X + RaycastOffset, Collider.Box.Center().Y);
        var upLeftDest = upLeftOrigin + new Vector2(0, -RaycastRange.Y);
        var upLeftRaycast = CollisionUtils.Raycast2D(upLeftOrigin, upLeftDest, new(){Collider});
        var upRightOrigin = new Vector2(Collider.Box.Max.X - RaycastOffset, Collider.Box.Center().Y);
        var upRightDest = upRightOrigin + new Vector2(0, -RaycastRange.Y);
        var upRightRaycast = CollisionUtils.Raycast2D(upRightOrigin, upRightDest, new(){Collider});
        upLeftRaycast.Match(
            Some: collision =>
            {
                Collider.CollisionDirections.TryAdd(collision.Other, CollisionDirection.UpLeft);
                RevertMovement(collision.Other, CollisionDirection.UpLeft);
                VisualDebugger.QueueCircleV(collision.ContactPoint, 3, Color.RED);
                VisualDebugger.QueueLineV(upLeftOrigin, collision.ContactPoint, Color.YELLOW);
            },
            None: () => upRightRaycast.IfSome(
                collision =>
                {
                    Collider.CollisionDirections.TryAdd(collision.Other, CollisionDirection.UpRight);
                    RevertMovement(collision.Other, CollisionDirection.UpRight);
                    VisualDebugger.QueueCircleV(collision.ContactPoint, 3, Color.RED);
                    VisualDebugger.QueueLineV(upRightOrigin, collision.ContactPoint, Color.YELLOW);
                }));
        
        
        // RIGHT SIDE COLLISION
        var topRightOrigin = new Vector2(Collider.Box.Center().X, Collider.Box.Min.Y + RaycastOffset);
        var topRightDest = topRightOrigin + new Vector2(RaycastRange.X, 0);
        var topRightRaycast = CollisionUtils.Raycast2D(topRightOrigin, topRightDest, new(){Collider});
        var botRightOrigin = new Vector2(Collider.Box.Center().X, Collider.Box.Max.Y - RaycastOffset);
        var botRightDest = botRightOrigin + new Vector2(RaycastRange.X, 0);
        var botRightRaycast = CollisionUtils.Raycast2D(botRightOrigin, botRightDest, new(){Collider});
        topRightRaycast.Match(
            Some: collision =>
            {
                Collider.CollisionDirections.TryAdd(collision.Other, CollisionDirection.RightTop);
                RevertMovement(collision.Other, CollisionDirection.RightTop);
                VisualDebugger.QueueCircleV(collision.ContactPoint, 3, Color.RED);
                VisualDebugger.QueueLineV(topRightOrigin, collision.ContactPoint, Color.YELLOW);
            },
            None: () => botRightRaycast.IfSome(
                collision =>
                {
                    Collider.CollisionDirections.TryAdd(collision.Other, CollisionDirection.RightBottom);
                    RevertMovement(collision.Other, CollisionDirection.RightBottom);
                    VisualDebugger.QueueCircleV(collision.ContactPoint, 3, Color.RED);
                    VisualDebugger.QueueLineV(botRightOrigin, collision.ContactPoint, Color.YELLOW);
                }));
        
        // LEFT SIDE COLLISION
        var topLeftOrigin = new Vector2(Collider.Box.Center().X, Collider.Box.Min.Y + RaycastOffset);
        var topLeftDest = topLeftOrigin + new Vector2(-RaycastRange.X, 0);
        var topLeftRaycast = CollisionUtils.Raycast2D(topLeftOrigin, topLeftDest, new(){Collider});
        var botLeftOrigin = new Vector2(Collider.Box.Center().X, Collider.Box.Max.Y - RaycastOffset);
        var botLeftDest = botLeftOrigin + new Vector2(-RaycastRange.X, 0);
        var botLeftRaycast = CollisionUtils.Raycast2D(botLeftOrigin, botLeftDest, new(){Collider});
        topLeftRaycast.Match(
            Some: collision =>
            {
                Collider.CollisionDirections.TryAdd(collision.Other, CollisionDirection.LeftTop);
                RevertMovement(collision.Other, CollisionDirection.LeftTop);
                VisualDebugger.QueueCircleV(collision.ContactPoint, 3, Color.RED);
                VisualDebugger.QueueLineV(topLeftOrigin, collision.ContactPoint, Color.YELLOW);
            },
            None: () => botLeftRaycast.IfSome(
                collision =>
                {
                    Collider.CollisionDirections.TryAdd(collision.Other, CollisionDirection.LeftBottom);
                    RevertMovement(collision.Other, CollisionDirection.LeftBottom);
                    VisualDebugger.QueueCircleV(collision.ContactPoint, 3, Color.RED);
                    VisualDebugger.QueueLineV(botLeftOrigin, collision.ContactPoint, Color.YELLOW);
                }));
    }

    void RevertMovement(Collider2D otherCollider, CollisionDirection direction)
    {
        // var direction = Collider.CollisionDirections[otherCollider];
        Vector2 newPos;
        switch (direction)
        {
            case CollisionDirection.LeftTop: 
            case CollisionDirection.LeftBottom:
                newPos.X = otherCollider.Box.Min.X + otherCollider.Box.Size().X + Collider.Offset.X;
                Position.X = newPos.X;
                Velocity.X = 0f;
                break;
            case CollisionDirection.RightTop: 
            case CollisionDirection.RightBottom:
                newPos.X = otherCollider.Box.Min.X - Collider.Box.Size().X - Collider.Offset.X;
                Position.X = newPos.X;
                Velocity.X = 0f;
                break;
            case CollisionDirection.UpRight: 
            case CollisionDirection.UpLeft:
                newPos.Y = otherCollider.Box.Min.Y + Collider.Box.Size().Y + Collider.Offset.Y;
                Position.Y = newPos.Y;
                Velocity.Y = 0f;
                break;
            case CollisionDirection.DownLeft: 
            case CollisionDirection.DownRight:
                newPos.Y = otherCollider.Box.Min.Y - Collider.Box.Size().Y - Collider.Offset.Y;
                Position.Y = newPos.Y;
                Velocity.Y = 0f;
                break;
        }
        
        RepositionChildren();
    }
}
