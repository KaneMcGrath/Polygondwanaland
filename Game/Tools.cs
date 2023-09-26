using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Raylib_cs;

namespace Polygondwanaland.Game
{
    public static class Tools
    {
        public static int ScreenCenterX()
        {
            return Raylib.GetScreenWidth() / 2;
        }
        public static int ScreenCenterY()
        {
            return Raylib.GetScreenHeight() / 2;
        }
    }
}
