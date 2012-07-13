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
using MapEditor;
using TGPA.Game.Graphics;

namespace TGPA
{
    /// <summary>
    /// A move pattern is suite of points that allow an enemy (or anything else) to follow a define path to move
    /// </summary>
    public class MovePattern
    {
        /// <summary>
        /// Move pattern identifier
        /// </summary>
        public enum DefinedMovePattern
        {
            Custom,
            Up,
            Down,
            Right,
            Left,
            LeftUpCorner,
            LeftDownCorner,
            RightUpCorner,
            RightDownCorner,
            SmallUpAndDown,
            LeftAndRight,
            BigUpAndDown,
            BigAndFastUpAndDown,
            SmallLace,
            SmallV,
            SmallInverseV,
            V,
            InverseV,
            SmallU,
            U,
            Z
        }

        public static Point DeadPoint = new Point(-9999999, -99999999);

        private List<Point> points;
        private int currentPointIndex;
        private bool initialized;
        private DefinedMovePattern identifer;

        /// <summary>
        /// Create an empty MovePattern (you have to fill it later)
        /// </summary>
        public MovePattern()
        {
            points = new List<Point>();
            initialized = true;
            currentPointIndex = -1;
            identifer = DefinedMovePattern.Custom;
        }

        /// <summary>
        /// Create a new move pattern by ID
        /// </summary>
        public MovePattern(DefinedMovePattern identifer)
            : this()
        {
            this.initialized = false;
            this.identifer = identifer;
        }

        public void Initialize(BadGuy badguy)
        {
            switch (identifer)
            {
                case DefinedMovePattern.Up:
                    AddPoint(new Point((int)badguy.Location.X, -badguy.DstRect.Height));
                    break;
                case DefinedMovePattern.Down:
                    AddPoint(new Point((int)badguy.Location.X, TGPAContext.Instance.ScreenHeight + badguy.DstRect.Height));
                    break;
                case DefinedMovePattern.Right:
                    AddPoint(new Point(TGPAContext.Instance.ScreenWidth + badguy.DstRect.Width, (int)badguy.Location.Y));
                    break;
                case DefinedMovePattern.Left:
                    AddPoint(new Point(-badguy.DstRect.Width, (int)badguy.Location.Y));
                    break;
                case DefinedMovePattern.LeftDownCorner:
                    AddPoint(new Point(-badguy.DstRect.Width, TGPAContext.Instance.ScreenHeight + badguy.DstRect.Height));
                    break;
                case DefinedMovePattern.LeftUpCorner:
                    AddPoint(new Point(-badguy.DstRect.Width, -badguy.DstRect.Height));
                    break;
                case DefinedMovePattern.RightUpCorner:
                    AddPoint(new Point(TGPAContext.Instance.ScreenWidth + badguy.DstRect.Width, -badguy.DstRect.Height));
                    break;
                case DefinedMovePattern.RightDownCorner:
                    AddPoint(new Point(TGPAContext.Instance.ScreenWidth + badguy.DstRect.Width, TGPAContext.Instance.ScreenHeight + badguy.DstRect.Height));
                    break;
                case DefinedMovePattern.SmallUpAndDown:

                    int upOrDown = (badguy.Location.Y > (TGPAContext.Instance.ScreenHeight / 2)) ? 3 : 1;

                    if (badguy.Flip == SpriteEffects.FlipHorizontally)
                    {
                        for (int i = 1; i < 9; i++)
                        {
                            AddPoint(new Point(2 * TGPAContext.Instance.ScreenWidth / 8, upOrDown * TGPAContext.Instance.ScreenHeight / 4));
                            upOrDown = upOrDown == 3 ? 1 : 3;
                        }
                        AddPoint(new Point(TGPAContext.Instance.ScreenWidth + badguy.DstRect.Width, TGPAContext.Instance.ScreenHeight / 2));
                    }
                    else
                    {
                        for (int i = 8; i > 0; i--)
                        {
                            AddPoint(new Point(i * TGPAContext.Instance.ScreenWidth / 8, upOrDown * TGPAContext.Instance.ScreenHeight / 4));
                            upOrDown = upOrDown == 3 ? 1 : 3;
                        }
                        AddPoint(new Point(-badguy.DstRect.Width, TGPAContext.Instance.ScreenHeight / 2));
                    }

                    break;
                case DefinedMovePattern.BigUpAndDown:

                    upOrDown = (badguy.Location.Y > (TGPAContext.Instance.ScreenHeight / 2)) ? 7 : 1;

                    if (badguy.Flip == SpriteEffects.FlipHorizontally)
                    {
                        for (int i = 2; i < 9; i++)
                        {
                            AddPoint(new Point(i * TGPAContext.Instance.ScreenWidth / 8, upOrDown * TGPAContext.Instance.ScreenHeight / 8));
                            upOrDown = upOrDown == 7 ? 1 : 7;
                        }
                        AddPoint(new Point(TGPAContext.Instance.ScreenWidth + badguy.DstRect.Width, TGPAContext.Instance.ScreenHeight / 2));
                    }
                    else if (badguy.Flip == SpriteEffects.None)
                    {
                        for (int i = 8; i > 0; i--)
                        {
                            AddPoint(new Point(i * TGPAContext.Instance.ScreenWidth / 8, upOrDown * TGPAContext.Instance.ScreenHeight / 8));
                            upOrDown = upOrDown == 7 ? 1 : 7;
                        }
                        AddPoint(new Point(-badguy.DstRect.Width, TGPAContext.Instance.ScreenHeight / 2));
                    }

                    break;

                case DefinedMovePattern.BigAndFastUpAndDown:

                    upOrDown = (badguy.Location.Y > (TGPAContext.Instance.ScreenHeight / 2)) ? 7 : 1;

                    if (badguy.Flip == SpriteEffects.FlipHorizontally)
                    {
                        for (int i = 2; i < 9; i++)
                        {
                            AddPoint(new Point(i * TGPAContext.Instance.ScreenWidth / 8, upOrDown * TGPAContext.Instance.ScreenHeight / 8));
                            upOrDown = upOrDown == 7 ? 1 : 7;
                        }
                        AddPoint(new Point(TGPAContext.Instance.ScreenWidth + badguy.DstRect.Width, TGPAContext.Instance.ScreenHeight / 2));
                    }
                    else if (badguy.Flip == SpriteEffects.None)
                    {
                        for (int i = 4; i > 0; i--)
                        {
                            AddPoint(new Point(i * TGPAContext.Instance.ScreenWidth / 4, upOrDown * TGPAContext.Instance.ScreenHeight / 6));
                            upOrDown = upOrDown == 6 ? 2 : 6;
                        }
                        AddPoint(new Point(-badguy.DstRect.Width, TGPAContext.Instance.ScreenHeight / 2));
                    }

                    break;
                case DefinedMovePattern.LeftAndRight:

                    int leftOrRight = (badguy.Location.X > (TGPAContext.Instance.ScreenWidth / 2)) ? 3 : 1;

                    for (int i = 1; i < 9; i++)
                    {
                        AddPoint(new Point(leftOrRight * TGPAContext.Instance.ScreenWidth / 4, i * TGPAContext.Instance.ScreenHeight / 8));
                        leftOrRight = leftOrRight == 3 ? 1 : 3;
                    }
                    AddPoint(new Point(TGPAContext.Instance.ScreenWidth / 2, TGPAContext.Instance.ScreenHeight + badguy.DstRect.Height));

                    break;
                case DefinedMovePattern.SmallLace:
                    if (badguy.Flip == SpriteEffects.FlipHorizontally)
                    {
                        AddPoint(new Point(TGPAContext.Instance.ScreenWidth / 8, TGPAContext.Instance.ScreenHeight / 2));
                        AddPoint(new Point(2 * TGPAContext.Instance.ScreenWidth / 8, TGPAContext.Instance.ScreenHeight / 2));

                        //Small looping
                        AddPoint(new Point(3 * TGPAContext.Instance.ScreenWidth / 8, (TGPAContext.Instance.ScreenHeight / 2) - 150));
                        AddPoint(new Point(5 * TGPAContext.Instance.ScreenWidth / 8, (TGPAContext.Instance.ScreenHeight / 2) - 250));
                        AddPoint(new Point(4 * TGPAContext.Instance.ScreenWidth / 8, (TGPAContext.Instance.ScreenHeight / 2) - 250));
                        AddPoint(new Point(6 * TGPAContext.Instance.ScreenWidth / 8, (TGPAContext.Instance.ScreenHeight / 2) - 150));

                        AddPoint(new Point(7 * TGPAContext.Instance.ScreenWidth / 8, TGPAContext.Instance.ScreenHeight / 2));
                        AddPoint(new Point(TGPAContext.Instance.ScreenWidth + badguy.DstRect.Width, TGPAContext.Instance.ScreenHeight / 2));
                    }
                    else
                    {
                        AddPoint(new Point(7 * TGPAContext.Instance.ScreenWidth / 8, TGPAContext.Instance.ScreenHeight / 2));
                        AddPoint(new Point(6 * TGPAContext.Instance.ScreenWidth / 8, (TGPAContext.Instance.ScreenHeight / 2) - 150));
                        AddPoint(new Point(4 * TGPAContext.Instance.ScreenWidth / 8, (TGPAContext.Instance.ScreenHeight / 2) - 250));
                        AddPoint(new Point(5 * TGPAContext.Instance.ScreenWidth / 8, (TGPAContext.Instance.ScreenHeight / 2) - 250));
                        AddPoint(new Point(3 * TGPAContext.Instance.ScreenWidth / 8, (TGPAContext.Instance.ScreenHeight / 2) - 150));
                        AddPoint(new Point(2 * TGPAContext.Instance.ScreenWidth / 8, TGPAContext.Instance.ScreenHeight / 2));
                        AddPoint(new Point(-badguy.DstRect.Width, TGPAContext.Instance.ScreenHeight / 2));
                    }
                    break;


                case DefinedMovePattern.SmallV:

                    if (badguy.Flip == SpriteEffects.None)
                    {
                        AddPoint(new Point(TGPAContext.Instance.ScreenWidth / 2, badguy.DstRect.Y + TGPAContext.Instance.ScreenHeight / 4));
                        AddPoint(new Point(-badguy.DstRect.Width, badguy.DstRect.Y));
                    }
                    else
                    {
                        AddPoint(new Point(TGPAContext.Instance.ScreenWidth / 2, badguy.DstRect.Y + TGPAContext.Instance.ScreenHeight / 4));
                        AddPoint(new Point(TGPAContext.Instance.ScreenWidth + badguy.DstRect.Width, badguy.DstRect.Y));
                    }

                    break;

                case DefinedMovePattern.SmallInverseV:

                    if (badguy.Flip == SpriteEffects.None)
                    {
                        AddPoint(new Point(TGPAContext.Instance.ScreenWidth / 2, badguy.DstRect.Y - TGPAContext.Instance.ScreenHeight / 4));
                        AddPoint(new Point(-badguy.DstRect.Width, badguy.DstRect.Y));
                    }
                    else
                    {
                        AddPoint(new Point(TGPAContext.Instance.ScreenWidth / 2, badguy.DstRect.Y - TGPAContext.Instance.ScreenHeight / 4));
                        AddPoint(new Point(TGPAContext.Instance.ScreenWidth + badguy.DstRect.Width, badguy.DstRect.Y));
                    }

                    break;

                case DefinedMovePattern.V:

                    if (badguy.Flip == SpriteEffects.None)
                    {
                        AddPoint(new Point(TGPAContext.Instance.ScreenWidth / 2, 7* TGPAContext.Instance.ScreenHeight / 8));
                        AddPoint(new Point(-badguy.DstRect.Width, badguy.DstRect.Y));
                    }
                    else
                    {
                        AddPoint(new Point(TGPAContext.Instance.ScreenWidth / 2, 7 * TGPAContext.Instance.ScreenHeight / 8));
                        AddPoint(new Point(TGPAContext.Instance.ScreenWidth + badguy.DstRect.Width, badguy.DstRect.Y));
                    }

                    break;
                case DefinedMovePattern.InverseV:

                    if (badguy.Flip == SpriteEffects.None)
                    {
                        AddPoint(new Point(TGPAContext.Instance.ScreenWidth / 2, TGPAContext.Instance.ScreenHeight / 8));
                        AddPoint(new Point(-badguy.DstRect.Width, badguy.DstRect.Y));
                    }
                    else
                    {
                        AddPoint(new Point(TGPAContext.Instance.ScreenWidth / 2, TGPAContext.Instance.ScreenHeight / 8));
                        AddPoint(new Point(TGPAContext.Instance.ScreenWidth + badguy.DstRect.Width, badguy.DstRect.Y));
                    }

                    break;
                case DefinedMovePattern.SmallU:

                    if (badguy.Flip == SpriteEffects.None)
                    {
                        AddPoint(new Point(3*TGPAContext.Instance.ScreenWidth / 4, badguy.DstRect.Y + TGPAContext.Instance.ScreenHeight / 4));
                        AddPoint(new Point(TGPAContext.Instance.ScreenWidth / 4, badguy.DstRect.Y + TGPAContext.Instance.ScreenHeight / 4));
                        AddPoint(new Point(-badguy.DstRect.Width, badguy.DstRect.Y));
                    }
                    else
                    {
                        AddPoint(new Point(TGPAContext.Instance.ScreenWidth / 4, badguy.DstRect.Y + TGPAContext.Instance.ScreenHeight / 4));
                        AddPoint(new Point(3 * TGPAContext.Instance.ScreenWidth / 4, badguy.DstRect.Y + TGPAContext.Instance.ScreenHeight / 4));
                        AddPoint(new Point(TGPAContext.Instance.ScreenWidth + badguy.DstRect.Width, badguy.DstRect.Y));
                    }

                    break;
                case DefinedMovePattern.U:
                    
                    if (badguy.Flip == SpriteEffects.None)
                    {
                        AddPoint(new Point(3 * TGPAContext.Instance.ScreenWidth / 4, 7 * TGPAContext.Instance.ScreenHeight / 8));
                        AddPoint(new Point(TGPAContext.Instance.ScreenWidth / 4, 7 * TGPAContext.Instance.ScreenHeight / 8));
                        AddPoint(new Point(-badguy.DstRect.Width, badguy.DstRect.Y));
                    }
                    else
                    {
                        AddPoint(new Point(TGPAContext.Instance.ScreenWidth / 4, 7 * TGPAContext.Instance.ScreenHeight / 8));
                        AddPoint(new Point(3 * TGPAContext.Instance.ScreenWidth / 4, 7 * TGPAContext.Instance.ScreenHeight / 8));
                        AddPoint(new Point(TGPAContext.Instance.ScreenWidth + badguy.DstRect.Width, badguy.DstRect.Y));
                    }

                    break;

                case DefinedMovePattern.Z:
                    if (badguy.Flip == SpriteEffects.None)
                    {
                        AddPoint(new Point(3 * TGPAContext.Instance.ScreenWidth / 4, TGPAContext.Instance.ScreenHeight / 4));
                        AddPoint(new Point(TGPAContext.Instance.ScreenWidth / 4, TGPAContext.Instance.ScreenHeight / 4));
                        AddPoint(new Point(3 * TGPAContext.Instance.ScreenWidth / 4, 3 * TGPAContext.Instance.ScreenHeight / 4));
                        AddPoint(new Point(TGPAContext.Instance.ScreenWidth / 4, 3 * TGPAContext.Instance.ScreenHeight / 4));
                        AddPoint(new Point(0, 3 * TGPAContext.Instance.ScreenHeight / 4));
                        AddPoint(new Point(-badguy.DstRect.Width, 3 * TGPAContext.Instance.ScreenHeight / 4));
                    }
                    else
                    {
                        AddPoint(new Point(TGPAContext.Instance.ScreenWidth / 4, TGPAContext.Instance.ScreenHeight / 4));
                        AddPoint(new Point(3 * TGPAContext.Instance.ScreenWidth / 4, TGPAContext.Instance.ScreenHeight / 4));
                        AddPoint(new Point(TGPAContext.Instance.ScreenWidth / 4, 3 * TGPAContext.Instance.ScreenHeight / 4));
                        AddPoint(new Point(3 * TGPAContext.Instance.ScreenWidth / 4, 3 * TGPAContext.Instance.ScreenHeight / 4));
                        AddPoint(new Point(TGPAContext.Instance.ScreenWidth + badguy.DstRect.Width, 3 * TGPAContext.Instance.ScreenHeight / 4));
                    }
                    break;
                default:
                    throw new Exception("Unimplemented MovePattern : " + identifer.ToString());
            }

            this.initialized = true;
        }

        /// <summary>
        /// Add a point to the move pattern
        /// </summary>
        /// <param name="newPoint"></param>
        public void AddPoint(Point newPoint)
        {
            currentPointIndex = 0;
            points.Add(newPoint);
        }

        public void AddPoint(int x, int y)
        {
            currentPointIndex = 0;
            points.Add(new Point(x, y));
        }

        public Point GetCurrentPoint()
        {
            if (currentPointIndex >= points.Count) return DeadPoint; //HACK
            return points[currentPointIndex];
        }

        public Point GetNextPoint()
        {
            currentPointIndex++;

            //No more point : end of the pattern
            if (currentPointIndex >= points.Count)
            {
                return DeadPoint;
            }

            return GetCurrentPoint();
        }

        /// <summary>
        /// Reset pattern to the first point
        /// </summary>
        public void ResetPattern()
        {
            currentPointIndex = 0;
        }

        public List<Point> Points
        {
            get { return points; }
        }

        public int PointIndex
        {
            set { currentPointIndex = value; }
            get { return currentPointIndex; }
        }

        public bool Initialized
        {
            get { return this.initialized; }
        }

        public override string ToString()
        {
            return this.identifer.ToString();
        }

        /// <summary>
        /// Draw the pattern of the specified enemy
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="badguy"></param>
        public static void DrawMovePattern(TGPASpriteBatch spriteBatch, BadGuy badguy, List<Node> nodes)
        {
            if (badguy.Pattern == null) return;

            for (int i = 0; i < badguy.Pattern.Points.Count; i++)
            {
                bool first = true;
                Point lastPoint = Point.Zero;

                foreach (Node n in nodes)
                {
                    //Draw line between node and player
                    if (first)
                    {
                        first = false;
                        Node.DrawLine(spriteBatch, new Point((int)badguy.Location.X, (int)badguy.Location.Y), n.Point);
                    }
                    else
                    {
                        //Draw lines between 2 points
                        Node.DrawLine(spriteBatch, lastPoint, n.Point);
                    }
                    n.Draw(spriteBatch);
                    lastPoint = n.Point;

                }
            }
        }
    }
}
