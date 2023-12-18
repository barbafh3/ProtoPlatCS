using System.Numerics;
using LanguageExt;
using ProtoPlat.Interfaces;
using ProtoPlat.Managers;
using Raylib_cs;

namespace ProtoPlat;

public class Animation2D
{
    public string Name { get; private set; }
    public string AtlasName { get; private set; }
    public Vector2 FrameSize { get; private set; }
    public float Duration { get; private set; }
    private readonly List<Vector2> _atlasCoords;
    public List<Vector2> AtlasCoords => new(_atlasCoords);

    public Animation2D(string name, string atlasName, Vector2 frameSize, float duration, List<Vector2> atlasCoords)
    {
        Name = name;
        AtlasName = atlasName;
        _atlasCoords = atlasCoords;
        FrameSize = frameSize;
        Duration = duration;
    }
}

public class AnimatedSprite2D : Entity2D, IUpdate, IDraw
{
    public int DrawLayer { get; set; }
    public Animation2D Animation;
    public bool Looping = true;
    public float CurrentDuration = 0f;
    public bool Done = false;
    public int Frame = 0;
    public (bool, bool) Flip = (false, false);
    
    public AnimatedSprite2D(){}

    public AnimatedSprite2D(Animation2D animation, bool looping)
    {
        Animation = animation;
        Looping = looping;
    }

    public void ChangeAnimation(Animation2D anim)
    {
        Animation = anim;
        CurrentDuration = 0f;
        Frame = 0;
    }


    public virtual void Update(float delta)
    {
        var newDuration = CurrentDuration + delta;
        if (newDuration >= Animation.Duration)
        {
            if (Looping || Frame + 1 < Animation.AtlasCoords.Count - 1)
            {
                Frame = Frame + 1 > Animation.AtlasCoords.Count - 1 ? 0 : Frame + 1;
                CurrentDuration = 0;
            }
        }
        else
            CurrentDuration = newDuration;
    }


    public void Draw()
    {
        var atlasFrame = Animation.AtlasCoords[Frame];
        var flipX = Flip.Item1 ? Animation.FrameSize.X * -1 : Animation.FrameSize.X;
        var flipY = Flip.Item2 ? Animation.FrameSize.Y * -1 : Animation.FrameSize.Y;
        var rect = new Rectangle
        {
            X = atlasFrame.X,
            Y = atlasFrame.Y,
            Width = flipX,
            Height = flipY
        };
        AssetManager
            .GetTextureAtlas(Animation.AtlasName)
            .Match(
                Some: atlas => Raylib.DrawTextureRec(atlas, rect, Position, Color.WHITE), 
                None: () => Console.WriteLine("Failed to get atlas texture"));
    }
}
