using Microsoft.AspNetCore.Mvc;

namespace Breezy.Controllers;

public class SessionsController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}