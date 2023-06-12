using System.Data;
using Microsoft.Data.SqlClient;

namespace Document_Extractor.Data;

public class AppDapperContext
{
    private readonly IConfiguration _configuration;
    private readonly string _connectionString;

    public AppDapperContext(IConfiguration configuration)
    {
        _configuration = configuration;
        _connectionString = _configuration.GetConnectionString("ApiConnectionString");
    }

    public IDbConnection CreateConnection()
        => new SqlConnection(_connectionString);
}