using Raylib_cs;
using System;
using System.Collections.Generic;
using System.IO;
using Polygondwanaland.FlatUI5;
using Polygondwanaland.Game;

namespace Polygondwanaland.Game
{
    public static class KaneGameManager
    {
        public static string Directory = "";

        public static void Init()
        {

            FlatUI.DefaultFont = Raylib.LoadFontEx(Environment.CurrentDirectory + "\\Fonts\\OpenSans_SemiCondensed-Bold.ttf", 64, null, 0);

            while (!Raylib.IsFontReady(FlatUI.DefaultFont));
        }
        public static void Update()
        {
            FlatUI.Label(new Rect(10, 10, 200, 30), "Hello FlatUI5!", 60);
            FlatUI.Label(new Rect(10, 50, 200, 30), Raylib.GetMouseX().ToString() + ", " + Raylib.GetMouseY().ToString(), 60);
            if (FlatUI.Button(new Rect(10, 90, 100, 30), "TestWindow!"))
            {
            }
        }
    }
}
