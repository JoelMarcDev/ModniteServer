namespace ModniteServer.API.Controllers
{
    internal sealed class FeaturesController : Controller
    {
        [Route("GET", "/fortnite/api/game/v2/enabled_features")]
        public void GetEnabledFeatures()
        {
            Response.StatusCode = 200;
            Response.ContentType = "application/json";
            Response.Write("[]");
        }
    }
}