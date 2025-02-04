using OpenTK.First.App.Core.Collision;
using OpenTK.First.App.Core.Primitives;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace OpenTK.First.App.Core.Control.Keyboard
{
	public class MovementController
	{
		private readonly SquareRenderer _squareRenderer;
		private readonly CollisionManager _collisionManager;
		private readonly IEnumerable<ICollidable> _staticObjects;

		private const float MoveSpeed = 0.5f; // Movement speed

		public MovementController(
			SquareRenderer squareRenderer,
			CollisionManager collisionManager,
			IEnumerable<ICollidable> staticObjects)
		{
			_squareRenderer = squareRenderer;
			_collisionManager = collisionManager;
			_staticObjects = staticObjects;
		}

		/// <summary>
		/// Handles user input and moves the square accordingly.
		/// </summary>
		/// <param name="keyboardState">Current keyboard state.</param>
		/// <param name="deltaTime">Time elapsed since the last frame.</param>
		public void HandleMovement(KeyboardState keyboardState, double deltaTime)
		{
			Vector2 direction = Vector2.Zero;

			if (keyboardState.IsKeyDown(Keys.W))
			{
				direction.Y += 1.0f;
			}
			if (keyboardState.IsKeyDown(Keys.S))
			{
				direction.Y -= 1.0f;
			}
			if (keyboardState.IsKeyDown(Keys.A))
			{
				direction.X -= 1.0f;
			}
			if (keyboardState.IsKeyDown(Keys.D))
			{
				direction.X += 1.0f;
			}

			if (direction != Vector2.Zero)
			{
				direction.Normalize();
				Vector2 previousPosition = _squareRenderer.Position;
				Vector2 newPosition = previousPosition + direction * MoveSpeed * (float)deltaTime;

				// Move to the tentative position
				_squareRenderer.Position = newPosition;
				_squareRenderer.IsDirty = true;

				// Check for collisions
				if (_collisionManager.CheckCollisions(_squareRenderer, _staticObjects))
				{
					// Collision detected, revert to previous position
					_squareRenderer.Position = previousPosition;
					_squareRenderer.IsDirty = true;

					// Update color to red on collision
					_squareRenderer.Color = new Vector4(1.0f, 0.0f, 0.0f, 1.0f);
				}
				else
				{
					// No collision, update color to green
					_squareRenderer.Color = new Vector4(0.0f, 1.0f, 0.0f, 1.0f);
				}
			}
			else
			{
				// Update collision color when not moving
				if (_collisionManager.CheckCollisions(_squareRenderer, _staticObjects))
				{
					_squareRenderer.Color = new Vector4(1.0f, 0.0f, 0.0f, 1.0f);
				}
				else
				{
					_squareRenderer.Color = new Vector4(0.0f, 1.0f, 0.0f, 1.0f);
				}
			}
		}
	}
}
