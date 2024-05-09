using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Myra;
using NNN.Entities.Events;
using NNN.Entities.Orbs;
using NNN.Systems;
using NNN.UI;

namespace NNN;

public class NeuralNexusNebula : Game
{
    private readonly GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private BackgroundManager _backgroundManager;
    private GameObjectManager _gameObjectManager;
    private ViewportManager _viewportManager;
    private GameUI _gameUI;
    private EventBus _eventBus;
    private Texture2D _knowledgeOrbTexture;

    public NeuralNexusNebula()
    {
        _graphics = new GraphicsDeviceManager(this);
        _graphics.PreferredBackBufferWidth = 1600;
        _graphics.PreferredBackBufferHeight = 900;
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    private Rectangle MovementBounds =>
        new(
            0,
            0,
            _viewportManager.GetGameplayViewport().Width,
            _viewportManager.GetGameplayViewport().Height
        );

    protected override void Initialize()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        MyraEnvironment.Game = this;
        _viewportManager = new ViewportManager(_graphics, _spriteBatch);
        _eventBus = new EventBus();
        _gameObjectManager = new GameObjectManager(_eventBus, MovementBounds);
        _backgroundManager = new BackgroundManager(Content);
        _gameUI = new GameUI(_eventBus);
        _eventBus.Subscribe<KnowledgeOrbSpawnEvent>(OnKnowledgeOrbSpawn);
        base.Initialize();
    }

    private void OnKnowledgeOrbSpawn(KnowledgeOrbSpawnEvent e)
    {
        var knowledgeOrb = new KnowledgeOrb(_knowledgeOrbTexture, null, MovementBounds, _eventBus);
        _gameObjectManager.AddEntity(knowledgeOrb);
    }

    protected override void LoadContent()
    {
        var orbTexture = Content.Load<Texture2D>("Textures/Orb_11");
        _knowledgeOrbTexture = Content.Load<Texture2D>("Textures/Orb_10");

        var playerOrb = new PlayerOrb(
            orbTexture,
            new Vector2(MovementBounds.Width / 2f, MovementBounds.Height / 2f),
            10f,
            MovementBounds,
            _eventBus
        );

        _gameObjectManager.AddEntity(playerOrb);
    }

    protected override void Update(GameTime gameTime)
    {
        _backgroundManager.Update(gameTime);
        _gameObjectManager.Update(gameTime);
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);

        _viewportManager.SetFullScreenViewport(GraphicsDevice);
        _gameUI.Draw(); // Draw UI elements that should be visible across the entire screen
        _viewportManager.DrawBorders();

        _viewportManager.SetGameplayViewport(GraphicsDevice);
        _spriteBatch.Begin();
        _backgroundManager.Draw(_spriteBatch, GraphicsDevice.Viewport);
        _gameObjectManager.Draw(_spriteBatch);
        _spriteBatch.End();

        base.Draw(gameTime);
    }
}
