﻿using System.Collections.Generic;
using System.Diagnostics;
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
        foreach (var orb in collidables)
        {
            var potentialColliders = new List<IEntity>();
            _quadTree.Retrieve(potentialColliders, orb);

            var nearbyKnowledgeOrbs = potentialColliders.OfType<KnowledgeOrb>()
                .Where(o => Vector2.Distance(o.Position, orb.Position) <= orb.VisionRange)
                .Distinct()
                .OrderBy(o => Vector2.Distance(o.Position, orb.Position))
                .Take(5)
                .ToList();

            var closestAlienOrb = potentialColliders.OfType<AlienOrb>()
                .Where(o => Vector2.Distance(o.Position, orb.Position) <= orb.VisionRange)
                .Distinct()
                .MinBy(o => Vector2.Distance(o.Position, orb.Position));

            orb.VisibleKnowledgeOrbs = new List<KnowledgeOrb>(nearbyKnowledgeOrbs);
            orb.ClosestAlienOrb = closestAlienOrb;

            foreach (var collidable in potentialColliders.OfType<ICollidable>())
            {
                orb.Intersects(collidable);
            }
        }
    }
}
