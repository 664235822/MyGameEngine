using Core.Render.Geometry;

namespace Core.Render;

public class Mesh : IDisposable
{
    public int VertexCount { get; }
    public int IndexCount { get; }
    public int MaterialIndex { get; }

    private IndexBufferObject? indexBufferObject;
    private VertexBufferObject vertexBufferObject;
    private VertexArrayObject vertexArrayObject;

    public Mesh(List<Vertex> vertices, List<uint> indices, int materialIndex)
    {
        VertexCount = vertices.Count;
        IndexCount = indices.Count;
        MaterialIndex = materialIndex;

        CreateBuffer(vertices, indices);
    }

    private void CreateBuffer(List<Vertex> vertices, List<uint> indices)
    {
        List<float> vertexData = new List<float>();
        foreach (var vertex in vertices)
        {
            vertexData.Add(vertex.Position.X);
            vertexData.Add(vertex.Position.Y);
            vertexData.Add(vertex.Position.Z);

            vertexData.Add(vertex.Normal.X);
            vertexData.Add(vertex.Normal.Y);
            vertexData.Add(vertex.Normal.Z);

            vertexData.Add(vertex.TexCoords.X);
            vertexData.Add(vertex.TexCoords.Y);

            vertexData.Add(vertex.Tangent.X);
            vertexData.Add(vertex.Tangent.Y);
            vertexData.Add(vertex.Tangent.Z);

            vertexData.Add(vertex.BiTangent.X);
            vertexData.Add(vertex.BiTangent.Y);
            vertexData.Add(vertex.BiTangent.Z);
        }

        vertexBufferObject = new VertexBufferObject(vertexData.ToArray());
        VertexBufferLayout layout = new VertexBufferLayout();
        layout.AddElement(new VertexBufferLayoutElement(0, 3),
            new VertexBufferLayoutElement(1, 3),
            new VertexBufferLayoutElement(2, 2),
            new VertexBufferLayoutElement(3, 3),
            new VertexBufferLayoutElement(4, 3));
        vertexBufferObject.AddLayout(layout);

        if (indices.Count > 3)
        {
            indexBufferObject = new IndexBufferObject(indices.ToArray());
        }

        vertexArrayObject = new VertexArrayObject(indexBufferObject, vertexBufferObject);
    }

    public void Bind()
    {
        vertexArrayObject.Bind();
    }

    public void UnBind()
    {
        vertexArrayObject.UnBind();
    }

    public void Dispose()
    {
        indexBufferObject?.Dispose();
        vertexBufferObject.Dispose();
        vertexArrayObject.Dispose();
    }
}