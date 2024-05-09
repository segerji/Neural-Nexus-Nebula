using DirtBox.Entities.Orbs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DirtBox
{
    public class Game1 : Game
    {
        private readonly GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private PlayerOrb _playerOrb;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        //protected override void Initialize()
        //{
        //    base.Initialize();
        //}

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            var orbTexture = Content.Load<Texture2D>("Orb_11");  // Ensure "Orb_11" is the correct texture name
            _playerOrb = new PlayerOrb(orbTexture, new Vector2(_graphics.PreferredBackBufferWidth / 2f, _graphics.PreferredBackBufferHeight / 2f), 200f);  // Position (100, 100) and speed 200
        }

        protected override void Update(GameTime gameTime)
        {
            _playerOrb.Update(gameTime);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            _spriteBatch.Begin();
            _playerOrb.Draw(_spriteBatch);
            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
