namespace OpenTK.First.App.Core.Objects
{
	public class GameObjectManager
	{
		private readonly List<GameObject> _gameObjects = new List<GameObject>();

		public void Add(GameObject gameObject)
		{
			if (gameObject != null)
			{
				_gameObjects.Add(gameObject);
			}
		}

		public void InitializeAll()
		{
			foreach (var obj in _gameObjects)
			{
				obj.Initialize();
			}
		}

		public void UpdateAll(double deltaTime)
		{
			foreach (var obj in _gameObjects)
			{
				if (obj.IsActive)
				{
					obj.Update(deltaTime);
				}
			}
		}

		public void RenderAll()
		{
			foreach (var obj in _gameObjects)
			{
				if (obj.IsActive)
				{
					obj.Render();
				}
			}
		}

		public void CleanupAll()
		{
			foreach (var obj in _gameObjects)
			{
				obj.Cleanup();
			}
			_gameObjects.Clear();
		}
	}
}