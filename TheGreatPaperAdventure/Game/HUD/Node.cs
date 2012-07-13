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
using TGPA;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using TGPA.Game.Graphics;

namespace MapEditor
{
    /// <summary>
    /// Point of a move pattern that can be created and moved
    /// </summary>
    public class Node : Entity
    {
        protected Point point;
        protected static Texture2D GlobalSprite;
        public static Rectangle sRectLine;

        /// <summary>
        /// Create a new point that can be printed on the screen
        /// </summary>
        /// <param name="_data"></param>
        public Node(Point _data) :
            base(new Vector2(_data.X, _data.Y),
            new Rectangle(35, 9, 17, 17),
            Vector2.Zero,
            Vector2.One,
            0f,
            Entity.InfiniteTimeToLive)
        {
            point = _data;

            sRectLine = new Rectangle(55, 15, 9, 9);
        }

        public override void Update(GameTime gameTime)
        {
            //Update Hitbox
            base.Update(gameTime);
        }

        public override void Draw(TGPASpriteBatch spriteBatch)
        {
            Rectangle dRect = this.SrcRect;
            dRect.X = (int)location.X;
            dRect.Y = (int)location.Y;

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);

            spriteBatch.Draw(Sprite, dRect, sRect, Color.White, (float)rotation, Vector2.Zero, Flip, 1.0f);

            spriteBatch.End();
        }

        public Point Point
        {
            get { return point; }
            set { point = value; }
        }

        /// <summary>
        /// Convert nodes in a move pattern
        /// </summary>
        /// <param name="nodes"></param>
        /// <returns></returns>
        public static MovePattern ConvertToMovePattern(Node[] nodes)
        {
            MovePattern mp = new MovePattern();

            foreach (Node n in nodes)
            {
                mp.AddPoint(n.Point);
            }

            return mp;

        }

        /// <summary>
        /// Convert a move pattern in a list of nodes
        /// </summary>
        /// <param name="mp"></param>
        /// <returns></returns>
        public static List<Node> ConvertToNodeList(MovePattern mp)
        {
            List<Node> list = new List<Node>();

            foreach (Point point in mp.Points)
            {
                list.Add(new Node(point));
            }

            return list;
        }

        #region Content Management

        /// <summary>
        /// To use this class you must load the file nodes.png
        /// </summary>
        /// <param name="cm"></param>
        public static void LoadContent(ContentManager cm)
        {
            GlobalSprite = cm.Load<Texture2D>("gfx/nodes");

        }

        public Texture2D Sprite
        {
            get { return GlobalSprite; }
        }


        #endregion

        /// <summary>
        /// Draw a line between 2 points
        /// </summary>
        /// <param name="sp"></param>
        public static void DrawLine(TGPASpriteBatch sp, Point p1, Point p2)
        {
            sp.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);

            for (int x = 1; x < 20; x++)
            {
                Vector2 tVec = new Vector2(p1.X, p1.Y);
                Vector2 nVec = new Vector2(p2.X, p2.Y);
                Vector2 iVec = (nVec - tVec) * ((float)x / 20.0f) + tVec;

                sp.Draw(GlobalSprite, iVec, sRectLine, Color.White);
            }

            sp.End();
        }
    }
}
