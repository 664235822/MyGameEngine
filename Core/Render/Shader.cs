using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using Vector2 = OpenTK.Mathematics.Vector2;
using Vector3 = OpenTK.Mathematics.Vector3;
using Vector4 = OpenTK.Mathematics.Vector4;

namespace Core.Render;

public class Shader : IDisposable
{
    public int Id { get; private set; }

    public string? Path { get; }

    public Dictionary<string, int> cache = new Dictionary<string, int>();

    public Shader(string path)
    {
        Path = path;
        (string vertexShaderSource, string fragmentShaderSource) = LoadShaderFromPath(path);

        CreateProgram(vertexShaderSource, fragmentShaderSource);
    }

    public Shader(string vertexShaderSource, string fragmentShaderSource)
    {
        CreateProgram(vertexShaderSource, fragmentShaderSource);
    }

    public void Bind()
    {
        GL.UseProgram(Id);
    }

    public void UnBind()
    {
        GL.UseProgram(0);
    }

    public void SetUniform(string name, float v) => GL.Uniform1(GetUniformLocation(name), v);

    public void SetUniform(string name, Vector2 v) => GL.Uniform2(GetUniformLocation(name), v);

    public void SetUniform(string name, Vector3 v) => GL.Uniform3(GetUniformLocation(name), v);

    public void SetUniform(string name, Vector4 v) => GL.Uniform4(GetUniformLocation(name), v);

    public void SetUniform(string name, Matrix2 v) => GL.UniformMatrix2(GetUniformLocation(name), true, ref v);

    public void SetUniform(string name, Matrix3 v) => GL.UniformMatrix3(GetUniformLocation(name), true, ref v);

    public void SetUniform(string name, Matrix4 v) => GL.UniformMatrix4(GetUniformLocation(name), true, ref v);

    private int GetUniformLocation(string name)
    {
        if (cache.ContainsKey(name))
            return cache[name];

        int location = GL.GetUniformLocation(Id, name);

        cache.Add(name, location);
        return location;
    }

    private void CreateProgram(string vertexShaderSource, string fragmentShaderSource)
    {
        int vertexShader = CreateShader(ShaderType.VertexShader, vertexShaderSource);
        int fragmentShader = CreateShader(ShaderType.FragmentShader, fragmentShaderSource);

        Id = GL.CreateProgram();
        GL.AttachShader(Id, vertexShader);
        GL.AttachShader(Id, fragmentShader);
        GL.LinkProgram(Id);
        GL.GetProgram(Id, GetProgramParameterName.LinkStatus, out int success);
        if (success == 0)
        {
            GL.GetProgramInfoLog(Id, out string info);
            Console.WriteLine(info);
        }

        GL.DeleteShader(vertexShader);
        GL.DeleteShader(fragmentShader);
    }

    private (string vertexShaderSource, string fragmentShaderSource) LoadShaderFromPath(string path)
    {
        string[] lines = File.ReadAllLines(path);
        int vertexIndex = Array.IndexOf(lines, "#shader vertex");
        int fragmentIndex = Array.IndexOf(lines, "#shader fragment");
        string[] vertexLines = lines.Skip(vertexIndex + 1).Take(fragmentIndex - vertexIndex - 1).ToArray();
        string[] fragmentLines = lines.Skip(fragmentIndex + 1).ToArray();
        string vertexShader = string.Join("\n", vertexLines);
        string fragmentShader = string.Join("\n", fragmentLines);
        return (vertexShader, fragmentShader);
    }

    private int CreateShader(ShaderType type, string source)
    {
        int id = GL.CreateShader(type);
        GL.ShaderSource(id, source);
        GL.CompileShader(id);
        GL.GetShader(id, ShaderParameter.CompileStatus, out int success);
        if (success == 0)
        {
            GL.GetShaderInfoLog(id, out string info);
            Console.WriteLine(info);
        }

        return id;
    }

    private void ReleaseUnmanagedResources()
    {
        GL.DeleteProgram(Id);
    }

    public void Dispose()
    {
        ReleaseUnmanagedResources();
        GC.SuppressFinalize(this);
    }

    ~Shader()
    {
        ReleaseUnmanagedResources();
    }
}