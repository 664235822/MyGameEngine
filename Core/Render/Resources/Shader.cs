using Core.Render.Geometry;
using Core.Render.Log;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using Vector2 = OpenTK.Mathematics.Vector2;
using Vector3 = OpenTK.Mathematics.Vector3;
using Vector4 = OpenTK.Mathematics.Vector4;

namespace Core.Render.Resources;

public class Shader : IDisposable
{
    public static Shader Create(string path)
    {
        Shader? shader = null;
        try
        {
            shader = new Shader(path);
        }
        catch (Exception e)
        {
            LogManager.ErrorLogCore(e.Message);
        }

        return shader;
    }

    public static Shader Create(string vertexShaderSource, string fragmentShaderSource) =>
        new Shader(vertexShaderSource, fragmentShaderSource);

    public int Id { get; private set; }

    public string? Path { get; }

    public Dictionary<string, int> cache { get; } = new Dictionary<string, int>();

    public List<UniformInfo> UniformInfos { get; } = new List<UniformInfo>();

    public bool IsDestroy { get; private set; } = false;

    private Shader(string path)
    {
        Path = path;
        (string vertexShaderSource, string fragmentShaderSource) = LoadShaderFromPath(path);

        CreateProgram(vertexShaderSource, fragmentShaderSource);
        QueryUniforms();
    }

    private Shader(string vertexShaderSource, string fragmentShaderSource)
    {
        CreateProgram(vertexShaderSource, fragmentShaderSource);
        QueryUniforms();
    }

    public void Bind()
    {
        GL.UseProgram(Id);
    }

    public void UnBind()
    {
        GL.UseProgram(0);
    }

    public void SetUniform(string name, int v) => GL.Uniform1(GetUniformLocation(name), v);

    public void SetUniform(string name, float v) => GL.Uniform1(GetUniformLocation(name), v);

    public void SetUniform(string name, Vector2 v) => GL.Uniform2(GetUniformLocation(name), v);

    public void SetUniform(string name, Vector3 v) => GL.Uniform3(GetUniformLocation(name), v);

    public void SetUniform(string name, Vector4 v) => GL.Uniform4(GetUniformLocation(name), v);

    public void SetUniform(string name, Matrix2 v) => GL.UniformMatrix2(GetUniformLocation(name), true, ref v);

    public void SetUniform(string name, Matrix3 v) => GL.UniformMatrix3(GetUniformLocation(name), true, ref v);

    public void SetUniform(string name, Matrix4 v) => GL.UniformMatrix4(GetUniformLocation(name), true, ref v);

    public void GetUniform(string name, out int v) => GL.GetUniform(Id, GetUniformLocation(name), out v);

    public void GetUniform(string name, out float v) => GL.GetUniform(Id, GetUniformLocation(name), out v);

    public void GetUniform(string name, out Vector2 v)
    {
        float[] res = new float[2];
        GL.GetUniform(Id, GetUniformLocation(name), res);
        v = new Vector2(res[0], res[1]);
    }

    public void GetUniform(string name, out Vector3 v)
    {
        float[] res = new float[3];
        GL.GetUniform(Id, GetUniformLocation(name), res);
        v = new Vector3(res[0], res[1], res[2]);
    }

    public void GetUniform(string name, out Vector4 v)
    {
        float[] res = new float[4];
        GL.GetUniform(Id, GetUniformLocation(name), res);
        v = new Vector4(res[0], res[1], res[2], res[3]);
    }

    public void GetUniform(string name, out Matrix2 v)
    {
        float[] res = new float[4];
        GL.GetUniform(Id, GetUniformLocation(name), res);
        v = new Matrix2(res[0], res[1], res[2], res[3]);
    }

    public void GetUniform(string name, out Matrix3 v)
    {
        float[] res = new float[9];
        GL.GetUniform(Id, GetUniformLocation(name), res);
        v = new Matrix3(res[0], res[1], res[2], res[3], res[4], res[5], res[6], res[7], res[8]);
    }

    public void GetUniform(string name, out Matrix4 v)
    {
        float[] res = new float[16];
        GL.GetUniform(Id, GetUniformLocation(name), res);
        v = new Matrix4(res[0], res[1], res[2], res[3], res[4], res[5], res[6], res[7], res[8], res[9], res[10],
            res[11], res[12], res[13], res[14], res[15]);
    }

    public void ReCompile()
    {
        if (Path != null)
        {
            GL.DeleteProgram(Id);

            (string vertexShaderSource, string fragmentShaderSource) = LoadShaderFromPath(Path);
            CreateProgram(vertexShaderSource, fragmentShaderSource);
            QueryUniforms();
        }
    }

    public void QueryUniforms()
    {
        GL.GetProgram(Id, GetProgramParameterName.ActiveUniforms, out int uniformCount);
        for (int i = 0; i < uniformCount; i++)
        {
            GL.GetActiveUniform(Id, i, 1024, out _, out _, out ActiveUniformType uniformType, out string name);
            object? value = null;
            switch (uniformType)
            {
                case ActiveUniformType.Int:
                    int v;
                    GetUniform(name, out v);
                    value = v;
                    break;
                case ActiveUniformType.Float:
                    float v1;
                    GetUniform(name, out v1);
                    value = v1;
                    break;
                case ActiveUniformType.Bool:
                    int v2;
                    GetUniform(name, out v2);
                    value = v2;
                    break;
                case ActiveUniformType.FloatVec2:
                    Vector2 v3 = new Vector2();
                    GetUniform(name, out v3);
                    value = v3;
                    break;
                case ActiveUniformType.FloatVec3:
                    Vector3 v4 = new Vector3();
                    GetUniform(name, out v4);
                    value = v4;
                    break;
                case ActiveUniformType.FloatVec4:
                    Vector4 v5 = new Vector4();
                    GetUniform(name, out v5);
                    value = v5;
                    break;
                case ActiveUniformType.FloatMat3:
                    Matrix3 v6 = new Matrix3();
                    GetUniform(name, out v6);
                    value = v6;
                    break;
                case ActiveUniformType.FloatMat4:
                    Matrix4 v7 = new Matrix4();
                    GetUniform(name, out v7);
                    value = v7;
                    break;
                case ActiveUniformType.Sampler2D:
                    break;
            }

            UniformInfos.Add(new UniformInfo(name, GetUniformLocation(name), uniformType, value));
        }
    }

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
            LogManager.ErrorLogCore($"{info}");
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
            LogManager.ErrorLogCore($"{type.ToString()}\t{info}");
        }

        return id;
    }

    private void ReleaseUnmanagedResources()
    {
        GL.DeleteProgram(Id);
    }

    public void Dispose()
    {
        if (!IsDestroy)
        {
            ReleaseUnmanagedResources();
            GC.SuppressFinalize(this);

            IsDestroy = true;
        }
    }

    ~Shader()
    {
        ReleaseUnmanagedResources();
    }
}