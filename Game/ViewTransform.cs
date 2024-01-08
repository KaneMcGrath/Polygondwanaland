using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Polygondwanaland.Game
{
    /// <summary>
    /// Holds an offset Position and Scale
    /// Has Functions to convert between screen coordinates and world coordinates
    /// and vice versa
    /// </summary>
    public class ViewTransform
    {
        public ViewTransform(Vector2 offset, float scale)
        {
            Offset = offset;
            Scale = scale;
        }

        public ViewTransform() 
        {
            Offset = Vector2.Zero;
            Scale = 1.0f;
        }

        public Vector2 Offset { get; set; }
        public float Scale { get; set; }

        public Vector2 ConvertToScreenSpace(Vector2 point)
        {
            return point + this.Offset + (this.Scale * point);
        }

        public Vector2 ConvertToWorldSpace(Vector2 point)
        {
            return point - this.Offset / this.Scale;
        }

        public float ConvertXToScreenSpace(float x)
        {
            return this.Offset.X + (this.Scale * x);
        }

        public float ConvertYToScreenSpace(float y)
        {
            return this.Offset.Y + (this.Scale * y);
        }

        public float ConvertXToWorldSpace(float x)
        {
            return (x - this.Offset.X) / this.Scale;
        }

        public float ConvertYToWorldSpace(float y)
        {
            return (y - this.Offset.Y) / this.Scale;
        }
        
        public void ScaleFromPoint(float x, float y, float scale)
        {
            float xFactor =x + Offset.X;
            float yFactor =y + Offset.Y;
            this.Scale += scale;
            this.Offset = new Vector2(Offset.X + (xFactor * scale),  Offset.Y + (yFactor * scale));
        }


    }
}
