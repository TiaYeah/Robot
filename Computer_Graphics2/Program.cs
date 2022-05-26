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
                Size = new Vector2i(800, 600),
                Title = "LearnOpenTK - Creating a Window",
                WindowState = WindowState.Normal,
                Flags = ContextFlags.Default,
                Profile = ContextProfile.Compatability
            };
            using (var game = new Game(GameWindowSettings.Default, nSettings))
            {
                game.Run();
            }
            
    }
    }
}
