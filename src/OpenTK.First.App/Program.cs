using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;
using OpenTK.First.App.Core;

namespace OpenTKFirstApp
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var nativeWindowSettings = new NativeWindowSettings()
            {
                ClientSize = new Vector2i(800, 600),
                Title = "OpenTK Game"
            };

            using (var game = new Game(GameWindowSettings.Default, nativeWindowSettings))
            {
                game.Run();
            }
        }
    }
}