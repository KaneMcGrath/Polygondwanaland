using Polygondwanaland.FlatUI5;
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
        public static int GameWidth = 1500;
        public static int GameHeight = 1500;
        public static bool[,] Game = new bool[GameWidth, GameHeight];
        public static bool[,] Game2 = new bool[GameWidth, GameHeight];
        public static bool GameSetup = false;
        public static bool GamePaused = false;
        //public static float zoomLevel = 1f;
        //public static Vector2 CameraPos = new Vector2(0,0);

        private static bool ShowFPS = true;
        private static Color ForegroundColor = new Color(55, 66, 77, 255);
        private static Window SettingsWindow = new Window(new Rect(Raylib.GetScreenWidth() - 300, 30, 300, 800), "Simulation") { showWindow = true, insideColor = new Color(55, 66, 77, 255), constraints = new Constraints(0, 0, 30, 0) };
        private static float MenuBarPosition = -30f;

        private static bool drawDebugText = false;
        private static bool drawDebugNumbers = false;

        private static float frameWaitTimer = 0f;
        private static int stepsPerSecond = 100;
        private static int maxStepsPerSecond = 100;

        public static Camera mainCamera = new Camera(Vector2.Zero, 0f, 1f);

        private static void DrawBorders()
        {
            Vector2 CameraSpace = mainCamera.WorldToScreen(new Vector2(1f, 1f));
            Raylib.DrawRectangle((int)(CameraSpace.X), (int)(CameraSpace.Y), (int)(mainCamera.Scale(18f)), (int)(mainCamera.Scale(18f)), Color.WHITE);
            //Raylib.DrawRectangle((int)(Tools.ScreenCenterX() + (CameraPos.X * zoomLevel) + 1f * zoomLevel), (int)(Tools.ScreenCenterY() + (CameraPos.Y * zoomLevel) + 1f * zoomLevel), (int)(18f * zoomLevel), GameHeight * (int)(20f * zoomLevel), Color.WHITE);
            //Raylib.DrawRectangle((int)(Tools.ScreenCenterX() + (CameraPos.X * zoomLevel) + 20f * GameWidth * zoomLevel), (int)(Tools.ScreenCenterY() + (CameraPos.Y * zoomLevel) + 1f * zoomLevel), (int)(18f * zoomLevel), GameHeight * (int)(20f * zoomLevel), Color.WHITE);
            //Raylib.DrawRectangle((int)(Tools.ScreenCenterX() + (CameraPos.X * zoomLevel) + 1f * zoomLevel), (int)(Tools.ScreenCenterY() + (CameraPos.Y * zoomLevel) + 20f * GameHeight * zoomLevel), GameWidth * (int)(20f * zoomLevel), (int)(18f * zoomLevel), Color.WHITE);
        }

        public static void Update()
        {
            Raylib.ClearBackground(Color.BLACK);
            CameraControl();

            if (GameSetup)
            {
                DrawBorders();

                int minX = 1; //Math.Max(1, (int)(((-1f * CameraPos.X * (zoomLevel) * zoomLevel) - Tools.ScreenCenterX()) / 20f));
                int maxX = GameWidth - 1;
                int minY = 1;
                int maxY = GameHeight - 1;

                FlatUI.Label(new Rect(10, 230, 100, 30), minX.ToString());
                FlatUI.Label(new Rect(10, 260, 100, 30), maxX.ToString());
                FlatUI.Label(new Rect(10, 290, 100, 30), minY.ToString());
                FlatUI.Label(new Rect(10, 320, 100, 30), maxY.ToString());

                for (int x = minX; x < maxX; x++)
                {
                    for (int y = minY; y < maxY; y++)
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
                    //CameraPos = new Vector2(0, 0);
                    mainCamera.Position = new Vector2(0, 0);
                }
                if (FlatUI.Button(SettingsWindow.IndexToRect(7), "Randomize Board"))
                {
                    GameHeight = 902;
                    GameWidth = 902;
                    Game = new bool[GameWidth, GameHeight];
                    Game2 = new bool[GameWidth, GameHeight];
                    CheckedBitmap = new bool[GameWidth, GameHeight];
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
                    GameHeight = 102;
                    GameWidth = 102;
                    Game = new bool[GameWidth, GameHeight];
                    Game2 = new bool[GameWidth, GameHeight];
                    CheckedBitmap = new bool[GameWidth, GameHeight];
                    for (int x = 1; x < GameWidth - 1; x++)
                    {
                        for (int y = 1; y < GameHeight - 1; y++)
                        {
                            Game[x, y] = Tools.Chance(10f);
                        }
                    }
                    GameSetup = true;
                }
                FlatUI.Label(SettingsWindow.IndexToRect(9), "Steps Per Second", 20, 7);
                stepsPerSecond = (int)FlatUI.Slider(SettingsWindow.IndexToRect(10), (float)stepsPerSecond, 1f, (float)maxStepsPerSecond);
            }

            if (drawDebugText) {
                //getMouseCell();
                //FlatUI.Label(new Rect(10, 30, 100, 30), "Zoom:" + zoomLevel.ToString());
                //FlatUI.Label(new Rect(10, 60, 100, 30), "Camera:" + CameraPos.ToString());
                FlatUI.Label(new Rect(10, 30, 100, 30), "Zoom:" + mainCamera.Zoom.ToString());
                FlatUI.Label(new Rect(10, 60, 100, 30), "Camera:" + mainCamera.Position.ToString());
            }
            if (ShowFPS)
            {
                Raylib.DrawFPS(Raylib.GetScreenWidth() - 100, 0);
            }
            if (GameSetup && !GamePaused)
            {
                if (stepsPerSecond >= maxStepsPerSecond)
                {
                    SimulateStep();
                }
                else
                {
                    float waitTime = 1f / (float)stepsPerSecond;
                    if (Tools.timer(ref frameWaitTimer, waitTime))
                    {
                        SimulateStep();
                    }
                }
            }
            //Raylib.DrawRectangle((int)CameraPos.X + (int)(50f * zoomLevel), (int)CameraPos.Y + (int)(50f * zoomLevel), (int)(10f * zoomLevel), (int)(10f * zoomLevel), Color.WHITE);
        }

        private static void getMouseCell()
        {
            //(int)(Tools.ScreenCenterX() + (CameraPos.X * zoomLevel) + 20f * x * zoomLevel), (int)(Tools.ScreenCenterY() + (CameraPos.Y * zoomLevel) + 20f * y * zoomLevel), (int)(zoomLevel * 18f), (int)(zoomLevel * 18f)
            //Vector2 mouse = Raylib.GetMousePosition();
            //int x = Math.Max(1, (int)(((-1f * (CameraPos.X + mouse.X) * zoomLevel)) / 20f));
            //int y = Math.Max(1, (int)(((-1f * (CameraPos.Y + mouse.Y) * zoomLevel)) / 20f));
            //FlatUI.DrawOutline(new Rect(x, y, (int)(zoomLevel * 20f), (int)(zoomLevel * 20f)), 1, Color.ORANGE);
            //
            //FlatUI.Label(new Rect(10, 90, 100, 30), "Mouse Cell X:" + (int)(Tools.ScreenCenterX() + (CameraPos.X * zoomLevel) + 20f * mouse.X * zoomLevel));
            //FlatUI.Label(new Rect(10, 120, 100, 30), "Mouse Cell Y:" + (mouse.X - (CameraPos.X * zoomLevel)).ToString());
        }

        private static bool[,] CheckedBitmap = new bool[GameWidth, GameHeight];
        private static void SimulateStep()
        {
            for (int x = 1; x < GameWidth - 1; x++)
            {
                for (int y = 1; y < GameHeight - 1; y++)
                {
                    CheckedBitmap[x, y] = false;
                }
            }
            for (int x = 1; x < GameWidth - 1; x++)
            {
                for (int y = 1; y < GameHeight - 1; y++)
                {
                    if (Game[x, y])
                    {
                        CheckCell(x, y);
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

        private static void CheckCell(int x, int y)
        {
            int neighbors = 0;
            for (int i = -1; i <= 1; i++)
                for (int j = -1; j <= 1; j++)
                    if (Game[x + i, y + j]) 
                    {
                        neighbors++;
                    }
                    else
                    {
                        CheckForGrowth(x + i, y + j);
                    }

            if (Game[x, y])
            {
                neighbors--;

                if (neighbors < 2) Game2[x, y] = false;
                else if (neighbors > 3) Game2[x, y] = false;
                else Game2[x, y] = true;
            }
            else
            {
                if (neighbors == 3) Game2[x, y] = true;
                else Game2[x, y] = false;
            }
        }

        private static void CheckForGrowth(int x, int y)
        {
            if (x == 0 || y == 0 || x == GameWidth - 1 || y == GameHeight - 1) return;
            int neighbors = 0;
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    if (Game[x + i, y + j])
                    {
                        neighbors++;
                    }
                }
            }
            if (neighbors == 3)
            {
                Game2[x, y] = true;
            }
        }
        
        private static float cellSize = 200;
        private static void DrawCell(int x, int y)
        {
            //Raylib.DrawRectangle((int)(Tools.ScreenCenterX() + (CameraPos.X * zoomLevel) + 20f * x * zoomLevel), (int)(Tools.ScreenCenterY() + (CameraPos.Y * zoomLevel) + 20f * y * zoomLevel), (int)(zoomLevel * 18f), (int)(zoomLevel * 18f), Color.WHITE);
            Vector2 CameraSpace = mainCamera.WorldToScreen(new Vector2(20f * x, 20f * y));
            Raylib.DrawRectangle((int)(CameraSpace.X), (int)(CameraSpace.Y), (int)(mainCamera.Scale(18f)), (int)(mainCamera.Scale(18f)), Color.WHITE);
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
                
                //FlatUI.DrawText(neighbors.ToString(), (int)(Tools.ScreenCenterX() + (CameraPos.X * zoomLevel) + 20f * x * zoomLevel), (int)(Tools.ScreenCenterY() + (CameraPos.Y * zoomLevel) + 20f * y * zoomLevel), 20, lives ? Color.GREEN : Color.RED);
            }
        }



        private static void CameraControl()
        {
            if (Raylib.IsMouseButtonDown(0) && !Raylib.IsKeyDown(KeyboardKey.KEY_LEFT_SHIFT) && !SettingsWindow.isDragging && !FlatUI.IsDraggingSlider && !FlatUI.IsMouseInRect(SettingsWindow.rect))
            {
                //CameraPos += Raylib.GetMouseDelta() / zoomLevel;
                mainCamera.Position -= Raylib.GetMouseDelta() * (1f / mainCamera.Zoom);
            }
            float zoomLevel = Raylib.GetMouseWheelMove() / 10f;
            if (Raylib.IsKeyDown(KeyboardKey.KEY_UP))
            {
                mainCamera.Zoom += 1f * Time.DeltaTime;
            }
            if (Raylib.IsKeyDown(KeyboardKey.KEY_DOWN))
            {
                mainCamera.Zoom -= 1f * Time.DeltaTime;
            }
            //Raylib.DrawCircle((int)View.ConvertXToScreenSpace(1000f), (int)View.ConvertYToScreenSpace(1000f), 10f, Color.GREEN);
            //if (zoomLevel != 0f) View.ScaleFromPoint(1000f, 1000f, zoomLevel);
            //if (View.Scale < 0.1f) View.Scale = 0.1f;
        }
    }
}
