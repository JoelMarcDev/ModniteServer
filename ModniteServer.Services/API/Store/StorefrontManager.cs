using System;
using System.Collections.Generic;

namespace ModniteServer.API.Store
{
    public static class StorefrontManager
    {
        /// <summary>
        /// Defines the number of hours before the store will refresh.
        /// </summary>
        public const int RefreshInterval = 24;

        /// <summary>
        /// Initializes all storefronts.
        /// </summary>
        static StorefrontManager()
        {
            Storefronts = new List<Storefront>
            {
                new DailyStorefront(),
                new WeeklyStorefront()
            };
        }

        /// <summary>
        /// Gets a list of all available storefronts.
        /// </summary>
        public static IReadOnlyList<Storefront> Storefronts { get; }

        /// <summary>
        /// Gets the expiration date of the store. In our case, the store does not expire.
        /// </summary>
        public static DateTime Expiration { get; } = DateTime.MaxValue;
    }
}