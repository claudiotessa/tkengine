using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace tkengine.src;
internal class Game : GameWindow {
    private int _width;
    private int _height;

    float[] vertices = {
        0f, .5f, 0f, // top vertex
        -.5f, -.5f, 0f, // bottom left
        .5f, -.5f, 0f // bottom right
    };

    // render pipeline vars
    int vao;
    int shaderProgram;

    public Game(int width, int height) : base(GameWindowSettings.Default, NativeWindowSettings.Default) {
        //center the window on monitor
        this.CenterWindow(new Vector2i(width, height));

        _width = width;
        _height = height;
    }

    protected override void OnResize(ResizeEventArgs e) {
        base.OnResize(e);
        GL.Viewport(0, 0, e.Width, e.Height);
        _width = e.Width;
        _height = e.Height;
    }

    protected override void OnLoad() {
        base.OnLoad();

        vao = GL.GenVertexArray();

        int vbo = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
        GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

        // bind the vao
        GL.BindVertexArray(vao);
        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0);
        GL.EnableVertexArrayAttrib(vao, 0);

        GL.BindBuffer(BufferTarget.ArrayBuffer, 0); // unbinding the vbo
        GL.BindVertexArray(0);

        // create the shader program
        shaderProgram = GL.CreateProgram();

        int vertexShader = GL.CreateShader(ShaderType.VertexShader);
        GL.ShaderSource(vertexShader, LoadShaderSource("Default.vert"));
        GL.CompileShader(vertexShader);

        int fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
        GL.ShaderSource(fragmentShader, LoadShaderSource("Default.frag"));
        GL.CompileShader(fragmentShader);

        GL.AttachShader(shaderProgram, vertexShader);
        GL.AttachShader(shaderProgram, fragmentShader);

        GL.LinkProgram(shaderProgram);

        // delete the shaders
        GL.DeleteShader(vertexShader);
        GL.DeleteShader(fragmentShader);
    }

    protected override void OnUnload() {
        base.OnUnload();

        GL.DeleteVertexArray(vao);
        GL.DeleteProgram(shaderProgram);
    }

    protected override void OnRenderFrame(FrameEventArgs args) {
        GL.ClearColor(0.6f, 0.3f, 1f, 1f);
        GL.Clear(ClearBufferMask.ColorBufferBit);

        // draw our triangle
        GL.UseProgram(shaderProgram);
        GL.BindVertexArray(vao);
        GL.DrawArrays(PrimitiveType.Triangles, 0, 3);


        Context.SwapBuffers();

        base.OnRenderFrame(args);
    }

    protected override void OnUpdateFrame(FrameEventArgs args) {
        base.OnUpdateFrame(args);
    }

    public static string LoadShaderSource(string filePath) {
        string shaderSource = "";

        try {
            using (StreamReader reader = new StreamReader("../../../shaders/" + filePath)) {
                shaderSource = reader.ReadToEnd();
            }
        } catch (Exception e) {
            Console.WriteLine("Failed to load shader source file: " + e.Message);
        }

        return shaderSource;
    }
}
