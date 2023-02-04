using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using StbImageSharp;

namespace Core.Render;

public class Texture2D : IDisposable
{
    public int Id { get; }

    public string? Path { get; }

    public TextureWrapMode WrapModeS { get; set; }

    public TextureWrapMode WrapModeT { get; set; }

    public TextureMagFilter TextureMagFilter { get; set; }

    public TextureMinFilter TextureMinFilter { get; set; }
    
    public bool IsMinimap { get; set; }

    public Texture2D(string path,
        TextureWrapMode wrapModeS = TextureWrapMode.Repeat,
        TextureWrapMode wrapModeT = TextureWrapMode.Repeat,
        TextureMagFilter magFilter = TextureMagFilter.Linear,
        TextureMinFilter minFilter = TextureMinFilter.Nearest,
        bool isMinimap = false)
    {
        ImageResult? image = LoadTexture2DFromPath(path);

        if (image != null)
        {
            Path = path;
            Id = GL.GenTexture();
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, Id);
            WrapModeS = wrapModeS;
            WrapModeT = wrapModeT;
            TextureMagFilter = magFilter;
            TextureMinFilter = minFilter;
            IsMinimap = isMinimap;
            int ws = (int)wrapModeS;
            int wt = (int)wrapModeT;
            int magF = (int)magFilter;
            int minF = (int)minFilter;
            GL.TextureParameterI(Id, TextureParameterName.TextureWrapS, ref ws);
            GL.TextureParameterI(Id, TextureParameterName.TextureWrapT, ref wt);
            GL.TextureParameterI(Id, TextureParameterName.TextureMagFilter, ref magF);
            GL.TextureParameterI(Id, TextureParameterName.TextureMinFilter, ref minF);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba32f, image.Width, image.Height, 0,
                PixelFormat.Rgba, PixelType.UnsignedByte, image.Data);
            if (isMinimap)
            {
                GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
            }
        }

        GL.BindTexture(TextureTarget.Texture2D, 0);
    }

    public Texture2D(Color4 color,
        TextureWrapMode wrapModeS = TextureWrapMode.Repeat,
        TextureWrapMode wrapModeT = TextureWrapMode.Repeat,
        TextureMagFilter magFilter = TextureMagFilter.Linear,
        TextureMinFilter minFilter = TextureMinFilter.Nearest,
        bool isMinimap = false)
    {
        Id = GL.GenTexture();
        GL.ActiveTexture(TextureUnit.Texture0);
        GL.BindTexture(TextureTarget.Texture2D, Id);
        WrapModeS = wrapModeS;
        WrapModeT = wrapModeT;
        TextureMagFilter = magFilter;
        TextureMinFilter = minFilter;
        IsMinimap = isMinimap;
        int ws = (int)wrapModeS;
        int wt = (int)wrapModeT;
        int magF = (int)magFilter;
        int minF = (int)minFilter;
        GL.TextureParameterI(Id, TextureParameterName.TextureWrapS, ref ws);
        GL.TextureParameterI(Id, TextureParameterName.TextureWrapT, ref wt);
        GL.TextureParameterI(Id, TextureParameterName.TextureMagFilter, ref magF);
        GL.TextureParameterI(Id, TextureParameterName.TextureMinFilter, ref minF);
        GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba32f, 1, 1, 0,
            PixelFormat.Rgba, PixelType.Float, new[] { color.R, color.G, color.B, color.A });
    }

    public void Bind(int solt = 0)
    {
        GL.ActiveTexture(TextureUnit.Texture0 + solt);
        GL.BindTexture(TextureTarget.Texture2D, Id);
    }

    public void UnBind()
    {
        GL.BindTexture(TextureTarget.Texture2D, 0);
    }

    private ImageResult LoadTexture2DFromPath(string path)
    {
        try
        {
            StbImage.stbi_set_flip_vertically_on_load(1);
            return ImageResult.FromStream(File.OpenRead(path), ColorComponents.RedGreenBlueAlpha);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return null;
        }
    }

    private void ReleaseUnmanagedResources()
    {
        GL.DeleteTexture(Id);
    }

    public void Dispose()
    {
        ReleaseUnmanagedResources();
        GC.SuppressFinalize(this);
    }

    ~Texture2D()
    {
        ReleaseUnmanagedResources();
    }
}