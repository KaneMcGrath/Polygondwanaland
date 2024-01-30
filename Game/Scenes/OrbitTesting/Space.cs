using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Polygondwanaland.Game.Scenes.OrbitTesting
{
    
    /// <summary>
    /// Container for planets and to consolidate all space physics simulation stuff
    /// </summary>
    public class Space
    {
        public List<Planet> planets;
        public List<Entity> entities;

        public static bool debugLines = true;

        public void Update()
        {
            foreach (Entity e in entities)
            {
                foreach (Planet planet in planets)
                {
                    if (Vector2.Distance(e.Position, planet.Position) < planet.SOIradius)
                    {
                        Vector2 CemterVector = e.Position - planet.Position;
                        e.Velocity = CemterVector;
                    }
                }
            }
        }
    }
}
