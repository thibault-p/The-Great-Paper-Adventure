//****************************************************************************************************
//                             The Great paper Adventure : 2009-2011
//                                   By The Great Paper Team
//©2009-2011 Valryon. All rights reserved. This may not be sold or distributed without permission.
//****************************************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using TGPA.Utils;
using TGPA.Audio;
using Microsoft.Xna.Framework.Content;
using TGPA.Game.Graphics;

namespace TGPA.SpecialEffects
{
    /// <summary>
    /// Manage particles ! Draw, update, kill, ...
    /// </summary>
    public class ParticleManager
    {
        public static int MAX_PARTICLES = 1536;

        private Particle[] particles = new Particle[MAX_PARTICLES];
        private Texture2D particuleTex;


        public ParticleManager()
        {

        }

        public void LoadContent(ContentManager Content)
        {
            particuleTex = Content.Load<Texture2D>(@"gfx/particles");
        }

        public void AddParticle(Particle newParticle)
        {
            AddParticle(newParticle, false);
        }

        public void AddParticle(Particle newParticle, bool background)
        {
            for (int i = 0; i < particles.Length; i++)
            {
                if (particles[i] == null)
                {
                    particles[i] = newParticle;
                    particles[i].Background = background;
                    break;
                }
            }
        }

        public void UpdateParticles(float frameTime)
        {
            for (int i = 0; i < particles.Length; i++)
            {
                if (particles[i] != null)
                {
                    particles[i].Update(frameTime, this);
                    if (!particles[i].Exists)
                    {
                        particles[i] = null;
                    }
                }
            }
        }

        public void DrawParticles(TGPASpriteBatch spriteBatch, bool background)
        {
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);

            foreach (Particle p in particles)
            {
                if (p != null)
                {
                    if (!p.Additive && p.Background == background)
                    {
                        p.Draw(spriteBatch, particuleTex);
                    }
                }
            }

            spriteBatch.End();

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive);

            foreach (Particle p in particles)
            {
                if (p != null)
                {
                    if (p.Additive && p.Background == background)
                    {
                        p.Draw(spriteBatch, particuleTex);
                    }
                }
            }

            spriteBatch.End();
        }

        /// <summary>
        /// Create explosion with blast
        /// </summary>
        /// <param name="loc"></param>
        /// <param name="mag"></param>
        public void MakeExplosion(Vector2 loc, float size)
        {
            for (int i = 0; i < 8; i++)
            {
                if (TGPAContext.Instance.Cheatcodes.IsKawaii)
                {
                    AddParticle(new HeartWave(loc, RandomMachine.GetRandomVector2(-100f, 100f, -100f, 100f),
                        RandomMachine.GetRandomFloat(size / 2, size * 2),
                        RandomMachine.GetRandomInt(0, 4)));
                }
                else
                {
                    AddParticle(new Smoke(loc, RandomMachine.GetRandomVector2(-100f, 100f, -100f, 100f),
                        0.40f, .40f, .40f, 1f,
                        RandomMachine.GetRandomFloat(size / 2, size * 2),
                        RandomMachine.GetRandomInt(0, 4)));
                }
            }

            for (int i = 0; i < 8; i++)
            {
                AddParticle(new Fire(loc, RandomMachine.GetRandomVector2(-80f, 80f, -80f, 80f),
                    size, RandomMachine.GetRandomInt(0, 4)));
            }

            //AddParticle(new Shockwave(loc, true, 25f));
            //AddParticle(new Shockwave(loc, false, 10f));

            ////Shake screen
            //QuakeManager.SetQuake(.5f);
            //QuakeManager.SetBlast(1f, loc);
        }

        /// <summary>
        /// Create explosion
        /// </summary>
        /// <param name="loc"></param>
        /// <param name="mag"></param>
        public void MakeExplosionWithoutQuake(Vector2 loc)
        {
            for (int i = 0; i < 4; i++)
            {
                if (TGPAContext.Instance.Cheatcodes.IsKawaii)
                {
                    AddParticle(new HeartWave(loc, RandomMachine.GetRandomVector2(-100f, 100f, -100f, 100f),
                        RandomMachine.GetRandomFloat(1f, 1.5f),
                        RandomMachine.GetRandomInt(0, 4)));
                }
                else
                {
                    AddParticle(new Smoke(loc, RandomMachine.GetRandomVector2(-100f, 100f, -100f, 100f),
                        0.40f, .40f, .40f, 1f,
                        RandomMachine.GetRandomFloat(1f, 1.5f),
                        RandomMachine.GetRandomInt(0, 4)));
                }
            }

            for (int i = 0; i < 4; i++)
            {
                AddParticle(new Fire(loc, RandomMachine.GetRandomVector2(-80f, 80f, -80f, 80f),
                    1f, RandomMachine.GetRandomInt(0, 4)));
            }

            //AddParticle(new Shockwave(loc, true, 25f));
            //AddParticle(new Shockwave(loc, false, 10f))
        }

        /// <summary>
        /// Create customizable explosion
        /// </summary>
        /// <param name="loc"></param>
        /// <param name="mag"></param>
        public void MakeCircularExplosion(Vector2 loc, float size, int precision)
        {
            for (int i = 0; i < precision; i++)
            {
                if (TGPAContext.Instance.Cheatcodes.IsKawaii)
                {
                    AddParticle(new HeartWave(loc, RandomMachine.GetRandomVector2(loc, size),
                        RandomMachine.GetRandomFloat(1f, 1.5f),
                        RandomMachine.GetRandomInt(0, 4)));
                }
                else
                {
                    AddParticle(new Smoke(loc, RandomMachine.GetRandomVector2(loc, size),
                        0.40f, .40f, .40f, 1f,
                        RandomMachine.GetRandomFloat(1f, 1.5f),
                        RandomMachine.GetRandomInt(0, 4)));
                }
            }

            for (int i = 0; i < precision; i++)
            {
                AddParticle(new Fire(loc, RandomMachine.GetRandomVector2(-(size - 20), size - 20, -(size - 20), size),
                    1f, RandomMachine.GetRandomInt(0, 4)));
            }            
        }

        public void MakeFireFlash(Vector2 loc, Vector2 traj, bool smoke)
        {
            for (int i = 0; i < 16; i++)
            {
                AddParticle(new MuzzleFlash(loc + (traj * (float)i) * 0.0001f + RandomMachine.GetRandomVector2(-5f, 5f, -5f, 5f),
                    traj / 5f,
                    (20f - (float)i) * 0.06f,
                    Color.PaleGoldenrod));

                AddParticle(new MuzzleFlash(loc + (traj * (float)i) * 0.0001f + RandomMachine.GetRandomVector2(-1f, 1f, -1f, 1f),
                    traj / 5f,
                    (5f - (float)i) * 0.06f,
                    Color.Orange));
            }

            if (smoke)
            {
                for (int i = 0; i < 4; i++)
                {
                    AddParticle(new Smoke(loc, RandomMachine.GetRandomVector2(-10f, 10f, -10f, 0f),
                        0f, 0f, 0f, 0.25f,
                        RandomMachine.GetRandomFloat(0.25f, 1.0f),
                        RandomMachine.GetRandomInt(0, 4)));
                }
            }
        }

        public void MakeBullet(Vector2 loc, Vector2 traj, bool background)
        {
            AddParticle(new Bullet(loc, traj + RandomMachine.GetRandomVector2(-90f, 90f, -90f, 90f)), background);
        }

        public void MakeBullet(Vector2 loc, Vector2 traj, bool background, Vector4 smokeColor, float smokeSize)
        {
            MakeBullet(loc, traj, background);

            AddParticle(new Smoke(loc, traj,
                    smokeColor.X, smokeColor.Y, smokeColor.Z, smokeColor.W,
                    smokeSize,
                    RandomMachine.GetRandomInt(0, 4)),background);
        }

        /// <summary>
        /// Destroy all particles
        /// </summary>
        public void Clear()
        {
            for (int i = 0; i < particles.Length; i++)
            {
                particles[i] = null;
            }
        }
    }
}
