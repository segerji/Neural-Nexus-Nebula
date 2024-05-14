using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NNN.Entities.Events;
using NNN.Systems;
using NNN.Systems.Services;

namespace NNN.Entities.Orbs;

public class AlienOrb : BaseOrb
{
    private const int MaxVisibleOrbs = 5;
    private readonly IDrawingService _drawingService;

    private static int BrainInputSize => 1 + 2 + 2 + (MaxVisibleOrbs * 2);

    public AlienOrb(IDrawingService drawingService, Rectangle bounds, EventBus eventBus)
        : base(bounds, eventBus)
    {
        _drawingService = drawingService;
        VisionRange = 200f;
        Brain = new OrbBrain(BrainInputSize, 2, new[] { BrainInputSize, Convert.ToInt32(BrainInputSize/2f) });
    }
    

    public OrbBrain Brain { get; set; }
    public List<KnowledgeOrb> VisibleKnowledgeOrbs { get; set; } = new();
    public AlienOrb ClosestAlienOrb { get; set; }
    public float Speed { get; set; } // How fast the orb can move
    public float VisionRange { get; set; } // How far the orb can see other entities
    public float KnowledgeScore { get; private set; }

    public void Initialize(Texture2D texture, Vector2? initialPosition, float speed)
    {
        Speed = speed;
        base.Initialize(texture, initialPosition);
    }

    public override void Update(GameTime gameTime)
    {
        var inputs = GatherInputs();
        var output = Brain.Predict(inputs);
        var movementVector = new Vector2(output[0], output[1]);

        movementVector.Normalize();

        var preMovePosition = new Vector2(Position.X, Position.Y);

        base.Update(gameTime, movementVector * Speed);

        var postMovePosition = new Vector2(Position.X, Position.Y);

        if (preMovePosition != postMovePosition) KnowledgeScore += 0.01f;

        PunishForEdges(postMovePosition);
    }

    private void PunishForEdges(Vector2 position)
    {
        if (IsTouchingWall(position))
        {
            KnowledgeScore -= 0.20f;
        }
    }

    private bool IsTouchingWall(Vector2 position)
    {
        return position.X - Radius <= MovementBounds.Left ||
               position.X + Radius >= MovementBounds.Right ||
               position.Y - Radius <= MovementBounds.Top ||
               position.Y + Radius >= MovementBounds.Bottom;
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        base.Draw(spriteBatch);
        // TODO : show when debugging
        //_drawingService.DrawCircle(spriteBatch, Position, VisionRange, Color.Red);
        //foreach (var orb in VisibleKnowledgeOrbs)
        //    _drawingService.DrawLine(spriteBatch, Position, orb.Position, Color.Yellow);
    }

    private float[] GatherInputs()
    {
        var inputs = new List<float>();

        inputs.Add(IsTouchingWall(Position) ? 1 : 0);

        var normalizedX = Position.X / MovementBounds.Width;
        var normalizedY = Position.Y / MovementBounds.Height;

        inputs.Add(normalizedX);
        inputs.Add(normalizedY);

        var normalizedClosestAlienOrbX = -1.0f;
        var normalizedClosestAlienOrbY = -1.0f;

        if (ClosestAlienOrb != null){
            normalizedClosestAlienOrbX = ClosestAlienOrb.Position.X / MovementBounds.Width;
            normalizedClosestAlienOrbY = ClosestAlienOrb.Position.Y / MovementBounds.Height;
        }

        inputs.Add(normalizedClosestAlienOrbX);
        inputs.Add(normalizedClosestAlienOrbY);

        foreach (var orb in VisibleKnowledgeOrbs)
        {
            var visibleKnowledgeOrbNormalizedX = orb.Position.X / MovementBounds.Width;
            var visibleKnowledgeOrbNormalizedY = orb.Position.Y / MovementBounds.Height;

            visibleKnowledgeOrbNormalizedX = Math.Clamp(visibleKnowledgeOrbNormalizedX, 0.0f, 1.0f);
            visibleKnowledgeOrbNormalizedY = Math.Clamp(visibleKnowledgeOrbNormalizedY, 0.0f, 1.0f);

            inputs.Add(visibleKnowledgeOrbNormalizedX);
            inputs.Add(visibleKnowledgeOrbNormalizedY);
        }

        while (inputs.Count < BrainInputSize)
        {
            inputs.Add(-1);
            inputs.Add(-1);
        }

        return inputs.ToArray();
    }

    public override bool Intersects(ICollidable entity)
    {
        if (!base.Intersects(entity))
            return false;

        switch (entity)
        {
            case KnowledgeOrb orb:
                KnowledgeScore++;
                orb.Destroy();
                break;
            case AlienOrb:
                KnowledgeScore -= 0.20f;
                break;
        }

        return true;
    }
}