using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Polygondwanaland.FlatUI5
{
    public class Window
    {
        public Rect rect;
        public string title;
        public Color insideColor;
        public bool minimize = false;
        public bool showWindow = false;
        public Rect ContentRect;
        public int MinimizedWidth;
        public bool isDragging = false;

        public static int ItemHeight = 30;

        private Rect titleBarRect;
        private Rect titleBarDragRect;
        private Rect minimizeButtonRect;
        private Rect closeButtonRect;

        private float dragXOffset = 0f;
        private float dragYOffset = 0f;



        public Window(Rect rect, string title, Color insideColor)
        {
            this.rect = rect;
            this.title = title;
            this.insideColor = insideColor;
            MinimizedWidth = rect.width;
            UpdateRects();
        }

        public Window(Rect rect, string title)
        {
            this.rect = rect;
            this.title = title;
            insideColor = FlatUI.insideColor;
            MinimizedWidth = rect.width;
            UpdateRects();
        }

        private void UpdateRects()
        {
            if (minimize)
            {
                titleBarRect = new Rect(rect.x + rect.width - MinimizedWidth, rect.y, MinimizedWidth, 30);
                titleBarDragRect = new Rect(rect.x + rect.width - MinimizedWidth, rect.y, MinimizedWidth - 60, 30);
            }
            else
            {
                titleBarRect = new Rect(rect.x, rect.y, rect.width, 30);
                titleBarDragRect = new Rect(rect.x, rect.y, rect.width - 60, 30);
            }
            minimizeButtonRect = new Rect(rect.x + rect.width - 60, rect.y, 30, 30);
            closeButtonRect = new Rect(rect.x + rect.width - 30, rect.y, 30, 30);
            ContentRect = new Rect(rect.x, rect.y + 30, rect.width, rect.height - 30);
        }

        /// <summary>
        /// Returns true if the window contents should be visible
        /// meaning it is not minimized or closed
        /// </summary>
        /// <returns></returns>
        public bool ContentVisible()
        {
            if (showWindow)
            {
                if (minimize)
                {
                    return false;
                }
                return true;
            }
            return false;
        }

        public void OnGUI()
        {
            if (showWindow)
            {
                if (isDragging)
                {
                    UpdateRects();
                }
                if (!minimize)
                {
                    FlatUI.Box(rect, insideColor);
                }
                FlatUI.Box(titleBarRect, insideColor);
                FlatUI.Label(titleBarDragRect, title, 24, 4);
                if (FlatUI.Button(minimizeButtonRect, "-"))
                {
                    minimize = !minimize;
                    UpdateRects();
                }
                if (FlatUI.Button(closeButtonRect, "x"))
                {
                    showWindow = false;
                }
                if (Raylib.IsMouseButtonPressed(MouseButton.MOUSE_LEFT_BUTTON))
                {
                    if (FlatUI.IsMouseInRect(titleBarDragRect))
                    {
                        isDragging = true;
                        dragXOffset = Raylib.GetMouseX() - rect.x;
                        dragYOffset = Raylib.GetMouseY() - rect.y;
                    }
                }
                if (Raylib.IsMouseButtonReleased(MouseButton.MOUSE_LEFT_BUTTON))
                {
                    isDragging = false;
                }
                if (isDragging)
                {
                    rect.x = Raylib.GetMouseX() - (int)dragXOffset;
                    rect.y = Raylib.GetMouseY() - (int)dragYOffset;
                    if (rect.x < 0) rect.x = 0;
                    if (rect.y < 0) rect.y = 0;
                    if (rect.x > Raylib.GetScreenWidth() - rect.width) rect.x = Raylib.GetScreenWidth() - rect.width;
                    if (rect.y > Raylib.GetScreenHeight() - 30f) rect.y = Raylib.GetScreenHeight() - 30;
                }
            }
        }

        /// <summary>
        /// Divides the entire window into rows based on RowHeight and returns a rect that coresponds to the index provided
        /// </summary>
        /// <param name="i">The Index of the row you want to use.  0 will be the top of the window</param>
        public Rect IndexToRect(int i)
        {
            return new Rect(ContentRect.x, ContentRect.y + i * ItemHeight, ContentRect.width, ItemHeight);
        }

        /// <summary>
        /// Divides the entire window into rows based on RowHeight and returns a rect that coresponds to the index provided
        /// </summary>
        /// <param name="i">The Index of the row you want to use.  0 will be the top of the window</param>
        /// <param name="divisions">Divide the row into a number of columns</param>
        /// <param name="n">which index of the columns to return (0-indexed)</param>
        /// <example>IndexToRect(0, 4, 2)  Will return the first row and will return the 3rd quarter of that row</example>
        public Rect IndexToRect(int i, int divisions, int n)
        {
            if (divisions < 1)
            {
                divisions = 1;
            }
            return new Rect(ContentRect.x + ContentRect.width / divisions * n, ContentRect.y + i * ItemHeight, ContentRect.width / divisions, ItemHeight);
        }

        /// <summary>
        /// Divides the entire window into rows based on RowHeight and returns a rect that coresponds to the index provided
        /// </summary>
        /// <param name="i">The Index of the row you want to use.  0 will be the top of the window</param>
        /// <param name="divisions">Divide the row into a number of columns</param>
        /// <param name="n">which index of the columns to return (0-indexed)</param>
        /// <param name="width">Number of divided columns to combine.</param>
        /// <example>IndexToRect(0, 6, 1, 2)  first row divided into 6 columns and 2 columns wide starting at the second column</example>
        public Rect IndexToRect(int i, int divisions, int n, int width)
        {
            if (divisions < 1)
            {
                divisions = 1;
            }
            return new Rect(ContentRect.x + ContentRect.width / divisions * n, ContentRect.y + i * ItemHeight, width * (ContentRect.width / divisions), ItemHeight);
        }
    }
}
