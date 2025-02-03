using OpenTK.First.App.Core.Collision;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System.Drawing;
using System.Linq;

namespace OpenTK.First.App.Core.Primitives
{
    public class SquareRenderer : ICollidable
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

        private Vector2 _position;
        public Vector2 Position
        {
            get => _position;
            set
            {
                if (_position != value)
                {
                    _position = value;
                    IsDirty = true; // Mark as dirty when position changes
                }
            }
        }
        
        public Vector4 Color { get; set; } = new Vector4(0.0f, 1.0f, 0.0f, 1.0f); // Default color: Green
        public RectangleF BoundingBox => ComputeBoundingBox();
        public bool IsStatic { get; } // True if the object doesn't move

        public Vector2[] Vertices => GetTransformedVertices();

        private Vector2[] _axes;
        public Vector2[] Axes
        {
            get
            {
                UpdateAxes();
                return _axes;
            }
        }

        public bool IsDirty { get; private set; } = true;

        public void Move(Vector2 newPosition)
        {
            Position = newPosition;
            IsDirty = true;
        }

        public void UpdateAxes()
        {
            if (IsDirty)
            {
                int numVertices = Vertices.Length;
                if (_axes == null || _axes.Length != numVertices)
                    _axes = new Vector2[numVertices];

                SATCollisionDetector.GetAxes(Vertices, _axes);
                IsDirty = false;
            }
        }

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

            // Display the vertices in the console
            DisplayVertices();
        }

        public void Cleanup()
        {
            GL.DeleteBuffer(_vertexBufferObject);
            GL.DeleteVertexArray(_vertexArrayObject);
            GL.DeleteProgram(_shaderProgram);
        }

        public Vector2[] GetTransformedVerticesAtPosition(Vector2 position)
        {
            Vector2[] transformedVertices = new Vector2[_vertices.Length / 2];

            // Create the model matrix (same as in the shader)
            Matrix4 model = Matrix4.CreateTranslation(new Vector3(position.X, position.Y, 0.0f));

            for (int i = 0; i < _vertices.Length; i += 2)
            {
                // Original vertex
                Vector4 vertex = new Vector4(_vertices[i], _vertices[i + 1], 0.0f, 1.0f);

                // Transform the vertex using the model matrix
                Vector4 transformedVertex = Vector4.TransformRow(vertex, model);

                // Store the transformed vertex
                transformedVertices[i / 2] = new Vector2(transformedVertex.X, transformedVertex.Y);
            }

            return transformedVertices;
        }


        private Vector2[] GetTransformedVertices()
        {
            Vector2[] transformedVertices = new Vector2[_vertices.Length / 2];

            // Create the model matrix (same as in the shader)
            Matrix4 model = Matrix4.CreateTranslation(new Vector3(Position.X, Position.Y, 0.0f));

            for (int i = 0; i < _vertices.Length; i += 2)
            {
                // Original vertex
                Vector4 vertex = new Vector4(_vertices[i], _vertices[i + 1], 0.0f, 1.0f);

                // Transform the vertex using the model matrix
                Vector4 transformedVertex = Vector4.TransformRow(vertex, model);

                // Store the transformed vertex
                transformedVertices[i / 2] = new Vector2(transformedVertex.X, transformedVertex.Y);
            }

            return transformedVertices;
        }

        private void DisplayVertices()
        {
            Vector2[] transformedVertices = GetTransformedVertices();
            Console.WriteLine("Square Vertices:");
            foreach (var vertex in transformedVertices)
            {
                Console.WriteLine($"({vertex.X}, {vertex.Y})");
            }
        }
        private RectangleF ComputeBoundingBox()
        {
            Vector2[] vertices = GetTransformedVertices();
            float minX = vertices.Min(v => v.X);
            float maxX = vertices.Max(v => v.X);
            float minY = vertices.Min(v => v.Y);
            float maxY = vertices.Max(v => v.Y);
            return new RectangleF(minX, minY, maxX - minX, maxY - minY);
        }
    }
}
