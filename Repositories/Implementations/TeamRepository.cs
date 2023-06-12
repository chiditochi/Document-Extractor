using Document_Extractor.Data;
using Document_Extractor.Models.DB;
using Document_Extractor.Models.Shared;
using Document_Extractor.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Document_Extractor.Repositories.Implementation;


public class TeamRepository : ITeamRepository
{
    private readonly AppDbContext _context;
    public TeamRepository(
        AppDbContext context
        )
    {
        _context = context;
    }

    public async Task<Team> Create(Team item)
    {
        _context.Entry(item).State = EntityState.Added;
        await _context.SaveChangesAsync();

        return item;
    }

    public async Task<IEnumerable<Team>> CreateMany(IEnumerable<Team> items)
    {
        _context.Teams.AddRange(items);
        await _context.SaveChangesAsync();
        return items;
    }

    public async Task<bool> Delete(long itemId)
    {
        var team = await GetOne(itemId);
        if (team == null) throw new Exception($"Error fetching Team with id ={itemId}");

        _context.Entry(team).State = EntityState.Deleted;
        await _context.SaveChangesAsync();

        return true;

    }

    public async Task<IEnumerable<Team>> GetAll()
    {
        var result = await _context.Teams.ToListAsync();
        return result;
    }

    public async Task<Team> GetOne(long itemId)
    {
        var result = await _context.Teams.FirstOrDefaultAsync(x => x.TeamId == itemId);
        return result;
    }

    public async Task<bool> Update(Team item, long itemId)
    {
        var team = await GetOne(itemId);
        if (team == null) throw new Exception($"Error fetching Team with id ={itemId}");

        _context.Entry(team).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        return true;
    }
}