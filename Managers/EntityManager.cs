using ProtoPlat.Interfaces;
using Raylib_cs;

namespace ProtoPlat.Managers;

public static class EntityManager
{
    private static readonly List<GameEntity> _entities = new();
    
    public static void RegisterGameEntity(GameEntity entity) => _entities.Add(entity);
    public static void UnregisterGameEntity(GameEntity entity) => _entities.Remove(entity);
    
    public static T NewEntity<T>() where T : GameEntity, new()
    {
        var entity = new T();
        RegisterGameEntity(entity);
        return entity;
    }

    public static void CheckCollisions()
    {
        var filteredList = _entities.Filter(entity => entity is Collision2D);
        foreach (Collision2D collider1 in filteredList)
        {
            foreach (Collision2D collider2 in filteredList)
            {
                if (collider1 != collider2)
                {
                    collider1.CheckCollision(collider2);
                }
                    
            }
        }
    }

    public static void UpdateGameEntities()
    {
        var delta = Raylib.GetFrameTime();
        foreach (GameEntity entity in _entities)
            (entity as IUpdate)?.Update(delta);
    }
    
    public static void FixedUpdateGameEntities()
    {
        var delta = Raylib.GetFrameTime();
        foreach (GameEntity entity in _entities)
            (entity as IFixedUpdate)?.FixedUpdate(delta);
    }
    
    public static void DrawGameEntities()
    {
        for (int i = 0; i < 100; i++)
        {
            foreach (GameEntity entity in _entities)
                if ((entity as IDraw)?.DrawLayer == i)
                    (entity as IDraw)?.Draw();
        }
    }
}