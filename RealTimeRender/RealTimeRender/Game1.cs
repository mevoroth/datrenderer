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

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            camera = new Camera(graphics);
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

            listModel.Add(ModelLowRes);
            listModel.Add(ModelMiddleRes);
            listModel.Add(ModelHighRes);

        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            KeyboardState kb = Keyboard.GetState();
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed) this.Exit();

            if (kb.IsKeyDown(Keys.Down) == true)    camera.Distance += camera.Speed * (gameTime.ElapsedGameTime.Milliseconds / 1000f);
            if (kb.IsKeyDown(Keys.Up) == true)      camera.Distance -= camera.Speed * (gameTime.ElapsedGameTime.Milliseconds / 1000f);

            modelRotation += (float)gameTime.ElapsedGameTime.TotalMilliseconds * MathHelper.ToRadians(0.01f);
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
			DepthStencilState dss = new DepthStencilState();
			dss.DepthBufferEnable = true;
			dss.DepthBufferWriteEnable = true;
			graphics.GraphicsDevice.DepthStencilState = dss;

            // Level Of Detail
            if (camera.Position.Z > 1500.0f)
                tempModel = ModelLowRes;
            else if (camera.Position.Z > 500.0f)
                tempModel = ModelMiddleRes;
            else
                tempModel = ModelHighRes;

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
                    effect.View = Matrix.CreateLookAt(camera.Position, Vector3.Zero, Vector3.Up);
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
