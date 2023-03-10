using Core.Math;
using OpenTK.Mathematics;

namespace Core.Render.Geometry;

public struct Frustum
{
    public Plane NearPlane;
    public Plane FarPlane;
    public Plane RightPlane;
    public Plane LeftPlane;
    public Plane TopPlane;
    public Plane BottomPlane;

    public void CalculateFrustum(Camera camera, float aspect, Vector3 position, Quaternion rotation)
    {
        Vector3 front = rotation * Vector3.UnitZ;
        Vector3 right = rotation * Vector3.UnitX;
        Vector3 up = rotation * Vector3.UnitY;
        float halfVSide = camera.Far * (float)MathHelper.Tan(MathHelper.DegreesToRadians(camera.Fov / 2));
        float halfHSide = halfVSide * aspect;
        Vector3 frontMultFar = camera.Far * front;

        NearPlane = new Plane
        {
            Position = position + camera.Near * front,
            Normal = front
        };

        FarPlane = new Plane
        {
            Position = position + camera.Far * front,
            Normal = -front
        };

        RightPlane = new Plane
        {
            Position = position,
            Normal = Vector3.Cross(frontMultFar - right * halfHSide, up)
        };

        LeftPlane = new Plane
        {
            Position = position,
            Normal = Vector3.Cross(up, frontMultFar + right * halfHSide)
        };

        TopPlane = new Plane
        {
            Position = position,
            Normal = Vector3.Cross(right, up * halfVSide + frontMultFar)
        };

        BottomPlane = new Plane
        {
            Position = position,
            Normal = Vector3.Cross(frontMultFar - up * halfVSide, right)
        };
    }

    public bool IsSphereInFrustum(Sphere sphere)
    {
        return NearPlane.DistanceToPlane(sphere.Position) >= -sphere.Radius &&
               FarPlane.DistanceToPlane(sphere.Position) >= -sphere.Radius &&
               BottomPlane.DistanceToPlane(sphere.Position) >= -sphere.Radius &&
               TopPlane.DistanceToPlane(sphere.Position) >= -sphere.Radius &&
               LeftPlane.DistanceToPlane(sphere.Position) >= -sphere.Radius &&
               RightPlane.DistanceToPlane(sphere.Position) >= -sphere.Radius;
    }

    public bool IsBoundingSphereInFrustum(Quaternion rotation, Vector3 scale, Sphere sphere)
    {
        float radius = MathHelper.Max(MathHelper.Max(scale.X, scale.Y), scale.Z) * sphere.Radius;
        Vector3 centerPos = rotation * sphere.Position;
        return IsSphereInFrustum(new Sphere() { Position = centerPos, Radius = radius });
    }
}