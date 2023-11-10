using Karpinski_XY.Models;
using Karpinski_XY_Server.Data.Models.Base;
using Karpinski_XY_Server.Data.Models.Configuration;
using Karpinski_XY_Server.Dtos.Identity;
using Karpinski_XY_Server.Services.Contracts;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Karpinski_XY_Server.Services
{
    public class IdentityService : IIdentityService
    {
        private readonly UserManager<User> userManager;
        private readonly AppSettings appSettings;
        private readonly ILogger<IdentityService> logger;

        public IdentityService(UserManager<User> userManager,
            IOptions<AppSettings> appSettings,
            ILogger<IdentityService> logger)
        {
            this.userManager = userManager;
            this.appSettings = appSettings.Value;
            this.logger = logger;
        }

        public async Task<Result<LoginResponseModel>> LoginAsync(LoginRequestModel model)
        {
            var user = await userManager.FindByNameAsync(model.Username);

            if (user == null)
            {
                logger.LogWarning("Login failed for user {Username}: User not found.", model.Username);
                return Result<LoginResponseModel>.Fail("No such user or wrong password");
            }

            var passwordValid = await userManager.CheckPasswordAsync(user, model.Password);

            if (!passwordValid)
            {
                logger.LogWarning("Login failed for user {Username}: Invalid password.", model.Username);
                return Result<LoginResponseModel>.Fail("Invalid password");
            }

            var token = GenerateJWTToken(user, appSettings.Secret);

            var responseModel = new LoginResponseModel
            {
                Token = token,
                Username = user.UserName,
                Id = user.Id.ToString()
            };

            logger.LogInformation("Login successful for user {Username}.", model.Username);

            return Result<LoginResponseModel>.Success(responseModel);
        }

        public async Task<Result<IdentityResult>> RegisterAsync(RegisterRequestModel model)
        {
            var user = new User
            {
                Email = model.Email,
                UserName = model.Username
            };
            var identityResult = await userManager.CreateAsync(user, model.Password);

            if (identityResult.Succeeded)
            {
                logger.LogInformation("User {Username} registered successfully.", model.Username);
                return Result<IdentityResult>.Success(identityResult);
            }
            else
            {
                logger.LogWarning("Registration failed for user {Username}. Errors: {Errors}", model.Username, string.Join(", ", identityResult.Errors));
                return Result<IdentityResult>.Fail(string.Join(", ", identityResult.Errors.Select(e => e.Description)));
            }
        }


        private string GenerateJWTToken(User user, string secret)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id),
                    new Claim(ClaimTypes.Name, user.UserName)
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var encryptedToken = tokenHandler.WriteToken(token);

            return encryptedToken;
        }
    }
}
