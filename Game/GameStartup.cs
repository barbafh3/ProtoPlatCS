using System.Numerics;
using ProtoPlat.Interfaces;
using ProtoPlat.Managers;
using Raylib_cs;

namespace ProtoPlat;

public static class GameStartup
{
    public static void Execute()
    {
        var scale = 2f;

        var collider = new Collider2D(
            new(100, 100, 16 * scale, 29 * scale), 
            new(16 * scale, 11 * scale)
            );

        var animatedSprite = new AnimatedSprite2D(PlayerAnimations.Idle, scale, 10);

        var player = new Player(
            collider, 
            animatedSprite, 
            500f, 
            new Vector2(100, 100) 
            );
        player.AddChild(animatedSprite);

        var ground = EntityManager.NewEntity<ColorPlatform>();
        ground.Position = new(0, 600);
        var g1rect = new Rectangle
        {
            X = ground.Position.X,
            Y = ground.Position.Y,
            Width = 2000,
            Height = 300
        };
        ground.Rect = g1rect;
        ground.Color = Color.DARKGREEN;
        ground.Collider = new Collider2D(g1rect, Vector2.Zero);
        ground.Collider.Parent = ground;
        
        var ground2 = EntityManager.NewEntity<ColorPlatform>();
        ground2.Position = new(500, 100);
        var g2rect = new Rectangle
        {
            X = ground2.Position.X,
            Y = ground2.Position.Y,
            Width = 48,
            Height = 600
        };
        ground2.Rect = g2rect;
        ground2.Color = Color.DARKBLUE;
        ground2.Collider = new Collider2D(g2rect, Vector2.Zero);
        ground2.Collider.Parent = ground2;
        
        var ground3 = EntityManager.NewEntity<ColorPlatform>();
        ground3.Position = new(700, 400);
        var g3rect = new Rectangle
        {
            X = ground3.Position.X,
            Y = ground3.Position.Y,
            Width = 500,
            Height = 48
        };
        ground3.Rect = g3rect;
        ground3.Color = Color.DARKBLUE;
        ground3.Collider = new Collider2D(g3rect, Vector2.Zero);
        ground3.Collider.Parent = ground3;

        // var bg1 = new Sprite2D("OakWoodsBG1", 4, -10);
        // var bg2 = new Sprite2D("OakWoodsBG2", 4, -9);
        // var bg3 = new Sprite2D("OakWoodsBG3", 4, -8);

        // var tilemap = EntityManager.NewEntity<TileMap>();
        // tilemap.CreateTileMap("OakWoodsTileSet", "Assets/tilemaps/tilemap1.json", 2f);
        // tilemap.DrawLayer = -5;

    }
}