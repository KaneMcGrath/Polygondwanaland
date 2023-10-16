using Polygondwanaland.Game;
using Polygondwanaland.Game.Scenes;
using Raylib_cs;
using Color = Raylib_cs.Color;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Polygondwanaland.FlatUI5
{
    /// <summary>
    /// Framework for text editing to integrate with flatUI and be as immediate mode as possible
    /// </summary>
    public static unsafe class TextHandler
    {
        //Im going to use the select area rect to identify which text box we are currently editing
        //otherwise I would have to have an ID or use an object for the text boxes
        public static Rect EditedTextBox = new Rect();
        public static bool IsEdititingText = false;

        public static bool TextDebugVis = true;

        //The Cursor will be defined in relation to the string it is editing

        public static int CursorIndex = 0;
        public static bool insert = true;           //toggle insert mode to replace text

        public static bool IsSelection = false;     //if text is curently highlighted
        public static int SelectionEndIndex = 0;    //if highlighting text highlight the range from CursorIndex to SelectionEndIndex

        public static float[] CurrentStringSpacing = new float[3];

        private static char lastChar = ' ';

        private static float CursorBlinkTimer = 0f;
        private static float CursorBlinkTime = 0.3f;
        private static bool DrawCursor = true;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="selectArea"></param>
        /// <param name="textArea"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string EditableText(Rect selectArea, Vector2 textOrigin, string text, int fontSize)
        {
            if (IsEdititingText && EditedTextBox == selectArea)
            {
                if (true)
                {
                    if (TextDebugVis)
                    {
                        FlatUI.DrawOutline(selectArea, 1, Color.ORANGE);
                        FlatUI.Label(new Rect(10, 160, 100, 30), "Key:" + lastChar.ToString());
                        FlatUI.Label(new Rect(10, 190, 100, 30), "Raw:" + Raylib.GetKeyPressed().ToString());
                        FlatUI.Label(new Rect(10, 220, 100, 30), "Cursor Index:" + CursorIndex.ToString());
                        FlatUI.Label(new Rect(10, 250, 100, 30), "Selection End Index:" + SelectionEndIndex.ToString());
                        FlatUI.Label(new Rect(10, 280, 100, 30), "Text Length:" + text.Length);
                    }

                    Vector2 textSize = Raylib.MeasureTextEx(FlatUI.DefaultFont, text, fontSize, 0);
                    Rect textArea = new Rect((int)textOrigin.X, (int)textOrigin.Y, (int)textSize.X, (int)textSize.Y);
                    if (TextDebugVis) FlatUI.DrawOutline(textArea, 1, Color.YELLOW);
                    if (Tools.timer(ref CursorBlinkTimer, CursorBlinkTime))
                    {
                        DrawCursor = !DrawCursor;
                    }
                    //draw Selection Rectangle behind text
                    if (IsSelection)
                    {
                        //FlatUI.DrawRect(new Rect(textArea.x + (int)(widthSum), textArea.y, 2, textArea.height), Color.BLACK);
                        //find start pixel and width
                        int startPixel = -1;
                        int width = -1;
                        float selectionWidthSum = 0f;
                        for (int c = 0; c < text.Length + 1; c++)
                        {
                            if (c == CursorIndex)
                            {
                                startPixel = textArea.x + (int)(selectionWidthSum);
                            }
                            if (c == SelectionEndIndex)
                            {
                                width = (textArea.x + (int)(selectionWidthSum)) - startPixel;
                            }
                            if (c < text.Length)
                            {
                                float charwidth = Raylib.MeasureTextEx(FlatUI.DefaultFont, text.Substring(c, 1), fontSize, 0).X;
                                selectionWidthSum += charwidth;
                            }
                        }
                        //create a rect from the startPixel and width
                        if (startPixel >= 0 && width > 0)
                        {
                            Rect selectionArea = new Rect(startPixel, textArea.y, width, textArea.height);
                            FlatUI.DrawRect(selectionArea, Color.LIME);
                        }
                    }

                    //handle keyboard inputs
                    if (InputManager.IsInputs)
                    {
                        for (int i = 0; i < InputManager.InputQueue.Count; i++)
                        {
                            int key = InputManager.InputQueue[i];
                            if (!InputManager.IsModifierKey())
                            {
                                if (InputManager.IsChar(key))
                                {
                                    text = text.Insert(CursorIndex, InputManager.KeyboardKeyToChar(key).ToString());
                                    CursorIndex++;
                                    continue;
                                }
                                if (key == (int)KeyboardKey.KEY_LEFT)
                                {
                                    if (CursorIndex > 0)
                                        CursorIndex--;
                                }
                                if (key == (int)KeyboardKey.KEY_RIGHT)
                                {
                                    if (CursorIndex < text.Length - 1)
                                        CursorIndex++;
                                }
                                if (key == (int)KeyboardKey.KEY_BACKSPACE)
                                {
                                    if (CursorIndex > 0)
                                    {
                                        if (CursorIndex <= text.Length)
                                        {
                                            CursorIndex--;
                                            text = text.Remove(CursorIndex, 1);
                                        }
                                    }
                                }
                                if (key == (int)KeyboardKey.KEY_DELETE)
                                {
                                    if (CursorIndex < text.Length)
                                        text = text.Remove(CursorIndex, 1);
                                }
                                if (key == (int)KeyboardKey.KEY_HOME)
                                {
                                    CursorIndex = 0;
                                }
                                if (key == (int)KeyboardKey.KEY_END)
                                {
                                    CursorIndex = text.Length;
                                }
                            }
                            else
                            {
                                if (InputManager.GetKey(KeyboardKey.KEY_LEFT_CONTROL))
                                {
                                    if (key == 86) //KEY_V = 86
                                    {
                                        try
                                        {
                                            sbyte* s = Raylib.GetClipboardText();
                                            string clip = new string(s);
                                            text = text.Insert(CursorIndex, clip);
                                            CursorIndex += clip.Length;
                                        }
                                        catch (Exception ex)
                                        {

                                        }
                                    }
                                }
                            }
                        }
                    }

                    //if the string is modified elsewhere keep our cursor inbounds
                    if (CursorIndex > text.Length)
                    {
                        CursorIndex = text.Length;
                    }
                    if (CursorIndex < 0) //not sure how this could ever happen
                    {
                        CursorIndex = 0;
                    }

                    //draw cursor
                    float widthSum = 0f;
                    for (int c = 0; c < text.Length + 1; c++)
                    {
                        if (c == CursorIndex)
                        {
                            if (DrawCursor)
                                FlatUI.DrawRect(new Rect(textArea.x + (int)(widthSum), textArea.y, 2, textArea.height), Color.BLACK);
                            break;
                        }
                        if (c < text.Length)
                        {
                            float charwidth = Raylib.MeasureTextEx(FlatUI.DefaultFont, text.Substring(c, 1), fontSize, 0).X;
                            widthSum += charwidth;
                        }
                    }
                    
                    //click the text to put the cursor somewhere
                    if (Raylib.IsMouseButtonPressed(0))
                    {
                        if (FlatUI.IsMouseInRect(selectArea))
                        {
                            if (FlatUI.IsMouseInRect(textArea))
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
                            else
                            {
                                CursorIndex = text.Length;
                            }
                        }
                        else
                        {
                            IsEdititingText = false;
                        }
                    }

                    //Drag the text to select an area
                    if (Raylib.IsMouseButtonDown(0))
                    {
                        if (FlatUI.IsMouseInRect(selectArea))
                        {
                            if (FlatUI.IsMouseInRect(textArea))
                            {
                                float widthSum3 = 0f;
                                for (int c = 0; c < text.Length; c++)
                                {
                                    float charwidth = Raylib.MeasureTextEx(FlatUI.DefaultFont, text.Substring(c, 1), fontSize, 0).X;
                                    float p = Raylib.GetMouseX() - textArea.x;
                                    if (c >= CursorIndex)
                                    {
                                        if (p > widthSum3 && p < widthSum3 + charwidth)
                                        {
                                            if (p < widthSum3 + (charwidth / 2f))
                                            {
                                                SelectionEndIndex = c;
                                            }
                                            else
                                            {
                                                SelectionEndIndex = c + 1;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        SelectionEndIndex = CursorIndex;
                                    }
                                    widthSum3 += charwidth;
                                }
                            }
                            else
                            {
                                SelectionEndIndex = text.Length;
                            }
                        }
                    }

                    if (FlatUI.IsMouseInRect(textArea))
                    {
                        if (TextDebugVis)
                        {
                            float widthSum2 = 0f;
                            for (int c = 0; c < text.Length; c++)
                            {
                                float charwidth = Raylib.MeasureTextEx(FlatUI.DefaultFont, text.Substring(c, 1), fontSize, 0).X;

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
                                        FlatUI.DrawRect(new Rect(textArea.x + (int)(widthSum2 + charwidth / 2f), textArea.y, (int)(charwidth / 2f), textArea.height), Color.GREEN);
                                    }
                                }
                                widthSum2 += charwidth;
                            }
                        }
                    }
                }
            }
            else
            {
                if (FlatUI.IsMouseInRect(selectArea) && Raylib.IsMouseButtonPressed(0))
                {
                    IsEdititingText = true;
                    EditedTextBox = selectArea;
                    Vector2 textSize = Raylib.MeasureTextEx(FlatUI.DefaultFont, text, fontSize, 0);
                    Rect textArea = new Rect((int)textOrigin.X, (int)textOrigin.Y, (int)textSize.X, (int)textSize.Y);
                    if (FlatUI.IsMouseInRect(textArea))
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
                    else
                    {
                        CursorIndex = text.Length;
                    }
                }
            }
            FlatUI.Label(selectArea, text, Color.BLACK, fontSize, 0);
            return text;
        }
    }
}
