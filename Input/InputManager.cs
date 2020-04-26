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
        #region key and mouse dictionaries
        /// Stores whether the specific key was pressed this frame
        private Dictionary<Keyboard.Key, bool> keyPressed;
        /// Stores whether the specific key is currently held
        private Dictionary<Keyboard.Key, bool> keyHeld;
        /// Stores whether the specific key was released this frame
        private Dictionary<Keyboard.Key, bool> keyReleased;
        /// Stores the number of times the specific key has been pressed in the last second
        private Dictionary<Keyboard.Key, byte> keyAccumulator;
        /// Stores whether the mouse button is currently held down
        private Dictionary<Mouse.Button, bool> mouseButton;
        /// Stores whether the mouse was clicked this frame
        private Dictionary<Mouse.Button, bool> mouseClick;
        /// Stores the number of times the mouse has been clicked in the last second
        private Dictionary<Mouse.Button, byte> mouseAccumulator;
        /// Stores whether a mouse button was released this frame
        private Dictionary<Mouse.Button, bool> mouseReleased;
        #endregion key and mouse dictionaries

        /// Stores the change in mouse scroll this frame
        private float mouseScrollDelta;
        // Stores the mouse position in screen coordinates
        private Vector2i mousePos;
        /// Stores the difference in the new and old mouse positions.  Useful for dragging items.
        private Vector2i mouseDiff;
        /// This client's menuFactory.  Menu's are created by input, so they are closely coupled.
        public MenuFactory menuFactory { get; set; }
        public EntityCollection entityCollection { get; set; }

        public ItemCollection itemCollection { get; set; }

        private string keyString;
        private bool keyStringConsumed;
        private int tickAccumulator = 0;
        private int keyResetInterval = 60;
        private List<IInputSubscriber> subscriberMenuList;
        private List<IInputSubscriber> subscriberList;
        private Camera subscribedCamera;
        private List<Keyboard.Key> keyConsumedForFrame; //Stores if a key has been consumed this frame, if so, doesn't allow it to be used
        private List<Mouse.Button> mouseButtonConsumedForFrame;
        private bool mouseScrollConsumedForFrame = false;
        private bool mousePositionConsumedForFrame = false;
        public InputManager(RenderWindow window) 
        {
            this.window = window;
            window.SetKeyRepeatEnabled(false);
            window.KeyPressed += HandleKeyPress;
            window.KeyReleased += HandleKeyRelease;
            window.MouseButtonPressed += HandleMouseClick;
            window.MouseButtonReleased += HandleMouseRelease;
            window.MouseWheelScrolled += HandleMouseWheel;
            window.TextEntered += HandleTextEntered;
            keyPressed = new Dictionary<Keyboard.Key, bool>();
            keyHeld = new Dictionary<Keyboard.Key, bool>();
            keyReleased = new Dictionary<Keyboard.Key, bool>();
            keyAccumulator = new Dictionary<Keyboard.Key, byte>();
            mouseButton = new Dictionary<Mouse.Button, bool>();
            mouseClick = new Dictionary<Mouse.Button, bool>();
            mouseAccumulator = new Dictionary<Mouse.Button, byte>();
            mouseReleased = new Dictionary<Mouse.Button, bool>();
            keyConsumedForFrame = new List<Keyboard.Key>();
            mouseButtonConsumedForFrame = new List<Mouse.Button>();
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
        public void Update(Program.GameState gameState)
        {
            //reconsider updating in an event
            mouseDiff = mousePos;
            mousePos = Mouse.GetPosition(window);

            //essentials
            HandleInput(gameState);
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
            keyConsumedForFrame.Clear();
            mouseButtonConsumedForFrame.Clear();
            mouseScrollConsumedForFrame = false;
            mousePositionConsumedForFrame = false;
            keyStringConsumed = false;
            mouseScrollDelta = 0.0f;
            keyString = "";
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

        private void HandleTextEntered(object sender, TextEventArgs e)
        {
            keyString = e.Unicode;
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

        public void PushMenuToFront(IInputSubscriber subscriber) 
        {
            if(subscriberMenuList.Remove(subscriber))
            {
                subscriberMenuList.Insert(0,subscriber);
            }
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

        public void HandleInput(Program.GameState gameState)
        {
            window.SetView(subscribedCamera.GetGUIView());
            for(int i = 0; i < subscriberMenuList.Count; i++)
            {
                subscriberMenuList[i].HandleInput(this);
            }
            if (gameState == Program.GameState.inGame)
            {
                window.SetView(subscribedCamera.GetGameView());
                for (int i = 0; i < subscriberList.Count; i++)
                {
                    subscriberList[i].HandleInput(this);
                }
                subscribedCamera.HandleInput(this);
            }
        }

        //Note: Consumed keys mean that the key cannot be accessed as true for the rest of the frame

        /// <summary>
        /// Gets whether a key was pressed this frame and can optionally consume it.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="consume"></param>
        /// <returns></returns>
        public bool GetKeyPressed(Keyboard.Key key, bool consume)
        {
            if(keyPressed[key] && !keyConsumedForFrame.Contains(key))
            {
                if (consume)
                {
                    keyConsumedForFrame.Add(key);
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Returns a string of keys pressed this frame
        /// </summary>
        /// <param name="consume"></param>
        /// <returns></returns>
        public string GetKeyString(bool consume)
        {
            if(keyStringConsumed)
            {
                return "";
            }
            if (consume)
            {
                keyStringConsumed = true;
                return keyString;
            }
            else
            {
                return keyString;
            }
            
            
        }
        /// <summary>
        /// Gets whether a key was held for this frame and optionally consumes it for the frame.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="consume"></param>
        /// <returns></returns>
        public bool GetKeyHeld(Keyboard.Key key, bool consume)
        {
            if(keyHeld[key] && !keyConsumedForFrame.Contains(key))
            {
                if (consume)
                {
                    keyConsumedForFrame.Add(key);
                }
                return true;
            }
            return false;
        }
        
        /// <summary>
        /// Gets whether a key was released this frame and optionally consumes it for the frame.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="consume"></param>
        /// <returns></returns>
        public bool GetKeyReleased(Keyboard.Key key, bool consume)
        {
            if (keyReleased[key] && !keyConsumedForFrame.Contains(key))
            {
                if (consume)
                {
                    keyConsumedForFrame.Add(key);
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Gets whether the mouse button was clicked this frame and optionally consumes it for the frame.
        /// </summary>
        /// <param name="mb"></param>
        /// <param name="consume"></param>
        /// <returns></returns>
        public bool GetMouseClicked(Mouse.Button mb, bool consume)
        {
            if(mouseClick[mb] && !mouseButtonConsumedForFrame.Contains(mb))
            {
                if(consume)
                {
                    mouseButtonConsumedForFrame.Add(mb);
                }
                return true;
            }
            return false;
        }

        public bool GetMouseClickedIgnoreConsume(Mouse.Button mb)
        {
            return mouseClick[mb];
        }

        /// <summary>
        /// Gets if the mouse button was held for this frame and optionally consumes it.
        /// </summary>
        /// <param name="mb"></param>
        /// <param name="consume"></param>
        /// <returns></returns>
        public bool GetMouseHeld(Mouse.Button mb, bool consume)
        {
            if (mouseButton[mb] && !mouseButtonConsumedForFrame.Contains(mb))
            {
                if (consume)
                {
                    mouseButtonConsumedForFrame.Add(mb);
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Gets if the mouse button was released this frame and optionally consumes it.
        /// </summary>
        /// <param name="mb"></param>
        /// <param name="consume"></param>
        /// <returns></returns>
        public bool GetMouseReleased(Mouse.Button mb, bool consume)
        {
            if (mouseReleased[mb] && !mouseButtonConsumedForFrame.Contains(mb))
            {
                if (consume)
                {
                    mouseButtonConsumedForFrame.Add(mb);
                }
                return true;
            }
            return false;
        }

        public float GetMouseScrollDelta(bool consume)
        {
            if (!mouseScrollConsumedForFrame)
            {
                if(consume)
                {
                    mouseScrollConsumedForFrame = true;
                }
                return mouseScrollDelta;
            }
            return 0.0f;
        }

        /// <summary>
        /// Returns whether the mouse position was available.  Also outputs the coordinates themselves as a Vector2f.
        /// </summary>
        /// <param name="consume"></param>
        /// <param name="coords"></param>
        /// <returns></returns>
        public bool GetMousePosition(out Vector2f coords)
        {
            coords = window.MapPixelToCoords(mousePos);
            if (mousePositionConsumedForFrame == false)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Returns whether the mouse position was available. Also outputs the coordinates themselves as a float array.
        /// </summary>
        /// <param name="consume"></param>
        /// <param name="coords"></param>
        /// <returns></returns>
        public bool GetMousePositionAsFloat(out float[] coords)
        {
            coords = new float[] { window.MapPixelToCoords(mousePos).X, window.MapPixelToCoords(mousePos).Y };
            if (mousePositionConsumedForFrame == false)
            {
                return true;
            }
            return false;
        }

        public void ConsumeMousePosition()
        {
            mousePositionConsumedForFrame = true;
        }

        public Vector2f GetMouseDiff()
        {
            return window.MapPixelToCoords(mousePos) - window.MapPixelToCoords(mouseDiff);
        }
    }
}
