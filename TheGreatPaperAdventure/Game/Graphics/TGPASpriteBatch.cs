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
using TGPA.SpecialEffects;

namespace TGPA.Game.Graphics
{
    /// <summary>
    /// Proxy SpriteBatch for TGPA, managing automatically resolution issues
    /// </summary>
    public class TGPASpriteBatch
    {
        /// <summary>
        /// The XNA spritebatch
        /// </summary>
        private SpriteBatch spriteBatch;

        /// <summary>
        /// Create a new custom spritebatch using proxy pattern
        /// </summary>
        /// <param name="spriteBatch"></param>
        public TGPASpriteBatch(SpriteBatch spriteBatch)
        {
            this.spriteBatch = spriteBatch;
        }

        //********************************** BEGIN/END/CLEAR PROXIES ****************************

        public void Begin()
        {
            this.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
        }

        public void Begin(SpriteSortMode spriteSortMode, BlendState blendState)
        {
            this.spriteBatch.Begin(spriteSortMode, blendState, null, null, null, null, Resolution.ResolutionMatrix);
        }

        public void End()
        {
            this.spriteBatch.End();
        }

        /// <summary>
        /// Clear all the drawable zone
        /// </summary>
        /// <param name="color"></param>
        public void ClearDevice(Color color)
        {
            this.spriteBatch.GraphicsDevice.Clear(color);
        }

        /// <summary>
        /// Clear only the viewport zone
        /// </summary>
        /// <param name="color"></param>
        public void ClearViewport(Color color)
        {
            this.spriteBatch.Begin();
            this.spriteBatch.Draw(  TGPAContext.Instance.NullTex,
                                    new Rectangle(0, 0, spriteBatch.GraphicsDevice.Viewport.Width, spriteBatch.GraphicsDevice.Viewport.Height), 
                                    color
                                    );
            this.spriteBatch.End();
        }

        //********************************** DRAW PROXIES ****************************

        public void Draw(Texture2D texture, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, SpriteEffects effects, float layerDepth)
        {
            this.spriteBatch.Draw(texture, destinationRectangle, sourceRectangle, color, rotation, origin, effects, layerDepth);
        }

        public void Draw(Texture2D texture, Vector2 position, Rectangle sourceRectangle, Color color, float rotation, Vector2 origin, Vector2 scale, SpriteEffects spriteEffects, float layerDepth)
        {
            this.spriteBatch.Draw(texture, position, sourceRectangle, color, rotation, origin, scale, spriteEffects, layerDepth);
        }

        public void Draw(Texture2D texture, Vector2 position, Rectangle sourceRectangle, Color color, float rotation, Vector2 origin, float scale, SpriteEffects spriteEffects, float layerDepth)
        {
            this.spriteBatch.Draw(texture, position, sourceRectangle, color, rotation, origin, scale, spriteEffects, layerDepth);
        }

        public void Draw(Texture2D texture, Rectangle destinationRectangle, Rectangle sourceRectangle, Color color, float rotation, Vector2 origin, SpriteEffects spriteEffects, float layerDepth)
        {
            this.spriteBatch.Draw(texture, destinationRectangle, sourceRectangle, color, rotation, origin, spriteEffects, layerDepth);
        }

        public void Draw(Texture2D texture, Rectangle destinationRectangle, Rectangle sourceRectangle, Color color)
        {
            this.spriteBatch.Draw(texture, destinationRectangle, sourceRectangle, color);
        }

        public void Draw(Texture2D texture, Rectangle destinationRectangle, Color color)
        {
            this.spriteBatch.Draw(texture, destinationRectangle, color);
        }

        public void Draw(Texture2D texture, Vector2 position, Rectangle destinationRectangle, Color color)
        {
            this.spriteBatch.Draw(texture, position, destinationRectangle, color);
        }

        public void Draw(Texture2D texture, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color color)
        {
            this.spriteBatch.Draw(texture, destinationRectangle, sourceRectangle, color);
        }

        public void DrawString(SpriteFont spriteFont, string text, Vector2 position, Color color, float rotation, Vector2 origin, float scale, SpriteEffects spriteEffects, float layerDepth)
        {
            this.spriteBatch.DrawString(spriteFont, text, position, color, rotation, origin, scale, spriteEffects, layerDepth);
        }
    }
}
