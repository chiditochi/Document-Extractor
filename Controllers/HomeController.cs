using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Document_Extractor.Models;
using Document_Extractor.Models.Shared;
using Document_Extractor.Models.DB;
using Document_Extractor.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Document_Extractor.Configuration.Filters;
using Document_Extractor.Repositories.Interfaces;

namespace Document_Extractor.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IConfiguration _config;
    private readonly IHelperService _helperService;
    private readonly IExtractorService _extractorService;
    private readonly ITeamService _teamService;
    private readonly IPatientService _patientService;
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManger;
    private readonly IAppUserRepository<AppUser> _appUserRepository;

    public HomeController(
        ILogger<HomeController> logger,
        IConfiguration config,
        IHelperService helperService,
        IExtractorService extractorService,
        ITeamService teamService,
        IPatientService patientService,
        UserManager<AppUser> userManager,
        SignInManager<AppUser> signInManager,
        IAppUserRepository<AppUser> appUserRepository

        )
    {
        _logger = logger;
        _config = config;
        _helperService = helperService;
        _extractorService = extractorService;
        _teamService = teamService;
        _patientService = patientService;
        _userManager = userManager;
        _signInManger = signInManager;
        _appUserRepository = appUserRepository;
    }

    [HttpGet("/")]
    [HttpGet("/Home/")]
    [AppAuthorize]
    [AppUserTypeFilter("Patient")]
    public IActionResult Index()
    {
        return View();
    }


    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }


    [HttpGet("/Home/Login")]
    [AllowAnonymous]
    public IActionResult Login()
    {
        return View();
    }

    [HttpGet("/Home/Logout")]
    [AppAuthorize]
    public async Task<IActionResult> Logout()
    {
        var result = new AppResult<bool>();
        try
        {
            await _signInManger.SignOutAsync();
            HttpContext.Session.Remove("username");

            result.Status = true;
            result.Message = "You are signed out!";
        }
        catch (Exception ex)
        {
            await _helperService.CustomLogError(ex, "Logout");
            result.Message = ex.Message;
            result.Status = false;
        }

        return Redirect(nameof(Login));

    }

    [HttpPost("/Home/Login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginDTO login)
    {
        var result = new AppResult<bool>();

        try
        {
            if (!ModelState.IsValid) throw new Exception($"Email and Password is Required!");
            var user = await _appUserRepository.GetByEmailForLogin(login.Email);
            if (user == null || !user!.IsActive) throw new Exception($"User not Found or is InActive");
            var r = await _signInManger.PasswordSignInAsync(user, login.Password, false, true);
            if (!r.Succeeded) throw new Exception($"Wrong login details provided!");

            //create Session 
            HttpContext.Session.SetString("username", user.UserName);
            HttpContext.Session.SetString("usertype", user.UserType.Label);

            result.Status = true;
            result.Message = $"Welcome {user!.LastName}, {user.FirstName}";


        }
        catch (Exception ex)
        {
            result.Status = false;
            result.Message = ex.Message;
            HttpContext.Session.Remove("username");

        }

        return Json(new { Data = result });
    }

    [HttpGet("/Home/Upload")]
    [AppAuthorize]
    public async Task<IActionResult> Upload()
    {
        await _helperService.CreateUploadFolders();
        return View();
    }

    [HttpGet("/Home/UploadData")]
    [AppAuthorize]
    public async Task<IActionResult> UploadData()
    {
        var result = await _patientService.GetPatients(true);
        return Json(new { Data = result });
    }

    [HttpPost("/Home/UploadPost")]
    public async Task<IActionResult> UploadPost([FromForm] UploadRequest formData)
    {
        var result = new AppResult<PatientTempDTO>();
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


    [HttpPost("/Home/UploadConfirmation")]
    public async Task<IActionResult> UploadConfirmation([FromBody] UploadConfirmationRequest payload)
    {
        //var result = await _patientService.ConfirmUploadedPatient(payload.PatientId, payload.Status);
        var result = await _patientService.ConfirmUpload(payload.PatientTempId, payload.Status);
        return Json(new { Data = result });
    }

    [HttpGet("/Home/ManageTeam")]
    [AppAuthorize]
    [AppUserTypeFilter("OpTeam")]
    public IActionResult ManageTeam()
    {
        return View();
    }

    [HttpGet("/Home/Teams")]
    [AppAuthorize]
    public async Task<IActionResult> Teams()
    {
        var result = await _teamService.GetTeams();
        return Json(new { Data = result });
    }

    [HttpPost("/Home/Team")]
    [AppUserTypeFilter("OpTeam")]
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

    [HttpPost("/Home/Team/{TeamId:long}")]
    [AppUserTypeFilter("OpTeam")]
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
