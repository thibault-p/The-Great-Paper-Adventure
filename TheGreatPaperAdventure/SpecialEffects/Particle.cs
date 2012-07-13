//****************************************************************************************************
//                             The Great paper Adventure : 2009-2011
//                                   By The Great Paper Team
//©2009-2011 Valryon. All rights reserved. This may not be sold or distributed without permission.
//****************************************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TGPA.Game.Graphics;

namespace TGPA.SpecialEffects
{
    /// <summary>
    /// Special effect FTW.
    /// This come from the book XNA 2.0 Games
    /// </summary>
    public class Particle
    {
        protected Vector2 location;
        protected Vector2 trajectory;
        protected float frame;
        protected float r, g, b, a;
        protected float size;
        protected float rotation;
        protected Color basicColor,particuleColor;

        protected int flag;
        protected int owner;

        private bool additive;

        public bool Exists;
        protected bool background;
        public bool Refract;

        public Particle(float r,float g,float b)
        {
            Exists = false;

            this.r = r;
            this.g = g;
            this.b = b;
            this.a = 1f;
            this.basicColor = new Color(r, g, b);
            this.particuleColor = this.basicColor;
        }

        public bool Additive
        {
            get { return additive; }
            set { additive = value; }
        }

        public Vector2 GameLocation
        {
            get { return location; } 
        }

        public bool Background
        {
            get { return background; }
            set { background = value; }
        }

        public virtual void Update(float gameTime,
            ParticleManager pMan)
        {
            location += trajectory * gameTime;
            frame -= gameTime;

            if(frame <0.0f) KillMe();

            this.particuleColor = this.basicColor;

            if (a < 1f)
            {
                this.particuleColor *= a;
            }
        }

        public virtual void KillMe()
        {
            Exists = false;
        }

        public virtual void Draw(TGPASpriteBatch sprite, Texture2D tex)
        {

        }
    }
}
