using System.Numerics;
using ProtoPlat.Managers;
using Raylib_cs;

namespace ProtoPlat;

class Program
{
    public static void Main()
    {
        GameManager.LoadGameConfig().Wait();

        Raylib.InitWindow(
            GameManager.WindowConfig.Width,
            GameManager.WindowConfig.Height,
            GameManager.WindowConfig.Title
            );
        Raylib.SetTargetFPS(GameManager.WindowConfig.TargetFPS);

        var phAtlasList = new Dictionary<string, Texture2D>();
        unsafe
        {
            var phIdle = new PHIdle();
            fixed (byte* bptr = phIdle.Data)
            {
                var image = new Image
                {
                    Data = bptr,
                    Width = phIdle.Width,
                    Height = phIdle.Height,
                    Mipmaps = 1,
                    Format = (PixelFormat)phIdle.Format
                };
            
                var texture = Raylib.LoadTextureFromImage(image);
                phAtlasList.Add("PHCharacterIdle", texture);
            }
            
            var phRun = new PHRun();
            fixed (byte* bptr = phRun.Data)
            {
                var image = new Image
                {
                    Data = bptr,
                    Width = phRun.Width,
                    Height = phRun.Height,
                    Mipmaps = 1,
                    Format = (PixelFormat)phRun.Format
                };
            
                var texture = Raylib.LoadTextureFromImage(image);
                phAtlasList.Add("PHCharacterRun", texture);
            }
            
            var phJump = new PHJump();
            fixed (byte* bptr = phJump.Data)
            {
                var image = new Image
                {
                    Data = bptr,
                    Width = phJump.Width,
                    Height = phJump.Height,
                    Mipmaps = 1,
                    Format = (PixelFormat)phJump.Format
                };
            
                var texture = Raylib.LoadTextureFromImage(image);
                phAtlasList.Add("PHCharacterJump", texture);
            }
        }

        var oakAtlasList = new Dictionary<string, string>()
        {
            {"OakWoodsBG1", "Assets/tileset/background/background_layer_1.png"},
            {"OakWoodsBG2", "Assets/tileset/background/background_layer_2.png"},
            {"OakWoodsBG3", "Assets/tileset/background/background_layer_3.png"},
            {"OakWoodsTileSet", "Assets/tileset/oak_woods_tileset.png"},
        };

        AssetManager.LoadAtlasList(phAtlasList);
        AssetManager.LoadAtlasList(oakAtlasList);

        GameStartup.Execute();
        
        while (!Raylib.WindowShouldClose())
        {
            // Clearing debug items from last frame
            VisualDebugger.ClearDebugItems();
            
            // Input phase
            InputManager.HandleInput();
            
            // Update phase
            EntityManager.UpdateGameEntities();
            
            // FixedUpdate phase
            EntityManager.FixedUpdateGameEntities();
            
            // Draw phase
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.GRAY);

            if (InputManager.IsInputActionPressed("ToggleDrawCollision"))
                GameManager.DrawCollisionEnabled = !GameManager.DrawCollisionEnabled;
            
            

            EntityManager.DrawGameEntities();
            
            // Draw debug itens before UI and after ingame stuff
            VisualDebugger.DrawDebugItems();
            
            Raylib.DrawFPS(10, 10);
            Raylib.EndDrawing();
        }

        Raylib.CloseWindow();
    }
}