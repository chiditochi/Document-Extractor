using Document_Extractor.Data;
using Document_Extractor.Models.DB;
using Document_Extractor.Models.Shared;
using Document_Extractor.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Document_Extractor.Repositories.Implementation;


public class AppUserRepository : IAppUserRepository<AppUser>
{
    private readonly AppDbContext _context;
    private readonly UserManager<AppUser> _userManager;
    private readonly RoleManager<AppRole> _roleManager;

    public AppUserRepository(
        AppDbContext context,
        UserManager<AppUser> userManager,
        RoleManager<AppRole> roleManager
        )
    {
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task<IEnumerable<AppUser>> GetAll()
    {
        var all = await _context.Users
                                //.Include(x => x.UserType)
                                //.Include(x => x.UserRoles)
                                .OrderByDescending(x => x.CreatedAt)
                                .AsNoTracking()
                                .ToListAsync();
        return all;
    }

    public async Task<AppUser> GetOne(long itemId)
    {
        var item = await _context.Users
                                //.Include(x => x.UserType)
                                //.Include(x => x.UserRoles)
                                .AsNoTracking()
                                .FirstOrDefaultAsync(x => x.Id == itemId);
        return item;
    }
    public async Task<AppUser> GetByEmail(string email)
    {
        var item = await _context.Users
                                //.Include(x => x.UserType)
                                //.Include(x => x.UserRoles)
                                .AsNoTracking().FirstOrDefaultAsync(x => x.Email == email);
        return item;
    }




    public async Task<AppResult<AppUser>> ToggleUser(long userId)
    {
        AppResult<AppUser> result = new AppResult<AppUser>();
        try
        {
            var dbUser = await GetOne(userId);
            if (dbUser == null) throw new Exception($"Error fetching user {userId}");

            dbUser.IsActive = !dbUser.IsActive;
            dbUser.UpdatedAt = DateTime.Now;

            _context.Entry(dbUser).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            result.Status = true;
            result.Data.Add(dbUser);

        }
        catch (Exception ex)
        {
            Console.WriteLine($"{ex.Message}");
            result.Status = false;
            result.Message = ex.Message;
        }

        return result;
    }

    public Task<AppUser> Create(AppUser item)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<AppUser>> CreateMany(IEnumerable<AppUser> items)
    {
        using var transaction = _context.Database.BeginTransaction();

        try
        {
            foreach (var item in items)
            {
                await _userManager.CreateAsync(item);
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

    public async Task<bool> Update(AppUser item, long itemId)
    {
        var r = await _userManager.UpdateAsync(item);
        if (!r.Succeeded) throw new Exception($"{string.Join(" | ", r.Errors)}");
        return true;
    }

    public async Task<bool> Delete(long itemId)
    {
        var item = await _userManager.FindByIdAsync(itemId.ToString());
        if (item == null) throw new Exception($"Role with id {itemId} does not exist");
        await _userManager.DeleteAsync(item);
        return true;
    }
}