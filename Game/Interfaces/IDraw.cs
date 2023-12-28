namespace ProtoPlat.Interfaces;

public interface IDraw
{
    public int DrawLayer { get; set; }
    
    public void Draw();
}