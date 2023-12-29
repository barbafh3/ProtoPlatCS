using System.Numerics;
using ExtensionMethods;
using ProtoPlat.Interfaces;
using ProtoPlat.Managers;
using Raylib_cs;

namespace ProtoPlat;

public static class GameStartup
{
    public static void Execute()
    {
        var scale = 2f;
        var playerPos = new Vector3(500, 100, 0);
        var playerBoxSize = new Vector3(16 * scale, 29 * scale, 0);
        var offset = new Vector2(16 * scale, 11 * scale);
        var collider = new Collider2D(
            new(playerPos + offset.ToVector3(), playerPos + offset.ToVector3() + playerBoxSize), 
            offset
            );
        collider.Static = false;
        
        var animatedSprite = new AnimatedSprite2D(PlayerAnimations.Idle, scale, 10);

        var player = new Player(
            collider, 
            animatedSprite, 
            500f, 
            playerPos.ToVector2()
            );

        // var ground = EntityManager.NewEntity<ColorPlatform>();
        // ground.Name = "Ground1";
        // ground.Position = new(0, 600);
        // var g1rect = new BoundingBox
        // {
        //     Min = ground.Position.ToVector3(),
        //     Max = ground.Position.ToVector3() + new Vector3(2000, 300, 0f)
        // };
        // ground.Box = g1rect;
        // ground.Color = Color.DARKGREEN;
        // ground.Collider = new Collider2D(g1rect, Vector2.Zero);
        // ground.Collider.Name = "Collider1";
        // ground.Collider.Parent = ground;
        //
        // var ground2 = EntityManager.NewEntity<ColorPlatform>();
        // ground2.Name = "Ground2";
        // ground2.Position = new(500, 100);
        // var g2rect = new BoundingBox()
        // {
        //     Min = ground2.Position.ToVector3(),
        //     Max = ground2.Position.ToVector3() + new Vector3(48, 600, 0f)
        //     // Max = new Vector3(48, 600, 0f)
        // };
        // ground2.Box = g2rect;
        // ground2.Color = Color.DARKBLUE;
        // ground2.Collider = new Collider2D(g2rect, Vector2.Zero);
        // ground2.Collider.Name = "Collider2";
        // ground2.Collider.Parent = ground2;
        //
        // var ground3 = EntityManager.NewEntity<ColorPlatform>();
        // ground3.Name = "Ground3";
        // ground3.Position = new(700, 400);
        // var g3rect = new BoundingBox
        // {
        //     Min = ground3.Position.ToVector3(),
        //     Max = ground3.Position.ToVector3() + new Vector3(500, 48, 0)
        // };
        // ground3.Box = g3rect;
        // ground3.Color = Color.DARKBLUE;
        // ground3.Collider = new Collider2D(g3rect, Vector2.Zero);
        // ground3.Collider.Name = "Collider3";
        // ground3.Collider.Parent = ground3;
        //
        // var ground4 = EntityManager.NewEntity<ColorPlatform>();
        // ground4.Name = "Ground2";
        // ground4.Position = new(495, 100);
        // var g4rect = new BoundingBox()
        // {
        //     Min = ground4.Position.ToVector3(),
        //     Max = ground4.Position.ToVector3() + new Vector3(48, 600, 0f)
        // };
        // ground4.Box = g4rect;
        // ground4.Color = Color.DARKBLUE;
        // ground4.Collider = new Collider2D(g4rect, Vector2.Zero);
        // ground4.Collider.Name = "Collider4";
        // ground4.Collider.Parent = ground4;

        var bg1 = new Sprite2D("OakWoodsBG1", 4, -10);
        var bg2 = new Sprite2D("OakWoodsBG2", 4, -9);
        var bg3 = new Sprite2D("OakWoodsBG3", 4, -8);

        var tilemap = EntityManager.NewEntity<TileMap>();
        tilemap.CreateTileMap("Assets/tilemaps/tilemap1.json");
        tilemap.DrawLayer = -5;

    }
}