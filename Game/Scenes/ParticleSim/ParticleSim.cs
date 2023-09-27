using Polygondwanaland.FlatUI5;
using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Polygondwanaland.Game.Scenes.ParticleSim
{
    public static class ParticleSim
    {
        private static Color ForegroundColor = new Color(100, 174, 170, 255);
        private static Window ControlWindow = new Window(new Rect(0, 60, 300, 800), "Controls", ForegroundColor)
        {
            showWindow = true,
            constraints = new Constraints(0, 0, 30, 0)
        };
        private static float MenuBarPosition = -30f;

        public static void Update()
        {
            Raylib.ClearBackground(Color.BLACK);
            if (MenuBarPosition > -30f)
            {
                FlatUI.Box(new Rect(0, (int)MenuBarPosition, Raylib.GetScreenWidth(), 30), ForegroundColor);
                if (FlatUI.Button(new Rect(0, (int)MenuBarPosition, 300, 30), "Controls"))
                {
                    ControlWindow.showWindow = true;
                }
            }
            if (Raylib.GetMousePosition().Y < 30)
            {
                if (MenuBarPosition < 0f)
                {
                    MenuBarPosition += 200 * Raylib.GetFrameTime();
                }
                else
                {
                    MenuBarPosition = 0f;
                }
            }
            else
            {
                if (MenuBarPosition > -30f)
                {
                    MenuBarPosition -= 200 * Raylib.GetFrameTime();
                }
            }
            ControlWindow.OnGUI();
            if (ControlWindow.ContentVisible())
            {
                if (FlatUI.Button(ControlWindow.IndexToRect(0), "test"))
                {

                }
            }
        }
    }
}
