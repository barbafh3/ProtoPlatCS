using System.Numerics;
using System.Reflection;
using LanguageExt;
using static LanguageExt.Prelude;

namespace ProtoPlat;

public class GameEntity
{
    public string Name = "";
    public Option<GameEntity> Parent = None;
    public List<GameEntity> Children { get; private set; } = new();

    public void AddChild(GameEntity child) => Children.Add(child);
    
    /// <summary>
    /// Fetches all children of this game entity.
    /// </summary>
    /// <returns>List of children</returns>
    public List<GameEntity> GetChildren() => new(Children);

    /// <summary>
    /// Finds a child of this game entity by the given type.
    /// </summary>
    /// <typeparam name="T">Type of the child</typeparam>
    /// <returns>Found child. <b>None</b> if no child of given type is found.</returns>
    public Option<GameEntity> GetChild<T>()
    {
        foreach (var entity in Children)
        {
            if (entity is T)
                return entity;
        }

        return None;
    }
    
    /// <summary>
    /// Finds a child of this game entity by the given type.
    /// </summary>
    /// <typeparam name="T">Type of the child.</typeparam>
    /// <param name="name">Name of the child.</param>
    /// <returns>Found child. <b>None</b> if no child of given name and type is found.</returns>
    public Option<GameEntity> GetChild<T>(string name)
    {
        foreach (var entity in Children)
        {
            if (entity is T && Name == name)
                return entity;
        }

        return None;
    }
    
    public bool HasProperty(string propertyName) => GetType().GetTypeInfo().GetProperty(propertyName) != null;
}

public class Entity2D : GameEntity
{
    public Vector2 Position { get; set; } = Vector2.Zero;
}
