using System;
using Polygondwanaland.Game;
using Raylib_cs;

namespace Polygondwanaland
{
    class Program
    {
        public static void Main(string[] args)
        {
            Raylib.InitWindow(1500, 1080, "Polygondwanaland");

            KaneGameManager.Init();

            while (!Raylib.WindowShouldClose())
            {
                Raylib.BeginDrawing();

                KaneGameManager.Update();
                
                Raylib.EndDrawing();
            }
        }
    }
}