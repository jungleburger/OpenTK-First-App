using OpenTK.First.App.Core.Collision;
using OpenTK.First.App.Core.Primitives;
using OpenTK.Mathematics;

namespace OpenTK.First.App.Core.Diagnostics.Visualisations
{
    /// <summary>
    /// Visualizes the collision detection process using the Separating Axis Theorem (SAT).
    /// </summary>
    public class SATCollisionVisualizer
    {
        private Vector2[] _axes1 = Array.Empty<Vector2>();
        private Vector2[] _axes2 = Array.Empty<Vector2>();

        /// <summary>
        /// Renders the SAT visualization using the provided line renderer.
        /// </summary>
        /// <param name="lineRenderer">The line renderer.</param>
        /// <param name="shape1">The first collidable shape.</param>
        /// <param name="shape2">The second collidable shape.</param>
        public void RenderSATVisualization(LineRenderer lineRenderer, ICollidable shape1, ICollidable shape2)
        {
            if (lineRenderer == null) throw new ArgumentNullException(nameof(lineRenderer));
            if (shape1 == null) throw new ArgumentNullException(nameof(shape1));
            if (shape2 == null) throw new ArgumentNullException(nameof(shape2));

            // Ensure axes are up-to-date
            shape1.UpdateAxes();
            shape2.UpdateAxes();

            int shape1AxesCount = shape1.Axes.Length;
            int shape2AxesCount = shape2.Axes.Length;

            // Resize the axes arrays if necessary
            if (_axes1.Length != shape1AxesCount)
                _axes1 = new Vector2[shape1AxesCount];
            if (_axes2.Length != shape2AxesCount)
                _axes2 = new Vector2[shape2AxesCount];

            // Get the axes (normals) from both shapes
            SATCollisionDetector.GetAxes(shape1.Vertices, _axes1);
            SATCollisionDetector.GetAxes(shape2.Vertices, _axes2);

            // Render axes and projections for both shapes
            foreach (Vector2 axis in _axes1)
            {
                RenderAxisAndProjections(lineRenderer, axis, shape1.Vertices, shape2.Vertices);
            }
            foreach (Vector2 axis in _axes2)
            {
                RenderAxisAndProjections(lineRenderer, axis, shape1.Vertices, shape2.Vertices);
            }
        }

        private void RenderAxisAndProjections(LineRenderer lineRenderer, Vector2 axis, Vector2[] shape1Vertices, Vector2[] shape2Vertices)
        {
            // Draw the axis for visualization
            Vector3[] axisLine =
            {
                new Vector3(0, 0, 0),
                new Vector3(axis * 0.5f) // Scale for visualization purposes
            };
            lineRenderer.Render(axisLine, new Vector4(1.0f, 1.0f, 0.0f, 1.0f)); // Yellow color for axes

            // Draw projections for shape1
            DrawProjections(lineRenderer, axis, shape1Vertices, new Vector4(1.0f, 0.0f, 0.0f, 1.0f)); // Red color

            // Draw projections for shape2
            DrawProjections(lineRenderer, axis, shape2Vertices, new Vector4(0.5f, 0.0f, 0.5f, 1.0f)); // Purple color
        }

        private void DrawProjections(LineRenderer lineRenderer, Vector2 axis, Vector2[] vertices, Vector4 color)
        {
            foreach (Vector2 vertex in vertices)
            {
                // Project the vertex onto the axis
                float projection = Vector2.Dot(axis, vertex);
                Vector2 projectedPoint = axis * projection;

                // Draw a line from the vertex to the projected point
                Vector3[] line =
                {
                    new Vector3(vertex.X, vertex.Y, 0.0f),
                    new Vector3(projectedPoint.X, projectedPoint.Y, 0.0f)
                };
                lineRenderer.Render(line, color);
            }
        }
    }
}