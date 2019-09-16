using System.Collections.Generic;

namespace ModniteServer.API.Store
{
    public sealed class DailyStorefront : Storefront
    {
        public DailyStorefront()
        {
            Name = "BRDailyStorefront";
            
            Catalog = new List<StoreItem>();
            foreach (var i in ApiConfig.Current.DailyShopItems)
            {
                Catalog.Add(new StoreItem
                {
                    TemplateId = i
                });
            }
        }
    }
}