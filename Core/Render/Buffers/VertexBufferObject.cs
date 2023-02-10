using OpenTK.Graphics.OpenGL4;

namespace Core.Render.Buffers;

public class VertexBufferObject : IDisposable
{
    private readonly int id;

    public int Length { get; }

    public int Stride => GetStride();

    internal VertexBufferLayout Layout;

    public VertexBufferObject(float[] vertices)
    {
        Length = vertices.Length;
        Layout = new VertexBufferLayout();
        
        id = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, id);
        GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * vertices.Length, vertices, BufferUsageHint.StaticDraw);
    }

    public void Bind()
    {
        GL.BindBuffer(BufferTarget.ArrayBuffer, id);
    }

    public void UnBind()
    {
        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
    }

    public void AddLayout(VertexBufferLayout layout)
    {
        Layout = layout;
    }

    private int GetStride()
    {
        int stride = 0;
        foreach (var vertexBufferLayoutElement in Layout.Elements)
        {
            stride += vertexBufferLayoutElement.Count * sizeof(float);
        }

        return stride;
    }

    private void ReleaseUnmanagedResources()
    {
        GL.DeleteBuffer(id);
    }

    public void Dispose()
    {
        ReleaseUnmanagedResources();
        GC.SuppressFinalize(this);
    }

    ~VertexBufferObject()
    {
        ReleaseUnmanagedResources();
    }
}

public struct VertexBufferLayoutElement
{
    public int Location;
    public int Count;
    public bool IsNormalized;

    public VertexBufferLayoutElement(int location, int count, bool isNormalized = false)
    {
        Location = location;
        Count = count;
        IsNormalized = isNormalized;
    }
}

public struct VertexBufferLayout
{
    internal List<VertexBufferLayoutElement> Elements;

    public VertexBufferLayout()
    {
        Elements = new List<VertexBufferLayoutElement>();
    }

    public void AddElement(params VertexBufferLayoutElement[] elements)
    {
        Elements.AddRange(elements);
    }
}