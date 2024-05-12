using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NNN.Systems;
using NNN.Systems.Services;

namespace NNN.Entities.Orbs;

public class AlienOrb : BaseOrb
{
    private readonly IDrawingService _drawingService;
    public float VisionRange { get; set; }
    public List<BaseOrb> VisibleOrbs { get; set; } = new List<BaseOrb>();

    public AlienOrb(IDrawingService drawingService, Rectangle bounds, EventBus eventBus)
        : base(bounds, eventBus)
    {
        _drawingService = drawingService;
        this.VisionRange = 120f;
    }

    public void Initialize(Texture2D texture, Vector2? initialPosition, float speed)
    {
        this.Speed = speed;
        base.Initialize(texture, initialPosition);
    }

    public float Speed { get; set; }

    public int KnowledgeScore { get; set; }
    
    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime, new Vector2());
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        base.Draw(spriteBatch);
        
        _drawingService.DrawCircle(spriteBatch, Position, VisionRange, Color.Red);
        foreach (var orb in VisibleOrbs)
        {
            _drawingService.DrawLine(spriteBatch, Position, orb.Position, Color.Yellow);
        }
    }


    public override bool Intersects(ICollidable entity)
    {
        if (!base.Intersects(entity))
            return false;

        if (entity is not KnowledgeOrb orb)
            return true;

        KnowledgeScore++;
        orb.Destroy();

        return true;
    }
}