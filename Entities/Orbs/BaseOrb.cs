using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;

namespace DirtBox.Entities.Orbs
{
    public abstract class BaseOrb
    {
        public Texture2D Texture { get; protected set; }
        public Vector2 Position { get; set; }
        public float Rotation { get; protected set; }
        public float Scale { get; protected set; }

        protected float RotationSpeed;
        protected float ScaleAmplitude;

        protected BaseOrb(Texture2D texture, Vector2 initialPosition)
        {
            Texture = texture;
            Position = initialPosition;
            Rotation = 0f;
            Scale = 1.0f;
            RotationSpeed = 1.0f / 5; 
            ScaleAmplitude = 0.05f;
        }

        public virtual void Update(GameTime gameTime)
        {
            Rotation += RotationSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            Scale = 1.0f + ScaleAmplitude * (float)Math.Sin(Rotation * 3 * Math.PI);
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(
                Texture,
                Position,
                null, 
                new Color(0, 0, 0, 64), 
                -Rotation * 0.2f, 
                new Vector2(Texture.Width / 2f, Texture.Height / 2f), 
                Scale, 
                SpriteEffects.None,
                0f
            );

            spriteBatch.Draw(
                Texture,
                Position,
                null, 
                new Color(128, 128, 128, 64),
                Rotation,
                new Vector2(Texture.Width / 2f, Texture.Height / 2f), 
                Scale, 
                SpriteEffects.None,
                0f
            );

            spriteBatch.Draw(
                Texture,
                Position,
                null, 
                new Color(128, 128, 128, 32),
                Rotation * 2, 
                new Vector2(Texture.Width / 2f, Texture.Height / 2f), 
                Scale, 
                SpriteEffects.None,
                0f
            );
        }
    }
}