using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Polygondwanaland.Game
{
    /// <summary>
    /// Handle The Inputbuffer and and add helpful functions to get keys and ignore when typing
    /// </summary>
    public static class InputManager
    {
        public static bool[] inputs = new bool[400]; //one for every key defined in input.cs

        public static Dictionary<int, char> charDict = new Dictionary<int, char>();
        public static void Init()
        {
            // Alphanumeric keys
            charDict[39] = '\''; //KEY_APOSTROPHE = 39,
            charDict[44] = ',';  //KEY_COMMA = 44,
            charDict[45] = '-';  //KEY_MINUS = 45,
            charDict[46] = '.';  //KEY_PERIOD = 46,
            charDict[47] = '/';  //KEY_SLASH = 47,
            charDict[48] = '0';  //KEY_ZERO = 48,
            charDict[49] = '1';  //KEY_ONE = 49,
            charDict[50] = '2';  //KEY_TWO = 50,
            charDict[51] = '3';  //KEY_THREE = 51,
            charDict[52] = '4';  //KEY_FOUR = 52,
            charDict[53] = '5';  //KEY_FIVE = 53,
            charDict[54] = '6';  //KEY_SIX = 54,
            charDict[55] = '7';  //KEY_SEVEN = 55,
            charDict[56] = '8';  //KEY_EIGHT = 56,
            charDict[57] = '9';  //KEY_NINE = 57,
            charDict[59] = ';';  //KEY_SEMICOLON = 59,
            charDict[61] = '=';  //KEY_EQUAL = 61,
            charDict[65] = 'a';  //KEY_A = 65,
            charDict[66] = 'b';  //KEY_B = 66,
            charDict[67] = 'c';  //KEY_C = 67,
            charDict[68] = 'd';  //KEY_D = 68,
            charDict[69] = 'e';  //KEY_E = 69,
            charDict[70] = 'f';  //KEY_F = 70,
            charDict[71] = 'g';  //KEY_G = 71,
            charDict[72] = 'h';  //KEY_H = 72,
            charDict[73] = 'i';  //KEY_I = 73,
            charDict[74] = 'j';  //KEY_J = 74,
            charDict[75] = 'k';  //KEY_K = 75,
            charDict[76] = 'l';  //KEY_L = 76,
            charDict[77] = 'm';  //KEY_M = 77,
            charDict[78] = 'n';  //KEY_N = 78,
            charDict[79] = 'o';  //KEY_O = 79,
            charDict[80] = 'p';  //KEY_P = 80,
            charDict[81] = 'q';  //KEY_Q = 81,
            charDict[82] = 'r';  //KEY_R = 82,
            charDict[83] = 's';  //KEY_S = 83,
            charDict[84] = 't';  //KEY_T = 84,
            charDict[85] = 'u';  //KEY_U = 85,
            charDict[86] = 'v';  //KEY_V = 86,
            charDict[87] = 'w';  //KEY_W = 87,
            charDict[88] = 'x';  //KEY_X = 88,
            charDict[89] = 'y';  //KEY_Y = 89,
            charDict[90] = 'z';  //KEY_Z = 90,

                               // Function keys
            charDict[32] = ' ';  //KEY_SPACE = 32,
            //charDict[256] = '-';  //KEY_ESCAPE = 256,
            //charDict[257] = '-';  //KEY_ENTER = 257,
            //charDict[258] = ' ';  //KEY_TAB = 258,
            //charDict[259] = '-';  //KEY_BACKSPACE = 259,
            //charDict[260] = '-';  //KEY_INSERT = 260,
            //charDict[261] = '-';  //KEY_DELETE = 261,
            //charDict[262] = '-';  //KEY_RIGHT = 262,
            //charDict[263] = '-';  //KEY_LEFT = 263,
            //charDict[264] = '-';  //KEY_DOWN = 264,
            //charDict[265] = '-';  //KEY_UP = 265,
            //charDict[266] = '-';  //KEY_PAGE_UP = 266,
            //charDict[267] = '-';  //KEY_PAGE_DOWN = 267,
            //charDict[268] = '-';  //KEY_HOME = 268,
            //charDict[269] = '-';  //KEY_END = 269,
            //charDict[280] = '-';  //KEY_CAPS_LOCK = 280,
            //charDict[281] = '-';  //KEY_SCROLL_LOCK = 281,
            //charDict[282] = '-';  //KEY_NUM_LOCK = 282,
            //charDict[283] = '-';  //KEY_PRINT_SCREEN = 283,
            //charDict[284] = '-';  //KEY_PAUSE = 284,
            //charDict[290] = '-';  //KEY_F1 = 290,
            //charDict[291] = '-';  //KEY_F2 = 291,
            //charDict[292] = '-';  //KEY_F3 = 292,
            //charDict[293] = '-';  //KEY_F4 = 293,
            //charDict[294] = '-';  //KEY_F5 = 294,
            //charDict[295] = '-';  //KEY_F6 = 295,
            //charDict[296] = '-';  //KEY_F7 = 296,
            //charDict[297] = '-';  //KEY_F8 = 297,
            //charDict[298] = '-';  //KEY_F9 = 298,
            //charDict[299] = '-';  //KEY_F10 = 299,
            //charDict[300] = '-';  //KEY_F11 =              300,
            //charDict[301] = '-';  //KEY_F12 =              301,
            //charDict[340] = '-';  //KEY_LEFT_SHIFT =       340,
            //charDict[341] = '-';  //KEY_LEFT_CONTROL =     341,
            //charDict[342] = '-';  //KEY_LEFT_ALT =         342,
            //charDict[343] = '-';  //KEY_LEFT_SUPER =       343,
            //charDict[344] = '-';  //KEY_RIGHT_SHIFT =      344,
            //charDict[345] = '-';  //KEY_RIGHT_CONTROL =    345,
            //charDict[346] = '-';  //KEY_RIGHT_ALT =        346,
            //charDict[347] = '-';  //KEY_RIGHT_SUPER =      347,
            //charDict[348] = '-';  //KEY_KB_MENU =          348,
            charDict[91] = '[';  //KEY_LEFT_BRACKET =      91,
            charDict[92] = '\\';  //KEY_BACKSLASH = 92,
            charDict[93] = ']';  //KEY_RIGHT_BRACKET = 93,
            charDict[96] = '`';  //KEY_GRAVE = 96,

                                      // Keypad keys
            charDict[320] = '0';  //KEY_KP_0 = 320,
            charDict[321] = '1';  //KEY_KP_1 = 321,
            charDict[322] = '2';  //KEY_KP_2 = 322,
            charDict[323] = '3';  //KEY_KP_3 = 323,
            charDict[324] = '4';  //KEY_KP_4 = 324,
            charDict[325] = '5';  //KEY_KP_5 = 325,
            charDict[326] = '6';  //KEY_KP_6 = 326,
            charDict[327] = '7';  //KEY_KP_7 = 327,
            charDict[328] = '8';  //KEY_KP_8 = 328,
            charDict[329] = '9';  //KEY_KP_9 = 329,
            charDict[330] = '.';  //KEY_KP_DECIMAL =   330,
            charDict[331] = '/';  //KEY_KP_DIVIDE =    331,
            charDict[332] = '*';  //KEY_KP_MULTIPLY =  332,
            charDict[333] = '-';  //KEY_KP_SUBTRACT =  333,
            charDict[334] = '+';  //KEY_KP_ADD =       334,
            //charDict[335] = '-';  //KEY_KP_ENTER =     335,
            charDict[336] = '=';  //KEY_KP_EQUAL =     336,

                                   // Android key buttons
            //charDict[4] = '-';  //KEY_BACK = 4,
            //charDict[8] = '-';  //KEY_MENU = 82,
            //charDict[24] = '-';  //KEY_VOLUME_UP = 24,
            //charDict[25] = '-';  //KEY_VOLUME_DOWN = 25
        }


        public static void Update()
        {
            for (int i = 0; i < 400; i++)
            {
                inputs[i] = false;
            }
            
            while (true)
            {
                int k = Raylib.GetKeyPressed();
                if (k != 0)
                {
                    inputs[k] = true;
                }
                else
                {
                    break;
                }
            }
        }

        //public static char[] GetChars()
        //{
        //
        //}
        //
        //
        //public static char KeyboardKeyToChar(int key)
        //{
        //    
        //}
    }
}
