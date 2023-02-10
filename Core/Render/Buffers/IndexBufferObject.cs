using OpenTK.Graphics.OpenGL4;

namespace Core.Render.Buffers;

public class IndexBufferObject : IDisposable
{
    private readonly int id;

    public int Length { get; }

    public IndexBufferObject(uint[] indices)
    {
        Length = indices.Length;

        id = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, id);
        GL.BufferData(BufferTarget.ElementArrayBuffer, sizeof(uint) * indices.Length, indices,
            BufferUsageHint.StaticDraw);
    }

    public void Bind()
    {
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, id);
    }

    public void UnBind()
    {
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
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

    ~IndexBufferObject()
    {
        ReleaseUnmanagedResources();
    }
}