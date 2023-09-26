using System;
using Polygondwanaland.Game;
using Raylib_cs;

namespace Polygondwanaland
{
    class Program
    {
        public static void Main(string[] args)
        {
            Raylib.InitWindow(1500, 1080, "Hello World");
            Raylib.SetWindowState(ConfigFlags.FLAG_WINDOW_RESIZABLE);

            KaneGameManager.Init();

            while (!Raylib.WindowShouldClose())
            {
                Raylib.BeginDrawing();
                Raylib.ClearBackground(Color.BLACK);


                KaneGameManager.Update();
                


                Raylib.EndDrawing();
            }
        }
    }
}