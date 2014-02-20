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

namespace LOD
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont font;
        // Set the 3D model to draw.
        Model ModelLowRes, ModelMiddleRes, ModelHighRes;

        // The aspect ratio determines how to scale 3d to 2d projection.
        float aspectRatio;
        // Set the position of the model in world space, and set the rotation.
        Vector3 modelPosition = Vector3.Zero;
        float modelRotation = 0.0f;
        int latency = 0;

        List<Model> listModel = new List<Model>();
        // Set the position of the camera in world space, for our view matrix.
        Vector3 cameraPosition = new Vector3(50.0f, 0.0f, 1700.0f);

        enum States { DistanceLOD, PlayerControlledLOD };

        States currentState = States.DistanceLOD;
        int currentIndex = 0;
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
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

            aspectRatio = (float)graphics.GraphicsDevice.Viewport.Width /
            (float)graphics.GraphicsDevice.Viewport.Height;
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            KeyboardState kb = Keyboard.GetState();
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            if (kb.IsKeyDown(Keys.Down) && currentState == States.DistanceLOD)
            {
                if (cameraPosition.Z > 0) cameraPosition.Z -= 25.0f;
            }
            if (kb.IsKeyDown(Keys.Up) && currentState == States.DistanceLOD)
                cameraPosition.Z += 25.0f;

            if (kb.IsKeyDown(Keys.Space) && currentState == States.DistanceLOD && latency == 0)
            {
                currentState = States.PlayerControlledLOD;
                latency = 25;
            }
            else if (kb.IsKeyDown(Keys.Space) && currentState == States.PlayerControlledLOD && latency == 0)
            {
                currentState = States.DistanceLOD;
                latency = 25;
            }

            if (kb.IsKeyDown(Keys.Left) && currentState == States.PlayerControlledLOD)
            {
                if (currentIndex > 0 && latency == 0)
                {
                    currentIndex--;
                    latency = 25;
                }
            }
            if (kb.IsKeyDown(Keys.Right) && currentState == States.PlayerControlledLOD)
            {
                if (currentIndex < 2 && latency == 0)
                {
                    currentIndex++;
                    latency = 25;
                }
            }

            if (latency > 0)
                latency--;

            modelRotation += (float)gameTime.ElapsedGameTime.TotalMilliseconds * MathHelper.ToRadians(0.01f);
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.CornflowerBlue);
			//BlendState.AlphaBlend, DepthStencilState.None, RasterizerState.CullCounterClockwise, SamplerState.LinearClamp
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            spriteBatch.DrawString(font,"  ZPos = " + cameraPosition.Z.ToString(), new Vector2(25.0f, 25.0f), Color.White);
            
            spriteBatch.End();
            Model tempModel;

            // active DepthBuffer
			//graphics.GraphicsDevice.DepthStencilState.DepthBufferEnable = true;
            //graphics.GraphicsDevice.RenderState.DepthBufferEnable = true;
			DepthStencilState dss = new DepthStencilState();
			dss.DepthBufferEnable = true;
			dss.DepthBufferWriteEnable = true;
			graphics.GraphicsDevice.DepthStencilState = dss;

            // Level Of Detail
            if (currentState == States.DistanceLOD)
            {
                if (cameraPosition.Z > 1500.0f)
                    tempModel = ModelLowRes;
                else if (cameraPosition.Z > 500.0f)
                    tempModel = ModelMiddleRes;
                else
                    tempModel = ModelHighRes;
            }
            else
            {
                tempModel = listModel[currentIndex];
            }

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
                    effect.View = Matrix.CreateLookAt(cameraPosition, Vector3.Zero, Vector3.Up);
                    effect.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45.0f),
                        aspectRatio, 1.0f, 10000.0f);
                }
                // Draw the mesh, using the effects set above.
                mesh.Draw();
            }
            base.Draw(gameTime);
        }
    }
}
