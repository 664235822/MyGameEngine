using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Render;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using StbImageSharp;

namespace Sandbox
{
    internal class Window : GameWindow
    {
        private float[] vertices =
        {
            0.5f, 0.5f, 0.0f, 1.0f, 0.0f, 0.0f, 1.0f, 1.0f, // 右上
            0.5f, -0.5f, 0.0f, 0.0f, 1.0f, 0.0f, 1.0f, 0.0f, // 右下
            -0.5f, -0.5f, 0.0f, 0.0f, 0.0f, 1.0f, 0.0f, 0.0f, // 左下
            -0.5f, 0.5f, 0.0f, 1.0f, 1.0f, 0.0f, 0.0f, 1.0f
        };

        private uint[] indices =
        {
            // 注意索引从0开始! 
            // 此例的索引(0,1,2,3)就是顶点数组vertices的下标，
            // 这样可以由下标代表顶点组合成矩形

            0, 1, 3, // 第一个三角形
            1, 2, 3 // 第二个三角形
        };

        private VertexArrayObject vao;
        private VertexBufferObject vbo;
        private IndexBufferObject ebo;
        private Shader shader;
        private Texture2D texture01;
        private Texture2D texture02;
        public Window(int width, int height, string title) : base(GameWindowSettings.Default,
            new NativeWindowSettings() { Size = (width, height), Title = title })
        {
        }

        protected override void OnLoad()
        {
            //GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);

            vbo = new VertexBufferObject(vertices);
            VertexBufferLayout layout = new VertexBufferLayout();
            layout.AddElement(new VertexBufferLayoutElement(0, 3),
                new VertexBufferLayoutElement(1,3),
                new VertexBufferLayoutElement(2,2));
            vbo.AddLayout(layout);
            ebo = new IndexBufferObject(indices);
            vao = new VertexArrayObject(ebo, vbo);
            shader = new Shader(@"G:\MyGameEngine\Core\Shader\Triangles.glsl");
            texture01 = new Texture2D(@"G:\MyGameEngine\Core\Image\SmallPig1997.jpg");
            //texture02 = new Texture2D(@"G:\MyGameEngine\Core\Image\Justin Timberlake.png");
            texture02 = new Texture2D(Color4.Red);
        }

        private double totalTime;

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);
            GL.ClearColor(Color.Black);
            vao.Bind();
            shader.Bind();
            shader.SetUniform("color",
                new Vector3(MathF.Sin((float)totalTime), MathF.Cos((float)totalTime), MathF.Atan((float)totalTime)));
            shader.SetUniform("mainTex", 0);
            texture01.Bind();
            shader.SetUniform("subTex", 1);
            texture02.Bind(1);
            
            if (vao.IndexBufferObject == null)
            {
                GL.DrawArrays(PrimitiveType.Triangles, 0, 3);
            }
            else
            {
                GL.DrawElements(PrimitiveType.Triangles, ebo.Length, DrawElementsType.UnsignedInt, 0);
            }

            totalTime += args.Time;
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