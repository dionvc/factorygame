﻿using System;
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
        public static Keyboard.Key showPauseMenu = Keyboard.Key.Escape;

        public static Keyboard.Key showMinimap = Keyboard.Key.N;
        public static Keyboard.Key showWorldMap = Keyboard.Key.M;
        public static Keyboard.Key showInventory = Keyboard.Key.E;

        public static Keyboard.Key returnItem = Keyboard.Key.Q;
        public static Keyboard.Key dropItem = Keyboard.Key.F;

        public static Keyboard.Key increasePlacementSize = Keyboard.Key.Add;
        public static Keyboard.Key decreasePlacementSize = Keyboard.Key.Subtract;

        public static Keyboard.Key backSpace = Keyboard.Key.Backspace;
        public static Keyboard.Key nullKey = Keyboard.Key.Home; //We need to reserve one key as a key which will not be
        //processed.  This is unfortunately due to the fact that Keyboard.Key does not provide a None enumeration value.
    }
}
