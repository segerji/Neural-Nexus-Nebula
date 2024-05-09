using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NNN.Systems;

namespace NNN.Entities.Orbs;

public class KnowledgeOrb : BaseOrb
{
    public KnowledgeOrb(Texture2D texture, Vector2? initialPosition, Rectangle bounds, EventBus eventBus)
        : base(texture, initialPosition, bounds, eventBus)
    {
        Radius = 5f;
    }
    
    public override void Update(GameTime gameTime)
    {
        // Update behavior for the KnowledgeOrb if different from BaseOrb
        base.Update(gameTime);
    }
}