using System.Numerics;
using ProtoPlat.Interfaces;
using ProtoPlat.Managers;

namespace ProtoPlat;

public static class PlayerAnimations
{
    public static readonly Animation2D Idle = new(
        "Idle", 
        "PHCharacterIdle", 
        new(48, 48), 
        0.1f,
        new List<Vector2>
        {
            new(48 * 0, 0),
            new(48 * 1, 0),
            new(48 * 2, 0),
            new(48 * 3, 0),
            new(48 * 4, 0),
            new(48 * 5, 0),
            new(48 * 6, 0),
            new(48 * 7, 0),
            new(48 * 8, 0),
            new(48 * 9, 0)
        });
    public static readonly Animation2D Run = new(
        "Run", 
        "PHCharacterRun", 
        new (48, 48), 
        0.1f,
        new()
        {
            new(48 * 0, 0),
            new(48 * 1, 0),
            new(48 * 2, 0),
            new(48 * 3, 0),
            new(48 * 4, 0),
            new(48 * 5, 0),
            new(48 * 6, 0),
            new(48 * 7, 0)
        });
}

public class Player : AnimatedSprite2D
{
    public Vector2 Direction = Vector2.Zero;
    public float Speed = 1f;
    
    public Collision2D Collision;

    public Player() { }

    public Player(Animation2D animation, bool looping) : base(animation, looping)
    {
    }
    
    public override void Update(float delta)
    {
        base.Update(delta);
        
        if (InputManager.GetInputActionState("MoveLeft") == InputState.Down)
        {
            Direction.X += -1 * Speed * delta;
            Flip.Item1 = true;
        }

        if (InputManager.GetInputActionState("MoveRight") == InputState.Down)
        {
            Direction.X += 1 * Speed * delta;
            Flip.Item1 = false;
        }

        if (!Collision.IsColliding)
            Direction.Y += Constants.Gravity * delta;

        if (Direction.X == 0 && Animation.Name != "Idle")
            ChangeAnimation(PlayerAnimations.Idle);
        if (Direction.X != 0 && Animation.Name != "Run")
            ChangeAnimation(PlayerAnimations.Run);
            

        Position += Direction;
        Direction = Vector2.Zero;
    }

}