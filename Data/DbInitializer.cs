using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Document_Extractor.Models.DB;
using Document_Extractor.Models.Shared;
using Document_Extractor.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Document_Extractor.Data
{
    public static class DbInitializer
    {
        public static async Task Initialize(IServiceScope scope)
        {
            AppDbContext _context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            ILogger<Program> _logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
            IConfiguration _config = scope.ServiceProvider.GetRequiredService<IConfiguration>();
            UserManager<AppUser> _userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
            RoleManager<AppRole> _roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<AppRole>>();
            IHelperService _helperService = scope.ServiceProvider.GetRequiredService<IHelperService>();
            IWebHostEnvironment _wenv = scope.ServiceProvider.GetRequiredService<IWebHostEnvironment>();

            if (_context.Users.Any())
            {
                return;   // DB has been seeded
            }

            _logger.LogInformation($"DBInit Started ...");
            //Enter seed data 
            using var transaction = _context.Database.BeginTransaction();
            try
            {

                /*
                    1. add default UserTypes 
                    2. add default AppRoles 
                    3. add users
                    4. add Teams
                    5. add AppConstant
                */
                var jsonFilePath = Path.Combine(_wenv.ContentRootPath, "data.json");
                string jsonFile = File.ReadAllText(jsonFilePath);
                TestData jsonData = JsonConvert.DeserializeObject<TestData>(jsonFile)!;

                //profile default UserTypes ...
                await StoreDefaultUserTypes(scope, jsonData.UserTypes!);

                //profile default AppRoles ...
                await StoreDefaultAppRoles(scope, jsonData.Roles!);

                //add default Admin ...
                await StoreDefaultAppUser(scope, jsonData.Users!);

                //Add app navigation 
                await StoreDefaultTeams(scope, jsonData.Teams!);

                //Add app navigation 
                await StoreDefaultAppConstants(scope, jsonData.AppConstant!);

                await transaction.CommitAsync();
                _logger.LogInformation($"Default data creation was successful ... ");
            }
            catch (Exception ex)
            {
                await _helperService.CustomLogError(ex, "Initialize");
            }

            _logger.LogInformation($"DBInit Completed ...");

        }

        private static async Task StoreDefaultUserTypes(IServiceScope scope, string[] userTypes)
        {
            AppDbContext _context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            ILogger<Program> _logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
            IHelperService _helperService = scope.ServiceProvider.GetRequiredService<IHelperService>();

            if (userTypes != null && userTypes?.Length > 0)
            {
                var userTypesToPersist = userTypes.Select(x => new UserType
                {
                    Label = x,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                }).ToList();

                foreach (var x in userTypes)
                {
                    var u = new UserType
                    {
                        Label = x,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    };

                    _context.Entry<UserType>(u).State = EntityState.Added;
                }

                await _context.SaveChangesAsync();
                _logger.LogInformation($"TestUserTypes: persisted {userTypes.Length} UserTypes");
            }

        }
        private static async Task StoreDefaultAppRoles(IServiceScope scope, TestRole[] roles)
        {
            AppDbContext _context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            ILogger<Program> _logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
            IHelperService _helperService = scope.ServiceProvider.GetRequiredService<IHelperService>();
            RoleManager<AppRole> _roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<AppRole>>();


            var dbUserTypes = await _context.UserTypes.ToListAsync();
            if (!dbUserTypes.Any()) throw new Exception($"Error fetching UserTypes ...");
            if (roles != null && roles.Length > 0)
            {
                foreach (var role in roles)
                {
                    var utId = dbUserTypes.First(x => x.Label == role.UserType).UserTypeId;

                    var r = new AppRole();
                    r.Name = role.Name;
                    r.NormalizedName = role?.Name?.ToUpper();
                    r.Label = role?.Name!;
                    r.UserTypeId = utId;

                    //await _roleManager.CreateAsync(r);
                    _context.Entry<AppRole>(r).State = EntityState.Added;

                }
                await _context.SaveChangesAsync();
                _logger.LogInformation($"TestRoles: persisted {roles.Length} Roles");

            }
        }
        private static async Task StoreDefaultAppUser(IServiceScope scope, TestUser[] users)
        {
            AppDbContext _context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            ILogger<Program> _logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
            UserManager<AppUser> _userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();

            var dbUserTypes = await _context.UserTypes.ToListAsync();
            if (!dbUserTypes.Any()) throw new Exception($"Error fetching UserTypes ...");


            if (users != null && users.Length > 0)
            {

                foreach (var userItem in users)
                {
                    var userType = dbUserTypes.First(x => x.Label == userItem.UserType);

                    var user = new AppUser
                    {
                        FirstName = userItem.FirstName,
                        LastName = userItem.LastName,
                        Gender = userItem.Gender,
                        UserTypeId = userType.UserTypeId,
                        UserType = userType,
                        Email = userItem.Email,
                        UserName = userItem.Email,
                        IsActive = true,
                        Password = userItem.Password,
                    };

                    var userCreationResult = await _userManager.CreateAsync(user, user.Password);
                    if (!userCreationResult.Succeeded) throw new Exception($"Error creating default user profile ...");

                    var userRoleCreationResult = await _userManager.AddToRoleAsync(user, userItem.Role);
                    if (!userRoleCreationResult.Succeeded) throw new Exception($"Error creating default user role ...");

                    _logger.LogInformation($"TestUsers: persisted {users.Length} Users");

                }

            }


        }
        private static async Task StoreDefaultTeams(IServiceScope scope, TestTeam[] teams)
        {
            AppDbContext _context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            ILogger<Program> _logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

            _context.ChangeTracker.AutoDetectChangesEnabled = false;

            if (teams != null && teams?.Length > 0)
            {
                var teamsToPersist = teams.Select(x => new Team
                {
                    Code = x.Code,
                    CodeDescription = x.Description,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                }).ToList();

                _context.Teams.AddRange(teamsToPersist);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"TestTeams: persisted {teams.Length} Teams");

            }

        }

        private static async Task StoreDefaultAppConstants(IServiceScope scope, TestAppConstant[]? appConstants)
        {
            AppDbContext _context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            ILogger<Program> _logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

            _context.ChangeTracker.AutoDetectChangesEnabled = false;


            if (appConstants != null && appConstants?.Length > 0)
            {
                var constantList = new List<AppConstant>();
                foreach (var constantItem in appConstants)
                {
                    var label = constantItem.Label;
                    foreach (var lvItem in constantItem.LabelValues!)
                    {
                        var a = new AppConstant();
                        a.Label = label;
                        a.LabelValue = lvItem;
                        a.CreatedAt = DateTime.Now;
                        a.UpdatedAt = DateTime.Now;

                        constantList.Add(a);
                    }
                }

                _context.AppConstants.AddRange(constantList);
                await _context.SaveChangesAsync();

                var msg = $"TestData: persisted {constantList.Count} AppConstants!";
                _logger.LogInformation(msg);
            }



        }




    }
}
