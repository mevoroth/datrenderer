using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace RealTimeRender
{
    abstract class GameComponent
    {
		public class Poly
		{
			float lod;
			Model model;
			public Model Model
			{
				get { return model; }
				set { model = value; }
			}
			public float Lod
			{
				get { return lod; }
				set { lod = value; }
			}
		}
        protected GraphicsDeviceManager graphics;
		protected ContentManager content;
		private float _lod = 1f;
		public float LOD
		{
			get { return _lod; }
			set { _lod = value; }
		}
        public GameComponent(GraphicsDeviceManager g, ContentManager Content)
        {
            graphics = g;
			content = Content;
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
		public abstract void Translate(Vector3 diff);
    }
}
