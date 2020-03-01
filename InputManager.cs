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
        public Dictionary<Mouse.Button, bool> mouseButton;
        Dictionary<Mouse.Button, bool> mouseClick;
        public int mouseX;
        public int mouseY;
        public int mouseXdiff;
        public int mouseYdiff;
        int tickAccumulator = 0;
        int keyResetInterval = 60;
        public InputManager(Window window) 
        {
            this.window = window;
            window.SetKeyRepeatEnabled(false);
            window.KeyPressed += HandleKeyPress;
            window.KeyReleased += HandleKeyRelease;
            window.MouseButtonPressed += HandleMouseClick;
            window.MouseButtonReleased += HandleMouseRelease;
            keyPressed = new Dictionary<Keyboard.Key, bool>();
            keyHeld = new Dictionary<Keyboard.Key, bool>();
            keyReleased = new Dictionary<Keyboard.Key, bool>();
            keyAccumulator = new Dictionary<Keyboard.Key, byte>();
            mouseButton = new Dictionary<Mouse.Button, bool>();
            mouseClick = new Dictionary<Mouse.Button, bool>();
            foreach (Keyboard.Key key in Enum.GetValues(typeof(Keyboard.Key)))
            {
                keyPressed[key] = false;
                keyReleased[key] = false;
                keyHeld[key] = false;
                keyAccumulator[key] = 0;
            }
            foreach (Mouse.Button mb in Enum.GetValues(typeof(Mouse.Button)))
            {
                mouseButton[mb] = false;
                mouseClick[mb] = false;
            }
        }
        public void Update()
        {
            mouseXdiff = Mouse.GetPosition(window).X - mouseX;
            mouseYdiff = Mouse.GetPosition(window).Y - mouseY;
            mouseX = Mouse.GetPosition(window).X;
            mouseY = Mouse.GetPosition(window).Y;

        }

        public void FlushInput()
        {
            foreach (Keyboard.Key key in Enum.GetValues(typeof(Keyboard.Key))) {
                keyPressed[key] = false;
                keyReleased[key] = false;
            }
            foreach (Mouse.Button mb in Enum.GetValues(typeof(Mouse.Button)))
            {
                mouseClick[mb] = false;
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

        public void HandleMouseClick(object sender, MouseButtonEventArgs e)
        {
            mouseButton[e.Button] = true;
            mouseClick[e.Button] = true;
        }
        public void HandleMouseRelease(object sender, MouseButtonEventArgs e)
        {
            mouseButton[e.Button] = false;
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
