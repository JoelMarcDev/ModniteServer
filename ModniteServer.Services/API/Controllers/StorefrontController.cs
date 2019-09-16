using ModniteServer.API.Store;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace ModniteServer.API.Controllers
{
    internal sealed class StorefrontController : Controller
    {
        /// <summary>
        /// Gets the keys needed to decrypt some assets.
        /// </summary>
        [Route("GET", "/fortnite/api/storefront/v2/keychain")]
        public void GetKeychain()
        {
            // This API gives the encryption key needed to decrypt data in pakchunk1000+

            var response = new[]
            {
                // [GUID]:[base64 encoded key]:[skin]
                // https://poggers.me/aes
                "D8FDE5644FE474C7C3D476AA18426FEB:orzs/Wp9XxE5vpybtd0tOxX6hrMyZZheFZusAw1c+6A=:BID_126_DarkBomber",
                "8F6ACF5D43BC4BC272D72EBC072BDB4F:rsT5K8O82gjB/BWAR7zl6cBstk0xxiu/E0AK/RQNUjE=:CID_246_Athena_Commando_F_Grave",
                "4A8216304A1A18CB9583BC8CFF99EE26:QF3nHCFt1vhELoU4q1VKTmpxnk20c2iAiBEBzlbzQAY=:CID_184_Athena_Commando_M_DurrburgerWorker",
                "FE0FA56F4B280D2F0CB2AB899C645F3E:hYi0DrAf6wtw7Zi+PlUi7/vIlIB3psBzEb5piGLEW6s=:CID_220_Athena_Commando_F_Clown",
                "D50ABA0F48BD66E4044616BDC40F4AD6:GNzmjA0ytPrD6J//HSbVF0qypflabpJ3guKTdX4ZStE=:BID_115_DieselpunkMale",
                "7FA4F2374FFE075000BC209360056A5A:nywIiZlIL8AIMkwCZfrYoAkpHM3zCwddhfszh++6ejI=:CID_223_Athena_Commando_M_Dieselpunk",
                "E45BD1CD4B6669367E57AFB5DC2B4478:EXfrtfslMES/Z2M/wWCEYeQoWzI1GTRaElXhaHBw8YM=:CID_229_Athena_Commando_F_DarkBomber"
            };

            Response.StatusCode = 200;
            Response.ContentType = "application/json";
            Response.Write(JsonConvert.SerializeObject(response));
        }

        [Route("POST", "/fortnite/api/game/v2/profile/*/client/PurchaseCatalogEntry")]
        public void PurchaseItem()
        {
            string accountId = Request.Url.Segments[Request.Url.Segments.Length - 3].Trim('/');
            Query.TryGetValue("profileId", out string profileId);
            Query.TryGetValue("rvn", out string rvn);

            var response = new
            {
                // TODO
            };

            Response.StatusCode = 200;
            Response.ContentType = "application/json";
            Response.Write(JsonConvert.SerializeObject(response));
        }

        /// <summary>
        /// Since we're never going to sell anything, there's not much to do here.
        /// </summary>
        [Route("GET", "/fortnite/api/receipts/v1/account/*/receipts")]
        public void GetReceipts()
        {
            Response.StatusCode = 200;
            Response.ContentType = "application/json";
            Response.Write("[]");
        }

        [Route("GET", "/fortnite/api/storefront/v2/catalog", isHidden: true)]
        public void GetCatalog()
        {
            var storefronts = new List<object>();
            foreach (Storefront storefront in StorefrontManager.Storefronts)
            {
                var catalogEntries = new List<object>();
                foreach (StoreItem item in storefront.Catalog)
                {
                    var catalogItem = new
                    {
                        devName = item.TemplateId,
                        offerId = "v2:/" + item.Guid,
                        fulfillmentIds = new List<object>(),
                        dailyLimit = -1,
                        weeklyLimit = -1,
                        monthlyLimit = -1,
                        categories = item.Categories,
                        prices = new[]
                        {
                            new
                            {
                                currencyType = "MtxCurrency",
                                currencySubType = "",
                                regularPrice = 0,
                                finalPrice = 0,
                                saleExpiration = DateTime.MaxValue.ToDateTimeString(),
                                basePrice = 0
                            }
                        },
                        matchFilter = "",
                        filterWeight = 0.0,
                        appStoreId = new List<object>(),
                        requirements = new List<object>()
                        {
                            new
                            {
                                requirementType = "DenyOnItemOwnership",
                                requiredId = item.TemplateId,
                                minQuantity = 1
                            }
                        },
                        offerType = "StaticPrice",
                        giftInfo = new
                        {
                            bIsEnabled = false,
                            forcedGiftBoxTemplateId = "",
                            purchaseRequirements = new List<object>(),
                            giftRecordIds = new List<object>()
                        },
                        refundable = true,
                        metaInfo = new List<object>(),
                        displayAssetPath = item.DisplayAssetPath,
                        itemGrants = new []
                        {
                            new
                            {
                                templateId = item.TemplateId,
                                quantity = 1
                            }
                        },
                        sortPriority = item.Priority,
                        catalogGroupPriority = 0
                    };

                    if (!item.DenyOnOwnership)
                    {
                        catalogItem.requirements.Clear();
                    }

                    if (item.BannerType != StoreItemBannerType.None)
                    {
                        if (storefront.IsWeeklyStore)
                        {
                            if (item.BannerType == StoreItemBannerType.New)
                            {
                                catalogItem.metaInfo.Add(new
                                {
                                    key = "StoreToastHeader",
                                    value = "New",
                                });
                                catalogItem.metaInfo.Add(new
                                {
                                    key = "StoreToastBody",
                                    value = "NewSet",
                                });
                            }

                            // TODO: add support for other banners in the weekly store section
                        }
                        else
                        {
                            catalogItem.metaInfo.Add(new
                            {
                                key = "BannerOverride",
                                value = item.BannerType.ToString()
                            });
                        }
                    }

                    catalogEntries.Add(catalogItem);
                }

                storefronts.Add(new
                {
                    name = storefront.Name,
                    catalogEntries
                });
            }

            var response = new
            {
                refreshIntervalHrs = StorefrontManager.RefreshInterval,
                dailyPurchaseHrs = StorefrontManager.RefreshInterval,
                expiration = StorefrontManager.Expiration.ToDateTimeString(),
                storefronts
            };

            Response.StatusCode = 200;
            Response.ContentType = "application/json";
            Response.Write(JsonConvert.SerializeObject(response));
        }
    }
}