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
        public static Random random = new Random();

        public static int ScreenCenterX()
        {
            return Raylib.GetScreenWidth() / 2;
        }
        public static int ScreenCenterY()
        {
            return Raylib.GetScreenHeight() / 2;
        }

        /// <summary>
        /// has a percent chance off returning true
        /// </summary>
        /// <param name="percent"></param>
        /// <returns></returns>
        public static bool Chance(float percent)
        {
            return (random.Next(0, 100) < percent);
        }
    }
}
