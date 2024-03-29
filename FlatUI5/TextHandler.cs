﻿using Polygondwanaland.Game;
using Raylib_cs;
using Color = Raylib_cs.Color;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;

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

        public static int CursorIndex = 0;
        public static bool insert = true;           //toggle insert mode to replace text

        public static bool IsSelection = true;     //if text is curently highlighted, but now is just a toggle for if you can select text
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
                    text = KeyboardInputs(text);

                    //Sanity Checks in case I suck
                    //if the string is modified elsewhere keep our cursor inbounds
                    if (CursorIndex > text.Length)
                    {
                        CursorIndex = text.Length;
                    }
                    if (SelectionEndIndex > text.Length) SelectionEndIndex = text.Length;
                    if (SelectionEndIndex < CursorIndex) SelectionEndIndex = CursorIndex;
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
                                SelectionEndIndex = text.Length;
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
                        }
                    }


                    if (TextDebugVis)
                    {
                        if (FlatUI.IsMouseInRect(textArea))
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

        private static string KeyboardInputs(string text)
        {
            if (InputManager.IsInputs)
            {
                for (int i = 0; i < InputManager.InputQueue.Count; i++)
                {
                    int key = InputManager.InputQueue[i];
                    if (!InputManager.IsModifierKey())
                    {
                        if (InputManager.IsChar(key))
                        {
                            if (SelectionEndIndex - CursorIndex > 0)
                            {
                                text = text.Remove(CursorIndex, SelectionEndIndex - CursorIndex);
                                text = text.Insert(CursorIndex, InputManager.KeyboardKeyToChar(key).ToString());
                                CursorIndex++;
                                SelectionEndIndex = CursorIndex;
                            }
                            else
                            {
                                text = text.Insert(CursorIndex, InputManager.KeyboardKeyToChar(key).ToString());
                                CursorIndex++;
                                SelectionEndIndex = CursorIndex;
                            }
                            continue;
                        }
                        if (key == (int)KeyboardKey.KEY_LEFT)
                        {

                            if (CursorIndex > 0)
                            {
                                CursorIndex--;
                                if (!InputManager.IsShiftKey())
                                    SelectionEndIndex = CursorIndex;
                            }
                        }
                        if (key == (int)KeyboardKey.KEY_RIGHT)
                        {
                            if (CursorIndex < text.Length)
                            {
                                if (!InputManager.IsShiftKey())
                                {
                                    CursorIndex++;
                                    SelectionEndIndex = CursorIndex;
                                }
                                else
                                {
                                    if (SelectionEndIndex < text.Length)
                                    {
                                        SelectionEndIndex++;
                                    }
                                }
                            }
                        }
                        if (key == (int)KeyboardKey.KEY_BACKSPACE)
                        {

                            if (CursorIndex <= text.Length)
                            {
                                if (SelectionEndIndex - CursorIndex > 0)
                                {
                                    text = text.Remove(CursorIndex, SelectionEndIndex - CursorIndex);
                                    SelectionEndIndex = CursorIndex;
                                }
                                else if (CursorIndex > 0)
                                {
                                    CursorIndex--;
                                    SelectionEndIndex = CursorIndex;
                                    text = text.Remove(CursorIndex, 1);
                                }
                            }
                        }
                        if (key == (int)KeyboardKey.KEY_DELETE)
                        {
                            if (CursorIndex < text.Length)
                            {
                                if (SelectionEndIndex - CursorIndex > 0)
                                {
                                    text = text.Remove(CursorIndex, SelectionEndIndex - CursorIndex);
                                    SelectionEndIndex = CursorIndex;
                                }
                                else
                                {
                                    text = text.Remove(CursorIndex, 1);
                                }
                            }
                        }
                        if (key == (int)KeyboardKey.KEY_HOME)
                        {
                            CursorIndex = 0;
                            if (!InputManager.IsShiftKey())
                                SelectionEndIndex = CursorIndex;
                        }
                        if (key == (int)KeyboardKey.KEY_END)
                        {
                            if (!InputManager.IsShiftKey())
                            {
                                CursorIndex = text.Length;
                                SelectionEndIndex = CursorIndex;
                            }
                            else
                            {
                                SelectionEndIndex = text.Length;
                            }
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
                                    if (SelectionEndIndex - CursorIndex > 0)
                                    {
                                        text = text.Remove(CursorIndex, SelectionEndIndex - CursorIndex);
                                    }
                                    sbyte* s = Raylib.GetClipboardText();
                                    string clip = new string(s);
                                    text = text.Insert(CursorIndex, clip);
                                    CursorIndex += clip.Length;
                                    SelectionEndIndex = CursorIndex;
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex.Message);
                                }
                            }
                            if (key == 67) //KEY_C = 67
                            {
                                try
                                {
                                    byte[] bytes = Encoding.ASCII.GetBytes(text.Substring(CursorIndex, SelectionEndIndex - CursorIndex));
                                    fixed (byte* b = bytes)
                                    {
                                        sbyte* sb = (sbyte*)b;
                                        Raylib.SetClipboardText(sb);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex.Message);
                                }
                            }
                            //Control + Left
                            //jump to the previous word seperated by a space
                            if (key == 263)//KEY_LEFT = 263
                            {
                                if (CursorIndex > 0)
                                {
                                    int space = text.LastIndexOf(' ', CursorIndex - 2, CursorIndex - 2);
                                    if (space != -1)
                                    {
                                        CursorIndex = space + 1;
                                    }
                                    else
                                    {
                                        CursorIndex = 0;
                                    }
                                    SelectionEndIndex = CursorIndex;
                                }
                            }
                            //Control + Right
                            //jump to the next word seperated by a space
                            if (key == 262)//KEY_RIGHT = 262
                            {
                                if (CursorIndex < text.Length)
                                {
                                    int space = text.IndexOf(' ', CursorIndex);
                                    if (space != -1)
                                    {
                                        CursorIndex = space + 1;
                                    }
                                    else
                                    {
                                        CursorIndex = text.Length;
                                    }
                                    SelectionEndIndex = CursorIndex;
                                }
                            }
                            //Control + A
                            //Select All
                            if (key == (int)(KeyboardKey.KEY_A))
                            {
                                CursorIndex = 0;
                                SelectionEndIndex = text.Length;
                            }
                        }
                    }
                }
            }

            return text;
        }
    }
}
