using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Polygondwanaland.Game.Scenes.ParticleSim
{
    public static class Simulation
    {
        public static List<Particle> particles = new List<Particle>();

        

        public static void Update()
        {
            foreach (var particle in particles)
            {

            }
        }
    }

    public struct Particle
    {
        public static float x, y, vx, vy;
        public static Color particleColor;

    }
}
