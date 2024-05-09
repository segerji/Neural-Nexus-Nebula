using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace NNN.Entities;

public interface IEntity
{
    Vector2 Position { get; }
    void Update(GameTime gameTime);
    void Draw(SpriteBatch spriteBatch);
}

public interface ICollidable : IEntity
{
    float Radius { get; }
    bool Intersects(ICollidable entity);
}