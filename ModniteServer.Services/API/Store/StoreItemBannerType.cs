namespace ModniteServer.API.Store
{
    public enum StoreItemBannerType
    {
        /// <summary>
        /// No banner overlay.
        /// </summary>
        None,

        /// <summary>
        /// Displays "New!" on top of the item in the store.
        /// </summary>
        New,

        /// <summary>
        /// Displays "Updated!" on top of the item in the store.
        /// </summary>
        Updated,

        /// <summary>
        /// Displays "Last Chance!" on top of the item in the store.
        /// </summary>
        LastChance,

        // There should be more...
    }
}