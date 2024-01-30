using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Polygondwanaland.Game.Scenes.OrbitTesting
{
    /// <summary>
    /// Any object in the space simulation that is affected by gravity
    /// </summary>
    public class Entity
    {
        public Vector2 Position { get; set; }
        public Vector2 Velocity { get; set;}
    }
}
