using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace RealTimeRender
{
    class Camera : GameComponent
    {
        #region Properties

        private float distance;
        public float Distance
        {
            get { return distance; }
            set {
                distance = value;
                if (distance < 0) distance = 0;
                position = new Vector3(position.X, position.Y, distance);
            }
        }

		private Vector3 target;
		public Vector3 Target
		{
			get { return target; }
			set { target = value; }
		}

        private Vector3 position;
        public Vector3 Position
        {
            get { return position; }
        }

        private float aspectRatio;
        public float AspectRatio
        {
            get { return aspectRatio; }
        }

        private float speed;
        public float Speed
        {
            get { return speed; }
        }

        #endregion

		public Camera(GraphicsDeviceManager graphics, ContentManager Content)
			: base(graphics, Content)
		{

        }

        public override void Initialize()
        {
            base.Initialize();

            Distance = 500;

            speed = 500;
            position = new Vector3(0f, 0f, Distance);
			target = Vector3.Zero;

            aspectRatio = (float)graphics.GraphicsDevice.Viewport.Width / (float)graphics.GraphicsDevice.Viewport.Height;
        }


		public override void Translate(Vector3 diff)
		{
			position += diff;
		}
	}
}
