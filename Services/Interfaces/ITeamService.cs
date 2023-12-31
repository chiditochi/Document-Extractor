
using Document_Extractor.Models.DB;
using Document_Extractor.Models.Shared;

namespace Document_Extractor.Services.Interfaces;

public interface ITeamService
{
    public Task<AppResult<TeamDTO>> GetTeam(long teamId);
    public Task<AppResult<TeamDTO>> GetTeams();
    public Task<AppResult<TeamDTO>> AddTeam(TeamDTO team);
    public Task<AppResult<TeamDTO>> UpdateTeam(TeamDTO team, long teamId);

}