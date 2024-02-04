using Polygondwanaland.FlatUI5;
using Polygondwanaland.Game.Scenes.OrbitTesting;
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
        public static int GameXSize = 40;
        public static int GameYSize = 40;

        public static Color[] TeamColors;

        public static PlayerBall Player1;
        public static PlayerBall Player2;
        public static Camera MainCamera = new Camera(Vector2.Zero, 0f, 1f);

        public static bool GameSetup = false;
        public static bool GamePaused = false;

        public static int frameCount = 0;

        private static Color ForegroundColor = new Color(55, 66, 77, 255);
        private static Window SettingsWindow = new Window(new Rect(Raylib.GetScreenWidth() - 300, 30, 300, 800), "Simulation") { showWindow = true, insideColor = new Color(55, 66, 77, 255), constraints = new Constraints(0, 0, 30, 0) };
        private static float MenuBarPosition = -30f;

        private static int stepsPerSecond = 100;
        private static int maxStepsPerSecond = 1000;
        private static float frameWaitTimer = 0f;

        private static bool DrawDebug = true;
        private static bool DebugIsNextCaseCorner = false;
        private static bool[] DebugLRTB = new bool[4];

        private static XY DebugCheckCell1 = new XY(0, 0);
        private static XY DebugCheckCell2 = new XY(0, 0);

        private static string GoToFrameTextBox = "0";
        private static int frameToGoTo = 0;
        private static bool GoToFrame = false;

        private static string[] TextBoxes = new string[] { "1", "1", "-1", "-1", "32", "32" };
        private static float[] PlayerVelocities = new float[] { 1f,1f,-1f,-1f };
        private static int[] GameSettings = new int[] { 32, 32 };
        public static float Score = 0f; //representation of the total area of the board a player controls from -1 to 1
        private static int debugTeam1Tiles = 0;
        private static int debugTeam2Tiles = 0;

        private static Random rand;

        public static void SetUpGame()
        {
            GameXSize = GameSettings[0];
            GameYSize = GameSettings[1];
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
            Vector2 P2Spawn = GetCellPos(GameXSize, xhalf);
            Player1 = new PlayerBall(P1Spawn.X, P1Spawn.Y, PlayerVelocities[0], PlayerVelocities[1], 0);
            Player2 = new PlayerBall(P2Spawn.X, P2Spawn.Y, PlayerVelocities[2], PlayerVelocities[3], 1);
            rand = new Random(1); //static seed for determinism
            GameSetup = true;
            frameCount = 0;
            Score = 0f;
        }
        
        /// <summary>
        /// Count the tiles held by Team 1 vs Team 2 and return a value from 0 to 1 where 0 is team 1 and 1 is team 2
        /// </summary>
        /// <returns></returns>
        public static float CalculateScore()
        {
            int team1 = 0;
            int team2 = 0;
            //count cells for each team
            for (int x = 0; x < GameBoard.GetLength(0); x++)
            {
                for (int y = 0; y < GameBoard.GetLength(1); y++)
                {
                    if (GameBoard[x, y] == 0)
                    {
                        team1++;
                    }
                    else
                    {
                        team2++;
                    }
                }
            }

            debugTeam1Tiles = team1;
            debugTeam2Tiles = team2;

            //get Percentage of team 1 territory from 0-1
            return (float)team1 / (float)(team1 + team2);

        }

        public static void SimulateStep()
        {
            UpdatePlayer(Player1);
            UpdatePlayer(Player2);
            if (DrawDebug)
            {
                DebugUpdate(Player2);
                DebugUpdate(Player1);
            }
            frameCount++;
        }

        public static void Update()
        {
            Raylib.ClearBackground(Color.BLACK);
            if (Raylib.IsKeyPressed(KeyboardKey.KEY_RIGHT))
            {
                SimulateStep();
            }
            if (GameSetup && !GamePaused)
            {
                if (GoToFrame)
                {
                    if (frameCount >= frameToGoTo)
                    {
                        GoToFrame = false;
                        GamePaused = true;
                    }
                }
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
            if (GameSetup)
            {
                MainCamera.CameraControls(!SettingsWindow.isDragging && !FlatUI.IsDraggingSlider && !FlatUI.IsMouseInRect(SettingsWindow.rect));
                
                for (int y = 0; y < GameYSize; y++)
                {
                    for (int x = 0; x < GameXSize; x++)
                    {
                        RenderCell(x, y);
                    }
                }
                RenderPlayer(Player1);
                RenderPlayer(Player2);
                if (DrawDebug)
                {
                    FlatUI.Label(new Rect(10, 10, 200, 30), "Pos: " + Player1.Position.ToString());
                    FlatUI.Label(new Rect(10, 40, 200, 30), "Cell: " + Player1.DebugCurrentCell.ToString());
                    FlatUI.Label(new Rect(10, 70, 200, 30), "NextCell: " + Player1.DebugNextCell.ToString());
                    FlatUI.Label(new Rect(10, 100, 200, 30), "SameCell: " + (Player1.DebugCurrentCell == Player1.DebugNextCell).ToString());
                    FlatUI.Label(new Rect(10, 130, 200, 30), "IsCorner: " + DebugIsNextCaseCorner.ToString());
                    FlatUI.Label(new Rect(10, 160, 400, 30), "CornerCases: [Left:" + DebugLRTB[0] + "] [Right:" + DebugLRTB[1] + "] Top:[" + DebugLRTB[2] + "] [Bottom:" + DebugLRTB[3] + "]");
                    FlatUI.Label(new Rect(10, 190, 400, 30), "Frame: " + frameCount);
                    FlatUI.Label(new Rect(10, 220, 400, 30), "Score: " + CalculateScore());
                    FlatUI.Label(new Rect(10, 250, 400, 30), "Team1: " + debugTeam1Tiles);
                    FlatUI.Label(new Rect(10, 280, 400, 30), "Team2: " + debugTeam2Tiles);
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
                if (FlatUI.Button(SettingsWindow.IndexToRect(0, 6, 0), GamePaused ? ">" : "||"))
                {
                    GamePaused = !GamePaused;
                }
                if (FlatUI.Button(SettingsWindow.IndexToRect(0, 6, 1), "Step"))
                {
                    SimulateStep();
                }
                FlatUI.Label(SettingsWindow.IndexToRect(1), "Go To Frame", 20, 7);
                GoToFrameTextBox = FlatUI.TextField(SettingsWindow.IndexToRect(2, 3, 0, 2), GoToFrameTextBox);
                
                if (FlatUI.Button(SettingsWindow.IndexToRect(2,3,1), "Go"))
                {
                    GoToFrame = true;
                    frameToGoTo = Convert.ToInt32(GoToFrameTextBox);
                    SetUpGame();
                    GameSetup = true;
                    return;
                }
                KaneGameManager.DrawFPS = FlatUI.Check(SettingsWindow.IndexToRect(3), KaneGameManager.DrawFPS, "FPS");
                PlayerVelocities[0] = FlatUI.NumberSelector(SettingsWindow.IndexToRect(4), "Player1 VX", PlayerVelocities[0], ref TextBoxes[0], 0.1f);
                PlayerVelocities[1] = FlatUI.NumberSelector(SettingsWindow.IndexToRect(5), "Player1 VY", PlayerVelocities[1], ref TextBoxes[1], 0.1f);
                PlayerVelocities[2] = FlatUI.NumberSelector(SettingsWindow.IndexToRect(6), "Player2 VX", PlayerVelocities[2], ref TextBoxes[2], 0.1f);
                PlayerVelocities[3] = FlatUI.NumberSelector(SettingsWindow.IndexToRect(7), "Player2 VY", PlayerVelocities[3], ref TextBoxes[3], 0.1f);
                GameSettings[0] = (int)FlatUI.NumberSelector(SettingsWindow.IndexToRect(8), "Player2 VY", (float)GameSettings[0], ref TextBoxes[4], 1f);
                GameSettings[1] = (int)FlatUI.NumberSelector(SettingsWindow.IndexToRect(9), "Player2 VY", (float)GameSettings[1], ref TextBoxes[5], 1f);
                if (FlatUI.Button(SettingsWindow.IndexToRect(10), "StartGame"))
                {
                    SetUpGame();
                    GameSetup = true;
                    return;
                }
                DrawDebug = FlatUI.Check(SettingsWindow.IndexToRect(11), DrawDebug, "Debug");
                FlatUI.Label(SettingsWindow.IndexToRect(12), "Steps Per Second", 20, 7);
                stepsPerSecond = (int)FlatUI.Slider(SettingsWindow.IndexToRect(13), (float)stepsPerSecond, 1f, (float)maxStepsPerSecond);
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

            if (DrawDebug)
            {
                Vector2 CameraSpace2 = MainCamera.WorldToScreen(Player.DebugNextPosition);
                Raylib.DrawCircle((int)(CameraSpace2.X), (int)(CameraSpace2.Y), MainCamera.Scale(1f), Color.GREEN);

                Vector2 Cell = MainCamera.WorldToScreen(new Vector2(CellSize * Player.DebugCurrentCell.X, CellSize * Player.DebugCurrentCell.Y));
                Raylib.DrawRectangleLines((int)(Cell.X - 0.05), (int)(Cell.Y - 0.05), (int)(MainCamera.Scale(CellSize)), (int)(MainCamera.Scale(CellSize)), Color.WHITE);

                if (Player.DebugCurrentCell != Player.DebugNextCell)
                {
                    Vector2 Cell2 = MainCamera.WorldToScreen(new Vector2(CellSize * Player.DebugNextCell.X, CellSize * Player.DebugNextCell.Y));
                    Raylib.DrawRectangleLines((int)(Cell2.X - 0.05), (int)(Cell2.Y - 0.05), (int)(MainCamera.Scale(CellSize)), (int)(MainCamera.Scale(CellSize)), Color.MAGENTA);
                }

                if (DebugIsNextCaseCorner)
                {
                    Vector2 Check1 = MainCamera.WorldToScreen(new Vector2(CellSize * DebugCheckCell1.X, CellSize * DebugCheckCell1.Y));
                    Raylib.DrawRectangleLines((int)(Check1.X - 0.05), (int)(Check1.Y - 0.05), (int)(MainCamera.Scale(CellSize)), (int)(MainCamera.Scale(CellSize)), Color.ORANGE);
                    Vector2 Check2 = MainCamera.WorldToScreen(new Vector2(CellSize * DebugCheckCell2.X, CellSize * DebugCheckCell2.Y));
                    Raylib.DrawRectangleLines((int)(Check2.X - 0.05), (int)(Check2.Y - 0.05), (int)(MainCamera.Scale(CellSize)), (int)(MainCamera.Scale(CellSize)), Color.LIME);
                }
            }
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

        private static void DebugUpdate(PlayerBall player)
        {
            DebugLRTB[0] = false;
            DebugLRTB[1] = false;
            DebugLRTB[2] = false;
            DebugLRTB[3] = false;
            DebugIsNextCaseCorner = false;
            Vector2 NextPosition = player.Position + player.Velocity;
            //check if next position is in bounds

            if (NextPosition.X < 0)
            {
                //player.Velocity.X = -player.Velocity.X;
                NextPosition.X = 0.01f;
                //NextPosition.Y = player.Position.Y;
            }
            if (NextPosition.Y < 0)
            {
                //player.Velocity.Y = -player.Velocity.Y;
                NextPosition.Y = 0.01f;
                //NextPosition.X = player.Position.X;
            }
            if (NextPosition.X > GameXSize * CellSize)
            {
                //player.Velocity.X = -player.Velocity.X;
                NextPosition.X = GameXSize * CellSize - 0.01f;
                // NextPosition.Y = player.Position.Y;
            }
            if (NextPosition.Y > GameYSize * CellSize)
            {
                //player.Velocity.Y = -player.Velocity.Y;
                NextPosition.Y = GameYSize * CellSize - 0.01f;
                // NextPosition.X = player.Position.X;
            }
            player.DebugNextPosition = NextPosition;
            player.DebugCurrentCell = GetCell(player.Position);
            player.DebugNextCell = GetCell(NextPosition);
            XY CurrentCell = GetCell(player.Position);
            XY NextCell = GetCell(NextPosition);
            int NextCellTeam = GetCellTeam(NextCell);

            if (CurrentCell != NextCell && CellValid(NextCell) && NextCellTeam != player.team)
            {
                bool left = (CurrentCell.X < NextCell.X);
                bool right = (CurrentCell.X > NextCell.X);
                bool top = (CurrentCell.Y < NextCell.Y);
                bool bottom = (CurrentCell.Y > NextCell.Y);
                DebugLRTB[0] = left;
                DebugLRTB[1] = right;
                DebugLRTB[2] = top;
                DebugLRTB[3] = bottom;
                //if (left) throw new Exception("Player hit box from two sides at once");
                if (left && right || top && bottom)
                {
                    throw new Exception("Player hit box from two sides at once");
                }
                
                //check for corner cases so we dont slip through gaps or bounce wrong
                if ((left || right) && (top || bottom)) //if we hit a corner
                {
                    DebugIsNextCaseCorner = true;
                    XY CheckCell1 = NextCell;
                    XY CheckCell2 = NextCell;
                    if (left)
                    {
                        CheckCell1.X -= 1;
                    }
                    else
                    {
                        CheckCell1.X += 1;
                    }
                    if (top)
                    {
                        CheckCell2.Y -= 1;
                    }
                    else
                    {
                        CheckCell2.Y += 1;
                    }
                    DebugCheckCell1 = CheckCell1;
                    DebugCheckCell2 = CheckCell2;
                    bool cell1 = (CellValid(CheckCell1) && GetCellTeam(CheckCell1) != player.team);
                    bool cell2 = (CellValid(CheckCell2) && GetCellTeam(CheckCell2) != player.team);
                    if (cell1 && cell2) //real corner set both check cells and ignore corner cell
                    {
                        //SetCellTeam(CheckCell1, player.team);
                        //SetCellTeam(CheckCell2, player.team);
                        //player.Velocity = -player.Velocity;
                    }
                    else if (cell1) //Cell1 only checks horizontal so we bounce up a floor and set nextCell
                    {
                        //SetCellTeam(CheckCell1, player.team);
                        //player.Velocity.Y = -player.Velocity.Y;
                    }
                    else if (cell2) //Cell2 only checks Vertical so we bounce off a wall and set nextCell
                    {
                        //player.Velocity.X = -player.Velocity.X;
                        //SetCellTeam(CheckCell2, player.team);
                    }
                }
                else // no corner case we just hit the side of a cell
                {
                    if (left || right)
                    {
                        //player.Velocity.X = -player.Velocity.X;
                    }
                    if (top || bottom)
                    {
                        //player.Velocity.Y = -player.Velocity.Y;
                    }
                    //SetCellTeam(NextCell, player.team);
                }

            }
        }

        private static void UpdatePlayer(PlayerBall player)
        {
            //Calculate next position
            Vector2 NextPosition = player.Position + player.Velocity;
            //check if next position is in bounds
            
            if (NextPosition.X < 0)
            {
                player.Velocity.X = -player.Velocity.X;
                NextPosition.X = 0.01f;
                //NextPosition.Y = player.Position.Y;
            }
            if (NextPosition.Y < 0)
            {
                player.Velocity.Y = -player.Velocity.Y;
                NextPosition.Y = 0.01f;
                //NextPosition.X = player.Position.X;
            }
            if (NextPosition.X > GameXSize * CellSize)
            {
                player.Velocity.X = -player.Velocity.X;
                NextPosition.X = GameXSize * CellSize - 0.01f;
                //NextPosition.Y = player.Position.Y;
            }
            if (NextPosition.Y > GameYSize * CellSize)
            {
                player.Velocity.Y = -player.Velocity.Y;
                NextPosition.Y = GameYSize * CellSize - 0.01f;
                //NextPosition.X = player.Position.X;
            }
            XY CurrentCell = GetCell(player.Position);
            XY NextCell = GetCell(NextPosition);
            int NextCellTeam = GetCellTeam(NextCell);
            if (CurrentCell != NextCell && CellValid(NextCell) && NextCellTeam != player.team)
            {
                //determine what side of the box the ball is hitting
                bool left = (CurrentCell.X < NextCell.X);
                bool right = (CurrentCell.X > NextCell.X);
                bool top = (CurrentCell.Y < NextCell.Y);
                bool bottom = (CurrentCell.Y > NextCell.Y);
                if (left && right || top && bottom)
                {
                    throw new Exception("Player hit box from two sides at once");
                }

                //check for corner cases so we dont slip through gaps or bounce wrong
                if ((left || right) && (top || bottom)) //if we hit a corner
                {
                    XY CheckCell1 = NextCell;
                    XY CheckCell2 = NextCell;
                    if (left)
                    {
                        CheckCell1.X -= 1;
                    }
                    else
                    {
                        CheckCell1.X += 1;
                    }
                    if (top)
                    {
                        CheckCell2.Y -= 1;
                    }
                    else
                    {
                        CheckCell2.Y += 1;
                    }
                    bool cell1 = (CellValid(CheckCell1) && GetCellTeam(CheckCell1) != player.team);
                    bool cell2 = (CellValid(CheckCell2) && GetCellTeam(CheckCell2) != player.team);
                    if (cell1 && cell2) //real corner set both check cells and ignore corner cell
                    {
                        SetCellTeam(CheckCell1, player.team);
                        SetCellTeam(CheckCell2, player.team);
                        player.Velocity = -player.Velocity;
                    }
                    else if (cell1) //Cell1 only checks horizontal so we bounce up a floor and set nextCell
                    {
                        player.Velocity.Y = -player.Velocity.Y;
                        SetCellTeam(NextCell, player.team);
                    }
                    else if (cell2) //Cell2 only checks Vertical so we bounce off a wall and set nextCell
                    {
                        player.Velocity.X = -player.Velocity.X;
                        SetCellTeam(NextCell, player.team);
                    }
                    else //Hit an outside corner.  Bounce Back and set next cell
                    {
                        player.Velocity.Y = -player.Velocity.Y;
                        player.Velocity.X = -player.Velocity.X;
                        SetCellTeam(NextCell, player.team);
                    }

                    //instead of switching the cornered cell, switch the two adjacent cells
                }
                else // no corner case we just hit the side of a cell
                {
                    if (left || right)
                    {
                        player.Velocity.X = -player.Velocity.X;
                    }
                    if (top || bottom)
                    {
                        player.Velocity.Y = -player.Velocity.Y;
                    }
                    SetCellTeam(NextCell, player.team);
                }

                //NextPosition = player.Position + player.Velocity;
                
                
            }
            player.Position = NextPosition;
            
        }

    }

    public class PlayerBall
    {
        public Vector2 Position;
        public Vector2 Velocity;
        public int team;
        public Vector2 DebugNextPosition;
        public XY DebugCurrentCell;
        public XY DebugNextCell;
        public PlayerBall(float x, float y, float vx, float vy, int team) 
        { 
            Position = new Vector2(x, y);
            Velocity = new Vector2(vx, vy);
            this.team = team;
        }

    }
}
