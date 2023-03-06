using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Render.Resources;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Sandbox
{
    internal class Window : GameWindow
    {
        private float width;
        private float height;

        private Shader shader;
        private Texture2D texture01;
        private Model myModel;

        public Window(int width, int height, string title) : base(GameWindowSettings.Default,
            new NativeWindowSettings() { Size = (width, height), Title = title })
        {
        }

        protected override void OnLoad()
        {
            //GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);

            myModel = Model.Create(@"G:\MyGameEngine\Core\Resources\backpack.obj");
            shader = Shader.Create(@"G:\MyGameEngine\Core\Shader\Triangles.glsl");
            texture01 = Texture2D.Create(@"G:\MyGameEngine\Core\Resources\diffuse.jpg");
        }

        private Matrix4 model;
        private Matrix4 view;
        private Matrix4 perspective;

        private double time;

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit |
                     ClearBufferMask.StencilBufferBit);
            GL.ClearColor(Color.SeaGreen);
            shader.Bind();
            model = Matrix4.Identity;
            view = Matrix4.LookAt(new Vector3(0, 0, -10), Vector3.Zero, Vector3.UnitY);
            perspective =
                Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45), width / height, 0.1f, 1000f);
            shader.SetUniform("mainTex", 0);
            texture01.Bind();
            shader.SetUniform("model", model);
            shader.SetUniform("view", view);
            shader.SetUniform("perspective", perspective);

            GL.Enable(EnableCap.DepthTest);

            foreach (var mesh in myModel.Meshes)
            {
                mesh.Bind();
                if (mesh.IndexCount > 3)
                {
                    GL.DrawElements(PrimitiveType.Triangles, mesh.IndexCount, DrawElementsType.UnsignedInt, 0);
                }
                else
                {
                    GL.DrawArrays(PrimitiveType.Triangles, 0, mesh.VertexCount);
                }
            }

            time += args.Time;

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
            width = e.Width;
            height = e.Height;
        }
    }
}