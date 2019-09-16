using System;
using System.Collections.Generic;

namespace ModniteServer.API.Store
{
    public class StoreItem
    {
        public Guid Guid { get; set; } = Guid.NewGuid();

        public bool DenyOnOwnership { get; set; } = true;

        public StoreItemBannerType BannerType { get; set; } = StoreItemBannerType.None;

        /// <summary>
        /// Negative integer for weekly store, positive integer for daily store.
        /// </summary>
        public int Priority { get; set; } = 0;

        public List<string> Categories { get; set; } = new List<string>();

        public string DisplayAssetPath { get; set; } = "";

        public string TemplateId { get; set; } = "";
    }
}