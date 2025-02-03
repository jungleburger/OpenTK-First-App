using OpenTK.Mathematics;
using System.Drawing;

namespace OpenTK.First.App.Core.Collision
{
    public interface ICollidable
    {
        RectangleF BoundingBox { get; } // Axis-Aligned Bounding Box
        Vector2[] Vertices { get; }     // Transformed vertices
        Vector2[] Axes { get; }
        bool IsDirty { get; } // Indicates if the shape has changed
        void UpdateAxes();
    }
}
