using Raylib_cs;
using System;
using System.Collections.Generic;
using System.IO;
using Polygondwanaland.FlatUI5;
using Polygondwanaland.Game;
using Polygondwanaland.Game.Scenes;

namespace Polygondwanaland.Game
{
    public static class KaneGameManager
    {
        public static string Directory = "";
        public static int CurrentScene = 0;
        

        public static void Init()
        {
            FlatUI.DefaultFont = Raylib.LoadFontEx(Environment.CurrentDirectory + "\\Fonts\\OpenSans_SemiCondensed-Bold.ttf", 64, null, 0);
            while (!Raylib.IsFontReady(FlatUI.DefaultFont));
        }

        public static void Update()
        {
            
            if (CurrentScene == 0)
            {
                MainMenu.Update();
            }
            else if (CurrentScene == 1)
            {
                ParticleSim.Update();
            }
        }
    }
}
