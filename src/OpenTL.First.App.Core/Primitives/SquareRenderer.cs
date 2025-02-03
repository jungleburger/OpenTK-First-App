using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System.Linq;

namespace OpenTK.First.App.Core.Primitives
{
    public class SquareRenderer
    {
        private int _vertexBufferObject;
        private int _vertexArrayObject;
        private int _shaderProgram;
        private int _modelLocation;
        private int _colorLocation;

        private readonly float[] _vertices = {
            // First triangle
            -0.1f, -0.1f,
            0.1f, -0.1f,
            0.1f,  0.1f,
            // Second triangle
            0.1f,  0.1f,
            -0.1f,  0.1f,
            -0.1f, -0.1f
        };

        private readonly string _vertexShaderSource = @"
            #version 330 core
            layout(location = 0) in vec2 aPosition;
            uniform mat4 model;
            void main()
            {
                gl_Position = model * vec4(aPosition, 0.0, 1.0);
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

        public Vector2 Position { get; set; } = new Vector2(-0.5f, 0.0f); // Initial position away from the origin
        public Vector4 Color { get; set; } = new Vector4(0.0f, 1.0f, 0.0f, 1.0f); // Default color: Green

        public Vector2[] Vertices => GetUniqueVertices();

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
            GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Length * sizeof(float), _vertices, BufferUsageHint.StaticDraw);

            // Generate and bind the vertex array object
            _vertexArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(_vertexArrayObject);

            GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 2 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            // Get the location of the model matrix and color uniform
            _modelLocation = GL.GetUniformLocation(_shaderProgram, "model");
            _colorLocation = GL.GetUniformLocation(_shaderProgram, "color");
        }

        public void Render()
        {
            GL.UseProgram(_shaderProgram);

            // Create a translation matrix to move the square
            Matrix4 model = Matrix4.CreateTranslation(new Vector3(Position.X, Position.Y, 0.0f));
            GL.UniformMatrix4(_modelLocation, false, ref model);

            // Set the color uniform
            GL.Uniform4(_colorLocation, Color);

            GL.BindVertexArray(_vertexArrayObject);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 6);
        }

        public void Cleanup()
        {
            GL.DeleteBuffer(_vertexBufferObject);
            GL.DeleteVertexArray(_vertexArrayObject);
            GL.DeleteProgram(_shaderProgram);
        }

        private Vector2[] GetUniqueVertices()
        {
            return new Vector2[]
            {
                new Vector2(-0.1f, -0.1f),
                new Vector2(0.1f, -0.1f),
                new Vector2(0.1f, 0.1f),
                new Vector2(-0.1f, 0.1f)
            };
        }
    }
}
