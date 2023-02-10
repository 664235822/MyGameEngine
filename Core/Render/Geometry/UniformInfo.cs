using OpenTK.Graphics.OpenGL4;

namespace Core.Render.Geometry;

public struct UniformInfo
{
    public string Name;
    public int Location;
    public ActiveUniformType Type;
    public object? Value;

    public UniformInfo(string name, int location, ActiveUniformType type, object? value)
    {
        Name = name;
        Location = location;
        Type = type;
        Value = value;
    }
}