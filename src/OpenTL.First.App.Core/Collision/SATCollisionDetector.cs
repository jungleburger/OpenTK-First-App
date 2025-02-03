using OpenTK.First.App.Core.Primitives;
using OpenTK.Mathematics;
using System.Collections.Generic;
using System.Linq;

namespace OpenTK.First.App.Core.Collision
{
    public class SATCollisionDetector
    {
        private List<Vector2> _axes;
        private List<(float min, float max)> _projections1;
        private List<(float min, float max)> _projections2;

        public SATCollisionDetector()
        {
            _axes = new List<Vector2>();
            _projections1 = new List<(float min, float max)>();
            _projections2 = new List<(float min, float max)>();
        }

        public bool IsColliding(Vector2 newPosition, Vector2[] squareVertices, Vector2[] triangleVertices)
        {
            Vector2[] newSquareVertices = new Vector2[squareVertices.Length];
            Vector2 originalPosition = squareVertices[0]; // Assuming the first vertex is the reference point

            for (int i = 0; i < squareVertices.Length; i++)
            {
                newSquareVertices[i] = newPosition + (squareVertices[i] - originalPosition);
            }

            return SATCollision(newSquareVertices, triangleVertices);
        }

        private void GenerateAxesAndProjections(Vector2[] shape1, Vector2[] shape2)
        {
            _axes.Clear();
            _projections1.Clear();
            _projections2.Clear();

            Vector2[] axes = GetAxes(shape1).Concat(GetAxes(shape2)).ToArray();
            _axes.AddRange(axes);

            foreach (Vector2 axis in axes)
            {
                (float min1, float max1) = ProjectShape(axis, shape1);
                (float min2, float max2) = ProjectShape(axis, shape2);

                _projections1.Add((min1, max1));
                _projections2.Add((min2, max2));
            }

            // Ensure the lists are synchronized
            if (_axes.Count != _projections1.Count || _axes.Count != _projections2.Count)
            {
                // Handle the mismatch case, e.g., log an error or throw an exception
                throw new InvalidOperationException("The lists _axes, _projections1, and _projections2 are not synchronized.");
            }
        }

        private bool CheckForOverlap()
        {
            for (int i = 0; i < _axes.Count; i++)
            {
                (float min1, float max1) = _projections1[i];
                (float min2, float max2) = _projections2[i];

                if (min1 > max2 || min2 > max1)
                {
                    return false; // Separating axis found, no collision
                }
            }

            return true; // No separating axis found, collision detected
        }

        private bool SATCollision(Vector2[] shape1, Vector2[] shape2)
        {
            GenerateAxesAndProjections(shape1, shape2);
            return CheckForOverlap();
        }

        public void RenderSATVisualization(LineRenderer lineRenderer, Vector2[] squareVertices, Vector2[] triangleVertices)
        {
            // Ensure the lists are synchronized
            if (_axes.Count != _projections1.Count || _axes.Count != _projections2.Count)
            {
                // Handle the mismatch case, e.g., log an error or return early
                throw new InvalidOperationException("The lists _axes, _projections1, and _projections2 are not synchronized.");
            }

            // Draw axes
            foreach (var axis in _axes)
            {
                Vector3[] vertices = { new Vector3(Vector2.Zero), new Vector3(axis * 0.5f) }; // Scale down for visualization
                lineRenderer.Render(vertices, new Vector4(1.0f, 1.0f, 0.0f, 1.0f)); // Yellow color for axes
            }

            // Draw lines from each vertex to the axes
            for (int i = 0; i < _axes.Count; i++)
            {
                Vector2 axis = _axes[i];

                // Draw lines from each vertex of shape1 (square) to the axis
                foreach (var vertex in squareVertices)
                {
                    float projection = Vector2.Dot(axis, vertex);
                    Vector2 projectedPoint = axis * projection;
                    Vector3[] line = { new Vector3(vertex), new Vector3(projectedPoint) };
                    lineRenderer.Render(line, new Vector4(1.0f, 0.0f, 0.0f, 1.0f)); // Red color for square projections
                }

                // Draw lines from each vertex of shape2 (triangle) to the axis
                foreach (var vertex in triangleVertices)
                {
                    float projection = Vector2.Dot(axis, vertex);
                    Vector2 projectedPoint = axis * projection;
                    Vector3[] line = { new Vector3(vertex), new Vector3(projectedPoint) };
                    lineRenderer.Render(line, new Vector4(0.5f, 0.0f, 0.5f, 1.0f)); // Purple color for triangle projections
                }
            }
        }

        private IEnumerable<Vector2> GetAxes(Vector2[] shape)
        {
            for (int i = 0; i < shape.Length; i++)
            {
                Vector2 p1 = shape[i];
                Vector2 p2 = shape[(i + 1) % shape.Length];
                Vector2 edge = p2 - p1;
                Vector2 normal = new Vector2(-edge.Y, edge.X).Normalized();
                yield return normal;
            }
        }

        private (float min, float max) ProjectShape(Vector2 axis, Vector2[] shape)
        {
            float min = Vector2.Dot(axis, shape[0]);
            float max = min;

            for (int i = 1; i < shape.Length; i++)
            {
                float projection = Vector2.Dot(axis, shape[i]);
                if (projection < min)
                {
                    min = projection;
                }
                if (projection > max)
                {
                    max = projection;
                }
            }

            return (min, max);
        }
    }
}
