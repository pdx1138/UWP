using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoGame2D
{
    class SpriteClass
    {
        const float HITBOXSCALE = .5f;

        public Texture2D texture
        {
            get;
        }

        public float x
        {
            get;
            set;
        }

        public float y
        {
            get;
            set;
        }

        public float angle
        {
            get;
            set;
        }

        public float dX
        {
            get;
            set;
        }

        public float dY
        {
            get;
            set;
        }

        public float dA
        {
            get;
            set;
        }

        public float scale
        {
            get;
            set;
        }

        public SpriteClass(GraphicsDevice graphicsDevice, string textureName, float scale)
        {
            this.scale = scale;

            // Load the specified texture
            //var stream = TitleContainer.OpenStream(textureName);
            //texture = Texture2D.FromStream(graphicsDevice, stream);

            if (texture == null)
            {
                using (var stream = TitleContainer.OpenStream(textureName))
                {
                    texture = Texture2D.FromStream(graphicsDevice, stream);
                }
            }
        }

        public void Update(float elapsedTime)
        {
            this.x += this.dX * elapsedTime;
            this.y += this.dY * elapsedTime;
            this.angle += this.dA * elapsedTime;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Vector2 spritePosition = new Vector2(this.x, this.y);
            spriteBatch.Draw(texture, spritePosition, null, Color.White, this.angle, new Vector2(texture.Width / 2, texture.Height / 2), new Vector2(scale, scale), SpriteEffects.None, 0f);
        }

        public bool RectangleCollision(SpriteClass otherSprite)
        {
            if (this.x + this.texture.Width * this.scale * HITBOXSCALE / 2 < otherSprite.x - otherSprite.texture.Width * otherSprite.scale / 2) return false;
            if (this.y + this.texture.Height * this.scale * HITBOXSCALE / 2 < otherSprite.y - otherSprite.texture.Height * otherSprite.scale / 2) return false;
            if (this.x - this.texture.Width * this.scale * HITBOXSCALE / 2 > otherSprite.x + otherSprite.texture.Width * otherSprite.scale / 2) return false;
            if (this.y - this.texture.Height * this.scale * HITBOXSCALE / 2 > otherSprite.y + otherSprite.texture.Height * otherSprite.scale / 2) return false;
            return true;
        }
    }
}
