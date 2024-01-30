using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Polygondwanaland.Game.Scenes.OrbitTesting
{
    /// <summary>
    /// Sphere Of Influence
    /// Mostly Attached to Planets but may be used for other things
    /// 
    /// </summary>
    public class SOI
    {
        public Vector2 position { get; set; }
        public float radius;
        public float mass;
    }
}
