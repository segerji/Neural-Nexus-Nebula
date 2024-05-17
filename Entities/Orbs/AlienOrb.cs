using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NNN.Systems;
using NNN.Systems.Services;

namespace NNN.Entities.Orbs
{
    public class AlienOrb : BaseOrb
    {
        private readonly IDrawingService _drawingService;
        public static int NumRays = 16;
        private static int BrainInputSize => (NumRays); // Adjust input size for rays

        public AlienOrb(IDrawingService drawingService, Rectangle bounds, EventBus eventBus)
            : base(bounds, eventBus)
        {
            _drawingService = drawingService;
            VisionRange = 100f;
            Brain = new OrbBrain(BrainInputSize, 2, new int[] { 4 });
        }

        public OrbBrain Brain { get; set; }
        public List<RaycastHit> Rays { get; set; } = new();

        public float Speed { get; set; }
        public float VisionRange { get; set; }
        public float KnowledgeScore { get; private set; }

        public void Initialize(Texture2D texture, Vector2? initialPosition, float speed)
        {
            Speed = speed;
            base.Initialize(texture, initialPosition);
        }

        public new void Initialize(Texture2D texture, Vector2? initialPosition)
        {
            Speed = 10f;
            base.Initialize(texture, initialPosition);
        }

        public override void Update(GameTime gameTime)
        {
            try
            {
                var inputs = GatherInputs();
                var output = Brain.Predict(inputs);
                var movementVector = new Vector2(output[0], output[1]);

                if (movementVector.X != 0 || movementVector.Y != 0)
                {
                    movementVector.Normalize();
                }

                base.Update(gameTime, movementVector * Speed);

                KnowledgeScore -= 0.05f;

                PunishForEdges();
            }
            catch (Exception e) { 
                Debug.Write(e.ToString());
            }
        }

        private float[] GatherInputs()
        {
            var inputs = new List<float>();

            //// Check if the AlienOrb is touching a wall
            //inputs.Add(IsTouchingWall() ? 1 : 0);

            //// AlienOrb's position normalized within the movement bounds
            //var normalizedX = Position.X / MovementBounds.Width;
            //var normalizedY = Position.Y / MovementBounds.Height;

            //inputs.Add(normalizedX);
            //inputs.Add(normalizedY);

            //var velocity = new Vector2(Velocity.X, Velocity.Y);
            //if (velocity.X != 0 || velocity.Y != 0)
            //{
            //    velocity.Normalize();
            //}
            //inputs.Add(velocity.X);
            //inputs.Add(velocity.Y);

            // Add the ray distances normalized within vision range
     
            for(int i = 0; i < NumRays; i++)
            {
                try { 
                    var ray = Rays[i];
                    if (ray.Distance < VisionRange) 
                    { 
                        inputs.Add(ray.Distance / VisionRange);
                    }
                    else
                    {
                        inputs.Add(0);
                    }
                    
                }
                catch
                {
                    inputs.Add(0);
                }
            }

            return inputs.ToArray();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            // TODO : show when debugging
            //_drawingService.DrawCircle(spriteBatch, Position, VisionRange, Color.Red);
            foreach (var ray in Rays)
                _drawingService.DrawLine(spriteBatch, Position, ray.Position, new Color(255, 255, 0, 100));
        }

        public override bool Intersects(ICollidable entity)
        {
            if (!base.Intersects(entity))
                return false;

            switch (entity)
            {
                case KnowledgeOrb orb:
                    KnowledgeScore += 3;
                    orb.Destroy();
                    break;
            }

            return true;
        }

        public void ResolveCollisionWithOtherOrb(AlienOrb otherOrb)
        {
            var distance = Vector2.Distance(Position, otherOrb.Position);
            var overlap = Radius + otherOrb.Radius - distance;

            if (overlap > 0)
            {
                //KnowledgeScore -= 0.20f;

                // Calculate the direction of the collision
                var collisionDirection = Vector2.Normalize(Position - otherOrb.Position);

                // Resolve the collision by moving the orbs away from each other
                Position += collisionDirection * overlap / 2;
                otherOrb.Position -= collisionDirection * overlap / 2;

                // Reflect the velocities
                Velocity = Vector2.Reflect(Velocity, collisionDirection);
                otherOrb.Velocity = Vector2.Reflect(otherOrb.Velocity, -collisionDirection);
            }
        }

        private void PunishForEdges()
        {
            if (IsTouchingWall())
            {
                KnowledgeScore -= 0.50f;
            }
        }
        private bool IsTouchingWall()
        {
            return Position.X - Radius <= MovementBounds.Left ||
                   Position.X + Radius >= MovementBounds.Right ||
                   Position.Y - Radius <= MovementBounds.Top ||
                   Position.Y + Radius >= MovementBounds.Bottom;
        }

        public void Reset(OrbBrain newBrain, Texture2D texture)
        {
            Brain = newBrain;
            KnowledgeScore = 0;
            base.Initialize(texture, null);
        }
    }
}
