using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Document_Extractor.Models;
using Document_Extractor.Models.Shared;

namespace Document_Extractor.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(
        ILogger<HomeController> logger

        )
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    [HttpGet("/Login")]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost("/Login")]
    public IActionResult Login(string email, string password)
    {
        return View();
    }

    [HttpGet("/Upload")]
    public IActionResult Upload()
    {
        return View();
    }

    [HttpPost("/Upload")]
    public IActionResult Upload([FromBody] UploadRequest formData)
    {
        return View();
    }


    [HttpGet("/Patients")]
    public IActionResult GetPatients()
    {
        return Json(new { });
    }

    [HttpPost("/Patient/Confirm")]
    public IActionResult PatientConfirm([FromBody] long patientId, [FromBody] bool status)
    {
        return Json(new { });
    }


    [HttpGet("/Teams")]
    public IActionResult Teams()
    {
        return Json(new { });
    }




}
