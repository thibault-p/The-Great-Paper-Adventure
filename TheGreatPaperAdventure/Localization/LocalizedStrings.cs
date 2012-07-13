//****************************************************************************************************
//                             The Great paper Adventure : 2009-2011
//                                   By The Great Paper Team
//©2009-2011 Valryon. All rights reserved. This may not be sold or distributed without permission.
//****************************************************************************************************
using System;
using System.Collections.Generic;
using TGPA.Utils;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using System.Xml.Linq;
using TGPA.Localization;
using System.Globalization;

namespace TGPA
{
    /// <summary>
    /// Languages management. Use the standard .NET API.
    /// </summary>
    public class LocalizedStrings
    {
        /// <summary>
        /// Get a string
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static String GetString(String stringID)
        {
            String s = TGPAStrings.ResourceManager.GetString(stringID, TGPAStrings.Culture);
            return s ?? stringID;
        }
    }
}
