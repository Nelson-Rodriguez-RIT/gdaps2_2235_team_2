using Moonwalk.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Moonwalk.Classes.Managers;
using Moonwalk.Classes.Helpful_Stuff;
using System.IO;
using System.Runtime.InteropServices;

namespace Moonwalk.Classes.Helpful_Stuff
{
    /// <summary>
    /// Different behaviors for particles
    /// </summary>
    enum ParticleEffects
    {
        None,
        Random
    }

    internal class Particle
    {
        public static List<Particle> Effects = new List<Particle>();

        private static Texture2D image = Loader.LoadTexture("particle");
        private static Random random = new Random();

        private Color color;
        private int duration;
        private ParticleEffects effect;
        private Point position;
        private int maxTimer;
        private int timer;
        Vector2 direction;

        public Particle(int duration, Color color, ParticleEffects effect, Point position, int frequency = 0, int number = 1, int radius = 0) 
        {
            this.duration = duration;
            this.color = color;
            this.effect = effect;
            this.position = position;
            this.direction = new Vector2(0, 0);

            //Timer is how often things happen
            maxTimer = frequency;

            //add more particles if needed
            for (int i = 0; i < number - 1; i++)
            {
                double angle = random.NextDouble() * 360;
                int distance = random.Next(0, radius + 1);

                Effects.Add(new Particle(
                    random.Next(1, duration + 1),
                    color,
                    effect,
                    position + new Point(
                        (int)(Math.Sin(angle) * distance),
                        (int)(Math.Cos(angle) * distance)),
                    frequency)
                    );
            }
        }

        public Particle(int duration, Color color, ParticleEffects effect, Point position, Vector2 direction, int frequency = 0, int number = 1, int radius = 0)
        {
            this.duration = duration;
            this.color = color;
            this.effect = effect;
            this.position = position;
            this.direction = direction;
            this.direction.Normalize();

            //Timer is how often things happen
            maxTimer = frequency;

            //add more particles if needed
            for (int i = 0; i < number - 1; i++)
            {
                double angle = random.NextDouble() * 360;
                int distance = random.Next(0, radius + 1);

                Effects.Add(new Particle(
                    random.Next(1, duration + 1),
                    color,
                    effect,
                    position + new Point(
                        (int)(Math.Sin(angle) * distance),
                        (int)(Math.Cos(angle) * distance)),
                    this.direction,
                    frequency)
                    );
            }
        }

        public void Update(GameTime gt)
        {
            if (duration == 0) 
            {
                Effects.Remove(this);
            }
            if (timer > 0)
            {
                timer--;
            }
            else
            {
                switch (effect)
                {
                    case ParticleEffects.Random:
                        position += new Point(
                            random.Next(-1, 2),
                            random.Next(-1, 2));
                        break;
                }

                timer = maxTimer;
            }

            position += new Point(
                (int)Math.Round(direction.X),
                (int)Math.Round(direction.Y));
            
            
            duration--;
        }

        public void Draw(SpriteBatch batch) 
        {
            Vector2 temp = Camera.RelativePosition
                (position.ToVector2());
            batch.Draw(image, new Rectangle((int)temp.X, (int)temp.Y, 3, 3), color); 
        }

        public override string ToString()
        {
            return $"{position.ToString()}";
        }
    }
}
