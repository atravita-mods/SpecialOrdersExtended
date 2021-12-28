﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using StardewValley;

namespace SpecialOrdersExtended
{
    internal class Utilities
    {
        public static List<string> ContextSort(IEnumerable<string> enumerable)
        {
            LocalizedContentManager contextManager = Game1.content;
            string langcode = contextManager.LanguageCodeString(contextManager.GetCurrentLanguage());
            List<string> outputlist = enumerable.ToList();
            outputlist.Sort(StringComparer.Create(new CultureInfo(langcode), true));
            return outputlist;
        }
    }
}
