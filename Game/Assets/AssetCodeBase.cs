using Raylib_cs;

namespace ProtoPlat;

public unsafe class AssetCodeBase
{
    public readonly int Width;
    public readonly int Height;
    public readonly int Format;
    public readonly byte[] Data;
    
    public Texture2D GetTexture()
    {
        fixed (byte* bptr = Data)
        {
            var image = new Image
            {
                Data = bptr,
                Width = Width,
                Height = Height,
                Mipmaps = 1,
                Format = (PixelFormat)Format
            };
        
            var texture = Raylib.LoadTextureFromImage(image);
            return texture;
        }
    }

}