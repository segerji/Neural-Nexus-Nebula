using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace NNN.Systems;

public class ViewportManager
{
    private const int BorderWidth = 2;
    private const int MarginHorizontal = 200;
    private const int MarginVertical = 4;

    private readonly GraphicsDeviceManager _graphics;
    private readonly SpriteBatch _spriteBatch;
    private Texture2D _borderTexture;
    private Viewport _gameplayViewport;
    private Viewport _fullScreenViewport;

    public ViewportManager(GraphicsDeviceManager graphics, SpriteBatch spriteBatch)
    {
        _graphics = graphics;
        _spriteBatch = spriteBatch;
        CreateBorderTexture();
        UpdateViewports();
    }

    private void CreateBorderTexture()
    {
        _borderTexture = new Texture2D(_graphics.GraphicsDevice, 1, 1);
        _borderTexture.SetData(new[] { Color.White }); // Solid color for border
    }

    private void UpdateViewports()
    {
        _gameplayViewport = new Viewport
        {
            X = MarginHorizontal + MarginVertical / 2,
            Y = MarginVertical / 2,
            Width = _graphics.PreferredBackBufferWidth - MarginHorizontal - MarginVertical,
            Height = _graphics.PreferredBackBufferHeight - MarginVertical
        };

        _fullScreenViewport = new Viewport
        {
            X = 0,
            Y = 0,
            Width = _graphics.PreferredBackBufferWidth,
            Height = _graphics.PreferredBackBufferHeight
        };
    }

    public void SetGameplayViewport(GraphicsDevice graphicsDevice)
    {
        graphicsDevice.Viewport = _gameplayViewport;
    }

    public void SetFullScreenViewport(GraphicsDevice graphicsDevice)
    {
        graphicsDevice.Viewport = _fullScreenViewport;
    }

    public Viewport GetGameplayViewport()
    {
        return _gameplayViewport;
    }

    public void DrawBorders()
    {
        _spriteBatch.Begin();

        // Draw border around the gameplay viewport
        Rectangle borderRect = new Rectangle(_gameplayViewport.X - BorderWidth, _gameplayViewport.Y - BorderWidth,
                                             _gameplayViewport.Width + 2 * BorderWidth, BorderWidth);
        _spriteBatch.Draw(_borderTexture, borderRect, Color.White);
        _spriteBatch.Draw(_borderTexture, new Rectangle(borderRect.X, _gameplayViewport.Y + _gameplayViewport.Height,
                                                        borderRect.Width, BorderWidth), Color.White);
        _spriteBatch.Draw(_borderTexture, new Rectangle(borderRect.X, borderRect.Y, BorderWidth,
                                                        _gameplayViewport.Height + 2 * BorderWidth), Color.White);
        _spriteBatch.Draw(_borderTexture, new Rectangle(_gameplayViewport.X + _gameplayViewport.Width, borderRect.Y,
                                                        BorderWidth, _gameplayViewport.Height + 2 * BorderWidth), Color.White);

        _spriteBatch.End();
    }
}
