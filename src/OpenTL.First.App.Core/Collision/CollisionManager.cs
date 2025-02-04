namespace OpenTK.First.App.Core.Collision
{
	public class CollisionManager
	{
		private readonly SATCollisionDetector _satCollisionDetector;

		public CollisionManager()
		{
			_satCollisionDetector = new SATCollisionDetector();
		}

		/// <summary>
		/// Checks if the movable object collides with any of the static objects.
		/// </summary>
		/// <param name="movable">The movable ICollidable object.</param>
		/// <param name="staticObjects">Collection of static ICollidable objects.</param>
		/// <returns>True if a collision is detected; otherwise, false.</returns>
		public bool CheckCollisions(ICollidable movable, IEnumerable<ICollidable> staticObjects)
		{
			// Update axes if the movable object has changed
			if (movable.IsDirty)
			{
				movable.UpdateAxes();
				movable.IsDirty = false;
			}

			foreach (var staticObject in staticObjects)
			{
				// Ensure the static object's axes are up-to-date
				if (staticObject.IsDirty)
				{
					staticObject.UpdateAxes();
					staticObject.IsDirty = false;
				}

				// Check for collision
				if (_satCollisionDetector.IsColliding(movable, staticObject))
				{
					return true;
				}
			}

			return false;
		}
	}
}