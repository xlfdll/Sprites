using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Sprites
{
	/// <summary>
	/// This is the main type for your game
	/// </summary>
	public class MainGame : Game
	{
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

			backgroundTexture2D = Content.Load<Texture2D>(@"Sprites\background");

			cannon = new GameObject(Content.Load<Texture2D>(@"Sprites\cannon"));
			cannon.Position = new Vector2(120, graphics.GraphicsDevice.Viewport.Height - 80);

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
			{
				this.Exit();
			}

			GamePadState gamePadState = GamePad.GetState(PlayerIndex.One);

			cannon.Rotation += gamePadState.ThumbSticks.Left.X * 0.1f;

			if (gamePadState.Buttons.A == ButtonState.Pressed && previousGamePadState.Buttons.A == ButtonState.Released)
			{
				this.FireCannonBall();
			}

			previousGamePadState = gamePadState;

#if !XBOX
			KeyboardState keyboardState = Keyboard.GetState();

			if (keyboardState.IsKeyDown(Keys.Left))
			{
				cannon.Rotation -= 0.1f;
			}
			if (keyboardState.IsKeyDown(Keys.Right))
			{
				cannon.Rotation += 0.1f;
			}
			if (keyboardState.IsKeyDown(Keys.Space) && previousKeyboardState.IsKeyUp(Keys.Space))
			{
				this.FireCannonBall();
			}

			previousKeyboardState = keyboardState;
#endif

			cannon.Rotation = MathHelper.Clamp(cannon.Rotation, -MathHelper.PiOver2, 0);

			this.UpdateCannonBalls();
			this.UpdateEnemies();

			base.Update(gameTime);
		}

		/// <summary>
		/// This is called when the game should draw itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.CornflowerBlue);

			spriteBatch.Begin();

			spriteBatch.Draw(backgroundTexture2D, viewportRect, Color.White);

			foreach (GameObject ball in cannonBalls)
			{
				if (ball.IsAlive)
				{
					spriteBatch.Draw(ball.Sprite, ball.Position, Color.White);
				}
			}

			spriteBatch.Draw(cannon.Sprite, cannon.Position, null, Color.White, cannon.Rotation, cannon.Center, 1.0f, SpriteEffects.None, 0);

			foreach (GameObject enemy in enemies)
			{
				if (enemy.IsAlive)
				{
					spriteBatch.Draw(enemy.Sprite, enemy.Position, Color.White);
				}
			}

			spriteBatch.DrawString(font, "Score: " + score.ToString(), new Vector2(scoreDrawPoint.X * viewportRect.Width, scoreDrawPoint.Y * viewportRect.Height), Color.Yellow);

			spriteBatch.End();

			base.Draw(gameTime);
		}

		public void UpdateCannonBalls()
		{
			foreach (GameObject ball in cannonBalls)
			{
				if (ball.IsAlive)
				{
					ball.Position += ball.Velocity;

					if (!viewportRect.Contains(new Point((Int32)ball.Position.X, (Int32)ball.Position.Y)))
					{
						ball.IsAlive = false;

						continue;
					}

					Rectangle cannonBallRect = new Rectangle((Int32)ball.Position.X, (Int32)ball.Position.Y, ball.Sprite.Width, ball.Sprite.Height);

					foreach (GameObject enemy in enemies)
					{
						Rectangle enemyRect = new Rectangle((Int32)enemy.Position.X, (Int32)enemy.Position.Y, enemy.Sprite.Width, enemy.Sprite.Height);

						if (cannonBallRect.Intersects(enemyRect))
						{
							ball.IsAlive = false;
							enemy.IsAlive = false;

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
				if (enemy.IsAlive)
				{
					enemy.Position += enemy.Velocity;

					if (!viewportRect.Contains(new Point((Int32)enemy.Position.X, (Int32)enemy.Position.Y)))
					{
						enemy.IsAlive = false;
					}
				}
				else
				{
					enemy.IsAlive = true;
					enemy.Position = new Vector2(viewportRect.Right,
						MathHelper.Lerp(
						(Single)viewportRect.Height * minEnemyHeight,
						(Single)viewportRect.Height * maxEnemyHeight,
						(Single)random.NextDouble()));
					enemy.Velocity = new Vector2(MathHelper.Lerp(-minEnemyVelocity, -maxEnemyVelocity, (Single)random.NextDouble()), 0);
				}
			}
		}

		public void FireCannonBall()
		{
			foreach (GameObject ball in cannonBalls)
			{
				if (!ball.IsAlive)
				{
					ball.IsAlive = true;
					ball.Position = cannon.Position + new Vector2((Single)Math.Sin(cannon.Sprite.Height)) - ball.Center;
					ball.Velocity = new Vector2((Single)Math.Cos(cannon.Rotation), (Single)Math.Sin(cannon.Rotation)) * 5.0f;

					return;
				}
			}
		}

		private GraphicsDeviceManager graphics;
		private SpriteBatch spriteBatch;

		private Texture2D backgroundTexture2D;
		private Rectangle viewportRect;

		private GameObject cannon;
		private GameObject[] cannonBalls;

		private GameObject[] enemies;

		private SpriteFont font;
		private Vector2 scoreDrawPoint = new Vector2(0.1f, 0.1f);

		private GamePadState previousGamePadState = GamePad.GetState(PlayerIndex.One);
		private KeyboardState previousKeyboardState = Keyboard.GetState();

		private readonly Int32 maxCannonBalls = 10;
		private readonly Int32 maxEnemies = 10;

		private readonly Single maxEnemyHeight = 0.1f;
		private readonly Single minEnemyHeight = 0.5f;
		private readonly Single maxEnemyVelocity = 5.0f;
		private readonly Single minEnemyVelocity = 1.0f;

		private Int32 score = 0;
		private Random random = new Random();
	}
}