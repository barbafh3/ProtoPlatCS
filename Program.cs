using System.Numerics;
using ProtoPlat.Managers;
using Raylib_cs;

namespace ProtoPlat;

class Program
{
    public static void Main()
    {
        GameManager.LoadGameConfig();

        Raylib.InitWindow(
            Int32.Parse(GameManager.WindowConfig["Width"].ToString() ?? throw new InvalidOperationException()),
            Int32.Parse(GameManager.WindowConfig["Height"].ToString() ?? throw new InvalidOperationException()),
            GameManager.WindowConfig["Title"].ToString()
            );
        Raylib.SetTargetFPS(
            Int32.Parse(GameManager.WindowConfig["TargetFPS"].ToString() ?? throw new InvalidOperationException())
            );

        AssetManager.LoadAtlas("SlimeIdle","Assets/slime/PlayerV2-Sheet.png");
        AssetManager.LoadAtlas("PHCharacterIdle","Assets/ph-character/48x48/idle.png");
        AssetManager.LoadAtlas("PHCharacterRun","Assets/ph-character/48x48/run.png");

        
        var player = EntityManager.NewEntity<Player>();
        player.Position = new Vector2(100, 100);
        player.Animation = PlayerAnimations.Idle;
        player.Speed = 250f;
        player.DrawLayer = 1;
        player.Collision = EntityManager.NewEntity<Collision2D>();
        player.Collision.Parent = player;
        player.Collision.Rect = new(100, 100, 48, 48);

        var ground = EntityManager.NewEntity<ColorPlatform>();
        ground.Position = new(0, 600);
        ground.Rect = new Rectangle
        {
            X = 0,
            Y = 600,
            Width = 2000,
            Height = 300
        };
        ground.Color = Color.BROWN;
        ground.Collision = EntityManager.NewEntity<Collision2D>();
        ground.Collision.Parent = ground;
        ground.Collision.Rect = new(0, 600, 2000, 300);
        
        while (!Raylib.WindowShouldClose())
        {
            // Input phase
            InputManager.HandleInput();
            
            // Update phase
            EntityManager.CheckCollisions();
            EntityManager.UpdateGameEntities();
            
            // FixedUpdate phase
            EntityManager.FixedUpdateGameEntities();
            
            // Draw phase
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.WHITE);

            EntityManager.DrawGameEntities();
            
            Raylib.EndDrawing();
        }

        Raylib.CloseWindow();
    }
}