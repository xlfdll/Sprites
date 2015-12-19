using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Sprites
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class MainGame : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Texture2D backgroundTexture2D;
        Rectangle viewportRect;

        GameObject cannon;
        GameObject[] cannonBalls;

        GameObject[] enemies;

        SpriteFont font;
        Vector2 scoreDrawPoint = new Vector2(0.1f, 0.1f);

        GamePadState previousGamePadState = GamePad.GetState(PlayerIndex.One);
        KeyboardState previousKeyboardState = Keyboard.GetState();

        readonly Int32 maxCannonBalls = 10;
        readonly Int32 maxEnemies = 10;

        readonly Single maxEnemyHeight = 0.1f;
        readonly Single minEnemyHeight = 0.5f;
        readonly Single maxEnemyVelocity = 5.0f;
        readonly Single minEnemyVelocity = 1.0f;

        Random random = new Random();
        Int32 score = 0;

        public MainGame()
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

            // TODO: use this.Content to load your game content here
            backgroundTexture2D = Content.Load<Texture2D>(@"Sprites\background");

            cannon = new GameObject(Content.Load<Texture2D>(@"Sprites\cannon"));
            cannon.position = new Vector2(120, graphics.GraphicsDevice.Viewport.Height - 80);

            cannonBalls = new GameObject[maxCannonBalls];

            for (Int32 i = 0; i < maxCannonBalls; i++)
            {
                cannonBalls[i] = new GameObject(Content.Load<Texture2D>(@"Sprites\cannonball"));
            }

            enemies = new GameObject[maxEnemies];

            for (Int32 i = 0; i < maxEnemies; i++)
            {
                enemies[i] = new GameObject(Content.Load<Texture2D>(@"Sprites\enemy"));
            }

            font = Content.Load<SpriteFont>(@"Fonts\GameFont");

            viewportRect = new Rectangle(0, 0, graphics.GraphicsDevice.Viewport.Width, graphics.GraphicsDevice.Viewport.Height);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here

            base.UnloadContent();
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // TODO: Add your update logic here
            GamePadState gamePadState = GamePad.GetState(PlayerIndex.One);
            cannon.rotation += gamePadState.ThumbSticks.Left.X * 0.1f;

            if (gamePadState.Buttons.A == ButtonState.Pressed && previousGamePadState.Buttons.A == ButtonState.Released)
            {
                FireCannonBall();
            }

            previousGamePadState = gamePadState;

#if !XBOX
            KeyboardState keyboardState = Keyboard.GetState();

            if (keyboardState.IsKeyDown(Keys.Left))
            {
                cannon.rotation -= 0.1f;
            }
            if (keyboardState.IsKeyDown(Keys.Right))
            {
                cannon.rotation += 0.1f;
            }
            if (keyboardState.IsKeyDown(Keys.Space) && previousKeyboardState.IsKeyUp(Keys.Space))
            {
                FireCannonBall();
            }

            previousKeyboardState = keyboardState;

            //MouseState mouseState = Mouse.GetState();
#endif

            cannon.rotation = MathHelper.Clamp(cannon.rotation, -MathHelper.PiOver2, 0);

            UpdateCannonBalls();
            UpdateEnemies();

            base.Update(gameTime);
        }

        public void UpdateCannonBalls()
        {
            foreach (GameObject ball in cannonBalls)
            {
                if (ball.alive)
                {
                    ball.position += ball.velocity;

                    if (!viewportRect.Contains(new Point((Int32)ball.position.X, (Int32)ball.position.Y)))
                    {
                        ball.alive = false;
                        continue;
                    }

                    Rectangle cannonBallRect = new Rectangle((Int32)ball.position.X, (Int32)ball.position.Y, ball.sprite.Width, ball.sprite.Height);

                    foreach (GameObject enemy in enemies)
                    {
                        Rectangle enemyRect = new Rectangle((Int32)enemy.position.X, (Int32)enemy.position.Y, enemy.sprite.Width, enemy.sprite.Height);

                        if (cannonBallRect.Intersects(enemyRect))
                        {
                            ball.alive = false;
                            enemy.alive = false;

                            score += 1;

                            break;
                        }
                    }
                }
            }
        }

        public void UpdateEnemies()
        {
            foreach (GameObject enemy in enemies)
            {
                if (enemy.alive)
                {
                    enemy.position += enemy.velocity;

                    if (!viewportRect.Contains(new Point((Int32)enemy.position.X, (Int32)enemy.position.Y)))
                    {
                        enemy.alive = false;
                    }
                }
                else
                {
                    enemy.alive = true;
                    enemy.position = new Vector2(viewportRect.Right,
                        MathHelper.Lerp(
                        (Single)viewportRect.Height * minEnemyHeight,
                        (Single)viewportRect.Height * maxEnemyHeight,
                        (Single)random.NextDouble()));
                    enemy.velocity = new Vector2(MathHelper.Lerp(-minEnemyVelocity, -maxEnemyVelocity, (Single)random.NextDouble()), 0);
                }
            }
        }

        public void FireCannonBall()
        {
            foreach (GameObject ball in cannonBalls)
            {
                if (!ball.alive)
                {
                    ball.alive = true;
                    ball.position = cannon.position + new Vector2((Single)Math.Sin(cannon.sprite.Height)) - ball.center;
                    ball.velocity = new Vector2((Single)Math.Cos(cannon.rotation), (Single)Math.Sin(cannon.rotation)) * 5.0f;

                    return;
                }
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            spriteBatch.Begin();

            spriteBatch.Draw(backgroundTexture2D, viewportRect, Color.White);

            foreach (GameObject ball in cannonBalls)
            {
                if (ball.alive)
                {
                    spriteBatch.Draw(ball.sprite, ball.position, Color.White);
                }
            }

            spriteBatch.Draw(cannon.sprite, cannon.position, null, Color.White, cannon.rotation, cannon.center, 1.0f, SpriteEffects.None, 0);

            foreach (GameObject enemy in enemies)
            {
                if (enemy.alive)
                {
                    spriteBatch.Draw(enemy.sprite, enemy.position, Color.White);
                }
            }

            spriteBatch.DrawString(font, "Score: " + score.ToString(), new Vector2(scoreDrawPoint.X * viewportRect.Width, scoreDrawPoint.Y * viewportRect.Height), Color.Yellow);

            spriteBatch.End();
            
            base.Draw(gameTime);
        }
    }
}
