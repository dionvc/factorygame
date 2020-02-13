using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Window;

namespace EngineeringCorpsCS
{
    class InputManager
    {
        Window window;
        Dictionary<Keyboard.Key, bool> keyPressed;
        Dictionary<Keyboard.Key, bool> keyHeld;
        Dictionary<Keyboard.Key, bool> keyReleased;
        Dictionary<Keyboard.Key, byte> keyAccumulator;
        int tickAccumulator = 0;
        int keyResetInterval = 60;
        public InputManager(Window window) 
        {
            this.window = window;
            window.SetKeyRepeatEnabled(false);
            window.KeyPressed += HandleKeyPress;
            window.KeyReleased += HandleKeyRelease;
            keyPressed = new Dictionary<Keyboard.Key, bool>();
            keyHeld = new Dictionary<Keyboard.Key, bool>();
            keyReleased = new Dictionary<Keyboard.Key, bool>();
            keyAccumulator = new Dictionary<Keyboard.Key, byte>();
            foreach (Keyboard.Key key in Enum.GetValues(typeof(Keyboard.Key)))
            {
                keyPressed[key] = false;
                keyReleased[key] = false;
                keyHeld[key] = false;
                keyAccumulator[key] = 0;
            }
        }

        public void FlushInput()
        {
            foreach (Keyboard.Key key in Enum.GetValues(typeof(Keyboard.Key))) {
                keyPressed[key] = false;
                keyReleased[key] = false;
            }
            if (tickAccumulator > keyResetInterval)
            {
                tickAccumulator -= keyResetInterval;
                foreach (Keyboard.Key key in Enum.GetValues(typeof(Keyboard.Key)))
                {
                    keyAccumulator[key] = 0;
                }
            }
            tickAccumulator++;
        }

        public void HandleKeyPress(object sender, KeyEventArgs e)
        {
            keyPressed[e.Code] = true;
            keyHeld[e.Code] = true;
            if (keyAccumulator[e.Code] < byte.MaxValue)
            {
                keyAccumulator[e.Code]++;
                Console.WriteLine("KeyAccumulated: " + keyAccumulator[e.Code] + " : " + e.ToString());
            }
            Console.WriteLine("KeyPressed: " + e.ToString());
        }

        public void HandleKeyRelease(object sender, KeyEventArgs e)
        {
            keyHeld[e.Code] = false;
            keyReleased[e.Code] = true;
            Console.WriteLine("KeyReleased: " + e.ToString());
        }
    }

    interface IInputSubscriber
    {
        /// <summary>
        /// Adds an entity to the input handling queue
        /// </summary>
        /// <param name="input"></param>
        void SubscribeToInput(InputManager input);
        /// <summary>
        /// Removes an entity from the input handling queue
        /// </summary>
        /// <param name="input"></param>
        void UnsubscribeToInput(InputManager input);
        /// <summary>
        /// Where input will be processed by an entity
        /// </summary>
        /// <param name="input"></param>
        void HandleInput(InputManager input);
    }
}
