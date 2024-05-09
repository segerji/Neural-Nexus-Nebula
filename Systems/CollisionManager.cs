using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using NNN.Entities;
using NNN.Entities.Orbs;

namespace NNN.Systems;

public class CollisionManager
{
    private readonly QuadTree _quadTree;
    private readonly List<IEntity> _entities;

    public CollisionManager(Rectangle bounds, ref List<IEntity> entities)
    {
        _quadTree = new QuadTree(0, bounds);
        _entities = entities;
    }

    public void Update(GameTime gameTime)
    {
        _quadTree.Clear();
        foreach (var entity in _entities.OfType<ICollidable>()) _quadTree.Insert(entity);
    }

    public void CheckCollisions()
    {
        var collidables = _entities.OfType<AlienOrb>().ToList();
        foreach (var entity in collidables)
        {
            var potentialColliders = new List<IEntity>();
            _quadTree.Retrieve(potentialColliders, entity);
            foreach (var collidable in potentialColliders.OfType<ICollidable>()) entity.Intersects(collidable);
        }
    }
}
