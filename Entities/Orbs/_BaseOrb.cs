using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NNN.Entities.Events;
using NNN.Systems;

namespace NNN.Entities.Orbs;

public abstract class BaseOrb : ICollidable
{
    private const float InertiaFactor = 0.92f;
    private static readonly Random Random = new();
    protected readonly EventBus _eventBus;
    protected readonly Rectangle MovementBounds;
    private readonly float _rotationSpeed;
    private readonly float _scaleAmplitude;
    private float _rotation;

    // Properties that need initialization or configuration
    public Texture2D Texture { get; private set; }

    // Constructor now only includes dependencies necessary for the lifetime of the object
    protected BaseOrb(Rectangle movementBounds, EventBus eventBus)
    {
        MovementBounds = movementBounds;
        _eventBus = eventBus;
        _rotation = 0f;
        Scale = 1.0f;
        _rotationSpeed = 1.0f / 5;
        _scaleAmplitude = 0.05f;
        Velocity = Vector2.Zero;
        Radius = 20f;
    }

    public float Scale { get; private set; }
    protected Vector2 Velocity { get; set; }
    public Vector2 Position { get; protected set; }
    public float Radius { get; protected set; }

    public bool IsDisposed { get; protected set; }

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
            -_rotation * 0.2f,
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
            _rotation + 128,
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
            _rotation * 2,
            new Vector2(Texture.Width / 2f, Texture.Height / 2f),
            scale,
            SpriteEffects.None,
            0f
        );
    }

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
            MovementBounds.X + Random.Next(MovementBounds.Width),
            MovementBounds.Y + Random.Next(MovementBounds.Height)
        );
    }

    public virtual void Destroy()
    {
        if (IsDisposed)
            return;

        IsDisposed = true;
        _eventBus.Publish(new ObjectDestroyedEvent(this));        
    }

    public virtual void Update(GameTime gameTime, Vector2 movementVector)
    {
        _rotation += _rotationSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
        Scale = 1.0f + _scaleAmplitude * (float)Math.Sin(_rotation * 3 * Math.PI);
        Velocity = Velocity * InertiaFactor + movementVector;
        Position += Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
        CheckBounds();
    }

    private void CheckBounds()
    {
        if (Position.X < MovementBounds.Left + Radius)
        {
            Position = Position with { X = MovementBounds.Left + Radius };
            Velocity = Velocity with { X = Math.Abs(Velocity.X) }; // Reflect velocity in X direction
        }
        else if (Position.X > MovementBounds.Right - Radius)
        {
            Position = Position with { X = MovementBounds.Right - Radius };
            Velocity = Velocity with { X = -Math.Abs(Velocity.X) }; // Reflect velocity in X direction
        }

        if (Position.Y < MovementBounds.Top + Radius)
        {
            Position = Position with { Y = MovementBounds.Top + Radius };
            Velocity = Velocity with { Y = Math.Abs(Velocity.Y) }; // Reflect velocity in Y direction
        }
        else if (Position.Y > MovementBounds.Bottom - Radius)
        {
            Position = Position with { Y = MovementBounds.Bottom - Radius };
            Velocity = Velocity with { Y = -Math.Abs(Velocity.Y) }; // Reflect velocity in Y direction
        }
    }
}