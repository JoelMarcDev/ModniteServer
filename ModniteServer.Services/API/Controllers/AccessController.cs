﻿using ModniteServer.API.Accounts;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Linq;

namespace ModniteServer.API.Controllers
{
    /// <summary>
    /// Handles requests for access including version, platform, and account checks.
    /// </summary>
    internal sealed class AccessController : Controller
    {
        /// <summary>
        /// Checks whether the given version and platform needs an update.
        /// </summary>
        [Route("GET", "/fortnite/api/v2/versioncheck/*")]
        public void CheckVersion()
        {
            if (!Authorize()) return;

            string platform = Request.Url.Segments.Last();

            if (Query.TryGetValue("version", out string versionInfo))
            {
                // Parse version and check if it meets the minimum version requirements.
                versionInfo = versionInfo.Substring("++Fortnite+Release-".Length);
                versionInfo = versionInfo.Replace("-" + platform, "");
                int.TryParse(versionInfo.Substring(0, versionInfo.IndexOf('.')), out int major);
                int.TryParse(versionInfo.Substring(versionInfo.IndexOf('.') + 1, versionInfo.IndexOf('-') - 2), out int minor);
                int.TryParse(versionInfo.Substring(versionInfo.LastIndexOf('-') + 1), out int build);
                var version = new Version(major, minor, build);

                // HACK: Temporary workaround to get matchmaking access in v6.10
                if (version.Major == 6 && version.Minor == 10)
                {
                    var response = new
                    {
                        type = "NO_UPDATE" // NO_UPDATE | NOT_ENABLED | SOFT_UPDATE | HARD_UPDATE | APP_REDIRECT
                    };

                    Response.StatusCode = 200;
                    Response.ContentType = "application/json";
                    Response.Write(JsonConvert.SerializeObject(response));
                }

                // Sending NO_UPDATE will cause the "Game was not launched correctly" error,
                // so we're sending the client nothing.
                Response.StatusCode = 204;

                return;
            }

            Response.StatusCode = 403;
        }

        /// <summary>
        /// Checks whether the account has the privilege of playing on this platform.
        /// </summary>
        [Route("POST", "/fortnite/api/game/v2/tryPlayOnPlatform/account/*")]
        public void TryPlayOnPlatform()
        {
            if (!Authorize()) return;

            string accountId = Request.Url.Segments.Last();
            Query.TryGetValue("platform", out string platform);

            if (!AccountManager.AccountExists(accountId))
            {
                Response.StatusCode = 404;
                return;
            }

            Response.StatusCode = 200;
            Response.ContentType = "text/plain";
            Response.Write("true");
        }

        /// <summary>
        /// Grants access to the game for the given account.
        /// </summary>
        [Route("POST", "/fortnite/api/game/v2/grant_access/*")]
        public void GrantAccess()
        {
            if (!Authorize()) return;

            string accountId = Request.Url.Segments.Last();

            if (!AccountManager.AccountExists(accountId))
            {
                Response.StatusCode = 404;
                return;
            }

            Response.StatusCode = 204;
        }
    }
}