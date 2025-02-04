using OpenTK.First.App.Core.Collision;
using OpenTK.Mathematics;
using System.Drawing;

[Obsolete("This class is temporary and will be removed in the future.", false)]
public class TemporaryCollidable : ICollidable
{
	public Vector2[] Vertices { get; }
	public Vector2[] Axes { get; }

	public RectangleF BoundingBox => throw new NotImplementedException();

	bool ICollidable.IsDirty { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

	public void UpdateAxes() { }   // No action needed

	public TemporaryCollidable(Vector2[] vertices, Vector2[] axes)
	{
		Vertices = vertices;
		Axes = axes;
	}
}
