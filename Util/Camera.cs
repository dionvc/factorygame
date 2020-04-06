using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Window;
using SFML.Graphics;
using SFML.System;
using SFML.Audio;

namespace EngineeringCorpsCS
{
    class Camera : IInputSubscriber
    {
        public Entity focusedEntity { get; set; }
        View gameView;
        View guiView;
        float viewScale;
        SurfaceContainer viewedSurface;
        Vector2f gameViewSize;
        public Camera()
        {
            gameView = new View(new FloatRect(0, 0, 1280, 720));
            guiView = new View(new Vector2f(640, 360), new Vector2f(1280, 720));
            viewScale = 2.0f;
            gameViewSize = new Vector2f(1280, 720);
        }
        public void Update()
        {
            
            if (focusedEntity != null)
            {
                gameView.Center = new Vector2f(focusedEntity.position.x, focusedEntity.position.y);
                viewedSurface = focusedEntity.surface;
                Listener.Position = new Vector3f(gameView.Center.X, 0.0f, gameView.Center.Y);
            }
        }
        public View GetGameView()
        {
            return gameView;
        }
        
        public float GetGameViewScale()
        {
            return ((float)Math.Round(viewScale) / 2);
        }

        public View GetGUIView()
        {
            return guiView;
        }

        public void HandleResize(Object s, SizeEventArgs e)
        {
            float rW = (float)Math.Pow(2, Math.Round(Math.Log(e.Width) / Math.Log(2)));
            float rH = (float)Math.Pow(2, Math.Round(Math.Log(e.Height) / Math.Log(2)));
            gameView.Size = new Vector2f(rW, rH);
            gameViewSize = gameView.Size;
            guiView = new View(new Vector2f(e.Width/2, e.Height/2), new Vector2f(e.Width, e.Height));
        }

        public void SubscribeToInput(InputManager input)
        {
            input.ChangeCamera(this);
        }
        public void UnsubscribeToInput(InputManager input)
        {
            input.RemoveCamera(this);
        }
        public void HandleInput(InputManager input)
        {
            //Handle zooming
            if (input.GetMouseScrollDelta(false) != 0)
            {
                viewScale -= (2 * input.GetMouseScrollDelta(true)/InputBindings.scrollSensitivity);
                if (viewScale < Props.cameraZoomMin)
                {
                    viewScale = Props.cameraZoomMin;
                }
                else if (viewScale > Props.cameraZoomMax)
                {
                    viewScale = Props.cameraZoomMax;
                }
                
                gameView.Size = gameViewSize * ((float)Math.Round(viewScale)/2);
            }
            if(input.GetKeyPressed(InputBindings.showWorldMap, true))
            {
                input.menuFactory.CreateWorldMap(this);
            }
            //Handle clicking on things that are in view here with consideration to focusedentity
        }
    }
}
