using System.Globalization;
using Document_Extractor.Data;
using Document_Extractor.Models.DB;
using Document_Extractor.Models.Shared;
using Document_Extractor.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Document_Extractor.Repositories.Implementation;


public class AppConstantRepository : IAppConstantRepository
{
    private readonly AppDbContext _context;
    public AppConstantRepository(
        AppDbContext context
    )
    {
        _context = context;
    }

    public async Task<AppConstant> Create(AppConstant item)
    {
        _context.Entry(item).State = EntityState.Added;
        await _context.SaveChangesAsync();

        return item;
    }

    public async Task<IEnumerable<AppConstant>> CreateMany(IEnumerable<AppConstant> items)
    {
        _context.AppConstants.AddRange(items);
        await _context.SaveChangesAsync();
        return items;
    }

    public async Task<bool> Delete(long itemId)
    {
        var AppConstant = await GetOne(itemId);
        if (AppConstant == null) throw new Exception($"Error fetching AppConstant with id ={itemId}");

        _context.Entry(AppConstant).State = EntityState.Deleted;
        await _context.SaveChangesAsync();

        return true;

    }

    public async Task<IEnumerable<AppConstant>> GetAll()
    {
        var result = await _context.AppConstants.ToListAsync();
        return result;
    }

    public async Task<AppConstant> GetOne(long itemId)
    {
        var result = await _context.AppConstants.FirstOrDefaultAsync(x => x.AppConstantId == itemId);
        return result;
    }
    public async Task<bool> Update(AppConstant item, long itemId)
    {
        var AppConstant = await GetOne(itemId);
        if (AppConstant == null) throw new Exception($"Error fetching AppConstant with id ={itemId}");

        _context.Entry(AppConstant).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<IEnumerable<AppConstant>> GetRequiredProps()
    {
        var result = await _context.AppConstants.Where(x => x.Label == SharedConstants.RequiredProps).ToListAsync();
        return result;
    }

    public async Task<IEnumerable<AppConstant>> GetUploadExistProps()
    {
        var result = await _context.AppConstants.Where(x => x.Label == SharedConstants.UploadExistProps).ToListAsync();
        return result;
    }

    

    }