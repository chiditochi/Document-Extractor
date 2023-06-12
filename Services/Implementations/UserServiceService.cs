using Document_Extractor.Services.Interfaces;

namespace Document_Extractor.Services.Implementations;


public class UserService : IUserService
{
    private readonly ILogger<UserService> _logger;

    public UserService(
        ILogger<UserService> logger
    )
    {
        _logger = logger;
    }


}