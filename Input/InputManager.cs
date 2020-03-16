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
        /// <summary>
        /// Stores whether the specific key was pressed this frame
        /// </summary>
        public Dictionary<Keyboard.Key, bool> keyPressed { get; protected set; }
        /// <summary>
        /// Stores whether the specific key is currently held
        /// </summary>
        public Dictionary<Keyboard.Key, bool> keyHeld { get; protected set; }
        /// <summary>
        /// Stores whether the specific key was released this frame
        /// </summary>
        public Dictionary<Keyboard.Key, bool> keyReleased { get; protected set; }
        /// <summary>
        /// Stores the number of times the specific key has been pressed in the last second
        /// </summary>
        public Dictionary<Keyboard.Key, byte> keyAccumulator { get; protected set; }
        /// <summary>
        /// Stores whether the mouse button is currently held down
        /// </summary>
        public Dictionary<Mouse.Button, bool> mouseButton { get; protected set; }
        /// <summary>
        /// Stores whether the mouse was clicked this frame
        /// </summary>
        public Dictionary<Mouse.Button, bool> mouseClick { get; protected set; }
        /// <summary>
        /// Stores the number of times the mouse has been clicked in the last second
        /// </summary>
        public Dictionary<Mouse.Button, byte> mouseAccumulator { get; protected set; }
        /// <summary>
        /// Stores whether a mouse button was released this frame
        /// </summary>
        public Dictionary<Mouse.Button, bool> mouseReleased { get; protected set; }
        /// <summary>
        /// Stores the change in mouse scroll this frame
        /// </summary>
        public float mouseScrollDelta { get; protected set; }

        public int mouseX;
        public int mouseY;
        public int mouseXdiff;
        public int mouseYdiff;

        private int tickAccumulator = 0;
        private int keyResetInterval = 60;
        private List<IInputSubscriber> subscriberMenuList;
        private List<IInputSubscriber> subscriberList;
        private Camera subscribedCamera;
        public InputManager(Window window) 
        {
            this.window = window;
            window.SetKeyRepeatEnabled(false);
            window.KeyPressed += HandleKeyPress;
            window.KeyReleased += HandleKeyRelease;
            window.MouseButtonPressed += HandleMouseClick;
            window.MouseButtonReleased += HandleMouseRelease;
            window.MouseWheelScrolled += HandleMouseWheel;
            keyPressed = new Dictionary<Keyboard.Key, bool>();
            keyHeld = new Dictionary<Keyboard.Key, bool>();
            keyReleased = new Dictionary<Keyboard.Key, bool>();
            keyAccumulator = new Dictionary<Keyboard.Key, byte>();
            mouseButton = new Dictionary<Mouse.Button, bool>();
            mouseClick = new Dictionary<Mouse.Button, bool>();
            mouseAccumulator = new Dictionary<Mouse.Button, byte>();
            mouseReleased = new Dictionary<Mouse.Button, bool>();
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
                mouseAccumulator[mb] = 0;
                mouseReleased[mb] = false;
            }
            subscriberList = new List<IInputSubscriber>();
            subscriberMenuList = new List<IInputSubscriber>();
        }
        public void Update()
        {
            //reconsider updating in an event
            mouseXdiff = Mouse.GetPosition(window).X - mouseX;
            mouseYdiff = Mouse.GetPosition(window).Y - mouseY;
            mouseX = Mouse.GetPosition(window).X;
            mouseY = Mouse.GetPosition(window).Y;

            //essentials
            HandleInput();
            FlushInput();
        }

        private void FlushInput()
        {
            foreach (Keyboard.Key key in Enum.GetValues(typeof(Keyboard.Key))) {
                keyPressed[key] = false;
                keyReleased[key] = false;
            }
            foreach (Mouse.Button mb in Enum.GetValues(typeof(Mouse.Button)))
            {
                mouseClick[mb] = false;
                mouseReleased[mb] = false;
            }
            if (tickAccumulator > keyResetInterval)
            {
                tickAccumulator -= keyResetInterval;
                foreach (Keyboard.Key key in Enum.GetValues(typeof(Keyboard.Key)))
                {
                    keyAccumulator[key] = 0;
                }
                foreach (Mouse.Button mb in Enum.GetValues(typeof(Mouse.Button)))
                {
                    mouseAccumulator[mb] = 0;
                }
            }
            mouseScrollDelta = 0.0f;
            tickAccumulator++;
        }

        private void HandleMouseClick(object sender, MouseButtonEventArgs e)
        {
            mouseButton[e.Button] = true;
            mouseClick[e.Button] = true;
            mouseAccumulator[e.Button]++;
        }
        private void HandleMouseRelease(object sender, MouseButtonEventArgs e)
        {
            mouseButton[e.Button] = false;
            mouseReleased[e.Button] = true;
        }

        private void HandleMouseWheel(object sender, MouseWheelScrollEventArgs e)
        {
            mouseScrollDelta = e.Delta;
        }

        private void HandleKeyPress(object sender, KeyEventArgs e)
        {
            keyPressed[e.Code] = true;
            keyHeld[e.Code] = true;
            if (keyAccumulator[e.Code] < byte.MaxValue)
            {
                keyAccumulator[e.Code]++;
                //Console.WriteLine("KeyAccumulated: " + keyAccumulator[e.Code] + " : " + e.ToString());
            }
            //Console.WriteLine("KeyPressed: " + e.ToString());
        }

        private void HandleKeyRelease(object sender, KeyEventArgs e)
        {
            keyHeld[e.Code] = false;
            keyReleased[e.Code] = true;
            //Console.WriteLine("KeyReleased: " + e.ToString());
        }

        /// <summary>
        /// Adds a subscriber to the input stream
        /// </summary>
        /// <param name="subscriber"></param>
        /// <param name="menu"></param>
        public void AddInputSubscriber(IInputSubscriber subscriber, bool menu)
        {
            subscriberList.Insert(0, subscriber);
            if (menu)
            {
                subscriberMenuList.Insert(0, subscriber);
            }
        }

        /// <summary>
        /// Removes a subscriber to the input stream if it is subscribed
        /// </summary>
        /// <param name="subscriber"></param>
        /// <param name="menu"></param>
        public void RemoveInputSubscriber(IInputSubscriber subscriber, bool menu)
        {
            subscriberList.Insert(0, subscriber);
        }

        public void ChangeCamera(Camera camera)
        {
            this.subscribedCamera = camera;
        }

        public void RemoveCamera(Camera camera)
        {
            if (camera.Equals(this.subscribedCamera))
            {
                this.subscribedCamera = null;
            }
        }

        public void HandleInput()
        {
            for(int i = 0; i < subscriberMenuList.Count; i++)
            {
                subscriberMenuList[i].HandleInput(this);
            }
            for(int i = 0; i < subscriberList.Count; i ++)
            {
                subscriberList[i].HandleInput(this);
            }
            if (subscribedCamera != null)
            {
                subscribedCamera.HandleInput(this);
            }
        }
    }
}
