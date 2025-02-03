// In a suitable namespace or within the Game class
using OpenTK.First.App.Core.Collision;
using OpenTK.Mathematics;
using System.Drawing;

public class TemporaryCollidable : ICollidable
{
    public Vector2[] Vertices { get; }
    public Vector2[] Axes { get; }
    public bool IsDirty => false; // Not used for temporary collidable

    public RectangleF BoundingBox => throw new NotImplementedException();

    public void UpdateAxes() { }   // No action needed

    public TemporaryCollidable(Vector2[] vertices, Vector2[] axes)
    {
        Vertices = vertices;
        Axes = axes;
    }
}
