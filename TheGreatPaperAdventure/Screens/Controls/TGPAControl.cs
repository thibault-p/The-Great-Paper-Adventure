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
using Microsoft.Xna.Framework.Content;
using TGPA.Localization;
using TGPA.Game.Graphics;

namespace TGPA.Screens.Controls
{
    /// <summary>
    /// Event type for control value changing
    /// </summary>
    /// <param name="game"></param>
    /// <param name="sender"></param>
    public delegate void ValueChangedEventhandler(object sender);

    /// <summary>
    /// Superclass of all GUI controls
    /// </summary>
    public abstract class TGPAControl
    {
        private static Texture2D sprite;
        
        private bool focus;

        /// <summary>
        /// Event called when cotnrol's value has changed
        /// </summary>
        public event ValueChangedEventhandler ValueChanged;

        public static void LoadContent(ContentManager Content) {
            sprite = Content.Load<Texture2D>(@"gfx/Controls/controlsSprites");
        }

        /// <summary>
        /// Component location
        /// </summary>
        protected Vector2 baseLocation,location;
        /// <summary>
        /// Component string
        /// </summary>
        protected String name, baseName;

        public TGPAControl(String name,Vector2 location)
        {
            this.baseName = name;
            this.name = LocalizedStrings.GetString(this.baseName);
            this.baseLocation = location;
            this.location = this.baseLocation;
            this.focus = false;
        }

        public virtual void Update(GameTime gameTime)
        {
            this.location = this.baseLocation;
            this.name = LocalizedStrings.GetString(this.baseName);

            if (this.Changed)
            {
                this.ValueChanged(this);
                this.Changed = false;
            }
        }

        public abstract void Draw(TGPASpriteBatch spriteBatch);

        /// <summary>
        /// Control has the focus
        /// </summary>
        public virtual bool Focus
        {
            get { return this.focus; }
            set { this.focus = value; } 
        }

        /// <summary>
        /// Global Rectangle
        /// </summary>
        public Rectangle DstRect { get; set; }

        /// <summary>
        /// User changed control value
        /// </summary>
        protected bool Changed { get; set; }

        /// <summary>
        /// Buttons texture
        /// </summary>
        protected Texture2D Sprite
        {
            get { return sprite; }
        }
    }
}
