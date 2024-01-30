using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Polygondwanaland.Game
{
    /// <summary>
    /// Holds X and Y position as integers
    /// </summary>
    public struct XY 
    {
        public int X;
        public int Y;

        public XY(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }

        public static bool operator ==(XY A, XY B)
        {
            return (A.X == B.X && A.Y == B.Y);
        }

        public static bool operator !=(XY A, XY B)
        {
            return !(A == B);
        }
    }


}
