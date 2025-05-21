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

        private static readonly Color4 ClearColor = new Color4(0.2f, 0.3f, 0.3f, 1.0f);
        private static readonly Color4 FillColor = new Color4(1.0f, 0.5f, 0.2f, 1.0f);

        public float Zoom { get; private set; } = 1.0f;

        public float ViewportAspectRatio { get; private set; } = 1.0f;

        private static readonly Vector3[] vertices = [
            new(-0.5f, -0.5f, 0.0f),
            new( 0.5f, -0.5f, 0.0f),
            new( 0.0f,  0.5f, 0.0f),
        ];

        protected override void OnLoad()
        {
            base.OnLoad();

            UpdateViewport(FramebufferSize.X, FramebufferSize.Y);

            GL.ClearColor(ClearColor);

            Shader = new ShaderBuilder()
                .AddVertexFile("Shaders/vertex.glsl")
                .AddFragmentFile("Shaders/fragment.glsl")
                .Build();

            VertexBufferObject = GL.GenBuffer();
            VertexArrayObject = GL.GenVertexArray();

            GL.BindVertexArray(VertexArrayObject);

            GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * Vector3.SizeInBytes, vertices, BufferUsageHint.StaticDraw);

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            Shader.Use();

            TransformLocation = Shader.GetUniformLocation("uTransform");
            ColorLocation = Shader.GetUniformLocation("uColor");
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
                Zoom = 1.0f;
                DragStartMousePosition = LastMousePosition;
            }
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            // Clear the screen
            GL.Clear(ClearBufferMask.ColorBufferBit);

            // Activate shader
            Shader.Use();

            // Set transformation matrix
            Matrix4 transform =
                Matrix4.CreateScale(1.0f / ViewportAspectRatio, 1.0f, 1.0f) * // Maintain aspect ratio
                Matrix4.CreateScale(Zoom) * // Apply zoom
                Matrix4.CreateTranslation(new Vector3(RenderOffset.X, RenderOffset.Y, 0.0f)); // Apply offset
            GL.UniformMatrix4(TransformLocation, false, ref transform);

            // Set fill color
            GL.Uniform4(ColorLocation, FillColor);

            // Draw objects
            GL.BindVertexArray(VertexArrayObject);
            GL.DrawArrays(PrimitiveType.Triangles, 0, vertices.Length);

            // Swap buffers
            SwapBuffers();
        }

        protected override void OnFramebufferResize(FramebufferResizeEventArgs e)
        {
            base.OnFramebufferResize(e);

            UpdateViewport(e.Width, e.Height);
        }

        protected override void OnUnload()
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.DeleteBuffer(VertexBufferObject);

            Shader?.Dispose();

            base.OnUnload();
        }

        private void UpdateViewport(int width, int height)
        {
            ViewportAspectRatio = (float)width / height;
            GL.Viewport(0, 0, width, height);
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

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);
            float zoomSpeed = 0.1f;
            Zoom *= 1.0f + e.OffsetY * zoomSpeed;
            Zoom = Math.Clamp(Zoom, 0.1f, 10.0f);
        }

        private int VertexBufferObject { get; set; }

        private int VertexArrayObject { get; set; }

        private Shader Shader { get; set; }

        private int TransformLocation { get; set; }

        private int ColorLocation { get; set; }

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
