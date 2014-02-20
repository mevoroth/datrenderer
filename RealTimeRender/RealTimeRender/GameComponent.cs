using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RealTimeRender
{
    class GameComponent
    {
        protected GraphicsDeviceManager graphics;

        public GameComponent(GraphicsDeviceManager g)
        {
            graphics = g;
        }

        public virtual void Initialize()
        {

        }

        public virtual void Update(GameTime gameTime)
        {

        }

        public virtual void Draw(GameTime gameTime)
        {

        }

    }
}
