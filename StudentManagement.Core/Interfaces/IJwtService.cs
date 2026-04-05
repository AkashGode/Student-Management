namespace StudentManagement.Core.Interfaces;

public interface IJwtService
{
    string GenerateToken(string username, string role);
}
