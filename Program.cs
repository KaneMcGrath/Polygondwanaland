using System;
using Raylib_cs;

namespace Polygondwanaland
{
    class Program
    {
        public static void Main(string[] args)
        {
            Raylib.InitWindow(1500, 1080, "Hello World");
            Raylib.SetWindowState(ConfigFlags.FLAG_WINDOW_RESIZABLE);
            while (!Raylib.WindowShouldClose())
            {
                Raylib.BeginDrawing();
                Raylib.ClearBackground(Color.BLACK);



                Raylib.EndDrawing();
            }
        }
    }
}