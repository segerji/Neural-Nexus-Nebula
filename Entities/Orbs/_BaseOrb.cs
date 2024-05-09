using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NNN.Entities.Events;
using NNN.Systems;

namespace NNN.Entities.Orbs;

public abstract class BaseOrb : ICollidable
{
    private const float InertiaFactor = 0.92f;
    private readonly EventBus _eventBus;
    private Rectangle _movementBounds;
    private static readonly Random Random = new();

    protected float RotationSpeed;
    protected float ScaleAmplitude;

    protected BaseOrb(Texture2D texture, Vector2? initialPosition, Rectangle bounds, EventBus eventBus)
    {
        Texture = texture;
        Position = ComputeStartPosition(initialPosition, bounds);
        Rotation = 0f;
        Scale = 1.0f;
        RotationSpeed = 1.0f / 5;
        ScaleAmplitude = 0.05f;
        Velocity = Vector2.Zero;
        _movementBounds = bounds;
        _eventBus = eventBus;
        Radius = Radius == 0 ? 20f : Radius;
    }

    public Texture2D Texture { get; protected set; }
    public float Rotation { get; protected set; }
    public float Scale { get; protected set; }
    protected Vector2 Velocity { get; set; }
    public float Radius { get; set; }

    private static Vector2 ComputeStartPosition(Vector2? initialPosition, Rectangle bounds)
    {
        if (initialPosition.HasValue) return initialPosition.Value;

        return new Vector2(
            bounds.X + Random.Next(bounds.Width),
            bounds.Y + Random.Next(bounds.Height)
        );
    }

    public virtual bool Intersects(ICollidable entity)
    {
        var distance = Vector2.Distance(Position, entity.Position);
        return distance < Radius + entity.Radius;
    }

    public Vector2 Position { get; set; }

    public virtual void Update(GameTime gameTime)
    {
        Update(gameTime, Vector2.Zero);
    }

    public virtual void Draw(SpriteBatch spriteBatch)
    {
        var scale = Scale * (2 * Radius / (Texture.Width * 0.6f));

        spriteBatch.Draw(
            Texture,
            Position,
            null,
            new Color(0, 0, 0, 64),
            -Rotation * 0.2f,
            new Vector2(Texture.Width / 2f, Texture.Height / 2f),
            scale,
            SpriteEffects.None,
            0f
        );

        spriteBatch.Draw(
            Texture,
            Position,
            null,
            new Color(128, 128, 128, 32),
            Rotation + 128,
            new Vector2(Texture.Width / 2f, Texture.Height / 2f),
            scale,
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
            scale,
            SpriteEffects.None,
            0f
        );
    }

    public virtual void Destroy()
    {
        _eventBus.Publish(new ObjectDestroyedEvent(this));
    }

    public virtual void Update(GameTime gameTime, Vector2 movementVector)
    {
        Rotation += RotationSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
        Scale = 1.0f + ScaleAmplitude * (float)Math.Sin(Rotation * 3 * Math.PI);
        Velocity = Velocity * InertiaFactor + movementVector;
        Position += Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
        CheckBounds();
    }

    private void CheckBounds()
    {
        if (Position.X < _movementBounds.Left + Radius)
        {
            Position = Position with { X = _movementBounds.Left + Radius };
            Velocity = Velocity with { X = -1 };
        }
        else if (Position.X > _movementBounds.Right - Radius)
        {
            Position = Position with { X = _movementBounds.Right - Radius };
            Velocity = Velocity with { X = -1 };
        }

        if (Position.Y < _movementBounds.Top + Radius)
        {
            Position = Position with { Y = _movementBounds.Top + Radius };
            Velocity = Velocity with { Y = -1 };
        }
        else if (Position.Y > _movementBounds.Bottom - Radius)
        {
            Position = Position with { Y = _movementBounds.Bottom - Radius };
            Velocity = Velocity with { Y = -1 };
        }
    }
}