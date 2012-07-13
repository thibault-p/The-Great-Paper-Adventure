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

namespace TGPA.Game
{
    /// <summary>
    /// Little scores flying
    /// </summary>
    public class FlyingScores : Entity
    {
        private String texte;
        private float alpha;

        public FlyingScores(Vector2 loc, String value) :
            base(loc,Rectangle.Empty,Vector2.Zero,Vector2.One,0.0f,InfiniteTimeToLive)
        {
            this.texte = value;
            ttl = 3000;
            alpha = 0.7f;
        }

        public override void Update(GameTime gt)
        {
            alpha -= 0.01f;
        }

        public override void Draw(TGPASpriteBatch spriteBatch)
        {
            TGPAContext.Instance.TextPrinter.Color = Color.Black *alpha;
            TGPAContext.Instance.TextPrinter.Size = 1.5f;
            TGPAContext.Instance.TextPrinter.Write(spriteBatch,location.X, location.Y, texte, 128);
            TGPAContext.Instance.TextPrinter.Color = Color.Black;
            TGPAContext.Instance.TextPrinter.Size = 1f;
        }
    }
}
