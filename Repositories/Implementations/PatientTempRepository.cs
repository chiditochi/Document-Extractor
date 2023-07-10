using System.Globalization;
using Document_Extractor.Data;
using Document_Extractor.Models.DB;
using Document_Extractor.Models.Shared;
using Document_Extractor.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Document_Extractor.Repositories.Implementation;


public class PatientTempRepository : IPatientTempRepository
{
    private readonly AppDbContext _context;
    public PatientTempRepository(
        AppDbContext context
        )
    {
        _context = context;
    }

    public async Task<PatientTemp> Create(PatientTemp item)
    {
        _context.Entry(item).State = EntityState.Added;
        await _context.SaveChangesAsync();

        return item;
    }

    public async Task<IEnumerable<PatientTemp>> CreateMany(IEnumerable<PatientTemp> items)
    {
        _context.PatientTemps.AddRange(items);
        await _context.SaveChangesAsync();
        return items;
    }

    public async Task<bool> Delete(long itemId)
    {
        var PatientTemp = await GetOne(itemId);
        if (PatientTemp == null) throw new Exception($"Error fetching PatientTemp with id ={itemId}");

        _context.Entry(PatientTemp).State = EntityState.Deleted;
        await _context.SaveChangesAsync();

        return true;

    }

    public async Task<IEnumerable<PatientTemp>> GetAll()
    {
        var result = await _context.PatientTemps.Include(x => x.Team).OrderByDescending(x => x.CreatedAt).ToListAsync();
        return result;
    }

    public async Task<PatientTemp> GetOne(long itemId)
    {
        var result = await _context.PatientTemps.FirstOrDefaultAsync(x => x.PatientTempId == itemId);
        return result;
    }

    public async Task<bool> Update(PatientTemp item, long itemId)
    {
        var PatientTemp = await GetOne(itemId);
        if (PatientTemp == null) throw new Exception($"Error fetching PatientTemp with id ={itemId}");

        _context.Entry(PatientTemp).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        return true;
    }




}