using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace NNN.Systems.Services;

public interface IDrawingService
{
    void DrawLine(SpriteBatch spriteBatch, Vector2 start, Vector2 end, Color color);
    void DrawCircle(SpriteBatch spriteBatch, Vector2 center, float radius, Color color, int segments = 100);
}