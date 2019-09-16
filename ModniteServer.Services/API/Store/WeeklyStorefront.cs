using System.Collections.Generic;

namespace ModniteServer.API.Store
{
    internal sealed class WeeklyStorefront : Storefront
    {
        public WeeklyStorefront()
        {
            Name = "BRWeeklyStorefront";
            IsWeeklyStore = true;
            Catalog = new List<StoreItem>();
            var Priority = -1;
            foreach (var i in ApiConfig.Current.FeaturedShopItems)
            {
                Catalog.Add(new StoreItem
                {
                    TemplateId = i,
                    Priority = Priority
                });
                Priority -= 1;
            }
        }
    }
}