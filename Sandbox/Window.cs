using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Graphics.OpenGL4;

namespace Sandbox
{
    internal class Window : GameWindow
    {
        float[] vertices = {
            0.5f, 0.5f, 0.0f, 1,0,0,  // 右上角
            0.5f, -0.5f, 0.0f, 0,1,0, // 右下角
            -0.5f, -0.5f, 0.0f, 0,0,1,// 左下角
            -0.5f, 0.5f, 0.0f ,1,0,1  // 左上角
        };

        private int[] indices =
        {
            // 注意索引从0开始! 
            // 此例的索引(0,1,2,3)就是顶点数组vertices的下标，
            // 这样可以由下标代表顶点组合成矩形

            0, 1, 3, // 第一个三角形
            1, 2, 3 // 第二个三角形
        };

        private int vao;
        private int vbo;
        private int ebo;
        private int program;
        
        public Window(int width, int height, string title) : base(GameWindowSettings.Default, new NativeWindowSettings() { Size = (width, height), Title = title }) {

        }

        protected override void OnLoad()
        {
            //GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
            vao = GL.GenVertexArray();
            GL.BindVertexArray(vao);
            
            vbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices,
                BufferUsageHint.StaticDraw);

            string vertexSource = @"#version 460 core 
                layout (location = 0) in vec3 aPos;
                layout (location = 1) in vec3 aColor;
                
                layout (location = 0) out vec3 color;
                void main()
                {
                    gl_Position = vec4(aPos.x, aPos.y, aPos.z, 1.0);
                    color = aColor;
                }";
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 3 * sizeof(float));
            GL.EnableVertexAttribArray(1);
            
            int vertexShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertexShader, vertexSource);
            GL.CompileShader(vertexShader);

            string fragmentSource = @"#version 460 core
                out vec4 FragColor;
                layout (location = 0) in vec3 color;

                void main()
                {
                    FragColor = vec4(color, 1.0f);
                }";
            int fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragmentShader, fragmentSource);
            GL.CompileShader(fragmentShader);

            program = GL.CreateProgram();
            GL.AttachShader(program, vertexShader);
            GL.AttachShader(program, fragmentShader);
            GL.LinkProgram(program);

            ebo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer,ebo);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices,
                BufferUsageHint.StaticDraw);
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);
            GL.ClearColor(Color.Black);
            GL.BindVertexArray(vao);
            GL.UseProgram(program);
            //GL.DrawArrays(PrimitiveType.Triangles,0,3);
            GL.DrawElements(PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);
            SwapBuffers();
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            KeyboardState input = KeyboardState;

            if (input.IsKeyDown(Keys.Escape))
            {
                Close();
            }
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            GL.Viewport(0, 0, e.Width, e.Height);
        }
    }
}
