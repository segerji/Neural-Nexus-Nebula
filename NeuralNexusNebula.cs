using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Myra;
using NNN.Entities.Events;
using NNN.Entities.Orbs;
using NNN.Systems;
using NNN.Systems.Services;
using NNN.UI;
using System.Diagnostics;
using System.Linq;

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
    private Texture2D _alienOrbTexture;

    private GeneticAlgorithm _geneticAlgorithm;
    private int _generation;
    private int _generationLength = 300; // Number of frames per generation
    private int _frameCount;

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
        var drawingService = new DrawingService(GraphicsDevice);
        Services.AddService<IDrawingService>(drawingService);
        Debug.WriteLine("DrawingService added.");

        _spriteBatch = new SpriteBatch(GraphicsDevice);
        MyraEnvironment.Game = this;
        _viewportManager = new ViewportManager(_graphics, _spriteBatch);
        _eventBus = new EventBus();
        _gameObjectManager = new GameObjectManager(_eventBus, MovementBounds);
        _backgroundManager = new BackgroundManager(Content);
        _gameUI = new GameUI(_eventBus);
        _eventBus.Subscribe<KnowledgeOrbSpawnEvent>(OnKnowledgeOrbSpawn);
        _eventBus.Subscribe<AlienOrbSpawnEvent>(OnAlienOrbSpawn);
        base.Initialize();


        _geneticAlgorithm = new GeneticAlgorithm(0.03f, drawingService, MovementBounds, _eventBus, _alienOrbTexture);
        SpawnInitialPopulation();

        _generation = 1;
        _frameCount = 0;
    }

    private void SpawnInitialPopulation()
    {
        for (int i = 0; i < 24; i++)
        {
            _eventBus.Publish(new AlienOrbSpawnEvent());
        }

        for (int i = 0; i < 200; i++)
        {
            _eventBus.Publish(new KnowledgeOrbSpawnEvent());
        }

    }

    private void OnAlienOrbSpawn(AlienOrbSpawnEvent obj)
    {
        var drawingService = (IDrawingService)Services.GetService(typeof(IDrawingService));
        var alienOrb = new AlienOrb(drawingService, MovementBounds, _eventBus);
        alienOrb.Initialize(_alienOrbTexture, null, 10);
        _gameObjectManager.AddEntity(alienOrb);
    }

    private void OnKnowledgeOrbSpawn(KnowledgeOrbSpawnEvent e)
    {
        var knowledgeOrb = new KnowledgeOrb(MovementBounds, _eventBus);
        knowledgeOrb.Initialize(_knowledgeOrbTexture, null);
        _gameObjectManager.AddEntity(knowledgeOrb);
    }

    protected override void LoadContent()
    {
        var drawingService = (IDrawingService)Services.GetService(typeof(IDrawingService));
        Debug.WriteLine("DrawingService read.");
        _alienOrbTexture = Content.Load<Texture2D>("Textures/Orb_11");
        _knowledgeOrbTexture = Content.Load<Texture2D>("Textures/Orb_10");

        var playerOrb = new PlayerOrb(drawingService, MovementBounds, _eventBus);
        playerOrb.Initialize(_alienOrbTexture, new Vector2(MovementBounds.Width / 2f, MovementBounds.Height / 2f), 10f);
        _gameObjectManager.AddEntity(playerOrb);
    }

    protected override void Update(GameTime gameTime)
    {
        _backgroundManager.Update(gameTime);
        _gameObjectManager.Update(gameTime);
        base.Update(gameTime);
        
        _frameCount++;
        if (_frameCount >= _generationLength)
        {
            var alienOrbs = _gameObjectManager.Entities.OfType<AlienOrb>().ToList();
            foreach (var alienOrb in alienOrbs)
            {
                _gameObjectManager.RemoveEntity(alienOrb);
            }

            var knowledgeOrbs = _gameObjectManager.Entities.OfType<KnowledgeOrb>().ToList();
            foreach (var knowledgeOrb in knowledgeOrbs)
            {
                _gameObjectManager.RemoveEntity(knowledgeOrb);
            }

            for (int i = 0; i < 200; i++)
            {
                _eventBus.Publish(new KnowledgeOrbSpawnEvent());
            }

            var newAlienOrbs = _geneticAlgorithm.Evolve(alienOrbs);
            foreach (var alienOrb in newAlienOrbs)
            {
                _gameObjectManager.AddEntity(alienOrb);
            }
            _generation++;
            _generationLength += 1;
            _frameCount = 0;
        }
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
