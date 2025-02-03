using OpenTK.First.App.Core.Collision;
using System.Drawing;

namespace OpenTK.First.App.Core.Partitioning
{
    public class Quadtree
    {
        private const int MaxObjects = 4;
        private const int MaxLevels = 5;

        private int _level;
        private List<ICollidable> _objects;
        private RectangleF _bounds;
        private Quadtree[] _nodes;

        public Quadtree(int level, RectangleF bounds)
        {
            _level = level;
            _bounds = bounds;
            _objects = new List<ICollidable>();
            _nodes = new Quadtree[4];
        }

        public void Clear()
        {
            _objects.Clear();

            for (int i = 0; i < _nodes.Length; i++)
            {
                if (_nodes[i] != null)
                {
                    _nodes[i].Clear();
                    _nodes[i] = null;
                }
            }
        }

        // Insert, Split, GetIndex, Retrieve methods go here
    }
}
