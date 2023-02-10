using OpenTK.Graphics.OpenGL4;

namespace Core.Render.Buffers;

public class VertexArrayObject : IDisposable
{
    private readonly int id;
    
    public IndexBufferObject IndexBufferObject { get; }
    
    public VertexArrayObject(IndexBufferObject? indexBufferObject, params VertexBufferObject[] vertexBufferObjects)
    {
        IndexBufferObject = indexBufferObject;
        
        id = GL.GenVertexArray();
        GL.BindVertexArray(id);

        foreach (var vertexBufferObject in vertexBufferObjects)
        {
            vertexBufferObject.Bind();
            
            int offset = 0;
            foreach (var element in vertexBufferObject.Layout.Elements)
            {
                GL.VertexAttribPointer(element.Location, element.Count, VertexAttribPointerType.Float,
                    element.IsNormalized, vertexBufferObject.Stride, offset);
                GL.EnableVertexAttribArray(element.Location);
                offset += element.Count * sizeof(float);
            }
        }
    }
    
    public void Bind()
    {
        GL.BindBuffer(BufferTarget.ArrayBuffer, id);
        IndexBufferObject?.Bind();
    }

    public void UnBind()
    {
        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
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

    ~VertexArrayObject()
    {
        ReleaseUnmanagedResources();
    }
}