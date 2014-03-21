using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

namespace RealTimeRender
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont font;
        Model ModelLowRes, ModelMiddleRes, ModelHighRes;
        Camera camera;

        Vector3 modelPosition = Vector3.Zero;
        float modelRotation = 0.0f;

        List<Model> listModel = new List<Model>();
		List<Vector3> listModelCenter = new List<Vector3>();
        // Set the position of the camera in world space, for our view matrix.
        Vector3 cameraPosition = new Vector3(50.0f, 0.0f, 1700.0f);
		Vector3 lookAt = Vector3.Zero;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            camera = new Camera(graphics, Content);
            camera.Initialize();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            font = Content.Load<SpriteFont>("SpriteFont1");
            ModelLowRes = Content.Load<Model>("low_res");
            ModelMiddleRes = Content.Load<Model>("mid_res");
            ModelHighRes = Content.Load<Model>("hight_res");

			Vector3 center = _GetCenter(ModelHighRes);

            listModel.Add(ModelLowRes);
            listModel.Add(ModelMiddleRes);
            listModel.Add(ModelHighRes);

			listModelCenter.Add(center);
			listModelCenter.Add(center);
			listModelCenter.Add(center);
        }

		private Vector3 _GetCenter(Model model)
		{
			Vector3 center = Vector3.Zero;
			int meshNum = 0;
			foreach (ModelMesh m in model.Meshes)
			{
				center += m.BoundingSphere.Center;
				++meshNum;
			}
			return center / (float)meshNum;
		}

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            KeyboardState kb = Keyboard.GetState();
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed) this.Exit();

            if (kb.IsKeyDown(Keys.Down) == true)    camera.Distance += camera.Speed * (gameTime.ElapsedGameTime.Milliseconds / 1000f);
            if (kb.IsKeyDown(Keys.Up) == true)      camera.Distance -= camera.Speed * (gameTime.ElapsedGameTime.Milliseconds / 1000f);
			if (kb.IsKeyDown(Keys.Left))
			{
				cameraPosition.X -= 25.0f;
				lookAt.X -= 25f;
			}
			if (kb.IsKeyDown(Keys.Right))
			{
				cameraPosition.X += 25.0f;
				lookAt.X += 25f;
			}

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            spriteBatch.DrawString(font,"Z Position = " + camera.Position.Z.ToString(), new Vector2(10f, 10f), Color.White);
            
            spriteBatch.End();
            Model tempModel;

            // active DepthBuffer
			//graphics.GraphicsDevice.DepthStencilState.DepthBufferEnable = true;
            

            // Level Of Detail
            if (camera.Position.Z > 1500.0f)
                tempModel = ModelLowRes;
            else if (camera.Position.Z > 500.0f)
                tempModel = ModelMiddleRes;
            else
                tempModel = ModelHighRes;

			// Resolution by angle
			Vector3 objCenter = listModelCenter[listModel.IndexOf(tempModel)];
			Vector3 d1 = (objCenter - cameraPosition);
			d1.Normalize();
			Vector3 d2 = lookAt - cameraPosition;
			d2.Normalize();
			float diff = Vector3.Dot(d1, d2);
			spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
			spriteBatch.DrawString(font, "  Angle = " + diff.ToString(), new Vector2(25.0f, 50f), Color.White);
			spriteBatch.End();
			if (diff > 0.9f)
			{
				tempModel = ModelHighRes;
			}
			else if (diff > 0.7f)
			{
				tempModel = ModelMiddleRes;
			}
			else
			{
				tempModel = ModelLowRes;
			}
			//graphics.GraphicsDevice.RenderState.DepthBufferEnable = true;
			DepthStencilState dss = new DepthStencilState();
			dss.DepthBufferEnable = true;
			dss.DepthBufferWriteEnable = true;
			graphics.GraphicsDevice.DepthStencilState = dss;
            // Copy any parent transforms.
            Matrix[] transforms = new Matrix[tempModel.Bones.Count];
            tempModel.CopyAbsoluteBoneTransformsTo(transforms);
			
            // Draw the model. A model can have multiple meshes, so loop.
            foreach (ModelMesh mesh in tempModel.Meshes)
            {
                // This is where the mesh orientation is set, as well as our camera and projection.
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.World = transforms[mesh.ParentBone.Index] * Matrix.CreateRotationY(modelRotation)
                        * Matrix.CreateTranslation(modelPosition);
                    effect.View = Matrix.CreateLookAt(cameraPosition, lookAt, Vector3.Up);
                    effect.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45.0f),
                        camera.AspectRatio, 1.0f, 10000.0f);
                }
                // Draw the mesh, using the effects set above.
                mesh.Draw();
            }
            base.Draw(gameTime);
        }
    }
}
