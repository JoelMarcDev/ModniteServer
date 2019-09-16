﻿using System.Linq;

namespace ModniteServer.API.Controllers
{
    internal sealed class AffiliateController : Controller
    {
        [Route("GET", "/affiliate/api/public/affiliates/slug/*")]
        public void CheckIfAffiliateExists()
        {
            string affiliateName = Request.Url.Segments.Last();

            Response.StatusCode = 404;
        }
    }
}