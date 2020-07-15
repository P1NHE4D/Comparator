using Microsoft.AspNetCore.Mvc;

namespace Comparator.Controllers {

    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("/")]
    public class LandingPageController : Controller {

        [HttpGet]
        public IActionResult Index() {
            return View("Index");
        }
    }
}