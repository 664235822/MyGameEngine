using System.Numerics;
using Vector3 = OpenTK.Mathematics.Vector3;

namespace Core.Render.Geometry;

public struct Vertex
{
    public Vector3 Position;
    public Vector3 Normal;
    public Vector2 TexCoords;
    public Vector3 Tangent;
    public Vector3 BiTangent;
}