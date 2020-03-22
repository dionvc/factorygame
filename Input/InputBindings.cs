using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Window;

namespace EngineeringCorpsCS
{
    class InputBindings
    {
        public static Keyboard.Key moveUp = Keyboard.Key.W;
        public static Keyboard.Key moveDown = Keyboard.Key.S;
        public static Keyboard.Key moveLeft = Keyboard.Key.A;
        public static Keyboard.Key moveRight = Keyboard.Key.D;

        public static Mouse.Button primary = Mouse.Button.Left;
        public static Mouse.Button secondary = Mouse.Button.Right;
        public static Mouse.Button tertiary = Mouse.Button.Middle;
        public static float scrollSensitivity = 2.0f;

        public static Keyboard.Key showDebugMenu = Keyboard.Key.F3;
        public static Keyboard.Key showPauseMenu = Keyboard.Key.Q;

        public static Keyboard.Key showMinimap = Keyboard.Key.N;

        public static Keyboard.Key increasePlacementSize = Keyboard.Key.Add;
        public static Keyboard.Key decreasePlacementSize = Keyboard.Key.Subtract;
    }
}
