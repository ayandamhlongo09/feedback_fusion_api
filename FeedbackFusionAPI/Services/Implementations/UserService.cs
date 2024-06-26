using Microsoft.EntityFrameworkCore;
using FeedbackFusionAPI.Context;
using FeedbackFusionAPI.Model;
using FeedbackFusionAPI.Services.Interfaces;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FeedbackFusionAPI.Helpers;

namespace FeedbackFusionAPI.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly AppSettings _appSettings;
        private MyDbContext _dbContext;

        public UserService(IOptions<AppSettings> appSettings, MyDbContext dbContext)
        {
            _appSettings = appSettings.Value;
            _dbContext = dbContext;
        }

        public async Task<AuthenticateResponse?> Authenticate(LoginRequest model)
        {
            var user = await _dbContext.Users.SingleOrDefaultAsync(x => x.Email == model.Email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(model.Password, user.Password))
                return null;

            var token = await GenerateJwtToken(user);

            return new AuthenticateResponse(user, token);
        }


        public async Task<IEnumerable<User>> GetAll()
        {
            //Role 1 is superuser. They should remain invisible. 
            return await _dbContext.Users.Where(x => x.UserId != "" && x.Role == 2).ToListAsync();
        }


        public async Task<User?> GetById(string id)
        {
            return await _dbContext.Users.FirstOrDefaultAsync(x => x.UserId == id);
        }


        public async Task<AuthenticateResponse> AddUser(RegisterRequest model)
        {
            var existingUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
            if (existingUser != null)
                throw new InvalidOperationException("User with this email already exists.");

            var passwordHash = StringHelper.HashPassword(model.Password);
            var userId = StringHelper.GenerateRandomString();

            var newUser = new User
            {
                UserId = userId,
                Role = 3,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                Password = passwordHash
            };

            await _dbContext.Users.AddAsync(newUser);
            await _dbContext.SaveChangesAsync();

            var token = await GenerateJwtToken(newUser);

            return new AuthenticateResponse(newUser, token);
        }

        public async Task<bool> DeleteUserAsync(string userId, string idToDelete)
        {

            var deleter = await _dbContext.Users.FindAsync(userId);
            if (deleter is not { Role: 1 } && deleter is not { Role: 2 })
            {
                throw new UnauthorizedAccessException("Only admin users can delete other users.");
            }

            var user = await _dbContext.Users.FindAsync(userId);
            if (user == null)
            {
                return false;
            }

            _dbContext.Users.Remove(user);
            await _dbContext.SaveChangesAsync();

            return true;
        }


        // helper methods
        private async Task<string> GenerateJwtToken(User user)
        {

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = await Task.Run(() =>
            {

                var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new[] { new Claim("id", user.UserId.ToString()) }),
                    Expires = DateTime.UtcNow.AddDays(7),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };
                return tokenHandler.CreateToken(tokenDescriptor);
            });

            return tokenHandler.WriteToken(token);
        }
    }
}

