using Microsoft.Xna.Framework;
using NNN.Systems;

namespace NNN.Entities.Orbs;

public class KnowledgeOrb : BaseOrb
{
    public KnowledgeOrb(Rectangle bounds, EventBus eventBus)
        : base(bounds, eventBus)
    {
        Radius = 5f;
    }
    
    public override void Update(GameTime gameTime)
    {
        // Update behavior for the KnowledgeOrb if different from BaseOrb
        base.Update(gameTime);
    }
}