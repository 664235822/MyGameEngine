using OpenTK.Mathematics;

namespace Core.ECS.Components;

public class Transform : IComponent
{
    public Guid Id { get; }

    public Transform(Guid id)
    {
        Id = id;

        position = new Vector3();
        rotation = Quaternion.Identity;
        scale = Vector3.One;
        parentMatrix = Matrix4.Identity;
        localMatrix = Matrix4.Identity;
        worldMatrix = Matrix4.Identity;

        isDirty = false;
    }

    private Vector3 position;
    private Quaternion rotation;
    private Vector3 scale;
    private Matrix4 parentMatrix;
    private Matrix4 localMatrix;
    private Matrix4 worldMatrix;

    private bool isDirty;

    public Vector3 LocalPosition
    {
        get
        {
            if (isDirty)
            {
                UpdateMatrices();
            }

            return position;
        }
        set
        {
            position = value;
            isDirty = true;
        }
    }

    public Quaternion LocalRotation
    {
        get
        {
            if (isDirty)
            {
                UpdateMatrices();
            }

            return rotation;
        }
        set
        {
            rotation = value;
            isDirty = true;
        }
    }

    public Vector3 LocalScale
    {
        get
        {
            if (isDirty)
            {
                UpdateMatrices();
            }

            return scale;
        }
        set
        {
            scale = value;
            isDirty = true;
        }
    }

    public Matrix4 ParentMatrix
    {
        get
        {
            if (isDirty)
            {
                UpdateMatrices();
            }

            return parentMatrix;
        }
        set
        {
            if (isDirty)
            {
                UpdateMatrices();
            }

            parentMatrix = value;
            worldMatrix = value * localMatrix;
        }
    }

    public Matrix4 LocalMatrix
    {
        get
        {
            if (isDirty)
            {
                UpdateMatrices();
            }

            return localMatrix;
        }
        set
        {
            localMatrix = value;
            worldMatrix = parentMatrix * value;

            ApplyTransform();
        }
    }

    public Vector3 WorldPosition
    {
        get
        {
            if (isDirty)
            {
                UpdateMatrices();
            }

            return worldMatrix.ExtractTranslation();
        }
    }

    public Quaternion WorldRotation
    {
        get
        {
            if (isDirty)
            {
                UpdateMatrices();
            }

            return worldMatrix.ExtractRotation();
        }
    }

    public Vector3 WorldScale
    {
        get
        {
            if (isDirty)
            {
                UpdateMatrices();
            }

            return worldMatrix.ExtractScale();
        }
    }

    public Vector3 LocalForward => LocalRotation * Vector3.UnitZ;
    public Vector3 LocalUp => LocalRotation * Vector3.UnitY;
    public Vector3 LocalRight => LocalRotation * Vector3.UnitX;
    
    public Vector3 WorldForward => WorldRotation * Vector3.UnitZ;
    public Vector3 WorldUp => WorldRotation * Vector3.UnitY;
    public Vector3 WorldRight => WorldRotation * Vector3.UnitX;

    private void UpdateMatrices()
    {
        localMatrix = Matrix4.CreateScale(scale) * Matrix4.CreateFromQuaternion(rotation) *
                      Matrix4.CreateTranslation(position);
        worldMatrix = parentMatrix * localMatrix;

        isDirty = false;
    }

    private void ApplyTransform()
    {
        position = localMatrix.ExtractTranslation();
        rotation = localMatrix.ExtractRotation();
        scale = localMatrix.ExtractScale();
    }
}