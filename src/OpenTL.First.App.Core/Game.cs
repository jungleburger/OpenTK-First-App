using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.First.App.Core.Primitives;
using OpenTK.First.App.Core.Collision;

namespace OpenTK.First.App.Core
{
    public class Game : GameWindow
    {
        private SquareRenderer _squareRenderer;
        private TriangleRenderer _triangleRenderer;
        private LineRenderer _lineRenderer;
        private SATCollisionDetector _satCollisionDetector;

        public Game(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
            : base(gameWindowSettings, nativeWindowSettings)
        {
        }

        protected override void OnLoad()
        {
            base.OnLoad();

            // Set up OpenGL settings here
            GL.ClearColor(Color4.CornflowerBlue);

            // Initialize the square and triangle renderers
            _squareRenderer = new SquareRenderer();
            _squareRenderer.Initialize();

            _triangleRenderer = new TriangleRenderer();
            _triangleRenderer.Initialize();

            // Initialize the line renderer
            _lineRenderer = new LineRenderer();
            _lineRenderer.Initialize();

            // Initialize the SAT collision detector
            _satCollisionDetector = new SATCollisionDetector();
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);

            // Render the blank screen
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            // Render the square and triangle
            _squareRenderer.Render();
            _triangleRenderer.Render();

            // Render the SAT visualization (optional)
            _satCollisionDetector.RenderSATVisualization(_lineRenderer, _squareRenderer.Vertices, _triangleRenderer.Vertices);
            
            SwapBuffers();
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);

            // Handle input and update game state here
            const float moveSpeed = 0.5f; // Adjust this value to control the movement speed

            float deltaTime = (float)args.Time; // Time elapsed since the last frame

            Vector2 newPosition = _squareRenderer.Position;

            if (KeyboardState.IsKeyDown(Keys.W))
            {
                newPosition += new Vector2(0.0f, moveSpeed * deltaTime);
            }
            if (KeyboardState.IsKeyDown(Keys.S))
            {
                newPosition += new Vector2(0.0f, -moveSpeed * deltaTime);
            }
            if (KeyboardState.IsKeyDown(Keys.A))
            {
                newPosition += new Vector2(-moveSpeed * deltaTime, 0.0f);
            }
            if (KeyboardState.IsKeyDown(Keys.D))
            {
                newPosition += new Vector2(moveSpeed * deltaTime, 0.0f);
            }

            // Get the transformed vertices at the new position
            Vector2[] newSquareVertices = _squareRenderer.GetTransformedVerticesAtPosition(newPosition);

            // Generate the axes for the new vertices
            Vector2[] newAxes = new Vector2[newSquareVertices.Length];
            SATCollisionDetector.GetAxes(newSquareVertices, newAxes);

            // Create a temporary ICollidable representing the square at the new position
            ICollidable tempSquare = new TemporaryCollidable(newSquareVertices, newAxes);

            // Ensure the triangle's axes are up-to-date
            _triangleRenderer.UpdateAxes();

            // Check for collision
            if (_satCollisionDetector.IsColliding(tempSquare, _triangleRenderer))
            {
                _squareRenderer.Color = new Vector4(1.0f, 0.0f, 0.0f, 1.0f); // Red color on collision
            }
            else
            {
                _squareRenderer.Color = new Vector4(0.0f, 1.0f, 0.0f, 1.0f); // Green color when no collision
                _squareRenderer.Position = newPosition; // Move the square
                // No need to set IsDirty here; Position setter handles it
            }

            if (KeyboardState.IsKeyDown(Keys.Escape))
            {
                Close();
            }
        }

        protected override void OnUnload()
        {
            base.OnUnload();

            // Clean up resources
            _squareRenderer.Cleanup();
            _triangleRenderer.Cleanup();
            _lineRenderer.Cleanup();
        }
    }
}
