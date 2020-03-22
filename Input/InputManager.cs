using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Window;
using SFML.Graphics;
using SFML.System;

namespace EngineeringCorpsCS
{
    class InputManager
    {
        RenderWindow window;
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
        /// <summary>
        /// This client's menuFactory.  Menu's are created by input, so they are closely coupled.
        /// </summary>
        public MenuFactory menuFactory { get; set; }

        public Vector2i mousePos { get; protected set; }
        public Vector2i mouseDiff { get; protected set; }

        private int tickAccumulator = 0;
        private int keyResetInterval = 60;
        private List<IInputSubscriber> subscriberMenuList;
        private List<IInputSubscriber> subscriberList;
        private Camera subscribedCamera;
        
        public InputManager(RenderWindow window) 
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
            mouseDiff = mousePos;
            mousePos = Mouse.GetPosition(window);

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
            if (menu)
            {
                subscriberMenuList.Insert(0, subscriber);
                return;
            }
            subscriberList.Insert(0, subscriber);
            
        }

        /// <summary>
        /// Removes a subscriber to the input stream if it is subscribed
        /// </summary>
        /// <param name="subscriber"></param>
        /// <param name="menu"></param>
        public void RemoveInputSubscriber(IInputSubscriber subscriber, bool menu)
        {
            if(menu)
            {
                subscriberMenuList.Remove(subscriber);
                return;
            }
            subscriberList.Remove(subscriber);
        }

        public void ClearGameSubscribers()
        {
            subscriberList.Clear();
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

        public Camera GetCamera()
        {
            return subscribedCamera;
        }

        public void HandleInput()
        {
            window.SetView(subscribedCamera.GetGUIView());
            for(int i = 0; i < subscriberMenuList.Count; i++)
            {
                subscriberMenuList[i].HandleInput(this);
            }
            window.SetView(subscribedCamera.GetGameView());
            for (int i = 0; i < subscriberList.Count; i++)
            {
                subscriberList[i].HandleInput(this);
            }
            if (subscribedCamera != null)
            {
                subscribedCamera.HandleInput(this);
            }
        }

        /// <summary>
        /// Consumes the specified key input and returns true.  If the input was not there, then the function returns false.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool ConsumeKeyPress(Keyboard.Key key)
        {
            if(keyPressed[key])
            {
                keyPressed[key] = false;
                return true;
            }
            return false;
        }

        public Vector2f GetMousePosition()
        {
            return window.MapPixelToCoords(mousePos);
        }

        public float[] GetMousePositionAsFloat()
        {
            return new float[] { window.MapPixelToCoords(mousePos).X, window.MapPixelToCoords(mousePos).Y };
        }

        public Vector2f GetMouseDiff()
        {
            return window.MapPixelToCoords(mousePos) - window.MapPixelToCoords(mouseDiff);
        }
    }
}
