using System.Numerics;
using ProtoPlat.Managers;
using LanguageExt;
using LanguageExt.UnsafeValueAccess;

namespace ProtoPlat;


public class Player : KinematicBody2D
{
    public float Speed;
    
    public AnimatedSprite2D AnimatedSprite;

    public float JumpForce { get; private set; } = -40f;

    public Player(Collider2D collider) : base(collider) { }

    public Player(Collider2D collider, AnimatedSprite2D animatedSprite, float speed, Vector2 position) : base(collider, position)
    {
        Speed = speed;
        Collider = collider;
        Collider.Parent = this;
        AddChild(collider);
        AnimatedSprite = animatedSprite;
        AnimatedSprite.Parent = this;
        AddChild(animatedSprite);
    }

    public override void Update(float delta)
    {
        base.Update(delta);
        Velocity.X = 0f;

        if (InputManager.GetInputActionState("MoveLeft") == InputState.Down)
        {
            Velocity.X += -1 * Speed * delta;
            AnimatedSprite.Flip.Item1 = true;
            FacingRight = false;
        }

        if (InputManager.GetInputActionState("MoveRight") == InputState.Down)
        {
            Velocity.X += Speed * delta;
            AnimatedSprite.Flip.Item1 = false;
            FacingRight = true;
        }

        if (InputManager.GetInputActionState("Jump") == InputState.Pressed)
            Velocity.Y = Constants.Gravity * JumpForce;

        MoveBody();
        
        if (Velocity.Y > 0 
            && AnimatedSprite.Animation.Name != "Fall" 
            && !Collider.CollisionDirections.Values.Contains(CollisionDirection.DownLeft))
            AnimatedSprite.ChangeAnimation(PlayerAnimations.Fall);
        else if (Velocity.Y < 0 
                 && AnimatedSprite.Animation.Name != "Jump" 
                 && AnimatedSprite.Animation.Name != "JumpApex" 
                 && !Collider.CollisionDirections.Values.Contains(CollisionDirection.DownLeft))
            AnimatedSprite.ChangeAnimation(PlayerAnimations.Jump);
        else if (Velocity.Y is >= -5 and <= 5 
                 && AnimatedSprite.Animation.Name == "Jump" 
                 && !Collider.CollisionDirections.Values.Contains(CollisionDirection.DownLeft))
            AnimatedSprite.ChangeAnimation(PlayerAnimations.JumpApex);
        else if (Velocity.X != 0 
                 && Velocity.Y == 0 
                 && AnimatedSprite.Animation.Name != "Run" 
                 && Collider.CollisionDirections.Values.Contains(CollisionDirection.DownLeft))
            AnimatedSprite.ChangeAnimation(PlayerAnimations.Run);
        else if (Velocity == Vector2.Zero 
                 && AnimatedSprite.Animation.Name != "Idle" 
                 && Collider.CollisionDirections.Values.Contains(CollisionDirection.DownLeft))
            AnimatedSprite.ChangeAnimation(PlayerAnimations.Idle);
    }
}

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
    
    public static readonly Animation2D Jump = new(
        "Jump", 
        "PHCharacterJump", 
        new (48, 48), 
        0.1f,
        new()
        {
            new(48 * 0, 0),
        });
    
    public static readonly Animation2D JumpApex = new(
        "JumpApex", 
        "PHCharacterJump", 
        new (48, 48), 
        0.1f,
        new()
        {
            new(48 * 1, 0),
        });
    
    public static readonly Animation2D Fall = new(
        "Fall", 
        "PHCharacterJump", 
        new (48, 48), 
        0.1f,
        new()
        {
            new(48 * 2, 0),
        });
}
