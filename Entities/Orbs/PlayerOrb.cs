using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace DirtBox.Entities.Orbs
{
    public class PlayerOrb : BaseOrb
    {
        public float Speed { get; set; }
        private const int DeadZone = 4096;

        public PlayerOrb(Texture2D texture, Vector2 initialPosition, float speed)
            : base(texture, initialPosition)
        {
            Speed = speed;
        }

        public override void Update(GameTime gameTime)
        {
            HandleInput(gameTime);
            base.Update(gameTime);
        }

        private void HandleInput(GameTime gameTime)
        {
            var keyboardState = Keyboard.GetState();
            var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (keyboardState.IsKeyDown(Keys.Up))
                Position = Position with { Y = Position.Y - (Speed * deltaTime) };

            if (keyboardState.IsKeyDown(Keys.Down))
                Position = Position with { Y = Position.Y + (Speed * deltaTime) };

            if (keyboardState.IsKeyDown(Keys.Left))
                Position = Position with { X = Position.X - (Speed * deltaTime) };

            if (keyboardState.IsKeyDown(Keys.Right))
                Position = Position with { X = Position.X + (Speed * deltaTime) };

            if (Joystick.LastConnectedIndex != 0)
                return;

            var jState = Joystick.GetState((int)PlayerIndex.One);

            var updatedBallSpeed = Speed * ((float)gameTime.ElapsedGameTime.TotalMilliseconds / 1000);

            var axisX = jState.Axes[0] / 32768f;
            var axisY = jState.Axes[1] / 32768f;

            if (Math.Abs(axisY) > (DeadZone / 32768f))
            {
                Position = Position with { Y = Position.Y + (updatedBallSpeed * axisY) };
            }

            if (Math.Abs(axisX) > (DeadZone / 32768f))
            {
                Position = Position with { X = Position.X + (updatedBallSpeed * axisX) };
            }
        }
    }
}
