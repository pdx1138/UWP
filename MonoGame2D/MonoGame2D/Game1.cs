using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using Windows.Graphics.Display;
using Windows.UI.ViewManagement;

namespace MonoGame2D
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        const float SKYRATIO = (2.0f / 3.0f);
        float screenWidth;
        float screenHeight;
        Texture2D grass;

        SpriteClass dino;
        SpriteClass broccoli;

        bool spaceDown;
        bool gameStarted;
        bool gameOver;

        float broccoliSpeedMultiplier;
        float gravitySpeed;
        float dinoSpeedX;
        float dinoJumpY;
        float score;

        Random random;

        Texture2D startGameSplash;
        SpriteFont scoreFont;
        SpriteFont stateFont;

        Texture2D gameOverTexture;

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

            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.FullScreen;

            screenHeight = ScaleToHighDPI((float)ApplicationView.GetForCurrentView().VisibleBounds.Height);
            screenWidth = ScaleToHighDPI((float)ApplicationView.GetForCurrentView().VisibleBounds.Width);

            this.IsMouseVisible = false;

            broccoliSpeedMultiplier = 0.5f;
            spaceDown = false;
            gameStarted = false;
            gameOver = false;
            score = 0;
            random = new Random();
            dinoSpeedX = ScaleToHighDPI(1000f);
            dinoJumpY = ScaleToHighDPI(-1200f);
            gravitySpeed = ScaleToHighDPI(30f);
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            grass = Content.Load<Texture2D>("grass");
            broccoli = new SpriteClass(GraphicsDevice, "Content/broccoli.png", ScaleToHighDPI(0.2f));
            dino = new SpriteClass(GraphicsDevice, "Content/ninja-cat-dino.png", ScaleToHighDPI(1f));

            startGameSplash = Content.Load<Texture2D>("start-splash");
            scoreFont = Content.Load<SpriteFont>("Score");
            stateFont = Content.Load<SpriteFont>("GameState");
            gameOverTexture = Content.Load<Texture2D>("game-over");
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
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
            // TODO: Add your update logic here

            float elapsedTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            KeyboardHandler(); // Handle keyboard input

            if (gameOver)
            {
                dino.dX = 0;
                dino.dY = 0;
                broccoli.dX = 0;
                broccoli.dY = 0;
                broccoli.dA = 0;
            }

            // Update animated SpriteClass objects based on their current rates of change
            dino.Update(elapsedTime);
            broccoli.Update(elapsedTime);

            // Accelerate the dino downward each frame to simulate gravity.
            dino.dY += gravitySpeed;

            // Set game floor so the player does not fall through it
            if (dino.y > screenHeight * SKYRATIO)
            {
                dino.dY = 0;
                dino.y = screenHeight * SKYRATIO;
            }

            // Set game edges to prevent the player from moving offscreen
            if (dino.x > screenWidth - dino.texture.Width / 2)
            {
                dino.x = screenWidth - dino.texture.Width / 2;
                dino.dX = 0;
            }
            if (dino.x < 0 + dino.texture.Width / 2)
            {
                dino.x = 0 + dino.texture.Width / 2;
                dino.dX = 0;
            }

            // If the broccoli goes offscreen, spawn a new one and iterate the score
            if (broccoli.y > screenHeight + 100 || broccoli.y < -100 || broccoli.x > screenWidth + 100 || broccoli.x < -100)
            {
                SpawnBroccoli();
                score++;
            }

            if (dino.RectangleCollision(broccoli)) gameOver = true;

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here

            base.Draw(gameTime);

            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin();
            spriteBatch.Draw(grass, new Rectangle(0, (int)(screenHeight * SKYRATIO),
                             (int)screenWidth, (int)screenHeight), Color.White);
            if (gameOver)
            {
                // Draw game over texture
                spriteBatch.Draw(gameOverTexture, new Vector2(screenWidth / 2 - gameOverTexture.Width / 2, screenHeight / 4 - gameOverTexture.Width / 2), Color.White);

                String pressEnter = "Press Enter to restart!";

                // Measure the size of text in the given font
                Vector2 pressEnterSize = stateFont.MeasureString(pressEnter);

                // Draw the text horizontally centered
                spriteBatch.DrawString(stateFont, pressEnter, new Vector2(screenWidth / 2 - pressEnterSize.X / 2, screenHeight - 200), Color.White);
            }
            broccoli.Draw(spriteBatch);
            dino.Draw(spriteBatch);
            spriteBatch.DrawString(scoreFont, score.ToString(),
                                   new Vector2(screenWidth - 100, 50), Color.Black);

            // If the game is not in progress
            if (!gameStarted)
            {
                // Fill the screen with black before the game starts
                spriteBatch.Draw(startGameSplash, new Rectangle(0, 0,
                (int)screenWidth, (int)screenHeight), Color.White);

                String title = "VEGGIE JUMP";
                String pressSpace = "Press Space to start";

                // Measure the size of text in the given font
                Vector2 titleSize = stateFont.MeasureString(title);
                Vector2 pressSpaceSize = stateFont.MeasureString(pressSpace);

                // Draw the text horizontally centered
                spriteBatch.DrawString(stateFont, title,
                new Vector2(screenWidth / 2 - titleSize.X / 2, screenHeight / 3),
                Color.ForestGreen);
                spriteBatch.DrawString(stateFont, pressSpace,
                new Vector2(screenWidth / 2 - pressSpaceSize.X / 2,
                screenHeight / 2), Color.White);
            }
            spriteBatch.End();
        }

        public float ScaleToHighDPI(float f)
        {
            DisplayInformation d = DisplayInformation.GetForCurrentView();
            f *= (float)d.RawPixelsPerViewPixel;
            return f;
        }

        public void SpawnBroccoli()
        {
            int direction = random.Next(1, 5);
            switch (direction)
            {
                case 1:
                    broccoli.x = -100;
                    broccoli.y = random.Next(0, (int)screenHeight);
                    break;
                case 2:
                    broccoli.y = -100;
                    broccoli.x = random.Next(0, (int)screenWidth);
                    break;
                case 3:
                    broccoli.x = screenWidth + 100;
                    broccoli.y = random.Next(0, (int)screenHeight);
                    break;
                case 4:
                    broccoli.y = screenHeight + 100;
                    broccoli.x = random.Next(0, (int)screenWidth);
                    break;
            }

            if (score % 5 == 0) broccoliSpeedMultiplier += 0.2f;

            broccoli.dX = (dino.x - broccoli.x) * broccoliSpeedMultiplier;
            broccoli.dY = (dino.y - broccoli.y) * broccoliSpeedMultiplier;
            broccoli.dA = 7f;
        }

        public void StartGame()
        {
            dino.x = screenWidth / 2;
            dino.y = screenHeight * SKYRATIO;
            broccoliSpeedMultiplier = 0.5f;
            SpawnBroccoli();
            score = 0;
        }

        void KeyboardHandler()
        {
            KeyboardState state = Keyboard.GetState();

            // Quit the game if Escape is pressed.
            if (state.IsKeyDown(Keys.Escape))
            {
                Exit();
            }

            // Start the game if Space is pressed.
            if (!gameStarted)
            {
                if (state.IsKeyDown(Keys.Space))
                {
                    StartGame();
                    gameStarted = true;
                    spaceDown = true;
                    gameOver = false;
                }
                return;
            }
            // Jump if Space is pressed
            if (state.IsKeyDown(Keys.Space) || state.IsKeyDown(Keys.Up) || state.IsKeyDown(Keys.W))
            {
                // Jump if the Space is pressed but not held and the dino is on the floor
                if (!spaceDown && dino.y >= screenHeight * SKYRATIO - 1) dino.dY = dinoJumpY;

                spaceDown = true;
            }
            else spaceDown = false;

            // Handle left and right
            if (state.IsKeyDown(Keys.Left) || state.IsKeyDown(Keys.A)) dino.dX = dinoSpeedX * -1;
            else if (state.IsKeyDown(Keys.Right) || state.IsKeyDown(Keys.D)) dino.dX = dinoSpeedX;
            else dino.dX = 0;

            // Reset the game if Enter is pressed AND the game is over
            if (gameOver && state.IsKeyDown(Keys.Enter)) { 
                StartGame();
                gameOver = false;
            }

        }
    }
}
