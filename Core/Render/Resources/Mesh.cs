using Core.Math;
using Core.Render.Buffers;
using Core.Render.Geometry;
using OpenTK.Mathematics;

namespace Core.Render.Resources;

public class Mesh : IDisposable
{
    public string MeshName { get; }
    public int VertexCount { get; }
    public int IndexCount { get; }
    public int MaterialIndex { get; }

    public Sphere BoundingSphere { get; private set; }

    private IndexBufferObject? indexBufferObject;
    private VertexBufferObject vertexBufferObject;
    private VertexArrayObject vertexArrayObject;

    public Mesh(string meshName, List<Vertex> vertices, List<uint> indices, int materialIndex)
    {
        MeshName = meshName;
        VertexCount = vertices.Count;
        IndexCount = indices.Count;
        MaterialIndex = materialIndex;

        CreateBuffer(vertices, indices);
        CreateBoundingSphere(vertices);
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

    private void CreateBoundingSphere(List<Vertex> vertices)
    {
        float minX = float.MaxValue;
        float minY = float.MaxValue;
        float minZ = float.MaxValue;

        float maxX = float.MinValue;
        float maxY = float.MinValue;
        float maxZ = float.MinValue;

        foreach (var vertex in vertices)
        {
            minX = MathHelper.Min(minX, vertex.Position.X);
            minY = MathHelper.Min(minY, vertex.Position.Y);
            minZ = MathHelper.Min(minZ, vertex.Position.Z);

            maxX = MathHelper.Max(maxX, vertex.Position.X);
            maxY = MathHelper.Max(maxY, vertex.Position.Y);
            maxZ = MathHelper.Max(maxZ, vertex.Position.Z);
        }

        Vector3 position = new Vector3(minX + maxX, minY + maxY, minZ + maxZ) / 2;
        float radius =
            MathHelper.InverseSqrtFast(vertices.Select(v => Vector3.DistanceSquared(position, v.Position)).Max());
        BoundingSphere = new Sphere
        {
            Position = position,
            Radius = radius
        };
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