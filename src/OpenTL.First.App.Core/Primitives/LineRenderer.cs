using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace OpenTK.First.App.Core.Primitives
{
    public class LineRenderer
    {
        private int _shaderProgram;
        private int _vertexBufferObject;
        private int _vertexArrayObject;
        private int _colorLocation;

        private readonly string _vertexShaderSource = @"
            #version 330 core
            layout(location = 0) in vec3 aPosition;
            void main()
            {
                gl_Position = vec4(aPosition, 1.0);
            }
        ";

        private readonly string _fragmentShaderSource = @"
            #version 330 core
            out vec4 FragColor;
            uniform vec4 color;
            void main()
            {
                FragColor = color;
            }
        ";

        public void Initialize()
        {
            // Compile shaders and link them into a program
            int vertexShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertexShader, _vertexShaderSource);
            GL.CompileShader(vertexShader);

            int fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragmentShader, _fragmentShaderSource);
            GL.CompileShader(fragmentShader);

            _shaderProgram = GL.CreateProgram();
            GL.AttachShader(_shaderProgram, vertexShader);
            GL.AttachShader(_shaderProgram, fragmentShader);
            GL.LinkProgram(_shaderProgram);

            GL.DeleteShader(vertexShader);
            GL.DeleteShader(fragmentShader);

            // Generate and bind the vertex buffer object
            _vertexBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);

            // Generate and bind the vertex array object
            _vertexArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(_vertexArrayObject);

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            // Get the location of the color uniform
            _colorLocation = GL.GetUniformLocation(_shaderProgram, "color");
        }

        public void Render(Vector3[] vertices, Vector4 color)
        {
            GL.UseProgram(_shaderProgram);
            GL.Uniform4(_colorLocation, color);

            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float) * 3, vertices, BufferUsageHint.DynamicDraw);

            GL.BindVertexArray(_vertexArrayObject);
            GL.DrawArrays(PrimitiveType.Lines, 0, vertices.Length);
        }

        public void Cleanup()
        {
            GL.DeleteBuffer(_vertexBufferObject);
            GL.DeleteVertexArray(_vertexArrayObject);
            GL.DeleteProgram(_shaderProgram);
        }
    }
}