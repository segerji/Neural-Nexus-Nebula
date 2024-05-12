using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;

namespace NNN.Systems.Services;

public class DrawingService : IDrawingService
{
    private readonly Texture2D _pixel;

    public DrawingService(GraphicsDevice graphicsDevice)
    {
        _pixel = new Texture2D(graphicsDevice, 1, 1);
        _pixel.SetData(new[] { Color.White });
    }

    public void DrawLine(SpriteBatch spriteBatch, Vector2 start, Vector2 end, Color color)
    {
        Vector2 edge = end - start;
        float angle = (float)Math.Atan2(edge.Y, edge.X);

        spriteBatch.Draw(_pixel, start, null, color, angle, Vector2.Zero, new Vector2(edge.Length(), 1), SpriteEffects.None, 0);
    }

    public void DrawCircle(SpriteBatch spriteBatch, Vector2 center, float radius, Color color, int segments)
    {
        Vector2 lastPoint = center + new Vector2(radius, 0);
        Vector2 firstPoint = lastPoint;
        float angleStep = MathHelper.TwoPi / segments;

        for (int i = 1; i <= segments; i++)
        {
            float angle = angleStep * i;
            Vector2 newPoint = center + new Vector2((float)Math.Cos(angle) * radius, (float)Math.Sin(angle) * radius);
            DrawLine(spriteBatch, lastPoint, newPoint, color);
            lastPoint = newPoint;
        }

        // Reconnect to the first point
        DrawLine(spriteBatch, lastPoint, firstPoint, color);
    }
}
