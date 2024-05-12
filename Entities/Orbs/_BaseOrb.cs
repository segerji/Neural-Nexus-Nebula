using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NNN.Entities.Events;
using NNN.Systems;

namespace NNN.Entities.Orbs;

public abstract class BaseOrb : ICollidable
{
    private const float InertiaFactor = 0.92f;
    private readonly Rectangle _movementBounds;
    private readonly EventBus _eventBus;
    private static readonly Random Random = new();

    protected float RotationSpeed;
    protected float ScaleAmplitude;

    // Constructor now only includes dependencies necessary for the lifetime of the object
    protected BaseOrb(Rectangle movementBounds, EventBus eventBus)
    {
        _movementBounds = movementBounds;
        _eventBus = eventBus;
        Rotation = 0f;
        Scale = 1.0f;
        RotationSpeed = 1.0f / 5;
        ScaleAmplitude = 0.05f;
        Velocity = Vector2.Zero;
        Radius = 20f; // Default value set here
    }

    // Properties that need initialization or configuration
    public Texture2D Texture { get; private set; }
    public Vector2 Position { get; private set; }
    public float Rotation { get; private set; }
    public float Scale { get; private set; }
    protected Vector2 Velocity { get; private set; }
    public float Radius { get; protected set; }

    // Initialize method for setting or resetting runtime-specific properties
    public void Initialize(Texture2D texture, Vector2? initialPosition)
    {
        Texture = texture;
        Position = ComputeStartPosition(initialPosition);
    }

    private Vector2 ComputeStartPosition(Vector2? initialPosition)
    {
        if (initialPosition.HasValue)
            return initialPosition.Value;

        return new Vector2(
            _movementBounds.X + Random.Next(_movementBounds.Width),
            _movementBounds.Y + Random.Next(_movementBounds.Height)
        );
    }

    public virtual bool Intersects(ICollidable entity)
    {
        var distance = Vector2.Distance(Position, entity.Position);
        return distance < Radius + entity.Radius;
    }
    
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