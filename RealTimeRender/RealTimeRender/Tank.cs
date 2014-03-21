using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace RealTimeRender
{
	class Tank : GameComponent
	{
		List<Poly> cpolys = new List<Poly>();
		int current = 0;
		private float modelRotation = 0f;
		Vector3 modelPosition = Vector3.Zero;

		Camera camera;

		public Tank(GraphicsDeviceManager g, ContentManager Content, Camera c, List<string> polys, List<float> lods)
			: base(g, Content)
        {
			if (polys.Count == 0 || polys.Count != lods.Count)
			{
				throw new Exception("POLY LIST == 0 OR POLYS COUNT != LODS COUNT");
			}
			graphics = g;
			camera = c;
			for (int i = 0, c = polys.Count; i < c; ++i)
			{
			    Poly p = new Poly();
				p.Lod = lods[i];
				p.Model = content.Load<Model>(polys[i]);
				cpolys.Add(p);
			}
        }

        public override void Initialize()
        {
			base.Initialize();
        }

		public override void Update(GameTime gameTime)
		{
			modelRotation += (float)gameTime.ElapsedGameTime.TotalMilliseconds * MathHelper.ToRadians(0.01f);
			base.Update(gameTime);
        }

		public override void Draw(GameTime gameTime)
		{
			Model tempModel = cpolys[current].Model;
			Matrix[] transforms = new Matrix[tempModel.Bones.Count];

			foreach (ModelMesh mesh in tempModel.Meshes)
			{
				// This is where the mesh orientation is set, as well as our camera and projection.
				foreach (BasicEffect effect in mesh.Effects)
				{
					effect.EnableDefaultLighting();
					effect.World = transforms[mesh.ParentBone.Index] * Matrix.CreateRotationY(modelRotation)
						* Matrix.CreateTranslation(modelPosition);
					effect.View = Matrix.CreateLookAt(camera.Position, camera.Target, Vector3.Up);
					effect.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45.0f),
						camera.AspectRatio, 1.0f, 10000.0f);
				}
				// Draw the mesh, using the effects set above.
				mesh.Draw();
			}
        }

		public override void Translate(Vector3 diff)
		{
			modelPosition += diff;
		}
	}
}
