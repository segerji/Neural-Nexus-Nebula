using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NNN.Entities;
using NNN.Entities.Events;

namespace NNN.Systems;

public class GameObjectManager
{
    private readonly CollisionManager _collisionManager;
    private readonly List<IEntity> _entities = new();

    public GameObjectManager(EventBus eventBus, Rectangle bounds)
    {
        _collisionManager = new CollisionManager(bounds, ref _entities);
        eventBus.Subscribe<ObjectDestroyedEvent>(HandleEntityDestroyed);
    }

    public void AddEntity(IEntity entity)
    {
        _entities.Add(entity);
    }

    private void HandleEntityDestroyed(ObjectDestroyedEvent eventMessage)
    {
        RemoveEntity(eventMessage.DestroyedObject);
    }

    public void RemoveEntity(IEntity entity)
    {
        _entities.Remove(entity);
    }

    public void Update(GameTime gameTime)
    {
        foreach (var entity in _entities) entity.Update(gameTime);
        _collisionManager.Update(gameTime);
        _collisionManager.CheckCollisions();
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        foreach (var entity in _entities) entity.Draw(spriteBatch);
    }
}