using Microsoft.AspNetCore.Mvc;

namespace SV21T1020581.Shop.Controllers
{
    public class CartController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
