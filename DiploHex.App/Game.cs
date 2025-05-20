using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.IO;

namespace DiploHex.App
{
    public class Game : GameWindow
    {
        public Game(int width, int height, string title)
            : base(GameWindowSettings.Default, MakeWindowSettings(width, height, title))
        {

        }

        protected override void OnLoad()
        {
            base.OnLoad();

            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);

            float[] vertices = {
                -0.5f, -0.5f, 0.0f, //Bottom-left vertex
                 0.5f, -0.5f, 0.0f, //Bottom-right vertex
                 0.0f,  0.5f, 0.0f  //Top vertex
            };

            Shader = new ShaderBuilder()
                .AddVertexFile("Shaders/vertex.glsl")
                .AddFragmentFile("Shaders/fragment.glsl")
                .Build();

            VertexBufferObject = GL.GenBuffer();
            VertexArrayObject = GL.GenVertexArray();

            GL.BindVertexArray(VertexArrayObject);

            GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            Shader.Use();
            OffsetLocation = Shader.GetUniformLocation("aOffset");
            GL.Uniform3(OffsetLocation, 0.0f, 0.0f, 0.0f);
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            if (KeyboardState.IsKeyDown(Keys.Escape))
            {
                Close();
            }
            else if (KeyboardState.IsKeyDown(Keys.R))
            {
                RenderOffsetBase = Vector2.Zero;
                DragStartMousePosition = LastMousePosition;
            }
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            GL.Clear(ClearBufferMask.ColorBufferBit);

            Shader.Use();
            GL.Uniform3(OffsetLocation, RenderOffset.X, RenderOffset.Y, 0.0f);

            GL.BindVertexArray(VertexArrayObject);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 3);

            SwapBuffers();
        }

        protected override void OnFramebufferResize(FramebufferResizeEventArgs e)
        {
            base.OnFramebufferResize(e);

            GL.Viewport(0, 0, e.Width, e.Height);
        }

        protected override void OnUnload()
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.DeleteBuffer(VertexBufferObject);

            Shader?.Dispose();

            base.OnUnload();
        }

        private bool IsMouseDragging { get; set; }

        private Vector2 RenderOffsetBase { get; set; }

        private Vector2 RenderOffset =>
            IsMouseDragging
            ? RenderOffsetBase + (LastMousePosition - DragStartMousePosition)
            : RenderOffsetBase;

        private Vector2 LastMousePosition { get; set; }

        private Vector2 DragStartMousePosition { get; set; }

        protected override void OnMouseMove(MouseMoveEventArgs e)
        {
            base.OnMouseMove(e);
            LastMousePosition = (2 * MousePosition.X / FramebufferSize.X, -2 * MousePosition.Y / FramebufferSize.Y);
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            DragStartMousePosition = LastMousePosition;
            IsMouseDragging = true;
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            RenderOffsetBase = RenderOffset;
            IsMouseDragging = false;
        }

        private int VertexBufferObject { get; set; }

        private int VertexArrayObject { get; set; }

        private Shader Shader { get; set; }

        private int OffsetLocation { get; set; }

        private static NativeWindowSettings MakeWindowSettings(int width, int height, string title)
        {
            return new NativeWindowSettings()
            {
                ClientSize = (width, height),
                Title = title
            };
        }
    }
}
