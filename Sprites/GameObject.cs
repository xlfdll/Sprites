using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Sprites
{
    class GameObject
    {
        public Texture2D sprite;
        public Vector2 position;
        public Single rotation;
        public Vector2 center;
        public Vector2 velocity;
        public Boolean alive;

        public GameObject(Texture2D loadedTexture)
        {
            rotation = 0.0f;
            position = Vector2.Zero;
            sprite = loadedTexture;
            center = new Vector2(sprite.Width / 2, sprite.Height / 2);
            velocity = Vector2.Zero;
            alive = false;
        }
    }
}