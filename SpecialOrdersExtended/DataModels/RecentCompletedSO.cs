﻿using System;
using System.Collections.Generic;
using System.Text;
using StardewModdingAPI;

using System.Linq;
using StardewValley;

namespace SpecialOrdersExtended.DataModels
{
    internal class RecentCompletedSO : AbstractDataModel
    {
        private const string identifier = "_SOmemory";

        public Dictionary<string, uint> RecentOrdersCompleted { get; set; } = new();

        public RecentCompletedSO(string savefile)
        {
            this.Savefile = savefile;
        }

        public static RecentCompletedSO Load()
        {
            if (!Context.IsWorldReady) { throw new SaveNotLoadedError(); }
            return ModEntry.DataHelper.ReadGlobalData<RecentCompletedSO>(Constants.SaveFolderName + identifier) ?? new RecentCompletedSO(Constants.SaveFolderName);
        }

        public void Save()
        {
            base.Save(identifier);
        }

        public void dayUpdate(uint daysPlayed)
        {
            foreach (string key in RecentOrdersCompleted.Keys)
            {
                if (daysPlayed > RecentOrdersCompleted[key] + 7)
                {
                    RecentOrdersCompleted.Remove(key);
                }
            }
        }

        public bool Add(string orderKey, uint daysPlayed) => RecentOrdersCompleted.TryAdd(orderKey, daysPlayed);

        public bool Remove(string orderKey) => RecentOrdersCompleted.Remove(orderKey);

        public bool IsWithinXDays(string orderKey, uint days)
        {
            if (RecentOrdersCompleted.TryGetValue(orderKey, out uint dayCompleted))
            {
                return dayCompleted + days > Game1.stats.daysPlayed;
            }
            return false;
        }

        public IEnumerable<string> GetKeys(uint days)
        {
            return RecentOrdersCompleted.Keys
                .Where(a => RecentOrdersCompleted[a] + days >= Game1.stats.DaysPlayed);
        }

        public override string ToString()
        {
            StringBuilder stringBuilder = new();
            stringBuilder.AppendLine($"RecentCompletedSO{Savefile}");
            foreach (string key in Utilities.ContextSort(RecentOrdersCompleted.Keys))
            {
                stringBuilder.AppendLine($"{key} completed on Day {RecentOrdersCompleted[key]}");
            }
            return stringBuilder.ToString();
        }
    }
}
