using Microsoft.AspNetCore.Mvc;

namespace Breezy.Controllers;

public class ProjectsController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}