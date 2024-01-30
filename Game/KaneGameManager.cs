using Raylib_cs;
using System;
using System.Collections.Generic;
using System.IO;
using Polygondwanaland.FlatUI5;
using Polygondwanaland.Game;
using Polygondwanaland.Game.Scenes;
using Polygondwanaland.Game.Scenes.ParticleSim;
using Polygondwanaland.Game.Scenes.CelularAutomata;
using Polygondwanaland.Game.Scenes.BattlePong;

namespace Polygondwanaland.Game
{
    public static class KaneGameManager
    {
        public static string Directory = "";
        public static int CurrentScene = 0;
        public static bool DrawFPS = true;

        public static void Init()
        {
            FlatUI.DefaultFont = Raylib.LoadFontEx(Environment.CurrentDirectory + "\\Fonts\\OpenSans_SemiCondensed-Bold.ttf", 64, null, 0);
            Raylib.SetWindowState(ConfigFlags.FLAG_WINDOW_RESIZABLE);
            InputManager.Init();
            while (!Raylib.IsFontReady(FlatUI.DefaultFont));
        }

        public static void Update()
        {
            InputManager.Update();
            Time.Update();
            if (CurrentScene == 0)
            {
                MainMenu.Update();
            }
            else if (CurrentScene == 1)
            {
                ParticleSim.Update();
            }
            else if (CurrentScene == 2)
            {
                GameOfLife.Update();
            }
            else if (CurrentScene == 3)
            {
                GameOfLife.Update();
            }
            else if (CurrentScene == 4)
            {
                BattlePong.Update();
            }
            if (DrawFPS)
            {
                Raylib.DrawFPS(Raylib.GetScreenWidth() - 100, 0);
            }
        }
    }
}
