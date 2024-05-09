using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DirtBox.Entities.Orbs
{
    public class KnowledgeOrb : BaseOrb
    {
        public KnowledgeOrb(Texture2D texture, Vector2 initialPosition)
            : base(texture, initialPosition)
        {
            
        }

        public override void Update(GameTime gameTime)
        {
            // Update behavior for the KnowledgeOrb if different from BaseOrb
            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            // Optionally, customize the drawing for KnowledgeOrb
            base.Draw(spriteBatch);
        }
    }
}
