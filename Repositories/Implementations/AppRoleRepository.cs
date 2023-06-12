
using Document_Extractor.Data;
using Document_Extractor.Models.DB;
using Document_Extractor.Models.Shared;
using Document_Extractor.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Document_Extractor.Repositories.Implementation;


public class AppRoleRepository : IAppRoleRepository<AppRole>
{
    private readonly AppDbContext _context;
    private readonly RoleManager<AppRole> _roleManager;

    public AppRoleRepository(AppDbContext context, RoleManager<AppRole> roleManager)
    {
        _context = context;
        _roleManager = roleManager;
    }

    public async Task<bool> RoleExists(AppRole role)
    {
        /*
            1. get all roles 
            2. get roles where role.Name and role.UserTypeId 
            3. return false if step 2 returns null or empty
        */
        var result = false;
        var allRoles = await GetAll();
        if (allRoles != null && allRoles.Any())
        {
            result = allRoles.Any(x => x.UserTypeId == role.UserTypeId && x.Name.ToUpper() == role.Name.ToUpper());
        }
        return result;
    }

    public async Task<AppRole> Create(AppRole item)
    {
        var roleExists = await RoleExists(item);
        if (roleExists) throw new Exception($"AppUserRole with Name = {item.Name} and UserTypeId = {item.UserTypeId} already exists");
        var r = await _roleManager.CreateAsync(item);
        if (!r.Succeeded) throw new Exception($"{string.Join(" | ", r.Errors)}");
        return item;
    }

    public async Task<AppResult<AppRole>> CreateRole(AppRole role)
    {
        var result = new AppResult<AppRole>();
        try
        {
            var response = await Create(role);
            if(response == null) throw new Exception($"Role creation failed");

            result.Data.Add(response);
            result.Status = true;
            result.Message = $"Success";

        }
        catch (Exception ex)
        {
            result.Status = false;
            result.Message = ex.Message;
        }
        return result;
    }

    public async Task<bool> Update(AppRole item, long itemId)
    {
        var r = await _roleManager.UpdateAsync(item);
        if (!r.Succeeded) throw new Exception($"{string.Join(" | ", r.Errors)}");
        return true;

    }

    public async Task<AppResult<AppRole>> UpdateRole(AppRole role)
    {
        var result = new AppResult<AppRole>();
        try
        {
            /*
                1. get role using role.Id 
                2. update only userTypeId | Label | updatedAt

            */
            var dbRole = await GetOne(role.Id);
            if (dbRole == null) throw new Exception($"Cannot find role with id = {role.Id}");
            // var roleExists = await RoleExists(role);
            // if (roleExists) throw new Exception($"Role with {role.UserTypeId} and {role.Name} already exists");
            dbRole.UserTypeId = role.UserTypeId;
            dbRole.Label = role.Label;
            dbRole.Name = role.Name;
            dbRole.UpdatedAt = DateTime.Now;
            _context.Entry(dbRole).State = EntityState.Modified;
            await _context.SaveChangesAsync();


            result.Data.Add(dbRole);
            result.Status = true;
            result.Message = $"Success";

        }
        catch (Exception ex)
        {
            result.Status = false;
            result.Message = ex.Message;
        }
        return result;
    }


    public async Task<IEnumerable<AppRole>> CreateMany(IEnumerable<AppRole> items)
    {
        using var transaction = _context.Database.BeginTransaction();

        try
        {
            foreach (var item in items)
            {
                await _roleManager.CreateAsync(item);
            }
            // Commit transaction if all commands succeed, transaction will auto-rollback
            // when disposed if either commands fails
            await transaction.CommitAsync();
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
        return items;
    }

    public async Task<bool> Delete(long itemId)
    {
        var role = await _roleManager.FindByIdAsync(itemId.ToString());
        if (role == null) throw new Exception($"Role with id {itemId} does not exist");
        await _roleManager.DeleteAsync(role);
        return true;

    }

    public async Task<IEnumerable<AppRole>> GetAll()
    {
        var all = await _context.Roles
                            .Include(u => u.UserType).AsNoTracking()
                            .OrderByDescending(x => x.CreatedAt).ToListAsync();
        return all;
    }

    public async Task<AppResult<long>> GetUserRoleIds(long userId)
    {
        var result = new AppResult<long>();
        var userRoleIds = await _context.UserRoles.Where(x => x.UserId == userId).Select(x => x.RoleId).ToListAsync();
        if (userRoleIds.Any())
        {
            result.Data = userRoleIds;
            result.Status = true;
            result.Message = "Success";
        }
        return result;
    }

    public async Task<AppRole?> GetByUserRoleName(string roleName)
    {
        return await _context.Roles.FirstOrDefaultAsync(x => x.Name == roleName);
    }

    public async Task<AppRole?> GetByUserTypeId(long userTypeId)
    {
        return await _context.Roles.FirstOrDefaultAsync(x => x.UserTypeId == userTypeId);
    }
    public async Task<AppRole?> GetByRoleIdAndUserTypeId(long userRoleId, long userTypeId)
    {
        return await _context.Roles.FirstOrDefaultAsync(x => x.UserTypeId == userTypeId && x.Id == userRoleId);
    }

    public async Task<AppRole> GetOne(long roleId)
    {
        var appRole = await _roleManager.FindByIdAsync(roleId.ToString());
        //var item = await _context.AppRoles.Include(u => u.UserType).AsNoTracking().FirstOrDefaultAsync(x => x.AppRoleId == itemId);
        return appRole;
    }
    public async Task<AppRole?> GetRoleByUserTypeIdAndName(AppRole role)
    {
        var appRole = await _context.Roles.FirstOrDefaultAsync(x => x.UserTypeId == role.UserTypeId && x.Name.ToUpper() == role.Name.ToUpper());
        return appRole;
    }

    // public async Task<IEnumerable<AppUserRoleDTO>> GetUserAndRoleIds()
    // {
    //     return await _context.UserRoles.Select(x => new AppUserRoleDTO(x.UserId, x.RoleId)).ToListAsync();
    // }


    public async Task<AppResult<AppRole>> GetUserRoles(long userId)
    {
        var result = new AppResult<AppRole>();
        try
        {
            /*
                1. get all roles 
                2. get user role ids 
                3. filter user role
            */
            var allRoles = await GetAll();
            if (!allRoles.Any()) throw new Exception($"No AppRoles fetched");
            var userRoleIdsResult = await GetUserRoleIds(userId);
            if (userRoleIdsResult.Data.Any())
            {
                result.Data = allRoles.Where(x => userRoleIdsResult.Data.Contains(x.Id)).ToList();
            }

            result.Status = true;
            result.Message = $"Success";

        }
        catch (Exception ex)
        {
            result.Status = false;
            result.Message = ex.Message;
        }
        return result;
    }




}