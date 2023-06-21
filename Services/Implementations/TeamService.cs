using AutoMapper;
using Document_Extractor.Models.DB;
using Document_Extractor.Models.Shared;
using Document_Extractor.Repositories.Interfaces;
using Document_Extractor.Services.Interfaces;

namespace Document_Extractor.Services.Implementations;


public class TeamService : ITeamService
{
    private readonly ILogger<TeamService> _logger;
    private readonly ITeamRepository _teamRepository;
    private readonly IHelperService _helperService;
    private readonly IMapper _mapper;

    public TeamService(
        ILogger<TeamService> logger,
        ITeamRepository teamRepository,
        IHelperService helperService,
        IMapper mapper
    )
    {
        _logger = logger;
        _teamRepository = teamRepository;
        _helperService = helperService;
        _mapper = mapper;


    }

    public async Task<AppResult<TeamDTO>> GetTeam(long teamId)
    {
        var result = new AppResult<TeamDTO>();
        try
        {
            var r = await _teamRepository.GetOne(teamId);
            if (r != null)
            {
                var dto = _mapper.Map<TeamDTO>(r);
                result.Data.Add(dto);
            }
            result.Status = true;

        }
        catch (Exception ex)
        {
            await _helperService.CustomLogError(ex, "GetTeam");
            result.Status = false;
            result.Message = ex.Message;
        }

        return result;
    }

    public async Task<AppResult<TeamDTO>> GetTeams()
    {
        var result = new AppResult<TeamDTO>();
        try
        {
            var r = await _teamRepository.GetAll();
            if (r.Any())
            {
                var dtos = _mapper.Map<IEnumerable<TeamDTO>>(r);
                result.Data = dtos.OrderByDescending(x =>x.CreatedAt).ToList();
            }
            result.Status = true;

        }
        catch (Exception ex)
        {
            await _helperService.CustomLogError(ex, "GetTeams");
            result.Status = false;
            result.Message = ex.Message;
        }

        return result;

    }

    public async Task<AppResult<TeamDTO>> AddTeam(TeamDTO team)
    {
        var result = new AppResult<TeamDTO>();
        try
        {
            var teamData = _mapper.Map<Team>(team);
            team.CreatedAt = DateTime.Now;
            team.UpdatedAt = DateTime.Now;

            var dbTeam = await _teamRepository.Create(teamData);
            if (dbTeam is null) throw new Exception($"Unable to Add team with Code = {team.Code}, Description = {team.CodeDescription}");

            var dto = _mapper.Map<TeamDTO>(dbTeam);
            result.Data.Add(dto);
            result.Message = $"New Team {dto.CodeDescription} was Added!";

            result.Status = true;
        }
        catch (Exception ex)
        {
            await _helperService.CustomLogError(ex, "AddTeam");
            result.Status = false;
            result.Message = ex.Message;
        }

        return result;

    }
    public async Task<AppResult<TeamDTO>> UpdateTeam(TeamDTO team, long teamId)
    {
        var result = new AppResult<TeamDTO>();
        try
        {
            var teamData = _mapper.Map<Team>(team);
            team.UpdatedAt = DateTime.Now;

            bool status = await _teamRepository.Update(teamData, teamId);
            if (!status) throw new Exception($"Unable to Update team with Code = {team.Code}, Description = {team.CodeDescription}");

            result.Data.Add(team);
            result.Status = true;
            result.Message = $"Team {team.Code} was Updated!";

        }
        catch (Exception ex)
        {
            await _helperService.CustomLogError(ex, "AddTeam");
            result.Status = false;
            result.Message = ex.Message;
        }

        return result;

    }





}