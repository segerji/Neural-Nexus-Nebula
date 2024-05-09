using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using NNN.Systems;

namespace NNN.Entities.Orbs;

public class PlayerOrb : AlienOrb
{
    private const int DeadZone = 4096;

    public PlayerOrb(Texture2D texture, Vector2 initialPosition, float speed, Rectangle bounds, EventBus eventBus)
        : base(texture, initialPosition, bounds, eventBus)
    {
        Speed = speed;
    }

    public float Speed { get; set; }

    public override void Update(GameTime gameTime)
    {
        var movementVector = CalculateMovement();
        base.Update(gameTime, movementVector);
    }

    private Vector2 CalculateMovement()
    {
        var keyboardState = Keyboard.GetState();

        var totalMovementX = 0f;
        var totalMovementY = 0f;

        if (keyboardState.IsKeyDown(Keys.Up))
            totalMovementY -= 1f;

        if (keyboardState.IsKeyDown(Keys.Down))
            totalMovementY += 1f;

        if (keyboardState.IsKeyDown(Keys.Left))
            totalMovementX -= 1f;

        if (keyboardState.IsKeyDown(Keys.Right))
            totalMovementX += 1f;

        var jState = Joystick.GetState((int)PlayerIndex.One);

        var axisX = jState.Axes[0] / 32768f;
        var axisY = jState.Axes[1] / 32768f;

        if (Math.Abs(axisY) > DeadZone / 32768f) totalMovementY += axisY;

        if (Math.Abs(axisX) > DeadZone / 32768f) totalMovementX += axisX;

        // Normalize the total movement vector
        var totalMovementLength = (float)Math.Sqrt(totalMovementX * totalMovementX + totalMovementY * totalMovementY);
        if (totalMovementLength > 1f)
        {
            totalMovementX /= totalMovementLength;
            totalMovementY /= totalMovementLength;
        }

        // Calculate the movement vector without considering inertia
        var movementVector = new Vector2(Speed * totalMovementX, Speed * totalMovementY);

        // Return the movement vector
        return movementVector;
    }
}