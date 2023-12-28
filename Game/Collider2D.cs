using System.Numerics;
using ExtensionMethods;
using LanguageExt.ClassInstances.Pred;
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
    public Rectangle Rect;
    public Vector2 Offset;
    public bool IsColliding = false;
    public List<string> Layers = new();
    public List<string> LayerMasks = new();
    public Dictionary<Collider2D, CollisionDirection> CollisionDirections = new();

    public Collider2D(Rectangle rect, Vector2 offset, string name = "Collider2D") : base(name)
    {
        Rect = rect;
        Offset = offset;
    }

    public Vector2 RelativePos => ((Entity2D)Parent).Position + Offset;
    
    public void Draw()
    {
        if (GameManager.DrawCollisionEnabled)
            Raylib.DrawRectangleRec(Rect, new Color(255, 0, 0, 170));
    }

    public void Update(float delta)
    {
    }
}