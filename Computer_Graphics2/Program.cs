using System;
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;



namespace Computer_Graphics2
{
    class Program
    {
        static void Main(string[] args)
        {
            NativeWindowSettings nSettings = new NativeWindowSettings()
            {
                Size = new Vector2i(1000, 800),
                Title = "Robot Window",
                WindowState = WindowState.Normal,
                Flags = ContextFlags.Default,
            };
            using (var game = new Game(GameWindowSettings.Default, nSettings))
            {
                game.Run();
            }
            
    }
    }
}
