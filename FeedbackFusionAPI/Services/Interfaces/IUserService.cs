using FeedbackFusionAPI.Model;

namespace FeedbackFusionAPI.Services.Interfaces
{
    public interface IUserService
    {

        Task<AuthenticateResponse?> Authenticate(LoginRequest model);
        Task<IEnumerable<User>> GetAll();
        Task<User?> GetById(string id);
        Task<AuthenticateResponse> AddUser(RegisterRequest model);
        Task<bool> DeleteUserAsync(string userId, string idToDelete);


    }
}