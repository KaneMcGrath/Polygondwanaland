using Polygondwanaland.FlatUI5;
using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Polygondwanaland.Game
{
    /// <summary>
    /// 2D Camera to pan zoom and rotate a transform
    /// Transforms the worldSpace coordinates to the cameras local screenSpace
    /// </summary>
    public class Camera
    {
        public Vector2 Position = Vector2.Zero;
        public float Rotation = 0f;
        public float Zoom = 1f;

        public int ViewportWidth 
        {
            get 
            { 
                return Raylib.GetScreenWidth(); 
            }
        }
        public int ViewportHeight
        {
            get
            {
                return Raylib.GetScreenHeight();
            }
        }

        public Camera(Vector2 position, float rotation, float zoom)
        {
            Position = position;
            Rotation = rotation;
            Zoom = zoom;
        }

        public Camera()
        {
            Position = Vector2.Zero;
            Rotation = 0f;
            Zoom = 1f;
        }

        /// <summary>
        /// Deffault set of camera controls
        /// </summary>
        public void CameraControls()
        {
            if (Raylib.IsMouseButtonDown(0) && !Raylib.IsKeyDown(KeyboardKey.KEY_LEFT_SHIFT) && !FlatUI.IsDraggingSlider)
            {
                //CameraPos += Raylib.GetMouseDelta() / zoomLevel;
                Position -= Raylib.GetMouseDelta() * (1f / Zoom);
            }
            float zoomDelta = (Raylib.GetMouseWheelMove() / 10f) * (MathF.Sqrt(Zoom) / 2f);
            if (MathF.Abs(zoomDelta) > 0) Zoom += zoomDelta;
            if (Zoom < 0f) Zoom = -Zoom;
            if (Raylib.IsKeyDown(KeyboardKey.KEY_UP))
            {
                Zoom += 1f * Time.DeltaTime;
            }
            if (Raylib.IsKeyDown(KeyboardKey.KEY_DOWN))
            {
                Zoom -= 1f * Time.DeltaTime;
            }
            //Raylib.DrawCircle((int)View.ConvertXToScreenSpace(1000f), (int)View.ConvertYToScreenSpace(1000f), 10f, Color.GREEN);
            //if (zoomLevel != 0f) View.ScaleFromPoint(1000f, 1000f, zoomLevel);
            //if (View.Scale < 0.1f) View.Scale = 0.1f;
        }

        // Convert world space to screen space
        public Vector2 WorldToScreen(Vector2 worldPosition)
        {
            Matrix3x2 transformMatrix = Matrix3x2.CreateTranslation(-Position) *
                                         Matrix3x2.CreateRotation(-Rotation) *
                                         Matrix3x2.CreateScale(Zoom) *
                                         Matrix3x2.CreateTranslation(ViewportWidth / 2, ViewportHeight / 2);

            return Vector2.Transform(worldPosition, transformMatrix);
        }

        // Convert screen space to world space
        public Vector2 ScreenToWorld(Vector2 screenPosition)
        {
            Matrix3x2 inverseTransformMatrix;
            Matrix3x2.Invert(Matrix3x2.CreateTranslation(-Position) *
                                                                 Matrix3x2.CreateRotation(-Rotation) *
                                                                 Matrix3x2.CreateScale(Zoom) *
                                                                 Matrix3x2.CreateTranslation(ViewportWidth / 2, ViewportHeight / 2)
                                                                    , out inverseTransformMatrix);

            return Vector2.Transform(screenPosition, inverseTransformMatrix);
        }

        public float Scale(float scale)
        {
            return Zoom * scale;
        }
    }
}
