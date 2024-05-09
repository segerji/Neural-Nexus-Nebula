using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NNN.Systems;

namespace NNN.Entities.Orbs;

public class AlienOrb : BaseOrb
{
    public AlienOrb(Texture2D texture, Vector2 initialPosition, Rectangle bounds, EventBus eventBus)
        : base(texture, initialPosition, bounds, eventBus)
    {
    }

    public int KnowledgeScore { get; set; }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime, new Vector2());
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