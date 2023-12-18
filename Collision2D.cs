using System.Numerics;
using LanguageExt.ClassInstances.Pred;
using ProtoPlat.Interfaces;
using Raylib_cs;

namespace ProtoPlat;

public enum CollisionDirection
{
    FromAbove,
    FromBelow,
    FromLeft,
    FromRight
}

public class Collision2D : GameEntity, IUpdate, IDraw
{
    public delegate void OnCollisionEventHandler(ref Collision2D other);

    public static event OnCollisionEventHandler OnCollision;

    public int DrawLayer { get; set; } = 99;
    public Rectangle Rect;
    // public List<CollisionDirection> Collisions = new();
    public bool IsColliding = false;
    public List<string> Layers = new();
    public List<string> LayerMasks = new();
    
    public void CheckCollision(Collision2D otherCollider, bool second = false)
    {
        if (second)
        {
            IsColliding = true;
            OnCollision.Invoke(ref otherCollider);
            return;
        }
        
        if (Raylib.CheckCollisionRecs(Rect, otherCollider.Rect))
        {
            IsColliding = true;
            OnCollision.Invoke(ref otherCollider);
            otherCollider.CheckCollision(this, true);
        }
    }

    public void Draw()
    {
        Raylib.DrawRectangleRec(Rect, new Color(170, 170, 170, 170));
    }

    public void Update(float delta)
    {
        Parent.Match(
            Some: parent =>
            {
                Rect.X = (parent as Entity2D).Position.X;
                Rect.Y = (parent as Entity2D).Position.Y;
            },
            None: () =>
            {
                GameLogger.Log(LogLevel.ERROR, "Collision2D entities require an Entity2D as parent.");
                Environment.Exit(1);
            });
        var a = "";
    }
}