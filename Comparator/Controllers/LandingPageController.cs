using Microsoft.AspNetCore.Mvc;

namespace Comparator.Controllers {

    /// <summary>
    /// View controller for the landing page
    /// </summary>
    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("/")]
    public class LandingPageController : Controller {

        [HttpGet]
        public IActionResult Index() {
            return View("Index");
        }
    }
}