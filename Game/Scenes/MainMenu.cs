using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Raylib_cs;
using Polygondwanaland.FlatUI5;

namespace Polygondwanaland.Game.Scenes
{
    public static class MainMenu
    {
        private static Window MainWindow = new Window(Rect.FromCenter(Tools.ScreenCenterX(), Tools.ScreenCenterY(), 300, 800), "Polygondwanaland", new Color(232,75,75,255)) { showWindow = true };
        private static Window TextTestWindow = new Window(Rect.FromCenter(Tools.ScreenCenterX() + 300, Tools.ScreenCenterY(), 800, 300), "Edit Text", new Color(232,75,75,255)) { showWindow = true };
        public static Color ClearColor = new Color(41, 75, 71, 255);

        private static string debugEditableString = "Hello Text Handler";
        private static string secondString = "This is another text box, which I didnt consider when making this!";

        public static void Update()
        {
            Raylib.ClearBackground(ClearColor);
            debugEditableString = FlatUI.TextField(new Rect(0, 30, Raylib.GetScreenWidth(), 30), debugEditableString, 30);
            if (MainWindow.showWindow == false)
            {
                if (FlatUI.Button(Rect.FromCenter(Tools.ScreenCenterX(), Tools.ScreenCenterY(), 100, 30), "Begin!"))
                {
                    MainWindow.showWindow = true;
                    MainWindow.minimize = false;
                    TextTestWindow.showWindow = true;
                    TextTestWindow.minimize = false;
                }
            }
            TextTestWindow.OnGUI();
            if (TextTestWindow.ContentVisible())
            {
                secondString = FlatUI.TextField(TextTestWindow.IndexToRect(0), secondString, 30);
            }
            MainWindow.OnGUI();
            if (MainWindow.ContentVisible())
            {
                if (FlatUI.Button(MainWindow.IndexToRect(0), "Particle Sim"))
                {
                    KaneGameManager.CurrentScene = 1;
                }
                if (FlatUI.Button(MainWindow.IndexToRect(1), "Celular Automata"))
                {
                    KaneGameManager.CurrentScene = 2;
                }
                if (FlatUI.Button(MainWindow.IndexToRect(2), "Orbit Test"))
                {
                    KaneGameManager.CurrentScene = 3;
                }
                if (FlatUI.Button(MainWindow.IndexToRect(3), "BattlePong"))
                {
                    KaneGameManager.CurrentScene = 4;
                }
                TextHandler.TextDebugVis = FlatUI.Check(MainWindow.IndexToRect(8), TextHandler.TextDebugVis, "Text Debug View");
                TextHandler.IsSelection = FlatUI.Check(MainWindow.IndexToRect(9), TextHandler.IsSelection, "IsSelection");
                if (FlatUI.Button(MainWindow.IndexToRect(10,2,0), "<<"))
                {
                    TextHandler.SelectionEndIndex--;
                }
                if (FlatUI.Button(MainWindow.IndexToRect(10, 2, 1), ">>"))
                {
                    TextHandler.SelectionEndIndex++;
                }
            }
        }
    }
}
