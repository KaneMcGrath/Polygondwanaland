using Polygondwanaland.FlatUI5;
using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Polygondwanaland.Game.Scenes.BattlePong
{
    //Battle pong a game based on some random post i saw on HN
    //two sides are defined by color and split down the middle divided into a grid
    //each side has a ball and when a ball hits the other sides color it converts it to one side
    public static class BattlePong
    {
        public static int[,] GameBoard;  //2d bool array representing the state of the board
        public static float CellSize = 10f; //the size of every cell on the board
        public static int GameXSize = 32;
        public static int GameYSize = 32;

        public static Color[] TeamColors;

        public static PlayerBall Player1;
        public static PlayerBall Player2;
        public static Camera MainCamera = new Camera(Vector2.Zero, 0f, 1f);

        public static bool GameSetup = false;
        public static bool GamePaused = false;

        private static Color ForegroundColor = new Color(55, 66, 77, 255);
        private static Window SettingsWindow = new Window(new Rect(Raylib.GetScreenWidth() - 300, 30, 300, 800), "Simulation") { showWindow = true, insideColor = new Color(55, 66, 77, 255), constraints = new Constraints(0, 0, 30, 0) };
        private static float MenuBarPosition = -30f;

        public static float GameSpeed = 1f;

        public static void SetUpGame()
        {
            GameBoard = new int[GameXSize, GameYSize];
            int xhalf = GameXSize / 2;
            int yhalf = GameYSize / 2;
            for (int y = 0; y < GameYSize; y++) 
            { 
                for (int x = 0; x < GameXSize; x++)
                {
                    if (x < xhalf) GameBoard[x, y] = 0;
                    else GameBoard[x, y] = 1;
                }
            }
            TeamColors = new Color[] { Color.RED, Color.BLUE };
            Vector2 P1Spawn = GetCellPos(0, yhalf);
            Vector2 P2Spawn = GetCellPos(GameYSize, yhalf);
            Player1 = new PlayerBall(P1Spawn.X,P1Spawn.Y, 135, 135, 0);
            Player2 = new PlayerBall(P2Spawn.X, P2Spawn.Y, -135, -135, 1);
            GameSetup = true;
        }

        public static void GameLogic()
        {

        }

        public static void Update()
        {
            Raylib.ClearBackground(Color.BLACK);
            if (GameSetup)
            {
                MainCamera.CameraControls();
                if (!GamePaused)
                {
                    UpdatePlayer(Player1);
                    UpdatePlayer(Player2);
                }
                for (int y = 0; y < GameBoard.GetLength(0); y++)
                {
                    for (int x = 0; x < GameBoard.GetLength(1); x++)
                    {
                        RenderCell(x, y);
                    }
                }
                RenderPlayer(Player1);
                RenderPlayer(Player2);
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
                if (FlatUI.Button(SettingsWindow.IndexToRect(0, 6, 0), GamePaused ? ">" : "||"))
                {
                    GamePaused = !GamePaused;
                }
                KaneGameManager.DrawFPS = FlatUI.Check(SettingsWindow.IndexToRect(2), KaneGameManager.DrawFPS, "FPS");
                if (FlatUI.Button(SettingsWindow.IndexToRect(3), "StartGame"))
                {
                    SetUpGame();
                    GameSetup = true;
                }
                GameSpeed = (int)FlatUI.Slider(SettingsWindow.IndexToRect(10), GameSpeed, 0.1f, 50f);
            }
        }


        public static void RenderCell(int x, int y)
        {
            int team = GetCellTeam(new XY(x, y));
            Vector2 CameraSpace = MainCamera.WorldToScreen(new Vector2(CellSize * x, CellSize * y));
            Raylib.DrawRectangle((int)(CameraSpace.X), (int)(CameraSpace.Y), (int)(MainCamera.Scale(CellSize - 0.1f)), (int)(MainCamera.Scale(CellSize - 0.1f)), TeamColors[team]);
        }

        public static void RenderPlayer(PlayerBall Player)
        {
            Vector2 CameraSpace = MainCamera.WorldToScreen(Player.Position);
            Raylib.DrawCircle((int)(CameraSpace.X), (int)(CameraSpace.Y), MainCamera.Scale(1f), Color.WHITE);
        }

        private static Vector2 GetCellPos(int x, int y)
        {
            return new Vector2(x * CellSize, y * CellSize);
        }

        private static XY GetCell(Vector2 pos)
        {
            return new XY((int)(pos.X / CellSize), (int)(pos.Y / CellSize));
        }

        private static bool CellValid(XY Cell)
        {
            return (Cell.X >= 0 && Cell.Y >= 0 && Cell.X < GameXSize && Cell.Y < GameYSize);
        }

        private static void SetCellTeam(XY Cell, int team)
        {
            GameBoard[Cell.X, Cell.Y] = team;
        }

        private static int GetCellTeam(XY Cell)
        {
            if (CellValid(Cell))
            {
                return GameBoard[Cell.X, Cell.Y];
            }
            else
            {
                return -1;
            }
        }

        private static void UpdatePlayer(PlayerBall player)
        {
            //Calculate next position
            Vector2 NextPosition = player.Position + player.Velocity * Time.DeltaTime * GameSpeed;
            //check if next position is in bounds
            if (NextPosition.X < 0)
            {
                player.Velocity.X = -player.Velocity.X;
                NextPosition.X = 0;
            }
            if (NextPosition.Y < 0)
            {
                player.Velocity.Y = -player.Velocity.Y;
                NextPosition.Y = 0;
            }
            if (NextPosition.X > GameXSize * CellSize)
            {
                player.Velocity.X = -player.Velocity.X;
                NextPosition.X = GameXSize * CellSize;
            }
            if (NextPosition.Y > GameYSize * CellSize)
            {
                player.Velocity.Y = -player.Velocity.Y;
                NextPosition.Y = GameXSize * CellSize;
            }
            XY CurrentCell = GetCell(player.Position);
            XY NextCell = GetCell(NextPosition);
            int NextCellTeam = GetCellTeam(NextCell);
            if (CurrentCell != NextCell && CellValid(NextCell) && NextCellTeam != player.team)
            {
                //determine what side of the box the ball is hitting
                bool left = (CurrentCell.X < NextCell.X);
                bool right = (CurrentCell.X > NextCell.X);
                bool top = (CurrentCell.Y > NextCell.Y);
                bool bottom = (CurrentCell.Y < NextCell.Y);
                if (left && right || top && bottom)
                {
                    throw new Exception("Player hit box from two sides at once");
                }
                if (left || right)
                {
                    player.Velocity.X = -player.Velocity.X;
                }
                if (top || bottom)
                {
                    player.Velocity.Y = -player.Velocity.Y;
                }
                NextPosition = player.Position + player.Velocity * Time.DeltaTime * GameSpeed;
                SetCellTeam(NextCell, player.team);
                
            }
            player.Position = NextPosition;
            
        }

    }

    public class PlayerBall
    {
        public Vector2 Position;
        public Vector2 Velocity;
        public int team;

        public PlayerBall(float x, float y, float vx, float vy, int team) 
        { 
            Position = new Vector2(x, y);
            Velocity = new Vector2(vx, vy);
            this.team = team;
        }

    }
}
