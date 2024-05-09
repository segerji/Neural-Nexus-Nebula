using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DirtBox.Entities.Orbs
{
    public class AlienOrb : BaseOrb
    {
        public AlienOrb(Texture2D texture, Vector2 initialPosition)
            : base(texture, initialPosition)
        {
            // Additional properties specific to AlienOrb
        }

        public override void Update(GameTime gameTime)
        {
            // AI behavior logic here
            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            // Optionally, customize the drawing for AlienOrb
            base.Draw(spriteBatch);
        }
    }
}