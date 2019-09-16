using System.Collections.Generic;

namespace ModniteServer.API.Store
{
    public class Storefront
    {
        public string Name { get; set; }

        public List<StoreItem> Catalog { get; set; }

        public bool IsWeeklyStore { get; set; }
    }
}