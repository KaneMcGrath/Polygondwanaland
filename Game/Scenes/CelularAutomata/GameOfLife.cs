﻿using Polygondwanaland.FlatUI5;
using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Net.Http.Headers;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Polygondwanaland.Game.Scenes.CelularAutomata
{
    public static class GameOfLife
    {
        public static int GameWidth = 500;
        public static int GameHeight = 500;
        public static bool[,] Game = new bool[GameWidth, GameHeight];
        public static bool[,] Game2 = new bool[GameWidth, GameHeight];
        public static bool GameSetup = false;
        public static bool GamePaused = false;
        public static float zoomLevel = 1f;
        public static Vector2 CameraPos = new Vector2(0,0);

        private static bool ShowFPS = true;
        private static Color ForegroundColor = new Color(55, 66, 77, 255);
        private static Window SettingsWindow = new Window(new Rect(Raylib.GetScreenWidth() - 300, 30, 300, 800), "Simulation") { showWindow = true, insideColor = new Color(55, 66, 77, 255), constraints = new Constraints(0, 0, 30, 0) };
        private static float MenuBarPosition = -30f;

        private static bool drawDebugText = false;
        private static bool drawDebugNumbers = false;

        private static void DrawBorders()
        {
            Raylib.DrawRectangle((int)(Tools.ScreenCenterX() + (CameraPos.X * zoomLevel) + 1f * zoomLevel), (int)(Tools.ScreenCenterY() + (CameraPos.Y * zoomLevel) + 1f * zoomLevel), GameWidth * (int)(20f * zoomLevel), (int)(18f * zoomLevel), Color.WHITE);
            Raylib.DrawRectangle((int)(Tools.ScreenCenterX() + (CameraPos.X * zoomLevel) + 1f * zoomLevel), (int)(Tools.ScreenCenterY() + (CameraPos.Y * zoomLevel) + 1f * zoomLevel), (int)(18f * zoomLevel), GameHeight * (int)(20f * zoomLevel), Color.WHITE);
            Raylib.DrawRectangle((int)(Tools.ScreenCenterX() + (CameraPos.X * zoomLevel) + 20f * GameWidth * zoomLevel), (int)(Tools.ScreenCenterY() + (CameraPos.Y * zoomLevel) + 1f * zoomLevel), (int)(18f * zoomLevel), GameHeight * (int)(20f * zoomLevel), Color.WHITE);
            Raylib.DrawRectangle((int)(Tools.ScreenCenterX() + (CameraPos.X * zoomLevel) + 1f * zoomLevel), (int)(Tools.ScreenCenterY() + (CameraPos.Y * zoomLevel) + 20f * GameHeight * zoomLevel), GameWidth * (int)(20f * zoomLevel), (int)(18f * zoomLevel), Color.WHITE);
        }

        public static void Update()
        {
            Raylib.ClearBackground(Color.BLACK);
            CameraControl();

            if (GameSetup)
            {

                DrawBorders();

                for (int x = 1; x < GameWidth - 1; x++)
                {
                    for (int y = 1; y < GameHeight - 1; y++)
                    {
                        if (Game[x, y])
                            DrawCell(x, y);
                    }
                }
            }

            if (MenuBarPosition > -30f)
            {
                FlatUI.Box(new Rect(0, (int)MenuBarPosition, Raylib.GetScreenWidth(), 30), ForegroundColor);
                if (FlatUI.Button(new Rect(0, (int)MenuBarPosition, 200, 30), "Controls"))
                {
                    SettingsWindow.showWindow = true;
                }
                if (FlatUI.Button(new Rect(Raylib.GetScreenWidth() - 200, (int)MenuBarPosition, 200, 30), "Return To Menu"))
                {
                    KaneGameManager.CurrentScene = 0;
                }
            }
            if (Raylib.GetMousePosition().Y < 30)
            {
                if (MenuBarPosition < 0f)
                {
                    MenuBarPosition += 200 * Raylib.GetFrameTime();
                }
                else
                {
                    MenuBarPosition = 0f;
                }
            }
            else
            {
                if (MenuBarPosition > -30f)
                {
                    MenuBarPosition -= 200 * Raylib.GetFrameTime();
                }
            }

            

            SettingsWindow.OnGUI();
            if (SettingsWindow.ContentVisible())
            {
                
                if (FlatUI.Button(SettingsWindow.IndexToRect(0,6,0), GamePaused ? ">" : "||"))
                {
                    GamePaused = !GamePaused;
                }
                if (FlatUI.Button(SettingsWindow.IndexToRect(0,6,1), "Step"))
                {
                    SimulateStep();
                }
                ShowFPS = FlatUI.Check(SettingsWindow.IndexToRect(2), ShowFPS, "FPS");
                drawDebugText = FlatUI.Check(SettingsWindow.IndexToRect(3), drawDebugText, "DebugText");
                drawDebugNumbers = FlatUI.Check(SettingsWindow.IndexToRect(4), drawDebugNumbers, "DebugNumbers");


                if (FlatUI.Button(SettingsWindow.IndexToRect(6), "Reset Camera"))
                {
                    CameraPos = new Vector2(0, 0);
                }
                if (FlatUI.Button(SettingsWindow.IndexToRect(7), "Randomize Board"))
                {
                    GameHeight = 502;
                    GameWidth = 502;
                    Game = new bool[GameWidth, GameHeight];
                    Game2 = new bool[GameWidth, GameHeight];
                    for (int x = 1; x < GameWidth - 1; x++)
                    {
                        for (int y = 1; y < GameHeight - 1; y++)
                        {
                            Game[x, y] = Tools.Chance(5f);
                        }
                    }
                    GameSetup = true;
                }
                if (FlatUI.Button(SettingsWindow.IndexToRect(8), "Small Board"))
                {
                    GameHeight = 12;
                    GameWidth = 12;
                    Game = new bool[GameWidth, GameHeight];
                    Game2 = new bool[GameWidth, GameHeight];
                    for (int x = 1; x < GameWidth - 1; x++)
                    {
                        for (int y = 1; y < GameHeight - 1; y++)
                        {
                            Game[x, y] = Tools.Chance(10f);
                        }
                    }
                    GameSetup = true;
                }
            }

            if (drawDebugText) { 
                getMouseCell();
                FlatUI.Label(new Rect(10, 30, 100, 30), "Zoom:" + zoomLevel.ToString());
                FlatUI.Label(new Rect(10, 60, 100, 30), "Camera:" + CameraPos.ToString());
            }
            if (ShowFPS)
            {
                Raylib.DrawFPS(Raylib.GetScreenWidth() - 100, 0);
            }
            if (GameSetup && !GamePaused)
                SimulateStep();
            //Raylib.DrawRectangle((int)CameraPos.X + (int)(50f * zoomLevel), (int)CameraPos.Y + (int)(50f * zoomLevel), (int)(10f * zoomLevel), (int)(10f * zoomLevel), Color.WHITE);
        }

        private static void getMouseCell()
        {
            //(int)(Tools.ScreenCenterX() + (CameraPos.X * zoomLevel) + 20f * x * zoomLevel), (int)(Tools.ScreenCenterY() + (CameraPos.Y * zoomLevel) + 20f * y * zoomLevel), (int)(zoomLevel * 18f), (int)(zoomLevel * 18f)
            Vector2 mouse = Raylib.GetMousePosition();

            FlatUI.Label(new Rect(10, 90, 100, 30), "Mouse Cell X:" + (int)(Tools.ScreenCenterX() + (CameraPos.X * zoomLevel) + 20f * mouse.X * zoomLevel));
            FlatUI.Label(new Rect(10, 120, 100, 30), "Mouse Cell Y:" + (mouse.X - (CameraPos.X * zoomLevel)).ToString());
        }

        private static void SimulateStep()
        {
            for (int x = 1; x < GameWidth - 1; x++)
            {
                for (int y = 1; y < GameHeight - 1; y++)
                {
                    int neighbors = 0;
                    for (int i = -1; i <= 1; i++)
                        for (int j = -1; j <= 1; j++)
                            if (Game[x + i,y + j]) neighbors++;

                    //if (Game[x - 1, y - 1]) neighbors++;
                    //if (Game[x    , y - 1]) neighbors++;
                    //if (Game[x + 1, y - 1]) neighbors++;
                    //
                    //if (Game[x - 1, y    ]) neighbors++;
                    //
                    //if (Game[x + 1, y    ]) neighbors++;
                    //
                    //if (Game[x - 1, y + 1]) neighbors++;
                    //if (Game[x    , y + 1]) neighbors++;
                    //if (Game[x + 1, y + 1]) neighbors++;

                    if (Game[x, y])
                    {
                        neighbors--;
                        
                        if (neighbors < 2)      Game2[x, y] = false;
                        else if (neighbors > 3) Game2[x, y] = false;
                        else                    Game2[x, y] = true;
                    }
                    else
                    {
                        if (neighbors == 3) Game2[x, y] = true;
                        else                Game2[x, y] = false;
                    }
                }
            }
            for (int x = 1; x < GameWidth - 1; x++)
            {
                for (int y = 1; y < GameHeight - 1; y++)
                {
                    Game[x,y] = Game2[x, y];
                }
            }
                    
        }
        
        private static float cellSize = 200;
        private static void DrawCell(int x, int y)
        {
            Raylib.DrawRectangle((int)(Tools.ScreenCenterX() + (CameraPos.X * zoomLevel) + 20f * x * zoomLevel), (int)(Tools.ScreenCenterY() + (CameraPos.Y * zoomLevel) + 20f * y * zoomLevel), (int)(zoomLevel * 18f), (int)(zoomLevel * 18f), Color.WHITE);
            if (drawDebugNumbers)
            {
                int neighbors = 0;
                for (int i = -1; i <= 1; i++)
                    for (int j = -1; j <= 1; j++)
                        if (Game[x + i, y + j]) neighbors++;
                
                if (Game[x, y])
                    neighbors--;
                
                bool lives = false;
                
                if (Game[x, y])
                {
                
                    if (neighbors < 2) lives = false;
                    else if (neighbors > 3) lives = false;
                    else lives = true;
                }
                else
                {
                    if (neighbors == 3) lives = true;
                    else lives = false;
                }
                
                FlatUI.DrawText(neighbors.ToString(), (int)(Tools.ScreenCenterX() + (CameraPos.X * zoomLevel) + 20f * x * zoomLevel), (int)(Tools.ScreenCenterY() + (CameraPos.Y * zoomLevel) + 20f * y * zoomLevel), 20, lives ? Color.GREEN : Color.RED);
            }
        }


        private static void CameraControl()
        {

            if (Raylib.IsMouseButtonDown(0) && !Raylib.IsKeyDown(KeyboardKey.KEY_LEFT_SHIFT) && !SettingsWindow.isDragging)
            {
                CameraPos += Raylib.GetMouseDelta() / zoomLevel;
            }
            float preZoom = zoomLevel;
            zoomLevel += Raylib.GetMouseWheelMove() / 10f;
            if (zoomLevel < 0.1f) zoomLevel = 0.1f;
        }
    }
}