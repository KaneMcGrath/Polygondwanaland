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
        private static Color ClearColor = new Color(41, 75, 71, 255);
        public static void Update()
        {
            Raylib.ClearBackground(ClearColor);
            if (MainWindow.showWindow == false)
            {
                if (FlatUI.Button(Rect.FromCenter(Tools.ScreenCenterX(), Tools.ScreenCenterY(), 100, 30), "Begin!"))
                {
                    MainWindow.showWindow = true;
                    MainWindow.minimize = false;
                }
            }
            MainWindow.OnGUI();
            if (MainWindow.ContentVisible())
            {
                if (FlatUI.Button(MainWindow.IndexToRect(0), "Particle"))
                {
                    KaneGameManager.CurrentScene = 1;
                }
            }
        }
    }
}
