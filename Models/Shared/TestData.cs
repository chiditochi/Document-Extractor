
namespace Document_Extractor.Models.Shared;

public class TestData
{
    public string[]? UserTypes { get; set; }
    public string[]? Roles { get; set; }
    public TestUser[]? Users { get; set; }
    public TestTeam[]? Teams { get; set; }
    public TestAppConstant[]? AppConstant { get; set; }

}

public class TestUser
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Password { get; set; }
    public string? Gender { get; set; }
    public string? UserType { get; set; }
    public string? Email { get; set; }
    public string? Role { get; set; }
}
public class TestTeam
{
    public string? Code { get; set; }
    public string? Description { get; set; }
}

public class TestAppConstant
{
    public string? Label { get; set; }
    public string[]? LabelValues { get; set; }
}