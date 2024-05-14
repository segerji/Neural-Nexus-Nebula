using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NNN.Entities;
using NNN.Entities.Events;

namespace NNN.Systems;

public class GameObjectManager
{
    private readonly CollisionManager _collisionManager;
    public readonly List<IEntity> Entities = new();

    public GameObjectManager(EventBus eventBus, Rectangle bounds)
    {
        _collisionManager = new CollisionManager(bounds, ref Entities);
        eventBus.Subscribe<ObjectDestroyedEvent>(HandleEntityDestroyed);
    }

    public void AddEntity(IEntity entity)
    {
        Entities.Add(entity);
    }

    private void HandleEntityDestroyed(ObjectDestroyedEvent eventMessage)
    {
        RemoveEntity(eventMessage.DestroyedObject);
    }

    public void RemoveEntity(IEntity entity)
    {
        Entities.Remove(entity);
    }

    public void Update(GameTime gameTime)
    {
        foreach (var entity in Entities) entity.Update(gameTime);
        _collisionManager.Update(gameTime);
        _collisionManager.CheckCollisions();
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        foreach (var entity in Entities) entity.Draw(spriteBatch);
    }
}