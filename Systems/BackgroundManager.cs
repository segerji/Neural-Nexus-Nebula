using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace NNN.Systems;

public class BackgroundManager
{
    private const float ScrollSpeed1 = 5f;
    private const float ScrollSpeed2 = 8f;
    private const float ScrollSpeed3 = 12f;
    private readonly Texture2D _layer1;
    private readonly Texture2D _layer2;
    private readonly Texture2D _layer3;
    private Vector2 _scrollPosition1;
    private Vector2 _scrollPosition2;
    private Vector2 _scrollPosition3;

    public BackgroundManager(ContentManager content)
    {
        _layer1 = content.Load<Texture2D>("Textures/Parallax100");
        _layer2 = content.Load<Texture2D>("Textures/Parallax80");
        _layer3 = content.Load<Texture2D>("Textures/Parallax60");
        _scrollPosition1 = new Vector2(new Random().Next(_layer1.Width), new Random().Next(_layer1.Height));
        _scrollPosition2 = new Vector2(new Random().Next(_layer2.Width), new Random().Next(_layer2.Height));
        _scrollPosition3 = new Vector2(new Random().Next(_layer3.Width), new Random().Next(_layer3.Height));
    }

    public void Update(GameTime gameTime)
    {
        var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
        _scrollPosition1.X += 0.5f * deltaTime;
        _scrollPosition2.X += 0.3f * deltaTime;
        _scrollPosition3.X += 0.1f * deltaTime;

        _scrollPosition1.Y += ScrollSpeed1 * deltaTime;
        _scrollPosition2.Y += ScrollSpeed2 * deltaTime;
        _scrollPosition3.Y += ScrollSpeed3 * deltaTime;

        _scrollPosition1.X %= _layer1.Width;
        _scrollPosition2.X %= _layer2.Width;
        _scrollPosition3.X %= _layer3.Width;

        _scrollPosition1.Y %= _layer1.Height;
        _scrollPosition2.Y %= _layer2.Height;
        _scrollPosition3.Y %= _layer3.Height;
    }

    public void Draw(SpriteBatch spriteBatch, Viewport viewport)
    {
        DrawBackgroundLayer(spriteBatch, _layer1, _scrollPosition1, viewport);
        DrawBackgroundLayer(spriteBatch, _layer2, _scrollPosition2, viewport);
        DrawBackgroundLayer(spriteBatch, _layer3, _scrollPosition3, viewport);
    }

    private void DrawBackgroundLayer(SpriteBatch spriteBatch, Texture2D texture, Vector2 scrollPosition,
        Viewport viewport)
    {
        var textureWidth = texture.Width;
        var textureHeight = texture.Height;

        // Adjust the initial positions for tiling
        var xStart = scrollPosition.X % textureWidth;
        if (xStart > 0) xStart -= textureWidth;

        var yStart = scrollPosition.Y % textureHeight;
        if (yStart > 0) yStart -= textureHeight;

        // Loop to cover the entire viewport
        for (var x = xStart; x < viewport.Width; x += textureWidth)
        for (var y = yStart; y < viewport.Height; y += textureHeight)
            spriteBatch.Draw(texture, new Vector2(x, y), Color.White);
    }
}