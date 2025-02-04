using OpenTK.First.App.Core.Collision;
using OpenTK.First.App.Core.Control.Keyboard;
using OpenTK.First.App.Core.Diagnostics.Visualisations;
using OpenTK.First.App.Core.Objects;
using OpenTK.First.App.Core.Primitives;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace OpenTK.First.App.Core
{
	public class Game : GameWindow
	{
		private GameObjectManager _gameObjectManager;
		private MovementController _movementController;
		private CollisionManager _collisionManager;
		private SATCollisionVisualizer _satCollisionVisualizer;
		private LineRenderer _lineRenderer;

		private SquareRenderer _squareRenderer;
		private TriangleRenderer _triangleRenderer;

		public Game(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
			: base(gameWindowSettings, nativeWindowSettings)
		{
		}

		protected override void OnLoad() 
		{
			base.OnLoad();

			// Set up OpenGL settings
			GL.ClearColor(Color4.CornflowerBlue);

			// Initialize managers
			_gameObjectManager = new GameObjectManager();
			_collisionManager = new CollisionManager();
			_satCollisionVisualizer = new SATCollisionVisualizer();
			_lineRenderer = new LineRenderer();
			_lineRenderer.Initialize();

			// Initialize game objects
			InitializeGameObjects();

			// Initialize movement controller with the movable and static objects
			var staticObjects = new List<ICollidable>
			{
				_triangleRenderer
				// Add more static objects here if needed
			};

			_movementController = new MovementController(
				_squareRenderer,
				_collisionManager,
				staticObjects);

			// Initialize all game objects
			_gameObjectManager.InitializeAll();
		}

		protected override void OnUpdateFrame(FrameEventArgs args)
		{
			base.OnUpdateFrame(args);

			// Handle input and movement
			_movementController.HandleMovement(KeyboardState, args.Time);

			// Update all game objects
			_gameObjectManager.UpdateAll(args.Time);

			if (KeyboardState.IsKeyDown(Keys.Escape))
			{
				Close();
			}
		}

		protected override void OnRenderFrame(FrameEventArgs args)
		{
			base.OnRenderFrame(args);

			// Clear the screen
			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

			// Render all game objects
			_gameObjectManager.RenderAll();

			// Render collision visualization
			_satCollisionVisualizer.RenderSATVisualization(
				_lineRenderer,
				_squareRenderer,
				_triangleRenderer);

			SwapBuffers();
		}

		protected override void OnUnload()
		{
			base.OnUnload();

			// Clean up resources
			_gameObjectManager.CleanupAll();
			_lineRenderer.Cleanup();
		}

		/// <summary>
		/// Initializes the game objects and adds them to the game object manager.
		/// </summary>
		private void InitializeGameObjects()
		{
			_squareRenderer = new SquareRenderer();
			_triangleRenderer = new TriangleRenderer();

			// Set initial positions if needed
			// _squareRenderer.Position = new Vector2(...);
			// _triangleRenderer.Position = new Vector2(...);

			// Add game objects to the manager
			_gameObjectManager.Add(_squareRenderer);
			_gameObjectManager.Add(_triangleRenderer);
		}
	}
}
