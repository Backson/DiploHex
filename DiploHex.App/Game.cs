using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

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

            Shader = MakeShader(VertexShaderSource, FragmentShaderSource);
            VertexBufferObject = GL.GenBuffer();
            VertexArrayObject = GL.GenVertexArray();

            GL.BindVertexArray(VertexArrayObject);

            GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            // Set the initial value of the uniform offset
            GL.UseProgram(Shader);
            OffsetLocation = GL.GetUniformLocation(Shader, "aOffset");
            GL.Uniform3(OffsetLocation, 0.0f, 0.0f, 0.0f);
        }

        private static int MakeVertexShader(string vertexShaderSource)
        {
            int shader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(shader, vertexShaderSource);

            GL.CompileShader(shader);

            GL.GetShader(shader, ShaderParameter.CompileStatus, out int success);
            if (success == 0)
            {
                string infoLog = GL.GetShaderInfoLog(shader);
                Console.WriteLine(infoLog);
            }

            return shader;
        }

        private static int MakeFragmentShader(string fragmentShaderSource)
        {
            int shader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(shader, fragmentShaderSource);

            GL.CompileShader(shader);

            GL.GetShader(shader, ShaderParameter.CompileStatus, out int success);
            if (success == 0)
            {
                string infoLog = GL.GetShaderInfoLog(shader);
                Console.WriteLine(infoLog);
            }

            return shader;
        }

        private static int MakeShader(string vertexShaderSource, string fragmentShaderSource)
        {
            int VertexShader = MakeVertexShader(vertexShaderSource);
            int FragmentShader = MakeFragmentShader(fragmentShaderSource);

            int shader = GL.CreateProgram();

            GL.AttachShader(shader, VertexShader);
            GL.AttachShader(shader, FragmentShader);

            GL.LinkProgram(shader);

            GL.GetProgram(shader, GetProgramParameterName.LinkStatus, out int success);
            if (success == 0)
            {
                string infoLog = GL.GetProgramInfoLog(shader);
                Console.WriteLine(infoLog);
            }

            GL.DetachShader(shader, VertexShader);
            GL.DetachShader(shader, FragmentShader);

            GL.DeleteShader(FragmentShader);
            GL.DeleteShader(VertexShader);

            return shader;
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            if (KeyboardState.IsKeyDown(Keys.Escape))
            {
                Close();
            }

            // Update the uniform offset dynamically if needed
            GL.UseProgram(Shader);
            GL.Uniform3(OffsetLocation, RenderOffset.X, RenderOffset.Y, 0.0f);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            GL.Clear(ClearBufferMask.ColorBufferBit);

            GL.UseProgram(Shader);

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

        private int Shader { get; set; }

        private int OffsetLocation { get; set; }

        private string VertexShaderSource => @"
            #version 330 core
            layout (location = 0) in vec3 aPosition;

            uniform vec3 aOffset;

            void main()
            {
                gl_Position = vec4(aPosition + aOffset, 1.0);
            }
        ";

        private string FragmentShaderSource => @"
            #version 330 core
            out vec4 FragColor;

            void main()
            {
                FragColor = vec4(1.0f, 0.5f, 0.2f, 1.0f);
            }
        ";

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
