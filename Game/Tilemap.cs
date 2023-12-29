using System.Numerics;
using ExtensionMethods;
using LanguageExt;
using LanguageExt.UnsafeValueAccess;
using static LanguageExt.Prelude;
using ProtoPlat.Interfaces;
using ProtoPlat.Managers;
using Raylib_cs;

namespace ProtoPlat;

public class JsonTile
{
    public Vector2 Coords { get; set; }
    public Vector2 AtlasCoords { get; set; }
}

public class JsonTileMap
{
    public string TileSet { get; set; }
    public float TileSetScale { get; set; }
    public Vector2 MapSizePx { get; set; }
    public Vector2 TileSize { get; set; }
    public Vector2 SpriteSize { get; set; }
    public List<JsonTile> Tiles { get; set; }
}

public class Tile : Entity2D
{
    public TileMap Tilemap { get; }
    public Vector2 MapCoords { get; }
    public Rectangle AtlasCoords { get; }
    public float SpriteScale { get; }

    public Collider2D Collider;
    
    public new Vector2 Position => Tilemap.Position + new Vector2(MapCoords.X * AtlasCoords.Width, MapCoords.Y * AtlasCoords.Height);

    public Tile(TileMap map, Vector2 coords, Rectangle atlasCoords, float spriteScale)
    {
        Name = $"Tile({coords.X},{coords.Y})";
        Tilemap = map;
        MapCoords = coords;
        AtlasCoords = atlasCoords;
        Parent = map;
        SpriteScale = spriteScale;
        Collider = new Collider2D(new BoundingBox
            {
                Min = Position.ToVector3(),
                Max = Position.ToVector3() + Tilemap.MapData.TileSize.ToVector3()
            }, 
            Vector2.Zero);
        Collider.Parent = this;
        Collider.Static = true;
    }

    public void DrawTile()
    {
        var src = AtlasCoords;
        src.X *= Tilemap.MapData.SpriteSize.X;
        src.Y *= Tilemap.MapData.SpriteSize.Y;
        src.Width = Tilemap.MapData.SpriteSize.X;
        src.Height = Tilemap.MapData.SpriteSize.Y;
        var dest = new Rectangle
        {
            X = Position.X,
            Y = Position.Y,
            Width = Tilemap.MapData.TileSize.X,
            Height = Tilemap.MapData.TileSize.Y
        };
        AssetManager
            .GetTextureAtlas(Tilemap.AtlasName)
            .Match(
                Some: atlas => Raylib.DrawTexturePro(atlas, src, dest, Vector2.Zero, 0f, Color.WHITE), 
                None: () => GameLogger.Log(LogLevel.ERROR, "Failed to get atlas texture"));
    }
}

public class Chunk
{
    public bool Visible = true;
    public TileMap Tilemap;
    public Rectangle Rect { get; private set; }
    public List<Tile> Tiles = new();

    public Chunk(TileMap map, Vector2 origin, float spriteScale)
    {
        Tilemap = map;
        var mapData = map.MapData;
        
        var chunkSize = Constants.ChunkSize;
        var worldStart = new Vector2(origin.X * chunkSize.X, origin.Y * chunkSize.Y);
        var worldEnd = new Vector2(worldStart.X + chunkSize.X, worldStart.Y + chunkSize.Y);
        Rect = new Rectangle(worldStart.X, worldStart.Y, mapData.TileSize.X * chunkSize.X, mapData.TileSize.Y * chunkSize.Y);

        var chunkTiles = new List<Vector2>();
        for (int x = (int)worldStart.X; x < (int)worldEnd.X; x++)
        {
            for (int y = (int)worldStart.Y; y < (int)worldEnd.X; y++)
            {
                chunkTiles.Add(new Vector2(x, y));
            }
        }
        foreach (var tile in mapData.Tiles)
        {
            if (chunkTiles.Contains(tile.Coords))
            {
                var rect = new Rectangle(tile.AtlasCoords.X, tile.AtlasCoords.Y, mapData.TileSize.X, mapData.TileSize.Y);
                Tiles.Add(new Tile(map, tile.Coords, rect, spriteScale));
            }
        }
    }

    public void DrawChunk()
    {
        if (Visible) Tiles.ForEach(tile => tile.DrawTile());
    }
}

public class TileMap : Entity2D, IDraw
{
    public string AtlasName { get; set; }
    public List<Chunk> Chunks { get; private set; } = new();
    public int DrawLayer { get; set; }
    public JsonTileMap MapData;
    
    public TileMap() : base("TileMap") {}

    public void CreateTileMap(string mapPath)
    {
        MapData = Utils.LoadTileMapJson(mapPath);
        AtlasName = MapData.TileSet;
        var mapTileCount = new Vector2(
            (float)Math.Ceiling(MapData.MapSizePx.X / MapData.TileSize.X),
            (float)Math.Ceiling(MapData.MapSizePx.Y / MapData.TileSize.Y));
        var chunkCount = new Vector2(
            Math.Max(1, (float)Math.Ceiling(mapTileCount.X / Constants.ChunkSize.X)),
            Math.Max(1, (float)Math.Ceiling(mapTileCount.X / Constants.ChunkSize.X))
        );
        var chunkCoordList = new List<Vector2>();
        for (int x = 0; x < (int)chunkCount.X; x++)
        {
            for (int y = 0; y < (int)chunkCount.Y; y++)
            {
                chunkCoordList.Add(new Vector2(x, y));
            }
        }

        foreach (var chunkCoords in chunkCoordList)
        {
            Chunks.Add(new Chunk(this, chunkCoords, MapData.TileSetScale));
        }
    }
    
    public void Draw() => Chunks.ForEach(chunk => chunk.DrawChunk());
}