using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Polygondwanaland.Game.Scenes.OrbitTesting
{
    /// <summary>
    /// A large body with gravity, but is not affected by gravity
    /// </summary>
    public class Planet
    {
        public Planet Parent;

        public float SOIradius;
        public float mass;
        public float radius;

        public Vector2 Position;
    }
}
