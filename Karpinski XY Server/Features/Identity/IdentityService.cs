using Karpinski_XY.Features.Identity.Models;
using Karpinski_XY.Models;
using Karpinski_XY_Server.Features.Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Karpinski_XY.Features.Identity
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

        public async Task<LoginResponseModel> LoginAsync(LoginRequestModel model)
        {
            var responseModel = new LoginResponseModel();
            var user = await userManager.FindByNameAsync(model.Username);

            if (user == null)
            {
                logger.LogWarning("Login failed for user {Username}: User not found.", model.Username);

                responseModel.IdentityResult = IdentityResult.Failed(new IdentityError
                {
                    Description = "No such user or wrong password"
                });

                return responseModel;
            }

            var passwordValid = await userManager.CheckPasswordAsync(user, model.Password);

            if (!passwordValid)
            {
                logger.LogWarning("Login failed for user {Username}: Invalid password.", model.Username);

                responseModel.IdentityResult = IdentityResult.Failed(new IdentityError
                {
                    Description = "Invalid password"
                });

                return responseModel;
            }

            var token = this.GenerateJWTToken(user, appSettings.Secret);

            responseModel.Token = token;
            responseModel.Username = user.UserName;
            responseModel.Id = user.Id.ToString();
            responseModel.IdentityResult = IdentityResult.Success;

            logger.LogInformation("Login successful for user {Username}.", model.Username);

            return responseModel;
        }

        public async Task<IdentityResult> RegisterAsync(RegisterRequestModel model)
        {
            var user = new User
            {
                Email = model.Email,
                UserName = model.Username
            };
            var result = await userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                logger.LogInformation("User {Username} registered successfully.", model.Username);
            }
            else
            {
                logger.LogWarning("Registration failed for user {Username}. Errors: {Errors}", model.Username, string.Join(", ", result.Errors));
            }

            return result;
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
