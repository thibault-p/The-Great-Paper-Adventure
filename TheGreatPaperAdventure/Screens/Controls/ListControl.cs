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
using TGPA.Audio;
using TGPA.Localization;
using TGPA.Utils.Input;
using TGPA.Game.Graphics;

namespace TGPA.Screens.Controls
{
    /// <summary>
    /// Visual list with arrows to choose value
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ListControl : TGPAControl
    {
        private static Rectangle arrowSrc = new Rectangle(10, 230, 50, 45);

        private List<ListElement> elements;
        private ListElement focusedElement;
        private int indexList;
        private Rectangle focus, leftArrowDst, rightArrowDst;
        private bool drawLeft, drawRight;
        private Rectangle confirmButton;
        private bool confirmButtonEnable;
        private string confirmButtonlabel;
        private float actionCooldown;

        public ListControl(String name, Vector2 location, List<ListElement> elements, string confirmButtonLabel)
            : this(name, location, elements)
        {
            this.confirmButtonEnable = true;
            this.confirmButtonlabel = confirmButtonLabel;
        }

        public ListControl(String name, Vector2 location, List<ListElement> elements)
            : base(name, location)
        {
            if (elements.Count == 0)
            {
                throw new ArgumentException("No elements in this list !");
            }

            this.elements = elements;
            this.focus = Rectangle.Empty;

            this.indexList = 0;
            this.focusedElement = elements[this.indexList];

            this.drawLeft = true;
            this.drawRight = true;

            this.confirmButtonEnable = false;
            this.actionCooldown = 0f;
        }

        public override void Update(GameTime gameTime)
        {
            bool left = false;
            bool right = false;
            bool apply = false;

            if (actionCooldown > 0f)
            {
                actionCooldown -= (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            }

            if (TGPAContext.Instance.Player1.IsPlayingOnWindows())
            {
                if (leftArrowDst.Intersects(TGPAContext.Instance.MouseDst))
                {
                    this.focus = leftArrowDst;
                    if (TGPAContext.Instance.InputManager.PlayerIsPressingButtonConfirm(TGPAContext.Instance.Player1))
                    {
                        if (actionCooldown <= 0f)
                        {
                            left = true;
                            actionCooldown = 100f;
                        }
                    }
                }
                else if (rightArrowDst.Intersects(TGPAContext.Instance.MouseDst))
                {
                    this.focus = rightArrowDst;
                    if (TGPAContext.Instance.InputManager.PlayerIsPressingButtonConfirm(TGPAContext.Instance.Player1))
                    {
                        if (actionCooldown <= 0f)
                        {
                            right = true;
                            actionCooldown = 100f;
                        }
                    }
                }
                else if (confirmButton.Intersects(TGPAContext.Instance.MouseDst))
                {
                    this.focus = confirmButton;
                    if (TGPAContext.Instance.InputManager.PlayerPressButtonConfirm(TGPAContext.Instance.Player1))
                    {
                        apply = true;
                    }
                }
                else
                {
                    this.focus = Rectangle.Empty;
                }
            }
            else
            {
                if (this.Focus)
                {
                    if (TGPAContext.Instance.InputManager.PlayerPressLeft(TGPAContext.Instance.Player1))
                    {
                        this.focus = leftArrowDst;

                        if (actionCooldown <= 0f)
                        {
                            left = true;
                            actionCooldown = 100f;
                        }
                    }
                    if (TGPAContext.Instance.InputManager.PlayerPressRight(TGPAContext.Instance.Player1))
                    {
                        this.focus = rightArrowDst;

                        if (actionCooldown <= 0f)
                        {
                            right = true;
                            actionCooldown = 100f;
                        }
                    }
                    else if (TGPAContext.Instance.InputManager.PlayerPressButtonConfirm(TGPAContext.Instance.Player1))
                    {
                        this.focus = confirmButton;
                        apply = true;
                    }
                }
            }

            //Update values     
            if (this.Focus)
            {
                if (left)
                {
                    this.PreviousElement();
                    SoundEngine.Instance.PlaySound("listSelectionChanged");
                }
                else if (right)
                {
                    this.NextElement();
                    SoundEngine.Instance.PlaySound("listSelectionChanged");
                }
            }

            this.DstRect = new Rectangle((int)location.X, (int)location.Y, 200 + leftArrowDst.Width + rightArrowDst.Width + this.focusedElement.SrcRect.Width, Math.Max(this.focusedElement.SrcRect.Height, arrowSrc.Height));


            this.focusedElement.Update(gameTime);

            this.drawLeft = true;
            if (this.indexList == 0)
            {
                this.drawLeft = false;
            }

            this.drawRight = true;
            if (this.indexList == this.elements.Count - 1)
            {
                this.drawRight = false;
            }

            if (confirmButtonEnable)
            {
                this.Changed = apply;

                confirmButton = new Rectangle(rightArrowDst.Right + 30,
                                                (int)this.location.Y - 10,
                                                50 + (int)(11f * TGPAContext.Instance.TextPrinter.Size) * LocalizedStrings.GetString(this.confirmButtonlabel).Length,
                                                55);
                Rectangle dRect = this.DstRect;
                dRect.Width += 95;
                this.DstRect = dRect;
            }

            base.Update(gameTime);
        }

        public override void Draw(TGPASpriteBatch spriteBatch)
        {
            Rectangle src = new Rectangle();
            Rectangle dst = new Rectangle();

            Color c = Color.White;

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);

            if (Focus)
            {
                //Draw left arrow
                //************************************************************

                if (!drawLeft)
                {
                    c = (Color.Gray * 0.3f);
                }

                leftArrowDst = arrowSrc;
                leftArrowDst.X = (int)location.X + 200;
                leftArrowDst.Y = (int)location.Y;

                src = arrowSrc;
                if (focus == leftArrowDst)
                {
                    src.X += src.Width;
                }
                spriteBatch.Draw(this.Sprite, leftArrowDst, src, c);
            }
            spriteBatch.End();

            //Draw element
            //************************************************************
            dst = this.focusedElement.SrcRect;
            dst.X = (int)location.X + 200 + arrowSrc.Width;
            dst.Y = (int)location.Y;

            src = this.focusedElement.SrcRect;
            this.focusedElement.Draw(dst, spriteBatch);

            if (Focus)
            {

                //Draw right arrow
                //************************************************************
                c = Color.White;
                if (!drawRight)
                {
                    c = Color.Gray * 0.3f;
                }

                rightArrowDst = arrowSrc;
                rightArrowDst.X = (int)location.X + 200 + arrowSrc.Width + this.focusedElement.SrcRect.Width;
                rightArrowDst.Y = (int)location.Y;

                src = arrowSrc;
                if (focus == rightArrowDst)
                {
                    src.X += src.Width;
                }
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
                spriteBatch.Draw(this.Sprite, rightArrowDst, src, c, 0.0f, Vector2.Zero, SpriteEffects.FlipHorizontally, 1.0f);

                //************************************************************
                spriteBatch.End();

                if (confirmButtonEnable)
                {

                    spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);

                    if (focus == this.confirmButton)
                    {
                        spriteBatch.Draw(TGPAContext.Instance.HudTex, this.confirmButton, TGPAContext.Instance.PaperRect, Color.Green * 0.5f);

                    }
                    else
                    {
                        spriteBatch.Draw(TGPAContext.Instance.HudTex, this.confirmButton, TGPAContext.Instance.PaperRect, Color.Aquamarine * 0.5f);
                    }

                    spriteBatch.End();

                    TGPAContext.Instance.TextPrinter.Write(spriteBatch, this.confirmButton.X + 15, this.confirmButton.Y + 15, LocalizedStrings.GetString(this.confirmButtonlabel), 512);
                }


                //Draw help buttons for Pad
                if (TGPAContext.Instance.Player1.IsPlayingOnWindows() == false && (Focus))
                {
                    if (confirmButtonEnable)
                    {
                        TGPAContext.Instance.ButtonPrinter.Draw(spriteBatch, "#Confirm", TGPAContext.Instance.Player1.Device.Type, confirmButton.X + confirmButton.Width, this.confirmButton.Y);
                    }
                }
            }

            TGPAContext.Instance.TextPrinter.Color = Color.Navy;
            TGPAContext.Instance.TextPrinter.Write(spriteBatch, this.location.X, this.location.Y, this.name, 512);
            TGPAContext.Instance.TextPrinter.Color = Color.Black;
        }

        /// <summary>
        /// Select the next element in the list
        /// </summary>
        public void NextElement()
        {
            if (this.indexList + 1 < this.elements.Count)
            {
                this.indexList++;
                this.Changed = true;
            }

            if (this.Changed)
            {
                this.focusedElement = elements[this.indexList];
            }
        }

        /// <summary>
        /// Select the previous element in the list
        /// </summary>
        public void PreviousElement()
        {
            if (this.indexList - 1 >= 0)
            {
                this.indexList--;
                this.Changed = true;
            }

            if (this.Changed)
            {
                this.focusedElement = elements[this.indexList];
            }
        }

        public void SetFocusedElementByValue(Object value)
        {
            foreach (ListElement listElement in elements)
            {
                if (listElement.Value.Equals(value))
                {
                    this.focusedElement = listElement;
                    this.indexList = elements.IndexOf(this.focusedElement);
                    break;
                }
            }

            this.Changed = false;
        }

        public List<ListElement> Elements
        {
            get { return this.elements; }
        }

        public ListElement FocusedElement
        {
            set
            {
                if (this.elements.Contains(value))
                {
                    this.focusedElement = value;
                    this.indexList = elements.IndexOf(this.focusedElement);
                }
            }
            get { return this.focusedElement; }
        }
    }
}
