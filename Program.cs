using Document_Extractor.Data;
using Document_Extractor.Models.DB;
using Document_Extractor.Repositories.Implementation;
using Document_Extractor.Repositories.Interfaces;
using Document_Extractor.Services.Implementation;
using Document_Extractor.Services.Implementations;
using Document_Extractor.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Microsoft.AspNetCore.Authentication.Negotiate;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

var logger = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Configuration)
                .Enrich.FromLogContext()
                .CreateLogger();

builder.Logging.AddSerilog(logger);

// Add services to the container.
builder.Services.AddIdentity<AppUser, AppRole>(config =>
                {
                    config.Password.RequireLowercase = true;
                })
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();
//AppDB service
var timeout = Convert.ToInt32(builder.Configuration.GetSection("App:DBTimeOutInMinutes").Value);

builder.Services.AddDbContextPool<AppDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("AppConnectionString"),
        opts => opts.CommandTimeout(timeout)
 ));

builder.Services.AddSingleton<AppDapperContext>();

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(Convert.ToDouble(builder.Configuration.GetSection("App:CookieTimeOutInMS").Value));
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.Name = builder.Configuration.GetSection("App:CookieTimeOutInMS").Value.Replace(" ", "");

});

// builder.Services.AddAuthentication(NegotiateDefaults.AuthenticationScheme)
//    .AddNegotiate();

// builder.Services.AddAuthorization(options =>
// {
//     // By default, all incoming requests will be authorized according to the default policy.
//     options.FallbackPolicy = options.DefaultPolicy;

// });

// builder.Services.ConfigureApplicationCookie(options =>
// {
//     options.LoginPath = new PathString("/Login");
//     options.AccessDeniedPath = new PathString("/Login");
// });

builder.Services.AddAutoMapper(typeof(Program).Assembly);

// Add services to the container.
builder.Services.AddControllersWithViews();


//add Repositories
builder.Services.AddScoped<ITeamRepository, TeamRepository>();
builder.Services.AddScoped<IPatientRepository, PatientRepository>();
builder.Services.AddScoped<IAppUserRepository<AppUser>, AppUserRepository>();
// builder.Services.AddScoped<IAppRoleRepository<AppRole>, AppRoleRepository>();
builder.Services.AddScoped<IAppConstantRepository, AppConstantRepository>();

//add Services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IExtractorService, ExtractorService>();
builder.Services.AddScoped<IHelperService, HelperService>();
builder.Services.AddScoped<ITeamService, TeamService>();
builder.Services.AddScoped<IPatientService, PatientService>();



var app = builder.Build();

//initialize the db
var scope = app.Services.CreateScope();
await DbInitializer.Initialize(scope);



// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

//app.UseAuthorization();

app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

