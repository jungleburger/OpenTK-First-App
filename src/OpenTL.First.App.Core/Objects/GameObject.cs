using OpenTK.Mathematics;

namespace OpenTK.First.App.Core.Objects
{
    public abstract class GameObject
    {
        public Vector2 Position { get; set; }
        public bool IsActive { get; set; } = true;

        public abstract void Initialize();
        public abstract void Update(double deltaTime);
        public abstract void Render();
        public abstract void Cleanup();
    }
}
