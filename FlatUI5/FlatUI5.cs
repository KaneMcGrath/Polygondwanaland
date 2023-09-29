using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Polygondwanaland.FlatUI5
{
    public struct Rect
    {
        public int x;
        public int y;
        public int width;
        public int height;

        public Rect(int x, int y, int width, int height)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
        }

        public static Rect FromCenter(int x, int y, int width, int height)
        {
            return new Rect(x - width / 2, y - height / 2, width, height);
        }

        public static bool operator ==(Rect rect1, Rect rect2)
        {
            return rect1.Equals(rect2);
        }

        public static bool operator !=(Rect rect1, Rect rect2)
        {
            return !rect1.Equals(rect2);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Rect))
                return false;

            Rect otherRect = (Rect)obj;
            return x == otherRect.x &&
                   y == otherRect.y &&
                   width == otherRect.width &&
                   height == otherRect.height;
        }

        public override int GetHashCode()
        {
            return x.GetHashCode() ^ y.GetHashCode() ^ width.GetHashCode() ^ height.GetHashCode();
        }
    }



    /// <summary>
    /// FlatUI5 with a Raylib Backend
    /// </summary>
    public static class FlatUI
    {
        public static readonly Color insideColor = new Color(99, 99, 99, 255);
        public static readonly Color outsideColor = new Color(25, 25, 25, 255);
        public static readonly Color defaultButtonColor = new Color(102, 102, 102, 255);
        public static readonly Color defaultTextFieldColor = new Color(204, 204, 204, 255);
        public static readonly Color defaultTextFieldOutlineColor = new Color(50, 50, 50, 255);
        public static readonly Color ChangedValueOutlineColor = new Color(255, 127, 0, 255);
        private static int defaultOutlineThickness = 2;

        public static Font DefaultFont = new Font();

        public static void DrawRect(Rect rectangle, Color c)
        {
            Raylib.DrawRectangle(rectangle.x, rectangle.y, rectangle.width, rectangle.height, c);
        }

        /// <summary>
        /// Draws a static box with an outline that will make up most of the GUI elements of this mod
        /// </summary>
        /// <param name="Rect"></param>
        public static void Box(Rect Rect, bool visible = true)
        {
            if (!visible) return;
            Rect insideRect = new Rect(Rect.x + defaultOutlineThickness, Rect.y + defaultOutlineThickness, Rect.width - defaultOutlineThickness * 2, Rect.height - defaultOutlineThickness * 2);

            DrawRect(Rect, outsideColor);
            DrawRect(insideRect, insideColor);
        }
        public static void Box(Rect Rect, Color insideColor)
        {
            Rect insideRect = new Rect(Rect.x + defaultOutlineThickness, Rect.y + defaultOutlineThickness, Rect.width - defaultOutlineThickness * 2, Rect.height - defaultOutlineThickness * 2);
            DrawRect(Rect, outsideColor);
            DrawRect(insideRect, insideColor);
        }
        public static void Box(Rect Rect, Color insideColor, Color outsideColor)
        {
            Rect insideRect = new Rect(Rect.x + defaultOutlineThickness, Rect.y + defaultOutlineThickness, Rect.width - defaultOutlineThickness * 2, Rect.height - defaultOutlineThickness * 2);
            DrawRect(Rect, outsideColor);
            DrawRect(insideRect, insideColor);
        }
        public static void Box(Rect Rect, Color insideColor, Color outsideColor, int outlineThickness)
        {
            Rect insideRect = new Rect(Rect.x + defaultOutlineThickness, Rect.y + defaultOutlineThickness, Rect.width - defaultOutlineThickness * 2, Rect.height - defaultOutlineThickness * 2);
            DrawRect(Rect, outsideColor);
            DrawRect(insideRect, insideColor);
        }

        /// <summary>
        /// A box that can invert its colors easily
        /// thickens when moused over to indicate interactibility
        /// </summary>
        /// <param name="Rect"></param>
        /// <param name="invert"></param>
        public static void SwitchBox(Rect Rect, bool invert)
        {
            int outlineModifier = 0;
            if (IsMouseInRect(Rect))
            {
                outlineModifier = 1;
            }
            Rect insideRect = new Rect(Rect.x + defaultOutlineThickness + outlineModifier, Rect.y + defaultOutlineThickness + outlineModifier, Rect.width - (defaultOutlineThickness + outlineModifier) * 2, Rect.height - (defaultOutlineThickness + outlineModifier) * 2);
            if (invert)
            {
                DrawRect(Rect, insideColor);
                DrawRect(insideRect, outsideColor);
            }
            else
            {
                DrawRect(Rect, outsideColor);
                DrawRect(insideRect, insideColor);
            }
        }
        public static void SwitchBox(Rect Rect, bool invert, Color insideColor)
        {
            int outlineModifier = 0;
            if (IsMouseInRect(Rect))
            {
                outlineModifier = 1;
            }
            Rect insideRect = new Rect(Rect.x + defaultOutlineThickness + outlineModifier, Rect.y + defaultOutlineThickness + outlineModifier, Rect.width - (defaultOutlineThickness + outlineModifier) * 2, Rect.height - (defaultOutlineThickness + outlineModifier) * 2);
            if (invert)
            {
                DrawRect(Rect, insideColor);
                DrawRect(insideRect, outsideColor);
            }
            else
            {
                DrawRect(Rect, outsideColor);
                DrawRect(insideRect, insideColor);
            }
        }

        public static void SwitchBox(Rect Rect, bool invert, Color insideColor, Color staticOutsideTex)
        {
            int outlineModifier = 0;
            if (IsMouseInRect(Rect))
            {
                outlineModifier = 1;
            }
            Rect insideRect = new Rect(Rect.x + defaultOutlineThickness + outlineModifier, Rect.y + defaultOutlineThickness + outlineModifier, Rect.width - (defaultOutlineThickness + outlineModifier) * 2, Rect.height - (defaultOutlineThickness + outlineModifier) * 2);
            if (invert)
            {
                DrawRect(Rect, staticOutsideTex);
                DrawRect(insideRect, outsideColor);
            }
            else
            {
                DrawRect(Rect, staticOutsideTex);
                DrawRect(insideRect, insideColor);
            }
        }

        /// <summary>
        /// Styled button, inverts colors when clicked
        /// </summary>
        /// <param name="Rect"></param>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool Button(Rect Rect, string label, bool draw = true)
        {
            if (draw)
            {
                SwitchBox(Rect, IsMouseInRect(Rect) && Raylib.IsMouseButtonDown(MouseButton.MOUSE_LEFT_BUTTON), defaultButtonColor);
                Label(Rect, label, 20, 4);
                return IsMouseInRect(Rect) && Raylib.IsMouseButtonPressed(MouseButton.MOUSE_LEFT_BUTTON);
            }
            else
            {
                return false;
            }
        }
        public static bool Button(Rect Rect, string label, Color insideColor, bool draw = true)
        {
            if (draw)
            {
                SwitchBox(Rect, IsMouseInRect(Rect) && Raylib.IsMouseButtonDown(MouseButton.MOUSE_LEFT_BUTTON), insideColor);
                Label(Rect, label, 20, 4);
                return IsMouseInRect(Rect) && Raylib.IsMouseButtonPressed(MouseButton.MOUSE_LEFT_BUTTON);
            }
            else
            {
                return false;
            }
        }

        public static bool Button(Rect Rect, string label, Color insideColor, Color outsideColor, bool draw = true)
        {
            if (draw)
            {
                SwitchBox(Rect, IsMouseInRect(Rect) && Raylib.IsMouseButtonPressed(MouseButton.MOUSE_LEFT_BUTTON), insideColor, outsideColor);
                Label(Rect, label, 20, 4);
                return IsMouseInRect(Rect) && Raylib.IsMouseButtonPressed(MouseButton.MOUSE_LEFT_BUTTON);
            }
            else
            {
                return false;
            }
        }

        //public static void TooltipButton(rect rect, string title, string content, int tabColor, bool draw = true)
        //{
        //    if (draw)
        //    {
        //        SwitchBox(new rect(rect.x + rect.width - 26f, rect.y + 4f, 26f, rect.height - 8f), IsMouseInRect(rect) && Input.GetKey(KeyCode.Mouse0), insideColor, outsideColor);
        //        if (Button(new rect(rect.x + rect.width - 28f, rect.y + 2f, 26f, rect.height - 4f), "?", ButtonStyle))
        //        {
        //            ToolTipWindow.Tooltip(title, content, tabColor);
        //        }
        //    }
        //}


        /// <summary>
        /// Draw Text with the default font
        /// Side: 0=topleft    1=topcenter    2=topright
        ///       3=midleft    4=center       5=midright 
        ///       6=bottomleft 7=bottomcenter 8=bottomright
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="message"></param>
        /// <param name="side"></param>
        /// <param name="draw"></param>
        public static void Label(Rect rect, string message, int fontSize = 20, int side = 0, bool draw = true)
        {
            if (draw)
            {
                int x = rect.x;
                int y = rect.y;
                if (side == 1) { x = rect.x + rect.width / 2 - (int)(Raylib.MeasureTextEx(DefaultFont, message, fontSize, 0).X / 2f); }
                else if (side == 2) { x = rect.x + rect.width - (int)Raylib.MeasureTextEx(DefaultFont, message, fontSize, 0).X; }
                else if (side == 3) { y = rect.y + rect.height / 2 - (int)(Raylib.MeasureTextEx(DefaultFont, message, fontSize, 0).Y / 2f); }
                else if (side == 4) { x = rect.x + rect.width / 2 - (int)(Raylib.MeasureTextEx(DefaultFont, message, fontSize, 0).X / 2f); ; y = rect.y + rect.height / 2 - (int)(Raylib.MeasureTextEx(DefaultFont, message, fontSize, 0).Y / 2f); }
                else if (side == 5) { x = rect.x + rect.width - (int)Raylib.MeasureTextEx(DefaultFont, message, fontSize, 0).X; y = rect.y + rect.height / 2 - (int)(Raylib.MeasureTextEx(DefaultFont, message, fontSize, 0).Y / 2f); }
                else if (side == 6) { y = rect.y + rect.height - (int)Raylib.MeasureTextEx(DefaultFont, message, fontSize, 0).Y; }
                else if (side == 7) { x = rect.x + rect.width / 2 - (int)(Raylib.MeasureTextEx(DefaultFont, message, fontSize, 0).X / 2f); y = rect.y + rect.height - (int)Raylib.MeasureTextEx(DefaultFont, message, fontSize, 0).Y; }
                else if (side == 8) { x = rect.x + rect.width - (int)Raylib.MeasureTextEx(DefaultFont, message, fontSize, 0).X; y = rect.y + rect.height - (int)Raylib.MeasureTextEx(DefaultFont, message, fontSize, 0).Y; }

                Raylib.DrawTextEx(DefaultFont, message, new Vector2(x, y), fontSize, 0, Color.WHITE);
            }
        }

        public static void Label(Rect rect, string message, Color color, int fontSize = 20, int side = 0, bool draw = true)
        {
            if (draw)
            {
                int x = rect.x;
                int y = rect.y;
                if (side == 1) { x = rect.x + rect.width / 2 - (int)(Raylib.MeasureTextEx(DefaultFont, message, fontSize, 0).X / 2f); }
                else if (side == 2) { x = rect.x + rect.width - (int)Raylib.MeasureTextEx(DefaultFont, message, fontSize, 0).X; }
                else if (side == 3) { y = rect.y + rect.height / 2 - (int)(Raylib.MeasureTextEx(DefaultFont, message, fontSize, 0).Y / 2f); }
                else if (side == 4) { x = rect.x + rect.width / 2 - (int)(Raylib.MeasureTextEx(DefaultFont, message, fontSize, 0).X / 2f); ; y = rect.y + rect.height / 2 - (int)(Raylib.MeasureTextEx(DefaultFont, message, fontSize, 0).Y / 2f); }
                else if (side == 5) { x = rect.x + rect.width - (int)Raylib.MeasureTextEx(DefaultFont, message, fontSize, 0).X; y = rect.y + rect.height / 2 - (int)(Raylib.MeasureTextEx(DefaultFont, message, fontSize, 0).Y / 2f); }
                else if (side == 6) { y = rect.y + rect.height - (int)Raylib.MeasureTextEx(DefaultFont, message, fontSize, 0).Y; }
                else if (side == 7) { x = rect.x + rect.width / 2 - (int)(Raylib.MeasureTextEx(DefaultFont, message, fontSize, 0).X / 2f); y = rect.y + rect.height - (int)Raylib.MeasureTextEx(DefaultFont, message, fontSize, 0).Y; }
                else if (side == 8) { x = rect.x + rect.width - (int)Raylib.MeasureTextEx(DefaultFont, message, fontSize, 0).X; y = rect.y + rect.height - (int)Raylib.MeasureTextEx(DefaultFont, message, fontSize, 0).Y; }
                Raylib.DrawTextEx(DefaultFont, message, new Vector2(x, y), fontSize, 0, color);
            }
        }

        

        /// <summary>
        /// Styled Check Box, inverts colors when checked.  Includes a label to the left side of the box
        /// </summary>
        /// <param name="Rect"></param>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool Check(Rect Rect, bool value, string label, bool draw = true)
        {
            if (!draw) return value;
            SwitchBox(new Rect(Rect.x + Rect.width - 28, Rect.y + 2, 26, Rect.height - 4), value);
            if (IsMouseInRect(Rect) && Raylib.IsMouseButtonPressed(MouseButton.MOUSE_LEFT_BUTTON))
            {
                return !value;
            }
            Label(new Rect(Rect.x + 4, Rect.y, Rect.width, Rect.height), label);
            return value;
        }
        public static string TextField(Rect rect, string text, int fontSize = 20, int side = 0, bool draw = true)
        {
            if (draw)
            {

                int x = rect.x;
                int y = rect.y;
                if (side == 1) { x = rect.x + rect.width / 2 - (int)(Raylib.MeasureTextEx(DefaultFont, text, fontSize, 0).X / 2f); }
                else if (side == 2) { x = rect.x + rect.width - (int)Raylib.MeasureTextEx(DefaultFont, text, fontSize, 0).X; }
                else if (side == 3) { y = rect.y + rect.height / 2 - (int)(Raylib.MeasureTextEx(DefaultFont, text, fontSize, 0).Y / 2f); }
                else if (side == 4) { x = rect.x + rect.width / 2 - (int)(Raylib.MeasureTextEx(DefaultFont, text, fontSize, 0).X / 2f); ; y = rect.y + rect.height / 2 - (int)(Raylib.MeasureTextEx(DefaultFont, text, fontSize, 0).Y / 2f); }
                else if (side == 5) { x = rect.x + rect.width - (int)Raylib.MeasureTextEx(DefaultFont, text, fontSize, 0).X; y = rect.y + rect.height / 2 - (int)(Raylib.MeasureTextEx(DefaultFont, text, fontSize, 0).Y / 2f); }
                else if (side == 6) { y = rect.y + rect.height - (int)Raylib.MeasureTextEx(DefaultFont, text, fontSize, 0).Y; }
                else if (side == 7) { x = rect.x + rect.width / 2 - (int)(Raylib.MeasureTextEx(DefaultFont, text, fontSize, 0).X / 2f); y = rect.y + rect.height - (int)Raylib.MeasureTextEx(DefaultFont, text, fontSize, 0).Y; }
                else if (side == 8) { x = rect.x + rect.width - (int)Raylib.MeasureTextEx(DefaultFont, text, fontSize, 0).X; y = rect.y + rect.height - (int)Raylib.MeasureTextEx(DefaultFont, text, fontSize, 0).Y; }

                Box(rect, defaultTextFieldColor, defaultTextFieldOutlineColor);
                Label(rect, text, fontSize, side);
                return TextHandler.EditableText(rect, new Vector2(x, y), text, fontSize);
            }
            else
                return text;
        }

        public static int tabs(Rect pos, string[] tabs, int index, bool top, Color color)
        {
            int num = tabs.Length;
            int num2 = pos.width / num;
            Color[] array = new Color[num];
            for (int i = 0; i < num; i++)
            {
                array[i] = color;
            }
            for (int j = 0; j < num; j++)
            {
                Rect rect;
                if (top)
                {
                    rect = new Rect(pos.x + num2 * j, pos.y, num2, pos.height);
                }
                else
                {
                    rect = new Rect(pos.x, pos.y + j * 30, 100, 30);
                }
                if (index == j)
                {
                    tab(rect, top, array[j], outsideColor);
                    Label(rect, tabs[j]);
                }
                else
                {
                    Box(rect, array[j], outsideColor);
                    if (Button(rect, tabs[j]))
                    {
                        return j;
                    }
                }
            }
            return index;
        }
        public static int tabs(Rect pos, string[] tabs, int index, bool top, Color[] tabColors)
        {
            int num = tabs.Length;
            int num2 = pos.width / num;
            Color[] array = new Color[num];
            for (int i = 0; i < num; i++)
            {
                if (i < tabColors.Length)
                {
                    array[i] = tabColors[i];
                }
                else
                {
                    array[i] = insideColor;
                }
            }
            for (int j = 0; j < num; j++)
            {
                Rect rect;
                if (top)
                {
                    rect = new Rect(pos.x + num2 * j, pos.y, num2, pos.height);
                }
                else
                {
                    rect = new Rect(pos.x, pos.y + j * 30, 100, 30);
                }
                if (index == j)
                {
                    tab(rect, top, array[j], outsideColor);
                    Label(rect, tabs[j]);
                }
                else
                {
                    Box(rect, array[j], outsideColor);
                    if (Button(rect, tabs[j]))
                    {
                        return j;
                    }
                }
            }
            return index;
        }
        private static void tab(Rect rect, bool top, Color inside, Color outside)
        {
            if (top)
            {
                DrawRect(new Rect(rect.x, rect.y, rect.width, rect.height), outside);
                DrawRect(new Rect(rect.x + 2, rect.y + 2, rect.width - 4, rect.height), inside);
                return;
            }
            else
            {
                DrawRect(new Rect(rect.x - 2, rect.y, rect.width, rect.height), outside);
                DrawRect(new Rect(rect.x - 2, rect.y + 2, rect.width - 2, rect.height - 4), inside);
                return;
            }
        }

        public struct PageData
        {
            int page;
            int elementMin;
            int elementMax;

            public PageData(int page, int elementMin, int elementMax)
            {
                this.page = page;
                this.elementMin = elementMin;
                this.elementMax = elementMax;
            }
        }

        private static PageData Page(Rect pageButtons, int page, int arrayLength, int pageLength)
        {
            if (arrayLength < pageLength)
            {
                return new PageData(0, 0, arrayLength);
            }
            int result = page;
            int width = 30;
            if (Button(new Rect(pageButtons.x, pageButtons.y, width, pageButtons.height), "<"))
            {
                result = page + 1;
            }
            if (Button(new Rect(pageButtons.x + pageButtons.width - 30, pageButtons.y, width, pageButtons.height), ">"))
            {
                result = page - 1;
            }
            int min = page * pageLength;
            int max = page * pageLength + pageLength;
            return new PageData(result, min, max);

        }

        //global variables to use between all sliders as we should only be using one at a time
        //and if I can get away with not making an object for this that would be sick
        public static bool IsDraggingSlider = false;

        private static float SliderDragOffset = 0f;
        private static float SliderThickness = 10f;
        private static Rect DefaultRect = new Rect();
        private static Rect HasSlider = DefaultRect;

        /// <summary>
        /// Slider with specified min and max values
        /// </summary>
        /// <param name="Rect"></param>
        /// <param name="value"></param>
        /// <param name="minValue"></param>
        /// <param name="maxValue"></param>
        /// <param name="draw"></param>
        /// <returns></returns>
        public static float Slider(Rect Rect, float value, float minValue, float maxValue, bool draw = true)
        {
            if (!draw) return value;

            float newValue = value;
            float valueSpan = maxValue - minValue;
            float halfThickSlider = SliderThickness / 2f;
            Box(Rect);
            Rect SliderRect = new Rect((int)(Rect.x + halfThickSlider + (value - minValue) / valueSpan * (Rect.width - SliderThickness) - halfThickSlider), Rect.y, (int)SliderThickness, Rect.height);
            Box(SliderRect);
            if (IsMouseInRect(Rect))
            {
                if (Raylib.GetMouseWheelMoveV().Y > 0f)
                {
                    newValue += valueSpan / 40f;
                    if (newValue < minValue) newValue = minValue;
                    if (newValue > maxValue) newValue = maxValue;
                }
                if (Raylib.GetMouseWheelMoveV().Y < 0f)
                {
                    newValue -= valueSpan / 40f;
                    if (newValue < minValue) newValue = minValue;
                    if (newValue > maxValue) newValue = maxValue;
                }
                if (Raylib.IsMouseButtonPressed(MouseButton.MOUSE_LEFT_BUTTON))
                {
                    if (IsMouseInRect(SliderRect))
                    {
                        IsDraggingSlider = true;
                        SliderDragOffset = Raylib.GetMouseX() - SliderRect.x;
                        HasSlider = Rect;
                    }
                    else
                    {

                        newValue = (Raylib.GetMouseX() - (Rect.x + halfThickSlider)) / (Rect.width - SliderThickness) * valueSpan + minValue;
                        IsDraggingSlider = true;
                        SliderDragOffset = halfThickSlider;
                        HasSlider = Rect;
                    }
                }
            }
            if (IsDraggingSlider && HasSlider == Rect)
            {
                int x = SliderRect.x;
                if (SliderRect.x > Raylib.GetScreenWidth() - 100)
                    x = Raylib.GetScreenWidth() - 100;
                Rect numberDisplay = new Rect(x, SliderRect.y - 25, 100, 25);
                Box(numberDisplay);
                Label(numberDisplay, value.ToString(), 20, 4);
                newValue = (Raylib.GetMouseX() - (Rect.x + halfThickSlider) - SliderDragOffset + halfThickSlider) / (Rect.width - SliderThickness) * valueSpan + minValue;
                if (newValue < minValue) newValue = minValue;
                if (newValue > maxValue) newValue = maxValue;
            }
            if (Raylib.IsMouseButtonReleased(MouseButton.MOUSE_LEFT_BUTTON))
            {
                IsDraggingSlider = false;
                HasSlider = DefaultRect;
            }

            return newValue;
        }

        public static float Slider(Rect Rect, float value, float minValue, float maxValue, Color sliderColor, bool draw = true)
        {
            if (!draw) return value;

            float newValue = value;
            float valueSpan = maxValue - minValue;
            float halfThickSlider = SliderThickness / 2f;
            Box(Rect);
            Rect SliderRect = new Rect((int)(Rect.x + halfThickSlider + (value - minValue) / valueSpan * (Rect.width - SliderThickness) - halfThickSlider), Rect.y, (int)SliderThickness, Rect.height);
            Box(SliderRect, sliderColor);
            if (IsMouseInRect(Rect))
            {
                if (Raylib.GetMouseWheelMoveV().Y > 0f)
                {
                    newValue += valueSpan / 40f;
                    if (newValue < minValue) newValue = minValue;
                    if (newValue > maxValue) newValue = maxValue;
                }
                if (Raylib.GetMouseWheelMoveV().Y < 0f)
                {
                    newValue -= valueSpan / 40f;
                    if (newValue < minValue) newValue = minValue;
                    if (newValue > maxValue) newValue = maxValue;
                }
                if (Raylib.IsMouseButtonPressed(MouseButton.MOUSE_LEFT_BUTTON))
                {
                    if (IsMouseInRect(SliderRect))
                    {
                        IsDraggingSlider = true;
                        SliderDragOffset = Raylib.GetMouseX() - SliderRect.x;
                        HasSlider = Rect;
                    }
                    else
                    {
                        newValue = (Raylib.GetMouseX() - (Rect.x + halfThickSlider)) / (Rect.width - SliderThickness) * valueSpan + minValue;
                        IsDraggingSlider = true;
                        SliderDragOffset = halfThickSlider;
                        HasSlider = Rect;
                    }
                }
            }
            if (IsDraggingSlider && HasSlider == Rect)
            {
                int x = SliderRect.x;
                if (SliderRect.x > Raylib.GetScreenWidth() - 100)
                    x = Raylib.GetScreenWidth() - 100;
                Rect numberDisplay = new Rect(x, SliderRect.y - 25, 100, 25);
                Box(numberDisplay);
                Label(numberDisplay, value.ToString(), 20, 4);
                newValue = (Raylib.GetMouseX() - (Rect.x + halfThickSlider) - SliderDragOffset + halfThickSlider) / (Rect.width - SliderThickness) * valueSpan + minValue;
                if (newValue < minValue) newValue = minValue;
                if (newValue > maxValue) newValue = maxValue;
            }
            if (Raylib.IsMouseButtonReleased(MouseButton.MOUSE_LEFT_BUTTON))
            {
                IsDraggingSlider = false;
                HasSlider = DefaultRect;
            }

            return newValue;
        }

        /// <summary>
        /// Creates 4 rects that form an outline of the input rect
        /// </summary>
        /// <param name="rect"></param>
        /// <returns></returns>
        public static Rect[] transparentOutlineRects(Rect rect, int thickness)
        {
            Rect[] result = new Rect[4];
            result[0] = new Rect(rect.x, rect.y, rect.width, thickness);
            result[1] = new Rect(rect.x, rect.y + rect.height - thickness, rect.width, thickness);
            result[2] = new Rect(rect.x, rect.y + thickness, thickness, rect.height - thickness);
            result[3] = new Rect(rect.x + rect.width - thickness, rect.y + thickness, thickness, rect.height - thickness);
            return result;
        }

        public static void DrawOutline(Rect rect, int thickness, Color outlineColor)
        {
            Rect[] outline = transparentOutlineRects(rect, defaultOutlineThickness);

            DrawRect(outline[0], outlineColor);
            DrawRect(outline[1], outlineColor);
            DrawRect(outline[2], outlineColor);
            DrawRect(outline[3], outlineColor);
        }

        /// <summary>
        /// bar that fills to represent progress towards a maximum
        /// 0 = left>right, 1=top>bottom, 2 = right>left, 3 = bottom>top
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="progress"></param>
        /// <param name="max"></param>
        /// <param name="orientation"></param>
        /// <param name="barTex"></param>
        /// <param name="boxInsideTex"></param>
        /// <param name="boxOutsideTex"></param>
        public static void ProgressBar(Rect rect, float progress, float max, int orientation, Color barColor, Color boxOutlineColor, bool draw = true)
        {
            float value = Math.Min(progress, max);

            DrawOutline(rect, defaultOutlineThickness, boxOutlineColor);

            int doubleOutline = defaultOutlineThickness * 2;
            if (orientation == 0)
            {
                DrawRect(new Rect(rect.x + defaultOutlineThickness, rect.y + defaultOutlineThickness, (int)((value / max) * (float)(rect.width - doubleOutline)), rect.height - doubleOutline), barColor);
            }
            if (orientation == 1)
            {
                DrawRect(new Rect(rect.x + defaultOutlineThickness, rect.y + defaultOutlineThickness, rect.width - doubleOutline, (int)((value / max) * (float)(rect.height - doubleOutline))), barColor);
            }
            if (orientation == 2)
            {
                int width = (int)((value / max) * (float)(rect.width - doubleOutline));
                DrawRect(new Rect(rect.x + rect.width - defaultOutlineThickness - width, rect.y + defaultOutlineThickness, width, rect.height - doubleOutline), barColor);
            }
            if (orientation == 3)
            {
                int height = (int)((value / max) * (float)(rect.height - doubleOutline));
                DrawRect(new Rect(rect.x + defaultOutlineThickness, rect.y + rect.height - defaultOutlineThickness - height, rect.width - doubleOutline, height), barColor);
            }

        }

        public static bool isDragging = false;
        public static int dragXOffset = 0;
        public static int dragYOffset = 0;

        /// <summary>
        /// Draw the Titlebar at the top of the screen to make the actual sdl Window in the style of the FlatUIWindow
        /// use ConfigFlags.FLAG_WINDOW_UNDECORATED to remove the default title bar
        /// optional outline to mimic the window style further
        /// </summary>
        public static void WindowDecoration(Color backgroundColor, Color TitleBarColor)
        {
            Rect rect = new Rect(0, 0, Raylib.GetScreenWidth(), Raylib.GetScreenHeight());
            Rect titleBarRect = new Rect(rect.x, rect.y, rect.width, 30);
            Rect titleBarDragRect = new Rect(rect.x, rect.y, rect.width - 60, 30);

            Rect minimizeButtonRect = new Rect(rect.x + rect.width - 60, rect.y, 30, 30);
            Rect closeButtonRect = new Rect(rect.x + rect.width - 30, rect.y, 30, 30);
            Rect ContentRect = new Rect(rect.x, rect.y + 30, rect.width, rect.height - 30);

            Rect ScreenRect = new Rect(0, 0, Raylib.GetScreenWidth(), Raylib.GetScreenHeight());
            FlatUI.Box(ScreenRect, backgroundColor);
            FlatUI.Box(titleBarRect, TitleBarColor);
            FlatUI.Label(titleBarDragRect, "Title", 24, 4);
            if (FlatUI.Button(minimizeButtonRect, "-", TitleBarColor))
            {
                Raylib.MinimizeWindow();
            }
            if (FlatUI.Button(closeButtonRect, "x", TitleBarColor))
            {
                Environment.Exit(0);
            }
            if (Raylib.IsMouseButtonPressed(MouseButton.MOUSE_LEFT_BUTTON))
            {
                if (FlatUI.IsMouseInRect(titleBarDragRect))
                {
                    isDragging = true;
                    GetCursorPos(out POINT mousePOS);
                    Vector2 pos = Raylib.GetWindowPosition();
                    dragXOffset = mousePOS.X - (int)pos.X;
                    dragYOffset = mousePOS.Y - (int)pos.Y;

                }
            }
            if (Raylib.IsMouseButtonReleased(MouseButton.MOUSE_LEFT_BUTTON))
            {
                isDragging = false;
            }
            if (isDragging)
            {
                //Vector2 pos = Raylib.GetWindowPosition();
                GetCursorPos(out POINT mousePOS);
                Raylib.SetWindowPosition(mousePOS.X - dragXOffset, mousePOS.Y - dragYOffset);  //(int)pos.Y + Raylib.GetMouseY() - dragYOffset     + (Raylib.GetMouseX() - dragXOffset)
                //Console.WriteLine("[off:" + dragXOffset + "]{mouse:" + mousePOS.X + "} = " + (mousePOS.X - dragXOffset));
            }
        }

        /// <summary>
        /// Returns true if the mouse pointer is currently inside the rectangle
        /// </summary>
        /// <param name="rect"></param>
        /// <returns></returns>
        public static bool IsMouseInRect(Rect rect)
        {
            return Raylib.GetMouseX() > rect.x && Raylib.GetMouseX() < rect.x + rect.width && Raylib.GetMouseY() > rect.y && Raylib.GetMouseY() < rect.y + rect.height;
        }

        /// <summary>
        /// Parity to Raylib.DrawText()
        /// </summary>
        /// <param name="text"></param>
        /// <param name="posX"></param>
        /// <param name="posY"></param>
        /// <param name="fontSize"></param>
        /// <param name="color"></param>
        public static void DrawText(string text, int posX, int posY, int fontSize, Color color)
        {
            Raylib.DrawTextEx(DefaultFont, text, new Vector2(posX, posY), fontSize, 0, color);
        }


        /// <summary>
        /// Struct representing a point.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;

            public static implicit operator System.Drawing.Point(POINT point)
            {
                return new System.Drawing.Point(point.X, point.Y);
            }
        }



        /// <summary>
        /// Retrieves the cursor's position, in screen coordinates.
        /// </summary>
        /// <see>See MSDN documentation for further information.</see>
        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(out POINT lpPoint);

        public static System.Drawing.Point GetCursorPosition()
        {
            POINT lpPoint;
            GetCursorPos(out lpPoint);
            // NOTE: If you need error handling
            // bool success = GetCursorPos(out lpPoint);
            // if (!success)

            return lpPoint;
        }
    }
}
