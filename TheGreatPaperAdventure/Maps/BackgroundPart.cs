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
using Microsoft.Xna.Framework.Content;
using TGPA.Game.Graphics;

namespace TGPA.Maps
{
    /// <summary>
    /// This class come from a tutorial :
    /// http://www.xnadevelopment.com/tutorials/scrollinga2dbackground/ScrollingA2DBackground.shtml
    /// I modified some elements to fit with my project, but, anyway, thanks a lot to the original author =)
    /// </summary>
    public class BackgroundPart
    {
        //The current position of the Sprite
        public Vector2 Position = new Vector2(0, 0);

        //The texture object used when drawing the sprite
        public Texture2D SpriteTexture { get; set; }

        //The asset name for the Sprite's Texture
        public string AssetName;

        //The Size of the Sprite (with scale applied)
        public Rectangle Size;

        /// <summary>
        /// Load the texture for the sprite using the Content Pipeline
        /// </summary>
        /// <param name="theContentManager"></param>
        /// <param name="theAssetName"></param>
        public void LoadContent(ContentManager theContentManager, string theAssetName)
        {
            SpriteTexture = theContentManager.Load<Texture2D>(@"gfx/Backgrounds/"+theAssetName);
            AssetName = theAssetName;
            Size = new Rectangle(0, 0, (int)(TGPAContext.Instance.ScreenWidth), (int)(TGPAContext.Instance.ScreenHeight));
        }

        /// <summary>
        /// Update the Sprite and change it's position based on the passed in speed, direction and elapsed time.
        /// </summary>
        /// <param name="theGameTime"></param>
        /// <param name="theSpeed"></param>e
        /// <param name="theDirection"></param>
        public void Update(GameTime theGameTime, Vector2 theSpeed, Vector2 theDirection)
        {
            this.Update(theGameTime,theSpeed,theDirection,CalculateScroll(theGameTime, theSpeed, theDirection));
        }

        public void Update(GameTime theGameTime, Vector2 theSpeed, Vector2 theDirection, Vector2 Scroll)
        {
            Position += Scroll;
        }

        /// <summary>
        /// Draw the sprite on the screen
        /// </summary>
        /// <param name="theSpriteBatch"></param>
        public void Draw(TGPASpriteBatch theSpriteBatch)
        {
            Rectangle dst = new Rectangle((int)Position.X, (int)Position.Y, TGPAContext.Instance.ScreenWidth, TGPAContext.Instance.ScreenHeight);
            theSpriteBatch.Draw(SpriteTexture,dst, Color.White);
        }

        /// <summary>
        /// Draw the sprite on the screen with specified dimensions (used in map editor)
        /// </summary>
        /// <param name="theSpriteBatch"></param>
        /// <param name="screenWidth"></param>
        /// <param name="screenHeight"></param>
        public void Draw(TGPASpriteBatch theSpriteBatch, int screenWidth, int screenHeight)
        {
            theSpriteBatch.Draw(SpriteTexture, Position,
                new Rectangle(0, 0, screenWidth, screenHeight),
                Color.White, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0);
        }

        /// <summary>
        /// Caculate how much to scroll depending on the time
        /// </summary>
        /// <param name="theGameTime"></param>
        /// <param name="theSpeed"></param>
        /// <param name="theDirection"></param>
        /// <returns></returns>
        public static Vector2 CalculateScroll(GameTime theGameTime, Vector2 theSpeed, Vector2 theDirection)
        {
            return theDirection * theSpeed * (float)theGameTime.ElapsedGameTime.TotalSeconds;
        }

        /// <summary>
        /// Calculate time elapsed depending on the scroll. ONLY WORK FOR X AXIS FOR NOW !
        /// </summary>
        /// <param name="scroll"></param>
        /// <param name="theSpeed"></param>
        /// <param name="theDirection"></param>
        /// <returns></returns>
        public static float CalculateTime(Vector2 scroll, Vector2 theSpeed, Vector2 theDirection)
        {
            Vector2 a = scroll / (theDirection * theSpeed);

            if (theDirection.X != 0)
                return a.X;

            else
                return 0f;

        }
    }
}
