namespace OnlineAcadimy.Services
{
    public interface IJwtService
    {
        string GenerateToken(int userId,string fullName,string email,string role);
    }
}