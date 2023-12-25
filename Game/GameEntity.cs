using System.Numerics;
using System.Reflection;
using LanguageExt;
using ProtoPlat.Managers;
using static LanguageExt.Prelude;

namespace ProtoPlat;

public class GameEntity
{
    public string Name = "GameEntity";
    public Option<GameEntity> Parent = None;
    protected List<GameEntity> _children = new();

    public GameEntity(string name = "GameEntity")
    {
        EntityManager.RegisterGameEntity(this);
    }

    public virtual void Start() { }

    public void AddChild(GameEntity child) => _children.Add(child);
    
    /// <summary>
    /// Fetches all children of this game entity.
    /// </summary>
    /// <returns>List of children</returns>
    public List<GameEntity> GetChildren() => new(_children);

    /// <summary>
    /// Finds the first child of this game entity with the given type.
    /// </summary>
    /// <typeparam name="T">Type of the child</typeparam>
    /// <returns>Found child. <b>None</b> if no child of given type is found.</returns>
    public Option<T> GetChild<T>() where T : GameEntity
    {
        foreach (var entity in _children)
        {
            if (entity is T gameEntity)
                return gameEntity;
        }

        return None;
    }
    
    /// <summary>
    /// Finds a child of this game entity by the given type and name.
    /// </summary>
    /// <typeparam name="T">Type of the child.</typeparam>
    /// <param name="name">Name of the child.</param>
    /// <returns>Found child. <b>None</b> if no child of given name and type is found.</returns>
    public Option<T> GetChild<T>(string name) where T : GameEntity
    {
        foreach (var entity in _children)
        {
            if (entity is T gameEntity && Name == name)
                return gameEntity;
        }

        return None;
    }
    
    public bool HasProperty(string propertyName) => GetType().GetTypeInfo().GetProperty(propertyName) != null;
}

public class Entity2D : GameEntity
{
    public Vector2 Position = Vector2.Zero;
    public Vector2 Offset;
    
    public Entity2D(string name = "Entity2D") : base(name) { }
    

    protected virtual void RepositionChildren()
    {
        _children.ForEach(child =>
        {
            if (child is Entity2D child2D)
            {
                child2D.Position = Position + Offset;
                child2D.RepositionChildren();
            }
        });
        
    }
}
