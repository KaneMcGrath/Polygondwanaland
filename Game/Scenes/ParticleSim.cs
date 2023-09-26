using Polygondwanaland.FlatUI5;
using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Polygondwanaland.Game.Scenes
{
    public static class ParticleSim
    {
        private static Window ControlWindow = new Window(new Rect(0, 60, 300, 800), "Controls") { showWindow = true };
        private static int MenuBarPosition = -30;


        public static void Update()
        {
            Raylib.ClearBackground(Color.BLACK);
            if (MenuBarPosition > -30)
            {
                if (FlatUI.Button(new Rect(0, MenuBarPosition, 300, 30), "Controls"))
                {
                    ControlWindow.showWindow = true;
                }
            }
            if (Raylib.GetMousePosition().Y < 60)
            {
                MenuBarPosition = 0;
            }
        }
    }
}
