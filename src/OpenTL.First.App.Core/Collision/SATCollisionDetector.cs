using OpenTK.First.App.Core.Primitives;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

namespace OpenTK.First.App.Core.Collision
{
    public class SATCollisionDetector
    {
        // Preallocated arrays to minimize memory allocations
        //private Vector2[] _axes1;
        //private Vector2[] _axes2;

        public bool IsColliding(ICollidable shape1, ICollidable shape2)
        {
            // Ensure axes are up-to-date
            shape1.UpdateAxes();
            shape2.UpdateAxes();

            // Check for separation on shape1's axes
            for (int i = 0; i < shape1.Axes.Length; i++)
            {
                Vector2 axis = shape1.Axes[i];

                if (!IsOverlapOnAxis(axis, shape1.Vertices, shape2.Vertices))
                {
                    return false;
                }
            }

            // Check for separation on shape2's axes
            for (int i = 0; i < shape2.Axes.Length; i++)
            {
                Vector2 axis = shape2.Axes[i];

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
            // Project the first vertex
            float projection = Vector2.Dot(axis, vertices[0]);
            min = max = projection;

            // Project the remaining vertices
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

        public void RenderSATVisualization(LineRenderer lineRenderer, Vector2[] shape1Vertices, Vector2[] shape2Vertices)
        {
            int shape1AxesCount = shape1Vertices.Length;
            int shape2AxesCount = shape2Vertices.Length;

            // Initialize or resize the axes arrays if necessary
            Vector2[] axes1 = new Vector2[shape1AxesCount];
            Vector2[] axes2 = new Vector2[shape2AxesCount];

            // Get the axes (normals) from both shapes
            GetAxes(shape1Vertices, axes1);
            GetAxes(shape2Vertices, axes2);

            // Render axes and projections for shape1
            foreach (Vector2 axis in axes1)
            {
                RenderAxisAndProjections(lineRenderer, axis, shape1Vertices, shape2Vertices);
            }

            // Render axes and projections for shape2
            foreach (Vector2 axis in axes2)
            {
                RenderAxisAndProjections(lineRenderer, axis, shape1Vertices, shape2Vertices);
            }
        }
        private void RenderAxisAndProjections(LineRenderer lineRenderer, Vector2 axis, Vector2[] shape1Vertices, Vector2[] shape2Vertices)
        {
            // Draw the axis for visualization
            Vector3[] axisLine = {
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
            foreach (var vertex in vertices)
            {
                // Project the vertex onto the axis
                float projection = Vector2.Dot(axis, vertex);
                Vector2 projectedPoint = axis * projection;

                // Draw a line from the vertex to the projected point
                Vector3[] line = {
                    new Vector3(vertex.X, vertex.Y, 0.0f),
                    new Vector3(projectedPoint.X, projectedPoint.Y, 0.0f)
                };
                lineRenderer.Render(line, color);
            }
        }
    }
}
