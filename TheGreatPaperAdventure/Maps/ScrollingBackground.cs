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
using System.Diagnostics;
using TGPA.Utils;
using TGPA.Game.Graphics;

namespace TGPA.Maps
{
    /// <summary>
    /// The Direction to scroll the background images
    /// </summary>
    public enum ScrollDirection
    {
        Left,
        Right,
        Down,
        Up
    }

    /// <summary>
    /// This class come from a tutorial :
    /// http://www.xnadevelopment.com/tutorials/scrollinga2dbackground/ScrollingA2DBackground.shtml
    /// I modified some elements to fit with my project, but, anyway, thanks a lot to the original author =)
    /// </summary>
    public class ScrollingBackground
    {
        public static ScrollDirection String2ScrollDirection(String scroll)
        {
            if (scroll.Equals(ScrollDirection.Left.ToString())) return ScrollDirection.Left;
            if (scroll.Equals(ScrollDirection.Right.ToString())) return ScrollDirection.Right;
            if (scroll.Equals(ScrollDirection.Down.ToString())) return ScrollDirection.Down;
            if (scroll.Equals(ScrollDirection.Up.ToString())) return ScrollDirection.Up;

            throw new Exception("Invalid scroll direction : "+scroll);
        }

        //The Sprites that make up the images to be scrolled
        //across the screen.
        private volatile List<BackgroundPart> mBackgroundSprites;
        public List<BackgroundPart> BackgroundSprites
        {
            get { return mBackgroundSprites; }
        }

        //The Sprite at the right end of the chain
        BackgroundPart mFirstImage;
        //The Sprite at the left end of the chain
        BackgroundPart mLastImage;

        //Direction for scrolling
        private ScrollDirection direction;
        public ScrollDirection Direction
        {
            get { return direction; }
            set { direction = value; }
        }

        private Vector2 basicSpeed;
        private Vector2 theSpeed;
        public Vector2 Speed
        {
            get { return theSpeed; }
            set
            {
                ResetSpeed();
                basicSpeed = theSpeed;
                theSpeed = value;
            }
        }

        private bool infinite;
        public bool Infinite
        {
            get { return infinite; }
            set { infinite = value; }
        }

        private Vector2 scroll;
        public Vector2 Scroll
        {
            get { return scroll; }
            set { scroll = value; }
        }

        public ScrollingBackground(ScrollDirection _direction, Vector2 _speed, bool _infinite)
        {
            mBackgroundSprites = new List<BackgroundPart>();
            mFirstImage = null;
            mLastImage = null;
            direction = _direction;

            theSpeed = _speed;
            basicSpeed = theSpeed;

            infinite = _infinite;
        }

        public void LoadContent(ContentManager theContentManager)
        {
            //Clear the Sprites currently stored as the left and right ends of the chain
            mFirstImage = null;
            mLastImage = null;

            //The total width of all the sprites in the chain
            float aWidth = 0;
            //The total height of all the sprites in the chain
            float aHeight = 0;

            //Cycle through all of the Background sprites that have been added
            //and load their content and position them.
            foreach (BackgroundPart aBackgroundSprite in mBackgroundSprites)
            {
                Logger.Log(LogLevel.Info, "Loading Asset : " + aBackgroundSprite.AssetName);

                //Load the sprite's content and apply it's scale, the scale is calculate by figuring
                //out how far the sprite needs to be stretech to make it fill the height of the viewport
                aBackgroundSprite.LoadContent(theContentManager, aBackgroundSprite.AssetName);
                //aBackgroundSprite.Scale = mViewport.Height / aBackgroundSprite.Size.Height;

                //If the Background sprite is the first in line, then mLastInLine will be null.
                if (mFirstImage == null)
                {
                    //Position the first Background sprite in line at the (0,0) position
                    aBackgroundSprite.Position = Vector2.Zero;
                    mLastImage = aBackgroundSprite;
                }
                else
                {
                    //Position the sprite after the last sprite in line
                    if (direction == ScrollDirection.Left)
                        aBackgroundSprite.Position = new Vector2(mFirstImage.Position.X + mFirstImage.Size.Width, 0);

                    else if (direction == ScrollDirection.Right)
                        aBackgroundSprite.Position = new Vector2(mFirstImage.Position.X - mFirstImage.Size.Width, 0);

                    else if (direction == ScrollDirection.Up)
                        aBackgroundSprite.Position = new Vector2(0, mFirstImage.Position.Y - mFirstImage.Size.Height);

                    else if (direction == ScrollDirection.Down)
                        aBackgroundSprite.Position = new Vector2(0, mFirstImage.Position.Y + mFirstImage.Size.Height);
                }

                //Set the sprite as the last one in line
                mFirstImage = aBackgroundSprite;

                //Increment the width of all the sprites combined in the chain
                aWidth += aBackgroundSprite.Size.Width;
                aHeight += aBackgroundSprite.Size.Height;
            }

            //If the Width of all the sprites in the chain does not fill the twice the Viewport width
            //then we need to cycle through the images over and over until we have added
            //enough background images to fill the twice the width. 
            int aIndex = 0;
            if (mBackgroundSprites.Count > 0 && aWidth < TGPAContext.Instance.ScreenWidth * 2)
            {
                do
                {
                    //Add another background image to the chain
                    BackgroundPart aBackgroundSprite = new BackgroundPart();
                    aBackgroundSprite.AssetName = mBackgroundSprites[aIndex].AssetName;
                    aBackgroundSprite.LoadContent(theContentManager, aBackgroundSprite.AssetName);
                    //aBackgroundSprite.Scale = mViewport.Height / aBackgroundSprite.Size.Height;
                    aBackgroundSprite.Position = new Vector2(mFirstImage.Position.X + mFirstImage.Size.Width, 0);

                    mBackgroundSprites.Add(aBackgroundSprite);
                    mFirstImage = aBackgroundSprite;

                    //Add the new background Image's width to the total width of the chain
                    aWidth += aBackgroundSprite.Size.Width;

                    //Move to the next image in the background images
                    //If we've moved to the end of the indexes, start over
                    aIndex += 1;
                    if (aIndex > mBackgroundSprites.Count - 1)
                    {
                        aIndex = 0;
                    }

                } while (aWidth < TGPAContext.Instance.ScreenWidth * 2);
            }

            //Same for Height
            aIndex = 0;
            if (mBackgroundSprites.Count > 0 && aHeight < TGPAContext.Instance.ScreenHeight * 2)
            {
                do
                {
                    //Add another background image to the chain
                    BackgroundPart aBackgroundSprite = new BackgroundPart();
                    aBackgroundSprite.AssetName = mBackgroundSprites[aIndex].AssetName;
                    aBackgroundSprite.LoadContent(theContentManager, aBackgroundSprite.AssetName);
                    //aBackgroundSprite.Scale = mViewport.Width / aBackgroundSprite.Size.Width;

                    if (direction == ScrollDirection.Up)
                    {
                        aBackgroundSprite.Position = new Vector2(0, mFirstImage.Position.Y - mFirstImage.Size.Height);
                    }
                    else
                    {
                        aBackgroundSprite.Position = new Vector2(0, mFirstImage.Position.Y + mFirstImage.Size.Height);
                    }
                    mBackgroundSprites.Add(aBackgroundSprite);
                    mFirstImage = aBackgroundSprite;

                    //Add the new background Image's width to the total width of the chain
                    aHeight += aBackgroundSprite.Size.Height;

                    //Move to the next image in the background images
                    //If we've moved to the end of the indexes, start over
                    aIndex += 1;
                    if (aIndex > mBackgroundSprites.Count - 1)
                    {
                        aIndex = 0;
                    }

                } while (aHeight < TGPAContext.Instance.ScreenHeight * 2);
            }
        }

        /// <summary>
        /// Adds a background sprite to be scrolled through the screen
        /// </summary>
        /// <param name="theAssetName"></param>
        public void AddBackground(string theAssetName)
        {
            BackgroundPart aBackgroundSprite = new BackgroundPart();
            aBackgroundSprite.AssetName = theAssetName;
            mBackgroundSprites.Add(aBackgroundSprite);
        }

        public void AddBackground(string theAssetName, Vector2 size)
        {
            BackgroundPart aBackgroundSprite = new BackgroundPart();
            aBackgroundSprite.AssetName = theAssetName;
            aBackgroundSprite.Size = new Rectangle(0, 0, (int)size.X, (int)size.Y);
            mBackgroundSprites.Add(aBackgroundSprite);
        }

        public void Update(GameTime theGameTime, Vector2 scroll)
        {
            if (direction == ScrollDirection.Left)
            {
                //Check to see if any of the Background sprites have moved off the screen
                //if they have, then move them to the right of the chain of scrolling backgrounds
                foreach (BackgroundPart aBackgroundSprite in mBackgroundSprites)
                {
                    if (aBackgroundSprite.Position.X < - aBackgroundSprite.Size.Width && infinite)
                    {
                        aBackgroundSprite.Position = new Vector2(mFirstImage.Position.X + mFirstImage.Size.Width, 0);
                        mFirstImage = aBackgroundSprite;
                    }
                }
            }
            else if (direction == ScrollDirection.Right)
            {
                //Check to see if any of the background images have moved off the screen
                //if they have, then move them to the left of the chain of scrolling backgrounds
                foreach (BackgroundPart aBackgroundSprite in mBackgroundSprites)
                {
                    if (aBackgroundSprite.Position.X > TGPAContext.Instance.ScreenWidth)
                    {
                        aBackgroundSprite.Position = new Vector2(mLastImage.Position.X - mLastImage.Size.Width, 0);
                        mLastImage = aBackgroundSprite;
                    }
                }
            }

            else if (direction == ScrollDirection.Down)
            {
                //Check to see if any of the Background sprites have moved off the screen
                //if they have, then move them to the right of the chain of scrolling backgrounds
                foreach (BackgroundPart aBackgroundSprite in mBackgroundSprites)
                {
                    if (aBackgroundSprite.Position.Y < - aBackgroundSprite.Size.Height && infinite)
                    {
                        aBackgroundSprite.Position = new Vector2(0, mFirstImage.Position.Y + mFirstImage.Size.Height);
                        mFirstImage = aBackgroundSprite;
                    }
                }
            }
            else if (direction == ScrollDirection.Up)
            {
                //Check to see if any of the background images have moved off the screen
                //if they have, then move them to the left of the chain of scrolling backgrounds
                foreach (BackgroundPart aBackgroundSprite in mBackgroundSprites)
                {
                    if (aBackgroundSprite.Position.Y > TGPAContext.Instance.ScreenHeight)
                    {
                        aBackgroundSprite.Position = new Vector2(0, mFirstImage.Position.Y - mFirstImage.Size.Height);
                        mFirstImage = aBackgroundSprite;
                    }
                }
            }
            //Set the Direction based on movement to the left or right that was passed in
            Scroll += scroll;

            //Update the position of each of the Background sprites
            foreach (BackgroundPart aBackgroundSprite in mBackgroundSprites)
            {
                aBackgroundSprite.Update(theGameTime, theSpeed, DirectionToVector2(direction), scroll);
            }
        }

        /// <summary>
        /// Update the position of the background images
        /// </summary>
        /// <param name="theGameTime"></param>
        public void Update(GameTime theGameTime)
        {
            this.Update(theGameTime, BackgroundPart.CalculateScroll(theGameTime, theSpeed, DirectionToVector2(direction)));
        }

        /// <summary>
        /// Draw the background images to the screen
        /// </summary>
        /// <param name="theSpriteBatch"></param>
        public void Draw(TGPASpriteBatch theSpriteBatch)
        {
            foreach (BackgroundPart aBackgroundSprite in mBackgroundSprites)
            {
                aBackgroundSprite.Draw(theSpriteBatch);
            }
        }

        public void ResetSpeed()
        {
            theSpeed = basicSpeed;
        }

        /// <summary>
        /// Draw the sprite on the screen with specified dimensions (used in map editor)
        /// </summary>
        /// <param name="theSpriteBatch"></param>
        /// <param name="screenWidth"></param>
        /// <param name="screenHeight"></param>
        public void Draw(TGPASpriteBatch theSpriteBatch, int screenWidth, int screenHeight)
        {
            foreach (BackgroundPart aBackgroundSprite in mBackgroundSprites)
            {
                aBackgroundSprite.Draw(theSpriteBatch, screenWidth, screenHeight);
            }
        }

        /// <summary>
        ///Set the Direction based on movement to the left or right that was passed in
        /// </summary>
        /// <param name="direction"></param>
        /// <returns></returns>
        public static Vector2 DirectionToVector2(ScrollDirection direction)
        {
            Vector2 aDirection = Vector2.Zero;

            if (direction == ScrollDirection.Left)
            {
                aDirection.X = -1;
            }
            else if (direction == ScrollDirection.Right)
            {
                aDirection.X = 1;
            }
            else if (direction == ScrollDirection.Down)
            {
                aDirection.Y = -1;
            }
            else if (direction == ScrollDirection.Up)
            {
                aDirection.Y = 1;
            }

            return aDirection;
        }
    }
}
