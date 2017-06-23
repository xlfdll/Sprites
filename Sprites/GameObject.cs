using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Sprites
{
	public class GameObject
	{
		public GameObject(Texture2D loadedTexture)
		{
			this.Sprite = loadedTexture;
			this.Position = Vector2.Zero;
			this.Center = new Vector2(Sprite.Width / 2, Sprite.Height / 2);
			this.Rotation = 0.0f;
			this.Velocity = Vector2.Zero;
			this.IsAlive = false;
		}

		public Texture2D Sprite { get; set; }
		public Vector2 Position { get; set; }
		public Vector2 Center { get; set; }
		public Single Rotation { get; set; }
		public Vector2 Velocity { get; set; }
		public Boolean IsAlive { get; set; }
	}
}