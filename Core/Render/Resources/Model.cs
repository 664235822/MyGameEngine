using Assimp;
using Core.Math;
using Core.Render.Geometry;
using Core.Render.Log;
using OpenTK.Mathematics;

namespace Core.Render.Resources;

public class Model : IDisposable
{
    public static Model Create(string path)
    {
        Model? model = null;
        try
        {
            model = new Model(path);
        }
        catch (Exception e)
        {
            LogManager.ErrorLogCore(e.Message);
        }

        return model;
    }

    public string Path { get; }
    public List<Mesh> Meshes { get; }
    public List<string> MaterialNames { get; }
    public bool IsDestroy { get; private set; } = false;
    public Sphere BoundingSphere { get; private set; }

    private Model(string path)
    {
        Path = path;

        (Meshes, MaterialNames) = LoadModel(path);
        CreateBoundingSphere(Meshes);
    }

    private void CreateBoundingSphere(List<Mesh> meshes)
    {
        if (meshes.Count == 1)
        {
            BoundingSphere = meshes[0].BoundingSphere;
            return;
        }

        if (meshes.Count > 1)
        {
            float minX = float.MaxValue;
            float minY = float.MaxValue;
            float minZ = float.MaxValue;

            float maxX = float.MinValue;
            float maxY = float.MinValue;
            float maxZ = float.MinValue;

            foreach (var mesh in meshes)
            {
                minX = MathHelper.Min(minX, mesh.BoundingSphere.Position.X - mesh.BoundingSphere.Radius);
                minY = MathHelper.Min(minY, mesh.BoundingSphere.Position.Y - mesh.BoundingSphere.Radius);
                minZ = MathHelper.Min(minZ, mesh.BoundingSphere.Position.Z - mesh.BoundingSphere.Radius);

                maxX = MathHelper.Max(maxX, mesh.BoundingSphere.Position.X + mesh.BoundingSphere.Radius);
                maxY = MathHelper.Max(maxY, mesh.BoundingSphere.Position.Y + mesh.BoundingSphere.Radius);
                maxZ = MathHelper.Max(maxZ, mesh.BoundingSphere.Position.Z + mesh.BoundingSphere.Radius);
            }

            Vector3 position = new Vector3(minX + maxX, minY + maxY, minZ + maxZ) / 2;
            float radius = MathHelper.Max(Vector3.Distance(position, new Vector3(minX, minY, minZ)),
                Vector3.Distance(position, new Vector3(maxX, maxY, maxZ)));
            BoundingSphere = new Sphere
            {
                Position = position,
                Radius = radius
            };
        }
    }

    private (List<Mesh> meshes, List<string> materialNames) LoadModel(string path,
        PostProcessSteps postProcessSteps = PostProcessSteps.None)
    {
        AssimpContext assimp = new AssimpContext();
        Scene scene = assimp.ImportFile(path, postProcessSteps);
        if (scene is null || (scene.SceneFlags & SceneFlags.Incomplete) == SceneFlags.Incomplete ||
            scene?.RootNode is null)
        {
            LogManager.ErrorLogCore("assimp load error!");
            return (null, null);
        }

        List<string> materialNames = ProcessMaterials(scene);
        List<Mesh> meshes = new List<Mesh>();
        ProcessNode(scene, meshes, scene.RootNode, Assimp.Matrix4x4.Identity);

        return (meshes, materialNames);
    }

    private List<string> ProcessMaterials(Scene scene)
    {
        List<string> materialNames = new List<string>();
        if (scene.HasMaterials)
        {
            foreach (var mat in scene.Materials)
            {
                materialNames.Add(mat.HasName ? mat.Name : "??");
            }
        }

        return materialNames;
    }

    private void ProcessNode(Scene scene, List<Mesh> meshes, Node rootNode, Assimp.Matrix4x4 transform)
    {
        Matrix4x4 nodeTransform = rootNode.Transform * transform;

        foreach (var index in rootNode.MeshIndices)
        {
            Assimp.Mesh mesh = scene.Meshes[index];
            meshes.Add(ProcessMesh(mesh, nodeTransform));
        }

        foreach (var node in rootNode.Children)
        {
            ProcessNode(scene, meshes, node, nodeTransform);
        }
    }

    private Mesh ProcessMesh(Assimp.Mesh mesh, Assimp.Matrix4x4 transform)
    {
        string meshName = mesh.Name;
        List<Vertex> vertices = new List<Vertex>();
        List<uint> indices = new List<uint>();
        int materialIndex = mesh.MaterialIndex;
        for (int i = 0; i < mesh.VertexCount; i++)
        {
            Vector3D position = transform * (mesh.HasVertices ? mesh.Vertices[i] : new Vector3D(0, 0, 0));
            Vector3D normal = transform * (mesh.HasNormals ? mesh.Normals[i] : new Vector3D(0, 0, 0));
            Vector3D texCoord = mesh.HasTextureCoords(0) ? mesh.TextureCoordinateChannels[0][i] : new Vector3D(0, 0, 0);
            Vector3D tangent = transform * (mesh.HasTangentBasis ? mesh.Tangents[i] : new Vector3D(0, 0, 0));
            Vector3D biTangent = transform * (mesh.HasTangentBasis ? mesh.BiTangents[i] : new Vector3D(0, 0, 0));

            Vertex vertex = new Vertex();
            vertex.Position.X = position.X;
            vertex.Position.Y = position.Y;
            vertex.Position.Z = position.Z;

            vertex.Normal.X = normal.X;
            vertex.Normal.Y = normal.Y;
            vertex.Normal.Z = normal.Z;

            vertex.TexCoords.X = texCoord.X;
            vertex.TexCoords.Y = texCoord.Y;

            vertex.Tangent.X = tangent.X;
            vertex.Tangent.Y = tangent.Y;
            vertex.Tangent.Z = tangent.Z;

            vertex.BiTangent.X = biTangent.X;
            vertex.BiTangent.Y = biTangent.Y;
            vertex.BiTangent.Z = biTangent.Z;

            vertices.Add(vertex);
        }

        if (mesh.HasFaces)
        {
            foreach (var face in mesh.Faces)
            {
                foreach (var faceIndex in face.Indices)
                {
                    indices.Add((uint)faceIndex);
                }
            }
        }

        return new Mesh(meshName, vertices, indices, materialIndex);
    }

    public void Dispose()
    {
        if (!IsDestroy)
        {
            foreach (var mesh in Meshes)
            {
                mesh.Dispose();
            }

            IsDestroy = true;
        }
    }
}