using OpenTK.Mathematics;

namespace OpenTK.First.App.Core.Collision
{
	/// <summary>
	/// Detects collisions between two shapes using the Separating Axis Theorem.
	/// </summary>
	public class SATCollisionDetector
	{
		// Preallocated arrays to minimize memory allocations
		private Vector2[] _axes1 = Array.Empty<Vector2>();
		private Vector2[] _axes2 = Array.Empty<Vector2>();

		/// <summary>
		/// Determines if two shapes are colliding.
		/// </summary>
		/// <param name="shape1">The first shape.</param>
		/// <param name="shape2">The second shape.</param>
		/// <returns>True if the shapes are colliding; otherwise, false.</returns>
		public bool IsColliding(ICollidable shape1, ICollidable shape2)
		{
			if (shape1 == null) throw new ArgumentNullException(nameof(shape1));
			if (shape2 == null) throw new ArgumentNullException(nameof(shape2));

			// Ensure axes are up-to-date
			shape1.UpdateAxes();
			shape2.UpdateAxes();

			// Check for separation on shape1's axes
			foreach (Vector2 axis in shape1.Axes)
			{
				if (!IsOverlapOnAxis(axis, shape1.Vertices, shape2.Vertices))
				{
					return false;
				}
			}

			// Check for separation on shape2's axes
			foreach (Vector2 axis in shape2.Axes)
			{
				if (!IsOverlapOnAxis(axis, shape1.Vertices, shape2.Vertices))
				{
					return false;
				}
			}

			return true;
		}

		public static void GetAxes(Vector2[] vertices, Vector2[] axes)
		{
			int numVertices = vertices.Length;

			for (int i = 0; i < numVertices; i++)
			{
				// Current and next vertex
				Vector2 p1 = vertices[i];
				Vector2 p2 = vertices[(i + 1) % numVertices];

				// Edge vector
				Vector2 edge = p2 - p1;

				// Perpendicular (normal) vector
				Vector2 normal = new Vector2(-edge.Y, edge.X);

				// Normalize the axis to avoid distortion in projections
				normal.Normalize();

				axes[i] = normal;
			}
		}

		private static bool IsOverlapOnAxis(Vector2 axis, Vector2[] shape1Vertices, Vector2[] shape2Vertices)
		{
			// Project both shapes onto the axis
			GetProjection(axis, shape1Vertices, out float min1, out float max1);
			GetProjection(axis, shape2Vertices, out float min2, out float max2);

			// Check for gap between projections
			return !(max1 < min2 || max2 < min1);
		}

		private static void GetProjection(Vector2 axis, Vector2[] vertices, out float min, out float max)
		{
			// Project the vertices onto the axis
			float projection = Vector2.Dot(axis, vertices[0]);
			min = max = projection;

			for (int i = 1; i < vertices.Length; i++)
			{
				projection = Vector2.Dot(axis, vertices[i]);

				if (projection < min)
				{
					min = projection;
				}
				else if (projection > max)
				{
					max = projection;
				}
			}
		}
	}
}
