using Microsoft.AspNetCore.Mvc;

namespace Breezy.Controllers;

public class SoundPacksController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}