using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using NNN.Entities;
using NNN.Entities.Orbs;

namespace NNN.Systems
{
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
            var destroyedEntities = new HashSet<IEntity>();

            foreach (var orb in collidables)
            {
                if (destroyedEntities.Contains(orb)) continue;

                var potentialColliders = new List<IEntity>();
                _quadTree.Retrieve(potentialColliders, orb);

                var activeColliders = potentialColliders.Where(e => !e.IsDisposed).ToList(); // Ensure only active entities are checked

                foreach (var collidable in activeColliders.OfType<KnowledgeOrb>())
                {
                    if (orb.Intersects(collidable))
                    {
                        destroyedEntities.Add(collidable);
                        break;
                    }
                }

                //foreach (var otherOrb in activeColliders.OfType<AlienOrb>())
                //{
                //    if (orb != otherOrb)
                //    {
                //        orb.ResolveCollisionWithOtherOrb(otherOrb);
                //    }
                //}

                // Perform raycasting
                orb.Rays = CastRays(orb);
            }

            // Remove destroyed entities from the main list
            _entities.RemoveAll(e => destroyedEntities.Contains(e));
        }

        private List<RaycastHit> CastRays(AlienOrb orb)
        {
            var rays = new List<RaycastHit>();
            float angleStep = MathHelper.TwoPi / AlienOrb.NumRays;

            for (int i = 0; i < AlienOrb.NumRays; i++)
            {
                float angle = i * angleStep;
                var direction = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
                rays.Add(CastRay(orb, direction));
            }

            return rays;
        }

        private RaycastHit CastRay(AlienOrb orb, Vector2 direction)
        {
            RaycastHit closestHit = null;
            float closestDistance = orb.VisionRange;

            foreach (var entity in _entities.OfType<ICollidable>().Where(e => e != orb && Vector2.Distance(e.Position, orb.Position) <= orb.VisionRange))
            {
                var hit = RayIntersect(orb, direction, entity);
                if (hit != null && hit.Distance < closestDistance)
                {
                    closestHit = hit;
                    closestDistance = hit.Distance;
                }
            }

            return closestHit ?? new RaycastHit { Position = orb.Position + direction * orb.VisionRange, Distance = orb.VisionRange };
        }

        private RaycastHit RayIntersect(AlienOrb orb, Vector2 direction, ICollidable entity)
        {
            if (entity is KnowledgeOrb otherOrb)
            {
                var toCenter = otherOrb.Position - orb.Position;
                float projLength = Vector2.Dot(toCenter, direction);
                if (projLength < 0 || projLength > orb.VisionRange) return null;

                var closestPoint = orb.Position + direction * projLength;
                var distanceToCenter = Vector2.Distance(closestPoint, otherOrb.Position);

                if (distanceToCenter <= otherOrb.Radius)
                {
                    var hitPoint = closestPoint - direction * (float)Math.Sqrt(otherOrb.Radius * otherOrb.Radius - distanceToCenter * distanceToCenter);
                    return new RaycastHit { Position = hitPoint, Distance = Vector2.Distance(orb.Position, hitPoint) };
                }
            }

            return null;
        }
    }

    public class RaycastHit
    {
        public Vector2 Position { get; set; }
        public float Distance { get; set; }
    }
}
