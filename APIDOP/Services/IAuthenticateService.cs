using APIDOP.Models.DTO;
using APIDOP.Models.DB;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using APIDOP.Enums;


namespace APIDOP.Services
{
    public interface IAuthenticateService
    {
        Task<bool> AddUser(RegisterModel model);
        Task<string> LoginUser(LoginModel model);
    }
    public class AuthenticateService: IAuthenticateService
    {
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _configuration;

        public AuthenticateService(UserManager<User> usersService, IConfiguration configuration)
        {
            _userManager = usersService;
            _configuration = configuration;
        }

        public string GetToken(ClaimsIdentity identity)
        {
            var now = DateTime.UtcNow;
            // создаем JWT-токен
            var jwt = new JwtSecurityToken(
                issuer: JwtConfigurations.Issuer,
                audience: JwtConfigurations.Audience,
                notBefore: now,
                claims: identity.Claims,
                expires: now.Add(new TimeSpan(JwtConfigurations.Lifetime / 60, 0, 0)),
                signingCredentials: new SigningCredentials(JwtConfigurations.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }

        public async Task<bool> AddUser(RegisterModel model)
        {
            User user = new()
            {
                Email = model.Email,
                Name = model.Username,
                UserName = model.Username
            };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
                return false;

            return true;
        }

        public async Task<string> LoginUser(LoginModel model)
        {
            var user = await _userManager.FindByNameAsync(model.Username);
            if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
            {
                var authClaims = new List<Claim>
                {
                    new Claim(ClaimsIdentity.DefaultNameClaimType,user.UserName),
                    new Claim(ClaimsIdentity.DefaultRoleClaimType, ApplicationRoleNames.User),
                };

                var claimsIdentity = new ClaimsIdentity(authClaims, "Token", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);

                return GetToken(claimsIdentity);
            }
            else
            {
                return null;
            }
        }


    }
}
