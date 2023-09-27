using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Polygondwanaland.FlatUI5
{
    /// <summary>
    /// Framework for text editing to integrate with flatUI and be as immediate mode as possible
    /// </summary>
    public static class TextHandler
    {
        public static bool IsEdititingText = false;

        public static bool TextDebugVis = true;

        //The Cursor will be defined in relation to the string it is editing

        public static int CursorIndex = 0;
        public static bool insert = true;           //toggle insert mode to replace text
        public static int SelectionEndIndex = 0;    //if highlighting text highlight the range from CursorIndex to SelectionEndIndex

        public static float[] CurrentStringSpacing = new float[3];

        /// <summary>
        /// 
        /// </summary>
        /// <param name="selectArea"></param>
        /// <param name="textArea"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string EditableText(Rect selectArea, Vector2 textOrigin, string text, int fontSize)
        {
            if (IsEdititingText)
            {
                if (TextDebugVis) FlatUI.DrawOutline(selectArea, 1, Color.ORANGE);
                if (!FlatUI.IsMouseInRect(selectArea) && Raylib.IsMouseButtonPressed(0))
                {
                    IsEdititingText = false;
                }
                Vector2 textSize = Raylib.MeasureTextEx(FlatUI.DefaultFont, text, fontSize, 0);
                Rect textArea = new Rect((int)textOrigin.X, (int)textOrigin.Y, (int)textSize.X, (int)textSize.Y);
                if (TextDebugVis) FlatUI.DrawOutline(textArea, 1, Color.YELLOW);

                //draw cursor
                float widthSum = 0f;
                for (int c = 0; c < text.Length; c++)
                {
                    float charwidth = Raylib.MeasureTextEx(FlatUI.DefaultFont, text.Substring(c, 1), fontSize, 0).X;
                    if (c == CursorIndex)
                    {
                        FlatUI.DrawRect(new Rect(textArea.x + (int)(widthSum), textArea.y, 2, textArea.height), Color.BLACK);
                    }
                    widthSum += charwidth;
                }


                if (FlatUI.IsMouseInRect(textArea))
                {
                    
                    if (TextDebugVis)
                    {
                        //int i = GetHoveredIndex(Raylib.GetMouseX(), textArea, text.Length);
                        //float w = textArea.width / text.Length;
                        //

                        float widthSum2 = 0f;
                        for (int c = 0; c < text.Length; c++)
                        {
                            float charwidth = Raylib.MeasureTextEx(FlatUI.DefaultFont, text.Substring(c,1), fontSize, 0).X;
                            
                            float p = Raylib.GetMouseX() - textArea.x;
                            
                            if (p > widthSum2 && p < widthSum2 + charwidth)
                            {
                                FlatUI.Label(new Rect(10, 10, 100, 30), "CharWidth:" + charwidth.ToString());
                                FlatUI.Label(new Rect(10, 40, 100, 30), "widthSum2:" + widthSum2.ToString());
                                FlatUI.Label(new Rect(10, 70, 100, 30), "P:" + p.ToString());
                                FlatUI.Label(new Rect(10, 100, 100, 30), "TX:" + textArea.x.ToString());
                                FlatUI.Label(new Rect(10, 130, 100, 30), "I:" + c.ToString());
                                FlatUI.DrawOutline(new Rect(textArea.x + (int)(widthSum2), textArea.y, (int)charwidth, textArea.height), 1, Color.BLUE);
                                if (p < widthSum2 + (charwidth / 2f))
                                {
                                    FlatUI.DrawRect(new Rect(textArea.x + (int)(widthSum2), textArea.y, (int)(charwidth / 2f), textArea.height), Color.GREEN);
                                }
                                else
                                {
                                    FlatUI.DrawRect(new Rect(textArea.x + (int)(widthSum2 + charwidth / 2f) , textArea.y, (int)(charwidth / 2f), textArea.height), Color.GREEN);
                                }
                            }
                            widthSum2 += charwidth;
                        }
                    }
                    if (Raylib.IsMouseButtonPressed(0))
                    {
                        float widthSum3 = 0f;
                        for (int c = 0; c < text.Length; c++)
                        {
                            float charwidth = Raylib.MeasureTextEx(FlatUI.DefaultFont, text.Substring(c, 1), fontSize, 0).X;

                            float p = Raylib.GetMouseX() - textArea.x;

                            if (p > widthSum3 && p < widthSum3 + charwidth)
                            {
                               
                                if (p < widthSum3 + (charwidth / 2f))
                                {
                                    CursorIndex = c;
                                }
                                else
                                {
                                    CursorIndex = c + 1;
                                }
                            }
                            widthSum3 += charwidth;
                        }
                    }
                }
            }
            else
            {
                if (FlatUI.IsMouseInRect(selectArea) && Raylib.IsMouseButtonPressed(0))
                {
                    IsEdititingText = true;
                    Vector2 textSize = Raylib.MeasureTextEx(FlatUI.DefaultFont, text, fontSize, 0);
                    Rect textArea = new Rect((int)textOrigin.X, (int)textOrigin.Y, (int)textSize.X, (int)textSize.Y);
                    if (FlatUI.IsMouseInRect(textArea))
                    {
                        if (TextDebugVis)
                        {
                            
                        }
                    }
                }
            }
            return text;
        }

        private static int GetHoveredIndex(float x, Rect area, int max)
        {
            float width = area.width / max;
            float n = x - area.x;
            return ((int)Math.Floor(n / width));
        }
    }
}
