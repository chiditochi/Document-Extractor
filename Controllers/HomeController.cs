using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Document_Extractor.Models;
using Document_Extractor.Models.Shared;
using Document_Extractor.Models.DB;
using Document_Extractor.Services.Interfaces;

namespace Document_Extractor.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IConfiguration _config;
    private readonly IHelperService _helperService;
    private readonly IExtractorService _extractorService;
    private readonly ITeamService _teamService;
    private readonly IPatientService _patientService;

    public HomeController(
        ILogger<HomeController> logger,
        IConfiguration config,
        IHelperService helperService,
        IExtractorService extractorService,
        ITeamService teamService,
        IPatientService patientService

        )
    {
        _logger = logger;
        _config = config;
        _helperService = helperService;
        _extractorService = extractorService;
        _teamService = teamService;
        _patientService = patientService;
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
    public async Task<IActionResult> Upload()
    {
        await _helperService.CreateUploadFolders();
        return View();
    }

    [HttpGet("/UploadData")]
    public async Task<IActionResult> UploadData()
    {
        var result = await _patientService.GetPatients(true);
        return Json(new { Data = result });
    }



    [HttpPost("/UploadPost")]
    public async Task<IActionResult> UploadPost([FromForm] UploadRequest formData)
    {
        var result = new AppResult<PatientDTO>();
        try
        {
            if (!ModelState.IsValid)
            {
                List<string> errorList = _helperService.GetModelStateErrors(ModelState);
                throw new Exception(string.Join(",", errorList));
            }
            result = await _extractorService.ProcessUpload(formData);

        }
        catch (Exception ex)
        {
            await _helperService.CustomLogError(ex, "UploadPost");
            result.Status = false;
            result.Message = ex.Message;
        }

        return Json(new { Data = result });
    }


    [HttpGet("/Patients")]
    public IActionResult GetPatients()
    {
        return Json(new { });
    }

    //delete this 
    [HttpGet("/Patient/{PatientId:long}")]
    public async Task<IActionResult> GetPatient(long PatientId)
    {
        var result = await _patientService.GetPatient(PatientId);
        return Json(new { Data = result });
    }

    [HttpPost("/Upload/Confirmation")]
    public async Task<IActionResult> UploadConfirmation([FromBody] UploadConfirmationRequest payload)
    {
        var result = await _patientService.ConfirmUploadedPatient(payload.PatientId, payload.Status);
        return Json(new { Data = result });
    }


    [HttpGet("/ManageTeam")]
    public IActionResult ManageTeam()
    {
        return View();
    }

    [HttpGet("/Teams")]
    public async Task<IActionResult> Teams()
    {
        var result = await _teamService.GetTeams();
        return Json(new { Data = result });
    }

    [HttpPost("/Team")]
    public async Task<IActionResult> AddTeam([FromBody] TeamDTO payload)
    {
        var result = new AppResult<TeamDTO>();
        try
        {
            if (!ModelState.IsValid)
            {
                List<string> errorList = _helperService.GetModelStateErrors(ModelState);
                throw new Exception(string.Join(",", errorList));
            }
            result = await _teamService.AddTeam(payload);

        }
        catch (Exception ex)
        {
            await _helperService.CustomLogError(ex, "AddTeam");
            result.Status = false;
            result.Message = ex.Message;
        }

        return Json(new { Data = result });
    }

    [HttpPost("/Team/{TeamId:long}")]
    public async Task<IActionResult> UpdateTeam([FromBody] TeamDTO payload)
    {
        var result = new AppResult<TeamDTO>();
        try
        {
            if (!ModelState.IsValid)
            {
                List<string> errorList = _helperService.GetModelStateErrors(ModelState);
                throw new Exception(string.Join(",", errorList));
            }
            result = await _teamService.UpdateTeam(payload, payload.TeamId);

        }
        catch (Exception ex)
        {
            await _helperService.CustomLogError(ex, "UpdateTeam");
            result.Status = false;
            result.Message = ex.Message;
        }

        return Json(new { Data = result });
    }




}
