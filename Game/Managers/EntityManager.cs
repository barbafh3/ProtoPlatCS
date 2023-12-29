using ProtoPlat.Interfaces;
using Raylib_cs;
using LanguageExt;
using static LanguageExt.Prelude;

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
        entity.Start();
        return entity;
    }

    public static List<T> GetEntities<T>() where T : GameEntity
    {
        var list = new List<T>();

        foreach (var entity in _entities)
            if (entity is T gameEntity)
            {
                list.Add(gameEntity);
            }

        return list;
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
        for (int i = -100; i < 100; i++)
        {
            foreach (GameEntity entity in _entities)
                if ((entity as IDraw)?.DrawLayer == i)
                    (entity as IDraw)?.Draw();
        }
    }
}